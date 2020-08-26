using System;

using Android.Widget;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Listener
{
    public class AccountsRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
	{
		LinearLayoutManager mLinearLayoutManager;
        HomeMenuContract.IHomeMenuPresenter mPresenter;
        int previousPos;
		public AccountsRecyclerViewOnScrollListener(HomeMenuContract.IHomeMenuPresenter presenter, LinearLayoutManager linearLayoutManager)
		{
			mLinearLayoutManager = linearLayoutManager;
            this.mPresenter = presenter;
        }
		public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
		{
			base.OnScrolled(recyclerView, dx, dy);
			int currentPosition = mLinearLayoutManager.FindFirstCompletelyVisibleItemPosition();
			if (currentPosition >= 0)
			{
                if (previousPos != currentPosition)
                {
                    this.mPresenter.LoadBatchSummarDetailsByIndex(currentPosition);
                }
                previousPos = currentPosition;

            }
		}
	}
}
