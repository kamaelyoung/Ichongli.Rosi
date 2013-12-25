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
        public async Task<Posts> get_recent_posts(int pageIndex)
        {
            var url = string.Format(IChongliHelper.get_recent_postsUrl, pageIndex);
            var json = await IChongliHelper.RequestAwait(url);
            return JsonTryParse.Parse<Posts>(json);
        }
    }
}
