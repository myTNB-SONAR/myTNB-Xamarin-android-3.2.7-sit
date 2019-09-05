using System;
using System.Collections.Generic;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.API;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    public class ItemisedBillingContract
    {
        public interface IView : IBaseFragmentCustomView
        {
            void PopulateAccountCharge(List<AccountChargeModel> accountChargesModelList);
            void PopulateBillingHistoryList(List<ItemisedBillingHistoryModel> billingHistoryList);
            void ShowBillPDFPage(ItemisedBillingHistoryModel.BillingHistoryData billHistoryData);
            void ShowPayPDFPage(ItemisedBillingHistoryModel.BillingHistoryData billHistoryData);
            void ShowShimmerLoading();
            void ShowRefreshPage(bool show);
        }
    }
}
