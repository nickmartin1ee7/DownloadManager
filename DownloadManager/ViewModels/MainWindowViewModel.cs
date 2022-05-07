using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace DownloadManager.ViewModels
{
    public class MainWindowViewModel
    {
        private StringBuilder _sb = new();
        private FileInfo? _saveLocation;

        public ObservableCollection<JobReportModel> Jobs { get; } = new();

        public ICommand AddCommand { get; }

        public ICommand ChangeSaveLocationCommand { get; set; }

        public MainWindowViewModel()
        {
            AddCommand = new Command(Add);
            ChangeSaveLocationCommand = new Command(ChangeSaveLocation);
        }

        public bool IsSaveLocationValid(string text) =>
            Directory.Exists(text);

        private void Add(object? _)
        {
            // TODO popup window
            Jobs.Add(new JobReportModel("https://google.com"));
        }


        private void ChangeSaveLocation(object? saveLocationPath)
        {
            var path = saveLocationPath as string;

            if (!File.Exists(path)) return;

            _saveLocation = new FileInfo(path);
        }

        private void Log(string message)
        {
            _sb.AppendLine($"[{DateTime.Now}] {message}");
        }
    }
}
