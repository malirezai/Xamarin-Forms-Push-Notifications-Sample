using System;
namespace pushsample
{
	public class User
	{
        public string SocialToken { get; set; }
		public string Name { get; set; }
		public string DeviceToken { get; set; }
		public string DevicePlatform { get; set; }
		public string UserID { get; set; }
		public string UserEmail { get; set; }
		public string SocialID { get; set; }
		public string ProfilePicture { get; set; }
        public string ServerAuthCodeGoogle { get; set; }
	}
}
