using LyricPlayer.LyricEngine;
using LyricPlayer.LyricFetcher;
using LyricPlayer.Model;
using LyricPlayer.Models;
using LyricPlayer.PlaylistController;
using LyricPlayer.SoundEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                if (SoundEngine != null && LyricEngine != null)
                {
                    SoundEngine.CurrentTime = value;
                    LyricEngine.CurrentTime = SoundEngine.CurrentTime;
                }
            }
        }
        public T CurrnetTrack => Playlist?.CurrentTrack;
        public TrackLyric Lyric => LyricEngine?.TrackLyric;
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
        public ISoundEngine SoundEngine { set; get; }
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
            LyricEngine.Stop();
            SoundEngine.Stop();
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
            SoundEngine.Load(track);
            SoundEngine.Play();

            Task.Run(() =>
            {
                var lyric = LyricFetcher.GetLyric(Path.GetFileNameWithoutExtension(track.FileAddress ?? ""), track.Title, track.Album, track.Artists, SoundEngine.TrackLength / 1000);
                if (lyric.Lyric.Any(x => x.Duration <= 0))
                    LyricEngine.Load(new TrackLyric
                    {
                        Lyric = new List<Lyric>
                        {
                            new Lyric
                            {
                            Duration = int.MaxValue,
                            StartAt = 0,
                            Text ="Lyric not found"
                            }
                        }
                    }, SoundEngine);

                else
                    LyricEngine.Load(lyric, SoundEngine);

                LyricEngine.Start();
                LyricEngine.CurrentTime = SoundEngine.CurrentTime;
            });
        }

        public abstract void Initialize();
    }
}
