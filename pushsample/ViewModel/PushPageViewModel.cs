using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using pushsample.Helpers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
namespace pushsample.ViewModel
{
    public class PushPageViewModel:BaseViewModel
    {

        Command _registerForPushNotificationCommand;
        Command _requestPushNotificationCommand;
        Command _logoutCommand;

        string registerForPushNotificationButtonLabel;
        string name = Settings.CurrentUser.Name;
        string email = Settings.CurrentUser.UserEmail;
        bool isRegisteredForPushNotifications = false;// = Settings.IsRegisteredForPushNotifications;

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
            if(IsRegisteredForPushNotifications)
                RegisterForPushNotificationButtonLabel = "REGISTERED";
            else
                RegisterForPushNotificationButtonLabel = "Register";
        }

        public string Name
        {
            get { return name; }
        }

		public string Email
		{
			get { return email; }
		}

        public string RegisterForPushNotificationButtonLabel
        {
            get
            {
                return registerForPushNotificationButtonLabel;
            }
            set
            {
                SetProperty(ref registerForPushNotificationButtonLabel, value);
            }
        }

        public bool IsRegisteredForPushNotifications
        {

            get
            {
                return isRegisteredForPushNotifications;
            }

            set
            {
                SetProperty(ref isRegisteredForPushNotifications,value);
            }
        }


        async Task ExecuteRegisterForPushNotifcationCommand()
        {

			if (Device.RuntimePlatform.Equals(Device.iOS))
			{
				DependencyService.Get<IPushNotifications>().RegisterForPushNotifications();
			}

			var token = Settings.DeviceToken;

			if (token != null)
			{
                IsBusy = true;
				var nhID = await AzureService.Instance.RegisterForPushNotifications();
				if (nhID != null)
				{
					Settings.AzureNHID = nhID;
                    IsRegisteredForPushNotifications = true;
					RegisterForPushNotificationButtonLabel = "REGISTERED";
                    IsBusy = false;
                    OnRegistrationCompleted(true);
				}
				else
				{
                    IsRegisteredForPushNotifications = false;
					RegisterForPushNotificationButtonLabel = "Register";
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
                IsRegisteredForPushNotifications = false;
                OnLogoutComplete(true, "success");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                OnLogoutComplete(false, e.ToString());
            }
        }

		void OnRegistrationCompleted(bool isSuccess) =>
            RegistrationCompleted?.Invoke(this, new PushNotifEventArgs { IsSuccessful = isSuccess});

		void OnLogoutComplete(bool isSuccess,string reason) =>
            LogoutComplete?.Invoke(this, new LogoutEventArgs { IsSuccessful = isSuccess, Reason = reason });

	}
}
