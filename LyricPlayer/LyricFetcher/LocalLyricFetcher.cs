using LyricPlayer.Model;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace LyricPlayer.LyricFetcher
{
    class LocalLyricFetcher : LyricFetcher
    {
        public override TrackLyric GetLyric(TrackInfo trackInfo)
        {
            var validFileName = trackInfo.TrackName.ReplaceToValidFileName();
            var filePath = Path.Combine("Lyrics", validFileName + ".lyr");
            if (!File.Exists(filePath))
                return null;

            var trackLyric = JsonConvert.DeserializeObject<TrackLyric>(File.ReadAllText(filePath), Fixed.JsonSerializationSetting);
            var elements = trackLyric.RootElement.ChildElements;

            if (elements.Count < 2 && elements.FirstOrDefault()?.Duration > 1000000)
                return null;

            trackLyric.Copyright = trackLyric.Copyright?.Trim()?.Replace("\n", " ") ?? string.Empty;
            elements.Last().Duration = uint.MaxValue/2;

            return trackLyric;
        }
    }
}
