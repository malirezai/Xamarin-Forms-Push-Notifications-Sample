using System;
using Xamarin.Forms;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.MobileServices;
using System.Net.Http;
using pushsample.Controls;
using pushsample.Helpers;
using pushsample.ViewModel;
using pushsample.Pages;

namespace pushsample
{
    public class LoginPage : BaseContentPage<LoginPageViewModel>
	{

        public LoginPage()
        {
            NavigationPage.SetHasBackButton(this, false);

			ViewModel.LoginSuccess += LoginSuccess;
			ViewModel.LoginFailed += LoginFailed;

            var mainRelativeLayout = new RelativeLayout
            {
                WidthRequest = App.ScreenWidth,
                HeightRequest = App.ScreenHeight
            };

            
            var userName = new Entry
            {
                Placeholder = "Username",
                WidthRequest = 200
            };
            userName.SetBinding(Entry.TextProperty, nameof(ViewModel.Name));
            Func<RelativeLayout, double> userNameWidth = (parent) => userName.Measure(mainRelativeLayout.Width, mainRelativeLayout.Height).Request.Width;

            var passWord = new Entry
            {
                Placeholder = "Password",
                IsPassword = true
            };
            passWord.SetBinding(Entry.TextProperty, nameof(ViewModel.Password));
            Func<RelativeLayout, double> passwordWidth = (parent) => passWord.Measure(mainRelativeLayout.Width, mainRelativeLayout.Height).Request.Width;


            var registerButton = new Button
            {
                Text = "Register",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = Color.Transparent,
                TextColor = Color.FromRgb(7, 62, 164),
                FontSize = 18,
                BorderRadius = 5,
                BorderWidth = 2,
                BorderColor = Color.FromRgb(7, 62, 164),
                WidthRequest = 140,
                AutomationId = "loginButton",
                Command = ViewModel.CustomLoginCommand
            };
            Func<RelativeLayout, double> registerBtnWidth = (parent) => registerButton.Measure(mainRelativeLayout.Width, mainRelativeLayout.Height).Request.Width;


            var activity = new ActivityIndicator
            {
                HeightRequest = 80,
                WidthRequest = 80,
                Color = Color.Blue,
            };
            activity.SetBinding(ActivityIndicator.IsRunningProperty, nameof(ViewModel.IsBusy));
			activity.SetBinding(IsEnabledProperty, nameof(ViewModel.IsBusy));

			Func<RelativeLayout, double> activityWidth = (parent) => activity.Measure(mainRelativeLayout.Width, mainRelativeLayout.Height).Request.Width;


            /* ADD CONTROLS TO MAIN LAYOUT */
            var loginLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children = {
                    userName,
                    passWord,
                    registerButton
                },
                Padding = 40
            };

            mainRelativeLayout.Children.Add(loginLayout,
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
                                                return loginLayout.Bounds.Center.Y;
                                            }));

            Content = mainRelativeLayout;

          
        }

		async void LoginSuccess(object sender, EventArgs e)
		{
			var pushPage = Navigation.GetPage<PushNotificationPage>();
			if (pushPage == null)
			{
				await Navigation.PushModalAsync(new PushNotificationPage());
			}
			else
			{
				await Navigation.PopModalAsync();
			}
		}

		void LoginFailed(object sender, string e)
		{
			Device.BeginInvokeOnMainThread(async () =>
                      await DisplayAlert("Error", e , "Ok"));
		}

        public override void SubscribeEventHandlers()
        {
            //ViewModel.LoginSuccess += LoginSuccess;
            //ViewModel.LoginFailed += LoginFailed;
        }

        public override void UnsubscribeEventHandlers()
        {
            //ViewModel.LoginSuccess -= LoginSuccess;
            //ViewModel.LoginFailed -= LoginFailed;
        }
    }
}
