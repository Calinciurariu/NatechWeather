//using Android.App;
//using System;
//using Android.App;
//using Android.Content;
//using Firebase.Messaging;
//using Plugin.Firebase.CloudMessaging;

//namespace NatechWeather.Platforms.Android.Services
//{
//    [Service(Exported = true)]
//    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
//    public class FirebaseService : FirebaseMessagingService
//    {
//        public override void OnNewToken(string token)
//        {
//            base.OnNewToken(token);
//            Console.WriteLine($"FCM Token: {token}");
//        }

//        public override void OnMessageReceived(RemoteMessage message)
//        {
//            base.OnMessageReceived(message);

//            var notification = message.GetNotification();
//            if (notification != null)
//            {
//                Console.WriteLine($"Notification Received: {notification.Title} - {notification.Body}");
//            }
//        }
//    }
//}
