using LyricPlayer.LyricEngine;
using LyricPlayer.Model;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Timer = System.Timers.Timer;

namespace LyricPlayer.SoundEngine
{
    public class SpotifyEngine : ISoundEngine
    {
        private Timer SongTrackingTimer { set; get; }
        private Timer AccessTokenRefreshTimer { set; get; }
        private static HttpClient Client { set; get; }
        private static string AccessToken { set; get; }
        private static SpotifyClient SpotifyClient { set; get; }
        private long Offset { set; get; }
        private Stopwatch Tracker { set; get; }
        public FullTrack TrackInfo { set; get; }
        ILyricEngine LyricEngine { set; get; }

        public SpotifyEngine()
        {
            Tracker = new Stopwatch();
            SongTrackingTimer = new Timer();
            Client = new HttpClient();
            AccessTokenRefreshTimer = new Timer();
            Offset = 0;
            AccessToken = string.Empty;
            SongTrackingTimer.Elapsed += (s, e) => UpdateSongProgression(); ;
            AccessTokenRefreshTimer.Elapsed += (s, e) => RefreshAccessToken();
            RefreshAccessToken();
            AccessTokenRefreshTimer.Start();
            SongTrackingTimer.Start();
        }

        public SpotifyEngine(ILyricEngine lyricEngine) : this()
        {
            LyricEngine = lyricEngine;
        }

        public PlayerStatus Status { get; private set; }

        public TimeSpan CurrentTime
        {
            get => TimeSpan.FromMilliseconds(Tracker.ElapsedMilliseconds + Offset);
            set
            {
                if (SpotifyClient != null)
                    SpotifyClient.Player.SeekTo(new PlayerSeekToRequest((long)value.TotalMilliseconds));
            }
        }

        public Model.FileInfo CurrentFileInfo { get; private set; }

        public int TrackLength { get; private set; }

        public float Volume
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
        public bool Muted
        {
            get => Volume < Fixed.AlmostZero;
            set => throw new NotImplementedException();
        }

        public event EventHandler TrackStopped;

        public void Load(Model.FileInfo trackInfo)
        { }

        public void Pause()
        {
            if (SpotifyClient.Player.PausePlayback().Result)
                Status = PlayerStatus.Paused;
        }

        public void Play()
        {
            if (SpotifyClient.Player.ResumePlayback().Result)
                Status = PlayerStatus.Playing;
        }

        public void Resume()
        {
            if (SpotifyClient.Player.ResumePlayback().Result)
                Status = PlayerStatus.Playing;
        }

        public void Stop()
        {
            if (SpotifyClient.Player.PausePlayback().Result && SpotifyClient.Player.SeekTo(new PlayerSeekToRequest(0)).Result)
            {
                Status = PlayerStatus.Stopped;
                TrackStopped?.Invoke(this, EventArgs.Empty);
            }
        }


        private void UpdateSongProgression()
        {
            SongTrackingTimer.Interval = Fixed.SongTrackingInterval;
            SongTrackingTimer.Stop();

            var request = new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track);
            var timeBeforeRequesting = Tracker.ElapsedMilliseconds;
            var currentPlaying = SpotifyClient.Player.GetCurrentlyPlaying(request).Result;
            if (currentPlaying != null)
            {
                var delta = (int)((Tracker.ElapsedMilliseconds - timeBeforeRequesting) / 4f);

                if (currentPlaying.IsPlaying)
                {
                    Status = PlayerStatus.Playing;
                    Offset = ((int)currentPlaying.ProgressMs) - Tracker.ElapsedMilliseconds + delta;
                    Tracker.Start();
                    if (LyricEngine?.Status != LyricPlayerStaus.Loading)
                        LyricEngine?.Resume();
                }
                else
                {
                    Tracker.Reset();
                    Status = PlayerStatus.Paused;
                    Offset = ((int)currentPlaying.ProgressMs) - Tracker.ElapsedMilliseconds;

                    if (LyricEngine?.Status != LyricPlayerStaus.Loading)  
                        LyricEngine?.Pause();
                }
                if (LyricEngine != null)
                    LyricEngine.CurrentTime = CurrentTime;

                Console.WriteLine($"finalTime:{CurrentTime} SpotifyTrack:{TimeSpan.FromMilliseconds((int)currentPlaying.ProgressMs)} Offset:{Offset} Delta:{delta}");
            }
            SongTrackingTimer.Start();

            if (currentPlaying?.Item is FullTrack track &&
                track.Name != (CurrentFileInfo as TrackInfo)?.TrackName)
            {
                TrackInfo = track;
                CurrentFileInfo = new TrackInfo
                {
                    Album = track.Album?.Name ?? "",
                    Artists = track.Artists.FirstOrDefault()?.Name ?? "",
                    Title = track.Name,
                    TrackName = track.Name
                };
                TrackStopped?.Invoke(this, EventArgs.Empty);
            }
        }

        private void RefreshAccessToken()
        {
            AccessTokenRefreshTimer.Interval = 3200000;
            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(
                    File.ReadAllText("SpotifyCredentials.Token"))));

            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type","refresh_token"),
                new KeyValuePair<string, string>("refresh_token",File.ReadAllText("SpotifyToken.Token")),
            });

            var response = Client.PostAsync("https://accounts.spotify.com/api/token", content).Result;

            if ((int)response.StatusCode == 429)
            {
                Thread.Sleep(int.Parse(response.Headers.FirstOrDefault(x => x.Key == "Retry-After").Value.FirstOrDefault() ?? "5000") + 1000);
                RefreshAccessToken();
                return;
            }

            var jsonBody = response.Content.ReadAsStringAsync().Result;
            var body = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonBody);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            { return; }

            AccessToken = body["access_token"];
            SpotifyClient = new SpotifyClient(AccessToken);
        }
    }
}
