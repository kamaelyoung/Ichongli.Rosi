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
using TaskEx = System.Threading.Tasks.Task;

namespace Ichongli.Rosi.ViewModels
{
    public class MainPageViewModel : Screen, IHandle<SampleMessage>
    {
        private readonly INavigationService navigationService;
        private readonly IEventAggregator eventAggregator;
        private readonly IServiceBroker serviceBroker;
        private readonly IServiceUser _serviceUser;
        private readonly IProgressService _progressService;
        private readonly IWindowManager _windowManager;

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

        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IServiceBroker serviceBroker, IServiceUser serviceUser, IProgressService progressService, IWindowManager windowManager)
        {
            this.navigationService = navigationService;
            this.eventAggregator = eventAggregator;
            this.serviceBroker = serviceBroker;
            this._serviceUser = serviceUser;
            this._progressService = progressService;
            this._windowManager = windowManager;
            this.eventAggregator.Subscribe(this);
        }

        protected override async void OnInitialize()
        {
            this._progressService.Show();
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
                catch (Exception ex)
                {
                    this._progressService.Hide();
                }
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
                    var p = new Models.Ui.HomeItem
                    {
                        Title = item.title,
                        UniqueId = item.id.ToString(),
                        Url = img,
                    };
                    this.Items.Add(p);
                }
                if (latest.posts[0].attachments != null && latest.posts[0].attachments.Count > 0)
                    BigImage = latest.posts[0].thumbnail;//[0].images.medium.url;
            }
            this._progressService.Hide();
        }

        public void NaivgatoDetail(HomeItem obj)
        {
            if (obj is HomeItem)
            {
                this.navigationService.UriFor<PostPageViewModel>()
                    .WithParam(viewMode => viewMode.PostID, int.Parse(obj.UniqueId))
                    .Navigate();
            }
        }

        public void NaivgatoCategorie(Item obj)
        {
            if (obj != null)
            {
                this.navigationService.UriFor<CategoriesPageViewModel>()
                    .WithParam<Item>(viewModel => viewModel.Item, obj)
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
            var dialogViewModel = new DialogViewModel
            {
                Title = "Dialog",
                Text = "It's a modal dialog. It blocks user interface.\r\n\r\nTap 'ok' to increase the counter."
            };
            dialogViewModel.Deactivated += (sender, args) =>
            {
                if (dialogViewModel.Result == DialogResult.Ok)
                {
                    this._progressService.Show("清除缓存");
                    this.ClearCacheMethod();
                }
                else
                    SystemTray.Opacity = 0;
            };
            _windowManager.ShowDialog(dialogViewModel);
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
                    this._progressService.Hide();
                    GC.Collect();
                    file.Dispose();
                    this.ShowDialogFor2Seconds();
                });
                bw.ProgressChanged += (ProgressChangedEventHandler)((s1, e1) =>
                {
                    this._progressService.Show("已清理" + (object)e1.ProgressPercentage + "%");
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
                    this._progressService.Hide();
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
        public async void ShowDialogFor2Seconds()
        {
            var dialogViewModel = new DialogViewModel
            {
                Title = "TryClose() closes the dialog",
                Text = "This dialog will be displayed only for 2 seconds."
            };
            _windowManager.ShowDialog(dialogViewModel);
            await TaskEx.Delay(TimeSpan.FromSeconds(2));
            dialogViewModel.TryClose();
        }

        public void Handle(SampleMessage message)
        {

        }
    }
}