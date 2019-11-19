
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.RearrangeAccount.MVP
{
    public class RearrangeAccountListAdapter : BaseCustomAdapter<CustomerBillingAccount>, IRearrangeAccountListAdapter
    {
        public List<CustomerBillingAccount> Items { get; set; }


        public int mMobileCellPosition { get; set; }

        Activity context;

        public RearrangeAccountListAdapter(Activity context, List<CustomerBillingAccount> items) : base()
        {
            Items = items;
            this.context = context;
            mMobileCellPosition = int.MinValue;
        }
        public RearrangeAccountListAdapter(Android.Content.Context context) : base(context)
        {
        }

        public RearrangeAccountListAdapter(Android.Content.Context context, bool notify) : base(context, notify)
        {
        }

        public RearrangeAccountListAdapter(Android.Content.Context context, List<CustomerBillingAccount> itemList) : base(context, itemList)
        {
        }

        public RearrangeAccountListAdapter(Android.Content.Context context, List<CustomerBillingAccount> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View cell = convertView;
            if (cell == null)
            {
                cell = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, parent, false);
                cell.SetMinimumHeight(150);
                cell.SetBackgroundColor(Color.DarkViolet);
            }

            var text = cell.FindViewById<TextView>(Android.Resource.Id.Text1);
            if (text != null)
            {
                text.Text = position.ToString();
            }

            cell.Visibility = mMobileCellPosition == position ? ViewStates.Invisible : ViewStates.Visible;
            cell.TranslationY = 0;

            return cell;
        }

        public override int Count
        {
            get
            {
                return Items.Count;
            }
        }

        public class RearrangeAccountListViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtSupplyAccountName)]
            public TextView txtSupplyAccountName;

            [BindView(Resource.Id.imageActionIcon)]
            public ImageView imageActionIcon;

            [BindView(Resource.Id.imageLeaf)]
            public ImageView imageLeaf;

            public AccountListViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtSupplyAccountName);


                public void SwapItems(int indexOne, int indexTwo)
        {
            var oldValue = Items[indexOne];
            Items[indexOne] = Items[indexTwo];
            Items[indexTwo] = oldValue;
            mMobileCellPosition = indexTwo;
            NotifyDataSetChanged();
        }

    }
}
