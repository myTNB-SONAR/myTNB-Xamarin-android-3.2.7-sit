using System;
using myTNB_Android.Src.Base.MVP;

namespace myTNB_Android.Src.Enquiry.GSL.MVP
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
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initialization in the Presenter
            /// </summary>
            void OnInitialize();

            void SetRebateModel(GSLRebateModel model);

            bool CheckRequiredFields();

            GSLRebateModel GetGSLRebateModel();
        }
    }
}
