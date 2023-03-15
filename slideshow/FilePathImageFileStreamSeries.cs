using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using static SlideShow.Utils;

namespace SlideShow
{
    class FilePathImageFileStreamSeries : IImageFileStreamSeries
    {
        readonly List<string> _FilePathList;
        readonly Func<bool> _IsLoopFunc;
        int _NextIndex;
        IImageFileStreamSeries _ImageSourceSeries;

        public override string ToString()
        {
            var sb = "FilePathImageFileStreamSeries(";
            for (var i = 0; i < _FilePathList.Count; i++)
            {
                if (i == _NextIndex)
                    sb += ",=>" + _FilePathList[i];
                else
                    sb += "," + _FilePathList[i];
            }
            return sb + ")";
        }

        public FilePathImageFileStreamSeries(string[] filePathList, Func<bool> isLoopFunc) : this(new List<string>(filePathList), isLoopFunc)
        {
        }

        public FilePathImageFileStreamSeries(IEnumerable<string> filePathList, Func<bool> isLoopFunc)
        {
            _FilePathList = new List<string>(filePathList);
            _IsLoopFunc = isLoopFunc;
            _NextIndex = 0;
            _ImageSourceSeries = null;
            //MoveNext();
        }

        //public ImageSource Current {
        //    get {
        //        if (_ImageSourceSeries != null)
        //            return _ImageSourceSeries.Current;
        //        return _ImageSource;
        //    }
        //}

        public ImageSource GetNext()
        {
            ImageSource imageSource;
        l:
            if (_ImageSourceSeries != null)
            {
                imageSource = _ImageSourceSeries.GetNext();
                if (imageSource != null)
                    return imageSource;
                _ImageSourceSeries = null;
            }
        l_first:
            if (_NextIndex == _FilePathList.Count)
            {
                if (_IsLoopFunc())
                {
                    Log("初めに戻ります。");
                    _NextIndex = 0;
                }
                else
                    return null;
            }
            var filePath = _FilePathList[_NextIndex++];
            _ImageSourceSeries = CreateImageFileStreamSeries(filePath);
            if (_ImageSourceSeries != null)
                goto l;

            imageSource = CreateImageSource(filePath);
            if (imageSource != null)
                return imageSource;
            goto l_first;
        }

        //public bool MovePrevious()
        //{
        //    if (_ImageSourceSeries != null)
        //    {
        //        if (_ImageSourceSeries.MovePrevious())
        //            return true;
        //        _ImageSourceSeries = null;
        //    }
        //l_first:
        //    if (_CurrentIndex == 0)
        //        return false;
        //    _CurrentIndex--;
        //    var filePath = _FilePathList[_CurrentIndex];
        //    _ImageSourceSeries = CreateImageFileStreamSeries(filePath);
        //    if (_ImageSourceSeries != null)
        //    {
        //        if (_ImageSourceSeries.MoveLast())
        //            return true;
        //        _ImageSourceSeries = null;
        //    }
        //    _ImageSource = CreateImageSource(filePath);
        //    if (_ImageSource != null)
        //    {
        //        return true;
        //    }
        //    goto l_first;
        //}

        //public bool MoveFirst()
        //{
        //    _ImageSourceSeries = null;
        //    _CurrentIndex = -1;
        //    return MoveNext();
        //}

        //public bool MoveLast()
        //{
        //    _ImageSourceSeries = null;
        //    _CurrentIndex = _FilePathList.Count;
        //    return MovePrevious();
        //}

        static ImageSource CreateImageSource(string filePath)
        {
            if (!IsImageFile(filePath))
            {
                Log($"{filePath}は画像ファイルではないので無視します。");
                return null;
            }

            Log($"{filePath}を表示します。");
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return CreateImageSourceFromStream(stream);
            }
            catch (IOException e)
            {
                if (e is FileNotFoundException)
                {
                    Log("ファイルが存在しません。無視して続行します。");
                }
                else
                {
                    Log("ファイルがオープンできません。無視して続行します。");
                    Log(e.ToString());
                }
                return null;
            }
        }

        /// <summary>
        /// 指定されたパス名から、ImageFileStreamSeriesを生成する。
        /// パスがディレクトリ、あるいはZIPファイルではない場合はnullを返す。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IImageFileStreamSeries CreateImageFileStreamSeries(string filePath)
        {
            if (Directory.Exists(filePath))
            {
                Log($"ディレクトリ{filePath}をチェックします。");
                var entries = Directory.GetFileSystemEntries(filePath);
                return new FilePathImageFileStreamSeries(entries, () => false);
            }

            if (Path.GetExtension(filePath).ToUpper() == ".ZIP")
            {
                Log($"ZIPファイル{filePath}をチェックします。");
                var s = new ZipArchiveImageSourceSeries(filePath);
                return s;
            }
            //Log($"{filePath}は、ディレクトリでも、ZIPファイルでもありません。");
            return null;
        }
    }
}
