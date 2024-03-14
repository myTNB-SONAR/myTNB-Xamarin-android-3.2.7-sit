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
using myTNB.Android.Src.Base.MVP;

namespace myTNB.Android.Src.FeedbackAboutBillEnquiryStepOne.MVP
{
    public class FeedbackAboutBillEnquiryStepOneContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

            void ShowSelectCategory();
            void ShowAboutBillEnquiry();

            void ClearErrors();

            void EnableSubmitButton();

            void ShowEmptyFeedbackError();

            void DisableSubmitButton();


            void ShowCamera();

            void ShowGallery();

            void ShowPDF();

            void ShowError(string message = null);

            string copyPDFGetFilePath(Android.Net.Uri realFilePath, string filename);

            string GetTemporaryImageFilePath(string pFolder, string pFileName);


            void ShowLoadingImage();

            void HideLoadingImage();

            Task<string> SaveGalleryImage(Android.Net.Uri selectedImage, string pTempImagePath, string pFileName);

            string GetImageName(int itemCount);

            void UpdateAdapter(string pFilePath, string pFileName, string tfileName="");

            Task<string> SaveCameraImage(string tempImagePath, string fileName);

            string getActualPath(Android.Net.Uri uri);

            string getFilename(Android.Net.Uri uri);


        }

        public interface IUserActionsListener : IBasePresenter
        {


            void OnSelectCategory();
            void OnAboutBillEnquiry();

            void CheckRequiredFields(string feedback);

            void OnAttachPhotoCamera();


            void  OnAttachPhotoGallery();

            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);


            void OnAttachPDF();

         



        }
    }
}