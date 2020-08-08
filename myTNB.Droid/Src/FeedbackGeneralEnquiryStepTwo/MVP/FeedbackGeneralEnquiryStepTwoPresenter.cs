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
using myTNB_Android.Src.Utils;
using Android.Util;
using myTNB_Android.Src.Base.Models;
using Android.Telephony;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using Java.Text;
using Android.Gms.Common.Apis;
using Newtonsoft.Json;
using myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.Model;
using Castle.Core.Internal;

namespace myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.MVP
{
    public class FeedbackGeneralEnquiryStepTwoPresenter : FeedbackGeneralEnquiryStepTwoContract.IUserActionsListener
    {

        FeedbackGeneralEnquiryStepTwoContract.IView mView;
        public FeedbackGeneralEnquiryStepTwoPresenter(FeedbackGeneralEnquiryStepTwoContract.IView mView)
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

        public void CheckRequiredFields(string fullname, string mobile_no, string email, bool tnc ,bool isNeedTNC)
        {

            try
            {
                if (!TextUtils.IsEmpty(fullname)  && !TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(email) )
                {

                    if (!Utility.isAlphaNumeric(fullname))
                    {
                        this.mView.ShowFullNameError();
                        this.mView.DisableRegisterButton();
                    }
                    else
                    {
                        this.mView.ClearFullNameError();
                    }


                    if (!Patterns.EmailAddress.Matcher(email).Matches())
                    {
                        this.mView.ShowInvalidEmailError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else
                    {
                        this.mView.ClearInvalidEmailError();
                    }

                    if (TextUtils.IsEmpty(mobile_no) || mobile_no.Length < 3 || !mobile_no.Contains("+60"))
                    {
                        this.mView.UpdateMobileNumber("+60");
                        this.mView.ClearInvalidMobileError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else if (mobile_no == "+60")
                    {
                        this.mView.UpdateMobileNumber("+60");
                        this.mView.ClearInvalidMobileError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else if (mobile_no.Contains("+60") && mobile_no.IndexOf("+60") > 0)
                    {
                        mobile_no = mobile_no.Substring(mobile_no.IndexOf("+60"));
                        if (mobile_no == "+60")
                        {
                            this.mView.UpdateMobileNumber("+60");
                            this.mView.ClearInvalidMobileError();
                            this.mView.DisableRegisterButton();
                            return;
                        }
                        else if (!Utility.IsValidMobileNumber(mobile_no))
                        {
                            this.mView.ShowInvalidMobileNoError();
                            this.mView.DisableRegisterButton();
                            return;
                        }
                        else
                        {
                            this.mView.ClearInvalidMobileError();
                        }
                    }
                    else
                    {
                        if (!Utility.IsValidMobileNumber(mobile_no))
                        {
                            this.mView.ShowInvalidMobileNoError();
                            this.mView.DisableRegisterButton();
                            return;
                        }
                        else
                        {
                            this.mView.ClearInvalidMobileError();
                        }
                    }

                    if (isNeedTNC)
                    {
                        if (tnc == false)
                        {
                            this.mView.DisableRegisterButton();
                            return;
                        }
                    }


                    this.mView.EnableRegisterButton();
                }
                else
                {

                    if (TextUtils.IsEmpty(mobile_no) || mobile_no.Length < 3 || !mobile_no.Contains("+60"))
                    {
                        this.mView.UpdateMobileNumber("+60");
                        this.mView.ClearInvalidMobileError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else if (mobile_no == "+60")
                    {
                        this.mView.UpdateMobileNumber("+60");
                        this.mView.ClearInvalidMobileError();
                        this.mView.DisableRegisterButton();
                        return;
                    }
                    else if (mobile_no.Contains("+60") && mobile_no.IndexOf("+60") > 0)
                    {
                        mobile_no = mobile_no.Substring(mobile_no.IndexOf("+60"));
                        if (mobile_no == "+60")
                        {
                            this.mView.UpdateMobileNumber("+60");
                            this.mView.ClearInvalidMobileError();
                            this.mView.DisableRegisterButton();
                            return;
                        }
                        else if (!Utility.IsValidMobileNumber(mobile_no))
                        {
                            this.mView.ShowInvalidMobileNoError();
                            this.mView.DisableRegisterButton();
                            return;
                        }
                        else
                        {
                            this.mView.ClearInvalidMobileError();
                        }
                    }
                    else
                    {
                        if (!Utility.IsValidMobileNumber(mobile_no))
                        {
                            this.mView.ShowInvalidMobileNoError();
                            this.mView.DisableRegisterButton();
                            return;
                        }
                        else
                        {
                            this.mView.ClearInvalidMobileError();
                        }
                    }


                    this.mView.DisableRegisterButton();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public async void OnSubmit(string acc , string feedback, string fullname, string mobile_no,string email, List<AttachedImage> attachedImages , List<FeedbackUpdateDetailsModel> feedbackUpdateDetailsModelList , bool isowner, int ownerRelationship , string relationshipDescription)
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
                    image.Name = this.mView.GetImageName(ctr) + ".jpeg";
                    var newImage = await this.mView.SaveImage(image);
                    imageRequest.Add(newImage);
                    ctr++;
                } ;

                int category=1;

                if (feedback.IsNullOrEmpty())
                {
                    //if empty it is update personal detail which will inject 4
                    category = 4;
                }
           

                SubmitEnquiryRequest submitEnquiryRequest  = new SubmitEnquiryRequest(category.ToString(), "", acc, fullname, mobile_no, feedback, "", "", "", mobile_no, fullname, email, isowner, ownerRelationship, relationshipDescription);
                foreach (AttachedImageRequest image in imageRequest)
                {

                        string fileFormat = "jpeg";
                        submitEnquiryRequest.SetFeedbackImage(image.ImageHex, image.FileName, image.FileSize.ToString(), fileFormat);

                }

                if (feedbackUpdateDetailsModelList != null)
                {
                    foreach (FeedbackUpdateDetailsModel update in feedbackUpdateDetailsModelList)
                    {
                        submitEnquiryRequest.SetFeedbackUpdateDetails(update.FeedbackUpdInfoType, update.FeedbackUpdInfoTypeDesc, update.FeedbackUpdInfoValue);
                    }
                }



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