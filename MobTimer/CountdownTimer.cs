using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace MobTimer
{
    class CountdownTimer
    {
        private Timer _timer = new Timer();
        private Stopwatch _stopwatch = new Stopwatch();
        private Action _action = () => { };

        private TimeSpan _interval = TimeSpan.FromMilliseconds(0);

        public TimeSpan ElapsedTime
        {
            get
            {
                return _stopwatch.Elapsed;
            }
        }
        public TimeSpan TimeLeft
        {
            get
            {
                return _interval > _stopwatch.Elapsed ?
                    _interval - _stopwatch.Elapsed :
                    TimeSpan.FromMilliseconds(0);
            }
        }
        public double Progress
        {
            get
            {
                if (_interval.TotalMilliseconds == 0)
                {
                    return 0;
                }
                return Math.Min(1, _stopwatch.Elapsed.TotalMilliseconds / _interval.TotalMilliseconds);
            }
        }

        public bool IsRunning
        {
            get
            {
                return _stopwatch.IsRunning;
            }
        }

        public CountdownTimer(Action elapsedAction)
        {
            _action = elapsedAction;

            _timer.AutoReset = false;
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _action();
        }

        public void StartTimer(TimeSpan interval)
        {
            _interval = interval;
            _timer.Interval = _interval.TotalMilliseconds;
            _timer.Start();
            _stopwatch.Restart();
        }
        public void PauseTimer()
        {
            _timer.Stop();
            _stopwatch.Stop();
        }
        public void ContinueTimer()
        {
            if (TimeLeft.TotalMilliseconds == 0)
            {
                return;
            }

            _timer.Interval = TimeLeft.TotalMilliseconds;
            _timer.Start();
            _stopwatch.Start();
        }
    }
}
