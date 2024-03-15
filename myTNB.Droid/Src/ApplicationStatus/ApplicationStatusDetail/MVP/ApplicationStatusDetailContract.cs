using Android.OS;
using myTNB.AndroidApp.Src.ApplicationStatus.ApplicationStatusDetail.Models;
using myTNB.AndroidApp.Src.NewAppTutorial.MVP;
using myTNB.Mobile;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.ApplicationStatus.ApplicationStatusDetail.MVP
{
    public class ApplicationStatusDetailContract
    {
        public interface IView
        {
            /// <summary>
            /// On Screen Load function
            /// </summary>
            /// <param name="extras"></param>
            void OnScreenLoad(Bundle extras);

            /// <summary>
            /// Updates the UI
            /// </summary>
            void UpdateUI();

            /// <summary>
            /// Shows loading indicator in full screen
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hides loading indicator
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            /// Triggers the share functionality for the downloaded file
            /// </summary>
            /// <param name="filePath"></param>
            /// <param name="fileExtension"></param>
            /// <param name="fileTitle"></param>
            void ShareDownloadedFile(string filePath, string fileExtension, string fileTitle);

            /// <summary>
            /// Shows Generic Error Pop Up
            /// </summary>
            void OnShowGenericErrorPopUp();

            /// <summary>
            /// Function to navigate to microsite
            /// </summary>
            /// <param name="resultCode"></param>
            /// <param name="cancelUrl"></param>
            void NavigateToMicrosite(string accessToken, int resultCode, string cancelUrl);

            /// <summary>
            /// Handles the result of Delete Draft API
            /// </summary>
            /// <param name="message"></param>
            void DeleteDraftOnResult(bool isSuccess, string message);

            /// <summary>
            /// Handles the result of GetApplication API call
            /// </summary>
            /// <param name="response"></param>
            /// <param name="updateType"></param>
            /// <param name="toastMessage"></param>
            void GetApplicationDetailOnResult(ApplicationDetailDisplay response, UpdateType updateType, string toastMessage = "");
        }

        public interface IPresenter
        {
            List<NewAppModel> OnGeneraNewAppTutorialNoActionList();
            List<NewAppModel> OnGeneraNewAppTutorialActionList();
            List<NewAppModel> OnGeneraNewAppTutorialNoneList();
            List<NewAppModel> OnGeneraNewAppTutorialInProgressList();

            /// <summary>
            /// Task to download file from webURL
            /// </summary>
            /// <param name="webURL"></param>
            /// <returns></returns>
            Task DownloadFile(string webURL);

            /// <summary>
            /// Function to call ApplicationDetail API to reload the screen
            /// </summary>
            void OnGetApplicationDetail(GetApplicationStatusDisplay statusDisplay, UpdateType updateType, string toastMessage = "");

            /// <summary>
            /// Gets the Access Token for Microsite reload
            /// </summary>
            /// <param name="resultCode"></param>
            /// <param name="cancelUrl"></param>
            void OnGetAccessToken(int resultCode, string cancelUrl);

            /// <summary>
            /// Function to delete draft application for NC, COT and COA
            /// </summary>
            /// <param name="type"></param>
            /// <param name="isCOTExistingOwner"></param>
            void OnDeleteDraft(string refNo, SupplyOfferingType type, bool isCOTExistingOwner = false);
        }
    }
}