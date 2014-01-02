
namespace Ichongli.Rosi.ViewModels
{
    using Caliburn.Micro;
    using System.Linq;
    using Ichongli.Rosi.Interfaces;
    using Ichongli.Rosi.Services;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Threading.Tasks;
    using System.Text.RegularExpressions;
    using Ichongli.Rosi.Models.Ui;
    using Ichongli.Rosi.Models;
    using System.Collections.Generic;
    using System;
    using System.IO.IsolatedStorage;
    using System.ComponentModel;
    using System.Windows.Threading;
    using Ichongli.Rosi.Utilities;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    public class MainPageViewModel : Screen, IHandle<SampleMessage>
    {
        private readonly INavigationService navigationService;
        private readonly IEventAggregator eventAggregator;
        private readonly IServiceBroker serviceBroker;
        private readonly IServiceUser _serviceUser;

        private ObservableCollection<Models.Ui.Item> _Categories = new ObservableCollection<Models.Ui.Item>();
        public ObservableCollection<Models.Ui.Item> Categories
        {
            get { return this._Categories; }
            set
            {
                if (_Categories != value)
                {
                    _Categories = value;
                    this.NotifyOfPropertyChange(() => Categories);
                }
            }
        }

        private ObservableCollection<Models.Ui.HomeItem> _Items = new ObservableCollection<Models.Ui.HomeItem>();
        public ObservableCollection<Models.Ui.HomeItem> Items
        {
            get { return this._Items; }
            set
            {
                if (_Items != value)
                {
                    _Items = value;
                    this.NotifyOfPropertyChange(() => Items);
                }
            }
        }

        private string _BigImage;
        public string BigImage
        {
            get { return this._BigImage; }
            set
            {
                _BigImage = value;
                NotifyOfPropertyChange(() => BigImage);
            }
        }
        private bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (value != isLoading)
                {
                    isLoading = value;
                    NotifyOfPropertyChange("IsLoading");
                }
            }
        }


        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IServiceBroker serviceBroker, IServiceUser serviceUser)
        {
            this.navigationService = navigationService;
            this.eventAggregator = eventAggregator;
            this.serviceBroker = serviceBroker;
            this._serviceUser = serviceUser;
            this.eventAggregator.Subscribe(this);
        }

        protected override async void OnInitialize()
        {
            this.isLoading = true;
            if (this.Categories.Count == 0)
            {
                try
                {
                    var categories = await serviceBroker.GetCategories();

                    if (categories.status.ToLower() == "ok")
                    {
                        var filtered = categories.categories.Where(o => o.parent == 0);
                        foreach (var item in filtered)
                        {
                            this.Categories.Add(new Models.Ui.Item { Title = item.title.ToLower(), ItemId = item.id.ToString() });
                        }

                        await LoadLastPosts();
                    }
                    else
                    {
                    }
                }
                catch (Exception ex) { }
            }
        }

        private async Task LoadLastPosts()
        {
            var latest = await serviceBroker.GetLatestPosts(0);
            Items.Clear();
            if (latest.count > 0)
            {
                foreach (var item in latest.posts)
                {
                    string img = item.thumbnail;
                    //if (string.IsNullOrEmpty(img))
                    //{
                    //    img = Regex.Match(item.content, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase).Groups[1].Value;
                    //}
                    var p = new Models.Ui.HomeItem
                    {
                        Title = item.title,
                        UniqueId = item.id.ToString(),
                        Url = img,
                    };
                    this.Items.Add(p);
                }
                //if (latest.posts[0].attachments != null && latest.posts[0].attachments.Count > 0)
                BigImage = latest.posts[0].thumbnail;//[0].images.medium.url;

                this.isLoading = false;
            }
        }

        public void NaivgatoDetail(HomeItem obj)
        {
            if (obj is HomeItem)
            {
                this.navigationService.UriFor<PostPageViewModel>()
                    .WithParam(viewMode => viewMode.PostID, int.Parse(obj.UniqueId))
                    .WithParam(viewMode => viewMode.CategoryId, obj.CategoryId)
                    .Navigate();
            }
        }

        public async void Register()
        {
            var r = await this._serviceUser.Register("xiaohai", "49403700@qq.com", "yongqi", "蔚蓝海");
        }


        private int ProcessContract(int n1, int n2)
        {
            try
            {
                return System.Convert.ToInt32(Math.Round(System.Convert.ToDecimal(float.Parse(n1.ToString()) / float.Parse(n2.ToString())), 2) * new Decimal(100));
            }
            catch
            {
            }
            return 0;
        }

        public void ClearCache()
        {
            Common.MsgBoxShow("提示", "确定", "取消", "清除缓存").Dismissed += (EventHandler<DismissedEventArgs>)((s1, e1) =>
            {
                if (e1.Result == CustomMessageBoxResult.LeftButton)
                {
                    //UmengAnalytics.onEvent("114", "手动清空缓存");
                    MyStatic.WaitState("清除缓存", 0.6);
                    this.ClearCacheMethod();
                }
                else
                    SystemTray.Opacity = 0;
            });
        }

        private void ClearCacheMethod()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (file.DirectoryExists("ImageCache"))
            {
                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += (DoWorkEventHandler)((s1, e1) =>
                {
                    string[] local_1 = file.GetFileNames("ImageCache" + "\\*");
                    int local_2 = local_1.Length - 1;
                    for (int local_3 = 0; local_3 < local_1.Length; ++local_3)
                    {
                        bw.ReportProgress(ProcessContract(local_3, local_2));
                        file.DeleteFile("ImageCache" + "\\" + local_1[local_3]);
                    }
                });
                bw.RunWorkerCompleted += (RunWorkerCompletedEventHandler)((s1, e1) =>
                {
                    //Common.ShowMsg(AppResource.ClearSuccess, new double[0]);
                    MyStatic.Cancel();
                    GC.Collect();
                    file.Dispose();
                });
                bw.ProgressChanged += (ProgressChangedEventHandler)((s1, e1) =>
                {
                    MyStatic.WaitState("已清理" + (object)e1.ProgressPercentage + "%", 0.6);
                });
                bw.RunWorkerAsync();
            }
            else
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(3000.0);
                timer.Tick += (EventHandler)((s1, e1) =>
                {
                    timer.Stop();
                    MyStatic.Cancel();
                    //Common.ShowMsg(AppResource.ClearSuccess, new double[0]);
                });
                timer.Start();
                try
                {
                    //SettingHelper.RemoveKey("ApiStart");
                    //SettingHelper.RemoveKey("Expires");
                    //SettingHelper.RemoveKey("ApiInit");
                }
                catch
                {
                }
            }
        }

        public void Handle(SampleMessage message)
        {

        }
    }
}