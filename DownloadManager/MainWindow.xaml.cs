using System;
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
        private readonly MainWindowViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new(text => OutputTextBox.Text = text);
            DataContext = _vm;

            SaveLocationTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private void SaveLocationTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var isValid = _vm.IsSaveLocationValid(SaveLocationTextBox.Text);

            SaveLocationTextBox.Foreground = isValid
                ? new SolidColorBrush(Colors.Green)
                : new SolidColorBrush(Colors.Red);

            if (isValid)
            {
                _vm.ChangeSaveLocation(SaveLocationTextBox.Text);
            }
        }
    }
}
