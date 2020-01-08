using System;
using System.IO;
using System.Net;
using System.Text;

namespace RawPrintingHTTPService.handlers
{
    class PermissionsHandler
    {
        private RawPrintingHTTPServer server;

        public PermissionsHandler(RawPrintingHTTPServer server)
        {
            this.server = server;
        }

        public bool handle(HttpListenerRequest req, HttpListenerResponse resp, string accesslog)
        {
            if (req.HttpMethod == "POST")
            {
                return _handlePost(req, resp, accesslog);
            }
            else if (req.HttpMethod == "GET")
            {
                return _handleGet(req, resp, accesslog);
            }
            else
            {
                return true;
            }
        }

        private bool _handlePost(HttpListenerRequest req, HttpListenerResponse resp, string accesslog)
        {
            if (req.HasEntityBody)
            {
                try
                {
                    using (Stream body = req.InputStream)
                    {
                        Encoding encoding = req.ContentEncoding;
                        using (StreamReader reader = new StreamReader(body, encoding))
                        {
                            string host = null;
                            string status = null;
                            string[] keyvalues = reader.ReadLine().Split('&');
                            foreach (string query in keyvalues)
                            {
                                string[] keyvalue = query.Split('=');
                                if (keyvalue.Length > 0)
                                {
                                    if (keyvalue[0].ToLower() == "host")
                                    {
                                        host = Uri.UnescapeDataString(keyvalue[1]);
                                    }
                                    else if (keyvalue[0].ToLower() == "status")
                                    {
                                        status = Uri.UnescapeDataString(keyvalue[1]);
                                    }
                                }
                            }
                            body.Close();
                            reader.Close();

                            string html = "<html>";
                            if (host != null)
                            {
                                accesslog += "\tsuccess";
                                if (status == "allow")
                                {
                                    if (!server.config.allowedDomains.Contains(host))
                                    {
                                        server.config.allowedDomains.Add(host);
                                        server.config.save();
                                    }
                                }
                                else if (status == "remove")
                                {
                                    if (server.config.allowedDomains.Contains(host))
                                    {
                                        server.config.allowedDomains.Remove(host);
                                        server.config.save();
                                    }
                                }
                                html += "<script>window.close()</script>";
                            }
                            else
                            {
                                accesslog += "\tfailed";
                                html += "FAILED";
                            }
                            html += "</html>";

                            ServerConfig.appendLog(accesslog);
                            byte[] data = Encoding.UTF8.GetBytes(html);
                            resp.ContentType = "text/html";
                            resp.ContentEncoding = Encoding.UTF8;
                            resp.ContentLength64 = data.LongLength;
                            resp.OutputStream.Write(data, 0, data.Length);
                            resp.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    ServerConfig.appendLog("Error: " + e.Message + "\n" + e.StackTrace);
                    accesslog += "\tfailed";
                    ServerConfig.appendLog(accesslog);
                }
            }
            else
            {
                ServerConfig.appendLog("Error: Body Required");
                return true;
            }
            return false;
        }

        private bool _handleGet(HttpListenerRequest req, HttpListenerResponse resp, string accesslog)
        {
            if (req.QueryString.Count > 0)
            {
                string html = "<html><form method=post><h2>Do you want to allow {0} to print?</h2><input type=hidden name=\"host\" value=\"{0}\" /><input type=hidden name=\"status\" value=\"allow\" /><button>Allow</button><button type=button onclick=\"window.close()\">Block</button></form></html>";
                string host = req.QueryString["h"];

                if (host != null)
                {
                    html = string.Format(html, host);

                    ServerConfig.appendLog(accesslog);
                    byte[] data = Encoding.UTF8.GetBytes(html);
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    resp.OutputStream.Write(data, 0, data.Length);
                    resp.Close();
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }
    }
}
