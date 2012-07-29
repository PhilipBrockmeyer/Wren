using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Audio
{
    public class SineChannel : Channel
    {
        public Double Frequency { get; set; }

        public SineChannel()
        {           
            
        }

        public override void WriteDataToBuffer(Int32 bufferStartIndex, Int32 count, Double phaseStartPosition)
        {
            Int32 startPosition = (Int32)(Math.Floor(phaseStartPosition));

            // Max amplitude for 16-bit audio
            Int64 amplitude = (Int64)((Double)MaximumAmplitude * Amplitude);

            // The "angle" used in the function, adjusted for the number of channels and sample rate.
            // This value is like the period of the wave.
            Double t = (Math.PI * 2.0 * Frequency) / (SampleRate);

            for (int i = 0; i < count - 1; i++)
            {
                Buffer[(i + bufferStartIndex) % SampleRate] = Convert.ToInt16(amplitude * Math.Sin(t * (i + startPosition)));
            }
        }
    }
}
