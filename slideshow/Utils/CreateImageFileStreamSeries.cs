using System;
using System.IO;
using System.IO.Compression;

namespace SlideShow
{
    static partial class Utils
    {
        public static IImageFileStreamSeries CreateImageFileStreamSeries(string filePath)
        {
            if (Directory.Exists(filePath))
            {
                var entries = Directory.GetFileSystemEntries(filePath);
                Log("ディレクトリでした");
                return new FilePathImageFileStreamSeries(entries);
            }

            if (Path.GetExtension(filePath).ToUpper() == ".ZIP")
            {
                var zipArchive = ZipFile.OpenRead(filePath);
                var s = new ZipArchiveImageSourceSeries(zipArchive);
                Log("ZIPファイルでした");
                return s;
            }
            Log("ディレクトリでもZIPファイルでもありませんでした");
            return null;
        }
    }
}
