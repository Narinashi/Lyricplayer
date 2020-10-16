using LyricPlayer.LyricEngine;
using LyricPlayer.LyricFetcher;
using LyricPlayer.Models;
using LyricPlayer.PlaylistController;
using LyricPlayer.SoundEngine;
using System;
using System.IO;

namespace LyricPlayer.MusicPlayer
{
    public abstract class MusicPlayer<T> where T : TrackInfo
    {
        public PlayerStatus PlayerStatus => SoundEngine == null ? PlayerStatus.Stopped : SoundEngine.Status;
        public RepeatType RepoeatType { set; get; }
        public TimeSpan CurrentTime
        {
            get => SoundEngine == null ? TimeSpan.Zero : SoundEngine.CurrentTime;
            set
            {
                if (SoundEngine != null && LyricEngine != null)
                {
                    SoundEngine.CurrentTime = value;
                    LyricEngine.CurrentTime = SoundEngine.CurrentTime;
                }
            }
        }
        public T CurrnetTrack => Playlist?.CurrentTrack;

        public PlaylistController<T> Playlist
        {
            protected set
            {
                if (Playlist != null)
                    Playlist.TrackChanged -= TrackChanged;

                _Playlist = value;

                if (Playlist != null)
                    value.TrackChanged += TrackChanged;

            }
            get => _Playlist;
        }
        protected ISoundEngine SoundEngine { set; get; }
        protected ILyricEngine LyricEngine { set; get; }
        protected ILyricFetcher LyricFetcher { set; get; }
        private PlaylistController<T> _Playlist;

        public virtual void Play()
        {
            SoundEngine.Play();

            if (LyricEngine.Status == PlayerStatus.Paused)
                LyricEngine.Resume();
            else
                LyricEngine.Start();

            LyricEngine.CurrentTime = SoundEngine.CurrentTime;
        }
        public virtual void Pause()
        {
            LyricEngine.CurrentTime = SoundEngine.CurrentTime;

            SoundEngine.Pause();
            LyricEngine.Pause();
        }
        public virtual void Stop()
        {
            SoundEngine.Stop();
            LyricEngine.Stop();
        }

        public virtual void Next()
           => Playlist.Next();


        public virtual void Previous()
           => Playlist.Previous();


        private void TrackChanged(object sender, T track)
            => TrackChanged(track);

        protected virtual void TrackChanged(T track)
        {
            Stop();
            var lyric = LyricFetcher.GetLyric(Path.GetFileNameWithoutExtension(track.FileAddress ?? ""), track.Title, track.Album, track.Artists, SoundEngine.TrackLength / 1000);

            LyricEngine.Load(lyric);
            SoundEngine.Load(track);
            Play();
        }

        public abstract void Initialize();
    }
}
