using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Enquiry.GSL.MVP
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
            this.view?.SetUpViews();
        }

        private void OnInit()
        {
            this.tncAccepted = false;
            this.rebateModel = new GSLRebateModel
            {
                AccountInfo = new GSLRebateAccountInfoModel(),
            };
        }

        public void Start() { }

        public void SetRebateModel(GSLRebateModel model)
        {
            this.rebateModel = model;
        }

        public GSLRebateModel GetGSLRebateModel()
        {
            return rebateModel;
        }

        public void SetAccountFullName(string name)
        {
            this.rebateModel.AccountInfo.FullName = name;
        }

        public void SetAccountEmailAddress(string email)
        {
            this.rebateModel.AccountInfo.Email = email;
        }

        public void SetAccountMobileNumber(string number)
        {
            this.rebateModel.AccountInfo.MobileNumber = number;
        }

        public bool CheckModelIfValid()
        {
            return this.rebateModel != null &&
                this.rebateModel.AccountInfo != null &&
                this.rebateModel.AccountInfo.FullName.IsValid() &&
                this.rebateModel.AccountInfo.Email.IsValid() &&
                this.rebateModel.AccountInfo.MobileNumber.IsValid();
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
            return this.rebateModel.AccountInfo.FullName;
        }

        public string GetAccountEmailAddress()
        {
            return this.rebateModel.AccountInfo.Email;
        }

        public bool GetIsOwner()
        {
            return this.rebateModel.IsOwner;
        }
    }
}
