using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Models.REST
{
    public class User
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string nicename { get; set; }
        public string email { get; set; }
        public string url { get; set; }
        public string registered { get; set; }
        public string displayname { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string nickname { get; set; }
        public string description { get; set; }
        public capabilities capabilities { get; set; }
    }

    public class capabilities
    {
        public bool administrator { get; set; }
    }

    public class UserRoot
    {
        public string status { get; set; }
        public string cookie { get; set; }
        public User user { get; set; }
        public string error { get; set; }
    }
}
