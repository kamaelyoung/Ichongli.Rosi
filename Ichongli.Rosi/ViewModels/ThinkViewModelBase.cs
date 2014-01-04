using Caliburn.Micro;
using Ichongli.Rosi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.ViewModels
{
    public class ThinkViewModelBase : Screen
    {
        protected bool _isInitialized;
        protected readonly IProgressService _progressService;
        protected readonly IWindowManager _windowManager;
        protected readonly INavigationService _navigationService;
        protected readonly IEventAggregator _eventAggregator;
        public ThinkViewModelBase(IProgressService progressService, IWindowManager windowManager, INavigationService navigationService)
        {
            this._progressService = progressService;
            this._windowManager = windowManager;
            // this._eventAggregator.Subscribe(this);
            this._navigationService = navigationService;
            // this.eventAggregator = eventAggregator;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }
    }
}
