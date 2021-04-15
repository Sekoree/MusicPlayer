using LibVLCSharp;
using LibVLCSharp.Shared;
using MusicPlayer.Entities;
using MusicPlayer.Entities.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicPlayer.Backend
{
    public class Player
    {
        private LibVLC libVLC { get; }
        public MediaPlayer MediaPlayer { get; set; }

        public delegate Task TrackEndReached();
        public event TrackEndReached OnTrackEndReached;

        public Player()
        {
            Core.Initialize();
            this.libVLC = new LibVLC(false, "--no-video", "--audio-filter=normvol");
            this.MediaPlayer = new MediaPlayer(this.libVLC);
            MediaPlayer.EndReached += (s,e) => { OnTrackEndReached.Invoke(); };
        }

        private void LogHappened(object sender, LogEventArgs e)
        {
            Debug.WriteLine(e.FormattedLog);
        }

        public async Task<bool> SetTrack(IBaseTrack track, bool playNext = false, bool normalize = false)
        {
            try
            {
                var location = await track.GetUpdatedLocation();
                this.MediaPlayer.Media?.Dispose();
                if (playNext)
                {
                    Media media = default;
                    if (normalize)
                    {
                        var premedia = new NormalizedMedia(location);
                        media = new Media(libVLC, premedia);
                    }
                    else
                    {
                        media = new Media(libVLC, track.RealLocation);
                    }
                    ThreadPool.QueueUserWorkItem(_ => this.MediaPlayer.Play(media));
                }
                else
                {
                    var media = new Media(libVLC, location, ":normvol", ":volnorm");
                    ThreadPool.QueueUserWorkItem(_ => this.MediaPlayer.Media = media);
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task<bool> PlayPause()
        {
            if (MediaPlayer.IsPlaying)
                ThreadPool.QueueUserWorkItem(_ => this.MediaPlayer.Pause());
            else
                ThreadPool.QueueUserWorkItem(_ => this.MediaPlayer.Play());
            return Task.FromResult(true);
        }

        public Task<bool> SetVolume(int volume)
        {
            this.MediaPlayer.Volume = volume;
            return Task.FromResult(true);
        }

        public Task<bool> Stop()
        {
            this.MediaPlayer.Stop();
            return Task.FromResult(true);
        }

        #region Custom Playback Template
        void PlayAudio(IntPtr data, IntPtr samples, uint count, long pts)
        {
        }

        void DrainAudio(IntPtr data)
        {
        }

        void FlushAudio(IntPtr data, long pts)
        {
        }

        void ResumeAudio(IntPtr data, long pts)
        {
        }

        void PauseAudio(IntPtr data, long pts)
        {
        }

        void AudioCleanup(IntPtr opaque) { }

        #endregion
    }
}
