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

        public PlaybackController()
        {
            var format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

            mvarOutputDevice = new WaveOutEvent();

            mvarMixer = new MixingSampleProvider(format);
            mvarMixer.ReadFully = true;

            mvarVolProv = new VolumeSampleProvider(mvarMixer);

            mvarOutputDevice.Init(mvarVolProv);
            mvarOutputDevice.Play();
        }

        /// <summary>
        /// Converts the given ISampleProvider to the desired channel count of the PlaybackController.
        /// </summary>
        /// <param name="input">ISampleProvider to convert</param>
        /// <returns>ISampleProvider with correct channel count</returns>
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

        /// <summary>
        /// Played the given CachedSound
        /// </summary>
        /// <param name="sound">CachedSound to play</param>
        /// <param name="volume">Volume to play the CachedSound at</param>
        public void PlaySound(CachedSound sound, float volume)
        {
            mvarOutputDevice.Volume = 1.0f;
            AddMixerInput(new CachedSoundSampleProvider(sound, volume));
        }

        /// <summary>
        /// Adds the given ISampleProvider to the PlaybackControllers mixer after converting it to the correct channel count.
        /// </summary>
        /// <param name="input">ISampleProvider to add</param>
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
