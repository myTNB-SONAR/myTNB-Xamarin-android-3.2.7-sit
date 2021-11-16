using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.MyTNBService.Response;

namespace myTNB_Android.Src.ViewReceipt.MVP
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