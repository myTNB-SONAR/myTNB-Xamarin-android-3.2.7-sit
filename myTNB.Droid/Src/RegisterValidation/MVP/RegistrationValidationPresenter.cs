using Android.Content;
using Android.Content.PM;
using Android.Text;
using Android.Util;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.RegistrationForm.Models;
using myTNB_Android.Src.RegistrationForm.Requests;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.RegisterValidation.MVP
{
    public class RegistrationValidationPresenter : RegistrationValidationContract.IUserActionsListener
    {
        private RegistrationValidationContract.IView mView;
        private UserCredentialsEntity userCredentialsEntity;
        private readonly string TAG = typeof(RegistrationValidationPresenter).Name;
        private ISharedPreferences mSharedPref;

        public RegistrationValidationPresenter(RegistrationValidationContract.IView mView, UserCredentialsEntity userCredentialsEntity, ISharedPreferences sharedPreferences)
        {
            this.mView = mView;
            this.userCredentialsEntity = userCredentialsEntity;
            this.mView.SetPresenter(this);
            this.mSharedPref = sharedPreferences;
        }

        public void OnComplete()
        {
            this.mView.EnableResendButton();
        }

        public void OnNavigateToAccountListActivity()
        {
            //this.mView.ShowAccountListActivity();
            this.mView.ShowEmailRegisterPopUp();
;
        }

        public async void OnRegister(string num1, string num2, string num3, string num4, string deviceId)
        {
            if (TextUtils.IsEmpty(num1))
            {
                this.mView.ShowEmptyErrorPin_1();
                return;
            }

            if (TextUtils.IsEmpty(num2))
            {
                this.mView.ShowEmptyErrorPin_2();
                return;
            }

            if (TextUtils.IsEmpty(num3))
            {
                this.mView.ShowEmptyErrorPin_3();
                return;
            }

            if (TextUtils.IsEmpty(num4))
            {
                //this.mView.ShowEmptyErrorPin_4();
                return;
            }
            if (mView.IsActive())
            {
                this.mView.ShowRegistrationProgress();
            }
            this.mView.DisableResendButton();
            this.mView.ClearErrors();

            try
            {
                CreateNewUserWithTokenRequest createNewUserWithTokenRequest = new CreateNewUserWithTokenRequest(userCredentialsEntity.Fullname, string.Format("{0}{1}{2}{3}", num1, num2, num3, num4),
                    userCredentialsEntity.Password, userCredentialsEntity.ICNo, userCredentialsEntity.IdType, userCredentialsEntity.MobileNo);
                createNewUserWithTokenRequest.SetUserName(userCredentialsEntity.Email);
                var userRegistrationResponse = await ServiceApiImpl.Instance.CreateNewUserWithToken_OT(createNewUserWithTokenRequest);

                if (userRegistrationResponse.IsSuccessResponse())
                {

                    string fcmToken = String.Empty;

                    if (FirebaseTokenEntity.HasLatest())
                    {
                        fcmToken = FirebaseTokenEntity.GetLatest().FBToken;
                    }
                    UserAuthenticateRequest userAuthenticateRequest = new UserAuthenticateRequest(DeviceIdUtils.GetAppVersionName(), userCredentialsEntity.Password);
                    userAuthenticateRequest.SetUserName(userCredentialsEntity.Email);
                    var userResponse = await ServiceApiImpl.Instance.UserAuthenticateLogin(userAuthenticateRequest);

                    if (!userResponse.IsSuccessResponse())
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideRegistrationProgress();
                        }
                        this.mView.ShowError(userResponse.Response.DisplayMessage);
                    }
                    else
                    {

                        List<NotificationTypesEntity> notificationTypes = NotificationTypesEntity.List();
                        NotificationFilterEntity.InsertOrReplace(Constants.ZERO_INDEX_FILTER, Utility.GetLocalizedCommonLabel("allNotifications"), true);
                        foreach (NotificationTypesEntity notificationType in notificationTypes)
                        {
                            if (notificationType.ShowInFilterList)
                            {
                                NotificationFilterEntity.InsertOrReplace(notificationType.Id, notificationType.Title, false);
                            }
                        }

                        int Id = UserEntity.InsertOrReplace(userResponse.GetData());
                        if (Id > 0)
                        {
                            BaseRequest getUserNotificationRequest = new BaseRequest();
                            getUserNotificationRequest.usrInf.eid = userCredentialsEntity.Email;
                            UserNotificationResponse response = await ServiceApiImpl.Instance.GetUserNotifications(getUserNotificationRequest);
                            if (response.IsSuccessResponse())
                            {
                                try
                                {
                                    UserNotificationEntity.RemoveAll();
                                }
                                catch (System.Exception ne)
                                {
                                    Utility.LoggingNonFatalError(ne);
                                }

                                if (response.GetData() != null && response.GetData().UserNotificationList != null &&
                                    response.GetData().UserNotificationList.Count > 0)
                                {
                                    foreach (UserNotification userNotification in response.GetData().UserNotificationList)
                                    {
                                        // tODO : SAVE ALL NOTIFICATIONs
                                        int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                                    }
                                }
                            }

                            if (mView.IsActive())
                            {
                                this.mView.HideRegistrationProgress();
                            }

                            try
                            {
                                RewardsParentEntity mRewardParentEntity = new RewardsParentEntity();
                                mRewardParentEntity.DeleteTable();
                                mRewardParentEntity.CreateTable();
                                RewardsCategoryEntity mRewardCategoryEntity = new RewardsCategoryEntity();
                                mRewardCategoryEntity.DeleteTable();
                                mRewardCategoryEntity.CreateTable();
                                RewardsEntity mRewardEntity = new RewardsEntity();
                                mRewardEntity.DeleteTable();
                                mRewardEntity.CreateTable();
                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }

                            try
                            {
                                WhatsNewParentEntity mWhatsNewParentEntity = new WhatsNewParentEntity();
                                mWhatsNewParentEntity.DeleteTable();
                                mWhatsNewParentEntity.CreateTable();
                                WhatsNewCategoryEntity mWhatsNewCategoryEntity = new WhatsNewCategoryEntity();
                                mWhatsNewCategoryEntity.DeleteTable();
                                mWhatsNewCategoryEntity.CreateTable();
                                WhatsNewEntity mWhatsNewEntity = new WhatsNewEntity();
                                mWhatsNewEntity.DeleteTable();
                                mWhatsNewEntity.CreateTable();
                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }

                            this.mView.ShowNotificationCount(UserNotificationEntity.Count());
                            MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                            HomeMenuUtils.ResetAll();
                            //this.mView.ShowAccountListActivity();
                            this.mView.ShowEmailRegisterPopUp();
                            UserSessions.SavePhoneVerified(mSharedPref, true);
                        }
                        else
                        {
                            if (mView.IsActive())
                            {
                                this.mView.HideRegistrationProgress();
                            }
                        }
                    }

                }
                else
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideRegistrationProgress();
                    }
                    // TODO : ADD REGISTRATION ERROR
                    string message = userRegistrationResponse.Response.DisplayMessage;
                    this.mView.ShowError(message);
                    this.mView.ShowEmptyErrorPin();
                }
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                if (mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                }
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                }
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                if (mView.IsActive())
                {
                    this.mView.HideRegistrationProgress();
                }
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }


        }

        public void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {

            // SILENTLY DIE , SMS RECEIVE IS ONLY OPTIONAL
            if (requestCode == Constants.RUNTIME_PERMISSION_SMS_REQUEST_CODE)
            {
                if (Utility.IsPermissionHasCount(grantResults))
                {
                    if (grantResults[0] == Permission.Denied)
                    {
                        //if (this.mView.ShouldShowSMSReceiveRationale())
                        //{
                        //    this.mView.ShowSMSPermissionRationale();
                        //}
                    }
                }
            }
        }

        public async void ResendAsync()
        {
            if (this.mView.IsStillCountingDown())
            {
                this.mView.ShowSnackbarError(Resource.String.registration_validation_please_wait);
                return;
            }
            this.mView.DisableResendButton();
            this.mView.StartProgress();

            try
            {
                SendRegistrationTokenSMSRequest sendRegistrationTokenSMSRequest = new SendRegistrationTokenSMSRequest(userCredentialsEntity.MobileNo);
                sendRegistrationTokenSMSRequest.SetUserName(userCredentialsEntity.Email);
                var verificationResponse = await ServiceApiImpl.Instance.SendRegistrationTokenSMS(sendRegistrationTokenSMSRequest);
                if (!verificationResponse.IsSuccessResponse())
                {
                    this.mView.ShowError(verificationResponse.Response.DisplayMessage);
                }

            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }


        }

        public void Start()
        {

            //bool isGranted = this.mView.IsGrantedSMSReceivePermission();
            //if (!isGranted)
            //{
            //    if (this.mView.ShouldShowSMSReceiveRationale())
            //    {
            //        this.mView.ShowSMSPermissionRationale();
            //    }
            //    else
            //    {
            //        this.mView.RequestSMSPermission();
            //    }
            //}

            this.mView.DisableResendButton();
            this.mView.StartProgress();
        }

    }
}
