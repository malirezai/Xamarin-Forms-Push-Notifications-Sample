using System;

using Xamarin.Forms;
using pushsample.Helpers;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Distribute;

namespace pushsample
{
	public class App : Application
	{
		public static double ScreenWidth;
		public static double ScreenHeight;
		public const string AppName = "pushsample";
		public static bool IS_PUSH_AVAILABLE;

		public App()
		{
			if (Settings.CurrentUser.Name!=null)
			{
                MainPage = new NavigationPage(new PushNotificationPage());
			}
			else
			{
				MainPage = new NavigationPage(new LoginPage());
			}
		}

		protected override void OnStart()
		{
            // Handle when your app starts
            InitializeMobileCenter();
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}

        void InitializeMobileCenter()
        {
            MobileCenter.Start($"android={Keys.Android_MobileCenter_Key};" +
				   $"ios={Keys.iOS_MobileCenter_Key}",
                               typeof(Analytics), typeof(Crashes),typeof(Distribute));

        }
	}
}
