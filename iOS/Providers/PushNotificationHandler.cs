using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using UserNotifications;

[assembly: Xamarin.Forms.Dependency(typeof(pushsample.iOS.PushNotificationHandler))]

namespace pushsample.iOS
{
    public class PushNotificationHandler: IPushProvider
	{
        public string DeviceToken { get; set; }

        public event EventHandler<NotificationEventArgs> OnNotificationReceived;

        public Task<bool> IsDeviceRegisteredForRemotePushNotifications()
		{
			var tcs = new TaskCompletionSource<bool>();

			UIApplication.SharedApplication.InvokeOnMainThread(() =>
			{
				tcs.SetResult(UIApplication.SharedApplication.IsRegisteredForRemoteNotifications);
			});

			return tcs.Task;
		}

        internal void NotifyReceived(NotificationEventArgs e)
        {
            OnNotificationReceived?.Invoke(this, e);
        }

        public async Task RegisterForPushNotifications()
        {
            await Task.Run(() =>
            {
                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                        RegiserForPushNotifications_iOS10();
                    else
                        RegisterForPushNotifications_iOS9();
                });
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
