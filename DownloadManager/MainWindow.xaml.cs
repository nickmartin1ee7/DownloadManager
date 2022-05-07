using System;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DownloadManager.ViewModels;

namespace DownloadManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Timer _timer = new(50);
        private readonly StringBuilder _outputTextBoxBuffer = new();
        private readonly MainWindowViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new(text =>
            {
                if (!_timer.Enabled) _timer.Start();

                _outputTextBoxBuffer.Append(text);
            });
            
            _timer.Elapsed += (_, _) =>
                Dispatcher.Invoke(() =>
                    OutputTextBox.Text = _outputTextBoxBuffer.ToString());

            DataContext = _vm;

            SaveLocationTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private void SaveLocationTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SaveLocationTextBox.Foreground = _vm.HandleSaveLocation(SaveLocationTextBox.Text)
                ? new SolidColorBrush(Colors.Green)
                : new SolidColorBrush(Colors.Red);
        }
    }
}
