using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using pushsample.iOS.Helpers;
using UIKit;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using pushsample.Helpers;
using Microsoft.Azure.Mobile.Distribute;

namespace pushsample.iOS
{
	[Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			App.ScreenWidth = (double)UIScreen.MainScreen.Bounds.Width;
			App.ScreenHeight = (double)UIScreen.MainScreen.Bounds.Height;

			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            Distribute.DontCheckForUpdatesInDebug();

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}


		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			pushsample.Helpers.Settings.DeviceToken = deviceToken.Description.Trim('<', '>').Replace(" ", "");
		}

		public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
		{
			DisplayAlert("Failed To Register Remote Notifications", error.ToString());
		}

		public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

			// The aps is a dictionary with the template values in it
			// You can adjust this section to do whatever you need to with the push notification

			string alert = string.Empty;
			if (aps.ContainsKey(new NSString("alert")))
				alert = (aps[new NSString("alert")] as NSString).ToString();

			string badge = string.Empty;
			if (aps.ContainsKey(new NSString("badge")))
				badge = (aps[new NSString("badge")] as NSNumber).ToString();

			string payload = string.Empty;
			if (aps.ContainsKey(new NSString("payload")))
				payload = (aps[new NSString("payload")] as NSString).ToString();

			var payloadData = JsonConvert.DeserializeObject<NotificationPayload>(payload);

			DisplayAlert("Success", alert + " " + badge);
		}

		void DisplayAlert(string title, string message, Action completionHandler = null)
		{
			var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

			alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));

			ViewControllerHelpers.GetVisibleViewController().PresentViewController(alert, true, completionHandler);
		}

    }
}
