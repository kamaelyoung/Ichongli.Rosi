using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Services
{
    public class ServiceUser : Interfaces.IServiceUser
    {
        private const string registerUrl = "?json=user/register&username={0}&nonce={1}&display_name={2}&email={3}&password={4}";
        public async Task<Models.REST.Register> Register(string username, string email, string password, string display_name)
        {
            var nonce = await IChongliHelper.get_nonce("User", "Register");
            if (nonce.status == "ok")
            {
                StringBuilder Url = new StringBuilder();
                Url.Append(IChongliHelper.baseUrl);
                Url.AppendFormat(registerUrl, username, nonce.nonce, display_name, email, password);
                return await IChongliHelper.DoHttpGet<Models.REST.Register>(Url);
            }
            else
            {
                return new Models.REST.Register() { status = "error", msg = "验证失败" };
            }
        }

    }
}
