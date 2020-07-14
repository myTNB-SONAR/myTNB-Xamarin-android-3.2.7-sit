using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.NewAppTutorial.MVP;

namespace myTNB_Android.Src.Billing.MVP
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
