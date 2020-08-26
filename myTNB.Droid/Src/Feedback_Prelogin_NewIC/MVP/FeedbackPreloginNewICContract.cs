using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using Org.BouncyCastle.Asn1.BC;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Feedback_Prelogin_NewIC.MVP
{
    public class FeedbackPreloginNewICContract
    {

        public interface IView : IBaseView<IUserActionsListener>
        {

            void ShowGeneralEnquiry();

            void ShowInvalidAccountNumberError();

            void RemoveNumberErrorMessage();

            void showUpdatePersonalDetail();

            void ShowEnterOrSelectAccNumber();

            void toggleEnableClick();

            void toggleDisableClick();

            void ShowWhereIsMyAcc();

            void OnSubmitError(string message = null);

            void ShowProgressDialog();

             void HideProgressDialog();

            void makeSetClick(bool setclick);

            void onScan();

            void ShowSelectAccount();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            void OnGeneralEnquiry();

            void OnSelectAccount();

            void CheckRequiredFields(string accno);

           void onUpdatePersonalDetail();

           void onShowWhereIsMyAcc();

            void ValidateAccountAsync(string contractAccounts, bool isUpdateUserInfo);

            void showScan();


        }
    }
}