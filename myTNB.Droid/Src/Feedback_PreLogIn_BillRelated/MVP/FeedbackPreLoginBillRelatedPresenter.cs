using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Java.Text;
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

namespace myTNB.AndroidApp.Src.Feedback_PreLogIn_BillRelated.MVP
{
    public class FeedbackPreLoginBillRelatedPresenter : FeedbackPreLoginBillRelatedContract.IUserActionsListener
    {
        private FeedbackPreLoginBillRelatedContract.IView mView;
        CancellationTokenSource cts;

        public FeedbackPreLoginBillRelatedPresenter(FeedbackPreLoginBillRelatedContract.IView mView)
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
            }
            catch (System.Exception e)
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
            catch (System.Exception e)
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
            catch (System.Exception e)
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

        public void Start()
        {
            // TODO : IMPL
            this.mView.DisableSubmitButton();
        }

        public async void OnSubmit(string deviceId, string fullname, string mobile_no, string email, string account_no, string feedback, List<AttachedImage> attachedImages)
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

            if (TextUtils.IsEmpty(account_no))
            {
                this.mView.ShowEmptyAccountNoError();
                return;
            }

            if (!TextUtils.IsDigitsOnly(account_no))
            {
                this.mView.ShowInvalidAccountNoError();
                return;
            }

            if (!Utility.AccountNumberValidation(account_no.Length))
            {
                this.mView.ShowInvalidAccountNoError();
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

                SubmitFeedbackRequest submitFeedbackRequest = new SubmitFeedbackRequest("1", "", account_no, fullname, mobile_no, feedback, "", "", "");
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
                        FeedbackCategoryId = "1"

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

        public string OnVerfiyCellularCode(string mobileNo)
        {
            try
            {
                if (TextUtils.IsEmpty(mobileNo) || mobileNo.Length < 3 || !mobileNo.Contains("+60"))
                {
                    mobileNo = "+60";
                    this.mView.ClearErrors();
                    //this.mView.DisableSubmitButton();
                }
                else if (mobileNo == "+60")
                {
                    this.mView.ClearErrors();
                    //this.mView.DisableSubmitButton();
                }
                else if (mobileNo.Contains("+60") && mobileNo.IndexOf("+60") > 0)
                {
                    mobileNo = mobileNo.Substring(mobileNo.IndexOf("+60"));
                    if (mobileNo == "+60")
                    {
                        this.mView.ClearErrors();
                        //this.mView.DisableSubmitButton();
                    }
                    else if (!Utility.IsValidMobileNumber(mobileNo))
                    {
                        this.mView.ShowInvalidMobileNoError();
                        //this.mView.DisableSubmitButton();
                    }
                    else
                    {
                        this.mView.ClearErrors();
                        //this.mView.EnableSubmitButton();
                    }
                }
                else
                {
                    if (!Utility.IsValidMobileNumber(mobileNo))
                    {
                        this.mView.ShowInvalidMobileNoError();
                        //this.mView.DisableSubmitButton();
                    }
                    else
                    {
                        this.mView.ClearErrors();
                        //this.mView.EnableSubmitButton();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return mobileNo;
        }

        public void CheckRequiredFields(string fullname, string mobile_no, string email, string account_no, string feedback)
        {
            try
            {
                if (!TextUtils.IsEmpty(fullname) && !TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(email) && !TextUtils.IsEmpty(account_no) && !TextUtils.IsEmpty(feedback))
                {
                    this.mView.ClearErrors();
                    bool isError = false;
                    if (!TextUtils.IsEmpty(fullname))
                    {
                        if (!Utility.isAlphaNumeric(fullname))
                        {

                            this.mView.ShowNameError();
                            isError = true;
                        }
                    }
                    else
                    {
                        this.mView.ShowEmptyFullnameError();
                        isError = true;
                    }

                    if (!TextUtils.IsEmpty(email))
                    {
                        if (!Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
                        {

                            this.mView.ShowInvalidEmailError();
                            isError = true;
                        }
                    }
                    else
                    {
                        isError = true;
                    }

                    if (!TextUtils.IsEmpty(account_no))
                    {
                        if (!TextUtils.IsDigitsOnly(account_no))
                        {
                            this.mView.ShowInvalidAccountNoError();
                            isError = true;
                        }

                        if (account_no.Length > 12)
                        {
                            string account_no_limit = "";
                            for (int i = 0; i < 12; i++)
                            {
                                account_no_limit += account_no[i];
                            }

                            this.mView.ReplaceAccountNum(account_no_limit);
                        }
                        else if (!Utility.AccountNumberValidation(account_no.Length))
                        {
                            this.mView.ShowInvalidAccountNoError();
                            isError = true;
                        }
                    }
                    else
                    {
                        isError = true;
                    }

                    if (TextUtils.IsEmpty(feedback) || feedback.Equals(" "))
                    {
                        //this.mView.DisableSubmitButton();
                        ////this.mView.ShowEmptyFeedbackError();
                        //return;
                        isError = true;
                    }
                    if (isError)
                    {
                        this.mView.DisableSubmitButton();
                    }
                    else
                    {
                        this.mView.EnableSubmitButton();

                    }
                }
                else
                {
                    this.mView.DisableSubmitButton();
                }
            }
            catch (System.Exception e)
            {
                this.mView.DisableSubmitButton();
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
