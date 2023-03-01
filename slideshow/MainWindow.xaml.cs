using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static SlideShow.Utils;

namespace SlideShow
{
    public partial class MainWindow : System.Windows.Window
    {
        readonly FilePathImageFileStreamSeries _ImageFileList;

        /// <summary>
        /// 繰り返すかどうか
        /// </summary>
        bool _LoopOption = false;

        /// <summary>
        /// 画像表示時間間隔[ms]
        /// </summary>
        int _IntervalOption = 2000;

        bool _IsFullScreenMode = false;
        readonly DispatcherTimer _DispatcherTimer = new();

        public MainWindow()
        {
            Log("開始");
            //var filePathList = new List<string> { "image_0.jpg", "b/image_1.jpg", "b/image_2.jpg", "b/image_3.jpg", "b/a/image_4.jpg", "b/a/aa/image_5.jpg" };
            //var filePathList = new List<string> { "a/aa/非アスキー文字.zip" };
            var filePathList = new List<string> { "b" };
            //_ImageFileList = new FilePathImageFileStreamSeries(Environment.GetCommandLineArgs().Skip(1), () => _LoopOption);
            _ImageFileList = new FilePathImageFileStreamSeries(filePathList, () => _LoopOption);

            _LoopOption = true;

            _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, _IntervalOption);
            _DispatcherTimer.Tick += (object sender, EventArgs e) => ShowNextImage();
            InitializeComponent();
        }

        void ShowNextImage()
        {
            Log("次の画像を表示します。");
            var imageSource = _ImageFileList.GetNext();
            if (imageSource == null)
            {
                Log("次の画像はないので終了します。");
                Close();
                return;
            }
#if false
            image.Source = imageSource;
#else
            var mat = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat((BitmapSource)imageSource);
            var bs = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(mat);
            image.Source = bs;
#endif
        }

        void ShowPreviousImage()
        {
            //if (!_ImageFileList.MovePrevious())
            //{
            //    _ImageFileList.MoveFirst();
            //}
            //image.Source = _ImageFileList.Current;
        }

        void Forward()
        {
            ShowNextImage();
            if (_DispatcherTimer.IsEnabled)
            {
                _DispatcherTimer.Stop();
                _DispatcherTimer.Start();
            }
            //            Console.WriteLine("function FORWARD is not implemented");
        }

        void Backward()
        {
            ShowPreviousImage();
            if (_DispatcherTimer.IsEnabled)
            {
                _DispatcherTimer.Stop();
                _DispatcherTimer.Start();
            }
        }

        void PauseOrResume()
        {
            if (_DispatcherTimer.IsEnabled)
                _DispatcherTimer.Stop();
            else
                _DispatcherTimer.Start();
        }

        void QuitApplication()
        {
            this.Close();
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    Forward();
                    break;
                case Key.Left:
                    Backward();
                    break;
                case Key.Space:
                    PauseOrResume();
                    break;
                case Key.Escape:
                    QuitApplication();
                    break;
                case Key.F:
                    ToggleFullScreenMode();
                    break;
                default:
                    break;
            }
        }

        void ToggleFullScreenMode()
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

        void Window_Initialized(object sender, EventArgs e)
        {
            ShowNextImage();
            _DispatcherTimer.Start();
        }
    }
}
