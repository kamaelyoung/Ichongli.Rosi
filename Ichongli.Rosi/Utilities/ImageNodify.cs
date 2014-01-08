namespace Ichongli.Rosi.Utilities
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Media.Imaging;
    using System.IO.IsolatedStorage;
    using System.IO;
    using System.Threading;
    using System.Windows.Threading;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Windows.Storage;

    public class ImageNodify
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached(
                "Source",
                typeof(string),
                typeof(ImageNodify),
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
                                    using (Stream stream = await new WebClient().OpenReadTaskAsync(new Uri(url, UriKind.Absolute)))
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
                                using (Stream stream = await new WebClient().OpenReadTaskAsync(new Uri(url, UriKind.Absolute)))
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
    }
}
