using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace ZipArchiveAndNonSeekableStreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return GetFileCallbackResult();
        }

        public IActionResult GetFileCallbackResult()
        {
            return new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"), async (outputStream, _) =>
            {
                // Switching to using the WriteOnlyStreamWrapper seem to fix the problem in .NET Framework 4.8, but is not required in 4.7 for things to work.
                //using (var zipArchive = new ZipArchive(new WriteOnlyStreamWrapper(outputStream), ZipArchiveMode.Create))
                using (var zipArchive = new ZipArchive(outputStream, ZipArchiveMode.Create))
                {
                    using (var memoryStream = new MemoryStream())
                    using (var writeStream = new StreamWriter(memoryStream))
                    {
                        // Write to stream
                        writeStream.Write("Some text to go in the file");
                        writeStream.Flush();
                        memoryStream.Position = 0;

                        var zipEntry = zipArchive.CreateEntry("ZippedFile.txt");
                        using (var zipStream = zipEntry.Open())
                        using (memoryStream)
                        {
                            await memoryStream.CopyToAsync(zipStream);
                        }
                    }
                }
            })
            {
                FileDownloadName = "MyZipfile.zip"
            };
        }
    }
}
