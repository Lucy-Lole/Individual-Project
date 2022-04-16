using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;

namespace CodeSonification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowDataContext mvarDataContext;

        public MainWindow()
        {
            mvarDataContext = new MainWindowDataContext();

            DataContext = mvarDataContext;

            InitializeComponent();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            string path;

            dlg.ShowDialog();
            if (dlg.FileName != "")
            {
                path = dlg.FileName;

                mvarDataContext.CurrentFilePath = path;
                mvarDataContext.CurrentCodeText = File.ReadAllText(path);
            }
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            mvarDataContext.BeginPlayback();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            mvarDataContext.StopPlayback();
            // Stop 'playback'.
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mvarDataContext.Volume = (float)e.NewValue;
        }

        private void BPM_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mvarDataContext.BPM = (int)e.NewValue;
        }

        private void Button_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                LayerState chosenLayer = LayerState.All;

                if (rb.Name == "ClassButton")
                {
                    chosenLayer = LayerState.Class;
                }
                else if (rb.Name == "MethodButton")
                {
                    chosenLayer = LayerState.Method;
                }
                else if (rb.Name == "InternalButton")
                {
                    chosenLayer = LayerState.Internals;
                }

                mvarDataContext.ChangeLayer(chosenLayer);
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Left)
            {
                if (mvarDataContext.Layer != LayerState.All)
                {
                    mvarDataContext.ChangeLayer(mvarDataContext.Layer - 1);
                }

                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.Right)
            {

                if (mvarDataContext.Layer != LayerState.Internals)
                {
                    mvarDataContext.ChangeLayer(mvarDataContext.Layer + 1);
                }
                e.Handled = true;
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            mvarDataContext.StopPlayback();
        }
    }
}
