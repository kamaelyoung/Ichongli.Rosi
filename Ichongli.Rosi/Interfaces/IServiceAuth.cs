using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Interfaces
{
    public interface IServiceAuth
    {
        Task<string> generate_auth_cookie(string username, string password);
        Task<string> validate_auth_cookie(string cookie);
        Task<string> get_currentuserinfo(string cookie);
    }
}
