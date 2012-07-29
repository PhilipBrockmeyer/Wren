using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Audio
{
    public class MixedChannel : Channel
    {
        List<Channel> _channels;

        public MixedChannel(Int32 sampleRate)
        {
            SampleRate = sampleRate;
            _channels = new List<Channel>();
            Buffer = new Int16[SampleRate];
        }

        public void AddChannel(Channel channel)
        {
            channel.Buffer = new Int16[SampleRate];
            channel.SampleRate = SampleRate;
            _channels.Add(channel);
        }

        public override void WriteDataToBuffer(Int32 count, Int32 bufferStartIndex, Double phaseStartPosition)
        {
            Double TotalAmplitude = 0.0;

            foreach (var channel in _channels)
                TotalAmplitude += channel.Amplitude;

            if (TotalAmplitude == 0.0)
                return;

            foreach (var channel in _channels)
            {
                channel.WriteDataToBuffer(count, bufferStartIndex, phaseStartPosition);
            }

            for (Int32 i = 0; i < count; i++)
            {
                Int32 bufferIndex = (i + bufferStartIndex) % Buffer.Length;
                Buffer[bufferIndex] = Convert.ToInt16(0);

                foreach (var channel in _channels)
                {
                    Buffer[bufferIndex] = (Int16)(Buffer[bufferIndex] + ((Double)channel.Buffer[bufferIndex] / TotalAmplitude));
                }
            }
        }
    }
}
