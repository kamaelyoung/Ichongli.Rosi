
namespace Ichongli.Rosi.ViewModels
{
    using Caliburn.Micro;
    using Ichongli.Rosi.Entitys;
    using Ichongli.Rosi.Services;
    using System.Collections.ObjectModel;

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
            await postService.get_recent_posts(1);
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