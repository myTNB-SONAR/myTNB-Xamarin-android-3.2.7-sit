﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.MyTNBService.Response;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace myTNB_Android.Src.AppLaunch.MVP
{
    public class AppLaunchContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Navigates/Show the Walkthrough sitecore
            /// </summary>
            void ShowWalkThrough();

            /// <summary>
            /// Navigates/Show the ResetPassword
            /// </summary>
            void ShowResetPassword();

            /// <summary>
            /// Navigates/Show the PreLogin
            /// </summary>
            void ShowPreLogin();

            /// <summary>
            /// Navigates/Show the Dashboard
            /// </summary>
            void ShowDashboard();


            /// <summary>
            /// The unique id of the device
            /// </summary>
            /// <returns>unique id alphanumeric strings</returns>
            string GetDeviceId();

            /// <summary>
            /// NOT USED
            /// </summary>
            /// <param name="accountTypeList"></param>
            void ShowAccountTypes(List<AccountType> accountTypeList);

            /// <summary>
            /// Shows an api exception with an option to retry
            /// </summary>
            /// <param name="apiException">the returned exception</param>
            void ShowRetryOptionApiException(ApiException apiException);

            /// <summary>
            /// Shows an unknown exception with an option to retry
            /// </summary>
            /// <param name="unkownException">the returned exception</param>
            void ShowRetryOptionUknownException(Exception unkownException);

            /// <summary>
            /// Shows a dialogue that the device is unsupported of play services
            /// </summary>
            void ShowDeviceNotSupported();

            /// <summary>
            /// The result code that is returned from play services
            /// </summary>
            /// <returns>integer</returns>
            int PlayServicesResultCode();

            /// <summary>
            /// Shows the play services error
            /// </summary>
            /// <param name="resultCode">integer resultCode</param>
            void ShowPlayServicesErrorDialog(int resultCode);

            /// <summary>
            /// Navigates/Show Notification
            /// </summary>
            void ShowNotification();

            /// <summary>
            /// Sets the no. of notification badge
            /// </summary>
            /// <param name="count">integer representation of no of badges</param>
            void ShowNotificationCount(int count);


            void OnSavedTimeStampRecievd(string timestamp);

            void OnTimeStampRecieved(string timestamp);

            void OnSiteCoreServiceFailed(string message);


            void OnSavedAppLaunchTimeStampRecievd(string timestamp);

            void OnAppLaunchTimeStampRecieved(string timestamp);

            /// <summary>
            /// Action to show request SMS Permission
            /// </summary>
            void RequestSMSPermission();

            /// <summary>
            /// Shows SMS Permission
            /// </summary>
            void ShowSMSPermissionRationale();

            /// <summary>
            /// Checks for sms permission
            /// </summary>
            /// <returns>boolean representation of granted(true) or denied(false)</returns>
            bool IsGrantedSMSReceivePermission();

            /// <summary>
            /// Checks for showing sms rationale
            /// </summary>
            /// <returns>boolean representation of should show rationale or not</returns>
            bool ShouldShowSMSReceiveRationale();

            ///<summary>
            /// Lgout function created to update device id after removing persmissin
            ///</summary>
            void ShowLogout();

            ///<summary>
            /// Lgout function created to update device id after removing persmissin
            ///</summary>
            void ShowUpdateAvailable(string title, string message, string btnLabel);

            ///<summary>
            /// Lgout function created to update device id after removing persmissin
            ///</summary>
            void OnAppUpdateClick();

            ///<summary>
            /// Update phone number flow
            ///</summary>
            void ShowUpdatePhoneNumber(string phoneNumber);

            ///<summary>
            /// Show Maintenance Screen
            ///</summary>
            void ShowMaintenance(MasterDataResponse masterDataResponse);

            ///<summary>
            /// Show Maintenance Screen
            ///</summary>
            void ShowMaintenance(AppLaunchMasterDataResponse masterDataResponse);

            void SetAppLaunchSuccessfulFlag(bool flag, AppLaunchNavigation navigationWay);

            bool GetAppLaunchSiteCoreDoneFlag();

            void SetDefaultAppLaunchImage();

            void SetCustomAppLaunchImage(AppLaunchModel item);

            void OnGoAppLaunchEvent();

            void SetAppLaunchSiteCoreDoneFlag(bool flag);

            /// <summary>
            /// Show something went wrong Snackbar
            /// </summary>
            void ShowSomethingWrongException();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to navigate to Walkthrough
            /// </summary>
            void NavigateWalkThrough();

            /// <summary>
            /// The returned result from another activity
            /// </summary>
            /// <param name="requestCode">integer</param>
            /// <param name="resultCode">enum</param>
            /// <param name="data">intent</param>
            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);

            /// <summary>
            /// Action to navigate to Notification
            /// </summary>
            void NavigateNotification();

            void GetSavedAppLaunchTimeStamp();

            Task OnGetTimeStamp();

            void OnGetAppLaunchTimeStamp();

            void OnGetAppLaunchItem();

            void GetSavedTimeStamp();

            /// <summary>
            /// Action to request sms permission
            /// </summary>
            void OnRequestSMSPermission();

            /// <summary>
            /// The returned permission result
            /// </summary>
            /// <param name="requestCode">integer</param>
            /// <param name="permissions">string[] array</param>
            /// <param name="grantResults">Permission[] array</param>
            void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults);

            ///<summary>
            /// Lgout function created to update device id after removing persmissin
            ///</summary>
            void OnUpdateApp();

            Task OnGetAppLaunchCache();

            Task OnWaitSplashScreenDisplay(int millisecondDelay);
        }
    }
}