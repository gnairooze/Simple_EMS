using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_EMS.EMS_Business
{
    public class Manager:IDisposable
    {
        #region attributes
        SimpleLog.Manager _LogManager = null;
        EMS_Data.EMS_Context _Context = new EMS_Data.EMS_Context();
        EventManager _EventManager = null;
        ListenerManager _ListenerManager = null;
        #endregion

        #region constructors
        public Manager()
        {
            initializeLog();

            this._ListenerManager = new ListenerManager(this._LogManager, this._Context);
            this._EventManager = new EventManager(this._LogManager, this._Context, this._ListenerManager);
        }
        #endregion

        private void initializeLog()
        {
            #region iniatilize console log
            dynamic consoleSettings = new System.Dynamic.ExpandoObject();

            consoleSettings.CanAddError = true;
            consoleSettings.CanAddWarning = true;
            consoleSettings.CanAddInfo = true;

            _LogManager = new SimpleLog.Manager(SimpleLog.Constants.LOG_CONSOLE, consoleSettings);
            #endregion

            #region to initialize db log, uncomment this region
            //dynamic dbSettings = new System.Dynamic.ExpandoObject();

            //dbSettings.CanAddError = true;
            //dbSettings.CanAddWarning = false;
            //dbSettings.CanAddInfo = false;
            
            //_LogManager.AddContext(SimpleLog.Constants.LOG_DB, dbSettings);
            #endregion
        }

        public bool TriggerEvent(BusinessModel.EventInstance businessEventInstance)
        {
            bool succeeded = false;

            #region log start
            this._LogManager.Add(new SimpleLog.Message()
            {
                CreatedOn = DateTime.Now,
                IdentifierName = "EventInstance_BusinessID",
                IdentifierValue = businessEventInstance.BusinessID.ToString(),
                Data = businessEventInstance.ToString(),
                Group = "TriggerEvent",
                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                Operation = "TriggerEvent started",
                Owner = this.GetType().ToString()
            });
            #endregion

            succeeded = this._EventManager.FireEvent(businessEventInstance);

            #region log end
            this._LogManager.Add(new SimpleLog.Message()
            {
                CreatedOn = DateTime.Now,
                IdentifierName = "EventInstance_BusinessID",
                IdentifierValue = businessEventInstance.BusinessID.ToString(),
                Data = businessEventInstance.ToString(),
                Group = "TriggerEvent",
                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                Operation = "TriggerEvent ended",
                Owner = this.GetType().ToString()
            });
            #endregion

            return succeeded;
        }

        public bool ProcessEvents()
        {
            bool succeeded = false;

            #region log start
            this._LogManager.Add(new SimpleLog.Message()
            {
                CreatedOn = DateTime.Now,
                IdentifierName = string.Empty,
                IdentifierValue = string.Empty,
                Data = "{}",
                Group = "ProcessEvents",
                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                Operation = "ProcessEvents started",
                Owner = this.GetType().ToString()
            });
            #endregion

            succeeded = this._EventManager.ProcessEvents();

            #region log start
            this._LogManager.Add(new SimpleLog.Message()
            {
                CreatedOn = DateTime.Now,
                IdentifierName = string.Empty,
                IdentifierValue = string.Empty,
                Data = "{}",
                Group = "ProcessEvents",
                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                Operation = "ProcessEvents ended",
                Owner = this.GetType().ToString()
            });
            #endregion

            return succeeded;
        }

        public bool ProcessListeners()
        {
            bool succeeded = false;


            return succeeded;
        }

        public void Dispose()
        {
            if (this._ListenerManager != null)
            {
                this._ListenerManager.Dispose();
            }
            
            if(this._EventManager != null)
            {
                this._EventManager.Dispose();
            }
            
            this._Context.Dispose();
        }
    }
}
