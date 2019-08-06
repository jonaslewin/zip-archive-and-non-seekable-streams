Bug description
===============================

It appears as if a bug similar to the one reported on .NET Core by petertiedemann (https://github.com/dotnet/corefx/issues/11497) and discussed by Stephen Cleary in his blog posts at https://blog.stephencleary.com/2016/11/streaming-zip-on-aspnet-core.html has re-appeared in .NET Framework 4.7.2 and 4.8.

ZipArchive CreateMode tries to read Position on non-seekable Stream. However, even in CreateMode, the code attempts to read the Position from the stream it is writing to, which is typically not supported on a non-seekable stream.

Apparently there was an old bug reported on this on the .NET framework at https://connect.microsoft.com/VisualStudio/feedback/details/816411/ziparchive-shouldnt-read-the-position-of-non-seekable-streams#commentContainer but the link doesn't work anymore.

The bug on .NET Core at https://github.com/dotnet/corefx/issues/11497, appears to have been fixed in https://github.com/dotnet/corefx/pull/12682.

However, I am using .NET Framework, 4.7 and 4.8 and while everything seems to work in 4.7, after upgrading to 4.7.2 or 4.8 it seems like this bug is re-appeared requiring the workaround suggested by svick at https://github.com/dotnet/corefx/issues/11497#issuecomment-245253608.


Steps to reproduce
===============================

Running the project above in .NET Framework 4.7 works fine.

Running it in .NET Framework 4.7.2 or 4.8 results in the following exeption.

System.NotSupportedException: Specified method is not supported.
   at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpResponseStream.get_Position()
   at System.IO.Compression.CheckSumAndSizeWriteStream.Dispose(Boolean disposing)
   at System.IO.Stream.Close()
   at System.IO.Compression.ZipArchiveEntry.DirectToArchiveWriterStream.Dispose(Boolean disposing)
   at System.IO.Stream.Close()
   at System.IO.Compression.ZipArchive.WriteFile()
   at System.IO.Compression.ZipArchive.Dispose(Boolean disposing)
   at System.IO.Compression.ZipArchive.Dispose()
   at ZipArchiveAndNonSeekableStreams.Controllers.ValuesController.<>c.<<GetFileCallbackResult>b__1_0>d.MoveNext() in C:\Dev\Source\Repos\ZipArchiveAndNonSeekableStreams\ZipArchiveAndNonSeekableStreams\Controllers\ValuesController.cs:line 47

This can be worked around by wrapping the output stream in WriteOnlyStreamWrapper which keeps track of the position in the output stream.
