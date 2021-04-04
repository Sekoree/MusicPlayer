using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Entities.Interfaces
{
    public interface IBaseTrack
    {
        public bool MetadataLoaded { get; set; }
        public string Title { get; }
        public string Artist { get;  }
        public TimeSpan Duration { get; }
        public Stream CoverImage { get; }
        public Uri Location { get; }
        public Uri RealLocation { get; }

        public Task<bool> LoadMetadata();
        public Task<Uri> GetUpdatedLocation();
        public Task<Stream> GetCoverImage();
    }
}
