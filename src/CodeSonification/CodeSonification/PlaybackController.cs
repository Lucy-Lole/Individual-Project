using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSonification
{
    class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }

        private double mvarWholeTone = (Math.Pow(2, 1.0 / 12) * Math.Pow(2, 1.0 / 12));

        public CachedSound(string audioFileName, int shiftAmnt = 0)
        {
            using (var audioFileReader = new AudioFileReader(audioFileName))
            {
                WaveFormat = audioFileReader.WaveFormat;

                var shift = new SmbPitchShiftingSampleProvider(audioFileReader);

                if (shiftAmnt > 0)
                {
                    shift.PitchFactor = (float)(shiftAmnt * mvarWholeTone);
                }
                else
                {
                    shift.PitchFactor = 1.0f;
                }

                
                var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                var readBuffer = new float[shift.WaveFormat.SampleRate * shift.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = shift.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    wholeFile.AddRange(readBuffer.Take(samplesRead));
                }
                AudioData = wholeFile.ToArray();
            }
        }
    }

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
            long availableSamples = mvarSound.AudioData.Length - mvarPosition;
            long samplesToCopy = Math.Min(availableSamples, count);

            Array.Copy(mvarSound.AudioData, mvarPosition, buffer, offset, samplesToCopy);

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

    class PlaybackController : IDisposable
    {
        private IWavePlayer mvarOutputDevice;
        private MixingSampleProvider mvarMixer;
        private VolumeSampleProvider mvarVolProv;

        public PlaybackController(int sampleRate = 44100, int channelCount = 2)
        {
            mvarOutputDevice = new WaveOutEvent();
            mvarMixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            mvarMixer.ReadFully = true;
            mvarVolProv = new VolumeSampleProvider(mvarMixer);
            mvarOutputDevice.Init(mvarVolProv);
            mvarOutputDevice.Play();
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == mvarMixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mvarMixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        public void PlaySound(CachedSound sound, float volume)
        {
            mvarOutputDevice.Volume = 1.0f;
            AddMixerInput(new CachedSoundSampleProvider(sound, volume));
        }

        private void AddMixerInput(ISampleProvider input)
        {
            mvarMixer.AddMixerInput(ConvertToRightChannelCount(input));
        }

        public void Dispose()
        {
            mvarOutputDevice.Dispose();
        }
    }
}
