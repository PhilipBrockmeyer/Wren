using System;
namespace Wren.Core
{
    public interface IFrameRateTimerFactory: IDisposable
    {
        FrameRateTimer GetFrameRateTimer(int framesPerSecond);
        void ShutDown();
    }
}
