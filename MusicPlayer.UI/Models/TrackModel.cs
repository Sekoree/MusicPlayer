using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using MusicPlayer.Entities.Interfaces;
using MusicPlayer.UI.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.UI.Models
{
    public class TrackModel : ReactiveObject
    {
        public readonly IBaseTrack BaseTrack;
        public readonly MainWindowViewModel viewModel;
        public bool IsPlaying { get; set; } = false;
        public string Title { get => BaseTrack.Title; }
        public string Artist { get => BaseTrack.Artist; }
        public long Length { get => Convert.ToInt64(BaseTrack.Duration.TotalMilliseconds); }
        public string LengthText 
        {
            get
            {
                if (this.BaseTrack.Duration.Hours != 0)
                    return BaseTrack.Duration.ToString(@"hh\:mm\:ss");
                else
                    return BaseTrack.Duration.ToString(@"mm\:ss");
            }
        }
        public Bitmap? CoverImage { get
            {
                try
                {
                    var bp = new Bitmap(BaseTrack.CoverImage);
                    BaseTrack.CoverImage.Position = 0;
                    return bp;
                }
                catch (Exception)
                { }
                return null;
            }
        }

        public TrackModel(MainWindowViewModel vm, IBaseTrack track)
        {
            this.viewModel = vm;
            this.BaseTrack = track;
        }

        public async Task Remove()
        {
            await this.viewModel.RemoveSong(this);
        }
    }
}
