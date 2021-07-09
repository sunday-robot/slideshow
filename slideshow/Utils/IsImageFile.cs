using System.IO;

namespace SlideShow
{
    static partial class Utils
    {
        public static bool IsImageFile(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToUpper();
            switch (ext)
            {
                case ".BMP":
                case ".JPG":
                case ".PNG":
                    return true;
                default:
                    //Log($"{filePath}は画像ファイルではないので無視します。");
                    return false;
            }
        }
    }
}
