using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeSonification
{
    public class AudioData
    {
        private bool mvarStaticSound;
        private bool mvarInherits;
        private MuteType mvarMute;
        private string mvarName;
        private Instrument mvarInstrument;
        private int mvarLine;
        private int mvarLength;
        private TypeSyntax mvarReturnType;
        private int mvarParamCount;
        private string mvarParentMethod;

        public AudioData(string Name, int line, bool isStatic, Instrument instrument, MuteType mute, bool inherits)
        {
            mvarStaticSound = isStatic;
            mvarMute = mute;
            mvarName = Name;
            mvarInherits = inherits;
            mvarInstrument = instrument;
            mvarLine = line;
            mvarLength = 1;
            mvarReturnType = null;
            mvarParamCount = -1;
            mvarParentMethod = null;
        }

        public int Line
        {
            get { return mvarLine; }
            set { mvarLine = value; }
        }

        public string Name
        {
            get { return mvarName; }
        }

        public Instrument InstrumentType 
        {
            get { return mvarInstrument; }
        }

        public string ParentMethod
        {
            get { return mvarParentMethod; }
            set { mvarParentMethod = value; }
        }

        public int ParamCount
        {
            get { return mvarParamCount; } 
            set { mvarParamCount = value; }
        }

        public TypeSyntax ReturnType
        {
            get { return mvarReturnType; }
            set { mvarReturnType = value; }
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
