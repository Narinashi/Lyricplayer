using LyricPlayer.LyricEngine;
using LyricPlayer.LyricFetcher.MusicmatchLyricFetcher;
using LyricPlayer.Models;
using LyricPlayer.PlaylistController;
using LyricPlayer.SoundEngine;
using System;

namespace LyricPlayer.MusicPlayer
{
    public class NarinoMusicPlayer : MusicPlayer<TrackInfo>
    {
        string AccessToken { set; get; }
        public NarinoMusicPlayer(string token)
        {
            Playlist = new PlaylistController<TrackInfo>();
            AccessToken = token;
        }
        public override void Initialize()
        {
            SoundEngine = new NAudioPlayer();
            LyricEngine = new NarinoLyricEngine();
            SoundEngine.TrackStopped += (s, e) => { Next(); };
            LyricEngine.LyricChanged += (s, e) =>
            { LyricChanged?.Invoke(this, e); };
            LyricFetcher = new MusicmatchLyricFetcher(AccessToken,"" /*"http://192.168.1.110:8888"*/);
        }


        public event EventHandler<Lyric> LyricChanged;
    }
}
