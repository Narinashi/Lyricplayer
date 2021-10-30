using LyricPlayer.Model;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace LyricPlayer.MusicPlayer
{
	public class NAudioPlayer : AudioPlayer
	{
		static long OffsetWithAudioPlayer = 0;
		static Stopwatch Watcher = Stopwatch.StartNew();
		public override PlayerStatus Status
		{
			get
			{
				if (WaveOutEvent == null)
					return PlayerStatus.Stopped;
				return (PlayerStatus)((int)WaveOutEvent.PlaybackState);
			}
			protected set
			{
				throw new NotImplementedException();
			}
		}

		public override TimeSpan CurrentTime
		{
			get
			{
				var playerTime = FileReader?.CurrentTime ?? TimeSpan.Zero;
				var currentTime = Watcher.ElapsedMilliseconds + OffsetWithAudioPlayer;
				if (Math.Abs(currentTime - playerTime.TotalMilliseconds) > 500 || Status != PlayerStatus.Playing)
					OffsetWithAudioPlayer = (long)(playerTime.TotalMilliseconds - Watcher.ElapsedMilliseconds);

				currentTime = Watcher.ElapsedMilliseconds + OffsetWithAudioPlayer;
				return TimeSpan.FromMilliseconds(currentTime);
			}
			set
			{
				if (FileReader != null)
					FileReader.CurrentTime = value;
			}
		}

		public override float Volume
		{
			get => WaveOutEvent?.Volume ?? 0;
			set
			{
				if (WaveOutEvent != null)
					WaveOutEvent.Volume = value;
				VolumneBeforeMute = value;
			}
		}
		public override bool Muted
		{
			get => (WaveOutEvent?.Volume ?? 0) < Fixed.AlmostZero;
			set
			{
				if (WaveOutEvent == null) return;
				WaveOutEvent.Volume = value ? 0 : VolumneBeforeMute;
			}
		}
		public override TrackInfo CurrentlyPlaying { protected set; get; }

		public WaveOutEvent WaveOutEvent { private set; get; }
		public Mp3FileReader FileReader { private set; get; }

		private float VolumneBeforeMute { set; get; } = 0.3f;

		private AudioAnalyse.RawSpectrumDataProvider SpectrumProvider = new AudioAnalyse.RawSpectrumDataProvider();

		public NAudioPlayer()
		{
			Playlist.TrackChanged += (_, e) => LoadTrack(e);
		}

		public override void LoadTrack(TrackInfo trackInfo)
		{
			if (trackInfo == null)
				throw new ArgumentNullException("fileInfo");
			if (string.IsNullOrEmpty(trackInfo.FileAddress = trackInfo.FileAddress?.Trim()) && trackInfo.FileContent == null)
				throw new ArgumentException("no file content specified");

			VolumneBeforeMute = WaveOutEvent?.Volume ?? 0.3f;
			FileReader = trackInfo.FileContent == null ? new Mp3FileReader(trackInfo.FileAddress) : new Mp3FileReader(new MemoryStream(trackInfo.FileContent));

			if (WaveOutEvent == null)
			{
				WaveOutEvent = new WaveOutEvent();
				WaveOutEvent.PlaybackStopped += WaveOutEventPlaybackStopped;
			}

			if (WaveOutEvent.PlaybackState == PlaybackState.Playing)
				Stop();

			WaveOutEvent.Init(FileReader);

			Volume = VolumneBeforeMute;
			trackInfo.TrackLength = FileReader.TotalTime.TotalMilliseconds;

			if (trackInfo != CurrentlyPlaying)
			{
				CurrentlyPlaying = trackInfo;
				SpectrumProvider = new AudioAnalyse.RawSpectrumDataProvider();
				OnTrackChanged();
			}
			Console.WriteLine($"loaded track {trackInfo.TrackName}");
			Watcher.Restart();
			CurrentlyPlaying = trackInfo;
			Play();
		}

		public override void Pause()
		{
			if (WaveOutEvent == null) return;
			WaveOutEvent.Pause();
		}

		public override void Play()
		{
			if (WaveOutEvent == null) return;
			WaveOutEvent.Play();
		}

		bool StoppedByUser = false;
		public override void Stop()
		{
			if (WaveOutEvent == null || WaveOutEvent.PlaybackState == PlaybackState.Stopped) return;
			StoppedByUser = true;
			WaveOutEvent.Stop();
		}

		public override List<TimeSpectrumData> CalculateSpecterumData(int bands)
		{
			Console.WriteLine($"CalculateSpecterumData called for {CurrentlyPlaying.TrackName}");
			SpectrumProvider.LoadFile(CurrentlyPlaying.FileAddress, bands);
			return SpectrumProvider.GenerateSpectrumDataSafeWay(4, 13);
		}

		public override void Dispose()
		{
			if (WaveOutEvent != null)
			{
				WaveOutEvent.PlaybackStopped -= WaveOutEventPlaybackStopped;
				WaveOutEvent.Dispose();
			}
			FileReader?.Dispose();
			WaveOutEvent = null;
			FileReader = null;
			CurrentlyPlaying = null;
		}

		private void WaveOutEventPlaybackStopped(object sender, StoppedEventArgs e)
		{
			OnTrackStopped();
			if (!StoppedByUser)
				Playlist.Next();
			
			StoppedByUser = false;
		}
	}
}
