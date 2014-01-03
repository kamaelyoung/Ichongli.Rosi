using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.ViewModels
{
    public class CategoriesPageViewModel : Screen
    {
        private Models.Ui.Item _Item;
        public Models.Ui.Item Item
        {
            get
            {
                return this._Item;
            }
            set
            {
                this._Item = value;
                this.NotifyOfPropertyChange(() => Item);
            }
        }
    }
}
