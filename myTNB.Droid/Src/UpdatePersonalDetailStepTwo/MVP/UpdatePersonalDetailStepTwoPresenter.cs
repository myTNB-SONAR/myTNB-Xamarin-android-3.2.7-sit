using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.UpdatePersonalDetailStepTwo.Activity.UpdatePersonalDetailStepTwoActivity;

namespace myTNB_Android.Src.UpdatePersonalDetailStepTwo.MVP
{
    public class UpdatePersonalDetailStepTwoPresenter : UpdatePersonalDetailStepTwoContract.IUserActionsListener
    {   

        UpdatePersonalDetailStepTwoContract.IView mView;
        private ISharedPreferences mSharedPref;

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
                    string fileName = string.Format("{0}.jpeg", Guid.NewGuid());
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
                    string fileName = string.Format("{0}.jpeg", Guid.NewGuid());

                    OnSaveGalleryImage(selectedImage, fileName);
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
            //this.mView.EnableSubmitButton();
        }



    }
}