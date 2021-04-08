using Avalonia.Controls;
using LibVLCSharp;
using MusicPlayer.Backend;
using MusicPlayer.Entities;
using MusicPlayer.Entities.Interfaces;
using MusicPlayer.UI.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace MusicPlayer.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<TrackModel> _trackList = new ObservableCollection<TrackModel>();

        public ObservableCollection<TrackModel> TrackList
        {
            get { return _trackList; }
            set { _trackList = this.RaiseAndSetIfChanged(ref _trackList, value); }
        }

        private TrackModel? _selectedTrack;

        public TrackModel? SelectedTrack
        {
            get { return _selectedTrack; }
            set { _selectedTrack = this.RaiseAndSetIfChanged(ref _selectedTrack, value); }
        }

        private TrackModel? _currentTrack;

        public TrackModel? CurrentTrack
        {
            get { return _currentTrack; }
            set { _currentTrack = this.RaiseAndSetIfChanged(ref _currentTrack, value); }
        }

        private long _currentTrackPosition = 0;

        public long CurrentTrackPosition
        {
            get { return _currentTrackPosition; }
            set { _currentTrackPosition = this.RaiseAndSetIfChanged(ref _currentTrackPosition, value); }
        }

        private string _currentTrackPositionText = "0:00";

        public string CurrentTrackPositionText
        {
            get { return _currentTrackPositionText; }
            set { _currentTrackPositionText = this.RaiseAndSetIfChanged(ref _currentTrackPositionText, value); }
        }


        private string _locationToAdd = "";

        public string LocationToAdd
        {
            get { return _locationToAdd; }
            set { _locationToAdd = this.RaiseAndSetIfChanged(ref _locationToAdd, value); }
        }

        public int Volume
        {
            get => this.player.MediaPlayer.Volume; set => this.player.MediaPlayer.Volume = value;
        }

        private bool? _isShuffle = false;

        public bool? IsShuffle
        {
            get { return _isShuffle; }
            set { _isShuffle = this.RaiseAndSetIfChanged(ref _isShuffle, value); }
        }

        private bool _isNormalize = false;

        public bool IsNormalize
        {
            get { return _isNormalize; }
            set { _isNormalize = this.RaiseAndSetIfChanged(ref _isNormalize, value); }
        }

        private bool _isNotBusy = true;

        public bool IsNotBusy
        {
            get { return _isNotBusy; }
            set { _isNotBusy = this.RaiseAndSetIfChanged(ref _isNotBusy, value); }
        }

        private bool _isFileOut = true;

        public bool IsFileOut
        {
            get { return _isFileOut; }
            set { _isFileOut = this.RaiseAndSetIfChanged(ref _isFileOut, value); }
        }


        private Window parent { get; set; }

        private Player player { get; set; }
        private HttpClient SharedHttpClient { get; set; }
        private YoutubeClient SharedYoutubeClient { get; set; }
        private Random Random { get; set; }
        private bool isStopping = false;

        public MainWindowViewModel(Window parent)
        {
            this.parent = parent;
            this.SharedHttpClient = new HttpClient();
            this.SharedYoutubeClient = new YoutubeClient();
            this.Random = new Random();
            this.player = new Player();
            this.player.OnTrackEndReached += Ended;
            this.player.MediaPlayer.TimeChanged += TimeChanged;
        }

        private void TimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
        {
            this.CurrentTrackPosition = e.Time;
            var toTS = TimeSpan.FromMilliseconds(e.Time);
            if (toTS.Hours != 0)
                this.CurrentTrackPositionText = toTS.ToString(@"hh\:mm\:ss");
            else
                this.CurrentTrackPositionText = toTS.ToString(@"mm\:ss");
        }

        private async Task Ended()
        {
            if (this.TrackList.Count == 0 || this.isStopping)
            {
                this.isStopping = false;
                return;
            }

            TrackModel nextTrack;
            if (this.IsShuffle == true)
                nextTrack = this.TrackList[this.Random.Next(0, this.TrackList.Count)];
            else
            {
                var index = 0;
                var current = this.TrackList.FirstOrDefault(x => x.IsPlaying);
                if (current != null)
                {
                    index = this.TrackList.IndexOf(current) + 1;
                    if (index >= this.TrackList.Count) index = 0;
                }
                nextTrack = this.TrackList[index];
            }
            foreach (var item in this.TrackList.Where(x => x.IsPlaying == true))
                this.TrackList[this.TrackList.IndexOf(item)].IsPlaying = false;
            this.TrackList[this.TrackList.IndexOf(nextTrack)].IsPlaying = true;
            await this.player.SetTrack(nextTrack.BaseTrack, true, this.IsNormalize);
            if (this.IsFileOut)
            {
                var curDir = Directory.GetCurrentDirectory();
                await File.WriteAllTextAsync(@$"{curDir}\Artist.txt", $"| {nextTrack.Artist} |");
                await File.WriteAllTextAsync(@$"{curDir}\Title.txt", $"| {nextTrack.Title} |");
                if (nextTrack.BaseTrack.CoverImage != null)
                {
                    var ms = new MemoryStream();
                    await nextTrack.BaseTrack.CoverImage.CopyToAsync(ms);
                    await File.WriteAllBytesAsync(@$"{curDir}\Cover.jpg", ms.ToArray());
                    await ms.DisposeAsync();
                    if (nextTrack.BaseTrack.CoverImage is MemoryStream cs)
                        cs.Position = 0;
                }
            }
            this.CurrentTrack = this.TrackList[this.TrackList.IndexOf(nextTrack)];
        }

        public async Task PlayPause()
        {
            
            this.IsNotBusy = false;
            if (this.SelectedTrack?.IsPlaying != true
                && !(this.SelectedTrack == null && this.TrackList.Any(x => x.IsPlaying)))
            {
                if (this.TrackList.Count == 0)
                {
                    this.IsNotBusy = true;
                    return;
                }

                TrackModel nextTrack;
                if (this.SelectedTrack != null)
                    nextTrack = this.SelectedTrack;
                else if (this.SelectedTrack == null && this.IsShuffle == true)
                    nextTrack = this.TrackList[this.Random.Next(0, this.TrackList.Count)];
                else
                    nextTrack = TrackList.First();
                foreach (var item in this.TrackList.Where(x => x.IsPlaying == true))
                    this.TrackList[this.TrackList.IndexOf(item)].IsPlaying = false;
                this.TrackList[this.TrackList.IndexOf(nextTrack)].IsPlaying = true;
                await this.player.SetTrack(nextTrack.BaseTrack, true, this.IsNormalize);
                this.SelectedTrack = default;
                if (this.IsFileOut)
                {
                    var curDir = Directory.GetCurrentDirectory();
                    await File.WriteAllTextAsync(@$"{curDir}\Artist.txt", $"| {nextTrack.Artist} |");
                    await File.WriteAllTextAsync(@$"{curDir}\Title.txt", $"| {nextTrack.Title} |");
                    if (nextTrack.BaseTrack.CoverImage != null)
                    {
                        var ms = new MemoryStream();
                        await nextTrack.BaseTrack.CoverImage.CopyToAsync(ms);
                        await File.WriteAllBytesAsync(@$"{curDir}\Cover.jpg", ms.ToArray());
                        await ms.DisposeAsync();
                        if (nextTrack.BaseTrack.CoverImage is MemoryStream cs)
                            cs.Position = 0;
                    }
                }
                this.CurrentTrack = this.TrackList[this.TrackList.IndexOf(nextTrack)];
            }
            else
            {
                await this.player.PlayPause();
            }
            this.IsNotBusy = true;
        }

        public Task RemoveSong(TrackModel tm)
        {
            this.TrackList.Remove(tm);
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            this.isStopping = true;
            await this.player.Stop();
        }
        public async Task Skip()
        {
            await this.Ended();
        }

        public async Task AddLocalTracks()
        {
            this.IsNotBusy = false;
            var ofd = new OpenFileDialog();
            ofd.AllowMultiple = true;
            ofd.Title = "Select media files";
            var result = await ofd.ShowAsync(parent);
            foreach (var item in result)
            {
                var nt = new LocalTrack(item);
                await nt.LoadMetadata();
                await nt.GetCoverImage();
                var tt = new TrackModel(this, nt);
                this.TrackList.Add(tt);
            }
            this.IsNotBusy = true;
        }

        public async Task AddURL()
        {
            this.IsNotBusy = false;
            if (this.LocationToAdd.Contains("youtu")
                && this.LocationToAdd.Contains("watch?v="))
            {
                var nt = new YoutubeTrack(new Uri(this.LocationToAdd), this.SharedYoutubeClient, this.SharedHttpClient);
                var couldLoad = await nt.LoadMetadata();
                if (!couldLoad)
                {
                    this.SharedYoutubeClient = new YoutubeClient();
                    this.IsNotBusy = true;
                    return;
                }
                await nt.GetCoverImage();
                var tt = new TrackModel(this, nt);
                this.TrackList.Add(tt);
                this.LocationToAdd = string.Empty;
            }
            this.IsNotBusy = true;
        }

        public async Task AddYoutubePlaylist()
        {
            this.IsNotBusy = false;
            if (this.LocationToAdd.Contains("youtu")
                && this.LocationToAdd.Contains("list"))
            {
                IReadOnlyList<PlaylistVideo?> videos;
                try
                {
                    videos = await this.SharedYoutubeClient.Playlists.GetVideosAsync(this.LocationToAdd);
                }
                catch (Exception)
                {
                    this.SharedYoutubeClient = new YoutubeClient();
                    this.IsNotBusy = true;
                    return;
                }
                foreach (var item in videos.Where(x => x != null))
                {
                    this.LocationToAdd = $"Adding {item?.Title}";
                    var nt = new YoutubeTrack(item, this.SharedYoutubeClient, this.SharedHttpClient);
                    await nt.GetCoverImage();
                    var tt = new TrackModel(this, nt);
                    this.TrackList.Add(tt);
                }
            }
            this.LocationToAdd = string.Empty;
            this.IsNotBusy = true;
        }

    }
}
