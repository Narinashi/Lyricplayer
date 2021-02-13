using LyricPlayer.Model;
using LyricPlayer.Models;
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
        public PlayerStatus Status { get; private set; }
        public TimeSpan CurrentTime
        {
            get => Watcher?.Elapsed.Add(TimeSpan.FromMilliseconds(WatcherOffset)) ?? TimeSpan.Zero;
            set
            {
                if (value > TimeSpan.Zero)
                    JumpAtTime((int)value.TotalMilliseconds);
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
                if (value < TrackLyric?.Lyric.Count)
                {
                    _CurrentIndex = value;
                    OnLyricChanged(CurrentLyric);
                }
            }
        }
        public Lyric CurrentLyric => TrackLyric?.Lyric[CurrentIndex];
        public TrackLyric TrackLyric { get; protected set; }
        public event EventHandler<Lyric> LyricChanged;
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

            if (!lyric.Synchronized)
                TrackLyric = new TrackLyric
                {
                    Copyright = lyric.Copyright ?? "",
                    Synchronized = true,
                    Lyric = new List<Lyric>
                    {
                        new Lyric { Duration=int.MaxValue, Text = string.Join("\r\n",lyric.Lyric.Select(x=>x.Text).ToArray()) }
                    }
                };
            else
                TrackLyric = lyric;

            SoundEngine = soundEngine;
            Initialize();
        }

        public void Pause()
        {
            if (Status != PlayerStatus.Playing)
                return;

            Watcher.Stop();
            Timer.Pause();
            Status = PlayerStatus.Paused;
        }

        public void Resume()
        {
            if (Status == PlayerStatus.Paused)
            {
                Timer.Resume();
                Watcher.Start();
                Status = PlayerStatus.Playing;
            }
            else if (Status == PlayerStatus.Stopped)
                Start();
        }

        public void Start()
        {
            if (Status == PlayerStatus.Stopped)
                return;
            if (Status == PlayerStatus.Paused)
            { Resume(); return; }

            CurrentIndex = 0;
            if (!(TrackLyric?.Lyric?.Any() ?? false))
                return;

            Timer.Stop();
            Timer.Interval = TrackLyric.Lyric[0].Duration;

            WatcherOffset = 0;
            Watcher.Restart();
            Timer.Start();
            Status = PlayerStatus.Playing;
        }

        public void Stop()
        {
            Watcher?.Reset();
            if (Timer != null)
            {
                Timer.Stop();
                Timer.Interval = int.MaxValue;
            }
            WatcherOffset = 0;
            CurrentIndex = 0;
            Status = PlayerStatus.Stopped;
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

        private void JumpAtTime(int time)
        {
            if (!(TrackLyric?.Lyric.Any() ?? false))
                return;
            Lyric lyric = TrackLyric.Lyric[0];

            if (lyric.StartAt >= time)
            {
                JumpToLyric(lyric, time);
                return;
            }

            for (int index = 0; index < TrackLyric.Lyric.Count; index++)
            {
                lyric = TrackLyric.Lyric[index];

                if (lyric.StartAt <= time && lyric.Duration + lyric.StartAt >= time)
                {
                    JumpToLyric(lyric, time);
                    break;
                }
            }
        }

        private void JumpToLyric(Lyric lyric, int time)
        {
            _CurrentIndex = TrackLyric.Lyric.IndexOf(lyric);
            Timer.Interval = lyric.EndAt - time;
            WatcherOffset = time - Watcher.ElapsedMilliseconds;
            OnLyricChanged(CurrentLyric);
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (CurrentIndex >= TrackLyric.Lyric.Count - 1)
            {
                Timer.Stop();
                Watcher.Reset();
                CurrentIndex = 0;
                WatcherOffset = Offset;
                Status = PlayerStatus.Stopped;
                return;
            }

            var finishedLyric = CurrentLyric;
            var incomingLyric = TrackLyric.Lyric[CurrentIndex + 1];
            var playerCurrentTime = SoundEngine.CurrentTime.TotalMilliseconds;

            if (playerCurrentTime < finishedLyric.StartAt || playerCurrentTime >= finishedLyric.EndAt)
            {
                JumpAtTime((int)playerCurrentTime);
                return;
            }

            if (Math.Abs(playerCurrentTime - (Watcher.ElapsedMilliseconds + WatcherOffset)) > 1)
                WatcherOffset += (long)playerCurrentTime - (Watcher.ElapsedMilliseconds + WatcherOffset);

            var currentTime = Watcher.ElapsedMilliseconds + WatcherOffset;
            var timerError = currentTime - incomingLyric.StartAt;

            Timer.Interval = incomingLyric.Duration - timerError < 5 ? 5 : incomingLyric.Duration - timerError;
            CurrentIndex++;
        }

        protected virtual void OnLyricChanged(Lyric lyric)
        {
            LyricChanged?.Invoke(this, lyric);
        }
    }
}
