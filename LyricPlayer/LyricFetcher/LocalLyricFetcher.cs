using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace LyricPlayer.LyricFetcher
{
    class LocalLyricFetcher : ILyricFetcher
    {
        public TrackLyric GetLyric(string trackName, string Title, string Album, string Artist, double trackLength)
        {
            var validFileName = trackName.ReplaceToValidFileName();
            var filePath = Path.Combine("Lyrics", validFileName + ".lyr");
            if (!File.Exists(filePath))
                return null;

            var trackLyric = JsonConvert.DeserializeObject<TrackLyric>(File.ReadAllText(filePath), Fixed.JsonSerializationSetting);
            var lyric = trackLyric.Lyric;

            if (lyric.Count < 2 && lyric.FirstOrDefault()?.Duration > 1000000)
                return null;

            trackLyric.Copyright = trackLyric.Copyright?.Trim()?.Replace("\n", " ") ?? string.Empty;
            if (lyric[0].StartAt > 0)
                lyric.Insert(0, new Lyric
                {
                    StartAt = 0,
                    Duration = lyric[0].StartAt,
                    Element = new TextElement("...") { FontName = Fixed.DefaultFontName, FontSize = Fixed.DefaultFontSize }
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

            foreach (var l in lyric.Where(x => x.Element == null))
                l.Element = new TextElement(l) { AutoSize = true, FontName = Fixed.DefaultFontName, FontSize = Fixed.DefaultFontSize };

            return trackLyric;
        }

    }
}
