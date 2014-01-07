using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Services
{
    public class ServiceAuth : Interfaces.IServiceAuth
    {
        private const string loginUrl = "api/auth/generate_auth_cookie/";
        private const string currentuserinfoUrl = "api/auth/get_currentuserinfo/?cookie={0}";
        private const string validate_auth_cookieUrl = "api/auth/validate_auth_cookie/?cookie={0}";
        public async Task<Models.REST.UserRoot> generate_auth_cookie(string username, string password)
        {
            var nonce = await IChongliHelper.get_nonce("auth", "generate_auth_cookie");
            if (nonce.status == "ok")
            {
                StringBuilder Url = new StringBuilder();
                Url.Append(IChongliHelper.baseUrl);
                Url.AppendFormat(loginUrl, username, nonce.nonce, password);
                var auth = await HttpPost.GetBackJson(IChongliHelper.baseUrl + loginUrl, string.Format("username={0}&nonce={1}&password={2}", username, nonce.nonce, password), null);
                if (auth != null && !string.IsNullOrEmpty(auth))
                    return JsonConvert.DeserializeObject<Models.REST.UserRoot>(auth);
                else
                    return new Models.REST.UserRoot() { status = "error", error = "" };
            }
            else
            {
                return new Models.REST.UserRoot() { status = "error", error = nonce.error };
            }
        }

        public async Task<Models.REST.Validate> validate_auth_cookie(string cookie)
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(IChongliHelper.baseUrl);
            Url.AppendFormat(validate_auth_cookieUrl, cookie);
            return await IChongliHelper.DoHttpGet<Models.REST.Validate>(Url);
        }

        public async Task<Models.REST.UserRoot> get_currentuserinfo(string cookie)
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(IChongliHelper.baseUrl);
            Url.AppendFormat(currentuserinfoUrl, cookie);
            return await IChongliHelper.DoHttpGet<Models.REST.UserRoot>(Url);

        }
    }
}
