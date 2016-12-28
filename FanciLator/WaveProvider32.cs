using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace FanciLator
{
    abstract class WaveProvider32 : IWaveProvider
    {
        private WaveFormat waveFormat;
        public int samplesToFill;

        public WaveProvider32()
            : this(44100, 1)
        {
        }

        public WaveProvider32(int sampleRate, int channels)
        {
            SetWaveFormat(sampleRate, channels);
        }

        public void SetWaveFormat(int sampleRate, int channels)
        {
            this.waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
            //Console.WriteLine("buffer " + waveFormat.BitsPerSample);
            // zgornje se vpada z spodnjim "samplesRequired" ki je "count / 4"
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            WaveBuffer waveBuffer = new WaveBuffer(buffer);
            //Console.WriteLine("wave flaot " + waveBuffer.FloatBuffer.Length);
            //Console.WriteLine("sample rate " + waveFormat.SampleRate);
            // iz kje pride vrednost spodnjega ?? zgleda da je odvisno od "samplerateja"
            //Console.WriteLine("wave length " + buffer.Length);
            int samplesRequired = count / 4;
            int samplesRead = Read(waveBuffer.FloatBuffer, offset / 4, samplesRequired);
            //Console.WriteLine("samples read " + samplesRead * 4);
            return samplesRead * 4;
        }

        public abstract int Read(float[] buffer, int offset, int sampleCount);

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }
    }
}
