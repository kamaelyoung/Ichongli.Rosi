using Caliburn.Micro;
using Ichongli.Rosi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.ViewModels
{
    public class LoginPageViewModel : Caliburn.Micro.Screen
    {
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;
        public LoginPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            this._navigationService = navigationService;
            this._eventAggregator = eventAggregator;
        }
    }
}
