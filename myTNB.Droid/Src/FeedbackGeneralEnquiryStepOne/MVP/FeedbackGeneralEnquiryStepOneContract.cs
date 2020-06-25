using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.MVP;

namespace myTNB_Android.Src.FeedbackGeneralEnquiryStepOne.MVP
{
    public class FeedbackGeneralEnquiryStepOneContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

            void ShowCamera();
            void ShowGallery();

            void ShowGeneralEnquiry();

            void ClearErrors();

            void EnableSubmitButton();

            void ShowEmptyFeedbackError();

            void DisableSubmitButton();

            string GetTemporaryImageFilePath(string pFolder, string pFileName);

            void ShowLoadingImage();

            Task<string> SaveCameraImage(string tempImagePath, string fileName);

            void UpdateAdapter(string pFilePath, string pFileName);


            void HideLoadingImage();

            Task<string> SaveGalleryImage(Android.Net.Uri selectedImage, string pTempImagePath, string pFileName);




        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to attach photo from camera
            /// </summary>
            void OnAttachPhotoCamera();

            /// <summary>
            /// Action to attach image from gallery
            /// </summary>
            void OnAttachPhotoGallery();


            void OnGeneralEnquiry();

            void CheckRequiredFields(string feedback);

            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);





        }
    }
}