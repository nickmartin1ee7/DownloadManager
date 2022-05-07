using System;

namespace DownloadManager.ViewModels;

public class JobReportModel
{
    public Uri Uri { get; init; }

    // public ushort ProcessCompleted =>
    //     (ushort)(BytesTransferred * 100 / FileSize);

    public ulong FileSize { get; set; }

    public ulong BytesTransferred { get; set; }

    public JobReportModel(Uri uri)
    {
        Uri = uri;
    }

    public JobReportModel(string uri)
    {
        Uri = new Uri(uri);
    }

    public override string ToString() =>
        Uri.AbsoluteUri;
}