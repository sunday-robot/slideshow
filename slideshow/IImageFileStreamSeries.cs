using System.Windows.Media;

namespace SlideShow
{
    interface IImageFileStreamSeries
    {
        ImageSource Current { get; }

        /// <summary>
        /// ポインタを次の要素に移す
        /// </summary>
        /// <returns>ポインタを動かせたかどうか</returns>
        bool MoveNext();

        /// <summary>
        /// ポインタをひとつ前の要素に移す
        /// </summary>
        /// <returns>ポインタを動かせたかどうか</returns>
        bool MovePrevious();

        /// <summary>
        /// ポインタを先頭の要素に移す
        /// </summary>
        /// <returns>ポインタを動かせたかどうか</returns>
        bool MoveFirst();

        /// <summary>
        /// ポインタを末尾の要素に移す
        /// </summary>
        /// <returns>ポインタを動かせたかどうか</returns>
        bool MoveLast();
    }
}
