using System;
using Android.App;
using Firebase.Iid;
using Android.Util;
using Firebase.Messaging;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;
using Newtonsoft.Json;
using pushsample.Helpers;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(pushsample.Droid.PushProvider))]

namespace pushsample.Droid
{
    public class PushProvider : IPushProvider
    {
        public PushProvider()
        {
            if (MyFirebaseInstanceIdService._loggedToken != null)
                DeviceToken = MyFirebaseInstanceIdService._loggedToken;
        }

        public string DeviceToken { get; set; }
        public event EventHandler<NotificationEventArgs> OnNotificationReceived;

        internal void NotifyReceived(NotificationEventArgs e)
        {
            OnNotificationReceived?.Invoke(this, e);
        }

        async Task IPushProvider.RegisterForPushNotifications()
        {
            throw new NotImplementedException();
        }
    }

    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseInstanceIdService : FirebaseInstanceIdService
    {
        internal static string _loggedToken;
        public override void OnTokenRefresh()
        {
            base.OnTokenRefresh();

            if (FirebaseInstanceId.Instance == null)
                return;

            var refreshedToken = FirebaseInstanceId.Instance.Token;

            if (Forms.IsInitialized)
                Push.Instance.DeviceToken = refreshedToken;
            else
                _loggedToken = refreshedToken;

            Settings.DeviceToken = refreshedToken;

            Console.Write($"Token: {refreshedToken}");
        }
    }

    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            global::Android.OS.PowerManager.WakeLock sWakeLock;
            var pm = global::Android.OS.PowerManager.FromContext(Forms.Context);
            sWakeLock = pm.NewWakeLock(global::Android.OS.WakeLockFlags.Partial, "FCM Broadcast Reciever Tag");
            sWakeLock.Acquire();

            var notification = message.GetNotification();
            string body = message.GetNotification()?.Body ?? message.Data["message"];
            string title = message.GetNotification()?.Title ?? message.Data["title"];

            //var msg = message.Data["message"];
            //var title = message.Data.ContainsKey("title") ? message.Data["title"] : "";

            var notificationArgs = new NotificationEventArgs
            {
                Title = message.Data["title"],
                Message = message.Data["message"],
            };

            var payload = message.Data["payload"];

            if (!string.IsNullOrWhiteSpace(payload))
            {
                notificationArgs.Payload = JsonConvert.DeserializeObject<NotificationPayload>(payload).Payload;
            }

            ((PushProvider)Push.Instance).NotifyReceived(notificationArgs);

            if (!string.IsNullOrEmpty(notificationArgs.Title) || !string.IsNullOrWhiteSpace(notificationArgs.Message))
                SendNotification(title, body);
        }

        void SendNotification(string title, string message)
        {
            var activityIntent = new Intent(this, typeof(MainActivity));
            activityIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, activityIntent, PendingIntentFlags.UpdateCurrent);
            var defaultSoundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);

            var n = new NotificationCompat.Builder(this)
                .SetSmallIcon(Resource.Drawable.notification_template_icon_bg)
                .SetLights(global::Android.Graphics.Color.Green, 300, 1000)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetTicker(message)
                .SetContentText(message)
                .SetAutoCancel(true)
                .SetSound(defaultSoundUri)
                .SetVibrate(new long[] { 200, 200, 100 });

            var nm = NotificationManager.FromContext(Forms.Context);
            nm.Notify(0, n.Build());
        }

        class NotificationPayload
        {
            public string Action;
            public Dictionary<string, string> Payload = new Dictionary<string, string>();
        }

    }
}