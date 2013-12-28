using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Services
{
    public class ServiceUser : Interfaces.IServiceUser
    {
        private const string nonceUrl = "?json=get_nonce&controller={0}&method={1}";
        private const string registerUrl = "?json=user/register&username={0}&nonce={1}&display_name={2}&email={3}&password={4}";
        public async Task<Models.REST.Register> Register(string username, string email, string password, string display_name)
        {
            var nonce = await this.get_nonce("", "");
            if (nonce.status == "ok")
            {
                StringBuilder Url = new StringBuilder();
                Url.Append(IChongliHelper.baseUrl);
                Url.AppendFormat(registerUrl, username, nonce.nonce, email, password, display_name);
                return await IChongliHelper.DoHttpGet<Models.REST.Register>(Url);
            }
            else
            {
                return new Models.REST.Register() { status = "error", msg = "验证失败" };
            }
        }

        private async Task<Models.REST.Nonce> get_nonce(string controller, string method)
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(IChongliHelper.baseUrl);
            Url.AppendFormat(nonceUrl, controller, method);
            return await IChongliHelper.DoHttpGet<Models.REST.Nonce>(Url);
        }
    }
}
