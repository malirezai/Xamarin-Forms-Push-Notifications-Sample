using System;

using Xamarin.Forms;
using pushsample.Helpers;

namespace pushsample
{
	public class App : Application
	{
		public static double ScreenWidth;
		public static double ScreenHeight;
		public const string AppName = "pushsample";
		public static bool IS_PUSH_AVAILABLE;
        public event EventHandler<NotificationEventArgs> AppNotificationReceived;
        static App _instance;
        public static App Instance => _instance;


        public App()
        {
            _instance = this;
            if (Settings.CurrentUser.Name != null)
            {
                MainPage = new NavigationPage(new PushNotificationPage());
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }

            Push.Instance.OnNotificationReceived += OnNotificationReceived;
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
       //     MobileCenter.Start($"android={Keys.Android_MobileCenter_Key};" +
				   //$"ios={Keys.iOS_MobileCenter_Key}",
                               //typeof(Analytics), typeof(Crashes),typeof(Distribute));

        }

        void OnNotificationReceived(object sender, NotificationEventArgs e)
        {
            AppNotificationReceived?.Invoke(this, e);
        }
	}
}
