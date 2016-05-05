using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_EMS.DataModel
{
    public class Constants
    {
        #region event instance status
        public const int EVENT_STATUS_NOT_PROCESSED = 0;
        public const int EVENT_STATUS_IN_PROGRESS = 1;
        public const int EVENT_STATUS_SUCCEEDED = 2;
        public const int EVENT_STATUS_SKIPPED = 3;
        public const int EVENT_STATUS_FAILED = 4;
        #endregion

        #region event instance status
        public const int LISTENER_STATUS_NOT_PROCESSED = 0;
        public const int LISTENER_STATUS_IN_PROGRESS = 1;
        public const int LISTENER_STATUS_SUCCEEDED = 2;
        public const int LISTENER_STATUS_SKIPPED = 3;
        public const int LISTENER_STATUS_FAILED = 4;
        #endregion
    }
}
