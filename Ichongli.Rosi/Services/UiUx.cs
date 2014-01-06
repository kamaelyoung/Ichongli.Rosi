using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using Ichongli.Rosi.Interfaces;
using Ichongli.Rosi.ViewModels;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TaskEx = System.Threading.Tasks.Task;

namespace Ichongli.Rosi.Services
{
    public class UiUx : Interfaces.IUxService
    {
        private readonly IDownloadHelper _downloadHelper;
        
        private readonly IWindowManager _windowManager;
        public UiUx(IWindowManager windowManager, IDownloadHelper downloadHelper)
        {
            this._windowManager = windowManager;
            this._downloadHelper = downloadHelper;
        }

        public async Task ShowAlertFor2Seconds(string title, string m)
        {
            var dialogviewModel = new MessageViewModel
            {
                Title = title,
                Text = m,
            };
            _windowManager.ShowPopup(dialogviewModel);

            await TaskEx.Delay(TimeSpan.FromSeconds(2));
            dialogviewModel.TryClose();
        }

        public void ShowAlert(string title, string m)
        {
            var dialogviewModel = new MessageViewModel
            {
                Title = title,
                Text = m,
            };
            _windowManager.ShowPopup(dialogviewModel);
        }

        public void ShowToast(string m)
        {
            ToastPrompt toast = new ToastPrompt();
            toast.FontSize = 20;
            toast.Title = "";
            toast.Message = m;
            // toast.ImageSource = new BitmapImage(new Uri("/ApplicationIcon.png", UriKind.RelativeOrAbsolute));
            toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;
            toast.Show();
        }

        public void Share(string url, string title)
        {
            ShareLinkTask taskL = new ShareLinkTask();
            taskL.LinkUri = new Uri(url);
            taskL.Message = "";
            taskL.Title = title;
            taskL.Show();
        }

        public void OpenIe(string Url)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask { Uri = new Uri(Url, UriKind.RelativeOrAbsolute) };
            webBrowserTask.Show();
        }

        public string CleanHTML(string baseHTML)
        {
            string expn = "<.*?>";

            baseHTML = baseHTML.Replace("&nbsp;", "");
            var Result = Regex.Replace(baseHTML, expn, string.Empty);
            return HttpUtility.HtmlDecode(Result);
        }

        public string PrepHTML(string baseHTML, string BackgroundColor, string FontColor)
        {

            string expn = "<.*?>";

            baseHTML = baseHTML.Replace("&nbsp;", "");
            var Result = Regex.Replace(baseHTML, expn, string.Empty);

            if (Result.Length > 2000)
            {
                Result = Result.Substring(0, 2000) + "...";
            }

            return HttpUtility.HtmlDecode(Result);

            //            string o = "";
            //            o += "<html><head>";

            //            //prevent zooming
            //            o += "<meta name='viewport' content='width=320,user-scalable=no'/>";

            //            //inject the theme
            //            o += "<style type='text/css'>" +
            //                "body {{font-size:1.0em;background-color:#c6bea6;" +
            //                "color:" + FontColor + ";}} " + "</style>";

            //            //inject the script to pass link taps out of the browser
            //            o += "<script type='text/javascript'>";
            //            o += @"function getLinks(){ 
            //                a = document.getElementsByTagName('a');
            //                    for(var i=0; i < a.length; i++){
            //                    var msg = a[i].href;
            //                    a[i].onclick = function() {notify(msg);
            //                    };
            //                    }
            //                    }
            //                    function notify(msg) {
            //                    window.external.Notify(msg);
            //                    event.returnValue=false;
            //                    return false;
            //                }";

            //            //inject the script to find height
            //            o += @"function Scroll() {
            //                            var elem = document.getElementById('content');
            //                            window.external.Notify(elem.scrollHeight + '');
            //                        }
            //                    ";

            //            //remove all anchors
            //            while (baseHTML.Contains("<a class=\"anchor\""))
            //            {
            //                int start = baseHTML.IndexOf("<a class=\"anchor\"");
            //                int end = baseHTML.IndexOf("</h2>", start);

            //                baseHTML = baseHTML.Remove(start, end - start);
            //            }

            //            //FIXME remove this when we fix the webbrowser
            //            //remove all links
            //            while (baseHTML.Contains("<a href"))
            //            {
            //                //remove most of the link
            //                int start = baseHTML.IndexOf("<a href");
            //                int end = baseHTML.IndexOf(">", start);

            //                baseHTML = baseHTML.Remove(start, end + 1 - start);

            //                //remove end tag
            //                start = baseHTML.IndexOf("</a>", start);
            //                baseHTML = baseHTML.Remove(start, "</a>".Length);
            //            }

            //            o += @"window.onload = function() {
            //                    Scroll();
            //                    getLinks();
            //                }";

            //            o += "</script>";
            //            o += "</head>";
            //            o += "<body><div id='content'>";
            //            o += baseHTML.Trim();
            //            o += "</div></body>";
            //            o += "</html>";
            //            return o;
        }

        public string FontColor()
        {
            return Application.Current.Resources["Mainforground"].ToString();
        }
        public string BackgroundColor()
        {
            return Application.Current.Resources["MainBackground"].ToString();
        }

        public void SendEmail(string destinationEmail, string subject, string body)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = destinationEmail;
            emailComposeTask.Subject = subject;
            emailComposeTask.Body = body;
            emailComposeTask.Show();
        }

        public bool HasLiveTile(string Uri)
        {
            if (!string.IsNullOrEmpty(Uri))
                return ShellTile.ActiveTiles.Any(o => o.NavigationUri == new Uri(Uri, UriKind.RelativeOrAbsolute));
            else
                return ShellTile.ActiveTiles.Count() > 1 ? true : false;
        }

        public void CreateLiveTile(string Uri)
        {
            //if (ShellTile.ActiveTiles.Any())
            //{
            //    var tile = ShellTile.ActiveTiles.First();
            //    var flipTileData = new RadFlipTileData
            //    {
            //        Count = 0,
            //        Title = Domain.AppBase.Current.Config.AppName,
            //        IsTransparencySupported = true,
            //        //Title = International.Translations.AppName,
            //        //BackTitle = International.Translations.AppName,
            //        BackgroundImage = new Uri("/Assets/FlipCycleTileSmall_159_159.png", UriKind.RelativeOrAbsolute),
            //        WideBackgroundImage = new Uri("/Assets/FlipCycleTitleLarge_691_336.png", UriKind.RelativeOrAbsolute),
            //        BackBackgroundImage = new Uri("/", UriKind.RelativeOrAbsolute),
            //        WideBackBackgroundImage = new Uri("/", UriKind.RelativeOrAbsolute),
            //        BackTitle = ""

            //    };
            //    if (!string.IsNullOrEmpty(Uri))
            //        LiveTileHelper.CreateOrUpdateTile(flipTileData, new Uri(Uri, UriKind.RelativeOrAbsolute), true, true);
            //    else
            //        LiveTileHelper.CreateOrUpdateTile(flipTileData, new Uri("/Home.xaml", UriKind.RelativeOrAbsolute), true, true);
            //}

        }

        public void StartAgent()
        {
            //WP_to_WP.Shared.Code.PeriodicTaskClient.Current.Start();
        }

        public void EndAgent()
        {
            //WP_to_WP.Shared.Code.PeriodicTaskClient.Current.Remove();
        }

        public bool AgentEnable()
        {
            return false;
            //return WP_to_WP.Shared.Code.PeriodicTaskClient.Current.Exists();
        }

        public async Task SaveImage(Ichongli.Rosi.Models.Ui.ItemWithUrl item)
        {
            if (item != null)
            {
                this.ShowToast("保存图片...");
                try
                {
                    var bitmapImage = await _downloadHelper.GetImage(item.ItemImage.Large);
                    WriteableBitmap wb = new WriteableBitmap(bitmapImage);
                    using (MemoryStream resource_0 = new MemoryStream())
                    {
                        System.Windows.Media.Imaging.Extensions.SaveJpeg(wb, (Stream)resource_0, wb.PixelWidth, wb.PixelHeight, 0, 100);
                        resource_0.Seek(0L, SeekOrigin.Begin);
                        var library = new MediaLibrary();
                        library.SavePicture(item.Title + ".jpg", (Stream)resource_0);
                    }

                    this.ShowToast("保存成功");
                }
                catch
                {
                    this.ShowToast("保存失败");
                }
            }
            else
            {
                this.ShowToast("不知道该保存什么了...");
            }
        }

        public async Task SetLockscreen(string url)
        {
            await SetLockscreenInternal(url);
        }

        private async Task SetLockscreenInternal(string url)
        {
            try
            {
                var isProvider = Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication;
                if (!isProvider)
                {
                    // If you're not the provider, this call will prompt the user for permission.
                    // Calling RequestAccessAsync from a background agent is not allowed.
                    var op = await Windows.Phone.System.UserProfile.LockScreenManager.RequestAccessAsync();

                    // Only do further work if the access was granted.
                    isProvider = op == Windows.Phone.System.UserProfile.LockScreenRequestResult.Granted;
                }

                // Create a filename for JPEG file in isolated storage.
                string fileName;
                try
                {
                    var currentImage = Windows.Phone.System.UserProfile.LockScreen.GetImageUri();

                    if (currentImage.ToString().EndsWith("_A.jpg"))
                    {
                        fileName = "LiveLockBackground_B.jpg";
                    }
                    else
                    {
                        fileName = "LiveLockBackground_A.jpg";
                    }
                }
                catch (Exception e)
                {
                    fileName = "LiveLockBackground_A.jpg";
                }

                var lockImage = string.Format("{0}", fileName);

                // Create virtual store and file stream. Check for duplicate tempJPEG files.
                using (System.IO.IsolatedStorage.IsolatedStorageFile myIsolatedStorage = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (myIsolatedStorage.FileExists(lockImage))
                    {
                        myIsolatedStorage.DeleteFile(lockImage);
                    }

                    System.IO.IsolatedStorage.IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(lockImage);

                    System.Windows.Resources.StreamResourceInfo sri = null;
                    Uri uri = new Uri(lockImage, UriKind.Relative);
                    sri = Application.GetResourceStream(uri);

                    var bitmapImage = await _downloadHelper.GetImage(url);
                    WriteableBitmap wb = new WriteableBitmap(bitmapImage);

                    // Encode WriteableBitmap object to a JPEG stream.
                    Extensions.SaveJpeg(wb, fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);

                    //wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                    fileStream.Close();
                }

                // call function to set downloaded image as lock screen 
                await LockHelper(lockImage, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task LockHelper(string filePathOfTheImage, bool isAppResource)
        {
            try
            {
                var isProvider = Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication;
                if (!isProvider)
                {
                    // If you're not the provider, this call will prompt the user for permission.
                    // Calling RequestAccessAsync from a background agent is not allowed.
                    var op = await Windows.Phone.System.UserProfile.LockScreenManager.RequestAccessAsync();

                    // Only do further work if the access was granted.
                    isProvider = op == Windows.Phone.System.UserProfile.LockScreenRequestResult.Granted;
                }

                if (isProvider)
                {
                    // At this stage, the app is the active lock screen background provider.

                    // The following code example shows the new URI schema.
                    // ms-appdata points to the root of the local app data folder.
                    // ms-appx points to the Local app install folder, to reference resources bundled in the XAP package.
                    var schema = isAppResource ? "ms-appx:///" : "ms-appdata:///Local/";
                    var uri = new Uri(schema + filePathOfTheImage, UriKind.Absolute);

                    // Set the lock screen background image.
                    Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);

                    // Get the URI of the lock screen background image.
                    var currentImage = Windows.Phone.System.UserProfile.LockScreen.GetImageUri();
                    System.Diagnostics.Debug.WriteLine("The new lock screen background image is set to {0}", currentImage.ToString());
                    this.ShowToast("设置成功.");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}
