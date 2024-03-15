using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Base.Fragments;
using myTNB.AndroidApp.Src.MyTNBService.Model;
using myTNB.AndroidApp.Src.NewAppTutorial.MVP;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
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
            void ShowUnavailableContent(bool isRefresh, string btnText, string contentText);
            void ShowAvailableBillContent();
            void ShowEmptyState();
            void ShowDowntimeSnackbar(string message);
            void OnShowItemizedFragmentTutorialDialog();
            void ItemizedBillingCustomScrolling(int yPosition);
            void ShowUnavailableChargeContent(bool isShowRefresh, string btnText, string contentText);
            void ShowUnavailableBillContent(bool isShowRefresh, string btnText, string contentText);
            bool GetIsPendingPayment();
            bool GetIsIneligiblePopUpActive();
        }
    }
}
