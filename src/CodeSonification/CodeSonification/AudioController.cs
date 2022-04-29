using System;
using System.Collections.Generic;
using System.Threading;

namespace CodeSonification
{
    public class AudioController
    {

        Dictionary<int, List<AudioData>> mvarCurrentAD;
        Dictionary<int, List<AudioData>> mvarHoldingData;

        OutputController mvarDustOut;
        OutputController mvarNamespaceOut;
        OutputController mvarOtherOut;

        int mvarIndentLevel = 0;
        bool mvarInNamespace = false;
        const int mvarMaxDust = 10;
        bool mvarNewData;
        int mvarDustSoundLevel;

        public int IndentLevel
        {
            get { return mvarIndentLevel; }
        }

        public bool InNamespace
        {
            get { return mvarInNamespace; }
        }

        public Dictionary<int, List<AudioData>> CurrentAudioData
        {
            get { return mvarCurrentAD; }
        }

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

            mvarDustOut = new OutputController();
            mvarOtherOut = new OutputController();
            mvarNamespaceOut = new OutputController();

        }

        /// <summary>
        /// Plays the sound that corresponds to the given AudioData.
        /// </summary>
        /// <param name="data">AudioData to play</param>
        /// <param name="vol">Volume to play at</param>
        public void PlaySound(AudioData data, float vol)
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

        /// <summary>
        /// Creates an dictionary of AudioData mapped to line number.
        /// </summary>
        /// <param name="data">List of AudioData to convert into dictionary</param>
        /// <returns>Dictionary of AudioData mapped to line number</returns>
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

        /// <summary>
        /// Changes the dictionary of AudioData that the playback thread is currently playing.
        /// </summary>
        /// <param name="newDict">Dictionary of AudioData to play</param>
        public void ChangeAudioData(Dictionary<int, List<AudioData>> newDict)
        {
            mvarHoldingData = newDict;
            mvarNewData = true;
        }

        /// <summary>
        /// Begins playback, utilizing the DataContext passed for AudioData information and BPM.
        /// </summary>
        /// <param name="data">Must be a initialised MainWindowDataContext object</param>
        /// <param name="ct">Must be a CancellationToken so that the thread can be stopped</param>
        public void StartPlayback(object data, object ct)
        {
            mvarDustSoundLevel = 0;
            mvarIndentLevel = 0;
            mvarInNamespace = false;

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
