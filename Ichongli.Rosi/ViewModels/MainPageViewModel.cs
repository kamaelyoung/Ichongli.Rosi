
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
            var r = await this._serviceUser.Register("xiaohai", "49403700@qq.com", "yongqi", "ÎµÀ¶º£");
        }

        public void Handle(SampleMessage message)
        {

        }
    }
}