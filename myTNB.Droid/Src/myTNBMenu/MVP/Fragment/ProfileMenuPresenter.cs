using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Models;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;
using myTNB;
using myTNB.Mobile;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class ProfileMenuPresenter : ProfileMenuContract.IUserActionsListener
    {
        private ProfileMenuContract.IView mView;
        private readonly string TAG = typeof(ProfileMenuPresenter).Name;
        private List<CreditCardData> cardList = new List<CreditCardData>();
        CancellationTokenSource cts;

        public ProfileMenuPresenter(ProfileMenuContract.IView mView)
        {
            this.mView = mView;
        }

        public void OnNotification(string deviceId)
        {
            LoadAllUserPrefNotifications(deviceId);
        }

        public void OnManageCards()
        {
            this.mView.ShowManageCards(cardList);
        }

        public void Start()
        {
            LoadAccount();
        }

        private async void LoadAccount()
        {
            if (mView.IsActive())
            {
                this.mView.ShowNotificationsProgressDialog();
            }

            UserEntity userEntity = UserEntity.GetActive();
            try
            {
                var cardsApiResponse = await ServiceApiImpl.Instance.GetRegisteredCards(new RegisteredCardsRequest(true));

                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }

                if (cardsApiResponse.IsSuccessResponse())
                {
                    foreach (CreditCard card in cardsApiResponse.GetData())
                    {
                        cardList.Add(CreditCardData.Copy(card));
                    }

                    this.mView.ShowUserData(userEntity, cardList.Count);

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
                    this.mView.DisableManageCards();
                    this.mView.ShowUserData(userEntity, 0);
                    this.mView.ShowCCErrorSnakebar();
                }

            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.DisableManageCards();
                this.mView.ShowUserData(userEntity, 0);
                this.mView.ShowCCErrorSnakebar();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.DisableManageCards();
                this.mView.ShowUserData(userEntity, 0);
                this.mView.ShowCCErrorSnakebar();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.DisableManageCards();
                this.mView.ShowUserData(userEntity, 0);
                this.mView.ShowCCErrorSnakebar();
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void LoadAllUserPrefNotifications(string deviceId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowNotificationsProgressDialog();
            }
            try
            {
                var notificationTypesApi = await ServiceApiImpl.Instance.UserNotificationTypePreferences(new MyTNBService.Request.BaseRequest());

                if (notificationTypesApi.IsSuccessResponse())
                {
                    var notificationChannelApi = await ServiceApiImpl.Instance.UserNotificationChannelPreferences(new MyTNBService.Request.BaseRequest());

                    if (mView.IsActive())
                    {
                        this.mView.HideNotificationsProgressDialog();
                    }

                    if (notificationChannelApi.IsSuccessResponse())
                    {
                        UserNotificationTypesEntity.RemoveActive();
                        UserNotificationChannelEntity.RemoveActive();

                        foreach (UserNotificationType notificationType in notificationTypesApi.GetData())
                        {
                            int newRecord = UserNotificationTypesEntity.InsertOrReplace(notificationType);
                            Console.WriteLine(string.Format("New Type Created {0}", newRecord));
                        }

                        foreach (UserNotificationChannel notificationChannel in notificationChannelApi.GetData())
                        {
                            int newRecord = UserNotificationChannelEntity.InsertOrReplace(notificationChannel);
                            Console.WriteLine(string.Format("New Channel Created {0}", newRecord));
                        }

                        this.mView.ShowNotifications();
                    }
                    else
                    {
                        // SHOW ERROR
                        this.mView.ShowRetryOptionsApiException(null);
                    }
                }
                else
                {
                    // SHOW ERROR
                    this.mView.ShowRetryOptionsApiException(null);
                }
            }
            catch (System.OperationCanceledException e)
            {
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
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
                    this.mView.HideNotificationsProgressDialog();
                }
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void OnLogout(string deviceId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowNotificationsProgressDialog();
            }

            UserEntity userEntity = UserEntity.GetActive();
            try
            {
                EligibilitySessionCache.Instance.Clear();
                if (userEntity != null)
                {
                    var logoutResponse = await ServiceApiImpl.Instance.LogoutUser(new LogoutUserRequest());

                    if (mView.IsActive())
                    {
                        this.mView.HideNotificationsProgressDialog();
                    }
                    AppInfoManager.Instance.SetUserInfo("0"
                        , string.Empty
                        , string.Empty
                        , UserSessions.GetDeviceId()
                        , DeviceIdUtils.GetAppVersionName()
                        , TextViewUtils.FontInfo
                        , LanguageUtil.GetAppLanguage() == "MS"
                            ? LanguageManager.Language.MS
                            : LanguageManager.Language.EN);

                    UserEntity.RemoveActive();
                    UserRegister.RemoveActive();
                    CustomerBillingAccount.RemoveActive();
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
                    LanguageUtil.SetIsLanguageChanged(false);
                    UserLoginCountEntity.RemoveAll();

                    this.mView.ShowLogout();
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateCardList(CreditCardData creditCard)
        {
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
}