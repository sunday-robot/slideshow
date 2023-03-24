using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Linq;
using System.Windows.Media;
using static SlideShow.Utils;

namespace SlideShow
{
    class ZipArchiveImageSourceSeries : IImageFileStreamSeries
    {
        readonly string _ZipfilePath;
        readonly ZipArchive _ZipArchive;
        readonly ReadOnlyCollection<ZipArchiveEntry> _ZipArchiveEntries;
        int _NextIndex;

        static readonly char[] _PathSeparatorChars = { '/', '\\' };

        public ZipArchiveImageSourceSeries(string filePath)
        {
            _ZipfilePath = filePath;
            _ZipArchive = ZipFile.OpenRead(filePath);
            var list = _ZipArchive.Entries.ToList();
            list.Sort((a, b) => CompareZipEntryFullName(a.FullName, b.FullName));
            _ZipArchiveEntries = list.AsReadOnly();
            _NextIndex = 0;
        }

        //public ImageSource Current { get; private set; }

        public ImageSource? GetNext()
        {
            while (_NextIndex < _ZipArchiveEntries.Count)
            {
                var entry = _ZipArchiveEntries[_NextIndex++];
                if (IsImageFile(entry.FullName))
                {
                    Log($"{_ZipfilePath}/{entry.FullName}を表示します。");
                    using var stream = entry.Open();
                    return CreateImageSourceFromStream(stream);
                }
            }
            return null;
        }

        public ImageSource? GetPrevious()
        {
            while (_NextIndex >= 0)
            {
                var entry = _ZipArchiveEntries[_NextIndex--];
                if (IsImageFile(entry.FullName))
                {
                    Log($"{_ZipfilePath}/{entry.FullName}を表示します。");
                    using var stream = entry.Open();
                    return CreateImageSourceFromStream(stream);
                }
            }
            return null;
        }

        //public bool MovePrevious()
        //{
        //    if (_CurrentIndex == 0)
        //    {
        //        return false;
        //    }
        //    _CurrentIndex--;
        //    updateCurrent();
        //    return true;
        //}

        //public bool MoveFirst()
        //{
        //    _CurrentIndex = -1;
        //    return MoveNext();
        //}

        //public bool MoveLast()
        //{
        //    _CurrentIndex = _ZipArchiveEntries.Count;
        //    return MovePrevious();
        //}

        //bool updateCurrent()
        //{
        //    var entry = _ZipArchiveEntries[_CurrentIndex];
        //    var stream = entry.Open();
        //    BitmapDecoder decoder;
        //    try
        //    {
        //        decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
        //    }
        //    catch (Exception e)
        //    {
        //        Log(e.ToString());
        //        return false;
        //    }
        //    Current = decoder.Frames[0];
        //    return true;
        //}
        static int CompareZipEntryFullName(string a, string b)
        {
            var ad = TakeDirectoryName(a);
            var bd = TakeDirectoryName(b);
            var r = CompareDirectoryName(ad.DirectoryName, bd.DirectoryName);
            if (r != 0)
                return r;
            return CompareZipEntryFullName(ad.Remaining, bd.Remaining);
        }

        static (string DirectoryName, string Remaining) TakeDirectoryName(string s)
        {
            var index = s.IndexOfAny(_PathSeparatorChars);
            if (index < 0)
                return ("", s);
            return (s[0..index], s[(index + 1)..^0]);
        }

        /// <summary>
        /// ディレクトリ名を比較する。
        /// 単純な文字列比較と異なり、「通常の文字列 < 空文字列」としている。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static int CompareDirectoryName(string a, string b)
        {
            if (a.Length == 0)
            {
                if (b.Length == 0)
                    return 0;
                return 1;
            }
            if (b.Length == 0)
                return -1;
            return a.CompareTo(b);
        }
    }
}
