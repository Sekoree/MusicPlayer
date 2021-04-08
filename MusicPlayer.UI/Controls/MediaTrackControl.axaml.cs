using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MusicPlayer.UI.Models;

namespace MusicPlayer.UI.Controls
{
    public class MediaTrackControl : UserControl
    {
        public MediaTrackControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
