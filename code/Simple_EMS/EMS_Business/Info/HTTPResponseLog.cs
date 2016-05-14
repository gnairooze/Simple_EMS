using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Simple_EMS.EMS_Business.Info
{
    internal class HTTPResponseLog
    {
        public dynamic EntityData { get; set; }
        public HttpResponseMessage Response { get; set; }
    }
}
