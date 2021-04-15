using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using MusicPlayer.UI.ViewModels;

namespace MusicPlayer.UI.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(this);
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override async void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Space
                || e.Key == Key.MediaPlayPause)
            {
                var dc = this.DataContext as MainWindowViewModel;
                if (dc != null)
                    await dc.PlayPause();
            }

            if (e.Key == Key.MediaNextTrack)
            {
                var dc = this.DataContext as MainWindowViewModel;
                if (dc != null)
                    await dc.Skip();
            }

            if (e.Key == Key.MediaStop)
            {
                var dc = this.DataContext as MainWindowViewModel;
                if (dc != null)
                    await dc.Stop();
            }
        }
    }
}
