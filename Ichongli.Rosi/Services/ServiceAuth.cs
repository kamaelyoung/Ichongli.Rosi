using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Services
{
    public class ServiceAuth : Interfaces.IServiceAuth
    {
        private const string loginUrl = "api/auth/generate_auth_cookie/?username={0}&nonce={1}&password={4}";
        private const string currentuserinfoUrl = "api/auth/get_currentuserinfo/?cookie={0}";
        private const string validate_auth_cookieUrl = "api/auth/validate_auth_cookie/?cookie={0}";
        public async Task<string> generate_auth_cookie(string username, string password)
        {
            var nonce = await IChongliHelper.get_nonce("auth", "generate_auth_cookie");
            if (nonce.status == "ok")
            {
                StringBuilder Url = new StringBuilder();
                Url.Append(IChongliHelper.baseUrl);
                Url.AppendFormat(loginUrl, username, nonce.nonce, password);
                //HttpPost.GetBackJson("http://www.moodjoy.com/api/respond/submit_comment/", string.Format(respond, HttpUtility.HtmlEncode(content)), null);

                return await IChongliHelper.DoHttpGet<string>(Url);
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<string> validate_auth_cookie(string cookie)
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(IChongliHelper.baseUrl);
            Url.AppendFormat(validate_auth_cookieUrl, cookie);
            return await IChongliHelper.DoHttpGet<string>(Url);
        }

        public async Task<string> get_currentuserinfo(string cookie)
        {
            StringBuilder Url = new StringBuilder();
            Url.Append(IChongliHelper.baseUrl);
            Url.AppendFormat(currentuserinfoUrl, cookie);
            return await IChongliHelper.DoHttpGet<string>(Url);

        }
    }
}
