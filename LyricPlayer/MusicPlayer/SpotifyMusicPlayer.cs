using LyricPlayer.LyricFetcher;
using LyricPlayer.Model;
using LyricPlayer.SoundEngine;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LyricPlayer.MusicPlayer
{
    public class SpotifyMusicPlayer : NarinoMusicPlayer
    {
        public SpotifyMusicPlayer(string token) : base(token) { }

        protected override void PlaylistTrackChanged(TrackInfo track)
        {

        }

        public override void Initialize()
        {
            SoundEngine = new SpotifyEngine();
            LyricFetcher = new LocalWithMusicmatchLyricFetcher(AccessToken);
            SoundEngine.TrackStopped += (s, e) =>
            {
                Task.Run(() =>
                {
                    var trackInfo = (SoundEngine as SpotifyEngine).TrackInfo;
                    CurrentTrackLyric = LyricFetcher.GetLyric(trackInfo.Name, trackInfo.Name, trackInfo.Album?.Name ?? "",
                        trackInfo.Artists?.FirstOrDefault()?.Name ?? "", SoundEngine.TrackLength / 1000);

                    Console.WriteLine($"Lyric for {trackInfo.Name} loaded");
                    OnTrackChanged();
                });
            };

        }
    }
}
