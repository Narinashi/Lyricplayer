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
      
    }
}
