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

        int mvarDustSoundLevel;

        CachedSound mvarDust;
        CachedSound mvarPiano;
        CachedSound mvarPianoDull;
        CachedSound mvarPianoSlight;
        CachedSound mvarPianoWB;
        CachedSound mvarPianoWBSlight;
        CachedSound mvarPianoWBDull;
        CachedSound mvarPianoEnd;
        CachedSound mvarPianoEndSlight;
        CachedSound mvarPianoEndDull;


        public AudioController()
        {
            mvarCurrentAD = new Dictionary<int, AudioData>();
            mvarDustSoundLevel = 0;
            mvarDust = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\Dust.wav");
            mvarPiano = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\Piano.wav");
            mvarPianoSlight = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoSlight.wav");
            mvarPianoDull = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoDull.wav");
            mvarPianoWB = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoWB.wav");
            mvarPianoWBSlight = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoWBSlight.wav");
            mvarPianoWBDull = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoWBDull.wav");
            mvarPianoEnd = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoEnd.wav");
            mvarPianoEndSlight = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoEndSlight.wav");
            mvarPianoEndDull = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoEndDull.wav");

        }

        private void PlaySound(AudioData data, float vol)
        {
            switch (data.InstrumentType)
            {
                case Instrument.dust:
                    mvarDustSoundLevel += 1;
                    break;
                case Instrument.pianoEnd:
                    if (data.Mute == MuteType.slight)
                    {
                        PlaybackController.Instance.PlaySound(mvarPianoEndSlight, vol);
                    }
                    else if (data.Mute == MuteType.mute)
                    {
                        PlaybackController.Instance.PlaySound(mvarPianoEndDull, vol);
                    }
                    else
                    {
                        PlaybackController.Instance.PlaySound(mvarPianoEnd, vol);
                    }
                    
                    break;
                case Instrument.piano:
                    if (data.Inherits)
                    {
                        if (data.Mute == MuteType.slight)
                        {
                            PlaybackController.Instance.PlaySound(mvarPianoWBSlight, vol);
                        }
                        else if (data.Mute == MuteType.mute)
                        {
                            PlaybackController.Instance.PlaySound(mvarPianoWBDull, vol);
                        }
                        else
                        {
                            PlaybackController.Instance.PlaySound(mvarPianoWB, vol);
                        }
                    }
                    else
                    {
                        if (data.Mute == MuteType.slight)
                        {
                            PlaybackController.Instance.PlaySound(mvarPianoSlight, vol);
                        }
                        else if (data.Mute == MuteType.mute)
                        {
                            PlaybackController.Instance.PlaySound(mvarPianoDull, vol);
                        }
                        else
                        {
                            PlaybackController.Instance.PlaySound(mvarPiano, vol);
                        }
                    }
                    
            break;
            }

            
            System.Diagnostics.Debug.WriteLine("Played " + data.Name + " on line " + data.Line);
        }

        public static Dictionary<int, AudioData> CreateAudioDict(List<AudioData> data)
        {
            Dictionary<int, AudioData> newDict = new Dictionary<int, AudioData>();


            foreach (AudioData ad in data)
            {
                if (!newDict.ContainsKey(ad.Line))
                {
                    newDict.Add(ad.Line, ad);
                }
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
            mvarDustSoundLevel = 0;

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

                    if (mvarDustSoundLevel > 0 && (CallingContext.Layer == LayerState.All || CallingContext.Layer == LayerState.Class))
                    {
                        PlaybackController.Instance.PlaySound(mvarDust, CallingContext.Volume * (mvarDustSoundLevel < 10 ? (float)mvarDustSoundLevel / 10 : 1));
                    }

                    if (mvarCurrentAD.TryGetValue(i, out currentData))
                    {
                        PlaySound(currentData, CallingContext.Volume);
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
