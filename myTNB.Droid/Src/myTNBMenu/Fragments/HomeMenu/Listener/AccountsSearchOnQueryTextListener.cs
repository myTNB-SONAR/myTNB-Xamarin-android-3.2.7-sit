using System;
using Android.Widget;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Listener
{
	public class AccountsSearchOnQueryTextListener : Java.Lang.Object, SearchView.IOnQueryTextListener
	{
        AccountsRecyclerViewAdapter mAdapter;
		public AccountsSearchOnQueryTextListener(AccountsRecyclerViewAdapter adapter)
		{
            this.mAdapter = adapter;
		}

		public bool OnQueryTextChange(string newText)
		{
            this.mAdapter.Filter.InvokeFilter(newText);
			return true;
		}

		public bool OnQueryTextSubmit(string query)
		{
			string searchFilter = query;
			return true;
		}
	}
}
