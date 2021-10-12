using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using myTNB_Android.Src.DBR.DBRApplication.MVP;
using myTNB.Mobile;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.ManageBillDelivery.MVP
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
                Title = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoTitle0"),
                Description = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoDescription0"),
                Image = "manage_bill_delivery_3"
            });
            ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
            {
                Title = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoTitle1"),
                Description = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoDescription1"),
                Image = "manage_bill_delivery_0"
            });

            if (selectedAccountData != null && BillRedesignUtility.Instance.IsCAEligible(selectedAccountData.AccountNum))
            {
                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoTitle2V2"),
                    Description = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoDescription2V2"),
                    Image = "dbr_paper_e_bill.png"
                });
            }
            else
            {
                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoTitle2"),
                    Description = Utility.GetLocalizedLabel("ManageDigitalBillLanding", "dbrInfoDescription2"),
                    Image = "manage_bill_delivery_1"
                });
            }

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