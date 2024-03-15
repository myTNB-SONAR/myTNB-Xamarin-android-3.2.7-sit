using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.ViewReceipt.Model;

namespace myTNB.AndroidApp.Src.ViewReceipt.MVP
{
    public class ViewReceiptMultiAccountContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void OnShowReceiptDetails(GetMultiReceiptByTransIdResponse response);

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