using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using pushsample.Helpers;
using System.Diagnostics;
namespace pushsample.ViewModel
{
    public class PushPageViewModel:BaseViewModel
    {

        Command _registerForPushNotificationCommand;
        Command _requestPushNotificationCommand;
        Command _logoutCommand;

        string name = Settings.CurrentUser.Name;
        string email = Settings.CurrentUser.UserEmail;
       
		public event EventHandler<PushNotifEventArgs> RegistrationCompleted;
        public class PushNotifEventArgs : EventArgs
        {
            public bool IsSuccessful;
        }

        public event EventHandler<LogoutEventArgs> LogoutComplete;
		public class LogoutEventArgs : EventArgs
		{
			public bool IsSuccessful;
            public string Reason;
		}

		public Command RegisterForPushNotificationCommand => 
            _registerForPushNotificationCommand ?? (_registerForPushNotificationCommand = new Command(async () => await ExecuteRegisterForPushNotifcationCommand()));

		public Command RequestPushNotificationCommand => 
            _requestPushNotificationCommand ?? (_requestPushNotificationCommand = new Command(async () => await ExecuteRequestPushNotifcationCommand()));

		public Command LogoutCommand =>
        _logoutCommand ?? (_logoutCommand = new Command(async () => await ExecuteLogoutCommand()));



		public PushPageViewModel()
        {
			Title = "Profile";
        }

        public string Name
        {
            get { return name; }
        }

		public string Email
		{
			get { return email; }
		}

        public string RegisterForPushNotificationButtonLabel =>
            IsRegisteredForPushNotifications ? "REGISTERED" : "REGISTER";
      
        public bool IsRegisteredForPushNotifications =>
            string.IsNullOrEmpty(Settings.AzureNHID) ? false : true;


        async Task ExecuteRegisterForPushNotifcationCommand()
        {

			if (Device.RuntimePlatform.Equals(Device.iOS))
			{
                await DependencyService.Get<IPushProvider>().RegisterForPushNotifications();
			}

			var token = Settings.DeviceToken;

			if (token != null)
			{
                IsBusy = true;
				var nhID = await AzureService.Instance.RegisterForPushNotifications();
				if (nhID != null)
				{
					Settings.AzureNHID = nhID;
                    IsBusy = false;
                    OnRegistrationCompleted(true);
				}
				else
				{
                    IsBusy = false;
                    OnRegistrationCompleted(false);
				}
			}

        }

		async Task ExecuteRequestPushNotifcationCommand()
		{
            await AzureService.Instance.RequestPushNotification();
		}

        async Task ExecuteLogoutCommand()
        {
            try
            {
                Settings.CurrentUser = null;
                OnLogoutComplete(true, "success");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                OnLogoutComplete(false, e.ToString());
            }
        }

        void OnRegistrationCompleted(bool isSuccess)
        {
            OnPropertyChanged(nameof(IsRegisteredForPushNotifications));
            OnPropertyChanged(nameof(RegisterForPushNotificationButtonLabel));
            RegistrationCompleted?.Invoke(this, new PushNotifEventArgs { IsSuccessful = isSuccess });
        }

		void OnLogoutComplete(bool isSuccess,string reason) =>
            LogoutComplete?.Invoke(this, new LogoutEventArgs { IsSuccessful = isSuccess, Reason = reason });

        public override void OnNotificationReceived(NotificationEventArgs args)
        {
            Device.BeginInvokeOnMainThread(async () =>
                await App.Instance.MainPage.DisplayAlert(args.Title, args.Message, "OK")
            );
        }
	}
}
