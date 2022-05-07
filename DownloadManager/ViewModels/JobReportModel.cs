using System;

namespace DownloadManager.ViewModels;

public class JobReportModel
{
    public Uri Uri { get; }

    public uint Id { get; }

    public ulong FileSize { get; set; }

    public ulong BytesTransferred { get; set; }
    
    public JobReportModel(uint id, string uri)
    {
        Id = id;
        Uri = new Uri(uri);
    }

    public override string ToString() =>
        Uri.AbsoluteUri;
}