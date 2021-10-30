using LyricPlayer.Model;

namespace LyricPlayer.LyricFetcher
{
    public abstract class LyricFetcher
    {
        public abstract TrackLyric GetLyric(TrackInfo trackInfo);
    }
}
