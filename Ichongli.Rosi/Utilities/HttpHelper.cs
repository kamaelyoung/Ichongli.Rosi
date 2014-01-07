using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ThinkWP.SDK.Third.Gzip;

namespace Ichongli.Rosi.Utilities
{
    public class HttpHelper
    {
        public static Task<string> RequestAwait(string url)
        {
            return Task.Run(async () =>
            {
                string json = string.Empty;
                HttpWebResponse response = null;
                try
                {
                    HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                    request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";

                    response = (HttpWebResponse)await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, TaskCreationOptions.None);
                    var gzip = response.Headers[HttpRequestHeader.ContentEncoding];
                    using (var stream = response.GetResponseStream())
                    {
                        if (gzip != null)
                        {
                            byte[] gzipBs = new byte[stream.Length];
                            await stream.ReadAsync(gzipBs, 0, gzipBs.Length);
                            json = GZipStream.UncompressString(gzipBs);
                        }
                        else
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                json = await reader.ReadToEndAsync();
                            }
                        }
                    }
                    if (response != null)
                        response.Close();
                }
                catch
                {
                    json = null;
                }
                return json;
            });
        }
    }
}
