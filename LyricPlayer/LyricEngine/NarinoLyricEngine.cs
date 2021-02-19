using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using LyricPlayer.SoundEngine;
using LyricPlayer.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace LyricPlayer.LyricEngine
{
    public class NarinoLyricEngine : ILyricEngine, IDisposable
    {
        public LyricPlayerStaus Status { get; private set; }
        public TrackLyric TrackLyric { get; protected set; }
        public TimeSpan CurrentTime
        {
            get => Watcher == null ? TimeSpan.Zero : TimeSpan.FromMilliseconds(Watcher.ElapsedMilliseconds + WatcherOffset);
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException();

                Synchronzie();
            }
        }
        public int Offset
        {
            get => _Offset;
            set
            {
                WatcherOffset += (value - _Offset);
                _Offset = value;
            }
        }
        public int CurrentIndex
        {
            get => _CurrentIndex;
            protected set
            {
                if (value < TrackLyric?.Lyric.Count && value >= 0)
                {
                    _CurrentIndex = value;
                    OnLyricChanged(EventArgs.Empty);
                }
            }
        }

        public List<Lyric> PlayingLyrics
        {
            get
            {
                var currentTime = CurrentTime.TotalMilliseconds;
                return TrackLyric?.Lyric.Where(x => x.StartAt <= currentTime && x.EndAt > currentTime).ToList();
            }
        }

        public event EventHandler LyricChanged;
        public bool IsDisposed { protected set; get; }


        private ISoundEngine SoundEngine { set; get; }
        PausableTimer Timer;
        Stopwatch Watcher;
        long WatcherOffset;
        int _Offset;
        int _CurrentIndex;

        public void Load(TrackLyric lyric, ISoundEngine soundEngine)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("Engine");
            if (lyric?.Lyric == null)
                throw new ArgumentNullException();
            if (Status == LyricPlayerStaus.Playing)
                throw new InvalidOperationException("Cant initialize while playing");

            Clear();

            if (!lyric.Synchronized)
                TrackLyric = new TrackLyric
                {
                    Copyright = lyric.Copyright ?? "",
                    Synchronized = true,
                    Lyric = new List<Lyric>
                    {
                        new Lyric {
                            Duration =int.MaxValue,
                            Element = new TextElement("Lyric isnt synchronized"){ FontName = Fixed.DefaultFontName, FontSize = Fixed.DefaultFontSize } }
                    }
                };
            else
                TrackLyric = lyric;

            SoundEngine = soundEngine;
            Initialize();
        }

        public void Pause()
        {
            if (Status != LyricPlayerStaus.Playing)
                return;

            Watcher.Stop();
            Timer.Pause();
            Status = LyricPlayerStaus.Paused;
        }

        public void Resume()
        {
            if (Status == LyricPlayerStaus.Paused)
            {
                Timer.Resume();
                Watcher.Start();
                Status = LyricPlayerStaus.Playing;
            }
            else if (Status == LyricPlayerStaus.Stopped || Status == LyricPlayerStaus.Loading)
                Start();
        }

        public void Start()
        {
            if (Status == LyricPlayerStaus.Playing)
                return;

            if (Status == LyricPlayerStaus.Paused)
            { Resume(); return; }

            if (Status == LyricPlayerStaus.Loading && TrackLyric == null)
                return;

            CurrentIndex = 0;
            if (!(TrackLyric?.Lyric?.Any() ?? false))
                return;

            Timer.Stop();
            Timer.Interval = TrackLyric.Lyric.Count > 1 ? TrackLyric.Lyric[1].StartAt : TrackLyric.Lyric[0].Duration;

            WatcherOffset = 0;
            Watcher.Restart();
            Timer.Start();
            Status = LyricPlayerStaus.Playing;
        }

        public void Stop()
        {
            if (Status == LyricPlayerStaus.Loading)
                return;

            Watcher?.Reset();
            if (Timer != null)
            {
                Timer.Stop();
                Timer.Interval = int.MaxValue;
            }
            WatcherOffset = 0;
            CurrentIndex = 0;
            Status = LyricPlayerStaus.Stopped;
        }

        public void Clear()
        {
            Stop();
            TrackLyric = null;
            Status = LyricPlayerStaus.Loading;
        }

        public void Dispose()
        {
            Timer?.Dispose();
            Watcher?.Stop();
            Timer = null;
            Watcher = null;
            TrackLyric = null;
            IsDisposed = true;
        }

        private void Initialize()
        {
            if (Timer == null)
            {
                Timer = new PausableTimer();
                Watcher = new Stopwatch();
                Timer.Elapsed += TimerElapsed;
            }
            Timer.Interval = int.MaxValue;
            WatcherOffset = 0;
            Timer.Stop();
            Watcher.Reset();
        }

        public void Synchronzie()
        {
            if (!(TrackLyric?.Lyric.Any() ?? false))
                return;

            WatcherOffset += (long)(SoundEngine.CurrentTime.TotalMilliseconds - CurrentTime.TotalMilliseconds);
            Timer.Interval = CalculateTimerDuration();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            //var currentLyrics = PlayingLyrics;
            //var currentLyric = currentLyrics.First();

            WatcherOffset += (long)(SoundEngine.CurrentTime.TotalMilliseconds - CurrentTime.TotalMilliseconds);
            var interval = CalculateTimerDuration();
            if (interval == int.MaxValue)
            {
                Timer.Stop();
                Watcher.Reset();
                WatcherOffset = Offset;
                Status = LyricPlayerStaus.Stopped;
                return;
            }

            Timer.Interval = interval;
            OnLyricChanged(EventArgs.Empty);
        }

        private int CalculateTimerDuration()
        {
            var currentLyrics = PlayingLyrics;
            if (!currentLyrics.Any())
                return int.MaxValue;

            Lyric nearestEvent = null;
            var nearestEventTime = int.MaxValue;
            var lastLyricIndex = TrackLyric.Lyric.IndexOf(currentLyrics.Last());
            if (lastLyricIndex <= TrackLyric.Lyric.Count - 1)
                currentLyrics.Add(TrackLyric.Lyric[lastLyricIndex + 1]);

            for (int i = 0; i < currentLyrics.Count; i++)
            {
                var lyric = currentLyrics[i];
                var currentTime = (int)CurrentTime.TotalMilliseconds;
                if (lyric.StartAt - currentTime < nearestEventTime && lyric.StartAt > currentTime)
                {
                    nearestEventTime = currentTime - lyric.StartAt;
                    nearestEvent = lyric;
                }
                if (lyric.EndAt - currentTime < nearestEventTime)
                {
                    nearestEventTime = lyric.EndAt - currentTime;
                    nearestEvent = lyric;
                }
            }

            Console.WriteLine($"Calculated Interval:{nearestEventTime}");
            return nearestEventTime < 3 ? 3 : nearestEventTime;
        }

        protected virtual void OnLyricChanged(EventArgs e)
        {
            LyricChanged?.Invoke(this, e);
        }
    }
}
