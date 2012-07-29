using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectSound;
using SlimDX.Multimedia;
using System.Threading;

namespace Wren.Core.Audio
{
    public class AudioPlayer<TBitRate>
        where TBitRate : struct
    {
        DirectSound _soundDevice;
        SecondarySoundBuffer _buffer;

        public Channel Channel { get; set; }

        Boolean _isPlaying;

        public AudioPlayer()
        {
        }

        public void Initialize(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return;

            //Create the device
            _soundDevice = new DirectSound();
            _soundDevice.SetCooperativeLevel(handle, CooperativeLevel.Priority);

            //Creat the wav format, it will be mono-44100-pcm-16bit
            WaveFormat wav = new WaveFormat();
            wav.FormatTag = WaveFormatTag.Pcm;
            wav.SamplesPerSecond = 44100;
            wav.Channels = 1;//mono
            wav.BitsPerSample = 16;
            wav.AverageBytesPerSecond = 88200;//wav.SamplesPerSecond * wav.Channels * (wav.BitsPerSample / 8);
            wav.BlockAlignment = 2;//(wfx.Channels * wfx.BitsPerSample / 8);
            //Description
            SoundBufferDescription des = new SoundBufferDescription();
            des.Format = wav;
            des.SizeInBytes = 88200;
            des.Flags = BufferFlags.GlobalFocus | BufferFlags.Software;
            //buffer
            _buffer = new SecondarySoundBuffer(_soundDevice, des);

            _buffer.Write(Channel.Buffer, 0, LockFlags.None);
        }

        public void Play()
        {
            if (_buffer == null)
                return;

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                _buffer.Play(0, PlayFlags.Looping);

                Int32 lastWritePosition = 0;
                Int32 currentWritePosition = 0;
                Int32 counter = 0;
                Boolean isBufferPlaying = true;
                _isPlaying = true;

                while (true)
                {
                    if (!_isPlaying)
                    {
                        if (isBufferPlaying)
                        {
                            _buffer.Stop();
                            isBufferPlaying = false;
                        }
                        
                        Thread.Sleep(400);
                        continue;
                    }

                    currentWritePosition = _buffer.CurrentWritePosition;
                    Int32 bytesIncremented = currentWritePosition - lastWritePosition;
                    if (bytesIncremented < 0)
                        bytesIncremented = (Channel.Buffer.Length * 2) - lastWritePosition + currentWritePosition;

                    Int32 dataIndex = (Int32)(Math.Floor((lastWritePosition / 2.0)) + 0x1000);

                    Channel.WriteDataToBuffer(dataIndex, (Int32)(bytesIncremented / 2.0) + 0x100, lastWritePosition / 2.0);
                    _buffer.Write(Channel.Buffer, 0, LockFlags.None);

                    counter++;
                    lastWritePosition = currentWritePosition;
                    Thread.Sleep(1);
                    isBufferPlaying = true;
                }
            }
            );
        }

        public void Stop()
        {
            _isPlaying = false;
        }
    }
}
