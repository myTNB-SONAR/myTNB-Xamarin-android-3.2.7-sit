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
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Base.Request;
using static myTNB_Android.Src.UpdatePersonalDetailStepTwo.Activity.UpdatePersonalDetailStepTwoActivity;

namespace myTNB_Android.Src.UpdatePersonalDetailStepTwo.MVP
{
    public class UpdatePersonalDetailStepTwoContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

             void ShowinfoLabelProofOfConsent();

            void ShowinfoLabelCopyOfIdentification();

            string getActualPath(Android.Net.Uri uri);
            void ShowError(string message=null);


            void ShowCamera();

            void ShowGallery();

            Task<AttachedImageRequest> SaveImage(AttachedImage attachedImage);

            string GetTemporaryImageFilePath(string pFolder, string pFileName);

            void DisableSubmitButton();

            void ShowLoadingImage();

            void HideLoadingImage();

            Task<string> SaveCameraImage(string tempImagePath, string fileName);

            void EnableSubmitButton();

            Task<string> SaveGalleryImage(Android.Net.Uri selectedImage, string pTempImagePath, string pFileName);

            void UpdateAdapter(string pFilePath, string pFileName, string tFullname = "");


            void ShowinfoLabelPermise();

            string GetImageName(int itemCount);

            void ShowPDF();
        }

        public interface IUserActionsListener : IBasePresenter
        {


            void OninfoLabelProofOfConsent();

            void OninfoLabelCopyOfIdentification();

            void OnAttachPhotoCamera();

            void OnAttachPhotoGallery();

            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);

            void OninfoLabelPermise();

            void OnAttachPDF();
        }
    }
}