using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Interfaces
{
    public interface IServiceAuth
    {
        Task<Models.REST.UserRoot> generate_auth_cookie(string username, string password);
        Task<Models.REST.Validate> validate_auth_cookie(string cookie);
        Task<Models.REST.UserRoot> get_currentuserinfo(string cookie);
    }
}
