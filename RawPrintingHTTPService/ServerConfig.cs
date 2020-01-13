﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace RawPrintingHTTPService
{
    class ServerConfig
    {
        private static List<string> _printers;
        public static FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
        public static string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + versionInfo.CompanyName + "\\" + versionInfo.ProductName;
        public static string configPath = basePath + "\\config.json";
        public static bool isInteractive = Environment.UserInteractive && System.Diagnostics.Debugger.IsAttached;
        public static void appendLog(string data, EventLog eventLog = null)
        {
            if (eventLog != null)
            {
                eventLog.WriteEntry(data, EventLogEntryType.Information);
            } else
            {
                string filePath = basePath + "\\print.log";
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(data);
                }
            }
        }

        public static string toJSON(object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        public static T fromJSON<T>(string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }

        public static List<string> listPrinters()
        {
            if (_printers == null || _printers.Capacity == 0)
            {
                _printers = new List<string>();
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    _printers.Add(printer);
                }
            }
            return _printers;
        }

        public static ServerConfig load()
        {
            string configJson;
            try
            {
                configJson = File.ReadAllText(configPath);
                ServerConfig config = fromJSON<ServerConfig>(configJson);
                config.validateValues();
                return config;
            }
            catch
            {
                ServerConfig res = new ServerConfig();
                res.validateValues();
                return res;
            }
        }

        public List<string> allowedDomains = new List<string>();
        public int port;
        public int testingMode = 0;

        public ServerConfig()
        {
            if (isInteractive)
            {
                Directory.CreateDirectory(basePath);
            }
        }

        private void validateValues()
        {
            if (port <= 0)
            {
                port = 9100;
            }
            if (allowedDomains == null)
            {
                allowedDomains = new List<string>();
            }
        }
        public void save()
        {
            try
            {
                string configJson = toJSON(this);
                File.WriteAllText(configPath, configJson);
            }
            catch
            {
                // ignore
            }
        }

        public override string ToString()
        {
            return toJSON(this);
        }
    }
}
