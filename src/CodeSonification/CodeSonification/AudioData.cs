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

        public AudioData(string Name, int line, bool isStatic, Instrument instrument, MuteType mute, bool inherits)
        {
            mvarStaticSound = isStatic;
            mvarMute = mute;
            mvarName = Name;
            mvarInherits = inherits;
            mvarInstrument = instrument;
            mvarLine = line;
        }

        public bool GetInherits()
        {
            return mvarInherits;
        }

        public MuteType GetMute()
        {
            return mvarMute;
        }

        public bool GetIsStaticSound()
        {
            return mvarStaticSound;
        }
    }
}
