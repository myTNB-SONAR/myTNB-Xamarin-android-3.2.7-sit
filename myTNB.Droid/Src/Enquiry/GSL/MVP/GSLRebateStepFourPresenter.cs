using System;
namespace myTNB_Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateStepFourPresenter : GSLRebateStepFourContract.IUserActionsListener
    {
        private readonly GSLRebateStepFourContract.IView view;

        private GSLRebateModel rebateModel;

        public GSLRebateStepFourPresenter(GSLRebateStepFourContract.IView view)
        {
            this.view = view;
            this.view?.SetPresenter(this);
        }

        public void OnInitialize()
        {
            OnInit();
            this.view?.UpdateButtonState(false);
            this.view?.SetUpViews();
        }

        private void OnInit()
        {
            this.rebateModel = new GSLRebateModel
            {
                Documents = new GSLRebateDocumentModel(),
            };
        }

        public void Start() { }

        public void SetRebateModel(GSLRebateModel model)
        {
            this.rebateModel = model;
        }

        public GSLRebateModel GetGSLRebateModel()
        {
            return rebateModel;
        }

        public bool CheckRequiredFields()
        {
            return true; //stub
        }
    }
}
