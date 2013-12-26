using Caliburn.Micro;
using Ichongli.Rosi.Interfaces;
using Ichongli.Rosi.Services;
using Microsoft.Phone.Shell;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi
{
    public partial class AppBootstrapper : PhoneBootstrapper
    {
        private IKernel _kernel;
        
        protected override void Configure()
        {
            this.ConfigureContainer();
        }

        private void ConfigureContainer()
        {
            this._kernel = new StandardKernel();

            this._kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            this._kernel.Bind<INavigationService>().ToConstant(new FrameAdapter(RootFrame)).InSingletonScope();
            this._kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            this._kernel.Bind<IPhoneService>().ToConstant(new PhoneApplicationServiceAdapter(PhoneApplicationService.Current, RootFrame)).InSingletonScope();
            this._kernel.Bind<ILog>().ToMethod(context => LogManager.GetLog(context.Request.Target == null ? typeof(ILog) : context.Request.Target.Type));
            this._kernel.Bind<IServiceBroker>().To<ServiceBroker>();

        }

        protected override object GetInstance(Type service, string key)
        {
            return this._kernel.Get(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this._kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            this._kernel.Inject(instance);
        }
    }
}
