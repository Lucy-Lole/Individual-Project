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

        public int GetBPM()
        {
            return mvarBPM;
        }

        public void SetBPM(int value)
        {
            mvarBPM = value;
        }

        public double GetVolume()
        {
            return mvarVolume;
        }
        public void SetVolume(double value)
        {
            mvarVolume = value;
        }
    }
}
