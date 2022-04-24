namespace CodeSonification
{
    class Settings
    {
        private int mvarBPM;
        private float mvarVolume;

        public Settings()
        {
            mvarBPM = 80;
            mvarVolume = 0.5f;
        }

        public int BPM
        {
            get { return mvarBPM; }
            set { mvarBPM = value; }
        }

        public float Volume
        {
            get { return mvarVolume; }
            set { mvarVolume = value; }
        }
    }
}
