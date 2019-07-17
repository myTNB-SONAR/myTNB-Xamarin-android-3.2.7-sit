using System;
using Android.Support.V7.Widget;
using Android.Widget;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Listener
{
    public class AccountsRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
	{
		LinearLayoutManager mLinearLayoutManager;
		LinearLayout mLinearLayout;
		public AccountsRecyclerViewOnScrollListener(LinearLayoutManager linearLayoutManager, LinearLayout linearLayout)
		{
			mLinearLayoutManager = linearLayoutManager;
			mLinearLayout = linearLayout;
		}
		public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
		{
			base.OnScrolled(recyclerView, dx, dy);
			int pos = mLinearLayoutManager.FindFirstCompletelyVisibleItemPosition();
			if (pos >= 0)
			{
				ImageView imageView;
				for (int i = 0; i < mLinearLayout.ChildCount; i++)
				{
					imageView = (ImageView)mLinearLayout.GetChildAt(i);
					if (i == pos)
					{
						imageView.SetImageResource(Resource.Drawable.circle_active);
					}
					else
					{
						imageView.SetImageResource(Resource.Drawable.circle);
					}
				}
			}
		}
	}
}
