using System;
using System.Collections.Generic;
using System.Linq;
using Android.Widget;
using Java.Lang;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.Adapter;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class AccountCardsFilter : Filter
    {
        List<List<AccountCardModel>> originalCardList;
        List<List<AccountCardModel>> filterCardList;
        List<AccountCardModel> originalCardModelList = new List<AccountCardModel>();
        AccountsRecyclerViewAdapter mAdapter;
        HomeMenuContract.IHomeMenuView mViewListerner;

        public AccountCardsFilter(HomeMenuContract.IHomeMenuView viewListerner, AccountsRecyclerViewAdapter adapter, List<AccountCardModel> cardModelList)
        {
            this.mViewListerner = viewListerner;
            this.mAdapter = adapter;
            if (cardModelList != null && cardModelList.Count > 0)
            {
                this.originalCardModelList = cardModelList;
            }
            else
            {
                this.originalCardModelList = new List<AccountCardModel>();
            }
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            FilterResults filterResults = new FilterResults();
            var results = new List<AccountCardModel>();

            try
            {
                if (constraint == null || (constraint != null && constraint.Count() == 0) || (constraint != null && constraint.Length() == 0)) results = this.originalCardModelList;

                if (this.mAdapter != null && constraint != null && constraint.Count() > 0 && constraint.Length() > 0)
                {
                    results.AddRange((
                        this.originalCardModelList.Where(
                            cardModel => cardModel.AccountNumber.ToLower().Contains(constraint.ToString()) ||
                            (cardModel.AccountName != null &&
                            cardModel.AccountName.ToLower().Contains(constraint.ToString().ToLower())))));
                }

                filterResults.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
                filterResults.Count = results.Count;

                if (constraint != null)
                {
                    constraint.Dispose();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return filterResults;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            try
            {
                using (var values = results.Values)
                    this.mAdapter.accountCardModelList = values.ToArray<Java.Lang.Object>()
                        .Select(r => r.ToNetObject<AccountCardModel>()).ToList();
                this.mAdapter.UpdatedCardList();
                this.mAdapter.NotifyDataSetChanged();

                if (constraint != null)
                {
                    constraint.Dispose();
                }

                if (results != null)
                {
                    results.Dispose();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
