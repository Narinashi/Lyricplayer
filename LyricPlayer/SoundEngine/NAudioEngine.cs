using LyricPlayer.Models;
using NAudio.Wave;
using System;
using System.IO;

namespace LyricPlayer.SoundEngine
{
    public class NAudioPlayer : ISoundEngine, IDisposable
    {
        public PlayerStatus Status
        {
            get
            {
                if (waveOutEvent == null)
                    return PlayerStatus.Stopped;
                return (PlayerStatus)((int)waveOutEvent.PlaybackState);
            }
        }

        public TimeSpan CurrentTime
        {
            get => fileReader?.CurrentTime ?? TimeSpan.Zero;
            set
            {
                if (fileReader != null)
                    fileReader.CurrentTime = value;
            }
        }
        public Models.FileInfo CurrentFileInfo => _CurrentFileInfo;
        public event EventHandler TrackStopped;
        public int TrackLength => (int)(fileReader?.TotalTime.TotalMilliseconds ?? 0);

        public float Volume
        {
            get => waveOutEvent?.Volume ?? 0;
            set
            {
                if (waveOutEvent != null)
                    waveOutEvent.Volume = value;
                volumeBeforeMute = value;
            }
        }
        public bool Muted
        {
            get => (waveOutEvent?.Volume ?? 0) < Fixed.AlmostZero;
            set
            {
                if (waveOutEvent == null) return;
                waveOutEvent.Volume = value ? 0 : volumeBeforeMute;
            }
        }

        private WaveOutEvent waveOutEvent { set; get; }
        private Mp3FileReader fileReader { set; get; }
        private Models.FileInfo _CurrentFileInfo { set; get; }
        private float volumeBeforeMute { set; get; } = 1.0f;
        private bool stoppedByUser { set; get; } = false;

        public void Load(Models.FileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");
            if (string.IsNullOrEmpty(fileInfo.FileAddress = fileInfo.FileAddress?.Trim()) && fileInfo.FileContent == null)
                throw new ArgumentException("no file content specified");

            volumeBeforeMute = waveOutEvent?.Volume ?? 1f;
            fileReader = fileInfo.FileContent == null ? new Mp3FileReader(fileInfo.FileAddress) : new Mp3FileReader(new MemoryStream(fileInfo.FileContent));

            if (waveOutEvent == null)
            {
                waveOutEvent = new WaveOutEvent();
                waveOutEvent.PlaybackStopped += WaveOutEventPlaybackStopped;
            }

            waveOutEvent.Init(fileReader);
            
            Volume = volumeBeforeMute;
            _CurrentFileInfo = fileInfo;
        }

        public void Pause()
        {
            if (waveOutEvent == null) return;
            waveOutEvent.Pause();
        }
        public void Resume()
        {
            if (waveOutEvent == null) return;
            waveOutEvent.Play();
        }
        public void Play()
        {
            if (waveOutEvent == null) return;
            waveOutEvent.Play();
        }

        public void Stop()
        {
            if (waveOutEvent == null) return;
            stoppedByUser = true;
            waveOutEvent.Stop(); 
        }

        public void Dispose()
        {
            if (waveOutEvent != null)
            {
                waveOutEvent.PlaybackStopped -= WaveOutEventPlaybackStopped;
                waveOutEvent.Dispose();
            }
            fileReader?.Dispose();
            waveOutEvent = null;
            fileReader = null;
            _CurrentFileInfo = null;
        }

        private void WaveOutEventPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if(stoppedByUser)
            {
                stoppedByUser = false;
                return;
            }
            TrackStopped?.Invoke(this, EventArgs.Empty);
        }
    }
}
