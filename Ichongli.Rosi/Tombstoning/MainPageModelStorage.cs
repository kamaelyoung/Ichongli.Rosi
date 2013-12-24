namespace Ichongli.Rosi.Tombstoning
{
    using Caliburn.Micro;
    using Ichongli.Rosi.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MainPageModelStorage : StorageHandler<MainPageViewModel>
    {
        public override void Configure()
        {
            //Property(x => x)
            //    .InPhoneState();
        }
    }
}
