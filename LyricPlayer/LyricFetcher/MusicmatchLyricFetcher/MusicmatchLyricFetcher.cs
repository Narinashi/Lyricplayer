using LyricPlayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace LyricPlayer.LyricFetcher.MusicmatchLyricFetcher
{
    class MusicmatchLyricFetcher : ILyricFetcher
    {
        static HttpClientHandler Handler = new HttpClientHandler();
        static HttpClient Client = new HttpClient(Handler);

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
                return new TrackLyric
                {
                    Synchronized = true,
                    Lyric = new List<Lyric> { new Lyric { Duration = int.MaxValue, Text = "Lyric not found" } }
                };

            var response = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(body);
            List<Lyric> lyric = null;
            var res = response["message"];
            var bkRes = response["message"];
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
                                    lyric = JsonConvert.DeserializeObject<List<MusicmatchLyricMDL>>(resBody)
                                    .Select(x => new Lyric
                                    {
                                        Text = x.text,
                                        StartAt = (int)(x.time.total * 1000)
                                    }).ToList();

                                    for (int index = 0; index < lyric.Count; index++)
                                    {
                                        if (string.IsNullOrEmpty(lyric[index].Text?.Trim()))
                                            lyric[index].Text = "...";

                                        if (index == lyric.Count - 1)
                                            lyric[index].Duration = int.MaxValue;
                                        else
                                            lyric[index].Duration = lyric[index + 1].StartAt - lyric[index].StartAt;
                                    }

                                    if (lyric[0].StartAt > 1)
                                        lyric.Insert(0, new Lyric { Duration = lyric[0].StartAt, Text = "..." });

                                    lyric.Add(new Lyric { Duration = int.MaxValue, Text = "..." });
                                    return new TrackLyric
                                    {
                                        Synchronized = true,
                                        Lyric = lyric
                                    };
                                }
                                catch { }
                                resBody = resBody.Replace(@"\", string.Empty);

                                lyric = JsonConvert.DeserializeObject<List<MusicmatchLyricMDL>>(resBody)
                                   .Select(x => new Lyric
                                   {
                                       Text = x.text,
                                       StartAt = (int)(x.time.total * 1000)
                                   }).ToList();

                                for (int index = 0; index < lyric.Count; index++)
                                {
                                    if (string.IsNullOrEmpty(lyric[index].Text?.Trim()))
                                        lyric[index].Text = "...";

                                    if (index == lyric.Count - 1)
                                        lyric[index].Duration = int.MaxValue;
                                    else
                                        lyric[index].Duration = lyric[index + 1].StartAt - lyric[index].StartAt;
                                }
                                if (lyric[0].StartAt > 1)
                                    lyric.Insert(0, new Lyric { Duration = lyric[0].StartAt, Text = "..." });

                                lyric.Add(new Lyric { Duration = int.MaxValue, Text = "..." });

                                return new TrackLyric
                                {
                                    Synchronized = true,
                                    Lyric = lyric
                                };
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
                            return new TrackLyric
                            {
                                Synchronized = false,
                                Lyric = resBody.Split('\n').Select(x => new Lyric { Text = x }).ToList()
                            };
                        }
                    }
                    else
                    {
                        res = response["message"]?["body"]?["macro_calls"]?["track.lyrics.get"]?["message"]?["body"];
                        res = res["lyrics"]?["lyrics_body"];
                        var resBody = ((string)res).Replace("\\", string.Empty);

                        return new TrackLyric
                        {
                            Synchronized = false,
                            Lyric = resBody.Split('\n').Select(x => new Lyric { Text = x }).ToList()
                        };
                    }
                }

                res = response["message"]?["body"]?["macro_calls"]?["track.lyrics.get"];
                //instrumental
                if (res != null && res["message"]?["header"]?["instrumental"] == 1)
                    return new TrackLyric
                    {
                        Synchronized = true,
                        Lyric = new List<Lyric>
                            {
                                new Lyric
                                {
                                    Text ="Let the beat goes on (Instrumental)",
                                    Duration = int.MaxValue
                                }
                            }
                    };
            }
            return new TrackLyric
            {
                Synchronized = true,
                Lyric = new List<Lyric> { new Lyric { Duration = int.MaxValue, Text = "Lyric not found" } }
            };
        }

        private string CallMusicMatchService(string trackName, string title, string album, string artist, double trackLength)
        {
            trackName = new string(trackName
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


            try
            {
                return Client.GetStringAsync($"https://apic.musixmatch.com/ws/1.1/macro.subtitles.get?" +
                    $"tags=playing&f_subtitle_length={trackLength.ToString()}&q_duration={trackLength.ToString()}" +
                      "&f_subtitle_length_max_deviation=1&subtitle_format=mxm&page_size=1&optional_calls=track.richsync" +
                     $"&q_album={album}&q_artist={artist}&q_track={trackName}&q_album_artist=" +
                     $"&usertoken={UserToken}" +
                     $"&app_id=android-player-v1.0&country=&part=track_artist%2Cartist_image%2Clyrics_crowd%2Cuser%2Clyrics_vote%2Clyrics_poll%2Ctrack_lyrics_translation_status%2Clyrics_verified_by%2C&language_iso_code=1&format=json").Result;
            }
            catch { return string.Empty; }
        }
    }
}
