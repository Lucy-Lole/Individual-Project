using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CodeSonification
{
    class MainWindowDataContext : INotifyPropertyChanged
    {
        private string mvarCurrentFilePath;
        private Settings mvarSettings;
        private PlaybackState mvarPlaybackState;
        private AudioController mvarAudioController;
        private List<AudioData> mvarCurrentData;
        private List<AudioData> mvarClassData;
        private List<AudioData> mvarMethodData;
        private List<AudioData> mvarInternalsData;
        private LayerState mvarLayer;
        private int mvarDataPosition;
        private int mvarCurrentBeat;
        private int mvarTotalBeats;

        private Thread mvarPlaybackThread;
        private CancellationTokenSource mvarTokenSource;

        public event PropertyChangedEventHandler PropertyChanged;

        public int TotalBeats
        {
            get { return mvarTotalBeats; }
        }

        public int CurrentBeat
        {
            get { return mvarCurrentBeat; }
            set 
            {
                mvarCurrentBeat = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentBeat"));
            }
        }

        public List<AudioData> CurrentData
        {
            get { return mvarCurrentData; }
        }

        public string CurrentBPM
        {
            get
            {
                return mvarSettings.GetBPM().ToString();
            }
        }

        private string[] mvarClassKeywords = {"public", "private", "protected", "internal"};

        
        public MainWindowDataContext()
        {
            mvarCurrentFilePath = "";
            mvarSettings = new Settings();
            mvarPlaybackState = PlaybackState.Stopped;
            mvarDataPosition = 0;
            mvarCurrentBeat = 0;
            mvarAudioController = new AudioController();
            mvarCurrentData = new List<AudioData>();
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
            return mvarSettings.GetVolume();
        }
        public void SetVolume(double value)
        {
            mvarSettings.SetVolume(value);
        }

        public int BPM
        {
            get { return mvarSettings.GetBPM(); }
            set { mvarSettings.SetBPM(value); }
        }

        public void IncrementPosition()
        {
            if (mvarDataPosition < mvarCurrentData.Count())
            {
                mvarDataPosition++;
            }
        }
        public void DecrementPosition()
        {
            if (mvarDataPosition > 0)
            {
                mvarDataPosition--;
            }
        }

        public PlaybackState GetPlaybackState()
        {
            return mvarPlaybackState;
        }

        private void ApplyCurrentLayer()
        {
            switch (mvarLayer)
            {
                case LayerState.Class:
                    mvarCurrentData = mvarClassData;
                    break;

                case LayerState.Method:
                    mvarCurrentData = mvarMethodData;
                    break;

                case LayerState.Internals:
                    mvarCurrentData = mvarInternalsData;
                    break;

                case LayerState.All:
                default:
                    mvarCurrentData = mvarClassData.Concat(mvarMethodData.Concat(mvarInternalsData).ToList()).ToList();
                    break;
            }
        }

        private MuteType GetMuteType(string word)
        {
            if (word == "private")
            {
                return MuteType.mute;
            }
            else if (word == "public")
            {
                return MuteType.normal;
            }
            else if (word == "protected" || word == "internal")
            {
                return MuteType.slight;
            }

            return MuteType.normal;
        }

        public void GetAudioData()
        {
            mvarClassData = new List<AudioData>();
            mvarMethodData = new List<AudioData>();
            mvarInternalsData = new List<AudioData>();

            string[] lines = File.ReadAllLines(mvarCurrentFilePath);

            mvarTotalBeats = lines.Length;

            for (int j = 0; j < mvarTotalBeats; j++)
            {
                string[] words = lines[j].Split(' ');

                for (int i = 0; i < words.Length; i++)
                {
                    string word = words[i];
                    MuteType mute = MuteType.normal;
                    bool inheritance = false;

                    if (word == "")
                        continue;

                    if (word == "using")
                    {
                        AudioData newData = new AudioData(words[i + 1], j, true, Instrument.dust, mute, false);
                        mvarClassData.Add(newData);
                    }
                    else if (word == "class")
                    {
                        // There can be up to 3 words before a class, but we only need to check in 2 positions for access modifiers.
                        if (i != 0 && mvarClassKeywords.Contains(words[i - 1]))
                        {
                            mute = GetMuteType(words[i - 1]);
                        }
                        else if (i > 1 && mvarClassKeywords.Contains(words[i - 2]))
                        {
                            mute = GetMuteType(words[i - 2]);
                        }

                        if (i+2 < words.Length && words[i + 2] == ":")
                        {
                            inheritance = true;
                        }


                        AudioData newData = new AudioData(words[i + 1], j, false, Instrument.piano, mute, inheritance);
                        mvarClassData.Add(newData);
                    }

                }

            }
        }

        public bool BeginPlayback()
        {
            if (mvarPlaybackState == PlaybackState.Stopped)
            {
                ApplyCurrentLayer();

                mvarPlaybackState = PlaybackState.Playing;

                CurrentBeat = 0;

                mvarTokenSource = new CancellationTokenSource();

                mvarPlaybackThread = new Thread(() => mvarAudioController.StartPlayback(this, mvarTokenSource.Token));

                mvarPlaybackThread.Start();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool StopPlayback()
        {
            if (mvarPlaybackState == PlaybackState.Playing)
            {
                CurrentBeat = 0;

                mvarTokenSource.Cancel();

                mvarPlaybackState = PlaybackState.Stopped;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
