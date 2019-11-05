using System;
using System.Collections.Generic;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.NewAppTutorial.MVP;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    public class ItemisedBillingContract
    {
        public interface IView : IBaseFragmentCustomView
        {
            void PopulateAccountCharge(List<AccountChargeModel> accountChargesModelList);
            void PopulateBillingHistoryList(List<AccountBillPayHistoryModel> billingHistoryList, List<AccountBillPayFilter> billPayFilterList);
            void ShowBillPDFPage(AccountBillPayHistoryModel.BillingHistoryData billHistoryData);
            void ShowPayPDFPage(AccountBillPayHistoryModel.BillingHistoryData billHistoryData);
            void ShowShimmerLoading();
            void ShowUnavailableBillContent(bool isRefresh);
            void ShowAvailableBillContent();
            void ShowEmptyState();
            void ShowDowntimeSnackbar(string message);
            NewAppTutorialDialogFragment OnShowItemizedFragmentTutorialDialog();
            void ItemizedBillingCustomScrolling(int yPosition);
        }
    }
}
