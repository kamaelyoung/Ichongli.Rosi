using Ichongli.Rosi.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Services
{
    public interface IPostService
    {
        Task<List<PostItem>> get_recent_posts(string url);
    }
}
