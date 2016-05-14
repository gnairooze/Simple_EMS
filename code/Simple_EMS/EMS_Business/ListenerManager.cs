using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple_EMS.DataModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

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

        public bool UpdateListenerInstanceStatus(DataModel.ListenerInstance listenerInstance, int status, bool decreaseTrialCount)
        {
            bool succeeded = false;

            listenerInstance.Status = status;
            listenerInstance.ModifiedDate = DateTime.Now;

            if (status == DataModel.Constants.LISTENER_STATUS_SUCCEEDED)
            {
                    listenerInstance.DeleteDate = listenerInstance.ModifiedDate.AddDays(listenerInstance.Keep_Duration);
            }

            if(decreaseTrialCount)
            {
                listenerInstance.RemainingRetrials--;
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

        public async Task<bool> InvokeListener(DataModel.ListenerInstance listenerInstance)
        {
            bool succeeded = false;

            using (var client = new HttpClient())
            {
                #region create http client object
                client.BaseAddress = new Uri(listenerInstance.URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                foreach (var header in listenerInstance.Headers.Split(Environment.NewLine.ToCharArray()))
                {
                    string[] splittedHeader = header.Split(":".ToCharArray());
                    client.DefaultRequestHeaders.Add(splittedHeader[0], splittedHeader[1]);
                }
                #endregion

                Info.HTTPResponseLog responseLog = new Info.HTTPResponseLog();
                responseLog.EntityData = listenerInstance;

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
                            #region log response error
                            responseLog.Response = response;
                            this._LogManager.Add(new SimpleLog.Message()
                            {
                                CreatedOn = DateTime.Now,
                                Data = JsonConvert.SerializeObject(responseLog),
                                Group = "InvokeListener",
                                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                                Operation = DataModel.Constants.LISTENER_METHOD_GET,
                                Owner = this.GetType().ToString()
                            });
                            #endregion
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
                            #region log response error
                            responseLog.Response = response;
                            this._LogManager.Add(new SimpleLog.Message()
                            {
                                CreatedOn = DateTime.Now,
                                Data = JsonConvert.SerializeObject(responseLog),
                                Group = "InvokeListener",
                                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                                Operation = DataModel.Constants.LISTENER_METHOD_POST,
                                Owner = this.GetType().ToString()
                            });
                            #endregion
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
                            #region log response error
                            responseLog.Response = response;
                            this._LogManager.Add(new SimpleLog.Message()
                            {
                                CreatedOn = DateTime.Now,
                                Data = JsonConvert.SerializeObject(responseLog),
                                Group = "InvokeListener",
                                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                                Operation = DataModel.Constants.LISTENER_METHOD_PUT,
                                Owner = this.GetType().ToString()
                            });
                            #endregion
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
                            #region log response error
                            responseLog.Response = response;
                            this._LogManager.Add(new SimpleLog.Message()
                            {
                                CreatedOn = DateTime.Now,
                                Data = JsonConvert.SerializeObject(responseLog),
                                Group = "InvokeListener",
                                MessageType = SimpleLog.Constants.MESSAGE_TYPE_INFO,
                                Operation = DataModel.Constants.LISTENER_METHOD_DELETE,
                                Owner = this.GetType().ToString()
                            });
                            #endregion
                        }
                        break;
                }
            }

            #region update listener instance status
            int status = DataModel.Constants.LISTENER_STATUS_IN_PROGRESS;

            if (succeeded)
            {
                status = DataModel.Constants.LISTENER_STATUS_SUCCEEDED;
            }
            
            UpdateListenerInstanceStatus(listenerInstance, status, true);
            #endregion

            return succeeded;
        }

        public void Dispose()
        {
            
        }
    }
}
