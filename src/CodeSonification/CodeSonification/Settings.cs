using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSonification
{
    class Settings
    {
        private int mvarBPM;
        private double mvarVolume;

        public Settings()
        {
            mvarBPM = 80;
            mvarVolume = 0.5;
        }

        public int BPM
        {
            get { return mvarBPM; }
            set { mvarBPM = value; }
        }

        public double Volume
        {
            get { return mvarVolume; }
            set { mvarVolume = value; }
        }
    }
}
