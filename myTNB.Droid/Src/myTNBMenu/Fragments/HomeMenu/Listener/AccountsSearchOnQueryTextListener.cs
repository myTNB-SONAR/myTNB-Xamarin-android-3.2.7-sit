﻿using System;
using Android.Widget;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Listener
{
	public class AccountsSearchOnQueryTextListener : Java.Lang.Object, SearchView.IOnQueryTextListener
	{
        AccountsRecyclerViewAdapter mAdapter;
        HomeMenuContract.IHomeMenuView mViewListerner;
        LinearLayout searchContainer;

		public AccountsSearchOnQueryTextListener(HomeMenuContract.IHomeMenuView viewListerner, AccountsRecyclerViewAdapter adapter, LinearLayout container)
		{
            this.mAdapter = adapter;
            this.mViewListerner = viewListerner;
            this.searchContainer = container;
		}

		public bool OnQueryTextChange(string newText)
		{
            this.mViewListerner.UpdateSearchViewBackground(newText);
            this.mAdapter.Filter.InvokeFilter(newText);
            return true;
        }

		public bool OnQueryTextSubmit(string query)
		{
            //this.mAdapter.Filter.InvokeFilter(query);
            this.mViewListerner.OnUpdateAccountListChanged(true);
            return false;
        }
	}
}
