using System;
using Xamarin.Forms;
using pushsample.Helpers;
using pushsample.Pages;
using pushsample.ViewModel;
namespace pushsample
{
	public class PushNotificationPage : BaseContentPage<PushPageViewModel>
	{
        public PushNotificationPage()
        {

            var currentUser = Settings.CurrentUser;

			var mainRelativeLayout = new RelativeLayout
			{
				WidthRequest = App.ScreenWidth,
				HeightRequest = App.ScreenHeight
			};

            var Name = new Label
            {
                HorizontalOptions = LayoutOptions.Center
            };
            Name.SetBinding(Label.TextProperty, nameof(ViewModel.Name), BindingMode.Default, null,
                            "Name: {0}");

            var Email = new Label
            {
                HorizontalOptions = LayoutOptions.Center
            };
            Email.SetBinding(Label.TextProperty, nameof(ViewModel.Email), BindingMode.Default, null,
                            "Email: {0}");

            var registerForPushNotificationsButton = new Button
            {
                Command = ViewModel.RegisterForPushNotificationCommand
            };

            registerForPushNotificationsButton.SetBinding(Button.TextProperty, nameof(ViewModel.RegisterForPushNotificationButtonLabel));

            var requestPushNotificationButton = new Button
            {
                Text = "Request Push Notification",
                Command = ViewModel.RequestPushNotificationCommand
            };
            requestPushNotificationButton.SetBinding(IsEnabledProperty, nameof(ViewModel.IsRegisteredForPushNotifications));


            var logOutButton = new Button
            {
                Text = "Logout",
                Command = ViewModel.LogoutCommand,
                Margin = 100
            };

			var activity = new ActivityIndicator
			{
				HeightRequest = 80,
				WidthRequest = 80,
				Color = Color.Blue,
			};
			activity.SetBinding(ActivityIndicator.IsRunningProperty, nameof(ViewModel.IsBusy));
            activity.SetBinding(IsEnabledProperty, nameof(ViewModel.IsBusy));
			Func<RelativeLayout, double> activityWidth = (parent) => activity.Measure(mainRelativeLayout.Width, mainRelativeLayout.Height).Request.Width;


			var pushLayout = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				Children = {
					Name,
					Email,
					registerForPushNotificationsButton,
					requestPushNotificationButton,
                    logOutButton
				},
				Padding = 30
			};


			mainRelativeLayout.Children.Add(pushLayout,
										   Constraint.RelativeToParent((parent) =>
										   {
											   return parent.X;
										   }),
										   Constraint.RelativeToParent((parent) =>
										   {
											   return parent.Y;
										   }),
										   Constraint.Constant(App.ScreenWidth),
										   Constraint.Constant(App.ScreenHeight * .8)
										  );

			/* LAY THE ACTIVITY INDICATOR OVER THE REST OF THE LOGIN SCREEN*/
			mainRelativeLayout.Children.Add(activity,
											Constraint.RelativeToParent((parent) =>
											{
												return parent.Bounds.Width / 2 - activityWidth(parent) / 2;
											}),
											Constraint.RelativeToParent((parent) =>
											{
												return pushLayout.Bounds.Center.Y;
											}));

			Content = mainRelativeLayout;

		}

        async void OnRegistrationCompleted(object sender, PushPageViewModel.PushNotifEventArgs e)
        {
            if (e.IsSuccessful)
            {
                await DisplayAlert("Success!", "Registration Succesful", "OK");
            }
            else
            {
				await DisplayAlert("Failure :(", "Registration Was Not Succesful, Sorry!", "OK");
            }
        }

		async void OnLogoutCompleted(object sender, PushPageViewModel.LogoutEventArgs e)
		{
			if (e.IsSuccessful)
			{
                var topPage = Navigation.GetPage<LoginPage>();
                if (topPage == null)
                {
                    await Navigation.PushModalAsync(new LoginPage());
                }
                else
                {
                    await Navigation.PopModalAsync();
                }

			}
			else
			{
                await DisplayAlert("Failed to Logout", $"Reason {e.Reason}", "OK");
			}
		}

        public override void SubscribeEventHandlers()
        {
			ViewModel.RegistrationCompleted += OnRegistrationCompleted;
            ViewModel.LogoutComplete += OnLogoutCompleted;
		}

        public override void UnsubscribeEventHandlers()
        {
            ViewModel.RegistrationCompleted -= OnRegistrationCompleted;
            ViewModel.LogoutComplete -= OnLogoutCompleted;
        }
    }
}
