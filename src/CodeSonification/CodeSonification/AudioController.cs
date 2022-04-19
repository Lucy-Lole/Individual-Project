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

        Dictionary<int, List<AudioData>> mvarCurrentAD;
        Dictionary<int, List<AudioData>> mvarHoldingData;

        PlaybackController mvarDustOut;
        PlaybackController mvarNamespaceOut;
        PlaybackController mvarOtherOut;

        int mvarIndentLevel = 0;

        bool mvarInNamespace = false;

        const int mvarMaxDust = 10;

        bool mvarNewData;

        int mvarDustSoundLevel;

        Dictionary<string, CachedSound> mvarSounds;

        public AudioController()
        {
            mvarCurrentAD = new Dictionary<int, List<AudioData>>();
            mvarSounds = new Dictionary<string, CachedSound>();
            mvarDustSoundLevel = 0;
            mvarSounds["dust"] = new CachedSound(".\\Resources\\dust.wav");

            mvarSounds["piano"] = new CachedSound(".\\Resources\\Piano.wav");
            mvarSounds["pianoSlight"] = new CachedSound(".\\Resources\\PianoSlight.wav");
            mvarSounds["pianoDull"] = new CachedSound(".\\Resources\\PianoDull.wav");

            mvarSounds["pianoWB"] = new CachedSound(".\\Resources\\PianoWB.wav");
            mvarSounds["pianoWBSlight"] = new CachedSound(".\\Resources\\PianoWBSlight.wav");
            mvarSounds["pianoWBDull"] = new CachedSound(".\\Resources\\PianoWBDull.wav");

            mvarSounds["pianoEnd"] = new CachedSound(".\\Resources\\PianoEnd.wav");
            mvarSounds["pianoEndSlight"] = new CachedSound(".\\Resources\\PianoEndSlight.wav");
            mvarSounds["pianoEndDull"] = new CachedSound(".\\Resources\\PianoEndDull.wav");

            mvarSounds["methodStart"] = new CachedSound(".\\Resources\\methodStart.wav");
            mvarSounds["methodStartSlight"] = new CachedSound(".\\Resources\\methodStartSlight.wav");
            mvarSounds["methodStartDull"] = new CachedSound(".\\Resources\\methodStartDull.wav");

            mvarSounds["methodEnd"] = new CachedSound(".\\Resources\\methodEnd.wav");
            mvarSounds["methodEndSlight"] = new CachedSound(".\\Resources\\methodEndSlight.wav");
            mvarSounds["methodEndDull"] = new CachedSound(".\\Resources\\methodEndDull.wav");

            mvarSounds["field"] = new CachedSound(".\\Resources\\field.wav");
            mvarSounds["property"] = new CachedSound(".\\Resources\\property.wav");

            mvarSounds["birds"] = new CachedSound(".\\Resources\\birds.wav");
            mvarSounds["namespace"] = new CachedSound(".\\Resources\\namespace.wav");

            mvarDustOut = new PlaybackController();
            mvarOtherOut = new PlaybackController();
            mvarNamespaceOut = new PlaybackController();

        }

        private void PlaySound(AudioData data, float vol)
        {
            switch (data.InstrumentType)
            {
                case Instrument.dust:
                    mvarDustSoundLevel += 1;
                    break;
                case Instrument.namesp:
                    mvarInNamespace = !mvarInNamespace;
                    break;
                case Instrument.codeBlock:
                    mvarOtherOut.PlaySound(new CachedSound(".\\Resources\\codeBlock.wav", mvarIndentLevel), vol);
                    mvarIndentLevel++;
                    break;
                case Instrument.codeBlockEnd:
                    mvarIndentLevel--;
                    mvarOtherOut.PlaySound(new CachedSound(".\\Resources\\codeBlock.wav", mvarIndentLevel), vol);
                    break;
                case Instrument.field:
                    mvarOtherOut.PlaySound(mvarSounds["field"], vol);
                    break;
                case Instrument.property:
                    mvarOtherOut.PlaySound(mvarSounds["property"], vol);
                    break;
                case Instrument.expression:
                    mvarOtherOut.PlaySound(mvarSounds["birds"], vol);
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

        public static Dictionary<int, List<AudioData>> CreateAudioDict(List<AudioData> data)
        {
            Dictionary<int, List<AudioData>> newDict = new Dictionary<int, List<AudioData>>();


            foreach (AudioData ad in data)
            {
                if (!newDict.ContainsKey(ad.Line))
                {
                    newDict.Add(ad.Line, new List<AudioData>());
                    newDict[ad.Line].Add(ad);
                }
                else
                {
                    newDict[ad.Line].Add(ad);
                }
            }


            return newDict;
        }

        public void ChangeAudioData(Dictionary<int, List<AudioData>> newDict)
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

                List<AudioData> currentData = null;

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

                    if (mvarInNamespace)
                    {
                        mvarNamespaceOut.PlaySound(mvarSounds["namespace"], CallingContext.Volume);
                    }

                    if (mvarCurrentAD.TryGetValue(i, out currentData))
                    {
                        foreach (var sound in currentData)
                        {
                            PlaySound(sound, CallingContext.Volume);
                        }
                    }

                    CallingContext.CurrentBeat++;

                    double waitTime = (60.00 / CallingContext.BPM) * 1000;

                    DateTime startTime = DateTime.UtcNow;

                    while (DateTime.UtcNow < startTime.AddMilliseconds(waitTime))
                    {
                        if (mvarNewData)
                        {
                            mvarCurrentAD = new Dictionary<int, List<AudioData>>(mvarHoldingData);
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
