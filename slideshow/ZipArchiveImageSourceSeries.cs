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
        readonly ReadOnlyCollection<ZipArchiveEntry> _ZipArchiveEntries;
        int _CurrentIndex;

        public ZipArchiveImageSourceSeries(ZipArchive zipArchive)
        {
            _ZipArchiveEntries = zipArchive.Entries;
            _CurrentIndex = -1;
            MoveNext();
        }

        public ImageSource Current { get; private set; }

        public bool MoveNext()
        {
            if (_CurrentIndex == _ZipArchiveEntries.Count - 1)
            {
                return false;
            }
            _CurrentIndex++;
            var entry = _ZipArchiveEntries[_CurrentIndex];

            Log(entry.FullName);

            var stream = entry.Open();
            var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            Current = decoder.Frames[0];
            return true;
        }

        public bool MovePrevious()
        {
            if (_CurrentIndex == 0)
            {
                return false;
            }
            _CurrentIndex--;
            updateCurrent();
            return true;
        }

        public bool MoveFirst()
        {
            _CurrentIndex = -1;
            return MoveNext();
        }

        public bool MoveLast()
        {
            _CurrentIndex = _ZipArchiveEntries.Count;
            return MovePrevious();
        }

        bool updateCurrent()
        {
            var entry = _ZipArchiveEntries[_CurrentIndex];
            var stream = entry.Open();
            BitmapDecoder decoder;
            try
            {
                decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            }
            catch (Exception e)
            {
                Log(e.ToString());
                return false;
            }
            Current = decoder.Frames[0];
            return true;
        }
    }
}
