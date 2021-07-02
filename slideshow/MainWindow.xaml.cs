using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using static SlideShow.Utils;

namespace SlideShow
{
    public partial class MainWindow : Window
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

        DispatcherTimer _DispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            _ImageFileList = new FilePathImageFileStreamSeries(Environment.GetCommandLineArgs().Skip(1));
            if (_ImageFileList.Current == null)
            {
                Close();
            }

            _LoopOption = true;

            _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, _IntervalOption);
            _DispatcherTimer.Tick += (object sender, EventArgs e) => ShowNextImage();
            InitializeComponent();
        }

        void ShowNextImage()
        {
            Log("画像を切り替えます。");
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

        void ShowPreviousImage()
        {
            if (!_ImageFileList.MovePrevious())
            {
                _ImageFileList.MoveFirst();
            }
            image.Source = _ImageFileList.Current;
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
            image.Source = _ImageFileList.Current;

            //            _ShowNextImage();
            _DispatcherTimer.Start();
        }
    }
}
