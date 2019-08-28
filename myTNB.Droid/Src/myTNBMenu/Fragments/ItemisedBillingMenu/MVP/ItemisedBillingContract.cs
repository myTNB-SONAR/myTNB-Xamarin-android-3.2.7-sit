using System;
using System.Collections.Generic;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.API;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    public class ItemisedBillingContract
    {
        public interface IView
        {
            void PopulateAccountCharge(List<AccountChargeModel> accountChargesModelList);
            void PopulateBillingHistoryList(List<ItemisedBillingHistoryModel> billingHistoryList);
        }
    }
}
