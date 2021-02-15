using LyricPlayer.LyricEngine;
using LyricPlayer.LyricFetcher;
using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using LyricPlayer.SoundEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LyricPlayer.MusicPlayer
{
    public class SpotifyMusicPlayer : NarinoMusicPlayer
    {
        public SpotifyMusicPlayer(string token) : base(token) { }

        protected override void TrackChanged(TrackInfo track)
        {

        }

        public override void Initialize()
        {
            LyricEngine = new NarinoLyricEngine();
            SoundEngine = new SpotifyEngine(LyricEngine);

            SoundEngine.TrackStopped += (s, e) =>
            {
                LyricEngine.Stop();
                Task.Run(() =>
                {
                    var trackInfo = (SoundEngine as SpotifyEngine).TrackInfo;
                    var lyric = LyricFetcher.GetLyric(trackInfo.Name, trackInfo.Name, trackInfo.Album?.Name ?? "",
                        trackInfo.Artists?.FirstOrDefault()?.Name ?? "", SoundEngine.TrackLength / 1000);

                    if (lyric.Lyric.Any(x => x.Duration <= 0))
                        LyricEngine.Load(new TrackLyric
                        {
                            Lyric = new List<Lyric>
                        {
                            new Lyric
                            {
                            Duration = int.MaxValue,
                            StartAt = 0,
                            Element = new TextElement("Lyric isnt synchronized")
                            }
                        }
                        }, SoundEngine);

                    else
                        LyricEngine.Load(lyric, SoundEngine);

                    LyricEngine.Start();
                    LyricEngine.CurrentTime = SoundEngine.CurrentTime;
                });
            };
            LyricEngine.LyricChanged += (s, e) =>
            { OnLyricChanged(e); };
            LyricFetcher = new LocalWithMusicmatchLyricFetcher(AccessToken);
        }
    }
}
