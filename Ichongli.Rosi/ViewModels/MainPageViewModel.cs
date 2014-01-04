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
using Coding4Fun.Toolkit.Controls;

namespace Ichongli.Rosi.ViewModels
{
    public class MainPageViewModel : ThinkViewModelBase
    {
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

        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IServiceBroker serviceBroker, IServiceUser serviceUser, IProgressService progressService, IWindowManager windowManager)
            : base(progressService, windowManager, navigationService)
        {
            this.serviceBroker = serviceBroker;
            this._serviceUser = serviceUser;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (!base._isInitialized)
            {
                this.OnLoadData();
                this._isInitialized = true;
            }
        }

        private async void OnLoadData()
        {
            base._progressService.Show();
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
                        throw new Exception(categories.status);
                    }
                }
                catch (Exception ex)
                {
                    this._progressService.Hide();
                    var dialogViewModel = new DialogViewModel
                    {
                        Title = "获取分类错误",
                        Text = ex.Message
                    };
                    this._windowManager.ShowPopup(dialogViewModel);
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
                base._navigationService.UriFor<PostPageViewModel>()
                    .WithParam(viewMode => viewMode.PostID, int.Parse(obj.UniqueId))
                    .Navigate();
            }
        }

        public void NaivgatoCategorie(Item obj)
        {
            if (obj != null)
            {
                base._navigationService.UriFor<CategoriesPageViewModel>()
                    .WithParam<string>(viewModel => viewModel.ItemID, obj.ItemId)
                    .WithParam<string>(viewModel => viewModel.DisplayName, obj.Title)
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
                Title = "提示",
                Text = "您确定要清除所有缓存吗？"
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
                    this.ShowDialogFor2Seconds("清除完毕");
                });
                bw.ProgressChanged += (ProgressChangedEventHandler)((s1, e1) =>
                {
                    this._progressService.Show("已清理" + (object)e1.ProgressPercentage + "%");
                });
                bw.RunWorkerAsync();
            }
        }

        public async void ShowDialogFor2Seconds(string text)
        {
            var dialogviewModel = new DialogViewModel
            {
                Title = "Ignore back",
                Text = "This dialog cannot be closed by pressing back key.",
            };
            _windowManager.ShowDialog(dialogviewModel, null, new Dictionary<string, object>
            {
                { "IgnoreBackKey", true }
            });

            await TaskEx.Delay(TimeSpan.FromSeconds(2));
            dialogviewModel.TryClose();
        }

        public void OnBackKeyPress(CancelEventArgs arg)
        {
            if (this._isCanClose)
                this.TryClose();
            else
            {
                arg.Cancel = true;
                this.ShowCloseToastPrompt();
            }
        }

        private bool _isCanClose;

        public void ShowCloseToastPrompt()
        {
            var toast = new ToastPrompt();
            toast.FontSize = 30;
            toast.Message = "亲  再来一下就出去了哦～";
            toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;
            toast.Completed += toast_Completed;
            this._isCanClose = true;
            toast.Show();
        }

        void toast_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            var toast = sender as ToastPrompt;
            if (toast != null)
            {
                toast.Completed -= toast_Completed;
                this._isCanClose = false;
            }
        }

        public void Handle(SampleMessage message)
        {

        }
    }
}