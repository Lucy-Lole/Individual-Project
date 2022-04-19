﻿using Microsoft.CodeAnalysis;
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
        private List<AudioData> mvarStaticsData;
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

        public float Volume
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
            mvarStaticsData = new List<AudioData>();
            mvarLayer = LayerState.All;
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
                    mvarCurrentData = mvarClassData.Concat(mvarStaticsData).ToList();
                    break;

                case LayerState.Method:
                    mvarCurrentData = mvarMethodData.Concat(mvarStaticsData).ToList();
                    break;

                case LayerState.Internals:
                    mvarCurrentData = mvarInternalsData.Concat(mvarStaticsData).ToList();
                    break;

                case LayerState.All:
                default:
                    mvarCurrentData = mvarClassData.Concat(mvarMethodData.Concat(mvarInternalsData.Concat(mvarStaticsData).ToList()).ToList()).ToList();
                    break;
            }

            if (mvarPlaybackState == PlaybackState.Playing)
            {
                Dictionary<int, List<AudioData>> newData = new Dictionary<int, List<AudioData>>();

                newData = AudioController.CreateAudioDict(mvarCurrentData);

                mvarAudioController.ChangeAudioData(newData);
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

        private void RecurseFindData(CSharpSyntaxNode member)
        {
            // From method -> Body -> for each code block statement get their length and position
            // search through method for outside calls?

            if (member is StatementSyntax st)
            {
                if (st is IfStatementSyntax ifs)
                {
                    AudioData newData = new AudioData(ifs.Condition.ToString(),
                        ifs.GetLocation().GetLineSpan().StartLinePosition.Line,
                        false,
                        Instrument.codeBlock,
                        MuteType.normal,
                        false);

                    newData.Length = ifs.GetLocation().GetLineSpan().EndLinePosition.Line - ifs.GetLocation().GetLineSpan().StartLinePosition.Line;

                    mvarInternalsData.Add(newData);

                    if (ifs.Statement is BlockSyntax bs)
                    {
                        foreach(var child in bs.Statements)
                        {
                            RecurseFindData(child);
                        }
                    }

                    AudioData endData = new AudioData(ifs.Condition.ToString() + " END",
                        ifs.GetLocation().GetLineSpan().EndLinePosition.Line,
                        false,
                        Instrument.codeBlockEnd,
                        MuteType.normal,
                        false);

                    if (ifs.Else != null)
                    {
                        endData.Line = ifs.Else.GetLocation().GetLineSpan().StartLinePosition.Line - 1;

                        if (ifs.Else.Statement is BlockSyntax ebs)
                        {
                            AudioData elseData = new AudioData("else " + ebs.GetLocation().GetLineSpan().StartLinePosition.Line,
                                ebs.GetLocation().GetLineSpan().StartLinePosition.Line,
                                false,
                                Instrument.codeBlock,
                                MuteType.normal,
                                false);

                            elseData.Length = ebs.GetLocation().GetLineSpan().EndLinePosition.Line - ebs.GetLocation().GetLineSpan().StartLinePosition.Line;

                            mvarInternalsData.Add(elseData);

                            foreach (var child in ebs.Statements)
                            {
                                RecurseFindData(child);
                            }

                            AudioData elseEndData = new AudioData("else " + ebs.GetLocation().GetLineSpan().EndLinePosition.Line + " END",
                                ebs.GetLocation().GetLineSpan().EndLinePosition.Line,
                                false,
                                Instrument.codeBlockEnd,
                                MuteType.normal,
                                false);

                            mvarInternalsData.Add(elseEndData);
                        }


                        RecurseFindData(ifs.Else.Statement);
                    }

                    mvarInternalsData.Add(endData);
                }
                else if (st is ForStatementSyntax fss)
                {
                    AudioData newData = new AudioData(fss.Condition.ToString(),
                        fss.GetLocation().GetLineSpan().StartLinePosition.Line,
                        false,
                        Instrument.codeBlock,
                        MuteType.normal,
                        false);

                    newData.Length = fss.GetLocation().GetLineSpan().EndLinePosition.Line - fss.GetLocation().GetLineSpan().StartLinePosition.Line;

                    mvarInternalsData.Add(newData);

                    if (fss.Statement is BlockSyntax bs)
                    {
                        foreach (var child in bs.Statements)
                        {
                            RecurseFindData(child);
                        }
                    }

                    AudioData endData = new AudioData(fss.Condition.ToString() + " END",
                        fss.GetLocation().GetLineSpan().EndLinePosition.Line,
                        false,
                        Instrument.codeBlockEnd,
                        MuteType.normal,
                        false);

                    mvarInternalsData.Add(endData);
                }
                else if (st is WhileStatementSyntax wss)
                {
                    AudioData newData = new AudioData(wss.Condition.ToString(),
                        wss.GetLocation().GetLineSpan().StartLinePosition.Line,
                        false,
                        Instrument.codeBlock,
                        MuteType.normal,
                        false);

                    newData.Length = wss.GetLocation().GetLineSpan().EndLinePosition.Line - wss.GetLocation().GetLineSpan().StartLinePosition.Line;

                    mvarInternalsData.Add(newData);

                    if (wss.Statement is BlockSyntax bs)
                    {
                        foreach (var child in bs.Statements)
                        {
                            RecurseFindData(child);
                        }
                    }

                    AudioData endData = new AudioData(wss.Condition.ToString() + " END",
                        wss.GetLocation().GetLineSpan().EndLinePosition.Line,
                        false,
                        Instrument.codeBlockEnd,
                        MuteType.normal,
                        false);

                    mvarInternalsData.Add(endData);
                }
                else if (st is UsingStatementSyntax uss)
                {
                    AudioData newData = new AudioData(uss.Declaration.ToString(),
                        uss.GetLocation().GetLineSpan().StartLinePosition.Line,
                        false,
                        Instrument.codeBlock,
                        MuteType.normal,
                        false);

                    newData.Length = uss.GetLocation().GetLineSpan().EndLinePosition.Line - uss.GetLocation().GetLineSpan().StartLinePosition.Line;

                    mvarInternalsData.Add(newData);

                    if (uss.Statement is BlockSyntax bs)
                    {
                        foreach (var child in bs.Statements)
                        {
                            RecurseFindData(child);
                        }
                    }

                    AudioData endData = new AudioData(uss.Declaration.ToString() + " END",
                        uss.GetLocation().GetLineSpan().EndLinePosition.Line,
                        false,
                        Instrument.codeBlockEnd,
                        MuteType.normal,
                        false);

                    mvarInternalsData.Add(endData);
                }
                else if (st is ForEachStatementSyntax fess)
                {
                    AudioData newData = new AudioData(fess.Expression.GetText().ToString(),
                        fess.GetLocation().GetLineSpan().StartLinePosition.Line,
                        false,
                        Instrument.codeBlock,
                        MuteType.normal,
                        false);

                    newData.Length = fess.GetLocation().GetLineSpan().EndLinePosition.Line - fess.GetLocation().GetLineSpan().StartLinePosition.Line;

                    mvarInternalsData.Add(newData);

                    if (fess.Statement is BlockSyntax bs)
                    {
                        foreach (var child in bs.Statements)
                        {
                            RecurseFindData(child);
                        }
                    }

                    AudioData endData = new AudioData(fess.Expression.GetText().ToString() + " END",
                        fess.GetLocation().GetLineSpan().EndLinePosition.Line,
                        false,
                        Instrument.codeBlockEnd,
                        MuteType.normal,
                        false);

                    mvarInternalsData.Add(endData);
                }
                else if (st is ExpressionStatementSyntax ess)
                {
                    if (ess.Expression is InvocationExpressionSyntax ies)
                    {
                        AudioData newData = new AudioData(ies.Expression.GetText().ToString(),
                            ies.GetLocation().GetLineSpan().StartLinePosition.Line,
                            false,
                            Instrument.expression,
                            MuteType.normal,
                            false);

                        newData.Length = 1;

                        mvarInternalsData.Add(newData);
                    }
                }
            }
            else if (member is FieldDeclarationSyntax fs)
            {
                AudioData newData = new AudioData(fs.Declaration.Variables.First().Identifier.Text,
                    fs.GetLocation().GetLineSpan().StartLinePosition.Line,
                    true,
                    Instrument.field,
                    MuteType.normal,
                    false);

                newData.ReturnType = fs.Declaration.Type;

                mvarMethodData.Add(newData);
            }
            else if (member is PropertyDeclarationSyntax ps)
            {
                AudioData newData = new AudioData(ps.Identifier.Text,
                    ps.GetLocation().GetLineSpan().StartLinePosition.Line,
                    true,
                    Instrument.property,
                    MuteType.normal,
                    false);

                newData.ReturnType = ps.Type;

                mvarMethodData.Add(newData);
            }
            else if (member is MethodDeclarationSyntax ms)
            {
                AudioData newData = new AudioData(ms.Identifier.Text,
                    ms.GetLocation().GetLineSpan().StartLinePosition.Line,
                    false,
                    Instrument.guitar,
                    ms.Modifiers.Count > 0 ? GetMuteType(ms.Modifiers.First().Text) : MuteType.normal,
                    false);

                newData.Length = ms.GetLocation().GetLineSpan().EndLinePosition.Line - ms.GetLocation().GetLineSpan().StartLinePosition.Line;
                newData.ParamCount = ms.ParameterList.Parameters.Count;


                AudioData endData = new AudioData(ms.Identifier.Text,
                    ms.GetLocation().GetLineSpan().EndLinePosition.Line,
                    false,
                    Instrument.guitarEnd,
                    ms.Modifiers.Count > 0 ? GetMuteType(ms.Modifiers.First().Text) : MuteType.normal,
                    false);

                endData.Length = 1;
                endData.ParamCount = ms.ParameterList.Parameters.Count;

                mvarMethodData.Add(newData);
                mvarMethodData.Add(endData);

                newData.ReturnType = ms.ReturnType;
                endData.ReturnType = ms.ReturnType;

                foreach (StatementSyntax child in ms.Body.Statements)
                {
                    RecurseFindData(child);
                }
            }
            else if (member is ConstructorDeclarationSyntax xs)
            {
                AudioData newData = new AudioData(xs.Identifier.Text,
                    xs.GetLocation().GetLineSpan().StartLinePosition.Line,
                    false,
                    Instrument.guitar,
                    xs.Modifiers.Count > 0 ? GetMuteType(xs.Modifiers.First().Text) : MuteType.normal,
                    false);

                newData.Length = xs.GetLocation().GetLineSpan().EndLinePosition.Line - xs.GetLocation().GetLineSpan().StartLinePosition.Line;
                newData.ParamCount = xs.ParameterList.Parameters.Count;


                AudioData endData = new AudioData(xs.Identifier.Text,
                    xs.GetLocation().GetLineSpan().EndLinePosition.Line,
                    false,
                    Instrument.guitarEnd,
                    xs.Modifiers.Count > 0 ? GetMuteType(xs.Modifiers.First().Text) : MuteType.normal,
                    false);

                endData.Length = 1;
                endData.ParamCount = xs.ParameterList.Parameters.Count;

                mvarMethodData.Add(newData);
                mvarMethodData.Add(endData);

                foreach (StatementSyntax child in xs.Body.Statements)
                {
                    RecurseFindData(child);
                }
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

                AudioData endData = new AudioData(cs.Identifier.Text,
                    cs.GetLocation().GetLineSpan().EndLinePosition.Line,
                    false,
                    Instrument.pianoEnd,
                    cs.Modifiers.Count > 0 ? GetMuteType(cs.Modifiers.First().Text) : MuteType.normal,
                    cs.BaseList != null);

                mvarClassData.Add(endData);

                foreach (var child in cs.Members)
                {
                    RecurseFindData(child);
                }
            }
            else if (member is NamespaceDeclarationSyntax ns)
            {
                AudioData newData = new AudioData(ns.Name.ToString(),
                    ns.GetLocation().GetLineSpan().StartLinePosition.Line,
                    true,
                    Instrument.namesp,
                    MuteType.normal,
                    false);

                newData.Length = ns.GetLocation().GetLineSpan().EndLinePosition.Line - ns.GetLocation().GetLineSpan().StartLinePosition.Line;

                mvarClassData.Add(newData);

                AudioData endData = new AudioData(ns.Name.ToString() + " END",
                    ns.GetLocation().GetLineSpan().EndLinePosition.Line,
                    true,
                    Instrument.namesp,
                    MuteType.normal,
                    false);

                mvarClassData.Add(endData);

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

            if (mvarCurrentCodeText == null)
            {
                return;
            }

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

                mvarStaticsData.Add(newData);
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
