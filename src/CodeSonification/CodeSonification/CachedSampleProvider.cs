using NAudio.Wave;
using System;

namespace CodeSonification
{
    class CachedSoundSampleProvider : ISampleProvider
    {
        private CachedSound mvarSound;
        private long mvarPosition;
        private float mvarVol;

        public CachedSoundSampleProvider(CachedSound cachedSound, float volume = 1.0f)
        {
            mvarSound = cachedSound;
            mvarVol = volume;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            long availableSamples = mvarSound.RawData.Length - mvarPosition;
            long samplesToCopy = Math.Min(availableSamples, count);

            Array.Copy(mvarSound.RawData, mvarPosition, buffer, offset, samplesToCopy);

            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] *= mvarVol;
            }


            mvarPosition += samplesToCopy;
            return (int)samplesToCopy;
        }

        public WaveFormat WaveFormat
        {
            get { return mvarSound.WaveFormat; }
        }
    }
}
