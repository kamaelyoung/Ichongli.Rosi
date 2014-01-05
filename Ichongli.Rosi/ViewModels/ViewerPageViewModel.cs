using Caliburn.Micro;
using Ichongli.Rosi.Interfaces;
using Ichongli.Rosi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.ViewModels
{
    public class ViewerPageViewModel : ThinkViewModelBase
    {
        private readonly ILockscreenHelper _lockscreenHelper;
        public ViewerPageViewModel(IProgressService progressService, IWindowManager windowManager, INavigationService navigationService, ILockscreenHelper lockscreenHelper)
            : base(progressService, windowManager, navigationService)
        {
            this._lockscreenHelper = lockscreenHelper;
            if (AppBase.Current.Photos != null)
            {
                foreach (var info in AppBase.Current.Photos)
                {
                    this.Photos.Add(info);
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

        private Models.Ui.ItemWithUrl _SelectItem;
        public Models.Ui.ItemWithUrl SelectItem
        {
            get
            {
                return this._SelectItem;
            }
            set
            {
                if (this._SelectItem != value)
                {
                    this._SelectItem = value;
                    this.NotifyOfPropertyChange(() => SelectItem);
                }
            }
        }

        private bool isLoading = false;

        private bool _progressBarIsVisible = true;
        public bool ProgressBarIsVisible
        {
            get
            {
                return _progressBarIsVisible;
            }
            set
            {
                if (this._progressBarIsVisible != value)
                {
                    this._progressBarIsVisible = value;
                    this.NotifyOfPropertyChange(() => ProgressBarIsVisible);
                }
            }
        }

        public string ItemID { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (!this._isInitialized)
            {
                if (this.Photos.Count > 0)
                {
                    var selected = this.Photos.FirstOrDefault(p => p.ItemId == this.ItemID);
                    if (selected != null)
                        this.SelectItem = selected;
                }
                this._isInitialized = true;
            }
        }

        public void OnSelectionChanged()
        {
            this.ProgressBarIsVisible = true;
        }

        public void OnImageOpened()
        {
            this.ProgressBarIsVisible = false;
        }

        public void OnImageFailed()
        {
            this.ProgressBarIsVisible = false;
        }

        public async void Lockscreen()
        {
            await this._lockscreenHelper.SetLockscreen(this.SelectItem.ItemImage.Medium);
        }
    }
}
