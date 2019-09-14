using System;
using System.Collections.Generic;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.Utils;

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

                switch (notificationDetails.BCRMNotificationTypeId)
                {
                    case Constants.BCRM_NOTIFICATION_NEW_BILL_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Details", delegate () { mView.ViewBill(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { mView.PayNow(); });
                            ctaList.Add(secondaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_BILL_DUE_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Details", delegate () { mView.ViewBill(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { mView.PayNow(); });
                            ctaList.Add(secondaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_bill_due_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_DISCONNECT_NOTICE_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Details", delegate () { mView.ViewBill(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { mView.PayNow(); });
                            ctaList.Add(secondaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_disconnect_notice_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_DISCONNECTED_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("Contact TNB", delegate () { mView.ViewBill(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { mView.PayNow(); });
                            ctaList.Add(secondaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_disconnected_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_RECONNECTED_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View My Usage", delegate () { mView.ViewBill(); });
                            ctaList.Add(primaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_reconnected_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_MAINTENANCE_ID:
                        {
                            imageResourceBanner = Resource.Drawable.notification_maintenance_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_METER_READING_OPEN_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Bill", delegate () { mView.ViewBill(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { mView.PayNow(); });
                            ctaList.Add(secondaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_METER_READING_REMIND_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Bill", delegate () { mView.ViewBill(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { mView.PayNow(); });
                            ctaList.Add(secondaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_DISABLED_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Bill", delegate () { mView.ViewBill(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { mView.PayNow(); });
                            ctaList.Add(secondaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_APPLY_SUCCESS_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Bill", delegate () { mView.ViewBill(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { mView.PayNow(); });
                            ctaList.Add(secondaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_APPLY_FAILED_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Bill", delegate () { mView.ViewBill(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { mView.PayNow(); });
                            ctaList.Add(secondaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_DISABLED_SUCCESS_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Bill", delegate () { mView.ViewBill(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { mView.PayNow(); });
                            ctaList.Add(secondaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            break;
                        }
                    case Constants.BCRM_NOTIFICATION_SMR_DISABLED_FAILED_ID:
                        {
                            primaryCTA = new NotificationDetailModel.NotificationCTA("View Bill", delegate () { mView.ViewBill(); });
                            ctaList.Add(primaryCTA);

                            secondaryCTA = new NotificationDetailModel.NotificationCTA("Pay Now", delegate () { mView.PayNow(); });
                            ctaList.Add(secondaryCTA);

                            imageResourceBanner = Resource.Drawable.notification_new_bill_banner;
                            break;
                        }
                    default:
                        imageResourceBanner = Resource.Drawable.notification_generic_banner;
                        break;
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
