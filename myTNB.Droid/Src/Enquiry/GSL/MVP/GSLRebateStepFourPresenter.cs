using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Base.Request;
using myTNB.Android.Src.Common.Model;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.MyTNBService.ServiceImpl;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using Java.Text;
using System.Globalization;
using Android.Gms.Common.Apis;

namespace myTNB.Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateStepFourPresenter : GSLRebateStepFourContract.IUserActionsListener
    {
        private readonly GSLRebateStepFourContract.IView view;

        private GSLRebateModel rebateModel;

        private bool tncAccepted;

        public GSLRebateStepFourPresenter(GSLRebateStepFourContract.IView view)
        {
            this.view = view;
            this.view?.SetPresenter(this);
        }

        public void OnInitialize()
        {
            OnInit();
            this.view?.UpdateButtonState(false);
        }

        private void OnInit()
        {
            this.tncAccepted = false;
            this.rebateModel = new GSLRebateModel();
        }

        public void Start() { }

        public void SetRebateModel(GSLRebateModel model)
        {
            this.rebateModel = model;
            this.rebateModel.ContactInfo = new GSLRebateAccountInfoModel();
        }

        public GSLRebateModel GetGSLRebateModel()
        {
            return rebateModel;
        }

        public void SetAccountFullName(string name)
        {
            this.rebateModel.ContactInfo.FullName = name;
        }

        public void SetAccountEmailAddress(string email)
        {
            this.rebateModel.ContactInfo.Email = email;
        }

        public void SetAccountMobileNumber(string number)
        {
            this.rebateModel.ContactInfo.MobileNumber = number;
        }

        public bool CheckModelIfValid()
        {
            return this.rebateModel != null &&
                this.rebateModel.ContactInfo != null &&
                this.rebateModel.ContactInfo.FullName.IsValid() &&
                this.rebateModel.ContactInfo.Email.IsValid() &&
                this.rebateModel.ContactInfo.MobileNumber.IsValid();
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == EnquiryConstants.COUNTRY_CODE_SELECT_REQUEST)
                {
                    string dataString = data.GetStringExtra(Constants.SELECT_COUNTRY_CODE);
                    Country selectedCountry = JsonConvert.DeserializeObject<Country>(dataString);
                    this.view.SetSelectedCountry(selectedCountry);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetTncAcceptedFlag(bool accepted)
        {
            this.tncAccepted = accepted;
        }

        public bool GetTncAcceptedFlag()
        {
            return this.tncAccepted;
        }

        public string GetAccountFullName()
        {
            return this.rebateModel.ContactInfo.FullName;
        }

        public string GetAccountEmailAddress()
        {
            return this.rebateModel.ContactInfo.Email;
        }

        public bool GetIsOwner()
        {
            return this.rebateModel.IsOwner;
        }


        public async Task OnSubmitActionAsync()
        {
            try
            {
                if (this.rebateModel == null)
                {
                    return;
                }

                if (this.view.IsActive())
                {
                    this.view.ShowProgressDialog();
                }

                List<AttachedImageRequest> imageRequest = new List<AttachedImageRequest>();
                if (this.rebateModel.Documents.OwnerIC.IsValid())
                {
                    List<AttachedImage> ownersICDocument = this.view.GetDeSerializeImage(this.rebateModel.Documents.OwnerIC);
                    foreach (AttachedImage image in ownersICDocument)
                    {
                        var iCImage = await this.view.SaveImage(image);
                        imageRequest.Add(iCImage);
                    }
                }

                if (this.rebateModel.Documents.TenancyAgreement.IsValid())
                {
                    List<AttachedImage> tenancyDocument = this.view.GetDeSerializeImage(this.rebateModel.Documents.TenancyAgreement);
                    foreach (AttachedImage image in tenancyDocument)
                    {
                        var tenancyImage = await this.view.SaveImage(image);
                        imageRequest.Add(tenancyImage);
                    }
                }

                SubmitGSLEnquiryRequest submitGSLEnquiryRequest = new SubmitGSLEnquiryRequest(this.rebateModel);
                foreach (AttachedImageRequest image in imageRequest)
                {
                    string fileFormat = image.FileName.ToLower().Contains("pdf") ? "pdf" : "jpeg";
                    submitGSLEnquiryRequest.feedback.SetEnquiryImage(image.ImageHex, image.FileName, image.FileSize.ToString(), fileFormat);
                }

                if (this.rebateModel.IncidentList != null && this.rebateModel.IncidentList.Count > 0)
                {
                    this.rebateModel.IncidentList.ForEach(incident =>
                    {
                        submitGSLEnquiryRequest.feedback.SetIncidentInfos(incident.IncidentDateTime, incident.RestorationDateTime);
                    });
                }

                var gslSubmitEnquiryResponse = await ServiceApiImpl.Instance.SubmitEnquiryWithType(submitGSLEnquiryRequest);

                if (gslSubmitEnquiryResponse.Response != null &&
                    gslSubmitEnquiryResponse.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                {
                    SimpleDateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
                    var newSubmittedFeedback = new SubmittedFeedback()
                    {
                        FeedbackId = gslSubmitEnquiryResponse.GetData().ServiceReqNo,
                        DateCreated = dateFormat.Format(Java.Lang.JavaSystem.CurrentTimeMillis()),
                        FeedbackMessage = string.Empty,
                        FeedbackCategoryId = EnquiryConstants.GSL_FEEDBACK_CATEGORY_ID

                    };
                    SubmittedFeedbackEntity.InsertOrReplace(newSubmittedFeedback);
                    this.view.ShowSuccess(gslSubmitEnquiryResponse.GetData().DateCreated, gslSubmitEnquiryResponse.GetData().ServiceReqNo, imageRequest.Count);
                }
                else
                {
                    this.view.OnSubmitError(gslSubmitEnquiryResponse.Response.DisplayMessage);
                }

                if (this.view.IsActive())
                {
                    this.view.HideProgressDialog();
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (this.view.IsActive())
                {
                    this.view.HideProgressDialog();
                }
                this.view.OnSubmitError();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (this.view.IsActive())
                {
                    this.view.HideProgressDialog();
                }
                this.view.OnSubmitError();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (this.view.IsActive())
                {
                    this.view.HideProgressDialog();
                }
                this.view.OnSubmitError();
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
