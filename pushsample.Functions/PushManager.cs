using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Newtonsoft.Json;

namespace pushsample.Functions
{
    public class PushManager
    {
        static PushManager _instance;
        public static PushManager Instance => 
            _instance ?? (_instance = new PushManager());
            
        
        NotificationHubClient _hub = NotificationHubClient.CreateClientFromConnectionString(Constants.HubConnectionString, Constants.HubName);

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
            
        }

        public async Task NotifyByTag(string message, string tag, NotificationPayload payload = null, int? badgeCount = null)
        {
            await NotifyByTags(message, new List<string> { tag }, payload, badgeCount);
        }

        public async Task<string> RegisterWithHub(DeviceRegistration deviceUpdate)
        {
            string newRegistrationId = null;
            try
            {
                // make sure there are no existing registrations for this push handle (used for iOS and Android)
                if (deviceUpdate.Handle != null)
                {
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


        public async Task SendTestPushNotification(string email)
        {
            if (email != null)
            {
                var message = "Push notifications are working for you - excellent!";
                var payload = new NotificationPayload { Action = "hello!" };
                payload.Payload.Add(
                    "email", email);
                await NotifyByTag(message, email, payload, 1);
            }
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








    }
}
