using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MusicPlayer.UI.Controls
{
    public class MediaControlsControl : UserControl
    {
        public MediaControlsControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
