using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Models
{
    public class AppBase
    {

        private static AppBase current = new AppBase();
        public static AppBase Current
        {
            get { return current; }
            set { current = value; }
        }

        //private Models.ConfigurationFile config = null;
        //public Models.ConfigurationFile Config
        //{
        //    get { return config; }
        //    set { config = value; }
        //}

        private Models.REST.Categories.RootObject categories;

        public Models.REST.Categories.RootObject Categories
        {
            get { return categories; }
            set { categories = value; }
        }
        
        public ObservableCollection<Models.Ui.ItemWithUrl> Photos { get; set; }

        private Dictionary<string, Models.REST.CategoryPosts.RootObject> posts = new Dictionary<string, Models.REST.CategoryPosts.RootObject>();
        public Dictionary<string, Models.REST.CategoryPosts.RootObject> Posts
        {
            get { return posts; }
            set { posts = value; }
        }

        public Models.Ui.Item CurrentCategory = new Models.Ui.Item();
        public int CurrentPage = 0;
        public int CurrentPostId = -1;

        public Models.REST.CategoryPosts.RootObject GetPostsFrom(string id)
        {
            return Posts[id];
        }

        public void AddPostsFrom(string id, Models.REST.CategoryPosts.RootObject posts)
        {

            if (Posts.ContainsKey(id))
            {
                Posts[id].posts.AddRange(posts.posts);

            }
            else
                Posts.Add(id, posts);
        }

    }
}
