using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Audio
{
    public abstract class Channel
    {
        
        public Double Amplitude { get; set; }
        public Int32 SampleRate { get; set; }
        public Int16[] Buffer { get; set; }
        public Int64 MaximumAmplitude { get; private set; }

        public Channel()
        {
            //if (!(new Type[] { typeof(Byte), typeof(Int16), typeof(Int32), typeof(Int64) }).Contains(typeof(TBitRate)))
                //throw new ApplicationException("Bit rate must be Byte, Int16, Int32, or Int64");

            MaximumAmplitude = Int16.MaxValue;
        
        }

        public abstract void WriteDataToBuffer(Int32 count, Int32 bufferStartIndex, Double phaseStartPosition);
    }
}
