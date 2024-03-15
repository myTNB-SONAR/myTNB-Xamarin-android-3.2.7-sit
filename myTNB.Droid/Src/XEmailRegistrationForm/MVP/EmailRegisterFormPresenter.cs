using Android.Content.PM;
using Android.Telephony;
using Android.Text;
using Android.Util;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.XEmailRegistrationForm.Models;
using myTNB.AndroidApp.Src.XEmailRegistrationForm.Requests;
using myTNB.AndroidApp.Src.Utils;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using myTNB.AndroidApp.Src.Database.Model;
using Firebase.Iid;

namespace myTNB.AndroidApp.Src.XEmailRegistrationForm.MVP
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

        public bool validateEmailAndPassword(string email, string password)
        {
            try
            {
                bool isCorrect = true;

                this.mView.DisableRegisterButton();

                if (!string.IsNullOrEmpty(email))
                {
                                      
                    if (!Patterns.EmailAddress.Matcher(email).Matches())
                    {
                        this.mView.ShowInvalidEmailError();
                        isCorrect = false;
                    }
                    else
                    {
                        this.mView.ClearInvalidEmailHint();
                    }
                }
                else
                {   //disable button if no text
                    //this.mView.ClearInvalidEmailHint();
                    this.mView.ShowEmptyEmailErrorNew();
                    isCorrect = false;
                }

                if (!string.IsNullOrEmpty(password))
                {
                    if (!CheckPasswordIsValid(password))
                    {
                        //if password not valid
                        this.mView.ShowPasswordMinimumOf6CharactersError();
                        isCorrect = false;
                    }
                    else
                    {
                        this.mView.ClearInvalidPasswordHint();
                    }
                }
                else
                {
                    //disable button if no text
                    //this.mView.ClearInvalidPasswordHint();
                    this.mView.ShowEmptyPasswordErrorNew();
                    isCorrect = false;
                }

                //handle button to enable or disable
                if (isCorrect == true)
                {
                    this.mView.EnableRegisterButton();
                    return true;
                }
                else
                {
                    this.mView.DisableRegisterButton();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
                return false;
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
                var userResponse = await ServiceApiImpl.Instance.UserAuthenticateEmailOnlyNew(getEmailVerify);
                
                    try
                    {

                        if (userResponse.IsSuccessResponse())
                        {
                            if (userResponse.Response.Data.isActive)
                            {
                                this.mView.ShowInvalidEmailPasswordError();
                                this.mView.DisableRegisterButton();
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
