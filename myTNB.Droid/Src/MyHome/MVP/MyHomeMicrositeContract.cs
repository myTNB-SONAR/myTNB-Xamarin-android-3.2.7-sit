using System.Threading.Tasks;
using Android.Content;
using myTNB.Mobile;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.MyHome.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.MyTNBService.Response.PaymentTransactionIdResponse;

namespace myTNB_Android.Src.MyHome.MVP
{
	public class MyHomeMicrositeContract
	{
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Setting Up Layout
            /// </summary>
            void SetUpViews();

            /// <summary>
            /// Shows loading overlay
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hides loading overlay
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            /// Shows generic error pop up
            /// </summary>
            void ShowGenericError();

            /// <summary>
            /// Views the downloaded file to a different screen
            /// </summary>
            /// <param name="filePath"></param>
            /// <param name="fileExtension"></param>
            /// <param name="fileTitle"></param>
            void ViewDownloadedFile(string filePath, string fileExtension, string fileTitle);

            /// <summary>
            /// Triggers the share functionality for the downloaded file
            /// </summary>
            /// <param name="filePath"></param>
            /// <param name="fileExtension"></param>
            /// <param name="fileTitle"></param>
            void ShareDownloadedFile(string filePath, string fileExtension, string fileTitle);

            /// <summary>
            /// Navigates to ViewBillActivity to display latest bill
            /// </summary>
            /// <param name="selectedBill"></param>
            void ShowLatestBill(string accountNum, bool isOwner);

            /// <summary>
            /// Shows Payment Details screen with data model
            /// </summary>
            /// <param name="paymentDetailsModel"></param>
            void ShowPaymentDetails(MyHomePaymentDetailsModel paymentDetailsModel);

            /// <summary>
            /// Shows Payment flow with applicationStatusDisplay param
            /// </summary>
            /// <param name="applicationStatusDisplay"></param>
            void ShowApplicationPayment(GetApplicationStatusDisplay applicationStatusDisplay);

            /// <summary>
            /// Shows Bill Payment screen flow
            /// </summary>
            /// <param name="accountData"></param>
            void ShowBillPayment(AccountData accountData);

            /// <summary>
            /// Reloads webview with new sso details
            /// </summary>
            /// <param name="details"></param>
            void ReloadWebview(MyHomeDetails details, string accessToken);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initialization in the Presenter
            /// </summary>
            void OnInitialize();


            /// <summary>
            /// Task to download and view file from webURL
            /// </summary>
            /// <param name="webURL"></param>
            /// <returns></returns>
            Task ViewFile(string webURL);

            /// <summary>
            /// Task to download file from webURL
            /// </summary>
            /// <param name="webURL"></param>
            /// <returns></returns>
            Task DownloadFile(string webURL);

            /// <summary>
            /// Gets the file path value for the downloaded file
            /// </summary>
            /// <returns></returns>
            string GetFilePath();

            /// <summary>
            /// Parses the url and calls GetCharges API
            /// </summary>
            /// <param name="webURL"></param>
            void GetPaymentDetails(string webURL);

            /// <summary>
            /// Method to parse webURL to show latest bill
            /// </summary>
            /// <param name="webURL"></param>
            /// <returns></returns>
            void GetLatestBill(string webURL);

            /// <summary>
            /// Parses the url to get the payment info
            /// </summary>
            /// <param name="webURL"></param>
            void GetPaymentInfo(string webURL);

            /// <summary>
            /// Reloads Microsite with new SSO details
            /// </summary>
            /// <param name="details"></param>
            void OnReloadMicrosite(MyHomeDetails details);

            /// <summary>
            /// Generates myHome SSO signature
            /// </summary>
            /// <param name="ssoDomain"></param>
            /// <param name="originURL"></param>
            /// <param name="redirectURL"></param>
            /// <param name="cancelURL"></param>
            /// <param name="accessToken"></param>
            /// <returns></returns>
            string OnGetMyHomeSignature(string ssoDomain, string originURL, string redirectURL, string cancelURL, string accessToken);
        }
    }
}

