using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SlideShow
{
    static partial class Utils
    {
        public static ImageSource CreateImageSource(string filePath)
        {
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return CreateImageSource(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ImageSource CreateImageSource(Stream stream)
        {
            var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            //if (decoder == null)
            //{
            //    Console.WriteLine("ImageSourceCreator#Create()" + stream + " failed.");
            //}
            return decoder.Frames[0];
        }
    }
}
