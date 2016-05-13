using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple_EMS.DataModel;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Simple_EMS.EMS_Business
{
    internal class ListenerManager : IDisposable
    {
        #region attributes
        private SimpleLog.Manager _LogManager = null;
        private EMS_Data.EMS_Context _Context = null;
        #endregion

        #region constructors
        public ListenerManager(SimpleLog.Manager logManager, EMS_Data.EMS_Context context)
        {
            this._LogManager = logManager;
            this._Context = context;
        }
        #endregion

        public bool AddListenerInstance(DataModel.EventInstance eventInstance, DataModel.ListenerSpecification listenerSpec)
        {
            bool succeeded = false;
            DataModel.ListenerInstance listenerInstance = createListenerInstance(eventInstance, listenerSpec);

            this._Context.ListenerInstances.Add(listenerInstance);

            this._Context.SaveChanges();

            succeeded = true;

            return succeeded;
        }

        public bool UpdateListenerInstanceStatus(DataModel.ListenerInstance listenerInstance, int status)
        {
            bool succeeded = false;

            listenerInstance.Status = status;
            listenerInstance.ModifiedDate = DateTime.Now;

            if (status == DataModel.Constants.LISTENER_STATUS_SUCCEEDED)
            {
                    listenerInstance.DeleteDate = listenerInstance.ModifiedDate.AddDays(listenerInstance.Keep_Duration);
            }

            this._Context.SaveChanges();

            succeeded = true;

            return succeeded;
        }

        private ListenerInstance createListenerInstance(EventInstance eventInstance, ListenerSpecification listenerSpec)
        {
            DataModel.ListenerInstance listenerInstance = new ListenerInstance();

            listenerInstance.BusinessID = Guid.NewGuid();
            listenerInstance.CreatedDate = DateTime.Now;
            listenerInstance.EventData = eventInstance.EventData;
            listenerInstance.EventInstance_ID = eventInstance.ID;
            listenerInstance.Headers = listenerSpec.Headers;
            listenerInstance.Identifier1Name = eventInstance.Identifier1Name;
            listenerInstance.Identifier1Value = eventInstance.Identifier1Value;
            listenerInstance.Identifier2Name = eventInstance.Identifier2Name;
            listenerInstance.Identifier2Value = eventInstance.Identifier2Value;
            listenerInstance.Identifier3Name = eventInstance.Identifier3Name;
            listenerInstance.Identifier3Value = eventInstance.Identifier3Value;
            listenerInstance.Keep_Duration = listenerSpec.Keep_Duration;
            listenerInstance.ListenerSpecification_ID = listenerSpec.ID;
            listenerInstance.Method = listenerSpec.Method;
            listenerInstance.ModifiedDate = listenerInstance.CreatedDate;
            listenerInstance.RemainingRetrials = listenerSpec.Max_Retrials;
            listenerInstance.Status = DataModel.Constants.LISTENER_STATUS_NOT_PROCESSED;
            listenerInstance.URL = listenerSpec.URL;

            return listenerInstance;
        }

        public async Task InvokeListener(DataModel.ListenerInstance listenerInstance)
        {
            bool succeeded = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(listenerInstance.URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                foreach (var header in listenerInstance.Headers.Split(Environment.NewLine.ToCharArray()))
                {
                    string[] splittedHeader = header.Split(":".ToCharArray());
                    client.DefaultRequestHeaders.Add(splittedHeader[0], splittedHeader[1]);
                }

                HttpResponseMessage response = null;

                switch (listenerInstance.Method)
                {
                    case DataModel.Constants.LISTENER_METHOD_GET:
                        response = await client.GetAsync(listenerInstance.URL);
                        if (response.IsSuccessStatusCode)
                        {
                            succeeded = true;
                        }
                        else
                        {
                            //log response
                        }
                        
                        break;
                    case DataModel.Constants.LISTENER_METHOD_POST:
                        response = await client.PostAsync(listenerInstance.URL, new StringContent(listenerInstance.EventData));
                        if (response.IsSuccessStatusCode)
                        {
                            succeeded = true;
                        }
                        else
                        {
                            //log response
                        }
                        
                        break;
                    case DataModel.Constants.LISTENER_METHOD_PUT:
                        response = await client.PutAsync(listenerInstance.URL, new StringContent(listenerInstance.EventData));
                        if (response.IsSuccessStatusCode)
                        {
                            succeeded = true;
                        }
                        else
                        {
                            //log response
                        }

                        break;
                    case DataModel.Constants.LISTENER_METHOD_DELETE:
                        response = await client.DeleteAsync(listenerInstance.URL);
                        if (response.IsSuccessStatusCode)
                        {
                            succeeded = true;
                        }
                        else
                        {
                            //log response
                        }
                        
                        break;
                }
            }

            //update listener instance status
        }

        public void Dispose()
        {
            this._Context.Dispose();
        }
    }
}
