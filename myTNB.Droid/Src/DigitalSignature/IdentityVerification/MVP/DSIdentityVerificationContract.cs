using System;
using myTNB;
using myTNB.AndroidApp.Src.Base.MVP;

namespace myTNB.AndroidApp.Src.DigitalSignature.IdentityVerification.MVP
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

            /// <summary>
            /// Shows API error pop up
            /// </summary>
            /// <param name="statusDetail"></param>
            void ShowErrorMessage(StatusDetail statusDetail);

            /// <summary>
            /// Shows or Hides Loading Shimmer
            /// </summary>
            /// <param name="toShow"></param>
            void UpdateLoadingShimmer(bool toShow);

            /// <summary>
            /// Shows or Hides Bottom Container
            /// </summary>
            /// <param name="toShow"></param>
            void UpdateBottomContainer(bool toShow);

            /// <summary>
            /// Updates the Button state
            /// </summary>
            /// <param name="toShow"></param>
            void UpdateButtonState(bool toShow);

            /// <summary>
            /// Calls the API from the view
            /// </summary>
            void VerifyMatchingID();
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
            /// Calls the GetEKYCStatus API
            /// </summary>
            void GetEKYCStatusOnCall(DSDynamicLinkParamsModel dynamicLinkParamsModel);

            /// <summary>
            /// Gets the DSDynamicLinkParamsModel from dynamic link parsing
            /// </summary>
            /// <returns></returns>
            DSDynamicLinkParamsModel GetDSDynamicLinkParamsModel();
        }
    }
}
