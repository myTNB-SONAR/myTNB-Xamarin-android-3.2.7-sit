using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.Base.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.Feedback_PreLogin_Others.MVP
{
    public class FeedbackPreLoginOthersContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Shows progress dialog invokes when user clicked logged in
            ///  Pre-Validation
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            ///  Hides progress dialog when logging in is finish
            ///  Pre-Validation
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            /// Shows the camera option
            /// </summary>
            void ShowCamera();

            /// <summary>
            /// Shows the gallery of images option
            /// </summary>
            void ShowGallery();

            /// <summary>
            /// NOT USED
            /// </summary>
            void ShowMaximumAttachPhotosAllowed();

            /// <summary>
            /// Returns the temporary image file path
            /// </summary>
            /// <param name="pFolder">string</param>
            /// <param name="pFileName">string</param>
            /// <returns>string</returns>
            string GetTemporaryImageFilePath(string pFolder, string pFileName);

            /// <summary>
            /// Task that saves the image asynchronously
            /// </summary>
            /// <param name="attachedImage">AttachedImage</param>
            /// <returns>Task</returns>
            Task<AttachedImageRequest> SaveImage(AttachedImage attachedImage);

            /// <summary>
            /// Task that saves the camera image asynchronously
            /// </summary>
            /// <param name="tempImagePath">string</param>
            /// <param name="fileName">string</param>
            /// <returns>Task</returns>
            Task<string> SaveCameraImage(string tempImagePath, string fileName);

            /// <summary>
            /// Task that saves the gallery image asynchronously
            /// </summary>
            /// <param name="selectedImage">Android.Net.Uri</param>
            /// <param name="tempImagePath">string</param>
            /// <param name="fileName">string</param>
            /// <returns>Task</returns>
            Task<string> SaveGalleryImage(Android.Net.Uri selectedImage, string tempImagePath, string fileName);

            /// <summary>
            /// Shows image loader
            /// </summary>
            void ShowLoadingImage();

            /// <summary>
            /// Hides image loader
            /// </summary>
            void HideLoadingImage();

            /// <summary>
            /// Updates the adapter's record
            /// </summary>
            /// <param name="pFilePath">string</param>
            /// <param name="pFileName">string</param>
            void UpdateAdapter(string pFilePath, string pFileName);

            /// <summary>
            /// Show empty fullname error
            /// </summary>
            void ShowEmptyFullnameError();

            /// <summary>
            /// Show empty mobile no error
            /// </summary>
            void ShowEmptyMobileNoError();

            /// <summary>
            /// Show invalid mobile no error
            /// </summary>
            void ShowInvalidMobileNoError();

            /// <summary>
            /// Show empty email error
            /// </summary>
            void ShowEmptyEmaiError();

            /// <summary>
            /// Show invalid email error
            /// </summary>
            void ShowInvalidEmailError();

            /// <summary>
            /// Show empty feedback error
            /// </summary>
            void ShowEmptyFeedbackError();

            /// <summary>
            /// Clears all errors
            /// </summary>
            void ClearErrors();

            /// <summary>
            /// Clears all entry fields
            /// </summary>
            void ClearInputFields();

            /// <summary>
            /// Shows the success screen of saving feedback
            /// </summary>
            /// <param name="date">string</param>
            /// <param name="feedbackId">string</param>
            /// <param name="imageCount">integer</param>
            void ShowSuccess(string date, string feedbackId, int imageCount);

            /// <summary>
            /// Shows the unsuccessful screen of saving feedback
            /// </summary>
            void ShowFail();

            /// <summary>
            /// Enable submit button
            /// </summary>
            void EnableSubmitButton();

            /// <summary>
            /// Disable submit button
            /// </summary>
            void DisableSubmitButton();

            /// <summary>
            /// Shows initial current feedback type
            /// </summary>
            /// <param name="feedbackType">FeedbackType</param>
            void ShowSelectedFeedbackType(FeedbackType feedbackType);

            /// <summary>
            /// Show a selection of feedback types
            /// </summary>
            void ShowSelectFeedbackType();

            /// <summary>
            /// Returns the image name
            /// </summary>
            /// <param name="itemCount">integer</param>
            /// <returns>string</returns>
            string GetImageName(int itemCount);

            void ShowNameError();


            void OnSubmitError(string message = null);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to select feedback type
            /// </summary>
            void OnSelectFeedbackType();

            /// <summary>
            /// Action to attach photo from camera
            /// </summary>
            void OnAttachPhotoCamera();

            /// <summary>
            /// Action to attach image from gallery
            /// </summary>
            void OnAttachPhotoGallery();

            /// <summary>
            /// The returned result from another activity
            /// </summary>
            /// <param name="requestCode">integer</param>
            /// <param name="resultCode">enum</param>
            /// <param name="data">intent</param>
            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);

            /// <summary>
            /// Action to submit feedback
            /// </summary>
            /// <param name="deviceId">string</param>
            /// <param name="fullname">string</param>
            /// <param name="mobile_no">string</param>
            /// <param name="email">string</param>
            /// <param name="feedbackType">FeedbackType</param>
            /// <param name="feedback">string</param>
            /// <param name="attachedImages">List<paramref name="AttachedImage"/></param>
            void OnSubmit(string deviceId, string fullname, string mobile_no, string email, FeedbackType feedbackType, string feedback, List<AttachedImage> attachedImages);

            /// <summary>
            /// Action to check empty fullname , mobile no, email and feedback
            /// </summary>
            /// <param name="fullname">string</param>
            /// <param name="mobile_no">string</param>
            /// <param name="email">string</param>
            /// <param name="feedback">string</param>
            void CheckRequiredFields(string fullname, string mobile_no, string email, string feedback);
        }
    }
}