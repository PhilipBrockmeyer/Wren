using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Audio;
using Wren.Core;

namespace Wren.Emulation.MasterSystem
{
    public class SN76489 : IPortAddressableSystemComponent
    {
        SineChannel _channel1;
        SineChannel _channel2;
        SineChannel _channel3;

        Int32 _currentChannel;
        Int32 _toneLowFrequencyBits;

        public SN76489()
        {
            AudioPlayer<Int16> audioPlayer = new AudioPlayer<Int16>();
            MixedChannel channel = new MixedChannel(44100);
            _channel1 = new SineChannel();
            _channel2 = new SineChannel();
            _channel3 = new SineChannel();
            
            channel.AddChannel(_channel1);
            channel.AddChannel(_channel2);
            channel.AddChannel(_channel3);

            audioPlayer.Channel = channel;
            audioPlayer.Initialize(WrenCore.WindowHandle);
            audioPlayer.Play();
        }

        public void RegisterPortAddresses(IPortManager portManager)
        {
            portManager.RegisterWritePort(0x7F, Write);
        }

        public void Write(Byte value)
        {
            // if leftmost bit is not set, we are setting the high 6 bits of the tone frequency.
            if ((value & 0x80) != 0x80)
            {
                switch (_currentChannel)
                {
                    case 0:
                        _channel1.Frequency = GetFrequency(value & 0x3F);
                        break;

                    case 1:
                        _channel2.Frequency = GetFrequency(value & 0x3F);
                        break;

                    case 2:
                        _channel3.Frequency = GetFrequency(value & 0x3F);
                        break;
                }
            }

            else
            {
                // Set amplitude
                if ((value & 0x10) == 0x10)
                {
                    SetAmplitude((value & 0x60) >> 5, value & 0x0F);
                }
                else
                {
                    var channelNumber = (value & 0x60) >> 5;

                    // Set Tone channel and Lowbit
                    if (channelNumber < 3)
                    {
                        _currentChannel = channelNumber;
                        _toneLowFrequencyBits = value & 0x0F;
                    }
                }
            }
        }

        private void SetAmplitude(Int32 channelNumber, Int32 amplitude)
        {
            Double dAmp = (16.0 - (Double)amplitude) / 16.0;

            switch (channelNumber)
            {
                case 0:
                    _channel1.Amplitude = dAmp;
                    break;

                case 1:
                    _channel1.Amplitude = dAmp;
                    break;

                case 2:
                    _channel1.Amplitude = dAmp;
                    break;
            }
        }

        private Double GetFrequency(Int32 highBits)
        {
            Double frequencyNumber = (Double)((highBits << 4) | _toneLowFrequencyBits);

            if (frequencyNumber == 0)
                return 0.0;

            return 125000.0 / frequencyNumber;
        }
    }
}
