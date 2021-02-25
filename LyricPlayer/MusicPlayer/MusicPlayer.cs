using LyricPlayer.LyricFetcher;
using LyricPlayer.Model;
using LyricPlayer.PlaylistController;
using LyricPlayer.SoundEngine;
using System;

namespace LyricPlayer.MusicPlayer
{
    public abstract class MusicPlayer<T> where T : TrackInfo
    {
        public PlayerStatus PlayerStatus => SoundEngine == null ? PlayerStatus.Stopped : SoundEngine.Status;
        public RepeatType Repeat { set; get; }
        public TimeSpan CurrentTime
        {
            get => SoundEngine == null ? TimeSpan.Zero : SoundEngine.CurrentTime;
            set
            {
                if (SoundEngine != null)
                    SoundEngine.CurrentTime = value;
            }
        }
        public T CurrentTrack => Playlist?.CurrentTrack;
        public PlaylistController<T> Playlist
        {
            protected set
            {
                if (Playlist != null)
                    Playlist.TrackChanged -= PlaylistTrackChanged;

                _Playlist = value;

                if (Playlist != null)
                    value.TrackChanged += PlaylistTrackChanged;

            }
            get => _Playlist;
        }
        public ISoundEngine SoundEngine { protected set; get; }
        protected ILyricFetcher LyricFetcher { set; get; }
        public TrackLyric CurrentTrackLyric { set; get; }
        private PlaylistController<T> _Playlist;

        public event EventHandler TrackChanged;

        public virtual void Play()
        {
            SoundEngine.Play();
        }
        public virtual void Pause()
        {
            SoundEngine.Pause();
        }
        public virtual void Stop()
        {
            SoundEngine.Stop();
        }

        public virtual void Next()
           => Playlist.Next();


        public virtual void Previous()
           => Playlist.Previous();


        private void PlaylistTrackChanged(object sender, T track)
            => PlaylistTrackChanged(track);

        protected virtual void PlaylistTrackChanged(T track)
        {
            Stop();
            SoundEngine.Load(track);
            SoundEngine.Play();
            OnTrackChanged();
        }

        protected virtual void OnTrackChanged()
        {
            TrackChanged?.Invoke(this, EventArgs.Empty);
        }
        public abstract void Initialize();
    }
}
