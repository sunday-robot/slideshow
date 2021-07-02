using System;
using System.Diagnostics;
using System.IO;

namespace SlideShow
{
    static partial class Utils
    {
        public static void Log(string msg,
                          [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
                          [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
                          [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            var dt = DateTime.Now;
            Debug.WriteLine(string.Format("{0:00}:{1:00}:{2:00}.{3:000}, {4}({5}), {6}, {7}", dt.Hour, dt.Minute, dt.Second, dt.Millisecond,
                Path.GetFileName(filePath), lineNumber, memberName, msg));
        }
    }
}
