using Caliburn.Micro;
using Caliburn.Micro.BindableAppBar;
using Caliburn.Micro.Coding4Fun;
using Ichongli.Rosi.Interfaces;
using Ichongli.Rosi.Services;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
namespace Ichongli.Rosi
{
    public partial class AppBootstrapper : PhoneBootstrapper
    {
        private IKernel _kernel;

        protected override void Configure()
        {
            this.ConfigureContainer();
            InitializePhoneApplication();
        }

        private void ConfigureContainer()
        {
            this._kernel = new StandardKernel();

            this._kernel.Bind<IWindowManager>().To<Coding4FunWindowManager>().InSingletonScope();
            this._kernel.Bind<INavigationService>().ToConstant(new FrameAdapter(RootFrame)).InSingletonScope();
            this._kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            this._kernel.Bind<IUxService>().To<UiUx>().InSingletonScope();
            this._kernel.Bind<IPhoneService>().ToConstant(new PhoneApplicationServiceAdapter(PhoneApplicationService.Current, RootFrame)).InSingletonScope();
            this._kernel.Bind<IProgressService>().ToConstant(new ProgressService(RootFrame)).InSingletonScope();
            this._kernel.Bind<ILockscreenHelper>().To<LockscreenHelper>().InSingletonScope();
            this._kernel.Bind<IDownloadHelper>().To<DownloadHelper>().InSingletonScope();


            this._kernel.Bind<ILog>().ToMethod(context => LogManager.GetLog(context.Request.Target == null ? typeof(ILog) : context.Request.Target.Type));
            this._kernel.Bind<IServiceBroker>().To<ServiceBroker>();
            this._kernel.Bind<IServiceUser>().To<ServiceUser>();
            this._kernel.Bind<IServiceAuth>().To<ServiceAuth>();

            AddCustomConventions();
        }

        private void AddCustomConventions()
        {
            // App Bar Conventions
            ConventionManager.AddElementConvention<BindableAppBarButton>(
                Control.IsEnabledProperty, "DataContext", "Click");
            ConventionManager.AddElementConvention<BindableAppBarMenuItem>(
                Control.IsEnabledProperty, "DataContext", "Click");
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
