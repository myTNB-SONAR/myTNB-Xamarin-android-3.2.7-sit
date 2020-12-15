﻿using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.MyTNBService.Request;
using Refit;

namespace myTNB_Android.Src.MyTNBService.InterfaceAPI
{
    public interface IServiceV6
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAppLaunchMasterData")]
        Task<T> GetAppLaunchMasterData<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccounts")]
        Task<T> GetCustomerAccountList<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/AddTNBAccountToUserReg")]
        Task<T> AddAccountToCustomer<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

		[Headers("Content-Type:application/json; charset=utf-8")]
		[Post("/v6/mytnbappws.asmx/ValidateManualAccountLinking")]
		Task<T> ValidateManualAccount<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetSearchForAccount")]
        Task<T> GetSearchForAccount<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetPhoneVerifyStatus")]
        Task<T> PhoneVerifyStatus<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAppNotificationChannels")]
        Task<T> AppNotificationChannels<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAppNotificationTypes")]
        Task<T> AppNotificationTypes<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetCustomerAccountsForICNum")]
        Task<T> CustomerAccountsForICNum<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SubmitFeedback")]
        Task<T> SubmitFeedback<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SubmitFeedbackWithContactDetails")]
        Task<T> SubmitEnquiry<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetSubmittedFeedbackList")]
        Task<T> SubmittedFeedbackList<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        //TODO
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetLocationsByKeyword")]
        Task<T> LocationsByKeyword<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/IsUserAuth")]
        Task<T> UserAuthenticate<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/UpdateAccountAccessRight")]
        Task<T> UpdateAccountAccessRight<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);              //2enapps wan //api update account access right

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/AddAccounts_OT")]
        Task<T> AddMultipleAccounts_OT<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);              //2enapps wan //api add account

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ValidateManualAccountLinking_OT")]
        Task<T> ValidateManualAccount_OT<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);            //2enapps wan //api validate manual linking acc

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountAccessRightList")]
        Task<T> GetAccountAccessRightList<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan //api get account access right list account

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountAccessRight")]
        Task<T> GetAccountAccessRight<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);       //2enapps wan //api get account access r account

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetReviewAccount")]
        Task<T> GetReviewAccount<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan //api review account

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/CreateNewUserWithToken_OT")]
        Task<T> CreateNewUserWithToken_OT<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan //api register baru

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/IsUserAuth_OT")]
        Task<T> UserAuthenticateLoginNew<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);      //2enapps wan //api login baru

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetRegisteredUser")]
        Task<T> UserAuthenticateEmail<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetRegisteredUser")]
        Task<T> UserAuthenticateID<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/UpdateIdentificationNo")]
        Task<T> UserUpdateIdentifcationNo<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/UpdateUserInfo")]
        Task<T> UserUpdateName<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);        //2enapps wan

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SaveUserNotificationTypePreference")]
        Task<T> SaveUserNotificationTypePreference<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SaveUserNotificationChannelPreference")]
        Task<T> SaveUserNotificationChannelPreference<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetUserNotificationTypePreferences")]
        Task<T> UserNotificationTypePreferences<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetUserNotificationChannelPreferences")]
        Task<T> UserNotificationChannelPreferences<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SendRegistrationTokenSMS_OT")]
        Task<T> SendRegistrationTokenSMS<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetRegisteredCards")]
        Task<T> GetRegisteredCards<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);
              
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetSubmittedFeedbackDetails")]
        Task<T> SubmittedFeedbackDetails<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetSubmittedFeedbackWithContactDetails")]
        Task<T> SubmittedFeedbackContactDetails<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        
        
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/CreateNewUserWithToken")]
        Task<T> CreateNewUserWithToken<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SendResetPasswordCode")]
        Task<T> SendResetPasswordCode<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ResetPasswordWithToken")]
        Task<T> ResetPasswordWithToken<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/LogoutUser")]
        Task<T> LogoutUser<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/RemoveRegisteredCard")]
        Task<T> RemoveRegisteredCard<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ChangeNewPassword")]
        Task<T> ChangeNewPassword<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SendUpdatePhoneTokenSMSV2")]
        Task<T> SendUpdatePhoneTokenSMSV2<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/UpdatePhoneNumber")]
        Task<T> UpdatePhoneNumber<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SubmitRateUs")]
        Task<T> SubmitRateUs<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/UpdateLinkedAccountNickName")]
        Task<T> UpdateLinkedAccountNickName<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetRateUsQuestions")]
        Task<T> GetRateUsQuestions<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetLanguagePreference")]
        Task<T> GetLanguagePreference<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SaveLanguagePreference")]
        Task<T> SaveLanguagePreference<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetBillHistory")]
        Task<T> GetBillHistory<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/RemoveAccount")]
        Task<T> RemoveAccount<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetLocations")]
        Task<T> GetLocations<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetLocationsByKeyword")]
        Task<T> GetLocationsByKeyword<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetPaymentReceipt")]
        Task<T> GetPaymentReceipt<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetUserNotifications")]
        Task<T> GetUserNotifications<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetNotificationDetails")]
        Task<T> GetNotificationDetails<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/DeleteUserNotification")]
        Task<T> DeleteUserNotification<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ReadUserNotification")]
        Task<T> ReadUserNotification<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/AddAccounts")]
        Task<T> AddMultipleAccounts<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountsCharges")]
        Task<T> GetAccountsCharges<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountBillPayHistory")]
        Task<T> GetAccountBillPayHistory<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetPaymentTransactionId")]
        Task<T> GetPaymentTransactionId<T>([Body] MyTNBService.Request.BaseRequest request, CancellationToken token);
    }
}
