using Caliburn.Micro;
using Ichongli.Rosi.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Ichongli.Rosi.ViewModels
{
    public class CategoriesPageViewModel : Screen
    {
        private readonly IServiceBroker _serviceBroker;
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProgressService _progressService;
        private readonly IWindowManager _windowManager;
        private bool _isInitialized;
        public CategoriesPageViewModel(INavigationService navigationService,
            IEventAggregator eventAggregator,
            IServiceBroker serviceBroker,
            IProgressService progressService,
            IWindowManager windowManager)
        {
            this._serviceBroker = serviceBroker;
            this._navigationService = navigationService;
            this._eventAggregator = eventAggregator;
            this._progressService = progressService;
            this._windowManager = windowManager;

            this.Items = new ObservableCollection<Models.Ui.HomeItem>();
        }

        private string _DisplayTitle;
        public string DisplayTitle
        {
            get
            {
                return this._DisplayTitle;
            }
            set
            {
                if (this._DisplayTitle != value)
                {
                    this._DisplayTitle = value;
                    this.NotifyOfPropertyChange(() => DisplayTitle);
                }
            }
        }

        private string _ItemID;
        public string ItemID
        {
            get
            {
                return this._ItemID;
            }
            set
            {
                if (this._ItemID != value)
                {
                    this._ItemID = value;
                    this.NotifyOfPropertyChange(() => ItemID);
                }
            }
        }

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get
            {
                return this._IsLoading;
            }
            set
            {
                if (value.Equals(this._IsLoading))
                    return;
                this._IsLoading = value;
                this.NotifyOfPropertyChange(() => IsLoading);
            }
        }

        private string _BigImage;
        public string BigImage
        {
            get { return this._BigImage; }
            set
            {
                if (_BigImage != value)
                {
                    _BigImage = value;
                    this.NotifyOfPropertyChange(() => BigImage);
                }
            }
        }

        private ObservableCollection<Models.Ui.Item> _Categories;
        public ObservableCollection<Models.Ui.Item> Categories
        {
            get
            {
                if (this._Categories == null)
                    this._Categories = new ObservableCollection<Models.Ui.Item>();
                return this._Categories;
            }
        }

        private ObservableCollection<Models.Ui.HomeItem> _Items;
        public ObservableCollection<Models.Ui.HomeItem> Items
        {
            get
            {
                return this._Items;
            }
            set
            {
                if (this._Items == value)
                    return;
                this._Items = value;
                this.NotifyOfPropertyChange(() => Items);
            }
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);

            if (!this._isInitialized)
            {
                this._isInitialized = true;
                this.Feedbacks();
            }
        }

        private int _pageIndex = 1;
        private int _totalPages = 0;

        public void Feedbacks()
        {
            this.OnLoadData();
        }

        private async void OnLoadData()
        {
            if (this.Items.Count <= 0 || this._pageIndex <= this._totalPages)
            {
                this.IsLoading = true;
                this._progressService.Show();
                var latest = await this._serviceBroker.GetPostsFrom(this.ItemID, this._pageIndex);

                if (latest != null)
                {
                    this._pageIndex += 1;
                    this._totalPages = latest.pages;

                    foreach (var item in latest.posts)
                    {
                        this.Items.Add(new Models.Ui.HomeItem
                        {
                            UniqueId = item.id.ToString(),
                            Title = item.title,
                            CategoryId = this.ItemID,
                            Url = item.thumbnail
                        });
                    }
                }

                this.IsLoading = false;
                this._progressService.Hide();
            }
        }

        public void NaivgatoDetail(Models.Ui.HomeItem obj)
        {
            if (obj is Models.Ui.HomeItem)
            {
                this._navigationService.UriFor<PostPageViewModel>()
                    .WithParam(viewMode => viewMode.PostID, int.Parse(obj.UniqueId))
                    .Navigate();
            }
        }

    }
}
