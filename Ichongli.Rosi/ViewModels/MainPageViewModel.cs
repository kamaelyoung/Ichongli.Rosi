
namespace Ichongli.Rosi.ViewModels
{
    using Caliburn.Micro;
    using Ichongli.Rosi.Services;

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
            await postService.get_recent_posts("");
        }

        public void Handle(SampleMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}