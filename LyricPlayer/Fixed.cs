using Newtonsoft.Json;
using System;

namespace LyricPlayer
{
    static class Fixed
    {
        public const float AlmostZero = 0.0001f;
        public const int SongTrackingInterval = 2000;
        public static long UnixTime => (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

        public static string DefaultFontName => "Antonio";
        public static int DefaultFontSize => 55;

        public static int RNG(int from, int to)
        {
            var random = new Random();
            return random.Next(from, to);
        }
        public static JsonSerializerSettings JsonSerializationSetting => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
    }
}
