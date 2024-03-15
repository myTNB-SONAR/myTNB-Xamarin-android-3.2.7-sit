using System;
using Android.Widget;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.Adapter;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.Listener
{
	public class AccountsSearchOnQueryTextListener : Java.Lang.Object, SearchView.IOnQueryTextListener
	{
        AccountsRecyclerViewAdapter mAdapter;
        HomeMenuContract.IHomeMenuView mViewListerner;

		public AccountsSearchOnQueryTextListener(HomeMenuContract.IHomeMenuView viewListerner, AccountsRecyclerViewAdapter adapter)
		{
            this.mAdapter = adapter;
            this.mViewListerner = viewListerner;
		}

		public bool OnQueryTextChange(string newText)
		{
            try
            {
                this.mViewListerner.UpdateQueryListing(newText);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return true;
        }

		public bool OnQueryTextSubmit(string query)
		{
            try
            {
                this.mViewListerner.OnUpdateAccountListChanged(true);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return false;
        }
	}
}
