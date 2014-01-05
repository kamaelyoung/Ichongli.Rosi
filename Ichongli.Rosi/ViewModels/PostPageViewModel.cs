using Caliburn.Micro;
using Ichongli.Rosi.Interfaces;
using Ichongli.Rosi.Models;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ichongli.Rosi.ViewModels
{
    public class PostPageViewModel : ThinkViewModelBase
    {
        private readonly IUxService _uxService;
        private readonly IServiceBroker _serviceBroker;

        private int _PostID;
        public int PostID
        {
            get
            { return this._PostID; }
            set
            {
                this._PostID = value;
                this.NotifyOfPropertyChange(() => PostID);
            }
        }

        private string _CategoryId;
        public string CategoryId
        {
            get { return this._CategoryId; }
            set
            {
                this._CategoryId = value;
                this.NotifyOfPropertyChange(() => CategoryId);
            }

        }

        public PostPageViewModel(IUxService uxService, INavigationService navigationService, IServiceBroker serviceBroker, IEventAggregator eventAggregator, IProgressService progressService, IWindowManager windowManager)
            : base(progressService, windowManager, navigationService)
        {
            this._uxService = uxService;
            this._serviceBroker = serviceBroker;
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

        private string _Url;
        public string Url
        {
            get { return this._Url; }
            set
            {
                if (_Url != value)
                {
                    _Url = value;
                    this.NotifyOfPropertyChange(() => Url);
                }
            }
        }

        private string _ArticleUrl;
        public string ArticleUrl
        {
            get { return this._ArticleUrl; }
            set
            {
                if (_ArticleUrl != value)
                {
                    _ArticleUrl = value;
                    this.NotifyOfPropertyChange(() => ArticleUrl);
                }
            }
        }

        private string _Content;
        public string Content
        {
            get { return this._Content; }
            set
            {
                if (_Content != value)
                {
                    _Content = value;
                    this.NotifyOfPropertyChange(() => Content);
                }
            }
        }

        private string _Date;
        public string Date
        {
            get { return this._Date; }
            set
            {
                if (_Date != value)
                {
                    _Date = value;
                    this.NotifyOfPropertyChange(() => Date);
                }
            }
        }

        private string _Title;
        public string Title
        {
            get { return this._Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    this.NotifyOfPropertyChange(() => Title);
                }
            }
        }

        private string _ShortTitle;
        public string ShortTitle
        {
            get { return this._ShortTitle; }
            set
            {
                if (_ShortTitle != value)
                {
                    _ShortTitle = value;
                    this.NotifyOfPropertyChange(() => ShortTitle);
                }
            }
        }

        private ObservableCollection<Models.Ui.PostPropertie> _Tags = new ObservableCollection<Models.Ui.PostPropertie>();
        public ObservableCollection<Models.Ui.PostPropertie> Tags
        {
            get { return this._Tags; }
            set
            {
                if (_Tags != value)
                {
                    _Tags = value;
                    this.NotifyOfPropertyChange(() => Tags);
                }
            }
        }

        private ObservableCollection<Models.Ui.PostPropertie> _Categories = new ObservableCollection<Models.Ui.PostPropertie>();
        public ObservableCollection<Models.Ui.PostPropertie> Categories
        {
            get { return this._Categories; }
            set
            {
                if (_Categories != value)
                {
                    _Categories = value;
                    this.NotifyOfPropertyChange("Categories");
                }
            }
        }

        private ObservableCollection<Models.Ui.ItemWithUrl> _Photos = new ObservableCollection<Models.Ui.ItemWithUrl>();
        public ObservableCollection<Models.Ui.ItemWithUrl> Photos
        {
            get { return this._Photos; }
            set
            {
                if (_Photos != value)
                {
                    _Photos = value;
                    this.NotifyOfPropertyChange("Photos");
                }
            }
        }

        private Models.Ui.Author _Author = new Models.Ui.Author();
        public Models.Ui.Author Author
        {
            get { return this._Author; }
            set
            {
                if (_Author != value)
                {
                    _Author = value;
                    this.NotifyOfPropertyChange("Author");
                }
            }
        }

        protected override void OnInitialize()
        {
            this.LoadData();
            //base.OnInitialize();
        }

        private async void LoadData()
        {
            this._progressService.Show();
            var rootPost = await this._serviceBroker.GetPostById(this.PostID);
            var post = rootPost.Post;
            try
            {
                // var post = AppBase.Current.Posts["2"].posts.SingleOrDefault(o => o.id == this.PostID);

                if (post.author != null)
                {
                    this.Author = new Models.Ui.Author()
                    {
                        Id = post.author.id,
                        Name = post.author.name,
                        Nickname = post.author.nickname
                    };
                }
                this.Content = this._uxService.CleanHTML(post.content);
                this.Title = this._uxService.CleanHTML(post.title_plain);
                this.ShortTitle = _uxService.CleanHTML(post.title + "...").ToLower();
                string img = post.thumbnail;
                if (string.IsNullOrEmpty(img))
                {
                    img = Regex.Match(post.content, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase).Groups[1].Value;
                }
                this.Url = img;
                //this.ArticleUrl = post.custom_fields == null ? string.Empty : post.custom_fields.download_value[0];
                this.Photos.Clear();
                if (post.attachments != null && post.attachments.Count > 0)
                {
                    var item = post.attachments[0];
                    this.BigImage = item.images.large.url;
                    foreach (var photo in post.attachments)
                    {
                        if (photo.images == null)
                            continue;

                        this.Photos.Add(new Models.Ui.ItemWithUrl
                        {
                            ItemId = photo.id.ToString(),
                            Title = photo.title,
                            ItemImage = new Models.Ui.ItemImage()
                            {
                                Thumbnail = photo.images.thumbnail.url,
                                Medium = photo.images.medium.url
                            }
                        });
                    }
                }
                this.Tags.Clear();
                if (post.tags != null && post.tags.Count > 0)
                {
                    foreach (var tag in post.tags)
                        this.Tags.Add(new Models.Ui.PostPropertie()
                        {
                            Id = tag.id,
                            Title = tag.title,
                            Count = tag.post_count,
                        });
                }
            }
            catch (Exception ex)
            {
            }

            this._progressService.Hide();
        }

        public void Download()
        {
            try
            {
                _uxService.OpenIe(this.ArticleUrl);
            }
            catch (Exception ex)
            {
            }
        }

        public void NaivgatoViewer(Models.Ui.ItemWithUrl obj)
        {
            if (obj != null)
            {
                AppBase.Current.Photos = this.Photos;
                base._navigationService.UriFor<ViewerPageViewModel>()
                    .WithParam(vm => vm.ItemID, obj.ItemId)
                    .Navigate();
            }
        }

        public void Handle(SampleMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
