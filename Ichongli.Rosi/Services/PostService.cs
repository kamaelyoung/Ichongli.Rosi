namespace Ichongli.Rosi.Services
{
    using Ichongli.Rosi.Entitys;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PostService : IPostService
    {
        public async Task<List<PostItem>> get_recent_posts(string url)
        {
            var result = new List<PostItem>();

            return result;
        }
    }
}
