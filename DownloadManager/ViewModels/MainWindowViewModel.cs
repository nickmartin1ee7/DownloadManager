using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DownloadManager.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly Action<string> _updateLogOutput;
        private StringBuilder _sb = new();
        private DirectoryInfo? _saveLocation;

        public ObservableCollection<JobReportModel> Jobs { get; } = new();

        public ICommand AddCommand { get; }

        public ICommand StartJobsCommand { get; }

        public MainWindowViewModel(Action<string> updateLogOutput)
        {
            _updateLogOutput = updateLogOutput;
            AddCommand = new Command(Add);
            StartJobsCommand = new Command(async o =>
                await StartJobsAsync(o));
        }

        public bool IsSaveLocationValid(string text) =>
            Directory.Exists(text);

        private void Add(object? _)
        {
            // TODO popup window
            Jobs.Add(new JobReportModel("https://google.com"));
        }


        public void ChangeSaveLocation(string? saveLocationPath)
        {
            if (saveLocationPath is null) return;

            _saveLocation = new DirectoryInfo(saveLocationPath);
        }

        private async Task StartJobsAsync(object? _)
        {
            if (_saveLocation is null) return;
            if (!Jobs.Any()) return;

            var jobTasks = new List<Task>();

            for (var i = 0; i < Jobs.Count; i++)
            {
                var jobReport = Jobs[i];
                jobTasks.Add(new Task(async () =>
                {
                    using var client = new WebClient();
                    
                    client.DownloadProgressChanged += (o, e) =>
                    {
                        jobReport.BytesTransferred = (ulong) e.BytesReceived;
                        jobReport.FileSize = (ulong) e.TotalBytesToReceive;
                    };

                    Log($"Starting job {i+1}");

                    var sw = new Stopwatch();
                    sw.Start();
                    
                    await client.DownloadFileTaskAsync(jobReport.Uri,
                        Path.Combine(_saveLocation.FullName, jobReport.Uri.DnsSafeHost));
                    
                    sw.Stop();

                    Log($"Job {i+1} finished! ({sw.ElapsedMilliseconds} ms)");
                }));
            }

            var sw = new Stopwatch();
            sw.Start();
            
            await Task.WhenAll(jobTasks);

            sw.Stop();

            Log($"All jobs finished! ({sw.ElapsedMilliseconds} ms)");
        }

        private void Log(string message)
        {
            _sb.AppendLine($"[{DateTime.Now}] {message}");
            _updateLogOutput(_sb.ToString());
        }
    }
}
