using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Models.REST
{
    public class Nonce
    {
        public string status { get; set; }
        public string controller { get; set; }
        public string method { get; set; }
        public string nonce { get; set; }
    }
}
