using Ichongli.Rosi.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Services
{
    public class ServiceBroker : IServiceBroker
    {
        private const string baseUrl = "http://www.moodjoy.com/";
        private const string CategoryIndex = "api/core/get_category_index/";
        private const string PostFromCategory = "api/core/get_category_posts/?include=id,title,thumbnail&category_id={0}&page={1}&count=12";
        private const string LatestPosts = "api/get_recent_posts/?include=id,title,thumbnail&page={0}&count=12";
        private const string Post = "api/get_post/?post_id={0}";

        public async Task<Models.REST.Categories.RootObject> GetCategories()
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(baseUrl);
            Url.Append(CategoryIndex);
            return await DoHttpGet<Models.REST.Categories.RootObject>(Url);
        }

        public async Task<Models.REST.CategoryPosts.RootObject> GetPostsFrom(string id, int page)
        {
            try
            {
                StringBuilder Url = new StringBuilder();
                Url.Append(baseUrl);
                Url.AppendFormat(PostFromCategory, id, page);
                Debug.WriteLine(Url.ToString());
                return await DoHttpGet<Models.REST.CategoryPosts.RootObject>(Url);
            }
            catch { return new Models.REST.CategoryPosts.RootObject() { status = "error" }; }
        }

        public async Task<Models.REST.CategoryPosts.RootObject> GetAppreCommended()
        {
            try
            {
                StringBuilder Url = new StringBuilder();
                Url.Append("http://www.thinkwp.org/api/get_category_posts/?slug=apprecommended");
                Debug.WriteLine(Url.ToString());
                return await DoHttpGet<Models.REST.CategoryPosts.RootObject>(Url);
            }
            catch { return new Models.REST.CategoryPosts.RootObject() { status = "error" }; }
        }

        public async Task<Models.REST.CategoryPosts.RootObject> GetLatestPosts(int page)
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(baseUrl);
            Url.AppendFormat(LatestPosts, page);
            return await DoHttpGet<Models.REST.CategoryPosts.RootObject>(Url);
        }

        public async Task<Models.REST.CategoryPosts.RootPost> GetPostById(int id)
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(baseUrl);
            Url.AppendFormat(Post, id);
            return await DoHttpGet<Models.REST.CategoryPosts.RootPost>(Url);
        }


        private async Task<T> DoHttpGet<T>(StringBuilder Url)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(Url.ToString());
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(content);
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (HttpRequestException ex)
            {
                SubmitRespond("HttpRequestException" + ex.Message);
                return default(T);
            }
            catch (JsonReaderException jsex)
            {
                SubmitRespond("JsonReaderException:" + jsex.Message);
                return default(T);
            }
        }

        public async void SubmitRespond(string content)
        {
            await PostAsync("http://www.moodjoy.com/api/respond/submit_comment/", string.Format("post_id=2&name=kamaelyoung&email=kamaelyoung@live.com&content={0}", HttpUtility.HtmlEncode(content)));
        }

        public async Task<Stream> PostAsync(string RequestUrl, string Context)
        {
            try
            {
                var responseContent = new StringContent(Context, Encoding.UTF8, "application/x-www-form-urlencoded");

                var cookie = new CookieContainer();

                var handler = new HttpClientHandler() { CookieContainer = cookie };

                var client = new HttpClient(handler);

                HttpResponseMessage httpResponse = await client.PostAsync(RequestUrl, responseContent);
                httpResponse.EnsureSuccessStatusCode();
                responseContent.Dispose();
                return await httpResponse.Content.ReadAsStreamAsync();
            }
            catch (HttpRequestException ex)
            {
                SubmitRespond("HttpRequestException" + ex.Message);
                return null;
            }
        }
    }
}
