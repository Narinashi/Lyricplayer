using LyricPlayer.Model;
using LyricPlayer.Model.Effects;
using LyricPlayer.Model.Elements;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LyricPlayer.LyricFetcher.MusicmatchLyricFetcher
{
    class MusicmatchLyricFetcher : ILyricFetcher
    {
        protected static HttpClientHandler Handler = new HttpClientHandler();
        protected static HttpClient Client = new HttpClient(Handler);
        protected string TrackName { set; get; }
        string UserToken { set; get; }

        public MusicmatchLyricFetcher(string token, string proxy)
        {
            UserToken = token;
            Handler.UseProxy = !string.IsNullOrEmpty(proxy);
            if (Handler.UseProxy)
                Handler.Proxy = new WebProxy { Address = new Uri(proxy) };
        }

        public TrackLyric GetLyric(string trackName, string title, string album, string artist, double trackLength)
        {
            string body = CallMusicMatchService(trackName, title, album, artist, trackLength);
            if (string.IsNullOrEmpty(body))
                body = CallMusicMatchService(trackName, title, album, artist, trackLength);
            if (string.IsNullOrEmpty(body))
                return CreateTrackLyric(new List<TextElement>()
                {
                    new TextElement()
                    {
                        Text = "Lyric not found",
                        FontName = Fixed.DefaultFontName,
                        FontSize = Fixed.DefaultFontSize,
                        Duration = int.MaxValue / 2
                    }
                });


            var response = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(body);
            List<TextElement> elements = null;
            string copyrightHolder = GetCopyright(response).Trim().Replace("\n", " ");

            var res = response["message"];
            if (res["header"]?["status_code"] == 200)
            {
                res = res["body"]?["macro_calls"]?["track.subtitles.get"];
                try
                {
                    if (res != null && res["message"]?["header"]?["status_code"] == 200)
                    {
                        res = res["message"]?["body"]?["subtitle_list"];

                        if (res != null && (res as Newtonsoft.Json.Linq.JArray).Count > 0)
                        {
                            res = res[0]?["subtitle"]?["subtitle_body"];
                            if (res != null)
                            {
                                var resBody = ((string)res);
                                try
                                {
                                    elements = JsonConvert.DeserializeObject<List<MusicmatchLyricMDL>>(resBody)
                                        .Select(x => new TextElement()
                                        {
                                            FontName = Fixed.DefaultFontName,
                                            FontSize = Fixed.DefaultFontSize,
                                            Text = x.text,
                                            StartAt = (uint)(x.time.total * 1000)
                                        }).ToList();

                                    for (int index = 0; index < elements.Count; index++)
                                    {
                                        if (string.IsNullOrEmpty(elements[index].Text?.Trim()))
                                            elements[index].Text = "...";

                                        if (index == elements.Count - 1)
                                            elements[index].Duration = int.MaxValue / 2;
                                        else
                                            elements[index].Duration = elements[index + 1].StartAt - elements[index].StartAt;
                                    }

                                    if (elements[0].StartAt > 1)
                                        elements.Insert(0,
                                         new TextElement()
                                         {
                                             FontName = Fixed.DefaultFontName,
                                             FontSize = Fixed.DefaultFontSize,
                                             Text = "...",
                                             Duration = elements[0].StartAt
                                         });

                                    return CreateTrackLyric(elements, copyrightHolder);
                                }
                                catch { }
                                resBody = resBody.Replace(@"\", string.Empty);

                                elements = JsonConvert.DeserializeObject<List<MusicmatchLyricMDL>>(resBody)
                                   .Select(x => new TextElement()
                                   {
                                       FontName = Fixed.DefaultFontName,
                                       FontSize = Fixed.DefaultFontSize,
                                       Text = x.text,
                                       StartAt = (uint)(x.time.total * 1000)
                                   }).ToList();

                                for (int index = 0; index < elements.Count; index++)
                                {
                                    if (string.IsNullOrEmpty(elements[index].Text?.Trim()))
                                        elements[index].Text = "...";

                                    if (index == elements.Count - 1)
                                        elements[index].Duration = int.MaxValue / 2;
                                    else
                                        elements[index].Duration = elements[index + 1].StartAt - elements[index].StartAt;
                                }
                                if (elements[0].StartAt > 1)
                                    elements.Insert(0, new TextElement()
                                    {
                                        FontName = Fixed.DefaultFontName,
                                        FontSize = Fixed.DefaultFontSize,
                                        Duration = elements[0].StartAt,
                                        Text = "..."
                                    });

                                return CreateTrackLyric(elements, copyrightHolder);
                            }
                        }
                    }
                }
                catch { }

                res = response["message"]?["body"]?["macro_calls"]?["track.lyrics.get"];
                if (res != null && res["message"]?["header"]?["status_code"] == 200)
                {
                    res = res["message"]?["body"]?["crowd_lyrics_list"];
                    if ((res as Newtonsoft.Json.Linq.JArray).Count > 0)
                    {
                        res = res[0]?["lyrics"]?["lyrics_body"];
                        if (res != null)
                        {
                            var resBody = ((string)res).Replace("\\", string.Empty);
                            return CreateTrackLyric(resBody.Split('\n')
                                .Select(x => new TextElement()
                                {
                                    Text = x,
                                    FontName = Fixed.DefaultFontName,
                                    FontSize = Fixed.DefaultFontSize
                                }), copyrightHolder, false);
                        }
                    }
                    else
                    {
                        res = response["message"]?["body"]?["macro_calls"]?["track.lyrics.get"]?["message"]?["body"];
                        res = res["lyrics"]?["lyrics_body"];
                        var resBody = ((string)res).Replace("\\", string.Empty);
                        return CreateTrackLyric(resBody.Split('\n')
                            .Select(x => new TextElement()
                            {
                                Text = x,
                                FontName = Fixed.DefaultFontName,
                                FontSize = Fixed.DefaultFontSize
                            }), copyrightHolder, false);
                    }
                }

                res = response["message"]?["body"]?["macro_calls"]?["track.lyrics.get"];
                //instrumental
                if (res != null && res["message"]?["header"]?["instrumental"] == 1)
                    return CreateTrackLyric(new List<TextElement>()
                    {
                        new TextElement()
                        {
                            Text = "Let the beat goes on (Instrumental)",
                            Duration = int.MaxValue,
                            FontName = Fixed.DefaultFontName,
                            FontSize = Fixed.DefaultFontSize
                        }
                    }, copyrightHolder);

            }
            return CreateTrackLyric(new List<TextElement>());
        }

        public TrackLyric CreateTrackLyric(IEnumerable<TextElement> elements, string copyRightHolder = "", bool synchronized = true)
        {
            if (!Directory.Exists("Lyrics"))
                Directory.CreateDirectory("Lyrics");

            var rootElement = new BasicElement() { BackgroundColor = Color.FromArgb(160, 20, 20, 20), };
            var child = new BasicElement
            {
                Duration = int.MaxValue,
                Dock = ElementDock.Fill,
                Effects = new List<Effect>
                {
                    new ShakeEffect{
                    Duration = int.MaxValue,
                    Trauma = 9,
                    TraumaDecay = 0.000000000001f,
                    TraumaMag = 3.5f,
                    TraumaMult = 2f
                    }
                }
            };

            foreach (var e in elements)
            {
                e.TextColor = Color.Azure;
                e.HorizontalAlignment = TextHorizontalAlignment.Center;
                e.VerticalAlignment = TextVerticalAlignment.Center;
                e.Dock = ElementDock.Fill;
                e.AutoSize = false;
            }
            child.ChildElements.Add(elements);
            rootElement.ChildElements.Add(child);
            var trackLyric = new TrackLyric
            {
                Copyright = copyRightHolder,
                Synchronized = synchronized,
                RootElement = rootElement
            };
            //File.WriteAllText(Path.Combine("Lyrics", TrackName.ReplaceToValidFileName() + ".lyr"), JsonConvert.SerializeObject(trackLyric, Fixed.JsonSerializationSetting));

            return trackLyric;
        }

        private string CallMusicMatchService(string trackName, string title, string album, string artist, double trackLength)
        {
            TrackName = trackName = new string(trackName
                .Replace("[Copyright Free]", "")
                .Replace("[Official Video]", "")
                .Replace("[Official Audio]", "")
                .Replace("[Official Release]", "")
                .Replace("[Monstercat Release]", "")
                .Replace("[NCS Release]", "")
                .Replace("[Ncs Release]", "")
                .Replace("[Dubstep]", "")
                .Replace("[Techno]", "")
                .Replace("[Trap]", "")
                .Where(x => char.IsLetterOrDigit(x) || x == ' ' || x == '-' || x == '(' || x == ')').ToArray()).Replace("  ", "");

            if (!string.IsNullOrEmpty(album))
                album = new string(album
                   .Replace("[Copyright Free]", "")
                   .Replace("[Official Video]", "")
                   .Replace("[Official Audio]", "")
                   .Replace("[Official Release]", "")
                   .Replace("[Monstercat Release]", "")
                   .Replace("[NCS Release]", "")
                   .Replace("[Ncs Release]", "")
                   .Where(x => char.IsLetterOrDigit(x) || x == ' ' || x == '-' || x == '(' || x == ')').ToArray()).Replace("  ", "");

            if (!string.IsNullOrEmpty(artist))
                artist = new string(artist
              .Replace("[Copyright Free]", "")
              .Replace("[Official Video]", "")
              .Replace("[Official Audio]", "")
              .Replace("[Official Release]", "")
              .Replace("[Monstercat Release]", "")
              .Replace("[NCS Release]", "")
              .Replace("[Ncs Release]", "")
              .Where(x => char.IsLetterOrDigit(x) || x == ' ' || x == '-' || x == '(' || x == ')').ToArray()).Replace("  ", "");

            var url = $"https://apic.musixmatch.com/ws/1.1/macro.subtitles.get?" +
                    $"tags=playing&f_subtitle_length={trackLength.ToString()}&q_duration={trackLength.ToString()}" +
                      "&f_subtitle_length_max_deviation=1&subtitle_format=mxm&page_size=1&optional_calls=track.richsync" +
                     $"&q_album={HttpUtility.UrlEncode(album)}&q_artist={HttpUtility.UrlEncode(artist)}&q_track={HttpUtility.UrlEncode(trackName)}&q_album_artist=" +
                     $"&usertoken={UserToken}" +
                     $"&app_id=android-player-v1.0&country=&part=track_artist%2Cartist_image%2Clyrics_crowd%2Cuser%2Clyrics_vote%2Clyrics_poll%2Ctrack_lyrics_translation_status%2Clyrics_verified_by%2C&language_iso_code=1&format=json";

            return SendRequest(url).GetAwaiter().GetResult();
        }
        protected virtual async Task<string> SendRequest(string address)
        {
            try
            {
                return await Client.GetStringAsync(address);
            }
            catch { return string.Empty; }
        }

        private string GetCopyright(Dictionary<string, dynamic> response)
        {
            try
            {
                var res = response["message"];
                if (res["header"]?["status_code"] != 200)
                    return string.Empty;

                res = response["message"]?["body"]?["macro_calls"]?["track.lyrics.get"];

                if (res != null)
                    return res["message"]?["body"]?["lyrics"]?["lyrics_copyright"] ?? string.Empty; ;

                return string.Empty;
            }
            catch { return string.Empty; }
        }
    }
}
