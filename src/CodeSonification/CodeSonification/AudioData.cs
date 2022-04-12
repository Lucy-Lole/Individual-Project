using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSonification
{
    class AudioData
    {
        private bool mvarStaticSound;
        private bool mvarInherits;
        private MuteType mvarMute;
        private string mvarName;
        private Instrument mvarInstrument;
        private int mvarLine;
        private int mvarLength;

        public AudioData(string Name, int line, bool isStatic, Instrument instrument, MuteType mute, bool inherits)
        {
            mvarStaticSound = isStatic;
            mvarMute = mute;
            mvarName = Name;
            mvarInherits = inherits;
            mvarInstrument = instrument;
            mvarLine = line;
            mvarLength = 1;
        }

        public int Line
        {
            get { return mvarLine; }
        }

        public int Length
        {
            get { return mvarLength; }
            set { mvarLength = value; }
        }

        public bool Inherits
        {
            get { return mvarInherits; }
        }

        public MuteType Mute
        {
            get { return mvarMute; }
        }

        public bool IsStaticSound
        {
            get { return mvarStaticSound; }
        }
    }
}
