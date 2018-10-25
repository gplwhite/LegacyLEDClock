using Caliburn.Micro;
using Clock4Windows.ViewModels;
using System.Windows;

namespace Clock4Windows
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }


        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            DisplayRootViewFor<ShellViewModel>();
        }


    }
}
