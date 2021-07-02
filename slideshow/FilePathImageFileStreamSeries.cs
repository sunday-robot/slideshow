using System.Collections.Generic;
using System.Windows.Media;
using static SlideShow.Utils;

namespace SlideShow
{
    class FilePathImageFileStreamSeries : IImageFileStreamSeries
    {
        List<string> _FilePathList;
        int _CurrentIndex;
        IImageFileStreamSeries _ImageSourceSeries;
        ImageSource _ImageSource;

        public FilePathImageFileStreamSeries(string[] filePathList) : this(new List<string>(filePathList))
        {
        }

        public FilePathImageFileStreamSeries(IEnumerable<string> filePathList)
        {
            this._FilePathList = new List<string>(filePathList);
            _CurrentIndex = -1;
            _ImageSourceSeries = null;
            MoveNext();
        }

        public ImageSource Current {
            get {
                if (_ImageSourceSeries != null)
                    return _ImageSourceSeries.Current;
                return _ImageSource;
            }
        }

        public bool MoveNext()
        {
            if (_ImageSourceSeries != null)
            {
                if (_ImageSourceSeries.MoveNext())
                    return true;
                _ImageSourceSeries = null;
            }
        l_first:
            if (_CurrentIndex == _FilePathList.Count - 1)
                return false;
            _CurrentIndex++;
            var filePath = _FilePathList[_CurrentIndex];

            Log($"{filePath}をチェックします。");

            _ImageSourceSeries = CreateImageFileStreamSeries(filePath);
            if (_ImageSourceSeries != null)
            {
                //Log("シリーズでした");
                if (_ImageSourceSeries.MoveFirst())
                    return true;
                Log("シリーズでしたが、内容がありませんでした。");
                _ImageSourceSeries = null;
            }
            _ImageSource = CreateImageSource(filePath);
            if (_ImageSource != null)
            {
                Log("画像ファイルでした");

                return true;
            }
            goto l_first;
        }

        public bool MovePrevious()
        {
            if (_ImageSourceSeries != null)
            {
                if (_ImageSourceSeries.MovePrevious())
                    return true;
                _ImageSourceSeries = null;
            }
        l_first:
            if (_CurrentIndex == 0)
                return false;
            _CurrentIndex--;
            var filePath = _FilePathList[_CurrentIndex];
            _ImageSourceSeries = CreateImageFileStreamSeries(filePath);
            if (_ImageSourceSeries != null)
            {
                if (_ImageSourceSeries.MoveLast())
                    return true;
                _ImageSourceSeries = null;
            }
            _ImageSource = CreateImageSource(filePath);
            if (_ImageSource != null)
            {
                return true;
            }
            goto l_first;
        }

        public bool MoveFirst()
        {
            _ImageSourceSeries = null;
            _CurrentIndex = -1;
            return MoveNext();
        }

        public bool MoveLast()
        {
            _ImageSourceSeries = null;
            _CurrentIndex = _FilePathList.Count;
            return MovePrevious();
        }
    }
}
