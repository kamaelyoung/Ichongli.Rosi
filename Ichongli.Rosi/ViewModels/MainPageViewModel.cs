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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TaskEx = System.Threading.Tasks.Task;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Tasks;

namespace Ichongli.Rosi.ViewModels
{
    public class MainPageViewModel : ThinkViewModelBase
    {
        private readonly IServiceBroker serviceBroker;
        private readonly IUxService _uxService;

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

        private ObservableCollection<Models.Ui.HomeItem> _AppItems;
        public ObservableCollection<Models.Ui.HomeItem> AppItems
        {
            get
            {
                if (this._AppItems == null)
                    this._AppItems = new ObservableCollection<HomeItem>();
                return this._AppItems;
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

        public MainPageViewModel(IUxService uxService, INavigationService navigationService, IEventAggregator eventAggregator, IServiceBroker serviceBroker, IProgressService progressService, IWindowManager windowManager)
            : base(progressService, windowManager, navigationService)
        {
            this.serviceBroker = serviceBroker;
            this._uxService = uxService;
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
                            this.Categories.Add(new Models.Ui.Item { Title = item.title.ToLower(), ItemId = item.id.ToString(), Description = item.description });
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
            this.LoadAppItems();

        }

        private async void LoadAppItems()
        {
            var apps = await serviceBroker.GetAppreCommended();
            this.AppItems.Clear();
            if (apps.count > 0)
            {
                foreach(var item in apps.posts)
                {
                    var img = item.thumbnail;
                    var p = new Models.Ui.HomeItem
                    {
                        Title = item.title,
                        UniqueId = item.id.ToString(),
                        Url = img,
                    };
                    this.AppItems.Add(p);
                }
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

        public void ClearCache()
        {
            this._uxService.ClearCache();
        }

        public void MarketplaceReviewTask()
        {
            MarketplaceReviewTask market = new MarketplaceReviewTask();
            market.Show();
        }

        public void ToMe()
        {
            this._uxService.OpenIe("https://me.alipay.com/moodjoy");
        }

        public void OnBackKeyPress(CancelEventArgs arg)
        {
            this.TryClose();
        }

        public void Handle(SampleMessage message)
        {

        }
    }
}