using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Model;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.NewAppTutorial.MVP;

namespace myTNB.AndroidApp.Src.Billing.MVP
{
    public class BillingDetailsContract
    {
        public interface IView
        {
            void ShowBillPDF();
            void ShowProgressDialog();
            void HideProgressDialog();
            void ShowBillErrorSnackBar();
            void ShowBillDetails(List<AccountChargeModel> accountChargeModelList);
            void ShowBillDetailsError(bool isRefresh, string btnText, string contentText);
            void OnUpdatePendingPayment(bool mIsPendingPayament);
            void ShowViewBillError(string title, string message);
            void EnableDisableViewBillButtons(bool flag);
        }

        public interface IPresenter
        {
            List<NewAppModel> OnGeneraNewAppTutorialList();
            void ShowBillDetails(AccountData selectedAccount, bool isCheckPendingNeeded);
            List<string> ExtractUrls(string text);
           

        }
    }
}
