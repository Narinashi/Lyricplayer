using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LyricPlayer.Utilities
{
    public class PausableTimer : Timer
    {
        public new double Interval
        {
            get { return base.Interval; }
            set
            {
                if (WasPaused)
                    IntervalBeforePausing = value;

                base.Interval = value == 0 ? 1 : value;
            }
        }

        Stopwatch Watcher = new Stopwatch();
        double RemainingTimeToNextTick;
        double IntervalBeforePausing;
        bool WasPaused;

        public new event EventHandler<ElapsedEventArgs> Elapsed;

        public PausableTimer()
        {
            base.Elapsed += BaseTimer_Elapsed;
            WasPaused = false;
            IntervalBeforePausing = 1;
            RemainingTimeToNextTick = 1;
            Watcher = new Stopwatch();
        }

        private void BaseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (WasPaused)
            {
                Interval = IntervalBeforePausing;
                WasPaused = false;
            }

            Watcher.Restart();
            Elapsed?.Invoke(this, e);
        }

        public new void Start()
        {
            base.Start();
            Watcher.Restart();
        }

        public void Pause()
        {
            if (!base.Enabled)
                return;

            base.Stop();

            IntervalBeforePausing = Interval;
            RemainingTimeToNextTick = Interval - Watcher.ElapsedMilliseconds % Interval;
            WasPaused = true;
        }

        public void Resume()
        {
            if (base.Enabled)
                return;

            Interval = RemainingTimeToNextTick;
            base.Start();
        }

        public new void Stop()
        {
            base.Stop();
            Watcher.Reset();
        }

        public new void Dispose()
        {
            Watcher?.Stop();
            Watcher = null;

            base.Dispose();
        }

    }
}
