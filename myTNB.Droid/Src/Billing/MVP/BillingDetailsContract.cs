using System;
using System.Collections.Generic;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.NewAppTutorial.MVP;

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
            List<NewAppModel> OnGeneraNewAppTutorialList();
        }
    }
}
