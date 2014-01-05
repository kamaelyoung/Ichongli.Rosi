using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Ichongli.Rosi.Interfaces
{

    public interface IDownloadHelper
    {
        Task<BitmapImage> GetImage(string url, bool go = false);
    }
}
