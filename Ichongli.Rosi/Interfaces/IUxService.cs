using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Interfaces
{
    public interface IUxService
    {
        void ShowAlert(string title, string m);
        Task ShowAlertFor2Seconds(string title, string m);
        void ShowToast(string m);

        string PrepHTML(string content, string BackgroundColor, string FontColor);

        string CleanHTML(string content);

        string BackgroundColor();

        string FontColor();

        void OpenIe(string url);

        void Share(string url, string title);

        void SendEmail(string destinationEmail, string subject, string body);

        bool HasLiveTile(string Uri);

        void CreateLiveTile(string Uri);

        void StartAgent();

        void EndAgent();

        bool AgentEnable();
    }
}
