using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Wren.Core
{
    public class FrameRateTimer
    {
        Int32 _ticksPerFrame;
        HighResolutionTimer _timer;
        Thread _timerThread;
        IList<Func<Boolean>> _actions;

        public Int32 FramesPerSecond { get; private set; }
        public Boolean HasCapacity { get; private set; }
        public Boolean IsRunning { get; private set; }

        public FrameRateTimer(Int32 framesPerSecond)
        {
            _timer = new HighResolutionTimer();
            _timerThread = new Thread(Run);
            _actions = new List<Func<Boolean>>();
            IsRunning = false;

            FramesPerSecond = framesPerSecond;
            _ticksPerFrame = (Int32)((Double)_timer.Frequency * (Double)(1.0 / (Double)framesPerSecond));
        }

        public void ScheduleAction(Func<Boolean> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            _actions.Add(action);
        }

        public void Start()
        {
            if (IsRunning)
                throw new InvalidOperationException();

            _timerThread.Priority = ThreadPriority.Normal;
            _timerThread.Start();

            IsRunning = true;
        }

        public void Stop()
        {
            if (_timerThread == null)
                return;

            _timerThread.Abort();
            IsRunning = false;
        }


        private void Run()
        {
            Int64 startTime;

            while (true)
            {
                startTime = _timer.GetCurrentTicks();

                for (int i = _actions.Count - 1; i >= 0; i--)
                {
                    if (!_actions[i].Invoke())
                    {
                        _actions.RemoveAt(i);
                    }
                }

                if (_timer.GetCurrentTicks() < (startTime + _ticksPerFrame) - 0.01 * _timer.Frequency)
                {
                    HasCapacity = true;
                    Thread.Sleep(10);
                }
                else
                {
                    HasCapacity = false;
                }

                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                while (_timer.GetCurrentTicks() < startTime + _ticksPerFrame) { };
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
            }
        }
    }
}
