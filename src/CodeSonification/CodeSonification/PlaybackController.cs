using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace CodeSonification
{
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
