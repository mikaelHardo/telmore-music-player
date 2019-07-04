using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WindowStyle = System.Windows.WindowStyle;

namespace MusicPlayerNew
{
    public partial class MainWindow : Window
    {
        private bool _showBrowser;
        private string _oldCover = "";
        private readonly Hook _hook = new Hook();
        private readonly Player _player;

        public MainWindow()
        {
            InitializeComponent();

            _hook.Register(KeyModifier.None, Keys.MediaPreviousTrack.GetHashCode(), () => { PreviousClicked(null, null); });   
            _hook.Register(KeyModifier.None, Keys.MediaPlayPause.GetHashCode(), () => { PlayPauseClicked(null, null); });
            _hook.Register(KeyModifier.None, Keys.MediaNextTrack.GetHashCode(), () => { NextClicked(null, null); });

            _player = new Player(WebBrowser, GetInfo);

            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;


            MouseEnter += (sender, args) => { ToggleButtons(true); };
            MouseLeave += (sender, args) => { ToggleButtons(false); };

            MouseDown += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    DragMove();
                }
            };

            MouseDoubleClick += Scale;
        }

        private void Scale(object sender, MouseButtonEventArgs e)
        {
            if (!Topmost)
            {
                Width = 120;
                Height = 80;
            }
            else
            {
                Width = 200;
                Height = 200;
            }

            Topmost = !Topmost;
        }

        private void ToggleButtons(bool show)
        {
            var buttons = new[] { PrevButton, NextButton, PlayButton, ToggleButton, CloseButton };

            foreach (var button in buttons)
            {
                button.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void GetInfo(Info info)
        {
            TrackTitle.Content = info.Title;
            TrackArtist.Content = info.Artist;

            Title = $"Telmore Music ({info.Artist} - {info.Title})";

            if (info.Cover != _oldCover && info.Cover.StartsWith("http"))
            {
                Cover.Source = new BitmapImage(new Uri(info.Cover.Replace("60x60", "300x300")));
                _oldCover = info.Cover;
            }
            else
            {
                //TODO: show placeholder image
            }

            ProgressBar.Value = info.Progress;
        }

        private void PlayPauseClicked(object sender, RoutedEventArgs e)
        {
            _player.PlayPause();
        }

        private void PreviousClicked(object sender, RoutedEventArgs e)
        {
            _player.Previous();
        }

        private void NextClicked(object sender, RoutedEventArgs e)
        {
            _player.Next();
        }

        private void ToggleClick(object sender, RoutedEventArgs e)
        {
            if (_showBrowser)
            {
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                WebBrowser.Visibility = Visibility.Collapsed;
                Width = 200;
                Height = 200;
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowStyle = WindowStyle.ThreeDBorderWindow;
                ResizeMode = ResizeMode.CanResizeWithGrip;
                WebBrowser.Visibility = Visibility.Visible;
            }

            _showBrowser = !_showBrowser;
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            _hook.Dispose();
            Close();
        }
    }
}
