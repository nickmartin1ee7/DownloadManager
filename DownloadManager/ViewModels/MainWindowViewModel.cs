using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager.ViewModels
{
    public class MainWindowViewModel
    {
        public ObservableCollection<JobModel> Type { get; set; }
    }

    public class JobModel
    {
        public Uri Uri { get; set; }
        
        public int ProcessCompleted { get; set; }

        public override string ToString()
        {
            return Uri.AbsoluteUri;
        }
    }
}
