namespace Thinkwp.Controls
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

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
}
