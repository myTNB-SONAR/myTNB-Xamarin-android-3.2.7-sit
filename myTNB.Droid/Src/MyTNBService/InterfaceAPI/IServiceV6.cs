using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.MyTNBService.Request;
using Refit;

namespace myTNB_Android.Src.MyTNBService.InterfaceAPI
{
    public interface IServiceV6
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAppLaunchMasterData")]
        Task<T> GetAppLaunchMasterData<T>([Body] EncryptedRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/GetAccounts_OT")]
        //Task<T> GetCustomerAccountList<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/GetAccountsV3")]
        //Task<T> GetCustomerAccountListAppLunch<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);    //lyana //merge DBR

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountsV4")]
        Task<T> GetCustomerAccountList<T>([Body] EncryptedRequest request, CancellationToken token); //lyana
        
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v2/launch/GetAppLaunchMasterData")]
        Task<T> GetAppLaunchMasterDataAWS<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v2/account/Getaccount")]
        Task<T> GetCustomerAccountListAppLaunch<T>([Body] MyTNBService.Request.BaseRequestV4 request, CancellationToken token);  //lyana //aws applaunch only

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/AddTNBAccountToUserReg")]
        Task<T> AddAccountToCustomer<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ValidateManualAccountLinking")]
        Task<T> ValidateManualAccount<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetSearchForAccount")]
        Task<T> GetSearchForAccount<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetPhoneVerifyStatus")]
        Task<T> PhoneVerifyStatus<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAppNotificationChannels")]
        Task<T> AppNotificationChannels<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAppNotificationTypes")]
        Task<T> AppNotificationTypes<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetCustomerAccountsForICNum")]
        Task<T> CustomerAccountsForICNum<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SubmitFeedback")]
        Task<T> SubmitFeedback<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SubmitFeedbackWithContactDetails")]
        Task<T> SubmitEnquiry<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SubmitEnquiry")]
        Task<T> SubmitEnquiryWithType<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetSubmittedFeedbackList")]
        Task<T> SubmittedFeedbackList<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetOvervoltageClaimDetail")]
        Task<T> OvervoltageClaimDetail<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/TriggerOVISServices")]
        Task<T> TriggerOVISServices<T>([Body] EncryptedRequest request, CancellationToken token);

        //TODO
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetLocationsByKeyword")]
        Task<T> LocationsByKeyword<T>([Body] EncryptedRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/IsUserAuth")]
        //Task<T> UserAuthenticate<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/IsUserAuthV2")]
        Task<T> UserAuthenticate<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/IsUserAuthV4")]
        Task<T> UserAuthenticateLoginNew<T>([Body] EncryptedRequest request, CancellationToken token);      //lyana //merge DBR

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/CancelInvitation_OT")]
        Task<T> CancelInvitation_OT<T>([Body] EncryptedRequest request, CancellationToken token);              //wan //api cancel invited user

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SendReInviteEmail")]
        Task<T> SendReInviteEmail<T>([Body] EncryptedRequest request, CancellationToken token);              //irul //api resend invited user

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/AddUserAccess_OT")]
        Task<T> AddUserAcess_OT<T>([Body] EncryptedRequest request, CancellationToken token);              //wan //api add user access

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/RemoveUserAccess_OT")]
        Task<T> RemoveUserAcess_OT<T>([Body] EncryptedRequest request, CancellationToken token);              //wan //api delete user access

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountActivityLogList")]
        Task<T> GetAccountActivityLogList<T>([Body] EncryptedRequest request, CancellationToken token);              //wan //api log activity user access

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/UpdateAccountAccessRight")]
        Task<T> UpdateAccountAccessRight<T>([Body] EncryptedRequest request, CancellationToken token);              //2enapps wan //api update account access right

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/AddAccounts_OT")]
        //Task<T> AddMultipleAccounts_OT<T>([Body] MyTNBService.Request.BaseRequestV2 request, CancellationToken token);              //2enapps wan //api add account

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/AddAccountsV3")]
        //Task<T> AddMultipleAccounts_OT<T>([Body] MyTNBService.Request.BaseRequestV2 request, CancellationToken token);    //lyana //merge DBR

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/AddAccountsV4")]
        Task<T> AddMultipleAccounts_OT<T>([Body] EncryptedRequest request, CancellationToken token);    //yana //merge DBR and CEP

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ValidateManualAccountLinking_OT")]
        Task<T> ValidateManualAccount_OT<T>([Body] EncryptedRequest request, CancellationToken token);            //2enapps wan //api validate manual linking acc

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountAccessRightList")]
        Task<T> GetAccountAccessRightList<T>([Body] EncryptedRequest request, CancellationToken token);        //2enapps wan //api get account access right list account

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountAccessRight")]
        Task<T> GetAccountAccessRight<T>([Body] EncryptedRequest request, CancellationToken token);       //2enapps wan //api get account access r account

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetReviewAccount")]
        Task<T> GetReviewAccount<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan //api review account

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/CreateNewUserWithToken_OT")]
        //Task<T> CreateNewUserWithToken_OT<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan //api register baru

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/CreateNewUserWithTokenV2")]
        Task<T> CreateNewUserWithToken_OT<T>([Body] EncryptedRequest request, CancellationToken token);        //yana - handle for idtype

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/IsUserAuth_OT")]
        //Task<T> UserAuthenticateLoginNew<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token); //2enapps wan //api login baru

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/IsUserAuthV3")]
        //Task<T> UserAuthenticateLoginNew<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);      //lyana //merge DBR


        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/GetRegisteredUser")]
        //Task<T> UserAuthenticateEmail<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/GetRegisteredUser")]
        //Task<T> UserAuthenticateID<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetRegisteredUserV2")]
        Task<T> UserAuthenticateEmail<T>([Body] EncryptedRequest request, CancellationToken token);        //yana - to handle id type based on master lookup

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetRegisteredUserV2")]
        Task<T> UserAuthenticateID<T>([Body] EncryptedRequest request, CancellationToken token);       //yana - to handle id type based on master lookup

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/UpdateIdentificationNo")]
        //Task<T> UserUpdateIdentifcationNo<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/UpdateIdentificationNoV2")]
        Task<T> UserUpdateIdentifcationNo<T>([Body] EncryptedRequest request, CancellationToken token);        //yana - to handle id type based on master lookup

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/UpdateUserInfo")]
        Task<T> UserUpdateName<T>([Body] EncryptedRequest request, CancellationToken token);        //2enapps wan

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/SaveUserNotificationTypePreference")]
        //Task<T> SaveUserNotificationTypePreference<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SaveUserNotificationTypePreferenceV3")]
        Task<T> SaveUserNotificationTypePreference<T>([Body] EncryptedRequest request, CancellationToken token);        //cep new api //wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SaveUserNotificationChannelPreferenceV3")]
        Task<T> SaveUserNotificationChannelPreference<T>([Body] EncryptedRequest request, CancellationToken token);

        // [Headers("Content-Type:application/json; charset=utf-8")]
        // [Post("/v6/mytnbappws.asmx/GetUserNotificationTypePreferencesV3")]
        // Task<T> UserNotificationTypePreferences<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);
        
        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/GetUserNotificationTypePreferencesV2")]
        //Task<T> UserNotificationTypePreferences<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetUserNotificationTypePreferencesV4")]
        Task<T> UserNotificationTypePreferences<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetUserNotificationChannelPreferences")]
        Task<T> UserNotificationChannelPreferences<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SendRegistrationTokenSMS_OT")]
        Task<T> SendRegistrationTokenSMS<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetRegisteredCards")]
        Task<T> GetRegisteredCards<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetSubmittedFeedbackDetails")]
        Task<T> SubmittedFeedbackDetails<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetSubmittedFeedbackWithContactDetails")]
        Task<T> SubmittedFeedbackContactDetails<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/CreateNewUserWithToken")]
        Task<T> CreateNewUserWithToken<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SendResetPasswordCode")]
        Task<T> SendResetPasswordCode<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ResetPasswordWithToken")]
        Task<T> ResetPasswordWithToken<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/LogoutUser")]
        Task<T> LogoutUser<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/RemoveRegisteredCard")]
        Task<T> RemoveRegisteredCard<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ChangeNewPassword")]
        Task<T> ChangeNewPassword<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SendResetPasswordCode_OT")]
        Task<T> SendResetPasswordCode_OT<T>([Body] EncryptedRequest request, CancellationToken token); //yana

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SendEmailVerification")]
        Task<T> SendEmailVerification<T>([Body] EncryptedRequest request, CancellationToken token); //yana

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SendUpdatePhoneTokenSMSV2")]
        Task<T> SendUpdatePhoneTokenSMSV2<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/UpdatePhoneNumber")]
        Task<T> UpdatePhoneNumber<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SubmitRateUs")]
        Task<T> SubmitRateUs<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SubmitRateUsV2")]
        Task<T> SubmitRateUsV2<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/UpdateLinkedAccountNickName")]
        Task<T> UpdateLinkedAccountNickName<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetRateUsQuestions")]
        Task<T> GetRateUsQuestions<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ShowEnergyBudgetRatingPage")]
        Task<T> ShowEnergyBudgetRatingPage<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ExperienceRatingUserLeaveOut")]
        Task<T> ExperienceRatingUserLeaveOut<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetLanguagePreference")]
        Task<T> GetLanguagePreference<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SaveLanguagePreferenceV2")]
        Task<T> SaveLanguagePreference<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetBillHistory")]
        Task<T> GetBillHistory<T>([Body] EncryptedRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/RemoveAccount_OT")]
        //Task<T> RemoveAccount<T>([Body] MyTNBService.Request.BaseRequestV2 request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/RemoveAccountV3")]
        //Task<T> RemoveAccount<T>([Body] MyTNBService.Request.BaseRequestV2 request, CancellationToken token);       //lyana //merge DBR

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/RemoveAccountV4")]
        Task<T> RemoveAccount<T>([Body] EncryptedRequest request, CancellationToken token);       //lyana //merge DBR


        //[Post("/v6/mytnbappws.asmx/RemoveAccount")]
        //Task<T> RemoveAccount<T>([Body] EncryptedRequest request, CancellationToken token);

        // [Headers("Content-Type:application/json; charset=utf-8")]
        // [Post("/v6/mytnbappws.asmx/RemoveAccountV2")]
        // Task<T> RemoveAccount<T>([Body] EncryptedRequest request, CancellationToken token);         //cep new api //wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SaveEnergyBudget")]
        Task<T> SaveEnergyBudget<T>([Body] EncryptedRequest request, CancellationToken token);         //cep new api //wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetLocations")]
        Task<T> GetLocations<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetLocationsByKeyword")]
        Task<T> GetLocationsByKeyword<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetPaymentReceipt")]
        Task<T> GetPaymentReceipt<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetUserNotifications")]
        Task<T> GetUserNotifications<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetUserNotificationsV2")]
        Task<T> GetUserNotificationsV2<T>([Body] EncryptedRequest request, CancellationToken token); //cep new api //yana

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetNotificationDetailedInfo_V2")]
        Task<T> GetNotificationDetails<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetNotificationDetailsByRequestId")]
        Task<T> GetNotificationDetailsByRequestId<T>([Body] EncryptedRequest request, CancellationToken token); //cep new api // yana

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetSubscriptionStatus")]
        Task<T> GetUserServiceDistruptionSub<T>([Body] EncryptedRequest request, CancellationToken token); //cep-sd new api // wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ServiceDisruptionInfo")]
        Task<T> ServiceDisruptionInfo<T>([Body] EncryptedRequest request, CancellationToken token); //cep-sd new api // wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SDSubmitRateUs")]
        Task<T> SDSubmitRateUs<T>([Body] EncryptedRequest request, CancellationToken token); //cep-sd new api // wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ShowSDRatingPage")]
        Task<T> ShowSDRatingPage<T>([Body] EncryptedRequest request, CancellationToken token); //cep-sd new api // wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/DeleteUserNotification")]
        Task<T> DeleteUserNotification<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ReadUserNotification")]
        Task<T> ReadUserNotification<T>([Body] EncryptedRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/AddAccounts")]
        //Task<T> AddMultipleAccounts<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/AddAccountsV3")]
        Task<T> AddMultipleAccounts<T>([Body] EncryptedRequest request, CancellationToken token);           //cep new api //wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountsCharges")]
        Task<T> GetAccountsCharges<T>([Body] EncryptedRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v6/mytnbappws.asmx/GetAccountBillPayHistory")]
        //Task<T> GetAccountBillPayHistory<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountBillPayHistoryV4")]
        Task<T> GetAccountBillPayHistory<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetPaymentTransactionId")]
        Task<T> GetPaymentTransactionId<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/UpdateUserStatusActivate")]
        Task<T> UpdateUserStatusActivate<T>([Body] EncryptedRequest request, CancellationToken token);   //dynamic link verified

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/UpdateUserStatusDeactivate")]
        Task<T> UpdateUserStatusDeactivate<T>([Body] EncryptedRequest request, CancellationToken token);   //dynamic link removed account

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/NCAutoAddAccounts")]
        Task<T> NCAutoAddAccounts<T>([Body] EncryptedRequest request, CancellationToken token);   //NC account

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v1/UserInfo/UpdateUserInfoDevice")]
        Task<T> UpdateUserInfoDevice<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v1/UserInfo/GetIdentificationNo")]
        Task<T> GetIdentificationNo<T>([Body] MyTNBService.Request.BaseRequestV4 request, CancellationToken token);
    }
}
