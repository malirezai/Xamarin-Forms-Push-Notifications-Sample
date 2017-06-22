using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common;
using Firebase.Messaging;
using Firebase.Iid;
using pushsample.Helpers;
using Android.Util;
using Xamarin.Forms;

namespace pushsample.Droid
{
	[Activity(Label = "pushsample.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		public static MainActivity instance;
		
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			instance = this;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			//  handle intents //
			if (Intent.Extras != null)
			{
				foreach (var key in Intent.Extras.KeySet())
				{
					var value = Intent.Extras.GetString(key);
					Log.Debug("FIREBASE SERVICE", "Key: {0} Value: {1}", key, value);
				}

			}

			App.IS_PUSH_AVAILABLE = IsPlayServicesAvailable();

			#region screen height & width

			var pixels = Resources.DisplayMetrics.WidthPixels;
			var scale = Resources.DisplayMetrics.Density;

			double dps = (double)((pixels - 0.5f) / scale);

			App.ScreenWidth = dps;

			pixels = Resources.DisplayMetrics.HeightPixels;
			dps = (double)((pixels - 0.5f) / scale);

			App.ScreenHeight = dps;

			#endregion

			LoadApplication(new App());
		}

		// CHECK IF GOOGLE PLAY SERVICES IS AVAILABLE

		public bool IsPlayServicesAvailable()
		{
			int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
			if (resultCode != ConnectionResult.Success)
			{
				//if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
				//	msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
				//else
				//{
				//	msgText.Text = "This device is not supported";
				//	Finish();
				//}
				return false;
			}
			else
			{
				//msgText.Text = "Google Play Services is available.";
				return true;
			}
		}
	}
}
