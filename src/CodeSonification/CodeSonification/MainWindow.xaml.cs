using System.Windows;
using System.IO;
using Microsoft.Win32;

namespace CodeSonification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowDataContext();

            InitializeComponent();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            string path = "";

            dlg.ShowDialog();
            if (dlg.FileName != "")
            {
                path = dlg.FileName;

                ((MainWindowDataContext)DataContext).SetCurrentFilePath(path);

                SetTextBoxText(File.ReadAllText(path));
            }
        }

        private void SetTextBoxText(string text)
        {
            CodeTextBox.Clear();
            CodeTextBox.Text = text;
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            AudioData data = ((MainWindowDataContext)DataContext).GetAudioData();

            if (((MainWindowDataContext)DataContext).BeginPlayback())
            {
                // Play the data returned.
            }
            else
            {
                // We have some issue.
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            // Stop 'playback'.
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((MainWindowDataContext)DataContext).SetVolume(e.NewValue);
        }
    }
}
