using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyricPlayer.Models;
using LyricPlayer.LyricFetcher.MusicmatchLyricFetcher;

namespace LyricPlayer.LyricFetcher
{
    class LocalWithMusicmatchLyricFetcher : ILyricFetcher
    {
        protected ProxiedMusicmatchLyricFetcher MusicMatchFetcher { set; get; }
        protected LocalLyricFetcher LocalLyricFetcher { set; get; }

        public LocalWithMusicmatchLyricFetcher(string token)
        {
            MusicMatchFetcher = new ProxiedMusicmatchLyricFetcher(token, string.Empty);
            LocalLyricFetcher = new LocalLyricFetcher();
        }
        public TrackLyric GetLyric(string trackName, string title, string album, string artist, double trackLength)
        {
            var result = LocalLyricFetcher.GetLyric(trackName, title, album, artist, trackLength);
            if (result == null)
                result = MusicMatchFetcher.GetLyric(trackName, title, album, artist, trackLength);

            return result;
        }
    }
}
