using LyricPlayer.Models;
using LyricPlayer.Utilities;
using System;
using System.Diagnostics;
using System.Linq;

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
                if (CurrentTime > TimeSpan.Zero)
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
                if (value < _Lyric?.Lyric.Count)
                {
                    _CurrentIndex = value;
                    OnLyricChanged(CurrentLyric);
                }
            }
        }
        public Lyric CurrentLyric => _Lyric?.Lyric[CurrentIndex];
        public TrackLyric TrackLyric => _Lyric;
        public event EventHandler<Lyric> LyricChanged;

        public bool IsDisposed { protected set; get; }

        TrackLyric _Lyric;
        PausableTimer Timer;
        Stopwatch Watcher;
        long WatcherOffset;
        int _Offset;
        int _CurrentIndex;

        public void Load(TrackLyric lyric)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("Engine");
            if (lyric?.Lyric == null)
                throw new ArgumentNullException();

            Initialize();
            _Lyric = lyric;
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
            if (Status != PlayerStatus.Stopped)
                return;

            CurrentIndex = 0;
            Timer.Interval = _Lyric.Lyric[0].Duration;
            Watcher.Restart();
            Timer.Start();
            Status = PlayerStatus.Playing;
        }

        public void Stop()
        {
            if (Status != PlayerStatus.Playing && Status != PlayerStatus.Paused)
                return;

            Watcher.Reset();
            Timer.Stop();
            CurrentIndex = 0;
            Status = PlayerStatus.Stopped;
        }

        public void Dispose()
        {
            Timer?.Dispose();
            Watcher?.Stop();
            Timer = null;
            Watcher = null;
            _Lyric = null;
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
            Timer.Stop();
            Watcher.Reset();
        }

        private void JumpAtTime(int time)
        {
            if (!(_Lyric?.Lyric.Any() ?? false))
                return;
            Lyric lyric = _Lyric.Lyric[0];

            if (lyric.StartAt >= time)
            {
                JumpToLyric(lyric, time);
                return;
            } 

            for (int index = 0; index < _Lyric.Lyric.Count; index++)
            {
                lyric = _Lyric.Lyric[index];

                if (lyric.StartAt <= time && lyric.Duration + lyric.StartAt >= time)
                {
                    JumpToLyric(lyric, time);
                    break;
                }
            }
        }

        private void JumpToLyric(Lyric lyric, int time)
        {
            _CurrentIndex = _Lyric.Lyric.IndexOf(lyric);
            Timer.Interval = lyric.EndAt - time;
            WatcherOffset = time - Watcher.ElapsedMilliseconds;
            OnLyricChanged(CurrentLyric);
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (CurrentIndex >= _Lyric.Lyric.Count - 1)
            {
                Timer.Stop();
                Watcher.Reset();
                CurrentIndex = 0;
                WatcherOffset = Offset;
                Status = PlayerStatus.Stopped;
                return;
            }

            var finishedLyric = CurrentLyric;
            var incomingLyric = _Lyric.Lyric[CurrentIndex + 1];
            var currentTime = Watcher.ElapsedMilliseconds + WatcherOffset;
            var timerError = currentTime - incomingLyric.StartAt;

            Timer.Interval = incomingLyric.Duration - timerError;
            CurrentIndex++;
        }

        protected virtual void OnLyricChanged(Lyric lyric)
        {
            LyricChanged?.Invoke(this, lyric);
        }
    }
}
