using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ManageBillDelivery.MVP
{
    public class ManageBillDeliveryPresenter : ManageBillDeliveryContract.IUserActionsListener
    {
        List<ManageBillDeliveryModel> ManageBillDeliveryList = new List<ManageBillDeliveryModel>();
        ManageBillDeliveryContract.IView mView;
        public ManageBillDeliveryPresenter(ManageBillDeliveryContract.IView view)
        {
            this.mView = view;
            this.mView.SetPresenter(this);
            ManageBillDeliveryList = new List<ManageBillDeliveryModel>();
        }

        public List<ManageBillDeliveryModel> GenerateManageBillDeliveryList()
        {
            ManageBillDeliveryList = new List<ManageBillDeliveryModel>();
                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel("ManageBillDelivery", "title1"),
                    Description = Utility.GetLocalizedLabel("ManageBillDelivery", "description1"),
                    Image = "manage_bill_delivery_0"
                });

                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel("ManageBillDelivery", "title2"),
                    Description = Utility.GetLocalizedLabel("ManageBillDelivery", "description2"),
                    Image = "manage_bill_delivery_1"
                });

                ManageBillDeliveryList.Add(new ManageBillDeliveryModel()
                {
                    Title = Utility.GetLocalizedLabel("ManageBillDelivery", "title3"),
                    Description = Utility.GetLocalizedLabel("ManageBillDelivery", "description3"),
                    Image = "manage_bill_delivery_2"
                });
           

            return ManageBillDeliveryList;
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