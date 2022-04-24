using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeSonification;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace CodeSonificationTests
{
    [TestClass]
    public class AudioDataTests
    {
        [TestMethod]
        public void AudioDataTest()
        {
            var ad = new AudioData("Test", 99, true, Instrument.guitar, MuteType.mute, true);
            ad.Length = 99;
            ad.ParamCount = 99;

            Assert.AreEqual("Test", ad.Name);
            Assert.AreEqual(99, ad.Line);
            Assert.AreEqual(99, ad.Length);
            Assert.AreEqual(99, ad.ParamCount);
            Assert.AreEqual(null, ad.ReturnType);
            Assert.AreEqual(MuteType.mute, ad.Mute);
            Assert.AreEqual(true, ad.IsStaticSound);
            Assert.AreEqual(true, ad.Inherits);
            Assert.AreEqual(Instrument.guitar, ad.InstrumentType);
        }
    }

    [TestClass]
    public class LayerToCheckedConverterTests
    {
        [TestMethod]
        public void ConvertTest()
        {
            LayerToCheckedConverter conv = new LayerToCheckedConverter();

            LayerState ls = LayerState.All;
            var result = conv.Convert(ls, null, "All", null);
            Assert.AreEqual(true, result);

            ls = LayerState.Class;
            result = conv.Convert(ls, null, "Class", null);
            Assert.AreEqual(true, result);

            ls = LayerState.Method;
            result = conv.Convert(ls, null, "Method", null);
            Assert.AreEqual(true, result);

            ls = LayerState.Internals;
            result = conv.Convert(ls, null, "Internals", null);
            Assert.AreEqual(true, result);

            result = conv.Convert(ls, null, "All", null);
            Assert.AreEqual(false, result);

            result = conv.Convert(ls, null, null, null);
            Assert.AreEqual(false, result);

            result = conv.Convert(null, null, "Internals", null);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ConvertBackTest()
        {
            LayerToCheckedConverter conv = new LayerToCheckedConverter();
            try
            {
                var test = conv.ConvertBack(null, null, null, null);

                Assert.AreEqual(null, test);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(NotImplementedException));
            }
        }
    }

    [TestClass]
    public class MainWindowDataContextTests
    {
        private MainWindowDataContext mvarDataContext;

        private const string TestAudioData =
            "using System; class SampleClass {public SampleMethod() {if (){}}}";
        private const string TestAudioData2 =
            "using System; class SampleClass {public SampleMethod() {}}";
        private const string TestAudioData3 =
            "using System; class SampleClass {}";
        private const string TestAudioData4 =
            "using System;";
        private const string TestAudioData5 =
            "";

        [TestMethod]
        public void ConstructorTest()
        {
            mvarDataContext = new MainWindowDataContext();
            Assert.AreEqual(mvarDataContext.Layer, LayerState.All);
            Assert.AreEqual(mvarDataContext.CurrentState, PlaybackState.Stopped);
        }

        [TestMethod]
        public void GetAudioDataTest()
        {
            mvarDataContext = new MainWindowDataContext();
            mvarDataContext.Layer = LayerState.All;

            mvarDataContext.CurrentCodeText = TestAudioData;
            mvarDataContext.GetAudioData();
            mvarDataContext.ApplyCurrentLayer();
            Assert.AreEqual(7, mvarDataContext.CurrentData.Count);
            Assert.AreEqual(0, mvarDataContext.TotalBeats);

            mvarDataContext.CurrentCodeText = TestAudioData2;
            mvarDataContext.GetAudioData();
            mvarDataContext.ApplyCurrentLayer();
            Assert.AreEqual(5, mvarDataContext.CurrentData.Count);

            mvarDataContext.CurrentCodeText = TestAudioData3;
            mvarDataContext.GetAudioData();
            mvarDataContext.ApplyCurrentLayer();
            Assert.AreEqual(3, mvarDataContext.CurrentData.Count);

            mvarDataContext.CurrentCodeText = TestAudioData4;
            mvarDataContext.GetAudioData();
            mvarDataContext.ApplyCurrentLayer();
            Assert.AreEqual(1, mvarDataContext.CurrentData.Count);

            mvarDataContext.CurrentCodeText = TestAudioData5;
            mvarDataContext.GetAudioData();
            mvarDataContext.ApplyCurrentLayer();
            Assert.AreEqual(0, mvarDataContext.CurrentData.Count);

        }

        [TestMethod]
        public void ApplyCurrentLayerTest()
        {
            mvarDataContext = new MainWindowDataContext();

            mvarDataContext.CurrentCodeText = TestAudioData;
            mvarDataContext.GetAudioData();

            mvarDataContext.Layer = LayerState.All;
            mvarDataContext.ApplyCurrentLayer();
            Assert.AreEqual(7, mvarDataContext.CurrentData.Count);

            mvarDataContext.Layer = LayerState.Class;
            mvarDataContext.ApplyCurrentLayer();
            Assert.AreEqual(3, mvarDataContext.CurrentData.Count);

            mvarDataContext.Layer = LayerState.Method;
            mvarDataContext.ApplyCurrentLayer();
            Assert.AreEqual(3, mvarDataContext.CurrentData.Count);

            mvarDataContext.Layer = LayerState.Internals;
            mvarDataContext.ApplyCurrentLayer();
            Assert.AreEqual(3, mvarDataContext.CurrentData.Count);
        }

        [TestMethod]
        public void GetMuteTypeTest()
        {
            mvarDataContext = new MainWindowDataContext();

            Assert.AreEqual(MuteType.mute, mvarDataContext.GetMuteType("private"));
            Assert.AreEqual(MuteType.normal, mvarDataContext.GetMuteType("public"));
            Assert.AreEqual(MuteType.slight, mvarDataContext.GetMuteType("protected"));
            Assert.AreEqual(MuteType.slight, mvarDataContext.GetMuteType("internal"));
            Assert.AreEqual(MuteType.normal, mvarDataContext.GetMuteType(""));
        }

        [TestMethod]
        public void PlaybackTests()
        {
            mvarDataContext = new MainWindowDataContext();

            Assert.AreEqual(PlaybackState.Stopped, mvarDataContext.CurrentState);
            Assert.IsFalse(mvarDataContext.StopPlayback());

            Assert.IsTrue(mvarDataContext.BeginPlayback());
            Assert.AreEqual(PlaybackState.Playing, mvarDataContext.CurrentState);
            Assert.IsFalse(mvarDataContext.BeginPlayback());

            Assert.IsTrue(mvarDataContext.StopPlayback());
            Assert.AreEqual(PlaybackState.Stopped, mvarDataContext.CurrentState);
        }

        [TestMethod]
        public void ChangeLayerTest()
        {
            mvarDataContext = new MainWindowDataContext();

            Assert.AreEqual(LayerState.All, mvarDataContext.Layer);

            mvarDataContext.ChangeLayer("ClassButton");
            Assert.AreEqual(LayerState.Class, mvarDataContext.Layer);

            mvarDataContext.ChangeLayer("MethodButton");
            Assert.AreEqual(LayerState.Method, mvarDataContext.Layer);

            mvarDataContext.ChangeLayer("InternalButton");
            Assert.AreEqual(LayerState.Internals, mvarDataContext.Layer);

            mvarDataContext.ChangeLayer("");
            Assert.AreEqual(LayerState.All, mvarDataContext.Layer);
        }

        [TestMethod]
        public void KeyPressTest()
        {
            mvarDataContext = new MainWindowDataContext();

            KeyData kd = new KeyData(Key.Space, ModifierKeys.Control);
            Assert.IsTrue(mvarDataContext.HandleKeyPress(ref kd));
            Assert.IsTrue(kd.HandledState);

            kd = new KeyData(Key.Left, ModifierKeys.None);
            mvarDataContext.Layer = LayerState.Internals;
            Assert.IsFalse(mvarDataContext.HandleKeyPress(ref kd));
            Assert.AreEqual(LayerState.Method, mvarDataContext.Layer);
            Assert.IsTrue(kd.HandledState);

            kd = new KeyData(Key.Right, ModifierKeys.None);
            mvarDataContext.Layer = LayerState.All;
            Assert.IsFalse(mvarDataContext.HandleKeyPress(ref kd));
            Assert.AreEqual(LayerState.Class, mvarDataContext.Layer);
            Assert.IsTrue(kd.HandledState);

            kd = new KeyData(Key.None, ModifierKeys.None);
            mvarDataContext.Layer = LayerState.Internals;
            Assert.IsFalse(mvarDataContext.HandleKeyPress(ref kd));
            Assert.AreEqual(LayerState.Internals, mvarDataContext.Layer);
            Assert.IsFalse(kd.HandledState);

        }
    }
}
