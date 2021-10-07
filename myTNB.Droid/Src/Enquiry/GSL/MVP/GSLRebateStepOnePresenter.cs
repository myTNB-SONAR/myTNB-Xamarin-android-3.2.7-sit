using System;
namespace myTNB_Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateStepOnePresenter : GSLRebateStepOneContract.IUserActionsListener
    {
        private readonly GSLRebateStepOneContract.IView view;

        public GSLRebateStepOnePresenter(GSLRebateStepOneContract.IView view)
        {
            this.view = view;
            this.view?.SetPresenter(this);
        }

        public void OnInitialize()
        {
            this.view?.SetUpViews();
        }

        public void Start() { }
    }
}
