using System;
namespace myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.MVP
{
    public class SearchApplicationStatusPresenter : SearchApplicationStatusContract.IPresenter
    {
        SearchApplicationStatusContract.IView mView;

        public SearchApplicationStatusPresenter(SearchApplicationStatusContract.IView view)
        {
            mView = view;
        }
    }
}