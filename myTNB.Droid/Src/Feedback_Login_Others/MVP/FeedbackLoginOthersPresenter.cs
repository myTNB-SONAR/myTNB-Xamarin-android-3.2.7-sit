﻿using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Telephony;
using Android.Text;
using Java.Text;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.Feedback_Login_Others.MVP
{
    public class FeedbackLoginOthersPresenter : FeedbackLoginOthersContract.IUserActionsListener
    {

        FeedbackLoginOthersContract.IView mView;
        CancellationTokenSource cts;

        public FeedbackLoginOthersPresenter(FeedbackLoginOthersContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
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
                else if (requestCode == Constants.SELECT_FEEDBACK_TYPE)
                {
                    if (resultCode == Result.Ok)
                    {
                        FeedbackType selectedType = JsonConvert.DeserializeObject<FeedbackType>(data.Extras.GetString(Constants.SELECTED_FEEDBACK_TYPE));
                        this.mView.ShowSelectedFeedbackType(selectedType);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void OnSaveCameraImage(string tempImagePath, string fileName)
        {
            try
            {
                this.mView.DisableSubmitButton();
                this.mView.ShowLoadingImage();
                string resultFilePath = await this.mView.SaveCameraImage(tempImagePath, fileName);
                this.mView.UpdateAdapter(resultFilePath, fileName);
                this.mView.HideLoadingImage();
                this.mView.EnableSubmitButton();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private async void OnSaveGalleryImage(Android.Net.Uri selectedImage, string fileName)
        {
            try
            {
                this.mView.DisableSubmitButton();
                this.mView.ShowLoadingImage();
                string resultFilePath = await this.mView.SaveGalleryImage(selectedImage, FileUtils.TEMP_IMAGE_FOLDER, fileName);
                this.mView.UpdateAdapter(resultFilePath, fileName);
                this.mView.HideLoadingImage();
                this.mView.EnableSubmitButton();
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


        public async void OnSubmit(string deviceId, FeedbackType feedbackType, string feedback, List<AttachedImage> attachedImages)
        {
            this.mView.ClearErrors();

            if (TextUtils.IsEmpty(feedback) && feedback.Equals(" "))
            {
                this.mView.ShowEmptyFeedbackError();
                return;
            }

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {
                UserEntity userEntity = UserEntity.GetActive();
                List<AttachedImageRequest> imageRequest = new List<AttachedImageRequest>();
                int ctr = 1;
                foreach (AttachedImage image in attachedImages)
                {
                    image.Name = this.mView.GetImageName(ctr) + ".jpeg";
                    var newImage = await this.mView.SaveImage(image);
                    imageRequest.Add(newImage);
                    ctr++;
                }

                SubmitFeedbackRequest submitFeedbackRequest = new SubmitFeedbackRequest("3", feedbackType.FeedbackTypeId, "", userEntity.DisplayName, userEntity.MobileNo, feedback, "", "", "");
                foreach (AttachedImageRequest image in imageRequest)
                {
                    submitFeedbackRequest.SetFeedbackImage(image.ImageHex, image.FileName, image.FileSize.ToString());
                }

                var preLoginFeedbackResponse = await ServiceApiImpl.Instance.SubmitFeedback(submitFeedbackRequest);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (preLoginFeedbackResponse.IsSuccessResponse())
                {
                    SimpleDateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
                    var newSubmittedFeedback = new SubmittedFeedback()
                    {
                        FeedbackId = preLoginFeedbackResponse.GetData().ServiceReqNo,
                        DateCreated = dateFormat.Format(Java.Lang.JavaSystem.CurrentTimeMillis()),
                        FeedbackMessage = feedback,
                        FeedbackCategoryId = "3"

                    };

                    SubmittedFeedbackEntity.InsertOrReplace(newSubmittedFeedback);
                    this.mView.ClearInputFields();
                    this.mView.ShowSuccess(preLoginFeedbackResponse.GetData().DateCreated, preLoginFeedbackResponse.GetData().ServiceReqNo, attachedImages.Count);
                }
                else
                {
                    //this.mView.ShowFail();
                    this.mView.OnSubmitError(preLoginFeedbackResponse.Response.Message);
                }

            }

            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                //this.mView.ShowFail();
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                //this.mView.ShowFail();
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                //this.mView.ShowFail();
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(e);
            }

        }

        public void Start()
        {
            try
            {
                // TODO: IMPL SHOW FEEDBACK TYPE
                this.mView.DisableSubmitButton();
                FeedbackTypeEntity.ResetSelected();
                FeedbackTypeEntity entity = FeedbackTypeEntity.GetFirstOrSelected();
                FeedbackTypeEntity.SetSelected(entity.Id);
                this.mView.ShowSelectedFeedbackType(FeedbackType.Copy(entity));

                UserEntity userEntity = UserEntity.GetActive();
                if (TextUtils.IsEmpty(userEntity.MobileNo))
                {
                    this.mView.ShowMobileNo();
                }
                else
                {
                    this.mView.HideMobileNo();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSelectFeedbackType()
        {
            this.mView.ShowSelectFeedbackType();
        }

        public void CheckRequiredFields(string feedback)
        {
            this.mView.ClearErrors();
            if (!TextUtils.IsEmpty(feedback) && !feedback.Equals(" "))
            {
                this.mView.EnableSubmitButton();
            }
            else
            {
                this.mView.ShowEmptyFeedbackError();
                this.mView.DisableSubmitButton();
            }
        }

        public async void OnSubmit(string deviceId, string mobile_no, FeedbackType feedbackType, string feedback, List<AttachedImage> attachedImages)
        {
            this.mView.ClearErrors();

            if (TextUtils.IsEmpty(feedback))
            {
                this.mView.ShowEmptyFeedbackError();
                return;
            }


            if (!Utility.IsValidMobileNumber(mobile_no))
            {
                this.mView.ShowInvalidMobileNoError();
                return;
            }

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {
                UserEntity userEntity = UserEntity.GetActive();
                List<AttachedImageRequest> imageRequest = new List<AttachedImageRequest>();
                int ctr = 1;
                foreach (AttachedImage image in attachedImages)
                {
                    image.Name = this.mView.GetImageName(ctr) + ".jpeg";
                    var newImage = await this.mView.SaveImage(image);
                    imageRequest.Add(newImage);
                    ctr++;
                }

                SubmitFeedbackRequest submitFeedbackRequest = new SubmitFeedbackRequest("3", feedbackType.FeedbackTypeId, "", userEntity.DisplayName, mobile_no, feedback, "", "", "");
                foreach (AttachedImageRequest image in imageRequest)
                {
                    submitFeedbackRequest.SetFeedbackImage(image.ImageHex, image.FileName, image.FileSize.ToString());
                }

                var preLoginFeedbackResponse = await ServiceApiImpl.Instance.SubmitFeedback(submitFeedbackRequest);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (preLoginFeedbackResponse.IsSuccessResponse())
                {
                    SimpleDateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
                    var newSubmittedFeedback = new SubmittedFeedback()
                    {
                        FeedbackId = preLoginFeedbackResponse.GetData().ServiceReqNo,
                        DateCreated = dateFormat.Format(Java.Lang.JavaSystem.CurrentTimeMillis()),
                        FeedbackMessage = feedback,
                        FeedbackCategoryId = "3"

                    };

                    SubmittedFeedbackEntity.InsertOrReplace(newSubmittedFeedback);
                    this.mView.ClearInputFields();
                    this.mView.ShowSuccess(preLoginFeedbackResponse.GetData().DateCreated, preLoginFeedbackResponse.GetData().ServiceReqNo, attachedImages.Count);
                }
                else
                {
                    this.mView.OnSubmitError(preLoginFeedbackResponse.Response.Message);
                }

            }

            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                //this.mView.ShowFail();
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                //this.mView.ShowFail();
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                //this.mView.ShowFail();
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(e);
            }

        }

        public void CheckRequiredFields(string mobile_no, string feedback)
        {
            //if (!TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(feedback))
            //{

            this.mView.ClearErrors();

            if (TextUtils.IsEmpty(mobile_no))
            {
                this.mView.DisableSubmitButton();
                this.mView.ShowEmptyMobileNoError();
                return;
            }

            if (!PhoneNumberUtils.IsGlobalPhoneNumber(mobile_no))
            {
                this.mView.DisableSubmitButton();
                this.mView.ShowInvalidMobileNoError();
                return;
            }
            else
            {
                this.mView.ClearMobileNoError();
            }


            if (!Utility.IsValidMobileNumber(mobile_no))
            {
                this.mView.DisableSubmitButton();
                this.mView.ShowInvalidMobileNoError();
                return;
            }
            else
            {
                this.mView.ClearMobileNoError();
            }


            if (TextUtils.IsEmpty(feedback) && feedback.Equals(" "))
            {
                this.mView.DisableSubmitButton();
                this.mView.ShowEmptyFeedbackError();
                return;
            }

            this.mView.EnableSubmitButton();
            //}
            //else
            //{
            //    this.mView.DisableSubmitButton();
            //}
        }
    }
}