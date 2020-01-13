using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace HTTPPrintService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            ServiceController sc = new ServiceController(serviceInstaller1.ServiceName);
            sc.Start();
        }
    }
}
