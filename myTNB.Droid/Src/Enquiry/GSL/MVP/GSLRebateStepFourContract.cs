using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Base.MVP;
using myTNB.Android.Src.Base.Request;
using myTNB.Android.Src.Common.Model;

namespace myTNB.Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateStepFourContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Setting Up Layout
            /// </summary>
            void SetUpViews();

            void UpdateButtonState(bool isEnabled);

            void SetSelectedCountry(Country country);

            void ShowProgressDialog();

            void HideProgressDialog();

            Task<AttachedImageRequest> SaveImage(AttachedImage attachedImage);

            List<AttachedImage> GetDeSerializeImage(string image);

            void OnSubmitError(string message = null);

            void ShowSuccess(string date, string feedbackId, int imageCount);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initialization in the Presenter
            /// </summary>
            void OnInitialize();

            void SetRebateModel(GSLRebateModel model);

            bool CheckModelIfValid();

            GSLRebateModel GetGSLRebateModel();

            void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);

            void SetAccountFullName(string name);

            void SetAccountEmailAddress(string email);

            void SetAccountMobileNumber(string number);

            void SetTncAcceptedFlag(bool accepted);

            bool GetTncAcceptedFlag();

            string GetAccountFullName();

            string GetAccountEmailAddress();

            bool GetIsOwner();

            Task OnSubmitActionAsync();
        }
    }
}
