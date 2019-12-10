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
using myTNB_Android.Src.MyTNBService.Notification;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.RegistrationForm.Api;
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
        private CancellationTokenSource cts;
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
            this.mView.ShowAccountListActivity();
        }

        public async void OnRegister(string num1, string num2, string num3, string num4, string deviceId)
        {

            cts = new CancellationTokenSource();
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
                this.mView.ShowEmptyErrorPin_4();
                return;
            }
            if (mView.IsActive())
            {
                this.mView.ShowRegistrationProgress();
            }
            this.mView.DisableResendButton();
            this.mView.ClearErrors();

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var registrationApi = RestService.For<IRegisterUser>(httpClient);
#else
            var registrationApi = RestService.For<IRegisterUser>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                var userRegistrationResponse = await registrationApi.RegisterUser(new RegistrationForm.Requests.UserRegistrationRequest(Constants.APP_CONFIG.API_KEY_ID)
                {
                    displayName = userCredentialsEntity.Fullname,
                    username = userCredentialsEntity.Email,
                    email = userCredentialsEntity.Email,
                    token = string.Format("{0}{1}{2}{3}", num1, num2, num3, num4),
                    password = userCredentialsEntity.Password,
                    confirmPassword = userCredentialsEntity.ConfirmEmail,
                    icNo = userCredentialsEntity.ICNo,
                    mobileNo = userCredentialsEntity.MobileNo,
                    ipAddress = Constants.APP_CONFIG.API_KEY_ID,
                    clientType = DeviceIdUtils.GetAppVersionName(),
                    activeUserName = Constants.APP_CONFIG.API_KEY_ID,
                    devicePlatform = Constants.DEVICE_PLATFORM,
                    deviceVersion = DeviceIdUtils.GetAndroidVersion(),
                    deviceCordova = Constants.APP_CONFIG.API_KEY_ID
                }, cts.Token);

                if (!userRegistrationResponse.userRegistration.IsError)
                {

                    string fcmToken = String.Empty;

                    if (FirebaseTokenEntity.HasLatest())
                    {
                        fcmToken = FirebaseTokenEntity.GetLatest().FBToken;
                    }

                    var userResponse = await ServiceApiImpl.Instance.UserAuthenticate(new UserAuthenticateRequest(DeviceIdUtils.GetAppVersionName(), userCredentialsEntity.Password));

                    if (!userResponse.IsSuccessResponse())
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideRegistrationProgress();
                        }
                        this.mView.ShowError(userResponse.Response.Message);
                    }
                    else
                    {

                        List<NotificationTypesEntity> notificationTypes = NotificationTypesEntity.List();
                        NotificationFilterEntity.InsertOrReplace(Constants.ZERO_INDEX_FILTER, Constants.ZERO_INDEX_TITLE, true);
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
                            NotificationApiImpl notificationAPI = new NotificationApiImpl();
                            MyTNBService.Response.UserNotificationResponse response = await notificationAPI.GetUserNotifications<MyTNBService.Response.UserNotificationResponse>(new Base.Request.APIBaseRequest());
                            if (response != null && response.Data != null && response.Data.ErrorCode == "7200")
                            {
                                if (response.Data.ResponseData != null && response.Data.ResponseData.UserNotificationList != null &&
                                    response.Data.ResponseData.UserNotificationList.Count > 0)
                                {
                                    foreach (UserNotification userNotification in response.Data.ResponseData.UserNotificationList)
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

                            this.mView.ShowNotificationCount(UserNotificationEntity.Count());
                            MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                            HomeMenuUtils.ResetAll();
                            this.mView.ShowAccountListActivity();
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
                    string message = userRegistrationResponse.userRegistration.Message;
                    this.mView.ShowError(message);
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

#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IGetVerificationCode>(httpClient);
#else
            var api = RestService.For<IGetVerificationCode>(Constants.SERVER_URL.END_POINT);
#endif


            try
            {
                var verificationResponse = await api.GetVerificationCodeThruSMSV2(new VerificationCodeRequest(Constants.APP_CONFIG.API_KEY_ID)
                {
                    userEmail = userCredentialsEntity.Email,
                    username = userCredentialsEntity.Email,
                    mobileNo = userCredentialsEntity.MobileNo,
                    ipAddress = Constants.APP_CONFIG.API_KEY_ID,
                    clientType = Constants.APP_CONFIG.API_KEY_ID,
                    activeUserName = Constants.APP_CONFIG.API_KEY_ID,
                    devicePlatform = Constants.APP_CONFIG.API_KEY_ID,
                    deviceVersion = Constants.APP_CONFIG.API_KEY_ID,
                    deviceCordova = Constants.APP_CONFIG.API_KEY_ID

                });

                if (verificationResponse.verificationCode.isError)
                {
                    this.mView.ShowError(verificationResponse.verificationCode.message);
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