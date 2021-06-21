using MusicPlayer.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace MusicPlayer.Entities
{
    public class YoutubeTrack : IBaseTrack
    {
        public bool MetadataLoaded { get; set; } = false;

        public string Title { get; set; }

        public string Artist { get; set; }

        public TimeSpan Duration { get; set; }

        public Stream CoverImage { get; set; }

        public Uri Location { get; set; }

        public Uri RealLocation { get; set; }
        private Video video { get; set; }
        private PlaylistVideo playlistVideo { get; set; }

        private YoutubeClient hostYoutubeClient { get; set; }
        private HttpClient hostHttpClient { get; set; }

        public YoutubeTrack(Uri url, YoutubeClient yC, HttpClient hC)
        {
            this.Location = url;
            this.hostHttpClient = hC;
            this.hostYoutubeClient = yC;
        }

        public YoutubeTrack(PlaylistVideo video, YoutubeClient yC, HttpClient hC)
        {
            this.playlistVideo = video;
            this.Location = new Uri(video.Url);
            this.Title = video.Title;
            this.Artist = video.Author.Title;
            this.Duration = video.Duration.Value;
            this.hostHttpClient = hC;
            this.hostYoutubeClient = yC;
            this.MetadataLoaded = true;
        }

        public async Task<Stream> GetCoverImage()
        {
            if (video == null && playlistVideo == null) return default;
            var thumb = await hostHttpClient.GetByteArrayAsync(video == null ? playlistVideo.Thumbnails[0].Url : video.Thumbnails[0].Url);
            var ms = new MemoryStream(thumb);
            ms.Position = 0;
            this.CoverImage = ms;
            return ms;
        }

        public async Task<Uri> GetUpdatedLocation()
        {
            if (video == null && playlistVideo == null) return default;
            var streamManifest = await hostYoutubeClient.Videos.Streams.GetManifestAsync(video == null ? playlistVideo.Id : video.Id);
            var audio = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            this.RealLocation = new Uri(audio.Url);
            return this.RealLocation;
        }

        public async Task<bool> LoadMetadata()
        {
            try
            {
                this.video = await hostYoutubeClient.Videos.GetAsync(this.Location.OriginalString);
                this.Title = video.Title;
                this.Artist = video.Author.Title;
                this.Duration = video.Duration.Value;
                this.MetadataLoaded = true;
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
