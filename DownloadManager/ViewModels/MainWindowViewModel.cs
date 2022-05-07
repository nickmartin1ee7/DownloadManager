using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DownloadManager.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly Action<string> _updateLogOutput;
        private int _lastJobId;
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

        private void Add(object? _)
        {
            // TODO popup window
            Jobs.Add(new JobReportModel("http://pi.hole"));
        }


        public bool HandleSaveLocation(string? saveLocationPath)
        {
            if (saveLocationPath is null || !Directory.Exists(saveLocationPath)) return false;

            _saveLocation = new DirectoryInfo(saveLocationPath);
            return true;
        }

        private async Task StartJobsAsync(object? _)
        {
            if (_saveLocation is null) return;
            if (!Jobs.Any()) return;

            var jobTasks = new List<Task>();

            Log($"Enqueueing {Jobs.Count} job(s)...)");

            for (var i = 0; i < Jobs.Count; i++)
            {
                var jobReport = Jobs[i];
                jobTasks.Add(new Task(async () => await RunJobAsync(jobReport)));
            }

            var sw = new Stopwatch();
            sw.Start();

            Parallel.ForEach(jobTasks, (job) => job.Start());
            await Task.WhenAll(jobTasks);

            sw.Stop();

            Log($"All jobs finished! ({sw.ElapsedMilliseconds} ms)");
        }

        private int GetNewJobId() =>
            ++_lastJobId;

        private async Task RunJobAsync(JobReportModel jobReport)
        {
            var jobId = GetNewJobId();

            try
            {
                using var client = new WebClient();

                client.DownloadProgressChanged += (o, e) =>
                {
                    jobReport.BytesTransferred = (ulong)e.BytesReceived;
                    jobReport.FileSize = (ulong)e.TotalBytesToReceive;
                };

                Log($"Starting job {jobId}...");

                var sw = new Stopwatch();
                sw.Start();

                var filePath = CreateNewFilePath(Path.GetFileName(jobReport.Uri.LocalPath));

                await File.Create(filePath).DisposeAsync();

                await client.DownloadFileTaskAsync(jobReport.Uri, filePath);

                sw.Stop();

                Log($"Job {jobId} finished! ({sw.ElapsedMilliseconds} ms)");
            }
            catch (Exception e)
            {
                Log($"Job {jobId} failed due to: {e.Message}");
            }
        }

        private string CreateNewFilePath(string fileName)
        {
            var fileInfo = new FileInfo(Path.Combine(_saveLocation!.FullName, fileName));

            while (fileInfo.Exists)
            {
                var filePath = Path.GetFileNameWithoutExtension(fileInfo.Name) + "_copy" + fileInfo.Extension;
                fileInfo = new FileInfo(filePath);
            }

            return fileInfo.FullName;
        }

        private void Log(string message)
        {
            _updateLogOutput($"[{DateTime.Now}] {message}{Environment.NewLine}");
        }
    }
}
