using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public class FrameRateTimerFactory : IFrameRateTimerFactory
    {
        IList<FrameRateTimer> _frameRateTimers;

        public FrameRateTimerFactory()
        {
            _frameRateTimers = new List<FrameRateTimer>();
        }

        public FrameRateTimer GetFrameRateTimer(Int32 framesPerSecond)
        {
            FrameRateTimer timerToUse = null;

            foreach (var timer in _frameRateTimers)
            {
                //if (timer.HasCapacity && timer.FramesPerSecond == framesPerSecond)
                if (timer.FramesPerSecond == framesPerSecond)
                {
                    timerToUse = timer;
                    break;
                }
            }

            if (timerToUse == null)
            {
                timerToUse = new FrameRateTimer(framesPerSecond);
                _frameRateTimers.Add(timerToUse);
            }

            return timerToUse;
        }

        public void Dispose()
        {
            foreach (var timer in _frameRateTimers)
            {
                if (timer.IsRunning)
                {
                    timer.Stop();
                }
            }
        }

        public void ShutDown()
        {
            foreach (var timer in _frameRateTimers)
            {
                timer.Stop();
            }
        }
    }
}
