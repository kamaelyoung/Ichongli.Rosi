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
    public class CategoriesPageViewModel : ThinkViewModelBase
    {
        private readonly IServiceBroker _serviceBroker;
        public CategoriesPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IServiceBroker _serviceBroker, IServiceBroker serviceBroker, IServiceUser serviceUser, IProgressService progressService, IWindowManager windowManager)
            : base(progressService, windowManager, navigationService)
        {
            this._serviceBroker = serviceBroker;
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
                if (this._IsLoading != value)
                {
                    this._IsLoading = value;
                    this.NotifyOfPropertyChange("IsLoading");
                }
            }
        }

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

        protected override void OnActivate()
        {
            base.OnActivate();
            if (!base._isInitialized)
            {
                this._isInitialized = true;
                this.Feedbacks();
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
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
                base._progressService.Show();
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
                else
                {

                }

                this.IsLoading = false;
                base._progressService.Hide();
            }
        }

        public void NaivgatoDetail(Models.Ui.HomeItem obj)
        {
            if (obj is Models.Ui.HomeItem)
            {
                base._navigationService.UriFor<PostPageViewModel>()
                    .WithParam(viewMode => viewMode.PostID, int.Parse(obj.UniqueId))
                    .Navigate();
            }
        }

    }
}
