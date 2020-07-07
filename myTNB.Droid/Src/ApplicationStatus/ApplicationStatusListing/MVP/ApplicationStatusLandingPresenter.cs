namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.MVP
{
    public class ApplicationStatusLandingPresenter : ApplicationStatusLandingContract.IPresenter
    {
        ApplicationStatusLandingContract.IView mView;

        public ApplicationStatusLandingPresenter(ApplicationStatusLandingContract.IView view)
        {
            mView = view;
        }

    }
}
