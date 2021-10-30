using LyricPlayer.Model;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.MusicPlayer
{
    public class SpotifyPlayer : AudioPlayer
    {
        public override PlayerStatus Status { get; protected set; }
        public override TimeSpan CurrentTime
        {
            get => TimeSpan.FromMilliseconds(Tracker.ElapsedMilliseconds + WatcherOffset);
            set => throw new NotImplementedException();
        }

        public override float Volume
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public override bool Muted
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public FullTrack TrackInfo { private set; get; }
        public override TrackInfo CurrentlyPlaying { get; protected set; }

        private static string SpotifyRefreshToken => ConfigurationManager.AppSettings["SpotifyRefreshToken"] ?? throw new InvalidOperationException("To use spotify api, u need to provide a refresh token through app config");
        private static string SpotifyClientID => ConfigurationManager.AppSettings["SpotifyClientID"] ?? throw new InvalidOperationException("To use spotify api, u need to provide both Client and ClientSeceret through app config");
        private static string SpotifyClientSecret => ConfigurationManager.AppSettings["SpotifyClientSecret"] ?? throw new InvalidOperationException("To use spotify api, u need to provide both Client and ClientSeceret through app config");
        private static uint SongTrackingInterval => Math.Min(uint.Parse(ConfigurationManager.AppSettings["SongTrackingInterval"] ?? "800"), 800);

        private System.Timers.Timer SongTrackingTimer { set; get; } = new System.Timers.Timer();
        private static System.Timers.Timer AccessTokenRefreshTimer { set; get; } = new System.Timers.Timer();

        private static HttpClient Client { set; get; } = new HttpClient();
        private static string AccessToken { set; get; } = string.Empty;
        private static SpotifyClient SpotifyClient { set; get; }

        private long WatcherOffset { set; get; }
        private Stopwatch Tracker { set; get; } = new Stopwatch();

        private readonly PlayerCurrentlyPlayingRequest Request =
            new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track);
        static bool Initialized = false;
        public SpotifyPlayer()
        {
            Initialize();
            WatcherOffset = 0;

            SongTrackingTimer.Interval = SongTrackingInterval;
            SongTrackingTimer.Elapsed += (s, e) => UpdateSongProgression();
            SongTrackingTimer.Start();
        }

        public override void LoadTrack(TrackInfo track)
        {
            throw new NotImplementedException();
        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override void Play()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            SongTrackingTimer?.Dispose();
            Tracker?.Stop();
            Tracker = null;
            CurrentlyPlaying = null;
            TrackInfo = null;
        }

        private void UpdateSongProgression()
        {
            SongTrackingTimer.Interval = SongTrackingInterval;
            SongTrackingTimer.Stop();
            if (SpotifyClient == null)
            {
                SongTrackingTimer.Start();
                return;
            }

            var timeBeforeRequesting = Tracker.ElapsedMilliseconds;
            CurrentlyPlaying currentPlaying = null;
            try
            {
                currentPlaying = SpotifyClient.Player.GetCurrentlyPlaying(Request).Result;
            }
            catch (Exception ex)
            {
                Logger.Error($"error while getting CurrentlyPlaying in SpotifyPlayer \n{ex.ToString()}");
            }

            if (currentPlaying == null)
            {
                SongTrackingTimer.Start();
                return;
            }

            var delta = (int)((Tracker.ElapsedMilliseconds - timeBeforeRequesting) / 2f);

            if (currentPlaying.IsPlaying)
            {
                Status = PlayerStatus.Playing;
                WatcherOffset = ((int)currentPlaying.ProgressMs) - Tracker.ElapsedMilliseconds + delta;
                Tracker.Start();
            }
            else
            {
                Tracker.Reset();
                Status = PlayerStatus.Paused;
                WatcherOffset = ((int)currentPlaying.ProgressMs) - Tracker.ElapsedMilliseconds;

                if (CurrentTime.TotalMilliseconds + 1000 >= CurrentlyPlaying?.TrackLength)
                {
                    Status = PlayerStatus.Stopped;
                    OnTrackStopped();
                }
            }

            SongTrackingTimer.Start();

            if (currentPlaying?.Item is FullTrack track && track.Id != TrackInfo?.Id)
            {
                TrackInfo = track;
                CurrentlyPlaying = new TrackInfo
                {
                    AlbumName = track.Album?.Name ?? "",
                    ArtistName = track.Artists.FirstOrDefault()?.Name ?? "",
                    TrackName = track.Name,
                    TrackLength = track.DurationMs,
                    SpotifyTrackID = track.Id,
                };
                OnTrackChanged();
            }
        }

        private static void Initialize()
        {
            if (Initialized)
                return;
            Initialized = true;

            RefreshAccessToken();
            AccessTokenRefreshTimer.Elapsed += (s, e) => RefreshAccessToken();

            AccessTokenRefreshTimer.Start();
        }

        private static async void RefreshAccessToken()
        {
            AccessTokenRefreshTimer.Interval = 3200000;

            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(
                    $"{SpotifyClientID}:{SpotifyClientSecret}")));

            using (var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type","refresh_token"),
                new KeyValuePair<string, string>("refresh_token",SpotifyRefreshToken),
            }))
            using (var response = await Client.PostAsync("https://accounts.spotify.com/api/token", content))
            {
                if ((int)response.StatusCode == 429)
                {
                    await Task.Delay(int.Parse(response.Headers.FirstOrDefault(x => x.Key == "Retry-After").Value.FirstOrDefault() ?? "5000") + 1500);
                    RefreshAccessToken();
                    return;
                }

                var jsonBody = await response.Content.ReadAsStringAsync();
                var body = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonBody);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    AccessToken = body["access_token"];
                    SpotifyClient = new SpotifyClient(AccessToken);
                }
            }
        }

        public override List<TimeSpectrumData> CalculateSpecterumData(int bands) => null;
    }
}
