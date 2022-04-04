using System;
using myTNB.Mobile.AWS.Models.DS.Identification;
using myTNB_Android.Src.Base.MVP;

namespace myTNB_Android.Src.DigitalSignature.IdentityVerification.MVP
{
    public class DSIdentityVerificationContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Setting Up Layout
            /// </summary>
            void SetUpViews();

            /// <summary>
            /// Rendering the UI content
            /// </summary>
            void RenderContent();

            /// <summary>
            /// Shows Loading Overlay
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hides Loading Overlay
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            /// Shows Completed on other device pop up
            /// </summary>
            void ShowCompletedOnOtherDevicePopUp();

            /// <summary>
            /// Show Id not registered pop up
            /// </summary>
            void ShowIdNotRegisteredPopUp();

            /// <summary>
            /// Shows identity has been verified
            /// </summary>
            void ShowIdentityHasBeenVerified();

            /// <summary>
            /// Shows prepare document pop up
            /// </summary>
            void ShowPrepareDocumentPopUp(int? idType);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initialization in the Presenter
            /// </summary>
            void OnInitialize();

            /// <summary>
            /// Method for triggering actions when screen has started
            /// </summary>
            void OnStart();

            /// <summary>
            /// Calls the GetEKYCIdentification API
            /// </summary>
            void GetEKYCIdentificationOnCall();

            /// <summary>
            /// Gets the Identification model
            /// </summary>
            /// <returns></returns>
            GetEKYCIdentificationModel GetIdentificationModel();
        }
    }
}
