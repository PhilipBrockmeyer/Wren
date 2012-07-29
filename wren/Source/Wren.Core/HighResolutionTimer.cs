using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Wren.Core
{
    public class HighResolutionTimer
    {
        private Int64 _frequency;

        public Int64 Frequency
        {
            get { return _frequency; }
        }

        public HighResolutionTimer()
        {
            if (!QueryPerformanceFrequency(out this._frequency))
            {
                throw new Win32Exception();
            }
        }

        public Int64 GetCurrentTicks()
        {
            Int64 num;
            QueryPerformanceCounter(out num);
            return num;
        }

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
    }
}
