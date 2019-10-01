using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.ViewReceipt.Model;

namespace myTNB_Android.Src.ViewReceipt.MVP
{
    public class ViewReceiptContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void OnShowReceiptDetails(GetReceiptResponse response);

            void ShowGetReceiptDialog();

            void HideGetReceiptDialog();

            void ShowErrorMessage(string msg);

            void OnDownloadPDF();

        }

        public interface IUserActionsListener : IBasePresenter
        {

            void NevigateToNextScreen();

            void GetReceiptDetails(string apiKeyId, string merchantTransId);
        }
    }
}