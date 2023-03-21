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
            var bmp = Bitmap.FromStream(stream);
            var hbmp = ((Bitmap)bmp).GetHbitmap();
            var bs = Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(hbmp);
            return bs;
        }
    }
}
