using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSonification
{
    class MainWindowDataContext
    {
        private string mvarCurrentFilePath;
        private Settings mvarSettings;

        public MainWindowDataContext()
        {
            mvarCurrentFilePath = "";
            mvarSettings = new Settings();
        }

        public string getCurrentFilePath()
        {
            return mvarCurrentFilePath;
        }
        public void setCurrentFilePath(string value)
        {
            mvarCurrentFilePath = value;
        }
    }
}
