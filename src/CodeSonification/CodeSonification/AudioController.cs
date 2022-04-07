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

        List<AudioData> mvarCurrentAD;

        public AudioController()
        {
            mvarCurrentAD = new List<AudioData>();
        }

        private void PlaySound(AudioData data)
        {
            
        }

        public void StartPlayback(object data)
        {
            if (data is MainWindowDataContext CallingContext)
            {
                for (int i = 0; i < CallingContext.TotalBeats; i++)
                {
                    double waitTime = (60.00 / CallingContext.BPM);

                    CallingContext.CurrentBeat++;

                    Thread.Sleep((int)(waitTime * 1000));
                }
            }
        }
    }
}
