using System.IO;

namespace SlideShow
{
    static partial class Utils
    {
        public static bool IsImageFile(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToUpper();
            return ext switch
            {
                ".BMP" or ".JPG" or ".PNG" => true,
                _ => false,//Log($"{filePath}は画像ファイルではないので無視します。");
            };
        }
    }
}
