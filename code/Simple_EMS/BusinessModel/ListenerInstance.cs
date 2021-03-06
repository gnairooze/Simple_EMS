﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_EMS.BusinessModel
{
    public class ListenerInstance
    {
        #region properties
        public long ID { get; set; }

        public Guid BusinessID { get; set; }

        public string Identifier1Name { get; set; }

        public string Identifier1Value { get; set; }

        public string Identifier2Name { get; set; }

        public string Identifier2Value { get; set; }

        public string Identifier3Name { get; set; }

        public string Identifier3Value { get; set; }

        public Uri URL { get; set; }

        public string Method { get; set; }

        public string Headers { get; set; }

        public Newtonsoft.Json.Linq.JObject EventData { get; set; }

        public int RemainingRetrials { get; set; }

        public int Status { get; set; }

        public string Comment { get; set; }

        public DateTime? DeleteDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
        #endregion

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
