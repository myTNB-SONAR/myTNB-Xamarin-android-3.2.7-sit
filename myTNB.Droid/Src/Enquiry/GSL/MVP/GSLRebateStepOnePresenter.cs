using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB;
using myTNB_Android.Src.Common;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateStepOnePresenter : GSLRebateStepOneContract.IUserActionsListener
    {
        private readonly GSLRebateStepOneContract.IView view;

        private List<Item> rebateTypeList;
        private GSLRebateModel rebateModel;

        public GSLRebateStepOnePresenter(GSLRebateStepOneContract.IView view)
        {
            this.view = view;
            this.view?.SetPresenter(this);
        }

        public void OnInitialize()
        {
            OnInit();
            GetRebateTypeFromSelector();
            this.view?.SetUpViews();
            this.view?.UpdateButtonState(false);
        }

        public void Start() { }

        private void OnInit()
        {
            rebateModel = new GSLRebateModel
            {
                TenantInfo = new GSLRebateTenantModel()
            };
            rebateTypeList = new List<Item>();
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
                if (requestCode == EnquiryConstants.SELECT_REBATE_TYPE_REQ_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        rebateTypeList = JsonConvert.DeserializeObject<List<Item>>(data.GetStringExtra("SELECTED_ITEM_LIST"));
                        Item selectedRebateType = rebateTypeList.Find(itemFilter =>
                        {
                            return itemFilter.selected;
                        });
                        this.rebateModel.RebateType = selectedRebateType.title;
                        this.view.UpdateSelectedRebateType(selectedRebateType);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void GetRebateTypeFromSelector()
        {
            var selector = LanguageManager.Instance.GetSelectorsByPage<SelectorModel>(LanguageConstants.SUBMIT_ENQUIRY);
            if (selector.ContainsKey(LanguageConstants.SubmitEnquiry.REBATE_TYPE))
            {
                var itemList = selector[LanguageConstants.SubmitEnquiry.REBATE_TYPE];
                if (itemList.Count > 0)
                {
                    itemList.ForEach(filter =>
                    {
                        Item item = new Item
                        {
                            title = filter.Description
                        };
                        rebateTypeList.Add(item);
                    });
                    rebateTypeList[0].selected = true;
                    this.rebateModel.RebateType = rebateTypeList[0].title;
                }
            }
        }

        public List<Item> GetRebateTypeList()
        {
            return rebateTypeList;
        }

        public Item GetDefaultSelectedRebateType()
        {
            return rebateTypeList.Count > 0 ? rebateTypeList[0] : null;
        }

        public void SetTenantFullName(string name)
        {
            this.rebateModel.TenantInfo.FullName = name;
        }

        public void SetTenantEmailAddress(string email)
        {
            this.rebateModel.TenantInfo.Email = email;
        }

        public void SetTenantMobileNumber(string number)
        {
            this.rebateModel.TenantInfo.MobileNumber = number;
        }

        public bool CheckRequiredFields()
        {
            var fullNameValid = rebateModel.TenantInfo.FullName.IsValid();
            if (!fullNameValid)
            {
                this.view.ShowEmptyError(GSLLayoutType.FULL_NAME);
            }

            var emailValid = rebateModel.TenantInfo.Email.IsValid();
            if (!emailValid)
            {
                this.view.ShowEmptyError(GSLLayoutType.EMAIL_ADDRESS);
            }

            var mobileValid = rebateModel.TenantInfo.MobileNumber.IsValid() && !this.view.IsMobileNumEmpty();

            return fullNameValid && emailValid && mobileValid;
        }

        public GSLRebateModel GetGSLRebateModel()
        {
            return rebateModel;
        }
    }
}
