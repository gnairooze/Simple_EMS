using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_EMS.EMS_Business
{
    internal class EventTrigger:IDisposable
    {
        #region attributes
        private SimpleLog.Manager _LogManager = null;
        private EMS_Data.EMS_Context _Context = null;
        #endregion

        #region constructors
        public EventTrigger(SimpleLog.Manager logManager, EMS_Data.EMS_Context context)
        {
            this._LogManager = logManager;
            this._Context = context;
        }
        #endregion

        public bool FireEvent(BusinessModel.EventInstance businessEventInstance)
        {
            bool succeeded = false;

            #region log start
            this._LogManager.Add(new SimpleLog.Message()
            {
                CreatedOn = DateTime.Now,
                Data = JsonConvert.SerializeObject(businessEventInstance),
                Group = "EventTrigger",
                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                Operation = "FireEvent started",
                Owner = this.GetType().ToString()
            });
            #endregion

            try
            {
                DataModel.EventInstance dataEventInstance = createDataModel(businessEventInstance);

                this._Context.EventInstances.Add(dataEventInstance);
                this._Context.SaveChanges();

                succeeded = true;
            }
            catch (Exception ex)
            {
                #region log
                string input = JsonConvert.SerializeObject(businessEventInstance);
                JObject exJson = convertExceptionToJson(ex);
                exJson.Add("input", input);

                this._LogManager.Add(new SimpleLog.Message()
                {
                    CreatedOn = DateTime.Now,
                    Data = JsonConvert.SerializeObject(exJson),
                    Group = "EventTrigger",
                    MessageType = SimpleLog.Constants.MESSAGE_TYPE_ERROR,
                    Operation = "FireEvent",
                    Owner = this.GetType().ToString()
                });
                #endregion
            }

            #region log end
            this._LogManager.Add(new SimpleLog.Message()
            {
                CreatedOn = DateTime.Now,
                Data = JsonConvert.SerializeObject(businessEventInstance),
                Group = "EventTrigger",
                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                Operation = "FireEvent ended",
                Owner = this.GetType().ToString()
            });
            #endregion

            return succeeded;
        }

        private JObject convertExceptionToJson(Exception ex)
        {
            throw new NotImplementedException();
        }

        private DataModel.EventInstance createDataModel(BusinessModel.EventInstance businessEventInstance)
        {
            #region log start
            this._LogManager.Add(new SimpleLog.Message()
            {
                CreatedOn = DateTime.Now,
                Data = JsonConvert.SerializeObject(businessEventInstance),
                Group = "EventTrigger",
                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                Operation = "createDataModel started",
                Owner = this.GetType().ToString()
            });
            #endregion

            #region create and initialize the data model object
            DataModel.EventInstance dataEventInstance = new DataModel.EventInstance();

            dataEventInstance.BusinessID = businessEventInstance.BusinessID;
            dataEventInstance.Comment = businessEventInstance.Comment;
            dataEventInstance.CreatedDate = businessEventInstance.CreatedDate;
            dataEventInstance.EventData = businessEventInstance.EventData.ToString();

            dataEventInstance.EventSpecification_ID = GetEventSpecification(businessEventInstance.EventSpecification_Name).ID;

            dataEventInstance.Identifier1Name = businessEventInstance.Identifier1Name;
            dataEventInstance.Identifier1Value = businessEventInstance.Identifier1Value;
            dataEventInstance.Identifier2Name = businessEventInstance.Identifier2Name;
            dataEventInstance.Identifier2Value = businessEventInstance.Identifier2Value;
            dataEventInstance.Identifier3Name = businessEventInstance.Identifier3Name;
            dataEventInstance.Identifier3Value = businessEventInstance.Identifier3Value;

            dataEventInstance.CreatedDate = DateTime.Now;
            dataEventInstance.ModifiedDate = dataEventInstance.CreatedDate;
            dataEventInstance.Status = DataModel.Constants.EVENT_STATUS_NOT_PROCESSED;
            #endregion

            #region log end
            this._LogManager.Add(new SimpleLog.Message()
            {
                CreatedOn = DateTime.Now,
                Data = JsonConvert.SerializeObject(businessEventInstance),
                Group = "EventTrigger",
                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                Operation = "createDataModel ended",
                Owner = this.GetType().ToString()
            });
            #endregion

            return dataEventInstance;
        }

        private DataModel.EventSpecification GetEventSpecification(string name)
        {
            #region log start
            this._LogManager.Add(new SimpleLog.Message()
            {
                CreatedOn = DateTime.Now,
                Data = string.Format("name : {0}", name),
                Group = "EventTrigger",
                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                Operation = "GetEventSpecification started",
                Owner = this.GetType().ToString()
            });
            #endregion

            bool found = this._Context.EventSpecifications.Where(es => es.Name == name).Any();

            if(!found)
            {
                #region log
                this._LogManager.Add(new SimpleLog.Message()
                {
                    CreatedOn = DateTime.Now,
                    Data = string.Format("name : {0}, message: event specification name not found", name),
                    Group = "EventTrigger",
                    MessageType = SimpleLog.Constants.MESSAGE_TYPE_ERROR,
                    Operation = "GetEventSpecification",
                    Owner = this.GetType().ToString()
                });
                #endregion

                throw new InvalidOperationException("Invalid EventSpecifications name");
            }

            #region log end
            this._LogManager.Add(new SimpleLog.Message()
            {
                CreatedOn = DateTime.Now,
                Data = string.Format("name : {0}", name),
                Group = "EventTrigger",
                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                Operation = "GetEventSpecification ended",
                Owner = this.GetType().ToString()
            });
            #endregion

            return this._Context.EventSpecifications.Where(es => es.Name == name).First();
        }

        public void Dispose()
        {
            #region log
            this._LogManager.Add(new SimpleLog.Message()
            {
                CreatedOn = DateTime.Now,
                Data = "{}",
                Group = "EventTrigger",
                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                Operation = "Disposed",
                Owner = this.GetType().ToString()
            });
            #endregion

            this._Context.Dispose();
        }
    }
}
