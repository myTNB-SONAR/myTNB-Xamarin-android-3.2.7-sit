using System;
namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    public class ApplicationFormSMRPresenter
    {
        ApplicationFormSMRContract.IView mView;
        public ApplicationFormSMRPresenter(ApplicationFormSMRContract.IView view)
        {
            mView = view;
        }


    }
}
