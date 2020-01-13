using RawPrintingHTTPService.responses;
using System.Net;
using System.Text;

namespace RawPrintingHTTPService.handlers
{
    class PrintersHandler
    {
        public bool handle(HttpListenerRequest req, HttpListenerResponse resp, string accesslog)
        {
            if (req.HttpMethod == "GET")
            {
                return _handleGet(req, resp, accesslog);
            }
            else
            {
                return true;
            }
        }
        
        private bool _handleGet(HttpListenerRequest req, HttpListenerResponse resp, string accesslog)
        {
            MachineInfoResponse packet = new MachineInfoResponse
            {
                machineName = System.Environment.MachineName
            };
            packet.printers = ServerConfig.listPrinters();

            ServerConfig.appendLog(accesslog);
            byte[] data = Encoding.UTF8.GetBytes(ServerConfig.toJSON(packet));
            resp.ContentType = "application/json";
            resp.ContentEncoding = Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            resp.OutputStream.Write(data, 0, data.Length);
            resp.Close();
            return false;
        }
    }
}
