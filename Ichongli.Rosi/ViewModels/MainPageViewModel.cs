
namespace Ichongli.Rosi.ViewModels
{
    using Caliburn.Micro;

    public class MainPageViewModel : Screen, IHandle<SampleMessage>
    {
        private readonly INavigationService navigationService;
        private readonly IEventAggregator eventAggregator;
        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            this.navigationService = navigationService;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.Subscribe(this);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();
        }

        public void Handle(SampleMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}