using myTNB_Android.Src.Base.MVP;

namespace myTNB_Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateStepOneContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Setting Up Layout
            /// </summary>
            void SetUpViews();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Initialization in the Presenter
            /// </summary>
            void OnInitialize();
        }
    }
}
