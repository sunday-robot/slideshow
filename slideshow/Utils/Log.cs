using System.Diagnostics;

namespace SlideShow
{
    static partial class Utils
    {
        public static void Log(string msg,
                          [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
                          [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
                          [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            Debug.WriteLine($"{filePath}({lineNumber}) {memberName} : {msg}");
        }
    }
}
