using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using myTNB_Android.Src.Base.Response;
using myTNB_Android.Src.Login.Models;
using myTNB_Android.Src.MyTNBService.InterfaceAPI;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Utils;
using Org.Json;
using Refit;

namespace myTNB_Android.Src.MyTNBService.ServiceImpl
{
    public class ServiceApiImpl
    {
        #region Properties/Fields

        private static readonly Lazy<ServiceApiImpl>
            lazy = new Lazy<ServiceApiImpl>(() => new ServiceApiImpl());
        private IServiceV6 api, apiAws;
        HttpClient httpClient, httpClientAws, httpClientAwsIsUserAuth;

        #endregion

        #region Constructor

        private ServiceApiImpl()
        {
#if DEBUG || DEVELOP || SIT
            httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            api = RestService.For<IServiceV6>(httpClient);

            httpClientAws = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT_AWS) };
            apiAws = RestService.For<IServiceV6>(httpClientAws);

            // httpClientAwsIsUserAuth = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT_ISUSERAUTH_AWS) };
            // apiAwsIsUserAuth = RestService.For<IServiceV6>(httpClientAwsIsUserAuth);
            
#else
            api = RestService.For<IServiceV6>(Constants.SERVER_URL.END_POINT);
            apiAws = RestService.For<IServiceV6>(Constants.SERVER_URL.END_POINT_AWS);
#endif
        }

        public static ServiceApiImpl Instance { get { return lazy.Value; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Call GetAppLaunchMasterData with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AppLaunchMasterDataResponse> GetAppLaunchMasterData([Body] Request.BaseRequest request)
        {
            return api.GetAppLaunchMasterData<AppLaunchMasterDataResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAppLaunchMasterData with timeout set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<AppLaunchMasterDataResponse> GetAppLaunchMasterData([Body] Request.BaseRequest request, CancellationToken token)
        {
            return api.GetAppLaunchMasterData<AppLaunchMasterDataResponse>(EncryptRequest(request), token);
        }

        /// <summary>
        /// Call GetAppLaunchMasterDataAWS with timeout set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<AppLaunchMasterDataResponseAWS> GetAppLaunchMasterDataAWS([Body] Request.BaseRequest request)
        {
            //Console.WriteLine("APIWAS call :" + apiAws.GetAppLaunchMasterDataAWS<AppLaunchMasterDataResponseAWS>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken()).ToString());
            return apiAws.GetAppLaunchMasterDataAWS<AppLaunchMasterDataResponseAWS>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAccounts with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<CustomerAccountListResponse> GetCustomerAccountList([Body] Request.BaseRequestV4 request)
        {
            return api.GetCustomerAccountList<CustomerAccountListResponse>(EncryptRequest(request),CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAccounts with timeout set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<CustomerAccountListResponse> GetCustomerAccountList([Body] Request.BaseRequestV4 request, CancellationToken token)
        {
            return api.GetCustomerAccountList<CustomerAccountListResponse>(EncryptRequest(request), token);
        }

        /// <summary>
        /// Call GetAccounts with default timeout.
        /// </summary>
        /// <param name = "request" ></ param >
        /// < returns ></ returns >
        public Task<CustomerAccountListResponseAppLaunch> GetCustomerAccountListAppLaunch([Body] Request.BaseRequestV4 request)
        {
            //Console.WriteLine("APIWAS call :" + apiAws.GetCustomerAccountListAppLaunch<CustomerAccountListResponseAppLaunch>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken()).ToString());
            return apiAws.GetCustomerAccountListAppLaunch<CustomerAccountListResponseAppLaunch>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAccounts with timeout set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<CustomerAccountListResponse> GetCustomerAccountListAppLaunch([Body] Request.BaseRequestV4 request, CancellationToken token)
        {
            return apiAws.GetCustomerAccountListAppLaunch<CustomerAccountListResponse>(EncryptRequest(request), token);
        }

        /// <summary>
        /// Call AddAccountToCustomer with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AccountToCustomerResponse> AddAccountToCustomer([Body] Request.BaseRequest request)
        {
            return api.AddAccountToCustomer<AccountToCustomerResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call ValidateManualAccount with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ValidateManualAccountResponse> ValidateManualAccount([Body] Request.BaseRequest request)
		{
            return api.ValidateManualAccount_OT<ValidateManualAccountResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
		}

        /// <summary>
        /// Call ValidateAccisExist with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetSearchForAccountResponse> ValidateAccIsExist([Body] Request.BaseRequest request)
        {
            return api.GetSearchForAccount<GetSearchForAccountResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetPhoneVerifyStatus with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<PhoneVerifyStatusResponse> PhoneVerifyStatus([Body] Request.BaseRequest request)
        {
            return api.PhoneVerifyStatus<PhoneVerifyStatusResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAppNotificationChannels with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AppNotificationChannelsResponse> AppNotificationChannels([Body] Request.BaseRequest request)
        {
            return api.AppNotificationChannels<AppNotificationChannelsResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAppNotificationTypes with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AppNotificationTypesResponse> AppNotificationTypes([Body] Request.BaseRequest request)
        {
            return api.AppNotificationTypes<AppNotificationTypesResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetCustomerAccountsForICNum with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<CustomerAccountsForICNumResponse> CustomerAccountsForICNum([Body] Request.BaseRequest request)
        {
            return api.CustomerAccountsForICNum<CustomerAccountsForICNumResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SubmitFeedback with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmitFeedbackResponse> SubmitFeedback([Body] Request.BaseRequest request)
        {
            return api.SubmitFeedback<SubmitFeedbackResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call submit enquiry with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmitFeedbackResponse> SubmitEnquiry([Body] Request.BaseRequest request)
        {
            return api.SubmitEnquiry<SubmitFeedbackResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }
        /// <summary>
        /// Call submit enquiry with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmitFeedbackResponse> SubmitEnquiryWithType([Body] Request.BaseRequest request)
        {
            return api.SubmitEnquiryWithType<SubmitFeedbackResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetSubmittedFeedbackList with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmittedFeedbackListResponse> SubmittedFeedbackList([Body] Request.BaseRequest request)
        {
            return api.SubmittedFeedbackList<SubmittedFeedbackListResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SubmittedFeedbackClaimIdDetail with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetOvervoltageClaimDetailModel> OvervoltageClaimDetail([Body] Request.BaseRequest request)
        {
            return api.OvervoltageClaimDetail<GetOvervoltageClaimDetailModel>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// CA number Verify.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<TriggerOVISServicesResponseModel> TriggerOVISServices([Body] Request.BaseRequest request)
        {
            return api.TriggerOVISServices<TriggerOVISServicesResponseModel>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetLocationsByKeyword with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<LocationsByKeywordResponse> LocationsByKeyword([Body] Request.BaseRequest request)
        {
            return api.LocationsByKeyword<LocationsByKeywordResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call IsUserAuth with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserAuthenticateResponse> UserAuthenticate([Body] Request.BaseRequest request)
        {
            return api.UserAuthenticate<UserAuthenticateResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call CancelInvitation_OT with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ManageAccessAccountListNullResponse> CancelInvitation_OT([Body] Request.BaseRequestV4 request)
        {
            return api.CancelInvitation_OT<ManageAccessAccountListNullResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());                     //wan   //api cancel invited user
        }


        /// <summary>
        /// Call SendReInviteEmail with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ManageAccessAccountListNullResponse> SendReInviteEmail([Body] Request.BaseRequestV4 request)
        {
            return api.SendReInviteEmail<ManageAccessAccountListNullResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());                     //irul   //api resend invited user
        }

        /// <summary>
        /// Call GetAccountActivityLogList with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<LogUserAccessResponse> AddUserAcess_OT([Body] Request.BaseRequestV4 request)
        {
            return api.AddUserAcess_OT<LogUserAccessResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());                     //wan   //api add user access
        }

        /// <summary>
        /// Call GetAccountActivityLogList with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<LogUserAccessResponse> RemoveUserAcess_OT([Body] Request.BaseRequestV4 request)
        {
            return api.RemoveUserAcess_OT<LogUserAccessResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());                     //wan   //api remove user access
        }

        /// <summary>
        /// Call GetAccountActivityLogList with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<LogUserAccessResponse> GetAccountActivityLogList([Body] Request.BaseRequestV4 request)
        {
            return api.GetAccountActivityLogList<LogUserAccessResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());                     //2enapps wan   //api log activity user access
        }

        /// <summary>
        /// Call UpdateAccountAccessRight with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ManageAccessAccountListNullResponse> UpdateAccountAccessRight([Body] Request.BaseRequestV4 request)
        {
            return api.UpdateAccountAccessRight<ManageAccessAccountListNullResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());                     //2enapps wan   //api add multiple account new
        }

        /// <summary>
        /// Call AddAccounts with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AddAccountsResponse> AddMultipleAccounts_OT([Body] Request.BaseRequestV4 request)
        {
            return api.AddMultipleAccounts_OT<AddAccountsResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());                     //2enapps wan   //api add multiple account new
        }

        /// <summary>
        /// Call GetAccountAccessRightList with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ManageAccessAccountListResponse> GetAccountAccessRightList([Body] Request.BaseRequest request)
        {
            return api.GetAccountAccessRightList<ManageAccessAccountListResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());     //2enapps wan   //api get manage access list
        }

        /// <summary>
        /// Call GetAccountAccessRight with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ManageAccessAccountListResponse> GetAccountAccessRight([Body] Request.BaseRequest request)
        {
            return api.GetAccountAccessRight<ManageAccessAccountListResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());     //2enapps wan   //api get manage access  
        }

        /// <summary>
        /// Call CreateNewUserWithToken with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<CreateNewUserWithTokenResponse> CreateNewUserWithToken_OT([Body] Request.BaseRequest request)
        {
            return api.CreateNewUserWithToken_OT<CreateNewUserWithTokenResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());     //2enapps wan   //api register baru
        }

        /// <summary>
        /// Call IsUserAuth with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserAuthenticateResponse> UserAuthenticateLogin([Body] Request.BaseRequest request)
        {
            return api.UserAuthenticateLoginNew<UserAuthenticateResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());  //Nurlyana //api login baru
        }

        public Task<UserAuthenticateResponseEmail> UserAuthenticateEmail([Body] Request.BaseRequest request)
        {
            return api.UserAuthenticate<UserAuthenticateResponseEmail>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }


        public Task<SendEmailRegisterCodeResponse> UserAuthenticateEmailOnlyNew([Body] Request.BaseRequest request)
        {
            return api.UserAuthenticateEmail<SendEmailRegisterCodeResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());      //irul
        }


        public Task<SendDetailRegisterCodeResponse> UserAuthenticateIDOnlyNew([Body] Request.BaseRequest request)
        {
            return api.UserAuthenticateID<SendDetailRegisterCodeResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());      //irul
        }


        public Task<UserAuthenticateResponseEmail> UserAuthenticateEmailOnly([Body] Request.BaseRequest request)
        {
            return api.UserAuthenticateEmail<UserAuthenticateResponseEmail>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());      //2enapps wan
        }

        public Task<UserAuthenticateResponseID> UserAuthenticateIDOnly([Body] Request.BaseRequest request)
        {
            return api.UserAuthenticateID<UserAuthenticateResponseID>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());      //2enapps wan
        }

        public Task<UserAuthenticateResponseID> UserAuthenticateUpdateID([Body] Request.BaseRequest request)
        {
            return api.UserUpdateIdentifcationNo<UserAuthenticateResponseID>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());      //2enapps wan
        }

        public Task<UserAuthenticateResponseName> UserAuthenticateUpdateName([Body] Request.BaseRequest request)
        {
            return api.UserUpdateName<UserAuthenticateResponseName>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());      //2enapps wan
        }

        /// <summary>
        /// Call SaveUserNotificationTypePreference with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SaveUserNotificationTypePreferenceResponse> SaveUserNotificationTypePreference([Body] Request.BaseRequest request)
        {
            return api.SaveUserNotificationTypePreference<SaveUserNotificationTypePreferenceResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SaveUserNotificationChannelPreference with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SaveUserNotificationChannelPreferenceResponse> SaveUserNotificationChannelPreference([Body] Request.BaseRequest request)
        {
            return api.SaveUserNotificationChannelPreference<SaveUserNotificationChannelPreferenceResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetUserNotificationTypePreferences with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationTypePreferencesResponse> UserNotificationTypePreferences([Body] Request.BaseRequest request)
        {
            return api.UserNotificationTypePreferences<UserNotificationTypePreferencesResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetUserNotificationChannelPreferences with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationChannelPreferencesResponse> UserNotificationChannelPreferences([Body] Request.BaseRequest request)
        {
            return api.UserNotificationChannelPreferences<UserNotificationChannelPreferencesResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SendRegistrationTokenSMS with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SendRegistrationTokenSMSResponse> SendRegistrationTokenSMS([Body] Request.BaseRequest request)
        {
            return api.SendRegistrationTokenSMS<SendRegistrationTokenSMSResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetRegisteredCards with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<RegisteredCardsResponse> GetRegisteredCards([Body] Request.BaseRequest request)
        {
            return api.GetRegisteredCards<RegisteredCardsResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetSubmittedFeedbackDetails with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmittedFeedbackDetailsResponse> SubmittedFeedbackDetails([Body] Request.BaseRequest request)
        {
            return api.SubmittedFeedbackDetails<SubmittedFeedbackDetailsResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetSubmittedFeedbackWithCotactDetails with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmittedFeedbackDetailsResponse> SubmittedFeedbackWithContactDetails([Body] Request.BaseRequest request)
        {
            return api.SubmittedFeedbackContactDetails<SubmittedFeedbackDetailsResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call CreateNewUserWithToken with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<CreateNewUserWithTokenResponse> CreateNewUserWithToken([Body] Request.BaseRequest request)
        {
            return api.CreateNewUserWithToken<CreateNewUserWithTokenResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SendResetPasswordCode with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SendResetPasswordCodeResponse> SendResetPasswordCode([Body] Request.BaseRequest request)
        {
            return api.SendResetPasswordCode<SendResetPasswordCodeResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call ResetPasswordWithToken with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ResetPasswordWithTokenResponse> ResetPasswordWithToken([Body] Request.BaseRequest request)
        {
            return api.ResetPasswordWithToken<ResetPasswordWithTokenResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call LogoutUser with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<LogoutUserResponse> LogoutUser([Body] Request.BaseRequest request)
        {
            return api.LogoutUser<LogoutUserResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call RemoveRegisteredCard with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<RemoveRegisteredCardResponse> RemoveRegisteredCard([Body] Request.BaseRequest request)
        {
            return api.RemoveRegisteredCard<RemoveRegisteredCardResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call ChangeNewPassword with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ChangeNewPasswordResponse> ChangeNewPassword([Body] Request.BaseRequest request)
        {
            return api.ChangeNewPassword<ChangeNewPasswordResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }


        /// <summary>
        /// Call ChangeNewPassword with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SendResetPasswordCodeResponse> ChangeNewPasswordNew([Body] Request.BaseRequest request)
        {
            return api.SendResetPasswordCode_OT<SendResetPasswordCodeResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken()); //yana
        }

        /// <summary>
        /// Call Resend Email Verification with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SendEmailVerificationResponse> SendEmailVerify([Body] Request.BaseRequest request)
        {
            return api.SendEmailVerification<SendEmailVerificationResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken()); //yana
        }

        /// <summary>
        /// Call SendUpdatePhoneTokenSMS with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SendUpdatePhoneTokenSMSResponse> SendUpdatePhoneTokenSMSV2([Body] Request.BaseRequest request)
        {
            return api.SendUpdatePhoneTokenSMSV2<SendUpdatePhoneTokenSMSResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call UpdatePhoneNumber with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UpdateNewPhoneNumberResponse> UpdatePhoneNumber([Body] Request.BaseRequest request)
        {
            return api.UpdatePhoneNumber<UpdateNewPhoneNumberResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SubmitRateUs with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmitRateUsResponse> SubmitRateUs([Body] Request.BaseRequest request)
        {
            return api.SubmitRateUs<SubmitRateUsResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SubmitRateUs with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmitRateUsResponse> SubmitRateUsV2([Body] Request.BaseRequest request)
        {
            return api.SubmitRateUsV2<SubmitRateUsResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call UpdateLinkedAccountNickName with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UpdateLinkedAccountNameResponse> UpdateLinkedAccountNickName([Body] Request.BaseRequest request)
        {
            return api.UpdateLinkedAccountNickName<UpdateLinkedAccountNameResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call UpdateLinkedAccountNickName with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetRateUsQuestionResponse> GetRateUsQuestions([Body] Request.BaseRequest request)
        {
            return api.GetRateUsQuestions<GetRateUsQuestionResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call ShowEnergyBudgetRatingPage with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetRateUsQuestionResponse> ShowEnergyBudgetRatingPage([Body] Request.BaseRequest request)
        {
            return api.ShowEnergyBudgetRatingPage<GetRateUsQuestionResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call ShowEnergyBudgetRatingPage with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetRateUsQuestionResponse> ExperienceRatingUserLeaveOut([Body] Request.BaseRequest request)
        {
            return api.ExperienceRatingUserLeaveOut<GetRateUsQuestionResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetLanguagePreference with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetLanguagePreferenceResponse> GetLanguagePreference([Body] Request.BaseRequest request)
        {
            return api.GetLanguagePreference<GetLanguagePreferenceResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SaveLanguagePreference with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SaveLanguagePreferenceResponse> SaveLanguagePreference([Body] Request.BaseRequest request)
        {
            return api.SaveLanguagePreference<SaveLanguagePreferenceResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetBillHistory with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetBillHistoryResponse> GetBillHistory([Body] Request.BaseRequest request)
        {
            return api.GetBillHistory<GetBillHistoryResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call RemoveAccount with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<RemoveAccountResponse> RemoveAccount([Body] Request.BaseRequestV4 request)
        {
            return api.RemoveAccount<RemoveAccountResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call SaveEnergyBudget with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SaveEnergyBudgetResponse> SaveEnergyBudget([Body] Request.BaseRequest request)
        {
            return api.SaveEnergyBudget<SaveEnergyBudgetResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetLocations with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetLocationListResponse> GetLocations([Body] Request.BaseRequest request)
        {
            return api.GetLocations<GetLocationListResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetLocationsByKeyword with default timeout.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetLocationListByKeywordResponse> GetLocationsByKeyword([Body] Request.BaseRequest request)
        {
            return api.GetLocationsByKeyword<GetLocationListByKeywordResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetPaymentReceipt with timeout set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetPaymentReceiptResponse> GetPaymentReceipt([Body] Request.BaseRequest request, CancellationToken token)
        {
            return api.GetPaymentReceipt<GetPaymentReceiptResponse>(EncryptRequest(request), token);
        }

        /// <summary>
        /// Call GetUserNotifications with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationResponse> GetUserNotifications([Body] Request.BaseRequest request)
        {
            return api.GetUserNotifications<UserNotificationResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// cep new //yana
        /// <summary>
        /// Call GetUserNotifications with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationResponse> GetUserNotificationsV2([Body] Request.BaseRequest request)
        {
            return api.GetUserNotificationsV2<UserNotificationResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetNotificationDetails with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationDetailsResponse> GetNotificationDetails([Body] Request.BaseRequest request)
        {
            return api.GetNotificationDetails<UserNotificationDetailsResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetNotificationDetails with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationDetailsResponse> GetNotificationDetailsByRequestId([Body] Request.BaseRequest request)
        {
            return api.GetNotificationDetailsByRequestId<UserNotificationDetailsResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken()); //yana
        }

        /// <summary>
        /// Call GetUserServiceDistruptionSub with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserServicedistruptionSubResponse> GetUserServiceDistruptionSub([Body] Request.BaseRequest request)
        {
            return api.GetUserServiceDistruptionSub<UserServicedistruptionSubResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken()); //wan //sd
        }

        /// <summary>
        /// Call ServiceDisruptionInfo with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserServiceDistruptionSetSubResponse> ServiceDisruptionInfo([Body] Request.BaseRequest request)
        {
            return api.ServiceDisruptionInfo<UserServiceDistruptionSetSubResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken()); //wan //sd
        }

        /// <summary>
        /// Call SDSubmitRateUs with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubmitRateUsResponse> SDSubmitRateUs([Body] Request.BaseRequest request)
        {
            return api.SDSubmitRateUs<SubmitRateUsResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken()); //wan //sd
        }

        /// <summary>
        /// Call ShowSDRatingPage with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserServiceDistruptionSetSubResponse> ShowSDRatingPage([Body] Request.BaseRequest request)
        {
            return api.ShowSDRatingPage<UserServiceDistruptionSetSubResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken()); //wan //sd
        }

        /// <summary>
        /// Call DeleteUserNotification with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationDeleteResponse> DeleteUserNotification([Body] Request.BaseRequest request)
        {
            return api.DeleteUserNotification<UserNotificationDeleteResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call ReadUserNotification with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UserNotificationReadResponse> ReadUserNotification([Body] Request.BaseRequest request)
        {
            return api.ReadUserNotification<UserNotificationReadResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call AddAccounts with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AddAccountsResponse> AddMultipleAccounts([Body] Request.BaseRequest request)
        {
            return api.AddMultipleAccounts<AddAccountsResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAccountsCharges with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AccountChargesResponse> GetAccountsCharges([Body] Request.BaseRequest request)
        {
            return api.GetAccountsCharges<AccountChargesResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetAccountBillPayHistory with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<AccountBillPayHistoryResponse> GetAccountBillPayHistory([Body] Request.BaseRequest request)
        {
            return api.GetAccountBillPayHistory<AccountBillPayHistoryResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetPaymentTransactionId with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<PaymentTransactionIdResponse> GetPaymentTransactionId([Body] Request.BaseRequest request)
        {
            return api.GetPaymentTransactionId<PaymentTransactionIdResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetUserStatusActive with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UpdateUserStatusActivateResponse> UpdateUserStatusActivate([Body] Request.UpdateUserStatusActivateRequest request)
        {
            return api.UpdateUserStatusActivate<UpdateUserStatusActivateResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetUserStatusDeactive with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<UpdateUserStatusActivateResponse> UpdateUserStatusDeactivate([Body] Request.UpdateUserStatusActivateRequest request)
        {
            return api.UpdateUserStatusDeactivate<UpdateUserStatusActivateResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetNCAccount with default timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<NCAutoAddAccountsResponse> NCAutoAddAccounts([Body] Request.NCAutoAddAccountsRequest request)
        {
            return api.NCAutoAddAccounts<NCAutoAddAccountsResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call FcmTokenUpdate with timeout set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<APIBaseResponse> UpdateUserInfoDevice([Body] Request.BaseRequest request)
        {
            //Console.WriteLine("APIWAS call :" + apiAws.UpdateUserInfoDevice<APIBaseResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken()).ToString());
            return apiAws.UpdateUserInfoDevice<APIBaseResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        /// <summary>
        /// Call GetIdentificationNo with timeout set.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<GetIdentificationNoResponse> GetIdentificationNo([Body] Request.BaseRequestV4 request)
        {
            return apiAws.GetIdentificationNo<GetIdentificationNoResponse>(EncryptRequest(request), CancellationTokenSourceWrapper.GetToken());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Encrypts request for security Purposes.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Encrypted Request</returns>
        private EncryptedRequest EncryptRequest(object request)
        {
            return myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
        }

        #endregion
    }
}
