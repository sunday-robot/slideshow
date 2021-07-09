using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SlideShow
{
    static partial class Utils
    {
        public static ImageSource CreateImageSourceFromStream(Stream stream)
        {
#if false
// 以下のやり方では、画面に画像が表示されない。デコード処理をやらずに終わってしまっていると思われる。
            var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            var bitmapFrame = decoder.Frames[0];
            Debug.WriteLine(bitmapFrame.IsDownloading);
            Debug.WriteLine(bitmapFrame.IsFrozen);
#if false
// 以下のように画像ファイルとして保存すると、画面に表示される。ファイル保存するためにはデコード処理をきちんと行うからと思われる。
            using var ws = new FileStream("a.png", FileMode.Create);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(imageSource);
            encoder.Save(ws);
#endif
            return bitmapFrame;
#else
            // 以下のページを参考にした。bitmapFrameをそのまま返すと、画面に表示されない(デコード処理をやらずに終わってしまっていると思われる)
            var bitmapFrame = BitmapFrame.Create(stream);
            var wbmp = new WriteableBitmap(bitmapFrame);
            wbmp.Freeze();
#if true
            // 以下のように画像ファイルとして保存すると、画面に表示される。ファイル保存するためにはデコード処理をきちんと行うからと思われる。
            using var ws = new FileStream("a.png", FileMode.Create);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(bitmapFrame);
            encoder.Save(ws);
#endif
            return wbmp;
#endif
        }
    }
}
