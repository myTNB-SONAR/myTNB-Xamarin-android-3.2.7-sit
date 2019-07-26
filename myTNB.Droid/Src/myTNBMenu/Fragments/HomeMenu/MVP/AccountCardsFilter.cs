using System;
using System.Collections.Generic;
using System.Linq;
using Android.Widget;
using Java.Lang;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class AccountCardsFilter : Filter
    {
        List<List<AccountCardModel>> originalCardList;
        List<List<AccountCardModel>> filterCardList;
        List<AccountCardModel> originalCardModelList;
        AccountsRecyclerViewAdapter mAdapter;

        public AccountCardsFilter(AccountsRecyclerViewAdapter adapter, List<AccountCardModel> cardModelList)
        {
            this.mAdapter = adapter;
            this.originalCardModelList = cardModelList;
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            FilterResults filterResults = new FilterResults();
            var results = new List<AccountCardModel>();

            if (constraint == null) results = this.originalCardModelList;

            if (this.mAdapter != null)
            {
                results.AddRange((
                    this.originalCardModelList.Where(
                        cardModel => cardModel.AccountNumber.ToLower().Contains(constraint.ToString()) ||
                        (cardModel.AccountName != null &&
                        cardModel.AccountName.ToLower().Contains(constraint.ToString().ToLower())))));
            }

            filterResults.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
            filterResults.Count = results.Count;
            constraint.Dispose();
            return filterResults;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            using (var values = results.Values)
                this.mAdapter.accountCardModelList = values.ToArray<Java.Lang.Object>()
                    .Select(r => r.ToNetObject<AccountCardModel>()).ToList();
            this.mAdapter.UpdatedCardList();
            this.mAdapter.NotifyDataSetChanged();

            // Don't do this and see GREF counts rising
            constraint.Dispose();
            results.Dispose();
        }
    }
}
