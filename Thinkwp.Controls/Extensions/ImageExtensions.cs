using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Thinkwp.Controls
{
    public static class ImageExtensions
    {
        public static BitmapImage ToBitmapImage(this byte[] bytes)
        {
            BitmapImage bitImage = new BitmapImage();
            bitImage.SetSource(new MemoryStream(bytes));
            return bitImage;
        }
    }
}
