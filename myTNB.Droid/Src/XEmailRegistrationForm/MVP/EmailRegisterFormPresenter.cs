using Android.Content.PM;
using Android.Telephony;
using Android.Text;
using Android.Util;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.XEmailRegistrationForm.Models;
using myTNB_Android.Src.XEmailRegistrationForm.Requests;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using myTNB_Android.Src.Database.Model;
using Firebase.Iid;

namespace myTNB_Android.Src.XEmailRegistrationForm.MVP
{
    public class EmailRegisterFormPresenter : EmailRegisterFormContract.IUserActionsListener
    {
        private EmailRegisterFormContract.IView mView;
        private readonly string MIN_6_ALPHANUMERIC_PATTERN = "[a-zA-Z0-9#?!@$%^&*-]{6,}";
        private Regex hasNumber = new Regex(@"[0-9]+");
        private Regex hasUpperChar = new Regex(@"[a-zA-Z]+");
        private Regex hasMinimum8Chars = new Regex(@".{8,}");

        CancellationTokenSource registerCts;
        private readonly string TAG = typeof(EmailRegisterFormPresenter).Name;

        public EmailRegisterFormPresenter(EmailRegisterFormContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void CheckRequiredFields(string email, string password)
        {

            try
            {
                if (!TextUtils.IsEmpty(email) && !TextUtils.IsEmpty(password))
                {

                    if (!Patterns.EmailAddress.Matcher(email).Matches())
                    {
                        this.mView.ShowInvalidEmailError();
                        //this.mView.ClearInvalidEmailHint();
                        //this.mView.ShowInvalidEmailPasswordError(string errorMessage);
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else
                    {
                        //this.mView.ClearInvalidEmailError();
                        this.mView.ShowEmailHint();
                    }

                    if (!CheckPasswordIsValid(password))
                    {
                        this.mView.ShowPasswordMinimumOf6CharactersError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else
                    {
                        this.mView.ClearPasswordMinimumOf6CharactersError();
                    }

                    this.mView.EnableRegisterButton();
                }
                else
                {
                    this.mView.ClearInvalidEmailError();
                    this.mView.ClearPasswordMinimumOf6CharactersError();
                    this.mView.ClearInvalidEmailHint();
                    this.mView.DisableRegisterButton();
                    //this.mView.EnableRegisterButton();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void GoBack()
        {
            this.mView.ShowBackScreen();
        }

       
        public async void OnAcquireToken (string email,  string password)
        {
            registerCts = new CancellationTokenSource();
            this.mView.ClearAllErrorFields();
           

            if (TextUtils.IsEmpty(email))
            {
                this.mView.ShowEmptyEmailError();
                return;
            }


            if (!Patterns.EmailAddress.Matcher(email).Matches())
            {
                this.mView.ShowInvalidEmailError();
                return;
            }


            if (TextUtils.IsEmpty(password))
            {
                this.mView.ShowEmptyPasswordError();
                return;
            }

            if (!CheckPasswordIsValid(password))
            {
                this.mView.ShowPasswordMinimumOf6CharactersError();
                return;
            }


            this.mView.ShowProgressDialog();
            this.mView.ClearAllErrorFields();
            Log.Debug(TAG, "Awaiting...");
            try
            {
                string fcmToken = String.Empty;

                if (FirebaseTokenEntity.HasLatest())
                {
                    fcmToken = FirebaseTokenEntity.GetLatest().FBToken;
                }
                else
                {
                    fcmToken = FirebaseInstanceId.Instance.Token;
                    FirebaseTokenEntity.InsertOrReplace(fcmToken, true);
                }

                string idType = "", idNo="";
                GetVerifyRequest getEmailVerify = new GetVerifyRequest(idType,idNo);
                getEmailVerify.SetUserName(email);
                var userResponse = await ServiceApiImpl.Instance.UserAuthenticateEmailOnly(getEmailVerify);
                
                    try
                    {

                        if (userResponse.IsSuccessResponse())
                        {
                            if (userResponse.Response.Data.IsRegistered)
                            {
                                this.mView.ShowInvalidEmailPasswordError();
                            }
                            else
                            {
                                var userCredentials = new UserCredentialsEntity()
                                {
                                    Email = email,
                                    Password = password,

                                };
                                this.mView.ShowRegister(userCredentials);
                                this.mView.HideProgressDialog();
                            }
                        }
                        else
                        {
                            this.mView.HideProgressDialog();
                            this.mView.ShowCCErrorSnakebar();

                        }
                    }
                    catch (ApiException apiException)
                    {
                        // ADD HTTP CONNECTION EXCEPTION HERE
                        this.mView.ShowRetryOptionsApiException(apiException);
                        Utility.LoggingNonFatalError(apiException);
                    }
                    catch (System.Exception e)
                    {
                        // ADD UNKNOWN EXCEPTION HERE
                        Log.Debug(TAG, "Stack " + e.StackTrace);
                        this.mView.ShowRetryOptionsUnknownException(e);
                        Utility.LoggingNonFatalError(e);
                    }
                    finally
                    {
                        this.mView.HideProgressDialog();
                    }
                
            
            }
            catch (System.OperationCanceledException e)

            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                if (this.mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                    this.mView.ShowRetryOptionsCancelledException(e);
                }
                ClearDataCache();
                Utility.LoggingNonFatalError(e);
            }

        }

       

        private void ClearDataCache()
        {
            try
            {
                UserEntity.RemoveActive();
                UserRegister.RemoveActive();
                CustomerBillingAccount.RemoveActive();
                UserManageAccessAccount.RemoveActive();
                NotificationFilterEntity.RemoveAll();
                UserNotificationEntity.RemoveAll();
                SubmittedFeedbackEntity.Remove();
                SMUsageHistoryEntity.RemoveAll();
                UsageHistoryEntity.RemoveAll();
                BillHistoryEntity.RemoveAll();
                PaymentHistoryEntity.RemoveAll();
                REPaymentHistoryEntity.RemoveAll();
                AccountDataEntity.RemoveAll();
                SummaryDashBoardAccountEntity.RemoveAll();
                SelectBillsEntity.RemoveAll();
                NewFAQParentEntity NewFAQParentManager = new NewFAQParentEntity();
                NewFAQParentManager.DeleteTable();
                SSMRMeterReadingScreensParentEntity SSMRMeterReadingScreensParentManager = new SSMRMeterReadingScreensParentEntity();
                SSMRMeterReadingScreensParentManager.DeleteTable();
                SSMRMeterReadingScreensOCROffParentEntity SSMRMeterReadingScreensOCROffParentManager = new SSMRMeterReadingScreensOCROffParentEntity();
                SSMRMeterReadingScreensOCROffParentManager.DeleteTable();
                SSMRMeterReadingThreePhaseScreensParentEntity SSMRMeterReadingThreePhaseScreensParentManager = new SSMRMeterReadingThreePhaseScreensParentEntity();
                SSMRMeterReadingThreePhaseScreensParentManager.DeleteTable();
                SSMRMeterReadingThreePhaseScreensOCROffParentEntity SSMRMeterReadingThreePhaseScreensOCROffParentManager = new SSMRMeterReadingThreePhaseScreensOCROffParentEntity();
                SSMRMeterReadingThreePhaseScreensOCROffParentManager.DeleteTable();
                EnergySavingTipsParentEntity EnergySavingTipsParentManager = new EnergySavingTipsParentEntity();
                EnergySavingTipsParentManager.DeleteTable();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public bool CheckPasswordIsValid(string password)
        {
            bool isValid = false;
            try
            {
                isValid = hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return isValid;
        }

        public void Start()
        {
            try
            {
                this.mView.DisableRegisterButton();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
