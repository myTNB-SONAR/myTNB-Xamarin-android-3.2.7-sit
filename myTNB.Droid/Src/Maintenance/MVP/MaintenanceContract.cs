using myTNB_Android.Src.Base.MVP;
using Refit;
using System;

namespace myTNB_Android.Src.Maintenance.MVP
{
    public class MaintenanceContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// The unique id of the device
            /// </summary>
            /// <returns>unique id alphanumeric strings</returns>
            string GetDeviceId();

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
            /// Launch AppLaunch
            /// </summary>
            void ShowLaunchViewActivity();

            void OnUpdateMaintenanceWord(string mTitle, string mMessage);

            //STUB
            string GetAppMasterLaunchStub();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Apps Back from Background to Foreground
            /// </summary>
            void OnResume();
        }
    }
}