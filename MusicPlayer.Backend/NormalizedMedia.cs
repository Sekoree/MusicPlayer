using LibVLCSharp;
using LibVLCSharp.Shared;
using System;
using System.Diagnostics;
using System.IO;

namespace MusicPlayer.Backend
{
    public class NormalizedMedia : MediaInput
    {
        private readonly Uri mediaUri;
        private readonly Process ffmpegProcess;

        public NormalizedMedia(Uri link)
        {
            this.mediaUri = link ?? throw new ArgumentNullException(nameof(mediaUri));
            this.ffmpegProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{this.mediaUri}"" -filter:a loudnorm -f opus -",
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });
            this.CanSeek = false;
        }

        public override bool Open(out ulong size)
        {
            try
            {
                //BaseStream throws not supported if accessed
                size = (ulong.MaxValue);
                return true;
            }
            catch (Exception)
            {
                size = 0UL;
                return false;
            }
        }

        public unsafe override int Read(IntPtr buf, uint len)
        {
            try
            {
                return this.ffmpegProcess.StandardOutput.BaseStream.Read(new Span<byte>(buf.ToPointer(), (int)len));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public override bool Seek(ulong offset)
        {
            return false;
        }


        public override void Close()
        {
            this.ffmpegProcess.Kill();
        }

        protected override void Dispose(bool disposing)
        {
            this.Close();
            base.Dispose(disposing);
        }
    }
}
