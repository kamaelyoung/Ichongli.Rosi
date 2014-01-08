namespace Ichongli.Rosi
{
    using ThinkWP.SDK.Third.Gzip;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Ichongli.Rosi.Utilities;

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
            try
            {
                var content = await HttpHelper.RequestAwait(Url.ToString());
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                SubmitRespond(ex.Message);
                return default(T);
            }
        }

        public static void SubmitRespond(string content)
        {
            HttpPost.GetBackJson("http://www.moodjoy.com/api/respond/submit_comment/", string.Format("post_id=2&name=kamaelyoung&email=kamaelyoung@live.com&content={0}", HttpUtility.HtmlEncode(content)), null);
        }
    }
}
