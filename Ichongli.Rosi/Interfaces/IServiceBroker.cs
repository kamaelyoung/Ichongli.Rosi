using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Interfaces
{
    public interface IServiceBroker
    {
        Task<Models.REST.Categories.RootObject> GetCategories();
        Task<Models.REST.CategoryPosts.RootObject> GetAppreCommended();
        Task<Models.REST.CategoryPosts.RootObject> GetPostsFrom(string id, int page);

        Task<Models.REST.CategoryPosts.RootObject> GetLatestPosts(int page);

        Task<Models.REST.CategoryPosts.RootPost> GetPostById(int id);
    }
}
