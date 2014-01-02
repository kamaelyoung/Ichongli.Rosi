namespace Ichongli.Rosi.Services
{
    using Ichongli.Rosi.Interfaces;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Navigation;

    public class ProgressService : IProgressService
    {
        const string DefaultIndicatorText = "Loading...";
        private readonly ProgressIndicator progressIndicator;

        public ProgressService(PhoneApplicationFrame rootFrame)
        {
            progressIndicator = new ProgressIndicator { Text = DefaultIndicatorText };

            rootFrame.Navigated += RootFrameOnNavigated;
        }

        private void RootFrameOnNavigated(object sender, NavigationEventArgs args)
        {
            var content = args.Content;
            var page = content as PhoneApplicationPage;
            if (page == null)
                return;

            page.SetValue(SystemTray.ProgressIndicatorProperty, progressIndicator);
        }

        public void Show()
        {
            Show(DefaultIndicatorText);
        }

        public void Show(string text)
        {
            progressIndicator.Text = text;
            progressIndicator.IsIndeterminate = true;
            progressIndicator.IsVisible = true;
            SystemTray.Opacity = 0.6;
        }

        public void Hide()
        {
            progressIndicator.IsIndeterminate = false;
            progressIndicator.IsVisible = false;
            SystemTray.Opacity = 0;
        }
    }
}
