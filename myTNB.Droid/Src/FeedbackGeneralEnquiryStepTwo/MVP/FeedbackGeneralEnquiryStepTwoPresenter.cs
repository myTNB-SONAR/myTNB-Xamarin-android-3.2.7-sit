using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.FeedbackGeneralEnquiryStepTwo.MVP
{
    public class FeedbackGeneralEnquiryStepTwoPresenter : FeedbackGeneralEnquiryStepTwoContract.IUserActionsListener
    {

        FeedbackGeneralEnquiryStepTwoContract.IView mView;
        public FeedbackGeneralEnquiryStepTwoPresenter(FeedbackGeneralEnquiryStepTwoContract.IView mView)
        {

            this.mView = mView;
            this.mView.SetPresenter(this);

        }

        public void NavigateToTermsAndConditions()
        {
            this.mView.ShowNavigateToTermsAndConditions();
        }

        public void Start()
        {
            try
            {

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }



    }
}