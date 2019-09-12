using System;
namespace myTNB_Android.Src.NotificationDetails.MVP
{
    public class UserNotificationDetailContract
    {
        public interface IView
        {
            void ViewBill();
            void PayNow();
            void ContactUs();
            void ViewUsage();
        }
    }
}
