using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB;
using myTNB_Android.Src.Common;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.Database.Model;
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
            SaveAccountInfo();
            GetRebateTypeFromSelector();
            this.view?.SetUpViews();
            this.view?.UpdateButtonState(false);
        }

        public void Start() { }

        private void OnInit()
        {
            rebateModel = new GSLRebateModel
            {
                IsOwner = false,
                FeedbackCategoryId = EnquiryConstants.GSL_FEEDBACK_CATEGORY_ID,
                AccountInfo = new GSLRebateAccountInfoModel(),
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
                else if (requestCode == EnquiryConstants.SELECT_REBATE_TYPE_REQ_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        rebateTypeList = JsonConvert.DeserializeObject<List<Item>>(data.GetStringExtra("SELECTED_ITEM_LIST"));
                        Item selectedRebateType = rebateTypeList.Find(itemFilter =>
                        {
                            return itemFilter.selected;
                        });
                        this.rebateModel.RebateTypeKey = selectedRebateType.key;
                        this.rebateModel.NeedsIncident = selectedRebateType.needsIncident;
                        this.view.UpdateSelectedRebateType(selectedRebateType);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SaveAccountInfo()
        {
            this.rebateModel.AccountInfo.FullName = UserEntity.GetActive().DisplayName;
            this.rebateModel.AccountInfo.Email = UserEntity.GetActive().Email;
            this.rebateModel.AccountInfo.MobileNumber = UserEntity.GetActive().MobileNo;
        }

        private void GetRebateTypeFromSelector()
        {
            Dictionary<string, List<RebateTypeModel>> selector = LanguageManager.Instance.GetSelectorsByPage<RebateTypeModel>(LanguageConstants.SUBMIT_ENQUIRY);
            if (selector.ContainsKey(LanguageConstants.SubmitEnquiry.REBATE_TYPE))
            {
                var itemList = selector[LanguageConstants.SubmitEnquiry.REBATE_TYPE];
                if (itemList.Count > 0)
                {
                    itemList.ForEach(filter =>
                    {
                        Item item = new Item
                        {
                            key = filter.Key,
                            title = filter.Description,
                            needsIncident = filter.NeedsIncident
                        };
                        rebateTypeList.Add(item);
                    });
                    rebateTypeList[0].selected = true;
                    this.rebateModel.RebateTypeKey = rebateTypeList[0].key;
                    this.rebateModel.NeedsIncident = rebateTypeList[0].needsIncident;
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

        public void SetIsOwner(bool isOwner)
        {
            this.rebateModel.IsOwner = isOwner;
            if (!isOwner)
            {
                this.view?.PrepopulateTenantFields();
            }
            else
            {
                this.view?.UpdateButtonState(true);
            }
        }

        public void SetAccountNumber(string accountNum)
        {
            this.rebateModel.AccountNum = accountNum;
            CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(this.rebateModel.AccountNum);
            if (account != null)
            {
                this.rebateModel.AccountInfo.Address = account.AccountStAddress;
            }
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
            bool fieldsAreValid;
            if (this.rebateModel.IsOwner)
            {
                fieldsAreValid = true;
            }
            else
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

                fieldsAreValid = rebateModel.TenantInfo.MobileNumber.IsValid() && !this.view.IsMobileNumEmpty();
            }

            return fieldsAreValid;
        }

        public GSLRebateModel GetGSLRebateModel()
        {
            return rebateModel;
        }
    }
}
