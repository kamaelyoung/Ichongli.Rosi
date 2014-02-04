namespace Thinkwp.Controls
{
    using System;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using Windows.Storage;

    /// <summary>
    /// 功能：同系统Image控件一同使用，接管下载图片的工作，提供缓存功能，对于缓存中已存在的图片，将使用缓存图片
    /// 用法：作为附加属性写到Image控件内，其中Source属性为真实的网络图片地址，LoadingSource为加载图片过程中占位在Image内的“加载中”图片
    /// </summary>
    public class ThinkImage
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached(
                "Source",
                typeof(string),
                typeof(ThinkImage),
                new PropertyMetadata(null, OnSourceWithSourceChanged));

        public static string GetSource(Image img)
        {
            if (img == null)
            {
                return null;
            }
            return (string)img.GetValue(SourceProperty);
        }

        public static void SetSource(Image img, string value)
        {
            if (img != null)
            {
                img.SetValue(SourceProperty, value);
            }
        }

        private const string path = "ImageCache";
        private static IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
        private async static void OnSourceWithSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }
            var img = sender as Image;
            img.Source = null;
            img.Opacity = 0;

            string url = e.NewValue.ToString();
            img.Tag = url;

            await Task.Delay(100);

            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    //if (url.EndsWith(".webp"))
                    {
                        if (!isoFile.DirectoryExists(path))
                        {
                            isoFile.CreateDirectory(path);
                        }
                        string fileName = MD5.GetMd5String(url);
                        string filePath = System.IO.Path.Combine(path, fileName);
                        if (isoFile.FileExists(filePath))
                        {
                            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(filePath);
                            using (Stream s = await file.OpenStreamForReadAsync())
                            {
                                if (s.Length == 0)
                                {
                                    #region 请求本地失败
                                    img.Tag = url;
                                    var httpClient = new HttpClient();
                                    var response = await httpClient.GetAsync(url);
                                    response.EnsureSuccessStatusCode();
                                    using (var stream = await response.Content.ReadAsStreamAsync())
                                    {
                                        byte[] bytes = new byte[stream.Length];
                                        stream.Read(bytes, 0, bytes.Length);
                                        if (img.Tag.Equals(url))
                                        {
                                            if (!isoFile.FileExists(filePath))
                                            {
                                                using (var fileStream = new IsolatedStorageFileStream(filePath, FileMode.Create, isoFile))
                                                {
                                                    fileStream.Write(bytes, 0, bytes.Length);
                                                }
                                            }

                                            var source = bytes.ToBitmapImage();
                                            if (img.Tag.Equals(url))
                                            {
                                                img.Source = source;
                                                StoryBordImg(img);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    byte[] bytes = new byte[s.Length];
                                    s.Read(bytes, 0, bytes.Length);
                                    var source = bytes.ToBitmapImage();
                                    if (MD5.GetMd5String(img.Tag.ToString()).Equals(fileName))
                                    {
                                        img.Source = source;
                                        StoryBordImg(img);
                                    }
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                var httpClient = new HttpClient();
                                var response = await httpClient.GetAsync(url);
                                response.EnsureSuccessStatusCode();
                                using (var stream = await response.Content.ReadAsStreamAsync())
                                {
                                    byte[] bytes = new byte[stream.Length];
                                    await stream.ReadAsync(bytes, 0, bytes.Length);
                                    var source = bytes.ToBitmapImage();
                                    if (img.Tag.Equals(url))
                                    {
                                        img.Source = source;
                                        StoryBordImg(img);
                                        if (!isoFile.FileExists(filePath))
                                        {
                                            using (var fileStream = new IsolatedStorageFileStream(filePath, FileMode.Create, isoFile))
                                            {
                                                // App.Current.sizes += bytes.Length / 1024;
                                                fileStream.Write(bytes, 0, bytes.Length);
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                catch { }

            }
        }

        public static void StoryBordImg(Image img)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 1;
            anim.Duration = TimeSpan.FromMilliseconds(500);
            Storyboard.SetTarget(anim, img);
            Storyboard.SetTargetProperty(anim, new PropertyPath("Opacity"));
            sb.Children.Add(anim);
            sb.Begin();
        }

        #region Public Method

        /// <summary>
        /// 获取当前图片缓存占用的空间
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetImageCacheSizeAsync()
        {
            var total = await Task.Run(() =>
            {
                long totalsize = 0;
                if (isoFile.DirectoryExists(path))
                {
                    foreach (var fileName in isoFile.GetFileNames(path + "/"))
                    {
                        using (var file = isoFile.OpenFile(path + "/" + fileName, FileMode.Open))
                        {
                            totalsize += file.Length;
                        }
                    }
                }
                return totalsize;
            });
            return SizeSuffix(total);
        }

        /// <summary>
        /// 清理当前的图片缓存
        /// </summary>
        public static Task ClearImageCacheAsync()
        {
            return Task.Run(() =>
            {
                if (isoFile.DirectoryExists(path))
                {
                    foreach (var file in isoFile.GetFileNames(path + "/"))
                    {
                        isoFile.DeleteFile(path + "/" + file);
                    }
                }
            });
        }

        #endregion

        #region Helper Method

        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        /// <summary>
        /// 将bytes大小的数据转换成友好的显示
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static string SizeSuffix(long value)
        {
            if (value == 0)
                return "0 bytes";
            var mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1 << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        #endregion
    }
}
