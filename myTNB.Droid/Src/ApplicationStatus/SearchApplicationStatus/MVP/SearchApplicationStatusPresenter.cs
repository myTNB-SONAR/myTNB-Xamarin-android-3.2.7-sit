using System;
namespace myTNB.AndroidApp.Src.ApplicationStatus.SearchApplicationStatus.MVP
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