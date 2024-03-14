using System;
using Android.OS;
using myTNB.Android.Src.Base.MVP;

namespace myTNB.Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateStepTwoContract
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

            void SetIncidentData(GSLIncidentDateTimePicker picker, DateTime dateTime, int index);

            void ResetIncidentData(GSLIncidentDateTimePicker picker, int index);

            bool CheckDateTimeFields();

            GSLRebateModel GetGSLRebateModel();
        }
    }
}
