using System;
using Android.App;
using Android.Content;
using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Diagnostics;
using pushsample;
using pushsample.Helpers;
using pushsample.Droid;
using Microsoft.WindowsAzure.MobileServices;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: Dependency(typeof(pushsample.Droid.PushNotifications))]

namespace pushsample.Droid
{
	public class PushNotifications : IPushNotifications
	{
		public void SetBadgeIcon(int badgeNum)
		{
			throw new NotImplementedException();
		}

		public void RegisterForPushNotifications()
		{
			
		}

		public void RequestPushNotification()
		{
			throw new NotImplementedException();
		}
	}

	[Service]
	[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
	public class MyFirebaseMessagingService : FirebaseMessagingService
	{
		const string TAG = "MyFirebaseMsgService";
		public override void OnMessageReceived(RemoteMessage message)
		{

			global::Android.OS.PowerManager.WakeLock sWakeLock;
			var pm = global::Android.OS.PowerManager.FromContext(Forms.Context);
			sWakeLock = pm.NewWakeLock(global::Android.OS.WakeLockFlags.Partial, "FCM Broadcast Reciever Tag");
			sWakeLock.Acquire();

			var notification = message.GetNotification();
			string body = message.GetNotification()?.Body ?? message.Data["message"];
			string title = message.GetNotification()?.Title ?? message.Data["title"];

			//if (!HandleNotification(message.GetNotification().Body, message.GetNotification().Title))
			//{
			//	base.OnMessageReceived(message);
			//}
			HandleNotification(body, title);

			sWakeLock.Release();

		}

		//internal static bool
		void HandleNotification(string messageBody, string messageTitle)
		{
			var intent = new Intent(Forms.Context, typeof(MainActivity));
			intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
			var pendingIntent = PendingIntent.GetActivity(Forms.Context, 0, intent, PendingIntentFlags.UpdateCurrent);


			var n = new Notification.Builder(Forms.Context);
			n.SetSmallIcon(Resource.Drawable.notification_template_icon_bg);
			n.SetLights(Android.Graphics.Color.Blue, 300, 1000);
			n.SetContentIntent(pendingIntent);
			n.SetContentTitle(messageTitle);
			n.SetTicker(messageBody);
			//n.SetLargeIcon(global::Android.Graphics.BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.icon));
			n.SetAutoCancel(true);
			n.SetContentText(messageBody);
			n.SetVibrate(new long[] {
						200,
						200,
						100,
					});

			var nm = NotificationManager.FromContext(Forms.Context);
			nm.Notify(0, n.Build());

			// Make call to Xamarin.Forms here with the event handler

		}
	}


	[Service]
	[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
	public class MyFirebaseIIDService : FirebaseInstanceIdService
	{
		const string TAG = "MyFirebaseIIDService";
		public override void OnTokenRefresh()
		{
			try
			{
				var refreshedToken = FirebaseInstanceId.Instance.Token;
				Settings.DeviceToken = refreshedToken;
				Log.Debug(TAG, "Refreshed token: " + refreshedToken);
				SendRegistrationToServer(refreshedToken);
			}
			catch (Exception e)
			{

			}
		}
		void SendRegistrationToServer(string token)
		{
			// Add custom implementation, as needed.
		}
	}
}