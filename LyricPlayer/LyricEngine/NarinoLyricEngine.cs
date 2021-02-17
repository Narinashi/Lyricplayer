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

        private void JumpAtTime(int time)
        {
            if (!(TrackLyric?.Lyric.Any() ?? false))
                return;
            //return;
            WatcherOffset = time - (int)Watcher.ElapsedMilliseconds;
            //var interval = CalculateTimerDuration();
            //Timer.Interval = interval < 3 ? 3 : interval;
            Timer.Interval = 3;
           //Console.WriteLine($"force set Interval:{interval}");

            //CurrentIndex = TrackLyric.Lyric.IndexOf(lyricAtTime);
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine($"CurrentTime:{CurrentTime}");
            if (CurrentIndex >= TrackLyric.Lyric.Count - 1)
            {
                Timer.Stop();
                Watcher.Reset();
                WatcherOffset = Offset;
                Status = LyricPlayerStaus.Stopped;
                return;
            }
            var currentLyrics = PlayingLyrics;
            var currentLyric = currentLyrics.First();

            var playerCurrentTime = SoundEngine.CurrentTime.TotalMilliseconds;
            if (playerCurrentTime < currentLyric.StartAt || playerCurrentTime >= currentLyric.EndAt)
            {
                Console.WriteLine("Time was off waaay too much");
                JumpAtTime((int)playerCurrentTime);
                //return;
            }

            if (Math.Abs(playerCurrentTime - (Watcher.ElapsedMilliseconds + WatcherOffset)) > 1)
                WatcherOffset += (long)playerCurrentTime - (Watcher.ElapsedMilliseconds + WatcherOffset);

            Timer.Interval = CalculateTimerDuration();
            
            OnLyricChanged(EventArgs.Empty);
            //Console.WriteLine($"Interval:{Timer.Interval}");
            //CurrentIndex = TrackLyric.Lyric.IndexOf(currentLyric);
        }

        private int CalculateTimerDuration()
        {
            var currentLyrics = PlayingLyrics;
            var currentLyric = currentLyrics.First();
            var currentIndex = TrackLyric.Lyric.IndexOf(currentLyric);
            Console.WriteLine($"Current lyric Index:{currentIndex}");
            Console.WriteLine($"CurrentTime :{CurrentTime}");
            var incomingLyric = currentLyrics.FirstOrDefault(x => x.StartAt > currentLyric.StartAt) ?? TrackLyric.Lyric[currentIndex + 1];
            var timerDuration = 0;

            if (currentLyrics.Count < 3)
            {
                timerDuration = incomingLyric.EndAt <= currentLyric.EndAt ?
                   incomingLyric.Duration : currentLyric.EndAt - (int)CurrentTime.TotalMilliseconds;
            }
            else
            {
                var nextLyricAfterIncomingOne = currentLyrics.FirstOrDefault(x => x.StartAt > incomingLyric.StartAt) ?? incomingLyric;

                if (nextLyricAfterIncomingOne.EndAt <= currentLyric.EndAt && nextLyricAfterIncomingOne.EndAt <= incomingLyric.EndAt)
                    timerDuration = nextLyricAfterIncomingOne.StartAt - (int)CurrentTime.TotalMilliseconds;
                else if (incomingLyric.EndAt <= nextLyricAfterIncomingOne.EndAt && incomingLyric.EndAt <= currentLyric.EndAt)
                    timerDuration = incomingLyric.StartAt - (int)CurrentTime.TotalMilliseconds;
                else
                    timerDuration = currentLyric.EndAt - (int)CurrentTime.TotalMilliseconds;
            }

            var timerError = (int)CurrentTime.TotalMilliseconds - incomingLyric.StartAt;
            Console.WriteLine($"Calculated Interval:{timerDuration - timerError}");
            return timerDuration - timerError < 3 ? 3 : timerDuration - timerError;
        }

        protected virtual void OnLyricChanged(EventArgs e)
        {
            LyricChanged?.Invoke(this, e);
        }
    }
}
