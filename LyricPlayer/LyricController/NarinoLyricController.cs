using LyricPlayer.Models;
using LyricPlayer.Utilities;
using System;
using System.Diagnostics;

namespace LyricPlayer.LyricController
{
    public class NarinoLyricController : ILyricController, IDisposable
    {
        public PlayerStatus ControllerStatus => Status;
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
            set => _Offset = value;
        }
        public int CurrentIndex
        {
            get => _CurrentIndex;
            protected set
            {
                if (value < Lyric?.Lyric.Count)
                {
                    _CurrentIndex = value;
                    OnLyricChanged(CurrentLyric);
                }
            }
        }
        public Lyric CurrentLyric => Lyric?.Lyric[CurrentIndex];
        public event EventHandler<Lyric> LyricChanged;

        public bool IsDisposed { set; get; }

        TrackLyric Lyric;
        PausableTimer Timer;
        Stopwatch Watcher;
        long WatcherOffset;
        int _Offset;
        int _CurrentIndex;
        PlayerStatus Status { set; get; }
        public void Load(TrackLyric lyric)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("Controller");
            if (lyric?.Lyric == null)
                throw new ArgumentNullException();

            Initialize();
            Lyric = lyric;
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
            Timer.Interval = Lyric.Lyric[0].Duration;
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
            Lyric = null;
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
            Timer.Interval = 1000;
            Timer.Stop();
            Watcher.Reset();
        }

        private void JumpAtTime(int time)
        {
            for (int index = 0; index < Lyric.Lyric.Count; index++)
            {
                var lyric = Lyric.Lyric[index];

                if (lyric.StartAt <= time && lyric.Duration + lyric.StartAt >= time)
                {
                    CurrentIndex = index;
                    Timer.Interval = lyric.EndAt - time;
                    WatcherOffset = time - (Watcher.ElapsedMilliseconds + WatcherOffset);
                    break;
                }
            }
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (CurrentIndex >= Lyric.Lyric.Count - 1)
            {
                Timer.Stop();
                Watcher.Reset();
                CurrentIndex = 0;
                WatcherOffset = 0;
                return;
            }

            var finishedLyric = CurrentLyric;
            var incomingLyric = Lyric.Lyric[CurrentIndex + 1];

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
