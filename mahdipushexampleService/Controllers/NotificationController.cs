using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using mahdipushexampleService.Models;
using System.Web.Http.Controllers;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using mahdipushexampleService.Controllers;
using System.Net.Http;

namespace mahdipushexampleService.Controllers
{
    [MobileAppController]
    public class NotificationController : ApiController
    {
        mahdipushexampleContext _context = new mahdipushexampleContext();
        NotificationHubClient _hub = NotificationHubClient.CreateClientFromConnectionString(Constants.HubConnectionString, Constants.HubName);

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
        }

        #region DEVICE REGISTRATION OBJECT
        public class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
        }

        public class NotificationPayload
        {
            public NotificationPayload()
            {
                Payload = new Dictionary<string, string>();
            }

            public string Action
            {
                get;
                set;
            }

            public Dictionary<string, string> Payload
            {
                get;
                set;
            }
        }

        #endregion


        // GET api/Notification
        public string Get()
        {
            return "Hello from custom controller!";
        }

        #region Notification

        public async Task NotifyByTags(string message, List<string> tags, NotificationPayload payload = null, int? badgeCount = null)
        {
            var notification = new Dictionary<string, string> { { "message", message } };

            if (payload != null)
            {
                var json = JsonConvert.SerializeObject(payload);
                notification.Add("payload", json);
            }
            else
            {
                notification.Add("payload", "");
            }

            if (badgeCount == null)
                badgeCount = 0;

            notification.Add("badge", badgeCount.Value.ToString());

            try
            {
                await _hub.SendTemplateNotificationAsync(notification, tags);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var i = 10;
        }

        public async Task NotifyByTag(string message, string tag, NotificationPayload payload = null, int? badgeCount = null)
        {
            await NotifyByTags(message, new List<string> { tag }, payload, badgeCount);
        }

        #endregion

        #region Registration

        [HttpPut]
        [Route("api/registerWithHub")]
        public async Task<string> RegisterWithHub(DeviceRegistration deviceUpdate)
        {
            string newRegistrationId = null;
            try
            {
                // make sure there are no existing registrations for this push handle (used for iOS and Android)
                if (deviceUpdate.Handle != null)
                {
                    //Azure likes to uppercase the iOS device handles for some reason - no worries tho, I only spent 2 hours tracking this down
                    if (deviceUpdate.Platform == "iOS")
                        deviceUpdate.Handle = deviceUpdate.Handle.ToUpper();

                    var registrations = await _hub.GetRegistrationsByChannelAsync(deviceUpdate.Handle, 100);

                    foreach (var reg in registrations)
                    {
                        if (newRegistrationId == null)
                        {
                            newRegistrationId = reg.RegistrationId;
                        }
                        else
                        {
                            await _hub.DeleteRegistrationAsync(reg);
                        }
                    }
                }

                if (newRegistrationId == null)
                    newRegistrationId = await _hub.CreateRegistrationIdAsync();

                RegistrationDescription registration = null;

                switch (deviceUpdate.Platform)
                {
                    case "iOS":
                        var alertTemplate = "{\"aps\":{\"alert\":\"$(message)\",\"badge\":\"#(badge)\",\"payload\":\"$(payload)\"}}";
                        registration = new AppleTemplateRegistrationDescription(deviceUpdate.Handle, alertTemplate);
                        break;
                    case "Android":
                        var messageTemplate = "{\"data\":{\"title\":\"Test\",\"message\":\"$(message)\",\"payload\":\"$(payload)\"}}";
                        registration = new GcmTemplateRegistrationDescription(deviceUpdate.Handle, messageTemplate);
                        break;
                    default:
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                registration.RegistrationId = newRegistrationId;
                registration.Tags = new HashSet<string>(deviceUpdate.Tags);
                await _hub.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return newRegistrationId;
        }

        [HttpGet]
        [Route("api/sendTestPushNotification")]
        public async Task SendTestPushNotification(string email)
        {
            if (email != null)
            {
                var message = "Push notifications are working for you - excellent!";
                var payload = new NotificationPayload { Action = "hello!" };
                payload.Payload.Add(
                    "email", email);
                await NotifyByTag(message, email,payload,1);
            }
        }
        #endregion

        #region unregister

        /* THIS MEHTOD SHOULD REALLY NEVER BE USED BECAUSE THE PLATFORM's TOKEN IS NOT TIED TO THE USER. INSTEAD CALL ABOVE MEHTOD TO REASSIGN THE TAG TO A DIFFERENT EMAIL ADDRESS FOR THE SAME PLATFORM TOKEN */
        [HttpDelete]
        [Route("api/unregister")]
        public async Task<HttpResponseMessage> Delete(string id)
        {
            await _hub.DeleteRegistrationAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        {
            var webex = e.InnerException as WebException;
            if (webex == null || webex.Status != WebExceptionStatus.ProtocolError)
                return;

            var response = (HttpWebResponse)webex.Response;
            if (response.StatusCode == HttpStatusCode.Gone)
                throw new HttpRequestException(HttpStatusCode.Gone.ToString());
        }

        #endregion
        
        //public static HttpResponseException ToException(this string s, HttpRequestMessage msg)
        //{
        //    return new HttpResponseException(msg.CreateBadRequestResponse(s));
        //}

    }
}
