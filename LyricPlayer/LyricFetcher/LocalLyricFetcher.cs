using LyricPlayer.LyricEffects;
using LyricPlayer.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace LyricPlayer.LyricFetcher
{
    class LocalLyricFetcher : ILyricFetcher
    {
        public TrackLyric GetLyric(string trackName, string Title, string Album, string Artist, double trackLength)
        {
            var trackLyric = JsonConvert.DeserializeObject<TrackLyric>(File.ReadAllText(Path.Combine("Lyrics", trackName + ".lyr")));
            var lyric = trackLyric.Lyric;

            if (lyric[0].StartAt > 0)
                lyric.Insert(0, new Lyric
                {
                    Text = "...",
                    StartAt = 0,
                    Duration = lyric[0].StartAt
                });

            for (int index = 0; index < lyric.Count - 1; index++)
                if (lyric[index].Duration < 1)
                    lyric[index].Duration = lyric[index + 1].StartAt - lyric[index].StartAt;

            lyric.Last().Duration = int.MaxValue;

            return AddDefaultLyricEffects(trackLyric);
        }


        public TrackLyric AddDefaultLyricEffects(TrackLyric trackLyric)
        {
            var lyric = trackLyric.Lyric;

            for (int index = 0; index < lyric.Count; index++)
            {
                if (index % 2 == 0)
                    lyric[index].Effects = new List<LyricEffect>
                    {
                        new ColorChangeEffect
                        {
                            BackgroundColorChangeFrom = Color.FromArgb(70, 0, 0, 0),
                            BackgroundColorChangeTo = Color.FromArgb(70, 255, 80, 255),
                            ForeColorChangeTo = Color.FromArgb(210, 255, 255, 80),
                            ForeColorChangeFrom = Color.FromArgb(210, 255, 255, 255),
                            Duration = lyric[index].Duration
                        }
                    };
                else
                    lyric[index].Effects = new List<LyricEffect>
                    {
                        new ColorChangeEffect
                        {
                            BackgroundColorChangeTo = Color.FromArgb(70, 0, 0, 0),
                            BackgroundColorChangeFrom = Color.FromArgb(70, 255, 80, 255),
                            ForeColorChangeFrom = Color.FromArgb(210, 255, 255, 80),
                            ForeColorChangeTo = Color.FromArgb(210, 255, 255, 255),
                            Duration = lyric[index].Duration
                        }
                    };
            }

            return trackLyric;
        }

    }
}
