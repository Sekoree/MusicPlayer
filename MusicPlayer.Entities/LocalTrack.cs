using ATL;
using MusicPlayer.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Entities
{
    public class LocalTrack : IBaseTrack
    {
        public bool MetadataLoaded { get; set; } = false;
        public string Title { get => this.trackInfo.Title; }
        public string Artist { get => this.trackInfo.Artist; }
        public TimeSpan Duration { get => TimeSpan.FromMilliseconds(this.trackInfo.DurationMs); }
        public Stream CoverImage { get; set; }
        public Uri Location { get; set; }
        public Uri RealLocation { get; set; }

        private Track trackInfo { get; set; }

        public LocalTrack(string path)
        {
            this.Location = new Uri(path);
            this.trackInfo = new Track(path);
            this.MetadataLoaded = true;
        }

        public Task<Uri> GetUpdatedLocation()
        {
            this.RealLocation = this.Location;
            return Task.FromResult(this.RealLocation);
        }

        public Task<bool> LoadMetadata() => Task.FromResult(true);

        public Task<Stream> GetCoverImage()
        {
            if (trackInfo.EmbeddedPictures.Count == 0) return Task.FromResult<Stream>(default);
            var ms = new MemoryStream(trackInfo.EmbeddedPictures[0].PictureData);
            ms.Position = 0;
            this.CoverImage = ms;
            return Task.FromResult((Stream)ms);
        }
    }
}
