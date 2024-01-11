using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Response;
using Refit;
using System;

namespace myTNB_Android.Src.ViewBill.Activity
{
    public class ViewBillContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void ShowBillPDF(GetBillHistoryResponse.ResponseData selectedBill = null);

            void ShowProgressDialog();

            void HideProgressDialog();

            void ShowViewBillError(string title, string message);

            void ShowBillErrorSnackBar();

            void GetFileGenerateData(string billNo, byte[] binaryBill);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            void LoadingBillsHistory(AccountData selectedAccount);
        }
    }
}