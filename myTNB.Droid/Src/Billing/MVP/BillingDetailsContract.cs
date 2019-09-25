using System;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.Billing.MVP
{
    public class BillingDetailsContract
    {
        public interface IView
        {
            void ShowBillPDF(BillHistoryV5 selectedBill = null);
            void ShowProgressDialog();
            void HideProgressDialog();
            void ShowBillErrorSnackBar();
        }

        public interface IPresenter
        {
            void GetBillHistory(AccountData selectedAccount);
        }
    }
}
