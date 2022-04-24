using System.Windows;
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
            dlg.ShowDialog();

            mvarDataContext.HandleImport(dlg.FileName);
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            mvarDataContext.BeginPlayback();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            mvarDataContext.StopPlayback();
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
                mvarDataContext.ChangeLayer(rb.Name);
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            KeyData kd = new KeyData(e.Key, e.KeyboardDevice.Modifiers);
            if(mvarDataContext.HandleKeyPress(ref kd))
            {
                mvarDataContext.PlayLine(CodeTextBox.GetLineIndexFromCharacterIndex(CodeTextBox.CaretIndex));
            }
            e.Handled = kd.HandledState;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            mvarDataContext.StopPlayback();
        }
    }
}
