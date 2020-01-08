using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RawPrintingHTTPService
{
    class PrintJobPostBody
    {
        public string printer;
        public string id;
        public string data;

        public byte[] DataToByteArray()
        {
            if (data != null)
            {
                return Convert.FromBase64String(data);
            }
            return null;
        }
    }
}
