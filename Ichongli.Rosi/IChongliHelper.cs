using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi
{
    public static class IChongliHelper
    {
        //public const string baseUrl = "http://appcnds.darkforcesteam.com.cn/";
        //public const string baseUrl = "http://rosimm.ichongli.com/";

        public const string baseUrl = "http://www.moodjoy.com/";

        private const string nonceUrl = "?json=get_nonce&controller={0}&method={1}";

        public async static Task<Models.REST.Nonce> get_nonce(string controller, string method)
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(IChongliHelper.baseUrl);
            Url.AppendFormat(nonceUrl, controller, method);
            return await IChongliHelper.DoHttpGet<Models.REST.Nonce>(Url);
        }

        public async static Task<T> DoHttpGet<T>(StringBuilder Url)
        {
            var content = await RequestAwait(Url.ToString());
            return JsonConvert.DeserializeObject<T>(content);
        }

        private static Task<string> RequestAwait(string url)
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
