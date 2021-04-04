using ATL;
using MusicPlayer.Backend;
using MusicPlayer.Entities;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using Un4seen.Bass;
using System.Runtime.InteropServices;

namespace MusicPlayer.Test
{
    class Program
    {
        static Player pl { get; set; }
        static async Task Main(string[] args)
        {
            //pl = new Player();
            var vid = new YoutubeTrack(new Uri("https://www.youtube.com/watch?v=uKqriTRYInY"), new YoutubeClient(), new HttpClient());
            var met = await vid.LoadMetadata();
            var da = await vid.GetUpdatedLocation();
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            var bi = new BASS_CHANNELINFO();
            //int stream2 = Bass.BASS_StreamCreateFile(@"C:\Users\Speyd\Desktop\01. Radio Happy.flac", 0L, 0L, BASSFlag.BASS_DEFAULT);
            int stream = Bass.BASS_StreamCreateURL(da.OriginalString, 0, BASSFlag.BASS_DEFAULT, null, IntPtr.Zero);
            Bass.BASS_ChannelPlay(stream, false);
            //var hm = await hc.GetAsync(vid.RealLocation);
            //Bass.Free();
            //var init = Bass.Init();
            //var op = ManagedBass.Opus.BassOpus.CreateStream(da.OriginalString);
            //var flac = Bass.PluginLoad(@"bassflac.dll");
            //var opus = Bass.PluginLoad(@"bassopus.dll");
            //var webm = Bass.PluginLoad(@"basswebm.dll");
            //var leng = await hc.GetAsync(da, HttpCompletionOption.ResponseHeadersRead);
            //var c = Bass.CreateStream(da.OriginalString);
            //Bass.ChannelPlay(op);
            await Task.Delay(-1);
        }

        private void MyDownloadProc(IntPtr buffer, int length, IntPtr user)
        {
            if (buffer != IntPtr.Zero && length == 0)
            {
                // the buffer contains HTTP or ICY tags.
                string txt = Marshal.PtrToStringAnsi(buffer);
                // you might instead also use "this.BeginInvoke(...)", which would call the delegate asynchron!
            }
        }
    }
}
