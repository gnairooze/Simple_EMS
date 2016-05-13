using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_EMS.EMS_Business.Info
{
    internal class SimpleException
    {
        public int Serial { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }

    }
}
