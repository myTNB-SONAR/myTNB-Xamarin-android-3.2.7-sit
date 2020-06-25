using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using myTNB_Android.Src.Base.MVP;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.MVP
{
    public class FeedbackGeneralEnquiryStepTwoContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

            void ShowNavigateToTermsAndConditions();



        }

        public interface IUserActionsListener : IBasePresenter
        {

            void NavigateToTermsAndConditions();

        }
    }
}