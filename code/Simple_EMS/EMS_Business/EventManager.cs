using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple_EMS.DataModel;

namespace Simple_EMS.EMS_Business
{
    internal class EventManager:IDisposable
    {
        #region attributes
        private SimpleLog.Manager _LogManager = null;
        private EMS_Data.EMS_Context _Context = null;
        private ListenerManager _ListenerManager = null;
        #endregion

        #region constructors
        public EventManager(SimpleLog.Manager logManager, EMS_Data.EMS_Context context, ListenerManager listenerManager)
        {
            this._LogManager = logManager;
            this._Context = context;
            this._ListenerManager = listenerManager;
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
                int counter = 1;
                List<Info.SimpleException> simpleExceptions = new List<Info.SimpleException>();

                convertExceptionToJson(ex, counter, ref simpleExceptions);

                Info.SimpleExceptonCollection exceptionCollection = new Info.SimpleExceptonCollection();
                exceptionCollection.SimpleExceptions = simpleExceptions;
                exceptionCollection.Input = businessEventInstance;

                this._LogManager.Add(new SimpleLog.Message()
                {
                    CreatedOn = DateTime.Now,
                    Data = JsonConvert.SerializeObject(exceptionCollection),
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

        public bool ProcessEvents()
        {
            bool succeeded = false;

            IQueryable<DataModel.EventInstance> eventInstances = ReadNotProcessedEvents();

            foreach (var eventInstance in eventInstances)
            {
                updateEventInstanceStatus(eventInstance, DataModel.Constants.EVENT_STATUS_IN_PROGRESS);

                IQueryable<DataModel.ListenerSpecification> listenerSpecificaions = GetListenerSpecifications(eventInstance);

                createListenerInstances(eventInstance, listenerSpecificaions);

                DeleteEventInstace(eventInstance);
            }

            succeeded = true;

            return succeeded;
        }

        private void createListenerInstances(EventInstance eventInstance, IQueryable<ListenerSpecification> listenerSpecificaions)
        {
            foreach (var listenerSpecificaion in listenerSpecificaions)
            {
                this._ListenerManager.AddListenerInstance(eventInstance, listenerSpecificaion);
            }
        }

        /// <summary>
        /// get listener specifications that have not made instances of
        /// </summary>
        /// <param name="eventInstance"></param>
        /// <returns></returns>
        private IQueryable<ListenerSpecification> GetListenerSpecifications(EventInstance eventInstance)
        {
            var listenerSpecs = from listenerSpec in this._Context.ListenerSpecifications
                        join eventSpec_ListenerSpec in this._Context.EventSpecification_ListenerSpecifications on listenerSpec.ID equals eventSpec_ListenerSpec.ListenerSpecification_ID
                        join eventSpec in this._Context.EventSpecifications on eventSpec_ListenerSpec.EventSpecification_ID equals eventSpec.ID
                        join listenerInstance in this._Context.ListenerInstances on listenerSpec.ID equals listenerInstance.ListenerSpecification_ID into listenerInstanceEventInstance
                        from listenerInstance in listenerInstanceEventInstance.DefaultIfEmpty()
                                where listenerInstance.EventInstance_ID == eventInstance.ID
                                && listenerInstance == null
                                select listenerSpec;

            return listenerSpecs;
        }

        private void DeleteEventInstace(EventInstance eventInstance)
        {
            this._Context.EventInstances.Remove(eventInstance);

            this._Context.SaveChanges();
        }

        private void updateEventInstanceStatus(DataModel.EventInstance eventInstance, int status)
        {
            eventInstance.Status = status;
            eventInstance.ModifiedDate = DateTime.Now;

            this._Context.SaveChanges();
        }

        private void convertExceptionToJson(Exception ex, int counter, ref List<Info.SimpleException> simpleExceptions)
        {
            Info.SimpleException simple_ex = new Info.SimpleException();
            simple_ex.Serial = counter++;
            simple_ex.Message = ex.Message;
            simple_ex.Stack = ex.StackTrace;

            simpleExceptions.Add(simple_ex);

            if (ex.InnerException != null)
            {
                convertExceptionToJson(ex.InnerException, counter, ref simpleExceptions);
            }
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

        private IQueryable<DataModel.EventInstance> ReadNotProcessedEvents()
        {
            return this._Context.EventInstances.Where(e => e.Status == DataModel.Constants.EVENT_STATUS_NOT_PROCESSED || e.Status == DataModel.Constants.EVENT_STATUS_IN_PROGRESS);
        }

        public void Dispose()
        {
            

            
        }
    }
}
