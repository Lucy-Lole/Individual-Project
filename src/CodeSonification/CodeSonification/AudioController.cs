using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CodeSonification
{
    class AudioController
    {

        Dictionary<int, AudioData> mvarCurrentAD;
        Dictionary<int, AudioData> mvarHoldingData;

        bool mvarNewData;


        public AudioController()
        {
            mvarCurrentAD = new Dictionary<int, AudioData>();
        }

        private void PlaySound(AudioData data)
        {
            System.Diagnostics.Debug.WriteLine("Played " + data.Name + " on line " + data.Line);
        }

        public static Dictionary<int, AudioData> CreateAudioDict(List<AudioData> data)
        {
            Dictionary<int, AudioData> newDict = new Dictionary<int, AudioData>();


            foreach (AudioData ad in data)
            {
                newDict.Add(ad.Line, ad);
            }


            return newDict;
        }

        public void ChangeAudioData(Dictionary<int, AudioData> newDict)
        {
            mvarHoldingData = newDict;
            mvarNewData = true;
        }

        public void StartPlayback(object data, object ct)
        {
            if (data is MainWindowDataContext CallingContext && ct is CancellationToken token)
            {
                mvarCurrentAD = CreateAudioDict(CallingContext.CurrentData);

                AudioData currentData = null;

                for (int i = 0; i < CallingContext.TotalBeats; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    if (mvarCurrentAD.TryGetValue(i, out currentData))
                    {
                        PlaySound(currentData);
                    }

                    CallingContext.CurrentBeat++;

                    double waitTime = (60.00 / CallingContext.BPM) * 1000;

                    DateTime startTime = DateTime.UtcNow;

                    while (DateTime.UtcNow < startTime.AddMilliseconds(waitTime))
                    {
                        if (mvarNewData)
                        {
                            mvarCurrentAD = new Dictionary<int, AudioData>(mvarHoldingData);
                            mvarHoldingData = null;
                            mvarNewData = false;
                        }

                        waitTime = (60.00 / CallingContext.BPM) * 1000;

                        // Busy wait until time has elapsed for the next beat.
                    }
                }
            }
        }
    }
}
