using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.MyTNBService.Response;

namespace myTNB.AndroidApp.Src.ViewReceipt.MVP
{
    public class ViewReceiptMultiAccountNewDesignContract
    {
        public ViewReceiptMultiAccountNewDesignContract()
        {
        }

        public interface IView : IBaseView<IUserActionsListener>
        {
            void OnShowReceiptDetails(GetPaymentReceiptResponse response);

            void ShowGetReceiptDialog();

            void HideGetReceiptDialog();

            void ShowErrorMessage(string msg);

            void CreatePDF(GetPaymentReceiptResponse response);

            void ShowPaymentReceiptError();
        }

        public interface IUserActionsListener : IBasePresenter
        {

            void NavigateToNextScreen();

            void GetReceiptDetails(string selectedAccountNumber, string detailedInfoNumber, bool isOwnedAccount, bool showAllReceipt);
        }
    }
}