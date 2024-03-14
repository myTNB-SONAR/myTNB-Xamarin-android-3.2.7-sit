using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.Android.Src.Base.MVP;

namespace myTNB.Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateStepThreeContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Setting Up Layout
            /// </summary>
            void SetUpViews();

            void UpdateButtonState(bool isEnabled);

            string GetImageName(int itemCount);

            string GetTemporaryImageFilePath(string pFolder, string pFileName);

            void UpdateAdapter(string pFilePath, string pFileName, string tFullname = "");

            Task<string> SaveCameraImage(string tempImagePath, string fileName);

            Task<string> SaveGalleryImage(Android.Net.Uri selectedImage, string pTempImagePath, string pFileName);

            void ShowLoadingImage();

            void HideLoadingImage();

            string GetFilename(Android.Net.Uri uri);

            string GetActualPath(Android.Net.Uri uri);

            string CopyPDFGetFilePath(Android.Net.Uri realFilePath, string filename);

            void ShowError(string message = null);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initialization in the Presenter
            /// </summary>
            void OnInitialize();

            void SetRebateModel(GSLRebateModel model);

            bool CheckRequiredFields();

            GSLRebateModel GetGSLRebateModel();

            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);

            void SetTenancyDocument(string value);

            void SetOwnerIC(string value);
        }
    }
}
