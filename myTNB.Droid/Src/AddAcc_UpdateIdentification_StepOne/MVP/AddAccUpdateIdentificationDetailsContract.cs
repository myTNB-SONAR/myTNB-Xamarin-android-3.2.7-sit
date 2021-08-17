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

namespace myTNB_Android.Src.AddAcc_UpdateIdentification_StepOne.MVP
{
    public class AddAccUpdateIdentificationDetailsContract
    {

        public interface IView : IBaseView<IUserActionsListener>
        {

            void ShowGeneralEnquiry();

            void ShowInvalidAccountNumberError();

            void RemoveNumberErrorMessage();

            void ClearICMinimumCharactersError();

            void ShowFullICError();

            void showNextStep();

            void ShowEnterOrSelectAccNumber();

            void toggleEnableClick();

            void toggleDisableClick();

            void DisableNextButton();

            void EnableNextButton();

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

            void OnSelectAccount();

            bool CheckRequiredFields(string accno, string ic);

            //void CheckRequiredFields(string accno, string icno);

            void OnGoNextStep();

            void onShowWhereIsMyAcc();

            void ValidateAccountAsync(string contractAccounts, bool isUpdateUserInfo);

            //bool validateField(string accno, string icno);

            void showScan();


        }
    }
}