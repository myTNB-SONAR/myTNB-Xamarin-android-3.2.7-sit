using System.Threading.Tasks;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.MyHome.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;

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
            /// Shows Payment Details screen with data model
            /// </summary>
            /// <param name="paymentDetailsModel"></param>
            void ShowPaymentDetails(MyHomePaymentDetailsModel paymentDetailsModel);
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
        }
    }
}

