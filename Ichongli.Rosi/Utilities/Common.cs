using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Ichongli.Rosi.Utilities
{
    public class Common
    {
        public static WriteableBitmap GetWriteableBitmap(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            WriteableBitmap writeableBitmap = new WriteableBitmap(148, 248);
            return writeableBitmap;
        }
    }
}
