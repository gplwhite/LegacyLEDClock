using Caliburn.Metro.Core;
using Caliburn.Micro;
using Clock4Windows.ViewModels;
using ClockLib;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Clock4Windows
{
    public class Bootstrapper : BootstrapperBase
    {

        private readonly SimpleContainer _container = new SimpleContainer();



        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// </summary>
        public Bootstrapper()
        {
            Initialize();
        }


        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            _container.Instance(_container);

            _container.Singleton<IWindowManager, MetroWindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();

            _container.Singleton<ShellViewModel>();
            _container.Singleton<ViewModelFactory>();
            _container.Singleton<PortManager>();

            _container.PerRequest<ClockViewModel>();
            _container.PerRequest<DeviceViewModel>();


            _container.GetInstance<PortManager>().Start();

            DisplayRootViewFor<ShellViewModel>();
        }



        protected override object GetInstance(Type serviceType, string key)
        {
            return _container.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

    }
}
