using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace SlideShow
{
    static partial class Utils
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]

        public static extern bool DeleteObject([In] IntPtr hObject);

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
#if false
            // 以下のページを参考にした。bitmapFrameをそのまま返すと、画面に表示されない(デコード処理をやらずに終わってしまっていると思われる)
            var bitmapFrame = BitmapFrame.Create(stream);
            var wbmp = new WriteableBitmap(bitmapFrame);
            wbmp.Freeze();
#if false
            // 以下のように画像ファイルとして保存すると、画面に表示される。ファイル保存するためにはデコード処理をきちんと行うからと思われる。
            using var ws = new FileStream("a.png", FileMode.Create);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(bitmapFrame);
            encoder.Save(ws);
#endif
            return wbmp;
#else
#if false
#if true
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            var data = ms.ToArray();
            var mat = Mat.FromImageData(data);
#else
            var mat = Mat.FromStream(stream, OpenCvSharp.ImreadModes.Color);
#endif
            var bs = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(mat);
            return bs;
#else
            var bmp = Bitmap.FromStream(stream);
            var hbmp = ((Bitmap)bmp).GetHbitmap();
            var bs = Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(hbmp);
            return bs;
#endif
#endif
#endif
        }
    }
}
