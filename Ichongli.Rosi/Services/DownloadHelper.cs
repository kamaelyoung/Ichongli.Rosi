using Ichongli.Rosi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Ichongli.Rosi.Utilities;

namespace Ichongli.Rosi.Services
{
    public class DownloadHelper : IDownloadHelper
    {
        public async Task<BitmapImage> GetImage(string url, bool go = false)
        {
            byte[] imageBytes = null;
            try
            {
                using (var stream = await new WebClient().OpenReadTaskAsync(new Uri(url, UriKind.Absolute)))
                {
                    imageBytes = new byte[stream.Length];
                    stream.Read(imageBytes, 0, imageBytes.Length);
                }

                var imageStream = new MemoryStream(imageBytes);

                BitmapImage image = new BitmapImage();
                image.SetSource(imageStream);
                return image;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
