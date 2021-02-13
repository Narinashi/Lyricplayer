using LyricPlayer.Model;

namespace LyricPlayer.LyricFetcher
{
    public interface ILyricFetcher
    {
        TrackLyric GetLyric(string trackName, string title, string album, string artist, double trackLength);
    }
}
