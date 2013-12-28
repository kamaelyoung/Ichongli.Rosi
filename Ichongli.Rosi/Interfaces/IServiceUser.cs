using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Interfaces
{
    public interface IServiceUser
    {
        Task<Models.REST.Register> Register(string username, string email, string password, string display_name);
    }
}
