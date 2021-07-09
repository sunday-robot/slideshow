using System;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static SlideShow.Utils;

namespace SlideShow
{
    class ZipArchiveImageSourceSeries : IImageFileStreamSeries
    {
        readonly string _ZipfilePath;
        readonly ZipArchive _ZipArchive;
        readonly ReadOnlyCollection<ZipArchiveEntry> _ZipArchiveEntries;
        int _NextIndex;

        public ZipArchiveImageSourceSeries(string filePath)
        {
            _ZipfilePath = filePath;
            _ZipArchive = ZipFile.OpenRead(filePath);
            _ZipArchiveEntries = _ZipArchive.Entries;
            _NextIndex = 0;
        }

        //public ImageSource Current { get; private set; }

        public ImageSource GetNext()
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
    }
}
