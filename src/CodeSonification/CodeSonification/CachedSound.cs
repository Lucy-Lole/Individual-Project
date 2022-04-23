using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSonification
{
    class CachedSound
    {
        private float[] mvarRawData;
        private WaveFormat mvarWaveFormat;

        public float[] RawData
        { 
            get { return mvarRawData; }
        }

        public WaveFormat WaveFormat
        {
            get { return mvarWaveFormat; }
        }

        public CachedSound(string audioFileName, int shiftAmnt = 0)
        {
            using (AudioFileReader audioFileReader = new AudioFileReader(audioFileName))
            {
                mvarWaveFormat = audioFileReader.WaveFormat;

                var shift = new SmbPitchShiftingSampleProvider(audioFileReader);

                if (shiftAmnt > 0)
                {
                    shift.PitchFactor = (float)Math.Pow(Math.Pow(2, 1.0 / 12), shiftAmnt);
                }
                else
                {
                    shift.PitchFactor = 1.0f;
                }

                List<float> wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                float[] readBuffer = new float[shift.WaveFormat.SampleRate * shift.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = shift.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    wholeFile.AddRange(readBuffer.Take(samplesRead));
                }

                mvarRawData = wholeFile.ToArray();
            }
        }
    }

}
