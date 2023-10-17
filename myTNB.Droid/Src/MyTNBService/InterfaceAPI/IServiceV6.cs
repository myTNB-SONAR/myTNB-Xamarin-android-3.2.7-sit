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
        [Post("/v7/mytnbws.asmx/GetAppLaunchMasterData")]
        Task<T> GetAppLaunchMasterData<T>([Body] EncryptedRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/GetAccounts_OT")]
        //Task<T> GetCustomerAccountList<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/GetAccountsV3")]
        //Task<T> GetCustomerAccountListAppLunch<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);    //lyana //merge DBR

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAccountsV4")]
        Task<T> GetCustomerAccountList<T>([Body] EncryptedRequest request, CancellationToken token); //lyana

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v3/launch/GetAppLaunchMasterData")]
        Task<T> GetAppLaunchMasterDataAWS<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v3/account/Getaccount")]
        Task<T> GetCustomerAccountListAppLaunch<T>([Body] EncryptedRequest request, CancellationToken token);  //lyana //aws applaunch only

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/AddTNBAccountToUserReg")]
        Task<T> AddAccountToCustomer<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/ValidateManualAccountLinking")]
        Task<T> ValidateManualAccount<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetSearchForAccount")]
        Task<T> GetSearchForAccount<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetPhoneVerifyStatus")]
        Task<T> PhoneVerifyStatus<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAppNotificationChannels")]
        Task<T> AppNotificationChannels<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAppNotificationTypes")]
        Task<T> AppNotificationTypes<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetCustomerAccountsForICNum")]
        Task<T> CustomerAccountsForICNum<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SubmitFeedback")]
        Task<T> SubmitFeedback<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SubmitFeedbackWithContactDetails")]
        Task<T> SubmitEnquiry<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SubmitEnquiry")]
        Task<T> SubmitEnquiryWithType<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetSubmittedFeedbackList")]
        Task<T> SubmittedFeedbackList<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetOvervoltageClaimDetail")]
        Task<T> OvervoltageClaimDetail<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/TriggerOVISServices")]
        Task<T> TriggerOVISServices<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        //TODO
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetLocationsByKeyword")]
        Task<T> LocationsByKeyword<T>([Body] EncryptedRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/IsUserAuth")]
        //Task<T> UserAuthenticate<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/IsUserAuthV2")]
        Task<T> UserAuthenticate<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/IsUserAuthV4")]
        Task<T> UserAuthenticateLoginNew<T>([Body] EncryptedRequest request, CancellationToken token);      //lyana //merge DBR

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/CancelInvitation_OT")]
        Task<T> CancelInvitation_OT<T>([Body] EncryptedRequest request, CancellationToken token);              //wan //api cancel invited user

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SendReInviteEmail")]
        Task<T> SendReInviteEmail<T>([Body] EncryptedRequest request, CancellationToken token);              //irul //api resend invited user

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/AddUserAccess_OT")]
        Task<T> AddUserAcess_OT<T>([Body] EncryptedRequest request, CancellationToken token);              //wan //api add user access

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/RemoveUserAccess_OT")]
        Task<T> RemoveUserAcess_OT<T>([Body] EncryptedRequest request, CancellationToken token);              //wan //api delete user access

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAccountActivityLogList")]
        Task<T> GetAccountActivityLogList<T>([Body] EncryptedRequest request, CancellationToken token);              //wan //api log activity user access

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/UpdateAccountAccessRight")]
        Task<T> UpdateAccountAccessRight<T>([Body] EncryptedRequest request, CancellationToken token);              //2enapps wan //api update account access right

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/AddAccounts_OT")]
        //Task<T> AddMultipleAccounts_OT<T>([Body] MyTNBService.Request.BaseRequestV2 request, CancellationToken token);              //2enapps wan //api add account

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/AddAccountsV3")]
        //Task<T> AddMultipleAccounts_OT<T>([Body] MyTNBService.Request.BaseRequestV2 request, CancellationToken token);    //lyana //merge DBR

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/AddAccountsV4")]
        Task<T> AddMultipleAccounts_OT<T>([Body] EncryptedRequest request, CancellationToken token);    //yana //merge DBR and CEP

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/ValidateManualAccountLinking_OT")]
        Task<T> ValidateManualAccount_OT<T>([Body] EncryptedRequest request, CancellationToken token);            //2enapps wan //api validate manual linking acc

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAccountAccessRightList")]
        Task<T> GetAccountAccessRightList<T>([Body] EncryptedRequest request, CancellationToken token);        //2enapps wan //api get account access right list account

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAccountAccessRight")]
        Task<T> GetAccountAccessRight<T>([Body] EncryptedRequest request, CancellationToken token);       //2enapps wan //api get account access r account

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetReviewAccount")]
        Task<T> GetReviewAccount<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan //api review account

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/CreateNewUserWithToken_OT")]
        //Task<T> CreateNewUserWithToken_OT<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan //api register baru

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/CreateNewUserWithTokenV2")]
        Task<T> CreateNewUserWithToken_OT<T>([Body] EncryptedRequest request, CancellationToken token);        //yana - handle for idtype

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/IsUserAuth_OT")]
        //Task<T> UserAuthenticateLoginNew<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token); //2enapps wan //api login baru

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/IsUserAuthV3")]
        //Task<T> UserAuthenticateLoginNew<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);      //lyana //merge DBR


        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/GetRegisteredUser")]
        //Task<T> UserAuthenticateEmail<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/GetRegisteredUser")]
        //Task<T> UserAuthenticateID<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetRegisteredUserV2")]
        Task<T> UserAuthenticateEmail<T>([Body] EncryptedRequest request, CancellationToken token);        //yana - to handle id type based on master lookup

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetRegisteredUserV2")]
        Task<T> UserAuthenticateID<T>([Body] EncryptedRequest request, CancellationToken token);       //yana - to handle id type based on master lookup

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/UpdateIdentificationNo")]
        //Task<T> UserUpdateIdentifcationNo<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/UpdateIdentificationNoV2")]
        Task<T> UserUpdateIdentifcationNo<T>([Body] EncryptedRequest request, CancellationToken token);        //yana - to handle id type based on master lookup

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/UpdateUserInfo")]
        Task<T> UserUpdateName<T>([Body] EncryptedRequest request, CancellationToken token);        //2enapps wan

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/SaveUserNotificationTypePreference")]
        //Task<T> SaveUserNotificationTypePreference<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SaveUserNotificationTypePreferenceV3")]
        Task<T> SaveUserNotificationTypePreference<T>([Body] EncryptedRequest request, CancellationToken token);        //cep new api //wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SaveUserNotificationChannelPreferenceV3")]
        Task<T> SaveUserNotificationChannelPreference<T>([Body] EncryptedRequest request, CancellationToken token);

        // [Headers("Content-Type:application/json; charset=utf-8")]
        // [Post("/v7/mytnbws.asmx/GetUserNotificationTypePreferencesV3")]
        // Task<T> UserNotificationTypePreferences<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/GetUserNotificationTypePreferencesV2")]
        //Task<T> UserNotificationTypePreferences<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetUserNotificationTypePreferencesV4")]
        Task<T> UserNotificationTypePreferences<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetUserNotificationChannelPreferences")]
        Task<T> UserNotificationChannelPreferences<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SendRegistrationTokenSMS_OT")]
        Task<T> SendRegistrationTokenSMS<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetRegisteredCards")]
        Task<T> GetRegisteredCards<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetSubmittedFeedbackDetails")]
        Task<T> SubmittedFeedbackDetails<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetSubmittedFeedbackWithContactDetails")]
        Task<T> SubmittedFeedbackContactDetails<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/CreateNewUserWithToken")]
        Task<T> CreateNewUserWithToken<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SendResetPasswordCode")]
        Task<T> SendResetPasswordCode<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/ResetPasswordWithToken")]
        Task<T> ResetPasswordWithToken<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/LogoutUser")]
        Task<T> LogoutUser<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/RemoveRegisteredCard")]
        Task<T> RemoveRegisteredCard<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/ChangeNewPassword")]
        Task<T> ChangeNewPassword<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SendResetPasswordCode_OT")]
        Task<T> SendResetPasswordCode_OT<T>([Body] EncryptedRequest request, CancellationToken token); //yana

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SendEmailVerification")]
        Task<T> SendEmailVerification<T>([Body] EncryptedRequest request, CancellationToken token); //yana

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SendUpdatePhoneTokenSMSV2")]
        Task<T> SendUpdatePhoneTokenSMSV2<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/UpdatePhoneNumber")]
        Task<T> UpdatePhoneNumber<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SubmitRateUs")]
        Task<T> SubmitRateUs<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SubmitRateUsV2")]
        Task<T> SubmitRateUsV2<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/UpdateLinkedAccountNickName")]
        Task<T> UpdateLinkedAccountNickName<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetRateUsQuestions")]
        Task<T> GetRateUsQuestions<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/ShowEnergyBudgetRatingPage")]
        Task<T> ShowEnergyBudgetRatingPage<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/ExperienceRatingUserLeaveOut")]
        Task<T> ExperienceRatingUserLeaveOut<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetLanguagePreference")]
        Task<T> GetLanguagePreference<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SaveLanguagePreferenceV2")]
        Task<T> SaveLanguagePreference<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetBillHistory")]
        Task<T> GetBillHistory<T>([Body] EncryptedRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/RemoveAccount_OT")]
        //Task<T> RemoveAccount<T>([Body] MyTNBService.Request.BaseRequestV2 request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/RemoveAccountV3")]
        //Task<T> RemoveAccount<T>([Body] MyTNBService.Request.BaseRequestV2 request, CancellationToken token);       //lyana //merge DBR

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/RemoveAccountV4")]
        Task<T> RemoveAccount<T>([Body] EncryptedRequest request, CancellationToken token);       //lyana //merge DBR


        //[Post("/v7/mytnbws.asmx/RemoveAccount")]
        //Task<T> RemoveAccount<T>([Body] EncryptedRequest request, CancellationToken token);

        // [Headers("Content-Type:application/json; charset=utf-8")]
        // [Post("/v7/mytnbws.asmx/RemoveAccountV2")]
        // Task<T> RemoveAccount<T>([Body] EncryptedRequest request, CancellationToken token);         //cep new api //wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SaveEnergyBudget")]
        Task<T> SaveEnergyBudget<T>([Body] EncryptedRequest request, CancellationToken token);         //cep new api //wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetLocations")]
        Task<T> GetLocations<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetLocationsByKeyword")]
        Task<T> GetLocationsByKeyword<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetPaymentReceipt")]
        Task<T> GetPaymentReceipt<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetUserNotifications")]
        Task<T> GetUserNotifications<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetUserNotificationsV2")]
        Task<T> GetUserNotificationsV2<T>([Body] EncryptedRequest request, CancellationToken token); //cep new api //yana

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetNotificationDetailedInfo_V2")]
        Task<T> GetNotificationDetails<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetNotificationDetailsByRequestId")]
        Task<T> GetNotificationDetailsByRequestId<T>([Body] EncryptedRequest request, CancellationToken token); //cep new api // yana

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetSubscriptionStatus")]
        Task<T> GetUserServiceDistruptionSub<T>([Body] EncryptedRequest request, CancellationToken token); //cep-sd new api // wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/ServiceDisruptionInfo")]
        Task<T> ServiceDisruptionInfo<T>([Body] EncryptedRequest request, CancellationToken token); //cep-sd new api // wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SDSubmitRateUs")]
        Task<T> SDSubmitRateUs<T>([Body] EncryptedRequest request, CancellationToken token); //cep-sd new api // wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/ShowSDRatingPage")]
        Task<T> ShowSDRatingPage<T>([Body] EncryptedRequest request, CancellationToken token); //cep-sd new api // wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/DeleteUserNotification")]
        Task<T> DeleteUserNotification<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/ReadUserNotification")]
        Task<T> ReadUserNotification<T>([Body] EncryptedRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/AddAccounts")]
        //Task<T> AddMultipleAccounts<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/AddAccountsV3")]
        Task<T> AddMultipleAccounts<T>([Body] EncryptedRequest request, CancellationToken token);           //cep new api //wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAccountsCharges")]
        Task<T> GetAccountsCharges<T>([Body] EncryptedRequest request, CancellationToken token);

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v7/mytnbws.asmx/GetAccountBillPayHistory")]
        //Task<T> GetAccountBillPayHistory<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAccountBillPayHistoryV4")]
        Task<T> GetAccountBillPayHistory<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetPaymentTransactionId")]
        Task<T> GetPaymentTransactionId<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/UpdateUserStatusActivate")]
        Task<T> UpdateUserStatusActivate<T>([Body] EncryptedRequest request, CancellationToken token);   //dynamic link verified

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/UpdateUserStatusDeactivate")]
        Task<T> UpdateUserStatusDeactivate<T>([Body] EncryptedRequest request, CancellationToken token);   //dynamic link removed account

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/NCAutoAddAccounts")]
        Task<T> NCAutoAddAccounts<T>([Body] EncryptedRequest request, CancellationToken token);   //NC account

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v2/UserInfo/UpdateUserInfoDevice")]
        Task<T> UpdateUserInfoDevice<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v2/UserInfo/GetIdentificationNo")]
        Task<T> GetIdentificationNo<T>([Body] EncryptedRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAccountsSMRIcon")]
        Task<T> GetAccountsSMRIcon<T>([Body] EncryptedRequest request, CancellationToken token);
    }
}
