using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using pushsample.Helpers;
using Xamarin.Forms;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Azure.EventHubs;

namespace pushsample
{
	public class AzureService
	{
        
        static EventHubClient eventHubClient;

        const string EventHubConnectionString = "Endpoint=sb://mahdidemo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=CFPOuz4wBXagBuQHxL8W1tcRoggf28Sp4rjDDL8tjas=";

        const string EventHubName = "myeventhub";


		static AzureService _instance;
		public static AzureService Instance
		{
			get
			{
				return _instance ?? (_instance = new AzureService());
			}
		}

        public AzureService()
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)

            {

                EntityPath = EventHubName

            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

        }

		//MobileServiceClient _client;
		//public MobileServiceClient Client
		//{
		//	get
		//	{
		//		if (_client == null)
		//		{
		//			//var authHandler = new AuthHandler();

		//			_client = new MobileServiceClient(Keys.AzureDomain);//,authHandler);

		//			//authHandler.Client = _client;

		//		}
		//		return _client;
		//	}
		//}

		#region Push Notifications

		public async Task<string> RegisterForPushNotifications()
		{

			try
			{
                //List of Tags to Register With. In this example we are taking the email and "All"
				var tags = new List<string>
				{
					Settings.CurrentUser.UserEmail,
					"All"
				};

				var OS = Device.RuntimePlatform;
				Settings.CurrentUser.DevicePlatform = OS;

				var reg = new DeviceRegistration
				{
					Handle = Settings.DeviceToken,
					Platform = OS,
					Tags = tags.ToArray()
				};



                HttpClient _client = new HttpClient();
                var json = JsonConvert.SerializeObject(reg);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await _client.PostAsync("https://nhfunctionssample.azurewebsites.net/api/RegisterWithHub", content);

                if (res.IsSuccessStatusCode)
                {
                    var registrationIdString = await res.Content.ReadAsStringAsync();
                    var regId = JsonConvert.DeserializeObject<Dictionary<string, string>>(registrationIdString)["Result"];
                    return regId;
                }
                //var registrationId = await Client.InvokeApiAsync<DeviceRegistration, string>("registerWithHub", reg, HttpMethod.Put, null);
                else
                    return null;

			}
			catch(Exception e)
			{
				Debug.WriteLine(e.Message);
				return null;
			}
		}


		public async Task RequestPushNotification()
		{
			var email = Settings.CurrentUser.UserEmail;
			try
			{
                HttpClient _client = new HttpClient();

                await _client.GetAsync($"https://nhfunctionssample.azurewebsites.net/api/SendPush/{email}");

				if (Device.RuntimePlatform.Equals(Device.iOS))
				{
					//DependencyService.Get<IPushNotifications>().SetBadgeIcon(0);
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}

		#endregion

        public class EventHubMessage
        {
            public string Message;
            public string Type;
            public string Tag;
        }

	}
}
