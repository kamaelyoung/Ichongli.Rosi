﻿using System;
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
        public const string baseUrl = "http://rosimm.ichongli.com/";

        public static Task<string> RequestAwait(string url)
        {
            return Task.Run(async () =>
            {
                string json = string.Empty;
                HttpWebResponse response = null;
                try
                {
                    HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;

                    response = (HttpWebResponse)await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, TaskCreationOptions.None);
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            json = await reader.ReadToEndAsync();
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
