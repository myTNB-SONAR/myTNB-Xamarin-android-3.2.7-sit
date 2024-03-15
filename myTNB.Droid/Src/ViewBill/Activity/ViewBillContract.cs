using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using Refit;
using System;

namespace myTNB.AndroidApp.Src.ViewBill.Activity
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