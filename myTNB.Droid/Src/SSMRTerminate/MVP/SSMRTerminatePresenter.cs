using Android.Telephony;
using Android.Text;
using Android.Util;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMRTerminate.MVP
{
    public class SSMRTerminatePresenter : SSMRTerminateContract.IPresenter
    {
        SSMRTerminateContract.IView mView;

        public SSMRTerminatePresenter(SSMRTerminateContract.IView view)
        {
            mView = view;
        }

        public void Start()
        {

        }

        public void CheckRequiredFields(string mobile_no, string email, bool isOtherReasonSelected, string otherReason)
        {

            try
            {
                if (!TextUtils.IsEmpty(mobile_no) && !TextUtils.IsEmpty(email))
                {
                    if (!Patterns.EmailAddress.Matcher(email).Matches())
                    {
                        this.mView.ShowInvalidEmailError();
                        this.mView.DisableSubmitButton();
                        return;
                    }
                    else
                    {
                        this.mView.ClearEmailError();
                    }

                    if (mobile_no == "+60")
                    {
                        this.mView.UpdateMobileNumber(mobile_no);
                        this.mView.DisableSubmitButton();
                        return;
                    }

                    if (!Utility.IsValidMobileNumber(mobile_no))
                    {
                        this.mView.ShowInvalidMobileNoError();
                        this.mView.DisableSubmitButton();
                        return;
                    }
                    else
                    {
                        this.mView.ClearInvalidMobileError();
                    }


                    this.mView.ClearErrors();
                    this.mView.EnableSubmitButton();
                }
                else
                {
                    this.mView.DisableSubmitButton();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
