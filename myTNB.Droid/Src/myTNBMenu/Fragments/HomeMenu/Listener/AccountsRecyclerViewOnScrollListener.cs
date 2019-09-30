using System;
using Android.Support.V7.Widget;
using Android.Widget;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Listener
{
    public class AccountsRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
	{
		LinearLayoutManager mLinearLayoutManager;
		LinearLayout mIndicatorContainer;
        HomeMenuContract.IHomeMenuPresenter mPresenter;
        int previousPos;
		public AccountsRecyclerViewOnScrollListener(HomeMenuContract.IHomeMenuPresenter presenter, LinearLayoutManager linearLayoutManager, LinearLayout indicatorContainer)
		{
			mLinearLayoutManager = linearLayoutManager;
            mIndicatorContainer = indicatorContainer;
            this.mPresenter = presenter;
        }
		public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
		{
			base.OnScrolled(recyclerView, dx, dy);
			int currentPosition = mLinearLayoutManager.FindFirstCompletelyVisibleItemPosition();
			if (currentPosition >= 0)
			{
                ImageView imageView;
                for (int i = 0; i < mIndicatorContainer.ChildCount; i++)
                {
                    imageView = (ImageView)mIndicatorContainer.GetChildAt(i);
                    if (i == currentPosition)
                    {
                        imageView.SetImageResource(Resource.Drawable.circle_active);
                    }
                    else
                    {
                        imageView.SetImageResource(Resource.Drawable.circle);
                    }
                }
                if (previousPos != currentPosition)
                {
                    this.mPresenter.LoadBatchSummarDetailsByIndex(currentPosition);
                }
                previousPos = currentPosition;

            }
		}
	}
}
