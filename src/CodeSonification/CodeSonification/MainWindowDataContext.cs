using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSonification
{
    class MainWindowDataContext
    {
        private string mvarCurrentFilePath;
        private Settings mvarSettings;
        private PlaybackState mvarPlaybackState;
        private double mvarVolume;

        public MainWindowDataContext()
        {
            mvarCurrentFilePath = "";
            mvarSettings = new Settings();
            mvarPlaybackState = PlaybackState.Stopped;
        }

        public string GetCurrentFilePath()
        {
            return mvarCurrentFilePath;
        }
        public void SetCurrentFilePath(string value)
        {
            mvarCurrentFilePath = value;
        }

        public double GetVolume()
        {
            return mvarVolume;
        }
        public void SetVolume(double value)
        {
            mvarVolume = value;
        }


        public PlaybackState GetPlaybackState()
        {
            return mvarPlaybackState;
        }

        public AudioData GetAudioData()
        {
            AudioData data = new AudioData();

            // Convert the current text to audio data.

            return data;
        }

        public bool BeginPlayback()
        {
            if (mvarPlaybackState == PlaybackState.Stopped)
            {
                mvarPlaybackState = PlaybackState.Playing;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
