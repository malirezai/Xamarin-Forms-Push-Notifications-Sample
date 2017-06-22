using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using UserNotifications;

[assembly: Xamarin.Forms.Dependency(typeof(pushsample.iOS.PushNotificationHandler_iOS))]

namespace pushsample.iOS
{
	public class PushNotificationHandler_iOS : IPushNotifications
	{
		public PushNotificationHandler_iOS()
		{
		}

		public Task<bool> IsDeviceRegisteredForRemotePushNotifications()
		{
			var tcs = new TaskCompletionSource<bool>();

			UIApplication.SharedApplication.InvokeOnMainThread(() =>
			{
				tcs.SetResult(UIApplication.SharedApplication.IsRegisteredForRemoteNotifications);
			});

			return tcs.Task;
		}

		public void RequestPushNotification()
		{
			throw new NotImplementedException();
		}

		public void RegisterForPushNotifications()
		{
			UIApplication.SharedApplication.InvokeOnMainThread(() =>
			{
				if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
					RegiserForPushNotifications_iOS10();
				else
					RegisterForPushNotifications_iOS9();
			});
		}

		void RegisterForPushNotifications_iOS9()
		{
			var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, new NSSet());

			UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
			UIApplication.SharedApplication.RegisterForRemoteNotifications();

		}

		void RegiserForPushNotifications_iOS10()
		{
			UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, async (approved, err) =>
			{
				var isDeviceRegisteredForPushNotifications = await IsDeviceRegisteredForRemotePushNotifications();

				if (approved && !isDeviceRegisteredForPushNotifications)
					UIApplication.SharedApplication.InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
			});
		}

		public void SetBadgeIcon(int badgeNum)
		{
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = badgeNum;
		}
	}
}
