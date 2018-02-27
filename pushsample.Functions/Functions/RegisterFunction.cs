
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace pushsample.Functions
{
    public static class RegisterFunction
    {
        [FunctionName(nameof(RegisterFunction))]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "RegisterWithHub")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("RegisterFunction Invoked");

            try
            {
                var requestBody = req.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<DeviceRegistration>(requestBody);

                if (string.IsNullOrEmpty(data.Handle) || string.IsNullOrEmpty(data.Platform))
                    return req.CreateResponse(HttpStatusCode.BadRequest, "Please ensure the requesty body includes Handle or Platform data");

                var NhID = PushManager.Instance.RegisterWithHub(data);

                return req.CreateResponse(HttpStatusCode.OK, NhID);
               
            }
            catch
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong, please ensure you pass a <DeviceRegistration> object in the request body");
            }
        }
    }

    public static class SendNotificationFunction
    {
        [FunctionName(nameof(SendNotificationFunction))]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SendPush/{tag}")]HttpRequestMessage req, string tag, TraceWriter log)
        {
            log.Info("SendNotification Invoked");
            
            try
            {
                if(string.IsNullOrEmpty(tag))
                    return req.CreateResponse(HttpStatusCode.BadRequest, "Please ensure you pass a tag in the query");

                PushManager.Instance.SendTestPushNotification(tag);

                return req.CreateResponse(HttpStatusCode.OK, "Push Notification Sent");
            }
            catch
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong, please ensure you pass a <DeviceRegistration> object in the request body");
            }
        }
    }

}
