// Helpers/Settings.cs
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace pushsample.Helpers
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
  public static class Settings
	{
		static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		#region Setting Constants

		const string _googleUserId = "googleUserId";
		const string _fbUserId = "fbUserId";
		const string _deviceToken = "deviceToken";
		const string _googleRefreshToken = "googleRefreshToken";
		const string _googleAccessToken = "googleAccessToken";
		const string _registrationComplete = "registrationComplete";
		const string _azureUserId = "azureUserId";
		const string _azureAuthToken = "azureAuthToken";
		const string _azureNHId = "nhID";

		#endregion

		public static bool RegistrationComplete
		{
			get
			{
				return AppSettings.GetValueOrDefault<bool>(_registrationComplete, false);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_registrationComplete, value);
			}
		}

		public static string DeviceToken
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(_deviceToken, null);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_deviceToken, value);
			}
		}

		public static string RefreshToken
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(_googleRefreshToken, null);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_googleRefreshToken, value);
			}
		}

		public static string AccessToken
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(_googleAccessToken, null);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_googleAccessToken, value);
			}
		}

		public static string AccessTokenAndType
		{
			get
			{
				return AccessToken == null ? null : $"Bearer {AccessToken}";
			}
		}

		public static string GoogUserId
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(_googleUserId, null);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_googleUserId, value);
			}
		}

		public static string FBUserId
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(_fbUserId, null);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_fbUserId, value);
			}
		}

		public static string AzureUserId
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(_azureUserId, null);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_azureUserId, value);
			}
		}

		public static string AzureAuthToken
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(_azureAuthToken, null);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_azureAuthToken, value);
			}
		}

		public static string AzureNHID
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(_azureNHId, null);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_azureNHId, value);
			}
		}

        public static string isRegisteredForPushNotifs = nameof(isRegisteredForPushNotifs);
        public static bool IsRegisteredForPushNotifications
		{
			get
			{
                return AppSettings.GetValueOrDefault<bool>(isRegisteredForPushNotifs, false);
			}
			set
			{
                AppSettings.AddOrUpdateValue(isRegisteredForPushNotifs, value);
			}
		}



		private const string CurrentUserIdKey = nameof(CurrentUserIdKey);
		public const string DefaultCurrentUserId = "";

		public static User CurrentUser
		{
			get
			{
				string obj = AppSettings.GetValueOrDefault<string>(CurrentUserIdKey, DefaultCurrentUserId);
				if (obj == "null" || obj == "")
				{
					return new User();
				}

				return JsonConvert.DeserializeObject<User>(obj);
			}
			set
			{
				AppSettings.AddOrUpdateValue<string>(CurrentUserIdKey, JsonConvert.SerializeObject(value));
			}
		}
	}
}