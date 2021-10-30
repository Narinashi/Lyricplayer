using LyricPlayer.Model;
using LyricPlayer.PlaylistController;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LyricPlayer.MusicPlayer
{
    public abstract class AudioPlayer : IDisposable
    {
        public abstract PlayerStatus Status { get; protected set; }
        public abstract TimeSpan CurrentTime { set; get; }
        public abstract TrackInfo CurrentlyPlaying { get; protected set; }
        public abstract void LoadTrack(TrackInfo track);
        public abstract float Volume { set; get; }
        public abstract bool Muted { set; get; }
     
        public PlaylistController<TrackInfo> Playlist { set; get; } = new PlaylistController<TrackInfo>();

        public event EventHandler TrackChanged;
        public event EventHandler TrackStopped;

        public abstract void Play();
        public abstract void Pause();
        public abstract void Stop();
        public abstract List<TimeSpectrumData> CalculateSpecterumData(int bands);

        public abstract void Dispose();

        protected virtual void OnTrackChanged()
        {
            TrackChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnTrackStopped()
        {
            TrackStopped?.Invoke(this, EventArgs.Empty);
        }

    }
}
