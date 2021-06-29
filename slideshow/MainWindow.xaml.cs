using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SlideShow
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        FilePathImageFileStreamSeries _ImageFileList;

        /// <summary>
        /// 繰り返すかどうか
        /// </summary>
        bool _LoopOption = false;

        /// <summary>
        /// 画像表示時間間隔[ms]
        /// </summary>
        int _IntervalOption = 2000;

        public MainWindow()
        {
            _ImageFileList = new FilePathImageFileStreamSeries(Environment.GetCommandLineArgs().Skip(1));
            if (_ImageFileList.Current == null)
            {
                Close();
            }

            _LoopOption = true;

            _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, _IntervalOption);
            _DispatcherTimer.Tick += (object sender, EventArgs e) => _ShowNextImage();
            InitializeComponent();
        }

        void _ShowNextImage()
        {
            if (!_ImageFileList.MoveNext())
            {
                if (_LoopOption)
                {
                    _ImageFileList.MoveFirst();
                }
                else
                {
                    Close();
                    return;
                }

            }
            image.Source = _ImageFileList.Current;
        }

        private void _ShowPreviousImage()
        {
            if (!_ImageFileList.MovePrevious())
            {
                _ImageFileList.MoveFirst();
            }
            image.Source = _ImageFileList.Current;
        }

        private void _Forward()
        {
            _ShowNextImage();
            if (_DispatcherTimer.IsEnabled)
            {
                _DispatcherTimer.Stop();
                _DispatcherTimer.Start();
            }
            //            Console.WriteLine("function FORWARD is not implemented");
        }

        private void _Backward()
        {
            _ShowPreviousImage();
            if (_DispatcherTimer.IsEnabled)
            {
                _DispatcherTimer.Stop();
                _DispatcherTimer.Start();
            }
        }

        private void _PauseOrResume()
        {
            if (_DispatcherTimer.IsEnabled)
                _DispatcherTimer.Stop();
            else
                _DispatcherTimer.Start();
        }

        private void _QuitApplication()
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    _Forward();
                    break;
                case Key.Left:
                    _Backward();
                    break;
                case Key.Space:
                    _PauseOrResume();
                    break;
                case Key.Escape:
                    _QuitApplication();
                    break;
                case Key.F:
                    _ToggleFullScreenMode();
                    break;
                default:
                    break;
            }
        }

        private bool _IsFullScreenMode = false;

        private void _ToggleFullScreenMode()
        {
            _IsFullScreenMode = !_IsFullScreenMode;
            if (_IsFullScreenMode)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.Topmost = true;
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                this.Topmost = false;
            }
        }

        DispatcherTimer _DispatcherTimer = new DispatcherTimer();

        void Window_Initialized(object sender, EventArgs e)
        {
            image.Source = _ImageFileList.Current;

            //            _ShowNextImage();
            _DispatcherTimer.Start();
        }
    }

    interface ImageFileStreamSeries
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

    class FilePathImageFileStreamSeries : ImageFileStreamSeries
    {
        List<string> _FilePathList;
        int _CurrentIndex;
        ImageFileStreamSeries _ImageSourceSeries;
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

        public ImageSource Current
        {
            get
            {
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

            Console.WriteLine(filePath);

            _ImageSourceSeries = ImageSourceSeriesCreator.Create(filePath);
            if (_ImageSourceSeries != null)
            {
                Console.WriteLine("シリーズでした");

                if (_ImageSourceSeries.MoveFirst())
                    return true;
                Console.WriteLine("シリーズでしたが、内容がありませんでした。");
                _ImageSourceSeries = null;
            }
            _ImageSource = ImageSourceCreator.Create(filePath);
            if (_ImageSource != null)
            {
                Console.WriteLine("画像ファイルでした");

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
            _ImageSourceSeries = ImageSourceSeriesCreator.Create(filePath);
            if (_ImageSourceSeries != null)
            {
                if (_ImageSourceSeries.MoveLast())
                    return true;
                _ImageSourceSeries = null;
            }
            _ImageSource = ImageSourceCreator.Create(filePath);
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

    class ImageSourceCreator
    {
        public static ImageSource Create(string filePath)
        {
            try
            {
                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return Create(stream);
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
        }

        public static ImageSource Create(Stream stream)
        {
            var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            //if (decoder == null)
            //{
            //    Console.WriteLine("ImageSourceCreator#Create()" + stream + " failed.");
            //}
            return decoder.Frames[0];
        }
    }

    class ImageSourceSeriesCreator
    {
        public static ImageFileStreamSeries Create(string filePath)
        {
            if (Directory.Exists(filePath))
            {
                var entries = Directory.GetFileSystemEntries(filePath);
                Console.WriteLine("ディレクトリでした");
                return new FilePathImageFileStreamSeries(entries);
            }
            try
            {
                var zipArchive = ZipFile.OpenRead(filePath);
                var s = new ZipArchiveImageSourceSeries(zipArchive);
                Console.WriteLine("ZIPファイルでした");
                return s;
            }
            catch (FileNotFoundException e)
            {
                // ZIPファイルではなかったというだけ。何もしない。
            }
            catch (InvalidDataException ee)
            {
                // ZIPファイルではなかったというだけ。何もしない。
            }
            catch (NotSupportedException ee)
            {
                // ZIPファイルではなかったというだけ。何もしない。
            }
            Console.WriteLine("ディレクトリでもZIPファイルでもありませんでした");
            return null;
        }
    }

    class ZipArchiveImageSourceSeries : ImageFileStreamSeries
    {
        ReadOnlyCollection<ZipArchiveEntry> _ZipArchiveEntries;
        int _CurrentIndex;
        ImageSource _ImageSource;

        public ZipArchiveImageSourceSeries(ZipArchive zipArchive)
        {
            _ZipArchiveEntries = zipArchive.Entries;
            _CurrentIndex = -1;
            MoveNext();
        }

        public ImageSource Current
        {
            get
            {
                return _ImageSource;
            }
        }

        public bool MoveNext()
        {
            if (_CurrentIndex == _ZipArchiveEntries.Count - 1)
            {
                return false;
            }
            _CurrentIndex++;
            var entry = _ZipArchiveEntries[_CurrentIndex];

            Console.WriteLine(entry.FullName);

            var stream = entry.Open();
            var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            _ImageSource = decoder.Frames[0];
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
                Console.WriteLine(e);
                return false;
            }
            _ImageSource = decoder.Frames[0];
            return true;
        }
    }
}
