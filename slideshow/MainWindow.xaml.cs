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
            //var filePathList = new List<string> { "image_0.jpg", "b/image_1.jpg", "b/image_2.jpg", "b/image_3.jpg", "b/a/image_4.jpg", "b/a/aa/image_5.jpg" };
            //var filePathList = new List<string> { "a/aa/非アスキー文字.zip" };
            //var filePathList = new List<string> { "b" };
            var filePathList = new List<string>(Environment.GetCommandLineArgs()[1..^0]);// コマンドラインオプションに指定されたパス名のリストを取得する。(先頭の要素は実行ファイルのパス名なので無視している。)
                                                                                         //            filePathList.RemoveAt(0);

            Log("以下の画像ファイル/フォルダ内の画像ファイル/Zipファイル内の画像を表示します。");
            filePathList.ForEach((e) =>
            {
                Log($"    {e}");
            });

            _ImageFileList = new FilePathImageFileStreamSeries(filePathList, () => _LoopOption);

            _LoopOption = true;

            _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, _IntervalOption);
            _DispatcherTimer.Tick += (object? sender, EventArgs e) => ShowNextImage();
            InitializeComponent();
        }

        void ShowNextImage()
        {
            //Log("次の画像を表示します。");
            var imageSource = _ImageFileList.GetNext();
            if (imageSource == null)
            {
                Log("次の画像はないので終了します。");
                Close();
                return;
            }
#if false
            image.Source = imageSource;
#elif true
            var mat = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat((BitmapSource)imageSource);
            var bs = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(mat);
            image.Source = bs;
#else
            imageSource.be
            image.Source = imageSource;
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
            Log("次に進みます。");
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
            Log("前に戻ります。");
            ShowPreviousImage();
            if (_DispatcherTimer.IsEnabled)
            {
                _DispatcherTimer.Stop();
                _DispatcherTimer.Start();
            }
        }

        void Pause()
        {
            if (_DispatcherTimer.IsEnabled)
            {
                Log("一時停止します。");
                _DispatcherTimer.Stop();
            }
        }

        void Resume()
        {
            if (!_DispatcherTimer.IsEnabled)
            {
                Log("再開します。");
                _DispatcherTimer.Start();
            }
        }

        void QuitApplication()
        {
            Log("終了します。");
            this.Close();
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    Forward();
                    break;
                case Key.Back:
                    Backward();
                    break;
                case Key.P:
                    Pause();
                    break;
                case Key.S:
                    Resume();
                    break;
                case Key.Escape:
                    QuitApplication();
                    break;
                case Key.Enter:
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
                Log("フルスクリーンモードにします。");
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.Topmost = true;
            }
            else
            {
                Log("フルスクリーンモードを解除します。");
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
