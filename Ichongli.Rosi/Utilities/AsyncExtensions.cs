using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Ichongli.Rosi.Utilities
{
    public static class AsyncExtensions
    {
        public static Task<Stream> OpenReadTaskAsync(this WebClient client, Uri uri)
        {
            var taskComplete = new TaskCompletionSource<Stream>();
            client.OpenReadCompleted += (sender, args) =>
            {
                try
                {
                    taskComplete.TrySetResult(args.Result);
                }
                catch
                {
                    taskComplete.TrySetResult(null);
                }
            };
            client.OpenReadAsync(uri);
            return taskComplete.Task;
        }

    }

    public static class ImageExtensions
    {
        public static BitmapImage ToBitmapImage(this byte[] bytes)
        {
            try
            {
                BitmapImage bitImage = new BitmapImage();
                bitImage.SetSource(new MemoryStream(bytes));
                return bitImage;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
