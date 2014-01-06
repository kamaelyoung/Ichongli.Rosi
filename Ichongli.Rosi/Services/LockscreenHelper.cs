﻿using Caliburn.Micro;
using Ichongli.Rosi.Interfaces;
using Ichongli.Rosi.ViewModels;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Windows.Phone.System.UserProfile;
using Ichongli.Rosi.Utilities;
using System.IO;
using Windows.Storage;
using Coding4Fun.Toolkit.Controls;

namespace Ichongli.Rosi.Services
{
    public class LockscreenHelper : ILockscreenHelper
    {
        private readonly IDownloadHelper _downloadHelper;

        private readonly IWindowManager _windowManager;

        private readonly IUxService _uxService;

        private readonly IProgressService _progressService;

        public LockscreenHelper(IWindowManager windowManager, IDownloadHelper downloadHelper, IUxService uxService, IProgressService progressService)
        {
            this._downloadHelper = downloadHelper;
            this._windowManager = windowManager;
            this._uxService = uxService;
            this._progressService = progressService;
        }

        private const string path = "ImageCache";

        public async Task SaveImage(Ichongli.Rosi.Models.Ui.ItemWithUrl item)
        {
            if (item != null)
            {
                this._uxService.ShowToast("保存图片...");
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

                    this._uxService.ShowToast("保存成功");
                }
                catch
                {
                    this._uxService.ShowToast("保存失败");
                }
                finally
                {
                }
            }
            else
            {
                this._uxService.ShowToast("不知道该保存什么了...");
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
                    var currentImage = LockScreen.GetImageUri();

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
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (myIsolatedStorage.FileExists(lockImage))
                    {
                        myIsolatedStorage.DeleteFile(lockImage);
                    }

                    IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(lockImage);

                    StreamResourceInfo sri = null;
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
                    MessageBox.Show("Lockscreen has been set!");
                }
                else
                {
                    MessageBox.Show("You said no, so I can't update your background.");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}
