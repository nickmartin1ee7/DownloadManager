using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager.ViewModels
{
    public class MainWindowViewModel
    {
        public ObservableCollection<JobReportModel> Jobs { get; set; }
    }
}
