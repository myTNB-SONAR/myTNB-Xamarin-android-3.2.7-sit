﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Utils;
using System.Threading;
using Android.Text;
using System.Net;
using System.Net.Http;
using Refit;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Request;
using Newtonsoft.Json;
using myTNB_Android.Src.Database.Model;
using Java.Text;

namespace myTNB_Android.Src.Feedback_PreLogin_Others.MVP
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
            cts = new CancellationTokenSource();
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

            this.mView.ShowProgressDialog();
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var preloginFeedbackApi = RestService.For<IFeedbackApi>(httpClient);
#else
            var preloginFeedbackApi = RestService.For<IFeedbackApi>(Constants.SERVER_URL.END_POINT);
#endif
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

                var request = new FeedbackRequest()
                {

                    Images = imageRequest,
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    FeedbackCategoryId = "3",
                    FeedbackTypeId = feedbackType.FeedbackTypeId,
                    PhoneNum = mobile_no,
                    AccountNum = "",
                    Name = fullname,
                    Email = email,
                    DeviceId = deviceId,
                    FeedbackMessage = feedback,
                    StateId = "",
                    Location = "",
                    PoleNum = ""

                };
                Console.WriteLine("Request => " + request);
                Console.WriteLine("Serialized Request => " + JsonConvert.SerializeObject(request));


                var preLoginFeedbackResponse = await preloginFeedbackApi.SubmitFeedback(request, cts.Token);

                if (!preLoginFeedbackResponse.Data.IsError)
                {
                    SimpleDateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
                    var newSubmittedFeedback = new SubmittedFeedback()
                    {
                        FeedbackId = preLoginFeedbackResponse.Data.Data.FeedbackId,
                        DateCreated = dateFormat.Format(Java.Lang.JavaSystem.CurrentTimeMillis()),
                        FeedbackMessage = feedback,
                        FeedbackCategoryId = "3"

                    };

                    SubmittedFeedbackEntity.InsertOrReplace(newSubmittedFeedback);
                    this.mView.ClearInputFields();
                    this.mView.ShowSuccess(preLoginFeedbackResponse.Data.Data.DateCreated, preLoginFeedbackResponse.Data.Data.FeedbackId, attachedImages.Count);
                }
                else
                {
                    //this.mView.ShowFail();
                    this.mView.OnSubmitError(preLoginFeedbackResponse.Data.Message);
                }

            }

            catch (System.OperationCanceledException e)
            {
                //this.mView.ShowFail();
                this.mView.OnSubmitError();

            }
            catch (ApiException apiException)
            {
                //this.mView.ShowFail();
                this.mView.OnSubmitError();
            }
            catch (Exception e)
            {
                //this.mView.ShowFail();
                this.mView.OnSubmitError();
            }

            this.mView.HideProgressDialog();

        }

        public void Start()
        {
            // TODO: IMPL SHOW FEEDBACK TYPE
            this.mView.DisableSubmitButton();
            FeedbackTypeEntity.ResetSelected();
            FeedbackTypeEntity entity = FeedbackTypeEntity.GetFirstOrSelected();
            FeedbackTypeEntity.SetSelected(entity.Id);
            this.mView.ShowSelectedFeedbackType(FeedbackType.Copy(entity));
        }

        public void OnSelectFeedbackType()
        {
            this.mView.ShowSelectFeedbackType();
        }

        public void  CheckRequiredFields(string fullname, string mobile_no, string email, string feedback)
        {
            //if (!TextUtils.IsEmpty(fullname) && !TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(email)  && !TextUtils.IsEmpty(feedback))
            //{

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
            //}
            //else
            //{
            //    this.mView.DisableSubmitButton();
            //}
        }
    }
}