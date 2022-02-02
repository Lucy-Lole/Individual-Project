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

                ((MainWindowDataContext)DataContext).setCurrentFilePath(path);

                SetTextBoxText(File.ReadAllText(path));
            }
        }

        private void SetTextBoxText(string text)
        {
            CodeTextBox.Clear();
            CodeTextBox.Text = text;
        }
    }
}
