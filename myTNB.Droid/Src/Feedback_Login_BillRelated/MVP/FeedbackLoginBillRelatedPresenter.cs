using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Telephony;
using Android.Text;
using Java.Text;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Base.Request;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;

namespace myTNB.AndroidApp.Src.Feedback_Login_BillRelated.MVP
{
    public class FeedbackLoginBillRelatedPresenter : FeedbackLoginBillRelatedContract.IUserActionsListener
    {

        FeedbackLoginBillRelatedContract.IView mView;
        CancellationTokenSource cts;

        CustomerBillingAccount selectedCustomerBillingAccount;

        public FeedbackLoginBillRelatedPresenter(FeedbackLoginBillRelatedContract.IView mView)
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
            else if (requestCode == Constants.SELECT_ACCOUNT_REQUEST_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    Android.OS.Bundle extras = data.Extras;

                    AccountData selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    selectedCustomerBillingAccount = CustomerBillingAccount.FindByAccNum(selectedAccount.AccountNum);
                    this.mView.ShowSelectedAccount(selectedCustomerBillingAccount);
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

        public async void OnSubmit(string deviceId, string accountNum, string feedback, List<AttachedImage> attachedImages)
        {
            this.mView.ClearErrors();

            if (TextUtils.IsEmpty(feedback))
            {
                this.mView.ShowEmptyFeedbackError();
                return;
            }


            cts = new CancellationTokenSource();

            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }

            try
            {
                UserEntity userEntity = new UserEntity();
                userEntity = UserEntity.GetActive();
                List<AttachedImageRequest> imageRequest = new List<AttachedImageRequest>();
                int ctr = 1;
                foreach (AttachedImage image in attachedImages)
                {
                    image.Name = this.mView.GetImageName(ctr) + ".jpeg";
                    var newImage = await this.mView.SaveImage(image);
                    imageRequest.Add(newImage);
                    ctr++;
                }

                SubmitFeedbackRequest submitFeedbackRequest = new SubmitFeedbackRequest("1","", accountNum, userEntity.DisplayName, userEntity.MobileNo, feedback,"","","");
                foreach (AttachedImageRequest image in imageRequest)
                {
                    submitFeedbackRequest.SetFeedbackImage(image.ImageHex, image.FileName, image.FileSize.ToString());
                }

                var preLoginFeedbackResponse = await ServiceApiImpl.Instance.SubmitFeedback(submitFeedbackRequest);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (preLoginFeedbackResponse.Response != null && preLoginFeedbackResponse.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
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
                    CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetSelectedOrFirst();
                    this.mView.ShowSelectedAccount(customerBillingAccount);
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
                // TODO: REPLACE WITH THE FIRST
                this.mView.DisableSubmitButton();
                if (selectedCustomerBillingAccount != null)
                {
                    this.mView.ShowSelectedAccount(selectedCustomerBillingAccount);
                }
                else
                {
                    CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetSelectedOrFirst();
                    if (customerBillingAccount != null)
                    {
                        this.mView.ShowSelectedAccount(customerBillingAccount);
                    }

                }

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

        public void OnSelectAccount()
        {
            try
            {
                if (CustomerBillingAccount.HasItems())
                {
                    if (selectedCustomerBillingAccount != null)
                    {
                        this.mView.ShowSelectAccount(AccountData.Copy(selectedCustomerBillingAccount, true));
                    }
                    else
                    {
                        CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetFirst();
                        this.mView.ShowSelectAccount(AccountData.Copy(customerBillingAccount, true));
                    }

                }
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
                    //this.mView.ShowEmptyFeedbackError();
                    this.mView.DisableSubmitButton();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void OnSubmit(string deviceId, string mobile_no, string accountNum, string feedback, List<AttachedImage> attachedImages)
        {
            this.mView.ClearErrors();

            if (TextUtils.IsEmpty(feedback) && feedback.Equals(" "))
            {
                this.mView.ShowEmptyFeedbackError();
                return;
            }

            if (TextUtils.IsEmpty(mobile_no))
            {
                this.mView.ShowEmptyMobileNoError();
                return;
            }


            if (!PhoneNumberUtils.IsGlobalPhoneNumber(mobile_no))
            {
                this.mView.ShowInvalidMobileNoError();
                return;
            }


            if (!Utility.IsValidMobileNumber(mobile_no))
            {
                this.mView.ShowInvalidMobileNoError();
                return;
            }

            if (!Utility.AccountNumberValidation(accountNum.Length))
            {
                this.mView.showInvalidAccountNumberError();
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

                SubmitFeedbackRequest submitFeedbackRequest = new SubmitFeedbackRequest("1", "", accountNum, userEntity.DisplayName, mobile_no, feedback, "", "", "");
                foreach (AttachedImageRequest image in imageRequest)
                {
                    submitFeedbackRequest.SetFeedbackImage(image.ImageHex, image.FileName, image.FileSize.ToString());
                }

                var preLoginFeedbackResponse = await ServiceApiImpl.Instance.SubmitFeedback(submitFeedbackRequest);

                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }

                if (preLoginFeedbackResponse.Response != null && preLoginFeedbackResponse.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
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
                    CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetSelectedOrFirst();
                    this.mView.ShowSelectedAccount(customerBillingAccount);
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

        public void CheckRequiredFields(string mobile_no, string feedback)
        {
            try
            {
                this.mView.ClearErrors();
                if (TextUtils.IsEmpty(mobile_no) || TextUtils.IsEmpty(feedback.Trim()))
                {
                    this.mView.DisableSubmitButton();
                    return;
                }
                this.mView.EnableSubmitButton();
            }
            catch (Exception e)
            {
                this.mView.DisableSubmitButton();
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
