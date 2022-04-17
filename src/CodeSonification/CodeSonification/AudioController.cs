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

        PlaybackController mvarDustOut;
        PlaybackController mvarOtherOut;

        const int mvarMaxDust = 10;

        bool mvarNewData;

        int mvarDustSoundLevel;

        Dictionary<string, CachedSound> mvarSounds;

        public AudioController()
        {
            mvarCurrentAD = new Dictionary<int, AudioData>();
            mvarSounds = new Dictionary<string, CachedSound>();
            mvarDustSoundLevel = 0;
            mvarSounds["dust"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\Dust.wav");

            mvarSounds["piano"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\Piano.wav");
            mvarSounds["pianoSlight"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoSlight.wav");
            mvarSounds["pianoDull"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoDull.wav");

            mvarSounds["pianoWB"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoWB.wav");
            mvarSounds["pianoWBSlight"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoWBSlight.wav");
            mvarSounds["pianoWBDull"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoWBDull.wav");

            mvarSounds["pianoEnd"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoEnd.wav");
            mvarSounds["pianoEndSlight"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoEndSlight.wav");
            mvarSounds["pianoEndDull"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\PianoEndDull.wav");

            mvarSounds["methodStart"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\methodStart.wav");
            mvarSounds["methodStartSlight"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\methodStartSlight.wav");
            mvarSounds["methodStartDull"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\methodStartDull.wav");

            mvarSounds["methodEnd"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\methodEnd.wav");
            mvarSounds["methodEndSlight"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\methodEndSlight.wav");
            mvarSounds["methodEndDull"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\methodEndDull.wav");

            mvarSounds["field"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\field.wav");
            mvarSounds["property"] = new CachedSound("C:\\Users\\Lucy\\Desktop\\COMS\\Individual-Project\\src\\CodeSonification\\CodeSonification\\property.wav");

            mvarDustOut = new PlaybackController();
            mvarOtherOut = new PlaybackController();

        }

        private void PlaySound(AudioData data, float vol)
        {
            switch (data.InstrumentType)
            {
                case Instrument.dust:
                    mvarDustSoundLevel += 1;
                    break;
                case Instrument.field:
                    mvarOtherOut.PlaySound(mvarSounds["field"], vol);
                    break;
                case Instrument.property:
                    mvarOtherOut.PlaySound(mvarSounds["property"], vol);
                    break;
                case Instrument.guitar:
                    if (data.Mute == MuteType.slight)
                    {
                        mvarOtherOut.PlaySound(mvarSounds["methodStartSlight"], vol);
                    }
                    else if (data.Mute == MuteType.mute)
                    {
                        mvarOtherOut.PlaySound(mvarSounds["methodStartDull"], vol);
                    }
                    else
                    {
                        mvarOtherOut.PlaySound(mvarSounds["methodStart"], vol);
                    }
                    break;
                case Instrument.guitarEnd:
                    if (data.Mute == MuteType.slight)
                    {
                        mvarOtherOut.PlaySound(mvarSounds["methodEndSlight"], vol);
                    }
                    else if (data.Mute == MuteType.mute)
                    {
                        mvarOtherOut.PlaySound(mvarSounds["methodEndDull"], vol);
                    }
                    else
                    {
                        mvarOtherOut.PlaySound(mvarSounds["methodEnd"], vol);
                    }
                    break;
                case Instrument.pianoEnd:
                    if (data.Mute == MuteType.slight)
                    {
                        mvarOtherOut.PlaySound(mvarSounds["pianoEndSlight"], vol);
                    }
                    else if (data.Mute == MuteType.mute)
                    {
                        mvarOtherOut.PlaySound(mvarSounds["pianoEndDull"], vol);
                    }
                    else
                    {
                        mvarOtherOut.PlaySound(mvarSounds["pianoEnd"], vol);
                    }
                    
                    break;
                case Instrument.piano:
                    if (data.Inherits)
                    {
                        if (data.Mute == MuteType.slight)
                        {
                            mvarOtherOut.PlaySound(mvarSounds["pianoWBSlight"], vol);
                        }
                        else if (data.Mute == MuteType.mute)
                        {
                            mvarOtherOut.PlaySound(mvarSounds["pianoWBDull"], vol);
                        }
                        else
                        {
                            mvarOtherOut.PlaySound(mvarSounds["pianoWB"], vol);
                        }
                    }
                    else
                    {
                        if (data.Mute == MuteType.slight)
                        {
                            mvarOtherOut.PlaySound(mvarSounds["pianoSlight"], vol);
                        }
                        else if (data.Mute == MuteType.mute)
                        {
                            mvarOtherOut.PlaySound(mvarSounds["pianoSlight"], vol);
                        }
                        else
                        {
                            mvarOtherOut.PlaySound(mvarSounds["piano"], vol);
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

                int i = 0;

                while (!token.IsCancellationRequested)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    if (mvarDustSoundLevel > 0 && (CallingContext.Layer == LayerState.All || CallingContext.Layer == LayerState.Class))
                    {
                        mvarDustOut.PlaySound(mvarSounds["dust"], CallingContext.Volume * (mvarDustSoundLevel < mvarMaxDust ? (float)mvarDustSoundLevel / mvarMaxDust : 1.0f));
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

                    if (i < CallingContext.TotalBeats)
                    {
                        i++;
                    }
                    else
                    {
                        i = 0;
                        mvarDustSoundLevel = 0;
                        CallingContext.CurrentBeat = 0;
                    }
                }
            }
        }
    }
}
