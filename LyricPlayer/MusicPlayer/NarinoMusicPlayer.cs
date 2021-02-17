using LyricPlayer.LyricEngine;
using LyricPlayer.LyricFetcher;
using LyricPlayer.Model;
using LyricPlayer.PlaylistController;
using LyricPlayer.SoundEngine;
using System;

namespace LyricPlayer.MusicPlayer
{
    public class NarinoMusicPlayer : MusicPlayer<TrackInfo>
    {
        protected string AccessToken { set; get; }
        public NarinoMusicPlayer(string token)
        {
            Playlist = new PlaylistController<TrackInfo>();
            AccessToken = token;
        }
        public override void Initialize()
        {
            if (LyricEngine == null)
            {
                LyricEngine = new NarinoLyricEngine();
                LyricEngine.LyricChanged += (s, e) => OnLyricChanged(e);
            }
            if (SoundEngine == null)
            {
                SoundEngine = new NAudioPlayer();
                SoundEngine.TrackStopped += (s, e) =>
                {
                    Next();
                };
            }
            LyricFetcher = new LocalWithMusicmatchLyricFetcher(AccessToken);
        }

        public event EventHandler LyricChanged;
        protected virtual void OnLyricChanged(EventArgs e)
        {
            LyricChanged?.Invoke(this, e);
        }
    }
}
