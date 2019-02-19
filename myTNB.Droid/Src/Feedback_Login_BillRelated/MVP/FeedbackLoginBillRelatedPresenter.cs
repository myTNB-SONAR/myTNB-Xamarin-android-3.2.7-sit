using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Models;
using System.Threading;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Database.Model;
using Android.Text;
using System.Net;
using System.Net.Http;
using Refit;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.myTNBMenu.Models;
using Newtonsoft.Json;
using Java.Text;
using Android.Telephony;

namespace myTNB_Android.Src.Feedback_Login_BillRelated.MVP
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
                    Bundle extras = data.Extras;

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


                var request = new FeedbackRequest()
                {

                    Images = imageRequest,
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    FeedbackCategoryId = "1",
                    FeedbackTypeId = "",
                    PhoneNum = userEntity.MobileNo,
                    AccountNum = accountNum,
                    Name = userEntity.DisplayName,
                    Email = userEntity.Email,
                    DeviceId = deviceId,
                    FeedbackMessage = feedback,
                    StateId = "",
                    Location = "",
                    PoleNum = ""

                };



                var preLoginFeedbackResponse = await preloginFeedbackApi.SubmitFeedback(request, cts.Token);

                if (!preLoginFeedbackResponse.Data.IsError)
                {
                    SimpleDateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
                    var newSubmittedFeedback = new SubmittedFeedback()
                    {
                        FeedbackId = preLoginFeedbackResponse.Data.Data.FeedbackId,
                        DateCreated = dateFormat.Format(Java.Lang.JavaSystem.CurrentTimeMillis()),
                        FeedbackMessage = feedback,
                        FeedbackCategoryId = "1"

                    };

                    SubmittedFeedbackEntity.InsertOrReplace(newSubmittedFeedback);
                    this.mView.ClearInputFields();
                    CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetSelectedOrFirst();
                    this.mView.ShowSelectedAccount(customerBillingAccount);
                    this.mView.ShowSuccess(preLoginFeedbackResponse.Data.Data.DateCreated, preLoginFeedbackResponse.Data.Data.FeedbackId , attachedImages.Count);
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

        public void OnSelectAccount()
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

        public void CheckRequiredFields(string feedback)
        {

            this.mView.ClearErrors();
            if (!TextUtils.IsEmpty(feedback) && !feedback.Equals(" "))
            {
                this.mView.EnableSubmitButton();
            }
            else
            {
                //this.mView.ShowEmptyFeedbackError();
                this.mView.DisableSubmitButton();
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

            cts = new CancellationTokenSource();
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

                var request = new FeedbackRequest()
                {

                    Images = imageRequest,
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    FeedbackCategoryId = "1",
                    FeedbackTypeId = "",
                    PhoneNum = mobile_no,
                    AccountNum = accountNum,
                    Name = userEntity.DisplayName,
                    Email = userEntity.Email,
                    DeviceId = deviceId,
                    FeedbackMessage = feedback,
                    StateId = "",
                    Location = "",
                    PoleNum = ""

                };



                var preLoginFeedbackResponse = await preloginFeedbackApi.SubmitFeedback(request, cts.Token);

                if (!preLoginFeedbackResponse.Data.IsError)
                {
                    SimpleDateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
                    var newSubmittedFeedback = new SubmittedFeedback()
                    {
                        FeedbackId = preLoginFeedbackResponse.Data.Data.FeedbackId,
                        DateCreated = dateFormat.Format(Java.Lang.JavaSystem.CurrentTimeMillis()),
                        FeedbackMessage = feedback,
                        FeedbackCategoryId = "1"

                    };

                    SubmittedFeedbackEntity.InsertOrReplace(newSubmittedFeedback);
                    this.mView.ClearInputFields();
                    CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetSelectedOrFirst();
                    this.mView.ShowSelectedAccount(customerBillingAccount);
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

        public void CheckRequiredFields(string mobile_no, string feedback)
        {
            //if ( && !TextUtils.IsEmpty(feedback))


            //{

            this.mView.ClearErrors();

            if (TextUtils.IsEmpty(feedback) && feedback.Equals(" ")) {
                //this.mView.ShowEmptyFeedbackError();    
                this.mView.DisableSubmitButton();
                return;
            }


            if (TextUtils.IsEmpty(mobile_no)) {
                this.mView.ShowEmptyMobileNoError();
                this.mView.DisableSubmitButton();
                return;
            }

                if (!PhoneNumberUtils.IsGlobalPhoneNumber(mobile_no))
                {
                    this.mView.ShowInvalidMobileNoError();
                    return;
                }
                else
                {
                    this.mView.ClearMobileNoError();
                }


                if (!Utility.IsValidMobileNumber(mobile_no))
                {
                    this.mView.ShowInvalidMobileNoError();
                    return;
                }
                else
                {
                    this.mView.ClearMobileNoError();
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