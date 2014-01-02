using Ichongli.Rosi.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Services
{
    public class ServiceBroker : IServiceBroker
    {
        private const string CategoryIndex = "api/core/get_category_index/";
        private const string PostFromCategory = "api/core/get_category_posts/?include=id,title,thumbnail&category_id={0}&page={1}";
        private const string LatestPosts = "api/get_recent_posts/?include=id,title,thumbnail&page={0}";
        private const string Post = "api/get_post/?post_id={0}";


        public async Task<Models.REST.Categories.RootObject> GetCategories()
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(IChongliHelper.baseUrl);
            Url.Append(CategoryIndex);
            return await IChongliHelper.DoHttpGet<Models.REST.Categories.RootObject>(Url);
        }

        public async Task<Models.REST.CategoryPosts.RootObject> GetPostsFrom(string id, int page)
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(IChongliHelper.baseUrl);
            Url.AppendFormat(PostFromCategory, id, page);
            Debug.WriteLine(Url.ToString());
            return await IChongliHelper.DoHttpGet<Models.REST.CategoryPosts.RootObject>(Url);
        }

        public async Task<Models.REST.CategoryPosts.RootObject> GetLatestPosts(int page)
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(IChongliHelper.baseUrl);
            Url.AppendFormat(LatestPosts, page);
            return await IChongliHelper.DoHttpGet<Models.REST.CategoryPosts.RootObject>(Url);
        }

        public async Task<Models.REST.CategoryPosts.RootPost> GetPostById(int id)
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(IChongliHelper.baseUrl);
            Url.AppendFormat(Post, id);
            return await IChongliHelper.DoHttpGet<Models.REST.CategoryPosts.RootPost>(Url);
        }
    }
}
