using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Feedback_Prelogin_NewIC.MVP
{
    public class FeedbackPreloginNewICContract
    {

        public interface IView : IBaseView<IUserActionsListener>
        {

             void ShowGeneralEnquiry();
            

             void ShowSelectAccount(AccountData accountData);

            void ShowInvalidAccountNumberError();

            void RemoveNumberErrorMessage();

            void showUpdatePersonalDetail();

            void ShowEnterOrSelectAccNumber();

            void toggleEnableClick();

            void toggleDisableClick();

            void ShowWhereIsMyAcc();





        }

        public interface IUserActionsListener : IBasePresenter
        {
            void OnGeneralEnquiry();

            void OnSelectAccount();

            void CheckRequiredFields(string accno);

           void onUpdatePersonalDetail();

           void onShowWhereIsMyAcc();


        }
    }
}