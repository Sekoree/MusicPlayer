using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MusicPlayer.Entities
{
    public class Shared
    {
        public static HttpClient SharedClient = new HttpClient();
    }
}
