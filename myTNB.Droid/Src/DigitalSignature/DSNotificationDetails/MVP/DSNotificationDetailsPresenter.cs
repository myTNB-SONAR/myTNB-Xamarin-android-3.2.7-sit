
using System;
using Android.Content;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using System.Collections.Generic;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.SSMRTerminate.Api;
using Refit;

namespace myTNB_Android.Src.DigitalSignature.DSNotificationDetails.MVP
{
    public class DSNotificationDetailsPresenter
    {
        DSNotificationDetailsContract.IView view;
        private ISharedPreferences mSharedPref;
        SSMRTerminateImpl terminationApi;
        public NotificationDetailModel notificationDetailModel;
        List<NotificationDetailModel.NotificationCTA> ctaList;

        public DSNotificationDetailsPresenter(DSNotificationDetailsContract.IView view, ISharedPreferences mSharedPref)
        {
            this.view = view;
            this.mSharedPref = mSharedPref;
            terminationApi = new SSMRTerminateImpl();
        }

        public void OnInitialize()
        {
            this.view?.SetUpViews();
        }

        public async void DeleteNotificationDetail(NotificationDetails.Models.NotificationDetails notificationDetails)
        {
            this.view.ShowLoadingScreen();
            try
            {
                List<Notifications.Models.UserNotificationData> selectedNotificationList = new List<Notifications.Models.UserNotificationData>();
                Notifications.Models.UserNotificationData data = new Notifications.Models.UserNotificationData();
                data.Id = notificationDetails.Id;
                data.NotificationType = notificationDetails.NotificationType;
                selectedNotificationList.Add(data);
                UserNotificationDeleteResponse notificationDeleteResponse = await ServiceApiImpl.Instance.DeleteUserNotification(new UserNotificationDeleteRequest(selectedNotificationList));
                if (notificationDeleteResponse.IsSuccessResponse())
                {
                    UserNotificationEntity.UpdateIsDeleted(notificationDetails.Id, true);
                    this.view.ShowNotificationListAsDeleted();
                }
                else
                {
                    this.view.ShowRetryOptionsCancelledException(null);
                }

            }
            catch (System.OperationCanceledException e)
            {
                // ADD OPERATION CANCELLED HERE
                this.view.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.view.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.view.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void EvaluateDetail(NotificationDetails.Models.NotificationDetails notificationDetails)
        {
            try
            {
                NotificationDetailModel.NotificationCTA primaryCTA;
                int imageResourceBanner = Resource.Drawable.Banner_DS_Notification_Detail;
                string pageTitle = Utility.GetLocalizedLabel(LanguageConstants.PUSH_NOTIF_DETAILS, LanguageConstants.PushNotificationDetails.NOTIF_TITLE_DEFAULT);
                string notificationDetailTitle = notificationDetails.Title;
                string notificationDetailMessage = notificationDetails.Message;
                bool isDynamicView = true;
                ctaList = new List<NotificationDetailModel.NotificationCTA>();

                if (notificationDetails.HeaderTitle.IsValid())
                {
                    pageTitle = notificationDetails.HeaderTitle;
                }

                switch (notificationDetails.BCRMNotificationTypeId)
                {
                    case Constants.BCRM_NOTIFICATION_EKYC_FIRST_NOTIFICATION:
                    case Constants.BCRM_NOTIFICATION_EKYC_SECOND_NOTIFICATION:
                        imageResourceBanner = Resource.Drawable.Banner_DS_Notification_Detail;
                        primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel(LanguageConstants.PUSH_NOTIF_DETAILS, LanguageConstants.PushNotificationDetails.VERIFY_NOW),
                                delegate () { VerifyNowOnTap(); });
                        ctaList.Add(primaryCTA);
                        isDynamicView = false;
                        break;
                    case Constants.BCRM_NOTIFICATION_EKYC_ID_NOT_MATCHING:
                    case Constants.BCRM_NOTIFICATION_EKYC_FAILED:
                    case Constants.BCRM_NOTIFICATION_EKYC_THIRD_PARTY_FAILED:
                    case Constants.BCRM_NOTIFICATION_EKYC_THIRD_PARTY_ID_NO_TMATCHING:
                        imageResourceBanner = Resource.Drawable.Banner_Notification_EKYC_Failed;
                        primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel(LanguageConstants.PUSH_NOTIF_DETAILS, LanguageConstants.PushNotificationDetails.VERIFY_NOW),
                                delegate () { VerifyNowOnTap(); });
                        ctaList.Add(primaryCTA);
                        break;
                    case Constants.BCRM_NOTIFICATION_EKYC_THREE_TIMES_FAILURE:
                        imageResourceBanner = Resource.Drawable.Banner_Notification_EKYC_Failed;
                        primaryCTA = new NotificationDetailModel.NotificationCTA(Utility.GetLocalizedLabel(LanguageConstants.PUSH_NOTIF_DETAILS, LanguageConstants.PushNotificationDetails.SET_APPOINTMENT_NOW),
                                delegate () { SetAppointmentNowOnTap(); });
                        ctaList.Add(primaryCTA);
                        break;
                    case Constants.BCRM_NOTIFICATION_EKYC_THIRD_PARTY_THREE_TIMES_FAILURE:
                        imageResourceBanner = Resource.Drawable.Banner_Notification_EKYC_Failed;
                        break;
                    case Constants.BCRM_NOTIFICATION_EKYC_SUCCESSFUL:
                    case Constants.BCRM_NOTIFICATION_EKYC_THIRD_PARTY_SUCCESSFUL:
                        imageResourceBanner = Resource.Drawable.Banner_Notification_EKYC_Success;
                        break;
                    default:
                        break;
                }

                notificationDetailModel = new NotificationDetailModel(imageResourceBanner, pageTitle, notificationDetailTitle,
                    notificationDetailMessage, ctaList);

                if (isDynamicView)
                {
                    view.SetUpDynamicView();
                }
                else
                {
                    view.SetUpVerifyNowView();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public NotificationDetailModel GetNotificationDetailModel()
        {
            return notificationDetailModel;
        }

        private void VerifyNowOnTap()
        {
            view.NavigateToIdentityVerification();
        }

        private void SetAppointmentNowOnTap()
        {
            var appointmentURL = DigitalSignatureConstants.EKYC_SET_APPOINTMENT_URL;
            var url = Utility.GetLocalizedLabel(LanguageConstants.PUSH_NOTIF_DETAILS, LanguageConstants.PushNotificationDetails.SET_APPOINTMENT_URL);
            if (url.IsValid())
            {
                appointmentURL = url;
            }
            view.NavigateToExternalBrowser(appointmentURL);
        }
    }
}
