using LyricPlayer.Model;
using LyricPlayer.Model.Effects;
using LyricPlayer.Model.Elements;
using LyricPlayer.Model.Elements.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;


namespace LyricPlayer.LyricFetcher.MusicmatchLyricFetcher
{
	public class MusicxMatchLyricFetcher : LyricFetcher
	{
		static HttpClientHandler Handler { set; get; } = new HttpClientHandler();
		static HttpClient Client { set; get; } = new HttpClient(Handler);

		static string UserToken => ConfigurationManager.AppSettings["MusicxMatchToken"] ?? throw new InvalidOperationException("MusicxMatch Usertoken must be present in application config file");
		static string CaptchaToken => ConfigurationManager.AppSettings["MusicxMatchCaptcha"] ?? string.Empty;

		static bool Initialized = false;

		public override TrackLyric GetLyric(TrackInfo trackInfo)
		{
			var lyricBody = FetchLyric(trackInfo);
			if (string.IsNullOrEmpty(lyricBody))
				return new TrackLyric { Synchronized = false, RootElement = new TextElement { Text = "Lyric not found", Duration = uint.MaxValue / 2 } };

			var trackLyric = CreateTrackLyrics(trackInfo, lyricBody);
			return trackLyric;
		}

		protected string FetchLyric(TrackInfo trackInfo)
		{
			RemoveExtraWords(trackInfo);
			Initialize();

			var url = $"https://apic.musixmatch.com/ws/1.1/macro.subtitles.get?" +
				  $"tags=playing&f_subtitle_length={trackInfo.TrackLength / 1000f}&q_duration={trackInfo.TrackLength / 1000f}" +
					"&f_subtitle_length_max_deviation=2&subtitle_format=mxm&page_size=1&optional_calls=track.richsync" +
				   $"&q_album={HttpUtility.UrlEncode(trackInfo.AlbumName ?? "")}&q_artist={HttpUtility.UrlEncode(trackInfo.ArtistName ?? "")}&q_track={HttpUtility.UrlEncode(trackInfo.TrackName)}&q_album_artist=" +
				   $"&usertoken={UserToken}" +
				   $"&app_id=android-player-v1.0&country=&part=track_artist%2C&language_iso_code=1&format=json";

			using (var response = Client.GetAsync(url).Result)
			{
				if ((int)response.StatusCode > 200 || (int)response.StatusCode < 200)
				{
					Logger.Error($"Received status code {response.StatusCode}, by calling \n {url}");
					Logger.Error($"Response body:\n{ response.Content.ReadAsStringAsync().Result}");
					return string.Empty;
				}
				return response.Content.ReadAsStringAsync().Result;
			}
		}

		protected TrackLyric CreateTrackLyrics(TrackInfo trackInfo, string lyricBody)
		{
			lyricBody = lyricBody.Replace("[]", "null");

			var musicXMatchTrackLyric = JsonConvert.DeserializeObject<TrackLyricResponse>(lyricBody);
			if (musicXMatchTrackLyric?.Message.Header.StatusCode != 200)
			{
				Logger.Error($"requesting from MusixMatch retunred {musicXMatchTrackLyric.Message.Header.StatusCode} code");
				return null;
			}

			var lyricLines = GetLyricLines(musicXMatchTrackLyric);
			var trackLyric = new TrackLyric
			{
				Copyright = GetCopyrightHolder(musicXMatchTrackLyric).Trim(),
				RootElement = new BasicElement
				{
					Dock = ElementDock.Fill,
					StartAt = 0,
					Duration = uint.MaxValue - 1,
					//BackgroundColor = Color.FromArgb(180, 30, 30, 30)
				},
			};
			trackLyric.RootElement.ChildElements.Add(new SpectrumVisualizer
			{
				Dock = ElementDock.Fill,
				Duration = uint.MaxValue - 1,
				//BandColor = Fixed.SpectrumBandColor,
				//BandColor = Color.FromArgb(255, 60, 140, 255),
				BandColor = Color.FromArgb(255, 255, 60, 60),
				BandCount = 1080 / 34 + 1,
				Size = new Point(150, 0),
				BandWidth = 27,
				BandSpace = 7,
				BandRadius = 8,
				Multiplier = 0.06f,
				VisualizationType = SpectrumVisualizationType.Right
			});

			trackLyric.RootElement.ChildElements.Add(new SpectrumVisualizer
			{
				Dock = ElementDock.None,
				Duration = uint.MaxValue - 1,
				//BandColor = Color.FromArgb(255, 60, 140, 255),
				BandColor = Color.FromArgb(255, 255, 150, 252),
				BandCount = 1080 / 34 + 1,
				Size = new Point(1920, 1080),
				BandWidth = 19,
				Location = new FloatPoint(0, 0),
				BandSpace = 15,
				BandRadius = 8,
				Multiplier = 0.059f,
				VisualizationType =  SpectrumVisualizationType.Right
			});

			//trackLyric.RootElement.ChildElements.Add(new SpectrumVisualizer
			//{
			//	Dock = ElementDock.None,
			//	Duration = uint.MaxValue - 1,
			//	BandColor = Color.FromArgb(255, 255, 60, 60),
			//	BandCount = 1080 / 14,
			//	Size = new Point(1920, 1080),
			//	BandWidth = 3,
			//	Location = new FloatPoint(0, -4),
			//	BandSpace = 11,
			//	BandRadius = 3,
			//	Multiplier = 0.025f,
			//	VisualizationType = SpectrumVisualizationType.Left | SpectrumVisualizationType.Right
			//});


			var secondaryRoot = new BasicElement
			{
				Dock = ElementDock.Fill,
				StartAt = 0,
				Duration = int.MaxValue,
				Effects = new List<ElementEffect>
				{
					new ShakeEffect
					{
						Duration = int.MaxValue,
						Trauma = 3.2f,
						TraumaDecay = 0.000000000001f,
						TraumaMag = 20f,
						TraumaMult = 4f
					}
				}
			};
			var cover = new BasicElement
			{
				StartAt = 0,
				Duration = int.MaxValue / 2,
				BackgroundColor = Color.FromArgb(255, 18, 18, 18),
				Size = new Point(560, 250),
				Location = new FloatPoint(10, 780),
			};
			cover.ChildElements.Add(secondaryRoot);
			lyricLines.ForEach(x => (x as TextElement).Text = (x as TextElement).Text.Replace("Fuck", "****").Replace("fuck", "****").Replace("Shit", "****").Replace("shit", "****").Replace("bitch","*****"));
			secondaryRoot.ChildElements.Add(lyricLines);
			trackLyric.RootElement.ChildElements.Add(cover);
			return trackLyric;
		}

		private TrackInfo RemoveExtraWords(TrackInfo trackInfo)
		{
			trackInfo.TrackName = new string(trackInfo.TrackName
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

			if (!string.IsNullOrEmpty(trackInfo.AlbumName))
				trackInfo.AlbumName = new string(trackInfo.AlbumName
				   .Replace("[Copyright Free]", "")
				   .Replace("[Official Video]", "")
				   .Replace("[Official Audio]", "")
				   .Replace("[Official Release]", "")
				   .Replace("[Monstercat Release]", "")
				   .Replace("[NCS Release]", "")
				   .Replace("[Ncs Release]", "")
				   .Where(x => char.IsLetterOrDigit(x) || x == ' ' || x == '-' || x == '(' || x == ')').ToArray()).Replace("  ", "");

			if (!string.IsNullOrEmpty(trackInfo.ArtistName))
				trackInfo.ArtistName = new string(trackInfo.ArtistName
			  .Replace("[Copyright Free]", "")
			  .Replace("[Official Video]", "")
			  .Replace("[Official Audio]", "")
			  .Replace("[Official Release]", "")
			  .Replace("[Monstercat Release]", "")
			  .Replace("[NCS Release]", "")
			  .Replace("[Ncs Release]", "")
			  .Where(x => char.IsLetterOrDigit(x) || x == ' ' || x == '-' || x == '(' || x == ')').ToArray()).Replace("  ", "");

			return trackInfo;
		}

		private List<RenderElement> GetLyricLines(TrackLyricResponse trackLyrics)
		{
			List<TextElement> lines = null;

			if (!string.IsNullOrEmpty(trackLyrics.Message.Body.MacroCalls.TrackRichsync?.Message?.Body?.Richsync?.RichsyncBody))
			{
				var richSync = trackLyrics.Message.Body.MacroCalls.TrackRichsync.Message.Body.Richsync;
				var richLyrics = JsonConvert.DeserializeObject<List<TrackRichSyncLyric>>(richSync.RichsyncBody);

				lines = richLyrics.Select(x => new TextElement
				{
					Text = x.Lyric,
					StartAt = (uint)(x.StartAt * 1000),
				}).ToList();
			}

			else if ((trackLyrics.Message.Body?.MacroCalls?.TrackSubtitle?.Message.Body?.SubtitleList.Any() ?? false))
			{
				var subtitle = trackLyrics.Message.Body.MacroCalls.TrackSubtitle.Message.Body.SubtitleList.First();
				if (!string.IsNullOrEmpty(subtitle.Subtitle.SubtitleBody))
				{
					var lyricSubtitle = JsonConvert.DeserializeObject<List<LyricSubtitle>>(subtitle.Subtitle.SubtitleBody);
					lines = lyricSubtitle.Select(x => new TextElement
					{
						StartAt = (uint)(x.Time.Total * 1000),
						Text = x.Text
					}).ToList();
				}
			}

			else if (trackLyrics.Message.Body?.MacroCalls?.TrackLyric?.Message.Body?.Lyrics != null)
			{
				var lyrics = trackLyrics.Message.Body.MacroCalls.TrackLyric.Message.Body.Lyrics;
				if (string.IsNullOrEmpty(lyrics.LyricsBody))
					return lines?.Select(x => x as RenderElement).ToList() ?? new List<RenderElement>();

				lines = lyrics.LyricsBody
					.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => new TextElement
					{
						Text = x.Trim(),
					}).ToList();
			}

			lines = lines ?? new List<TextElement>();
			for (int i = 0; i < lines.Count; i++)
			{
				lines[i].Duration = (i == lines.Count - 1) ?
					  uint.MaxValue / 2 - lines[i - 1].EndAt :
					  lines[i + 1].StartAt - lines[i].StartAt;

				lines[i].AutoSize = false;
				lines[i].FontName = Fixed.DefaultFontName;
				lines[i].FontSize = Fixed.DefaultFontSize;
				lines[i].Dock = ElementDock.Fill;
				lines[i].HorizontalAlignment = TextHorizontalAlignment.Center;
				lines[i].VerticalAlignment = TextVerticalAlignment.Center;
			}

			return lines.Select(x => x as RenderElement).ToList();
		}

		private string GetCopyrightHolder(TrackLyricResponse trackLyrics)
		{
			var lyrics = trackLyrics.Message.Body?.MacroCalls?.TrackLyric?.Message?.Body?.Lyrics;
			return lyrics?.LyricsCopyright ?? string.Empty;
		}

		private static void Initialize()
		{
			if (Initialized)
				return;

			Initialized = true;
			Handler.AllowAutoRedirect = true;
			Handler.UseCookies = true;

			if (!string.IsNullOrWhiteSpace(CaptchaToken))
			{
				Handler.CookieContainer = new CookieContainer();
				Handler.CookieContainer.Add(new Cookie("captcha_id", CaptchaToken, "/", "apic.musixmatch.com"));
			}
			else
			{
				Logger.Warning("Using MusicxMatch api without a Captcha token, you'll probably hit ratelimit soon");
			}
		}
	}
}
