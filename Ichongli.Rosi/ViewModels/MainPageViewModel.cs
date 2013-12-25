
namespace Ichongli.Rosi.ViewModels
{
    using Caliburn.Micro;
    using Ichongli.Rosi.Entitys;
    using Ichongli.Rosi.Services;
    using System.Collections.ObjectModel;
    using System.Windows;

    public class MainPageViewModel : Screen, IHandle<SampleMessage>
    {
        private readonly INavigationService navigationService;
        private readonly IEventAggregator eventAggregator;
        private readonly IPostService postService;       
        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IPostService postService)
        {
            this.navigationService = navigationService;
            this.eventAggregator = eventAggregator;
            this.postService = postService;
            this.eventAggregator.Subscribe(this);
        }

        protected override async void OnActivate()
        {            
            var posts = await postService.get_recent_posts(1);

            if (posts.status.ToLower() == "ok")
            {
                //this.Posts.Clear();
                if (posts.posts != null)
                {
                    foreach (var post in posts.posts)
                    {
                        this.Posts.Add(post);
                    }
                }
            }
            else
            {
                MessageBox.Show(posts.error);
            }
        }

        public void Handle(SampleMessage message)
        {
            throw new System.NotImplementedException();
        }

        private ObservableCollection<Post> _posts;
        public ObservableCollection<Post> Posts
        {
            get
            {
                if (this._posts == null)
                    this._posts = new ObservableCollection<Post>();
                return this._posts;
            }
        }
    }
}