using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ThinkWP.SDK.Third.Gzip;

namespace Ichongli.Rosi
{
    public class HttpPost
    {
        public static Task<string> GetBackJson(string url, string postData, IDictionary<string, string> dic)
        {
            return Task.Run(async () =>
            {
                string json = string.Empty;
                HttpWebResponse response = null;
                try
                {
                    HttpWebRequest request = HttpWebRequest.CreateHttp(new Uri(url));
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    
                    //返回应答请求异步操作的状态

                    using (Stream stream = await Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, TaskCreationOptions.None))
                    {
                        byte[] bs = System.Text.Encoding.UTF8.GetBytes(postData);
                        await stream.WriteAsync(bs, 0, bs.Length);
                    }
                   
                    response = (HttpWebResponse)await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, TaskCreationOptions.None);
                    // var gzip = response.Headers[HttpRequestHeader.ContentEncoding];
                    using (Stream s = response.GetResponseStream())
                    {
                        // GZipStream zip = new GZipStream(s, CompressionMode.Compress, true);
                        //if (gzip != null)
                        //{
                        //    byte[] gzipBs = new byte[s.Length];
                        //    await s.ReadAsync(gzipBs, 0, gzipBs.Length);
                        //    json = GZipStream.UncompressString(gzipBs);
                        //}
                        //else
                        //{
                        using (var sr = new StreamReader(s))
                        {
                            json = await sr.ReadToEndAsync();
                        }
                        //}
                    }
                }
                catch { }
                if (response != null)
                {
                    response.Close();
                }
                return json;
            });
        }
    }
}
