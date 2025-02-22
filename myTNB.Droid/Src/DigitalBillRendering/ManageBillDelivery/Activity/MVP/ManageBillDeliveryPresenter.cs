﻿using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using myTNB.AndroidApp.Src.DBR.DBRApplication.MVP;
using myTNB.Mobile;
using myTNB.AndroidApp.Src.myTNBMenu.Models;

namespace myTNB.AndroidApp.Src.ManageBillDelivery.MVP
{
    public class ManageBillDeliveryPresenter : ManageBillDeliveryContract.IUserActionsListener, ManageBillDeliveryContract.IPresenter
    {
        List<ManageBillDeliveryModel> ManageBillDeliveryList = new List<ManageBillDeliveryModel>();
        ManageBillDeliveryContract.IView mView;

        //SMRregistrationApi api;
        public ManageBillDeliveryPresenter(ManageBillDeliveryContract.IView view)
        {
            this.mView = view;
            this.mView.SetPresenter(this);
            ManageBillDeliveryList = new List<ManageBillDeliveryModel>();
        }

        public async void CheckDBRAccountEligibility(List<DBRAccount> dbrAccountList)
        {
            List<string> accountList = new List<string>();

            if (accountList.Count == 0)
            {
                this.mView.ShowDBREligibleAccountList(dbrAccountList);
            }
        }

        public List<ManageBillDeliveryModel> GenerateManageBillDeliveryList(AccountData selectedAccountData)
        {
            ManageBillDeliveryList = new List<ManageBillDeliveryModel>();
            ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
            {
                Title = Utility.GetLocalizedLabel(LanguageConstants.MANAGE_DIGITAL_BILL, LanguageConstants.ManageDigitalBill.DBR_INFO_TITLE_0),
                Description = Utility.GetLocalizedLabel(LanguageConstants.MANAGE_DIGITAL_BILL, LanguageConstants.ManageDigitalBill.DBR_INFO_DESC_0),
                Image = "manage_bill_delivery_3"
            });
            ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
            {
                Title = Utility.GetLocalizedLabel(LanguageConstants.MANAGE_DIGITAL_BILL, LanguageConstants.ManageDigitalBill.DBR_INFO_TITLE_1),
                Description = Utility.GetLocalizedLabel(LanguageConstants.MANAGE_DIGITAL_BILL, LanguageConstants.ManageDigitalBill.DBR_INFO_DESC_1),
                Image = "manage_bill_delivery_0"
            });

            if (selectedAccountData != null && BillRedesignUtility.Instance.IsCAEligible(selectedAccountData.AccountNum))
            {
                var isResidential = BillRedesignUtility.Instance.IsResidential(selectedAccountData.RateCategory);
                System.Console.WriteLine("isResidential: " + isResidential);
                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel(LanguageConstants.MANAGE_DIGITAL_BILL, LanguageConstants.ManageDigitalBill.DBR_INFO_TITLE_2_V2),
                    Description = Utility.GetLocalizedLabel(LanguageConstants.MANAGE_DIGITAL_BILL, isResidential ? LanguageConstants.ManageDigitalBill.DBR_INFO_DESC_2_V2
                    : LanguageConstants.ManageDigitalBill.DBR_INFO_DESC_2_V2_NON_RES),
                    Image = "dbr_paper_e_bill"
                });
            }
            else
            {
                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel(LanguageConstants.MANAGE_DIGITAL_BILL, LanguageConstants.ManageDigitalBill.DBR_INFO_TITLE_2),
                    Description = Utility.GetLocalizedLabel(LanguageConstants.MANAGE_DIGITAL_BILL, LanguageConstants.ManageDigitalBill.DBR_INFO_DESC_2),
                    Image = "manage_bill_delivery_1"
                });
            }

            ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
            {
                Title = Utility.GetLocalizedLabel(LanguageConstants.MANAGE_DIGITAL_BILL, LanguageConstants.ManageDigitalBill.DBR_INFO_TITLE_3),
                Description = Utility.GetLocalizedLabel(LanguageConstants.MANAGE_DIGITAL_BILL, LanguageConstants.ManageDigitalBill.DBR_INFO_DESC_3),
                Image = "manage_bill_delivery_2"
            });
            return ManageBillDeliveryList;
        }

        public List<ManageBillDeliveryModel> GenerateNewWalkthroughList(string currentAppNavigation)
        {
            throw new System.NotImplementedException();
        }

        public void InitialSetFilterName()
        {
            throw new System.NotImplementedException();
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == Constants.SELECT_ACCOUNT_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        Bundle extras = data.Extras;

                        CustomerBillingAccount selectedAccount = JsonConvert.DeserializeObject<CustomerBillingAccount>(extras.GetString(Constants.SELECTED_ACCOUNT));

                        this.mView.SetAccountName(selectedAccount);

                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SelectSupplyAccount()
        {
            List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
            if (accountList.Count >= 1)
            {
                this.mView.ShowSelectSupplyAccount();
            }
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}