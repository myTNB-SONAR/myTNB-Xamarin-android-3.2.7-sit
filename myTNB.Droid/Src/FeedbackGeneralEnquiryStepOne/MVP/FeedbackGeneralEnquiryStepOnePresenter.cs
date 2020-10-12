﻿using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Text;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.FeedbackGeneralEnquiryStepOne.MVP
{
    public class FeedbackGeneralEnquiryStepOnePresenter : FeedbackGeneralEnquiryStepOneContract.IUserActionsListener
    {

        FeedbackGeneralEnquiryStepOneContract.IView mView;


        public FeedbackGeneralEnquiryStepOnePresenter(FeedbackGeneralEnquiryStepOneContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

       public  void OnGeneralEnquiry()
        {
            this.mView.ShowGeneralEnquiry();
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

        public void CheckRequiredFields(string feedback)
        {
            try
            {
                this.mView.ClearErrors();
                if (!TextUtils.IsEmpty(feedback.Trim()))
                {
                    this.mView.EnableSubmitButton();
                }
                else
                {
                  //  this.mView.ShowEmptyFeedbackError();
                    this.mView.DisableSubmitButton();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        public void OnAttachPhotoCamera()
        {
            this.mView.ShowCamera();
        }

        public void OnAttachPhotoGallery()
        {
            this.mView.ShowGallery();
        }

        public void OnAttachPDF()
        {
            this.mView.ShowPDF();
        }

        private async void OnSaveCameraImage(string tempImagePath, string fileName)
        {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();
            string resultFilePath = await this.mView.SaveCameraImage(tempImagePath, fileName);
            this.mView.UpdateAdapter(resultFilePath, fileName);
            this.mView.HideLoadingImage();
            this.mView.EnableSubmitButton();
        }


        private async void OnSaveGalleryImage(Android.Net.Uri selectedImage, string fileName)
        {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();
            string resultFilePath = await this.mView.SaveGalleryImage(selectedImage, FileUtils.TEMP_IMAGE_FOLDER, fileName);
          

            this.mView.UpdateAdapter(resultFilePath, fileName);
            this.mView.HideLoadingImage();
            this.mView.EnableSubmitButton();
        }

        private async void OnSaveGalleryPDF(Android.Net.Uri selectedImage, string fileName)
        {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();
            //  string resultFilePath = await this.mView.SaveGalleryImage(selectedImage, FileUtils.TEMP_IMAGE_FOLDER, fileName);
            string resultFilePath = removeRawtag(selectedImage.LastPathSegment);
           // string cutRawtag = resultFilePath.Substring(4, resultFilePath.Length);



            this.mView.UpdateAdapter(resultFilePath, fileName);
            this.mView.HideLoadingImage();
            this.mView.EnableSubmitButton();
        }

        public string removeRawtag(string data)
        { string removedString;

            removedString = data.Substring(4);

            return removedString;
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
            else if (requestCode == Constants.RUNTIME_PERMISSION_GALLERY_PDF_REQUEST_CODE)
            {
                if (resultCode == Result.Ok)
                {  
                  
                    Android.Net.Uri selectedImage = data.Data;
                    string fileName = string.Format("{0}.pdf", Guid.NewGuid());

                    OnSaveGalleryPDF(selectedImage, fileName);
                    GC.Collect();
                }
            }

        }







    }
}