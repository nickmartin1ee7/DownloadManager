﻿using System;
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
        private readonly Action<Action> _dispatch;
        private DirectoryInfo? _saveLocation;

        public ObservableCollection<JobReportModel> Jobs { get; } = new();

        public ICommand AddCommand { get; }

        public ICommand StartJobsCommand { get; }

        public MainWindowViewModel(Action<string> updateLogOutput,
            Action<Action> dispatch)
        {
            _updateLogOutput = updateLogOutput;
            _dispatch = dispatch;
            AddCommand = new Command(Add);
            StartJobsCommand = new Command(async o =>
            {
                _dispatch(RemoveCompletedJobs);
                await StartJobsAsync(o);
            });
        }

        private void RemoveCompletedJobs()
        {
            var jobsToRemove = Jobs.Where(jobReport => jobReport.PercentageComplete == 100).ToArray();

            foreach (var jobReport in jobsToRemove)
            {
                Jobs.Remove(jobReport);
            }
        }

        private void Add(object? _)
        {
            // TODO popup window
            Jobs.Add(new JobReportModel((uint)Jobs.Count + 1, "http://pi.hole"));
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

        private async Task RunJobAsync(JobReportModel jobReport)
        {
            try
            {
                using var client = new WebClient();

                client.DownloadProgressChanged += (o, e) =>
                {
                    jobReport.PercentageComplete = e.ProgressPercentage;
                };

                Log($"Starting job {jobReport.Id}...");

                var sw = new Stopwatch();
                sw.Start();

                var filePath = CreateNewFilePath(Path.GetFileName(jobReport.Uri.LocalPath));

                await File.Create(filePath).DisposeAsync();

                await client.DownloadFileTaskAsync(jobReport.Uri, filePath);

                sw.Stop();

                Log($"Job {jobReport.Id} finished! ({sw.ElapsedMilliseconds} ms)");
            }
            catch (Exception e)
            {
                Log($"Job {jobReport.Id} failed due to: {e.Message}");
            }
        }

        private string CreateNewFilePath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) fileName = "index.html";

            var fileInfo = new FileInfo(Path.Combine(_saveLocation!.FullName, fileName));

            while (fileInfo.Exists)
            {
                var filePath = Path.Combine(_saveLocation!.FullName, Path.GetFileNameWithoutExtension(fileInfo.Name) + "_copy" + fileInfo.Extension);
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
