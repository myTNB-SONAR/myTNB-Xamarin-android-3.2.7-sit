using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.MyTNBService.InterfaceAPI;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Utils;
using Org.Json;
using Refit;

namespace myTNB_Android.Src.MyTNBService.ServiceImpl
{
    public class ServiceApiImpl
    {
        private static readonly Lazy<ServiceApiImpl>
            lazy = new Lazy<ServiceApiImpl>(() => new ServiceApiImpl());
        private IServiceV6 api;
        HttpClient httpClient;

        private ServiceApiImpl()
        {
#if DEBUG || DEVELOP || SIT
            httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            api = RestService.For<IServiceV6>(httpClient);
#else
            api = RestService.For<IServiceV6>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public static ServiceApiImpl Instance { get { return lazy.Value; } }

        /// <summary>
        /// Call GetAppLaunchMasterData with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AppLaunchMasterDataResponse> GetAppLaunchMasterData([Body] Request.BaseRequest request)
        {
            return api.GetAppLaunchMasterData<AppLaunchMasterDataResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }
        /// <summary>
        /// Call GetAppLaunchMasterData with timeout set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<AppLaunchMasterDataResponse> GetAppLaunchMasterData([Body] Request.BaseRequest request, CancellationToken token)
        {
            return api.GetAppLaunchMasterData<AppLaunchMasterDataResponse>(request, token);
        }

        /// <summary>
        /// Call GetAccounts with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<CustomerAccountListResponse> GetCustomerAccountList([Body] Request.BaseRequest request)
        {
            return api.GetCustomerAccountList<CustomerAccountListResponse>(request,CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAccounts with timeout set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<CustomerAccountListResponse> GetCustomerAccountList([Body] Request.BaseRequest request, CancellationToken token)
        {
            return api.GetCustomerAccountList<CustomerAccountListResponse>(request, token);
        }

        /// <summary>
        /// Call AddAccountToCustomer with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AccountToCustomerResponse> AddAccountToCustomer([Body] Request.BaseRequest request)
        {
            return api.AddAccountToCustomer<AccountToCustomerResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call ValidateManualAccount with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ValidateManualAccountResponse> ValidateManualAccount([Body] Request.BaseRequest request)
		{
			return api.ValidateManualAccount<ValidateManualAccountResponse>(request, CancellationTokenSourceWrapper.GetToken());
		}

        /// <summary>
        /// Call ValidateAccisExist with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetSearchForAccountResponse> ValidateAccIsExist([Body] Request.BaseRequest request)
        {

            return api.GetSearchForAccount<GetSearchForAccountResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetPhoneVerifyStatus with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<PhoneVerifyStatusResponse> PhoneVerifyStatus([Body] Request.BaseRequest request)
        {
            return api.PhoneVerifyStatus<PhoneVerifyStatusResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAppNotificationChannels with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AppNotificationChannelsResponse> AppNotificationChannels([Body] Request.BaseRequest request)
        {
            return api.AppNotificationChannels<AppNotificationChannelsResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAppNotificationTypes with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AppNotificationTypesResponse> AppNotificationTypes([Body] Request.BaseRequest request)
        {
            return api.AppNotificationTypes<AppNotificationTypesResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetCustomerAccountsForICNum with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<CustomerAccountsForICNumResponse> CustomerAccountsForICNum([Body] Request.BaseRequest request)
        {
            return api.CustomerAccountsForICNum<CustomerAccountsForICNumResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SubmitFeedback with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmitFeedbackResponse> SubmitFeedback([Body] Request.BaseRequest request)
        {
            return api.SubmitFeedback<SubmitFeedbackResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call submit enquiry with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmitFeedbackResponse> SubmitEnquiry([Body] Request.BaseRequest request)
        {
            return api.SubmitEnquiry<SubmitFeedbackResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }


        /// <summary>
        /// Call GetSubmittedFeedbackList with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmittedFeedbackListResponse> SubmittedFeedbackList([Body] Request.BaseRequest request)
        {
            return api.SubmittedFeedbackList<SubmittedFeedbackListResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SubmittedFeedbackClaimIdDetail with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetOvervoltageClaimDetailModel> OvervoltageClaimDetail([Body] Request.BaseRequest request)
        {
            return api.OvervoltageClaimDetail<GetOvervoltageClaimDetailModel>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// CA number Verify.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<TriggerOVISServicesResponseModel> TriggerOVISServices([Body] Request.BaseRequest request)
        {
            return api.TriggerOVISServices<TriggerOVISServicesResponseModel>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetLocationsByKeyword with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<LocationsByKeywordResponse> LocationsByKeyword([Body] Request.BaseRequest request)
        {
            return api.LocationsByKeyword<LocationsByKeywordResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call IsUserAuth with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserAuthenticateResponse> UserAuthenticate([Body] Request.BaseRequest request)
        {
            return api.UserAuthenticate<UserAuthenticateResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SaveUserNotificationTypePreference with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SaveUserNotificationTypePreferenceResponse> SaveUserNotificationTypePreference([Body] Request.BaseRequest request)
        {
            return api.SaveUserNotificationTypePreference<SaveUserNotificationTypePreferenceResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SaveUserNotificationChannelPreference with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SaveUserNotificationChannelPreferenceResponse> SaveUserNotificationChannelPreference([Body] Request.BaseRequest request)
        {
            return api.SaveUserNotificationChannelPreference<SaveUserNotificationChannelPreferenceResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetUserNotificationTypePreferences with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationTypePreferencesResponse> UserNotificationTypePreferences([Body] Request.BaseRequest request)
        {
            return api.UserNotificationTypePreferences<UserNotificationTypePreferencesResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetUserNotificationChannelPreferences with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationChannelPreferencesResponse> UserNotificationChannelPreferences([Body] Request.BaseRequest request)
        {
            return api.UserNotificationChannelPreferences<UserNotificationChannelPreferencesResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SendRegistrationTokenSMS with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SendRegistrationTokenSMSResponse> SendRegistrationTokenSMS([Body] Request.BaseRequest request)
        {
            return api.SendRegistrationTokenSMS<SendRegistrationTokenSMSResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetRegisteredCards with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<RegisteredCardsResponse> GetRegisteredCards([Body] Request.BaseRequest request)
        {
            return api.GetRegisteredCards<RegisteredCardsResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetSubmittedFeedbackDetails with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmittedFeedbackDetailsResponse> SubmittedFeedbackDetails([Body] Request.BaseRequest request)
        {
            return api.SubmittedFeedbackDetails<SubmittedFeedbackDetailsResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

          /// <summary>
        /// Call GetSubmittedFeedbackWithCotactDetails with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmittedFeedbackDetailsResponse> SubmittedFeedbackWithContactDetails([Body] Request.BaseRequest request)
        {
            return api.SubmittedFeedbackContactDetails<SubmittedFeedbackDetailsResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call CreateNewUserWithToken with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<CreateNewUserWithTokenResponse> CreateNewUserWithToken([Body] Request.BaseRequest request)
        {
            return api.CreateNewUserWithToken<CreateNewUserWithTokenResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SendResetPasswordCode with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SendResetPasswordCodeResponse> SendResetPasswordCode([Body] Request.BaseRequest request)
        {
            return api.SendResetPasswordCode<SendResetPasswordCodeResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call ResetPasswordWithToken with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ResetPasswordWithTokenResponse> ResetPasswordWithToken([Body] Request.BaseRequest request)
        {
            return api.ResetPasswordWithToken<ResetPasswordWithTokenResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call LogoutUser with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<LogoutUserResponse> LogoutUser([Body] Request.BaseRequest request)
        {
            return api.LogoutUser<LogoutUserResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call RemoveRegisteredCard with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<RemoveRegisteredCardResponse> RemoveRegisteredCard([Body] Request.BaseRequest request)
        {
            return api.RemoveRegisteredCard<RemoveRegisteredCardResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call ChangeNewPassword with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ChangeNewPasswordResponse> ChangeNewPassword([Body] Request.BaseRequest request)
        {
            return api.ChangeNewPassword<ChangeNewPasswordResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SendUpdatePhoneTokenSMS with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SendUpdatePhoneTokenSMSResponse> SendUpdatePhoneTokenSMSV2([Body] Request.BaseRequest request)
        {
            return api.SendUpdatePhoneTokenSMSV2<SendUpdatePhoneTokenSMSResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call UpdatePhoneNumber with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UpdateNewPhoneNumberResponse> UpdatePhoneNumber([Body] Request.BaseRequest request)
        {
            return api.UpdatePhoneNumber<UpdateNewPhoneNumberResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SubmitRateUs with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmitRateUsResponse> SubmitRateUs([Body] Request.BaseRequest request)
        {
            return api.SubmitRateUs<SubmitRateUsResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call UpdateLinkedAccountNickName with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UpdateLinkedAccountNameResponse> UpdateLinkedAccountNickName([Body] Request.BaseRequest request)
        {
            return api.UpdateLinkedAccountNickName<UpdateLinkedAccountNameResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call UpdateLinkedAccountNickName with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetRateUsQuestionResponse> GetRateUsQuestions([Body] Request.BaseRequest request)
        {
            return api.GetRateUsQuestions<GetRateUsQuestionResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetLanguagePreference with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetLanguagePreferenceResponse> GetLanguagePreference([Body] Request.BaseRequest request)
        {
            return api.GetLanguagePreference<GetLanguagePreferenceResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SaveLanguagePreference with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SaveLanguagePreferenceResponse> SaveLanguagePreference([Body] Request.BaseRequest request)
        {
            return api.SaveLanguagePreference<SaveLanguagePreferenceResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetBillHistory with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetBillHistoryResponse> GetBillHistory([Body] Request.BaseRequest request)
        {
            return api.GetBillHistory<GetBillHistoryResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call RemoveAccount with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<RemoveAccountResponse> RemoveAccount([Body] Request.BaseRequest request)
        {
            return api.RemoveAccount<RemoveAccountResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetLocations with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetLocationListResponse> GetLocations([Body] Request.BaseRequest request)
        {
            return api.GetLocations<GetLocationListResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetLocationsByKeyword with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetLocationListByKeywordResponse> GetLocationsByKeyword([Body] Request.BaseRequest request)
        {
            return api.GetLocationsByKeyword<GetLocationListByKeywordResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetPaymentReceipt with timeout set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetPaymentReceiptResponse> GetPaymentReceipt([Body] Request.BaseRequest request, CancellationToken token)
        {
            return api.GetPaymentReceipt<GetPaymentReceiptResponse>(request, token);
        }

        /// <summary>
        /// Call GetUserNotifications with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationResponse> GetUserNotifications([Body] Request.BaseRequest request)
        {
            return api.GetUserNotifications<UserNotificationResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetNotificationDetails with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationDetailsResponse> GetNotificationDetails([Body] Request.BaseRequest request)
        {
            return api.GetNotificationDetails<UserNotificationDetailsResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call DeleteUserNotification with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationDeleteResponse> DeleteUserNotification([Body] Request.BaseRequest request)
        {
            return api.DeleteUserNotification<UserNotificationDeleteResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call ReadUserNotification with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationReadResponse> ReadUserNotification([Body] Request.BaseRequest request)
        {
            return api.ReadUserNotification<UserNotificationReadResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call AddAccounts with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AddAccountsResponse> AddMultipleAccounts([Body] Request.BaseRequest request)
        {
            return api.AddMultipleAccounts<AddAccountsResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAccountsCharges with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AccountChargesResponse> GetAccountsCharges([Body] Request.BaseRequest request)
        {
            return api.GetAccountsCharges<AccountChargesResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAccountBillPayHistory with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AccountBillPayHistoryResponse> GetAccountBillPayHistory([Body] Request.BaseRequest request)
        {
            return api.GetAccountBillPayHistory<AccountBillPayHistoryResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetPaymentTransactionId with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<PaymentTransactionIdResponse> GetPaymentTransactionId([Body] Request.BaseRequest request)
        {
            return api.GetPaymentTransactionId<PaymentTransactionIdResponse>(request, CancellationTokenSourceWrapper.GetToken());
        }
    }
}
