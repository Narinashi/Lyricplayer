using LyricPlayer.Model;
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
            var elements = trackLyric.RootElement.ChildElements;

            if (elements.Count < 2 && elements.FirstOrDefault()?.Duration > 1000000)
                return null;

            trackLyric.Copyright = trackLyric.Copyright?.Trim()?.Replace("\n", " ") ?? string.Empty;
            elements.Last().Duration = int.MaxValue;

            return trackLyric;
        }
    }
}
