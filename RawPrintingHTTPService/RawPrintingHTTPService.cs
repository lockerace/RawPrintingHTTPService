using System.ServiceProcess;

namespace RawPrintingHTTPService
{
    public partial class RawPrintingHTTPService : ServiceBase
    {
        private RawPrintingHTTPServer server;

        public RawPrintingHTTPService()
        {
            InitializeComponent();
            server = new RawPrintingHTTPServer();
        }

        protected override void OnStart(string[] args)
        {
            server.Start();
        }

        protected override void OnStop()
        {
            server.Stop();
        }
    }
}
