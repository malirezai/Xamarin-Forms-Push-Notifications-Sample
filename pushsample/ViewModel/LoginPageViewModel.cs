using System;
using System.Reflection;
using Xamarin.Forms;
using System.Threading.Tasks;
using pushsample.Helpers;
using System.Collections.Generic;
namespace pushsample.ViewModel
{
    public class LoginPageViewModel : BaseViewModel
    {

        public Command CustomLoginCommand { get; set; }

		public event EventHandler<string> LoginFailed;
		public event EventHandler LoginSuccess;

        string _name, _password;


		public LoginPageViewModel()
        {
            Title = "Login";
            CustomLoginCommand = new Command(async () => await ExecuteLoginCommand(AuthOption.Custom));

		}

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        public string Password
        {
			get
			{
				return _password;
			}
			set
			{
				SetProperty(ref _password, value);
			}
        }


        async Task ExecuteLoginCommand(AuthOption authOption)
        {
            if (string.IsNullOrEmpty(Name))
            {
                OnLoginFailed("Username Field is Empty");
            }
            else
            {
                Settings.CurrentUser = new User
                {
                    Name = Name,
                    UserEmail = $"{Name}@example.com"
                };

                OnLoginSuccess();
            }
        }

        public void OnLoginSuccess() =>
            LoginSuccess?.Invoke(this, EventArgs.Empty);

		public void OnLoginFailed(string reason) =>
            LoginFailed?.Invoke(this, reason);

    }
}
