﻿
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
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.RearrangeAccount.MVP
{
    public class RearrangeAccountListAdapter : BaseCustomAdapter<CustomerBillingAccount>, IRearrangeAccountListAdapter
    {
        public List<CustomerBillingAccount> Items { get; set; }


        public int mMobileCellPosition { get; set; }

        Context context;


        public RearrangeAccountListAdapter(Context context) : base(context)
        {
        }

        public RearrangeAccountListAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public RearrangeAccountListAdapter(Context context, List<CustomerBillingAccount> itemList) : base(context, itemList)
        {
            Items = itemList;
            this.context = context;
            mMobileCellPosition = int.MinValue;
        }

        public RearrangeAccountListAdapter(Android.Content.Context context, List<CustomerBillingAccount> itemList, bool notify) : base(context, itemList, notify)
        {

        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            RearrangeAccountListViewHolder vh = null;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.AccountRearrangeItemLayout, parent, false);
                vh = new RearrangeAccountListViewHolder(convertView);
                convertView.Tag = vh;
            }
            else
            {
                vh = convertView.Tag as RearrangeAccountListViewHolder;

            }
            try
            {
                CustomerBillingAccount item = GetItemObject(position);
                vh.txtSupplyAccountName.Text = item.AccDesc;

                if (item.AccountCategoryId.Equals("2"))
                {
                    vh.imageLeaf.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.imageLeaf.Visibility = ViewStates.Gone;
                }

                vh.imageActionIcon.Visibility = ViewStates.Visible;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return convertView;
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

            public RearrangeAccountListViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtSupplyAccountName);
            }
        }


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
