using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyricPlayer.Model;
using LyricPlayer.LyricFetcher.MusicmatchLyricFetcher;

namespace LyricPlayer.LyricFetcher
{
    class LocalWithMusicmatchLyricFetcher : LyricFetcher
    {
        protected MusicxMatchLyricFetcher MusicMatchFetcher { set; get; }
        protected LocalLyricFetcher LocalLyricFetcher { set; get; }

        public LocalWithMusicmatchLyricFetcher()
        {
            MusicMatchFetcher = new MusicxMatchLyricFetcher();
            LocalLyricFetcher = new LocalLyricFetcher();
        }

        public override TrackLyric GetLyric(TrackInfo trackInfo)
        {
            var result = LocalLyricFetcher.GetLyric(trackInfo);
            if (result == null)
                result = MusicMatchFetcher.GetLyric(trackInfo);

            return result;
        }
    }
}
