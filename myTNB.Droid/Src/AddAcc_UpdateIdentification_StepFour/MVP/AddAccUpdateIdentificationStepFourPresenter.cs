using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using myTNB.Android.Src.Utils;
using Android.Util;
using myTNB.Android.Src.Base.Models;
using Android.Telephony;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.Base.Request;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.MyTNBService.ServiceImpl;
using Java.Text;
using Android.Gms.Common.Apis;
using Newtonsoft.Json;
using myTNB.Android.Src.FeedbackGeneralEnquiryStepTwo.Model;
using Castle.Core.Internal;

namespace myTNB.Android.Src.AddAcc_UpdateIdentification_StepFour.MVP
{
    public class AddAccUpdateIdentificationStepFourPresenter : AddAccUpdateIdentificationStepFourContract.IUserActionsListener
    {

        AddAccUpdateIdentificationStepFourContract.IView mView;
        public AddAccUpdateIdentificationStepFourPresenter(AddAccUpdateIdentificationStepFourContract.IView mView)
        {

            this.mView = mView;
            this.mView.SetPresenter(this);

        }

        public void NavigateToTermsAndConditions()
        {
            this.mView.ShowNavigateToTermsAndConditions();
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

        public void CheckRequiredFields(string fullname, bool mobile_no, string email, bool tnc, bool isNeedTNC)
        {
            bool enableButton = true;

            try
            {

                if (!TextUtils.IsEmpty(fullname))
                {
                    this.mView.ClearFullNameError();
                    //if (!Utility.isAlphaNumeric(fullname))
                    //{
                    //    this.mView.ShowFullNameError();
                    //    enableButton = false;
                    //}
                    //else
                    //{
                    //    this.mView.ClearFullNameError();
                    //}
                }
                else
                {
                    this.mView.ShowFullNameError();
                    enableButton = false;
                }


                if (!TextUtils.IsEmpty(email))
                {
                    if (!Patterns.EmailAddress.Matcher(email).Matches())
                    {
                        this.mView.ShowInvalidEmailError();
                        enableButton = false;
                    }
                    else
                    {
                        this.mView.ClearInvalidEmailError();
                    }
                }


                if (isNeedTNC)
                {
                    if (tnc == false)
                    {
                        enableButton = false;
                    }
                }

                if (!mobile_no)
                {
                    enableButton = false;
                }

                //disable if 3 item is not filled

                if (TextUtils.IsEmpty(fullname) || TextUtils.IsEmpty(email) || !mobile_no)
                {
                    enableButton = false;
                }


                //disable or enable button
                if (enableButton == false)
                {
                    this.mView.DisableRegisterButton();
                }
                else
                {
                    this.mView.EnableRegisterButton();
                }

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public async void OnSubmit(string acc, string feedback, string fullname, string mobile_no, string email, List<AttachedImage> attachedImages, List<FeedbackUpdateDetailsModel> feedbackUpdateDetailsModelList, bool isowner, int ownerRelationship, string relationshipDescription)
        {


            //random checking
            if (!PhoneNumberUtils.IsGlobalPhoneNumber(mobile_no))
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
                    string filetype;
                    if (image.Name.ToLower().Contains(".pdf"))
                    {
                        filetype = ".pdf";
                    }
                    else
                    {
                        filetype = ".jpeg";
                    }

                    //image.Name = this.mView.GetImageName(ctr) + filetype;
                    var newImage = await this.mView.SaveImage(image);
                    imageRequest.Add(newImage);
                    ctr++;
                };

                int category = 1;

                if (feedback.IsNullOrEmpty())
                {
                    //if empty it is update personal detail which will inject 4
                    category = 4;
                }

                string devicePhoneNumber = userEntity != null ? userEntity.MobileNo : mobile_no;  //set device phone number


                SubmitEnquiryRequest submitEnquiryRequest = new SubmitEnquiryRequest(category.ToString(), "", acc, fullname, devicePhoneNumber, feedback, "", "", "", mobile_no, fullname, email, isowner, ownerRelationship, relationshipDescription);

                submitEnquiryRequest.email = userEntity != null ? userEntity.Email : email;  //set device email

                foreach (AttachedImageRequest image in imageRequest)
                {
                    string fileFormat;
                    if (image.FileName.ToLower().Contains("pdf"))
                    {
                        fileFormat = "pdf";
                    }
                    else
                    {
                        fileFormat = "jpeg";
                    }

                    submitEnquiryRequest.SetFeedbackImage(image.ImageHex, image.FileName, image.FileSize.ToString(), fileFormat);

                }

                if (feedbackUpdateDetailsModelList != null)
                {
                    foreach (FeedbackUpdateDetailsModel update in feedbackUpdateDetailsModelList)
                    {
                        submitEnquiryRequest.SetFeedbackUpdateDetails(update.FeedbackUpdInfoType, update.FeedbackUpdInfoTypeDesc, update.FeedbackUpdInfoValue);
                    }
                }

                var tempReq = JsonConvert.SerializeObject(submitEnquiryRequest);

                var preLoginFeedbackResponse = await ServiceApiImpl.Instance.SubmitEnquiry(submitEnquiryRequest);

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
                    // this.mView.ClearInputFields();
                    CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.GetSelectedOrFirst();
                    //this.mView.ShowSelectedAccount(customerBillingAccount);
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





    }
}