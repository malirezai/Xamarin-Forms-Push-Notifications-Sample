using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using pushsample.Helpers;
using Xamarin.Forms;
using System.Diagnostics;

namespace pushsample
{
	public class AzureService
	{
		public AzureService()
		{
		}

		static AzureService _instance;
		public static AzureService Instance
		{
			get
			{
				return _instance ?? (_instance = new AzureService());
			}
		}

		MobileServiceClient _client;
		public MobileServiceClient Client
		{
			get
			{
				if (_client == null)
				{
					//var authHandler = new AuthHandler();

					_client = new MobileServiceClient(Keys.AzureDomain);//,authHandler);

					//authHandler.Client = _client;

				}
				return _client;
			}
		}

		//public void SetupPushNotifications()
		//{
		//	Client.

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

				var registrationId = await Client.InvokeApiAsync<DeviceRegistration, string>("registerWithHub", reg, HttpMethod.Put, null);

				return registrationId;

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
				await Client.InvokeApiAsync($"sendTestPushNotification?email={email}", HttpMethod.Get, null);
				if (Device.RuntimePlatform.Equals(Device.iOS))
				{
					DependencyService.Get<IPushNotifications>().SetBadgeIcon(0);
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}

		#endregion

	}
}
