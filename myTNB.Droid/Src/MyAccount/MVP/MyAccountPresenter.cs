using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.LogoutRate.Api;
using myTNB_Android.Src.MakePayment.Api;
using myTNB_Android.Src.MakePayment.Models;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.MyAccount.MVP
{
    internal class MyAccountPresenter : MyAccountContract.IUserActionsListener
    {

        private MyAccountContract.IView mView;
        CancellationTokenSource cts;

        private List<CreditCardData> cardList = new List<CreditCardData>();

        public MyAccountPresenter(MyAccountContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == Constants.UPDATE_MOBILE_NO_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        UserEntity userEntity = UserEntity.GetActive();
                        this.mView.ShowMobileUpdateSuccess(userEntity.MobileNo);
                    }
                }
                else if (requestCode == Constants.UPDATE_PASSWORD_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        this.mView.ShowPasswordUpdateSuccess();
                    }
                }
                else if (requestCode == Constants.MANAGE_CARDS_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {
                        CreditCardData creditCard = JsonConvert.DeserializeObject<CreditCardData>(data.Extras.GetString(Constants.REMOVED_CREDIT_CARD));
                        cardList.Remove(cardList.Single(item => item.Id.Equals(creditCard.Id)));
                        this.mView.ShowRemovedCardSuccess(creditCard, cardList.Count);
                        if (cardList.Count > 0)
                        {
                            this.mView.EnableManageCards();
                        }
                        else
                        {
                            this.mView.DisableManageCards();
                        }
                    }
                }
                else if (requestCode == Constants.MANAGE_SUPPLY_ACCOUNT_REQUEST)
                {
                    if (resultCode == Result.Ok)
                    {

                        this.mView.ClearAccountsAdapter();
                        List<CustomerBillingAccount> customerAccountList = CustomerBillingAccount.List();
                        if (customerAccountList != null && customerAccountList.Count > 0)
                        {
                            this.mView.ShowAccountList(customerAccountList);
                        }
                        else
                        {
                            this.mView.ShowEmptyAccount();
                        }
                        if (data != null && data.HasExtra(Constants.ACCOUNT_REMOVED_FLAG) && data.GetBooleanExtra(Constants.ACCOUNT_REMOVED_FLAG, false))
                        {
                            this.mView.ShowAccountRemovedSuccess();
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }


        }

        public void OnAddAccount()
        {
            this.mView.ShowAddAccount();
        }

        public async void OnLogout(string deviceId)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            cts = new CancellationTokenSource();


            if (mView.IsActive())
            {
                this.mView.ShowLogoutProgressDialog();
            }



#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };

            var logoutApi = RestService.For<ILogoutApi>(httpClient);
#else
            var logoutApi = RestService.For<ILogoutApi>(Constants.SERVER_URL.END_POINT);
#endif
            UserEntity userEntity = UserEntity.GetActive();
            try
            {
                if (userEntity != null)
                {
                    var logoutResponse = await logoutApi.LogoutUserV2(new myTNB_Android.Src.LogoutRate.Request.LogoutRequestV2()
                    {
                        ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                        Email = userEntity.Email,
                        DeviceId = deviceId,
                        AppVersion = DeviceIdUtils.GetAppVersionName(),
                        OsType = Constants.DEVICE_PLATFORM,
                        OsVersion = DeviceIdUtils.GetAndroidVersion()
                    }, cts.Token);

                    if (mView.IsActive())
                    {
                        this.mView.HideLogoutProgressDialog();
                    }

                    if (!logoutResponse.Data.IsError)
                    {
                        UserEntity.RemoveActive();
                        UserRegister.RemoveActive();
                        CustomerBillingAccount.RemoveActive();
                        NotificationFilterEntity.RemoveAll();
                        UserNotificationEntity.RemoveAll();
                        SubmittedFeedbackEntity.Remove();
                        SMUsageHistoryEntity.RemoveAll();
                        UsageHistoryEntity.RemoveAll();
                        PromotionsEntityV2.RemoveAll();
                        PromotionsParentEntityV2.RemoveAll();
                        BillHistoryEntity.RemoveAll();
                        PaymentHistoryEntity.RemoveAll();
                        REPaymentHistoryEntity.RemoveAll();
                        AccountDataEntity.RemoveAll();
                        SummaryDashBoardAccountEntity.RemoveAll();
                        SelectBillsEntity.RemoveAll();
                        this.mView.ShowLogout();
                    }
                    else
                    {
                        this.mView.ShowLogoutErrorMessage(logoutResponse.Data.Message);
                    }
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideLogoutProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideLogoutProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideLogoutProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnManageCards()
        {

            this.mView.ShowManageCards(cardList);
        }

        public async void OnManageSupplyAccount(CustomerBillingAccount customerBillingAccount, int position)
        {
            //ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            //cts = new CancellationTokenSource();
            //this.mView.ShowProgressDialog();
            //#if DEBUG || STUB
            //            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            //            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(httpClient);
            //#else
            //            var detailedAccountApi = RestService.For<IDetailedCustomerAccount>(Constants.SERVER_URL.END_POINT);
            //#endif

            //            try
            //            {
            //                var customerBillingDetails = await detailedAccountApi.GetDetailedAccount(new AddAccount.Requests.AccountDetailsRequest()
            //                {
            //                    apiKeyID = Constants.APP_CONFIG.API_KEY_ID,
            //                    CANum = customerBillingAccount.AccNum
            //                } , cts.Token);


            //                if (!customerBillingDetails.Data.IsError)
            //                {

            //                    this.mView.ShowManageSupplyAccount(AccountData.Copy(customerBillingDetails.Data.AccountData, false) , position);
            //                }
            //                else
            //                {
            //                    // TODO : SHOW ERROR WHEN NO BILLING IS RETURNED
            //                    this.mView.ShowRetryOptionsApiException(null);
            //                }

            //            }
            //            catch (System.OperationCanceledException e)
            //            {
            //                // ADD OPERATION CANCELLED HERE
            //                this.mView.ShowRetryOptionsCancelledException(e);
            //            }
            //            catch (ApiException apiException)
            //            {
            //                // ADD HTTP CONNECTION EXCEPTION HERE
            //                this.mView.ShowRetryOptionsApiException(apiException);
            //            }
            //            catch (Exception e)
            //            {
            //                // ADD UNKNOWN EXCEPTION HERE
            //                this.mView.ShowRetryOptionsUnknownException(e);
            //            }

            this.mView.ShowManageSupplyAccount(AccountData.Copy(customerBillingAccount, false), position);
            //this.mView.HideShowProgressDialog();
        }

        public void OnUpdateMobileNo()
        {

            this.mView.ShowUpdateMobileNo();
        }

        public void OnUpdatePassword()
        {
            this.mView.ShowUpdatePassword();
        }

        public void Start()
        {
            //
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                if (CustomerBillingAccount.Enumerate().Count() > 0)
                {
                    List<CustomerBillingAccount> customerAccountList = CustomerBillingAccount.List();
                    if (customerAccountList != null && customerAccountList.Count > 0)
                    {
                        this.mView.ShowAccountList(customerAccountList);
                    }
                    else
                    {
                        this.mView.ShowEmptyAccount();
                    }
                }
                else
                {
                    this.mView.ShowEmptyAccount();
                }



                LoadCards();

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        private async void LoadCards()
        {
            cts = new CancellationTokenSource();

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            UserEntity userEntity = UserEntity.GetActive();
#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<GetRegisteredCardsApi>(httpClient);
#else
            var api = RestService.For<GetRegisteredCardsApi>(Constants.SERVER_URL.END_POINT);
#endif
            try
            {
                var cardsApiResponse = await api.GetRegisteredCards(new MakePayment.Models.GetRegisteredCardsRequest(
                        Constants.APP_CONFIG.API_KEY_ID,
                        userEntity.Email
                    ));

                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }

                if (!cardsApiResponse.Data.IsError)
                {
                    foreach (CreditCard card in cardsApiResponse.Data.creditCard)
                    {
                        cardList.Add(CreditCardData.Copy(card));
                    }

                    this.mView.ShowUserData(userEntity, cardsApiResponse.Data.creditCard.Count);

                    if (cardList.Count > 0)
                    {
                        this.mView.EnableManageCards();
                    }
                    else
                    {
                        this.mView.DisableManageCards();
                    }
                }
                else
                {
                    this.mView.ShowUserData(userEntity, 0);
                }

            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }
    }
}