using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pushsample
{
	public interface IPushNotifications
	{
		void RegisterForPushNotifications();
		void RequestPushNotification();
		void SetBadgeIcon(int badgeNum);
	}

	public class IncomingPushNotificationEventArgs : EventArgs
	{
		public Dictionary<string, object> Payload
		{
			get;
			set;
		}
	}
}
