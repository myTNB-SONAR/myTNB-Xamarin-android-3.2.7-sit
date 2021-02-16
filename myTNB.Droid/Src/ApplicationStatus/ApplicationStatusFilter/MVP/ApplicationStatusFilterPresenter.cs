using System;
namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.MVP
{
    public class ApplicationStatusFilterPresenter : ApplicationStatusFilterContract.IPresenter
    {
        ApplicationStatusFilterContract.IView mView;

        public ApplicationStatusFilterPresenter(ApplicationStatusFilterContract.IView view)
        {
            mView = view;
        }
    }
}
