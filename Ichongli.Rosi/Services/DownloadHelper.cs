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
            bool shouldGet = false;
            //string url = go ? imageMetaData.imageThumbnail : imageMetaData.imageUrl;
            try
            {
                using (var stream = await new WebClient().OpenReadTaskAsync(new Uri(url, UriKind.Absolute)))
                {
                    imageBytes = new byte[stream.Length];
                    stream.Read(imageBytes, 0, imageBytes.Length);
                }
            }
            catch (Exception e)
            {
                shouldGet = true;
            }
            if (shouldGet)
            {
                //imageBytes = await queue.Enqueue(1, async () =>
                //    {

                //        using (var client = new HttpClient())
                //        {
                //            byte[] tempimageBytes = null;
                //            try
                //            {
                //                tempimageBytes = await client.GetByteArrayAsync(url);
                //                await BlobCache.LocalMachine.Insert(url, tempimageBytes);
                //            }
                //            catch (Exception e)
                //            {
                //                //MessageBox.Show("Please check your network connection");
                //            }
                //            return tempimageBytes;
                //        }
                //    });
            }
            var imageStream = new MemoryStream(imageBytes);

            //BECAUSE WP8 SAID SO
            BitmapImage image = new BitmapImage();
            image.SetSource(imageStream);
            return image;
        }
    }
}
