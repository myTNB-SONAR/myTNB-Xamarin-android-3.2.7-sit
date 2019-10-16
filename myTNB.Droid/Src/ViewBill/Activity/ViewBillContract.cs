using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using Refit;
using System;

namespace myTNB_Android.Src.ViewBill.Activity
{
    public class ViewBillContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void ShowBillPDF(BillHistoryV5 selectedBill = null);

            void ShowProgressDialog();

            void HideProgressDialog();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            void LoadingBillsHistory(AccountData selectedAccount);
        }
    }
}