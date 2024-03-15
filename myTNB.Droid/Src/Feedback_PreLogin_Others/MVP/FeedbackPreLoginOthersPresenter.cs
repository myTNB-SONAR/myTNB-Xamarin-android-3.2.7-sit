using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Java.Text;
using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.AndroidApp.Src.Base.Api;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Base.Request;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB.AndroidApp.Src.Feedback_PreLogin_Others.MVP
{
    public class FeedbackPreLoginOthersPresenter : FeedbackPreLoginOthersContract.IUserActionsListener
    {

        FeedbackPreLoginOthersContract.IView mView;
        CancellationTokenSource cts;

        public FeedbackPreLoginOthersPresenter(FeedbackPreLoginOthersContract.IView mView)
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


        public async void OnSubmit(string deviceId, string fullname, string mobile_no, string email, FeedbackType feedbackType, string feedback, List<AttachedImage> attachedImages)
        {
            this.mView.ClearErrors();
            if (TextUtils.IsEmpty(fullname))
            {
                this.mView.ShowEmptyFullnameError();
                return;
            }

            if (!Utility.isAlphaNumeric(fullname))
            {
                this.mView.ShowNameError();
                return;
            }

            if (TextUtils.IsEmpty(email))
            {
                this.mView.ShowEmptyEmaiError();
                return;
            }

            if (!Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
            {
                this.mView.ShowInvalidEmailError();
                return;
            }

            if (TextUtils.IsEmpty(mobile_no))
            {
                this.mView.ShowEmptyMobileNoError();
                return;
            }

            if (!Android.Util.Patterns.Phone.Matcher(mobile_no).Matches())
            {
                this.mView.ShowInvalidMobileNoError();
                return;
            }

            if (!Utility.IsValidMobileNumber(mobile_no))
            {
                this.mView.ShowInvalidMobileNoError();
                return;
            }


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
                List<AttachedImageRequest> imageRequest = new List<AttachedImageRequest>();
                int ctr = 1;
                foreach (AttachedImage image in attachedImages)
                {
                    image.Name = this.mView.GetImageName(ctr) + ".jpeg";
                    var newImage = await this.mView.SaveImage(image);
                    imageRequest.Add(newImage);
                    ctr++;
                }

                SubmitFeedbackRequest submitFeedbackRequest = new SubmitFeedbackRequest("3", feedbackType.FeedbackTypeId, "", fullname, mobile_no, feedback, "", "", "");
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
                    this.mView.OnSubmitError(preLoginFeedbackResponse.Response.DisplayMessage);
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

        public void CheckRequiredFields(string fullname, string mobile_no, string email, string feedback)
        {
            //if (!TextUtils.IsEmpty(fullname) && !TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(email)  && !TextUtils.IsEmpty(feedback))
            //{
            try
            {
                this.mView.ClearErrors();
                if (TextUtils.IsEmpty(fullname))
                {
                    this.mView.ShowEmptyFullnameError();
                    this.mView.DisableSubmitButton();
                    return;
                }

                if (!Utility.isAlphaNumeric(fullname))
                {
                    this.mView.ShowNameError();
                    this.mView.DisableSubmitButton();
                    return;
                }


                if (TextUtils.IsEmpty(mobile_no))
                {
                    this.mView.ShowEmptyMobileNoError();
                    this.mView.DisableSubmitButton();
                    return;
                }

                if (!Android.Util.Patterns.Phone.Matcher(mobile_no).Matches())
                {
                    this.mView.ShowInvalidMobileNoError();
                    this.mView.DisableSubmitButton();
                    return;
                }

                if (!Utility.IsValidMobileNumber(mobile_no))
                {
                    this.mView.ShowInvalidMobileNoError();
                    this.mView.DisableSubmitButton();
                    return;
                }

                if (TextUtils.IsEmpty(email))
                {
                    this.mView.ShowEmptyEmaiError();
                    this.mView.DisableSubmitButton();
                    return;
                }

                if (!Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
                {
                    this.mView.ShowInvalidEmailError();
                    this.mView.DisableSubmitButton();
                    return;
                }




                if (TextUtils.IsEmpty(feedback) && feedback.Equals(" "))
                {
                    //this.mView.ShowEmptyFeedbackError();
                    this.mView.DisableSubmitButton();
                    return;
                }


                this.mView.EnableSubmitButton();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            //}
            //else
            //{
            //    this.mView.DisableSubmitButton();
            //}
        }
    }
}
