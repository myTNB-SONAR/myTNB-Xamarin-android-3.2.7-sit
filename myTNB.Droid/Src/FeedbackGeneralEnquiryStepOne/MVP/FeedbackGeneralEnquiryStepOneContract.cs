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


            void ShowGeneralEnquiry();

            void ClearErrors();

            void EnableSubmitButton();

            void ShowEmptyFeedbackError();

            void DisableSubmitButton();


            void ShowCamera();

            void ShowGallery();

            void ShowPDF();





            string GetTemporaryImageFilePath(string pFolder, string pFileName);


            void ShowLoadingImage();

            void HideLoadingImage();

            Task<string> SaveGalleryImage(Android.Net.Uri selectedImage, string pTempImagePath, string pFileName);

            string GetImageName(int itemCount);

            void UpdateAdapter(string pFilePath, string pFileName, string tfileName="");

            Task<string> SaveCameraImage(string tempImagePath, string fileName);

        }

        public interface IUserActionsListener : IBasePresenter
        {



            void OnGeneralEnquiry();

            void CheckRequiredFields(string feedback);

            void OnAttachPhotoCamera();


            void  OnAttachPhotoGallery();

            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);


            void OnAttachPDF();

         



        }
    }
}