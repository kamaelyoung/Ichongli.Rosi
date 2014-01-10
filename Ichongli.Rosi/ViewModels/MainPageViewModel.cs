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
        private readonly IUxService _uxService;
        private readonly IServiceAuth _serviceAuth;

        private ObservableCollection<Models.Ui.Item> _Categories;
        public ObservableCollection<Models.Ui.Item> Categories
        {
            get
            {
                if (this._Categories == null)
                    this._Categories = new ObservableCollection<Item>();
                return this._Categories;
            }
        }

        private ObservableCollection<Models.Ui.HomeItem> _Items;
        public ObservableCollection<Models.Ui.HomeItem> Items
        {
            get
            {
                if (this._Items == null)
                    this._Items = new ObservableCollection<HomeItem>();
                return this._Items;
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

        public MainPageViewModel(IServiceAuth serviceAuth, IUxService uxService, INavigationService navigationService, IEventAggregator eventAggregator, IServiceBroker serviceBroker, IServiceUser serviceUser, IProgressService progressService, IWindowManager windowManager)
            : base(progressService, windowManager, navigationService)
        {
            this.serviceBroker = serviceBroker;
            this._serviceUser = serviceUser;
            this._uxService = uxService;
            this._serviceAuth = serviceAuth;
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

                    if (categories != null && categories.status.ToLower() == "ok")
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
                        throw new Exception(categories == null ? "没有分类信息" : categories.status);
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
                    .WithParam<string>(viewModel => viewModel.DisplayTitle, obj.Title)
                    .Navigate();
            }
        }

        public async void Register()
        {
            var r = await this._serviceUser.Register("xiaohai", "49403700@qq.com", "yongqi", "蔚蓝海");
        }

        public void ClearCache()
        {
            this._uxService.ClearCache();
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
            toast.FontSize = 20;
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

        public async void Login()
        {
            var userRoot = await this._serviceAuth.generate_auth_cookie("tuigirls", "yongqi29");
            if (userRoot.status == "ok")
            {
                this._uxService.ShowToast("欢迎回来" + userRoot.user.displayname);
            }
        }
    }
}