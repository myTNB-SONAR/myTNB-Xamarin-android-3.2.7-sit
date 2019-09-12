using System;
using System.Collections.Generic;
using myTNB_Android.Src.NotificationDetails.Models;

namespace myTNB_Android.Src.NotificationDetails.MVP
{
    public class UserNotificationDetailPresenter
    {
        UserNotificationDetailContract.IView mView;
        public NotificationDetailModel notificationDetailModel;
        List<NotificationDetailModel.NotificationCTA> ctaList;
        public UserNotificationDetailPresenter(UserNotificationDetailContract.IView view)
        {
            mView = view;
        }

        public void EvaluateDetail(Models.NotificationDetails notificationDetails)
        {
            try
            {
                NotificationDetailModel.NotificationCTA primaryCTA;
                NotificationDetailModel.NotificationCTA secondaryCTA;
                int imageResourceBanner = 0;
                ctaList = new List<NotificationDetailModel.NotificationCTA>();

                if (notificationDetails.BCRMNotificationTypeId == "99")
                {
                    primaryCTA = new NotificationDetailModel.NotificationCTA("View Bill", delegate () { mView.ViewBill(); });
                    ctaList.Add(primaryCTA);

                    secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { mView.PayNow(); });
                    ctaList.Add(secondaryCTA);

                    imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                }

                notificationDetailModel = new NotificationDetailModel(imageResourceBanner, notificationDetails.Title,
                    notificationDetails.Message, ctaList);
            }
            catch (Exception e)
            {

            }
        }

        public NotificationDetailModel GetNotificationDetailModel()
        {
            return notificationDetailModel;
        }
    }
}
