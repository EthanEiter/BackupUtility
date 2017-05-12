using System.Windows;
using Caliburn.Micro;

namespace Backup.Utility
{
    public class UtilityBootstrapper : BootstrapperBase
    {
        public UtilityBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<BackupViewModel>();
        }
    }
}
