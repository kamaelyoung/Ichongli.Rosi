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
using System.Net.Http;

namespace Ichongli.Rosi.Services
{
    public class DownloadHelper : IDownloadHelper
    {
        public async Task<BitmapImage> GetImage(string url, bool go = false)
        {
            byte[] imageBytes = null;
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    imageBytes = new byte[stream.Length];
                    stream.Read(imageBytes, 0, imageBytes.Length);
                }

                var imageStream = new MemoryStream(imageBytes);

                BitmapImage image = new BitmapImage();
                image.SetSource(imageStream);
                return image;
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
        }
    }
}
