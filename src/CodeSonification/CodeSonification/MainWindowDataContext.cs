using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CodeSonification
{
    class MainWindowDataContext : INotifyPropertyChanged
    {
        private string mvarCurrentFilePath;
        private string mvarCurrentCodeText;
        private Settings mvarSettings;
        private PlaybackState mvarPlaybackState;
        private AudioController mvarAudioController;
        private List<AudioData> mvarCurrentData;
        private List<AudioData> mvarClassData;
        private List<AudioData> mvarMethodData;
        private List<AudioData> mvarInternalsData;
        private LayerState mvarLayer;
        private int mvarDataPosition;
        private int mvarCurrentBeat;
        private int mvarTotalBeats;

        private Thread mvarPlaybackThread;
        private CancellationTokenSource mvarTokenSource;

        public event PropertyChangedEventHandler PropertyChanged;

        public int TotalBeats
        {
            get { return mvarTotalBeats; }
        }

        public LayerState Layer
        {
            get { return mvarLayer; }
            private set
            {
                mvarLayer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Layer"));
            }
        }

        public string CurrentCodeText
        {
            get { return mvarCurrentCodeText; }
            set
            {
                mvarCurrentCodeText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentCodeText"));
            }
        }

        public int CurrentBeat
        {
            get { return mvarCurrentBeat; }
            set
            {
                mvarCurrentBeat = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentBeat"));
            }
        }

        public List<AudioData> CurrentData
        {
            get { return mvarCurrentData; }
        }

        public string CurrentBPM
        {
            get
            {
                return mvarSettings.BPM.ToString();
            }
        }

        public string CurrentFilePath
        {
            get { return mvarCurrentFilePath; }
            set { mvarCurrentFilePath = value; }
        }

        public double Volume
        {
            get { return mvarSettings.Volume; }
            set { mvarSettings.Volume = value; }
        }

        public int BPM
        {
            get { return mvarSettings.BPM; }
            set { mvarSettings.BPM = value; }
        }

        public PlaybackState CurrentState
        {
            get { return mvarPlaybackState; }
        }

        private string[] mvarClassKeywords = {"public", "private", "protected", "internal"};

        
        public MainWindowDataContext()
        {
            mvarCurrentFilePath = "";
            mvarSettings = new Settings();
            mvarPlaybackState = PlaybackState.Stopped;
            mvarDataPosition = 0;
            mvarCurrentBeat = 0;
            mvarAudioController = new AudioController();
            mvarCurrentData = new List<AudioData>();
            mvarClassData = new List<AudioData>();
            mvarMethodData = new List<AudioData>();
            mvarInternalsData = new List<AudioData>();
            mvarLayer = LayerState.Method;
        }

        public void IncrementPosition()
        {
            if (mvarDataPosition < mvarCurrentData.Count())
            {
                mvarDataPosition++;
            }
        }

        public void DecrementPosition()
        {
            if (mvarDataPosition > 0)
            {
                mvarDataPosition--;
            }
        }

        private void ApplyCurrentLayer()
        {
            switch (mvarLayer)
            {
                case LayerState.Class:
                    mvarCurrentData = mvarClassData;
                    break;

                case LayerState.Method:
                    mvarCurrentData = mvarMethodData;
                    break;

                case LayerState.Internals:
                    mvarCurrentData = mvarInternalsData;
                    break;

                case LayerState.All:
                default:
                    mvarCurrentData = mvarClassData.Concat(mvarMethodData.Concat(mvarInternalsData).ToList()).ToList();
                    break;
            }
        }

        private MuteType GetMuteType(string word)
        {
            if (word == "private")
            {
                return MuteType.mute;
            }
            else if (word == "public")
            {
                return MuteType.normal;
            }
            else if (word == "protected" || word == "internal")
            {
                return MuteType.slight;
            }

            return MuteType.normal;
        }

        private void RecurseFindData(MemberDeclarationSyntax member)
        {
            if (member is MethodDeclarationSyntax ms)
            {
                AudioData newData = new AudioData(ms.Identifier.Text,
                    ms.GetLocation().GetLineSpan().StartLinePosition.Line,
                    false,
                    Instrument.guitar,
                    ms.Modifiers.Count > 0 ? GetMuteType(ms.Modifiers.First().Text) : MuteType.normal,
                    false);

                newData.Length = ms.GetLocation().GetLineSpan().EndLinePosition.Line - ms.GetLocation().GetLineSpan().StartLinePosition.Line;

                mvarMethodData.Add(newData);
            }
            else if (member is ClassDeclarationSyntax cs)
            {
                AudioData newData = new AudioData(cs.Identifier.Text, 
                    cs.GetLocation().GetLineSpan().StartLinePosition.Line, 
                    false, 
                    Instrument.piano,
                    cs.Modifiers.Count > 0 ? GetMuteType(cs.Modifiers.First().Text) : MuteType.normal, 
                    cs.BaseList != null);

                newData.Length = cs.GetLocation().GetLineSpan().EndLinePosition.Line - cs.GetLocation().GetLineSpan().StartLinePosition.Line;

                mvarClassData.Add(newData);

                foreach (var child in cs.Members)
                {
                    RecurseFindData(child);
                }
            }
            else if (member is NamespaceDeclarationSyntax ns)
            {
                foreach (var child in ns.Members)
                {
                    RecurseFindData(child);
                }
            }
        }

        public void GetAudioData()
        {
            mvarClassData = new List<AudioData>();
            mvarMethodData = new List<AudioData>();
            mvarInternalsData = new List<AudioData>();

            SyntaxTree tree = CSharpSyntaxTree.ParseText(mvarCurrentCodeText);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            foreach (UsingDirectiveSyntax us in root.Usings)
            {
                AudioData newData = new AudioData(us.Name.ToString(),
                    us.GetLocation().GetLineSpan().StartLinePosition.Line,
                    true,
                    Instrument.dust,
                    MuteType.normal,
                    false);

                mvarClassData.Add(newData);
            }

            foreach (var member in root.Members)
            {
                RecurseFindData(member);
            }

            mvarTotalBeats = root.GetLocation().GetLineSpan().EndLinePosition.Line - root.GetLocation().GetLineSpan().StartLinePosition.Line;
        }

        public bool BeginPlayback()
        {
            if (mvarPlaybackState == PlaybackState.Stopped)
            {
                GetAudioData();

                ApplyCurrentLayer();

                mvarPlaybackState = PlaybackState.Playing;

                CurrentBeat = 0;

                mvarTokenSource = new CancellationTokenSource();

                mvarPlaybackThread = new Thread(() => mvarAudioController.StartPlayback(this, mvarTokenSource.Token));

                mvarPlaybackThread.Start();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool StopPlayback()
        {
            if (mvarPlaybackState == PlaybackState.Playing)
            {
                CurrentBeat = 0;

                mvarTokenSource.Cancel();

                mvarPlaybackState = PlaybackState.Stopped;

                return true;
            }
            else
            {
                return false;
            }
        }

        public void ChangeLayer(LayerState newLayer)
        {
            Layer = newLayer;
            ApplyCurrentLayer();
        }
    }
}
