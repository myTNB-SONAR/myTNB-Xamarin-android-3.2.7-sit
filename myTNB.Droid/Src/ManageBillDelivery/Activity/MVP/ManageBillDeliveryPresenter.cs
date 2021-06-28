using System;
using System.Collections.Generic;
using System.Net;
using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Runtime;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.SSMRMeterHistory.Api;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ManageBillDelivery.MVP
{
    public class ManageBillDeliveryPresenter : ManageBillDeliveryContract.IUserActionsListener, ManageBillDeliveryContract.IPresenter
    {
        List<ManageBillDeliveryModel> ManageBillDeliveryList = new List<ManageBillDeliveryModel>();
        ManageBillDeliveryContract.IView mView;
        SMRregistrationApi api;
        public ManageBillDeliveryPresenter(ManageBillDeliveryContract.IView view)
        {
            this.mView = view;
            this.mView.SetPresenter(this);
            ManageBillDeliveryList = new List<ManageBillDeliveryModel>();
        }
        public async void CheckSMRAccountEligibility(List<SMRAccount> smrAccountList)
        {
            
            List<string> accountList = new List<string>();
            
            if (accountList.Count == 0)
            {
                this.mView.ShowSMREligibleAccountList(smrAccountList);
            }
         
        }
        public List<ManageBillDeliveryModel> GenerateManageBillDeliveryList()
        {
            ManageBillDeliveryList = new List<ManageBillDeliveryModel>();
                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoTitle1"),
                    Description = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoDescription1"),
                    Image = "manage_bill_delivery_0"
                });

                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoTitle2"),
                    Description = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoDescription2"),
                    Image = "manage_bill_delivery_1"
                });

                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoTitle3"),
                    Description = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoDescription3"),
                    Image = "manage_bill_delivery_2"
                });
           

            return ManageBillDeliveryList;
        }

        public List<ManageBillDeliveryModel> GenerateNewWalkthroughList(string currentAppNavigation)
        {
            throw new System.NotImplementedException();
        }

        public List<SMRAccount> GetEligibleSMRAccountList()
        {
            List<CustomerBillingAccount> eligibleSMRAccountList = CustomerBillingAccount.List();
            List<SMRAccount> smrEligibleAccountList = new List<SMRAccount>();
            SMRAccount smrEligibleAccount;
            eligibleSMRAccountList.ForEach(account =>
            {
                smrEligibleAccount = new SMRAccount();
                smrEligibleAccount.accountNumber = account.AccNum;
                smrEligibleAccount.accountName = account.AccDesc;
                smrEligibleAccount.accountSelected = account.IsSelected;
                smrEligibleAccount.isTaggedSMR = account.IsTaggedSMR;
                smrEligibleAccount.accountAddress = account.AccountStAddress;
                smrEligibleAccount.accountOwnerName = account.OwnerName;
                smrEligibleAccountList.Add(smrEligibleAccount);
            });
            return smrEligibleAccountList;
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

                            this.mView.SetAccountName(selectedAccount.AccDesc);
                       
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