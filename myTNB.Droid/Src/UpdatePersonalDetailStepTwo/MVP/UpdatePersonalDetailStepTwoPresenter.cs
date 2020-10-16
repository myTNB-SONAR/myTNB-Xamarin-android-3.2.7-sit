using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.UpdatePersonalDetailStepTwo.MVP
{
    public class UpdatePersonalDetailStepTwoPresenter : UpdatePersonalDetailStepTwoContract.IUserActionsListener
    {   

        UpdatePersonalDetailStepTwoContract.IView mView;
        private ISharedPreferences mSharedPref;
        private int countUserPick = 1;
        public UpdatePersonalDetailStepTwoPresenter(UpdatePersonalDetailStepTwoContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }


    
        public void Start()
        {
            try
            {

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OninfoLabelProofOfConsent()
        {
            this.mView.ShowinfoLabelProofOfConsent();
        }

         public void OninfoLabelCopyOfIdentification()
        {
            this.mView.ShowinfoLabelCopyOfIdentification();
        }

        public void OnAttachPhotoCamera()
        {
            this.mView.ShowCamera();
        }


        public void OnAttachPhotoGallery()
        {
            this.mView.ShowGallery();
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == Constants.REQUEST_ATTACHED_CAMERA_IMAGE)
            {
                if (resultCode == Result.Ok)
                {
                    // TODO : ADD PROGRESS
                    string fileName = string.Format("{0}.jpeg", this.mView.GetImageName(countUserPick));
                    countUserPick++;
                    string tempImagePath = this.mView.GetTemporaryImageFilePath(FileUtils.TEMP_IMAGE_FOLDER, string.Format("{0}.jpeg", "temporaryImage"));


                    OnSaveCameraImage(tempImagePath, fileName);

                    GC.Collect();
                }
            }
            else if (requestCode == Constants.RUNTIME_PERMISSION_GALLERY_REQUEST_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    Android.Net.Uri selectedImage = data.Data;
                    string fileName = string.Format("{0}.jpeg", this.mView.GetImageName(countUserPick));
                    countUserPick++;
                    OnSaveGalleryImage(selectedImage, fileName);
                    GC.Collect();
                }
            }

            else if (requestCode == Constants.RUNTIME_PERMISSION_GALLERY_PDF_REQUEST_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    Android.Net.Uri selectedImage = data.Data;
                    string fileName = string.Format("{0}.pdf", this.mView.GetImageName(countUserPick));
                    countUserPick++;
                    OnSaveGalleryPDF(selectedImage, fileName);
                    GC.Collect();
                }
            }

        }

        private async void OnSaveCameraImage(string tempImagePath, string fileName)
        {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();
            string resultFilePath = await this.mView.SaveCameraImage(tempImagePath, fileName);
            this.mView.UpdateAdapter(resultFilePath, fileName);
            this.mView.HideLoadingImage();
          //  this.mView.EnableSubmitButton();
        }

        private async void OnSaveGalleryImage(Android.Net.Uri selectedImage, string fileName)
        {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();
            string resultFilePath = await this.mView.SaveGalleryImage(selectedImage, FileUtils.TEMP_IMAGE_FOLDER, fileName);
            this.mView.UpdateAdapter(resultFilePath, fileName);
            this.mView.HideLoadingImage();
     
        }

        private async void OnSaveGalleryPDF(Android.Net.Uri selectedImage, string fileName)
        {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();
            string resultFilePath = removeRawtag(selectedImage.LastPathSegment);

            string result = selectedImage.Path;
            int cut = result.LastIndexOf('/');
            if (cut != -1)
            {
                result = result.Substring(cut + 1);
            }

            string actualPdfName = result;

            this.mView.UpdateAdapter(resultFilePath, fileName , actualPdfName);
            this.mView.HideLoadingImage();
          
        }

        public string removeRawtag(string data)
        {
            string removedString;

            removedString = data.Substring(4);

            return removedString;
        }

        public void OninfoLabelPermise()
        {

            this.mView.ShowinfoLabelPermise();
        }

        public void OnAttachPDF()
        {
            this.mView.ShowPDF();
        }



    }
}