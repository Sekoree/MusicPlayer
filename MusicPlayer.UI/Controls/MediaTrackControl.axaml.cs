using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using MusicPlayer.UI.Models;
using System.Reactive.Subjects;

namespace MusicPlayer.UI.Controls
{
    public class MediaTrackControl : UserControl
    {
        public MediaTrackControl()
        {
            InitializeComponent();
            this.DoubleTapped += TestTap;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void TestTap(object sender, RoutedEventArgs e)
        {
            var wnd = new Window() 
            {
                Background = Brush.Parse("#BB000000"),
                TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur,
                ExtendClientAreaToDecorationsHint = true,
                ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.PreferSystemChrome,
                ExtendClientAreaTitleBarHeightHint = 75
            };
            wnd.PointerPressed += (s, e) =>
            {
                wnd.BeginMoveDrag(e);
            };
            var cp = new MediaTrackControl();
            cp.Bind(MediaTrackControl.DataContextProperty, this.GetObservable(MediaTrackControl.DataContextProperty));
            cp.Margin = new Thickness(0,30,0,0);
            wnd.Content = cp;
            wnd.Topmost = true;
            wnd.Show();
        }
    }
}
