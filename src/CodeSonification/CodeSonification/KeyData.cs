
using System.Windows.Input;

namespace CodeSonification
{
    public class KeyData
    {
        private Key mvarKey;
        private ModifierKeys mvarModifiers;
        private bool mvarHandledState;

        public KeyData(Key key, ModifierKeys modifiers)
        {
            this.mvarKey = key;
            this.mvarModifiers = modifiers;
        }

        public Key Key
        {
            get { return mvarKey; }
        }

        public ModifierKeys Modifiers
        {
            get { return mvarModifiers; }
        }

        public bool HandledState
        {
            get { return mvarHandledState; }
            set { mvarHandledState = value; }
        }
    }
}
