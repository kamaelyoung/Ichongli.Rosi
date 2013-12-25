using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Entitys
{
    public class Post
    {
        public int id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string thumbnail { get; set; }
        public string content { get; set; }

        private List<string> _attachs;
        public List<string> attachs
        {
            get
            {
                if (this._attachs == null)
                    this._attachs = new List<string>();

                return this._attachs;
            }

        }
    }

    public class Posts
    {
        public string status { get; set; }
        public int count { get; set; }
        public int count_total { get; set; }
        public int pages { get; set; }
        public List<Post> posts { get; set; }
        public string error { get; set; }
    }
}
