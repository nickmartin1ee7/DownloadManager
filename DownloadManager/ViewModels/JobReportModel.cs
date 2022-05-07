using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DownloadManager.ViewModels;

public class JobReportModel : INotifyPropertyChanged
{
    private int _percentageComplete;

    public Uri Uri { get; }

    public uint Id { get; }
    

    public int PercentageComplete
    {
        get => _percentageComplete;
        set
        {
            if (_percentageComplete == value) return;

            _percentageComplete = value;

            OnPropertyChanged(nameof(PercentageComplete));
        }
    }

    public JobReportModel(uint id, string uri)
    {
        Id = id;
        Uri = new Uri(uri);
    }

    public override string ToString() =>
        Uri.AbsoluteUri;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}