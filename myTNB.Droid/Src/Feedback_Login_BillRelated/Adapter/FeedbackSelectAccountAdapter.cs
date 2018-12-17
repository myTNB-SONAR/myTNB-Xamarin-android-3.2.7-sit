﻿using System;
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
using CheeseBind;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.Feedback_Login_BillRelated.Adapter
{
    public class FeedbackSelectAccountAdapter : BaseCustomAdapter<AccountData>
    {
        public FeedbackSelectAccountAdapter(Context context) : base(context)
        {
        }

        public FeedbackSelectAccountAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public FeedbackSelectAccountAdapter(Context context, List<AccountData> itemList) : base(context, itemList)
        {
        }

        public FeedbackSelectAccountAdapter(Context context, List<AccountData> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            AccountListViewHolder vh = null;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.FeedbackSelectAccountRow, parent, false);
                vh = new AccountListViewHolder(convertView);
                convertView.Tag = vh;
            }
            else
            {
                vh = convertView.Tag as AccountListViewHolder;

            }

            AccountData item = GetItemObject(position);
            vh.txtSupplyAccountName.Text = item.AccountNickName;

            if (item.IsSelected)
            {
                vh.imageActionIcon.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.imageActionIcon.Visibility = ViewStates.Gone;

            }

            return convertView;
        }

    }

    public class AccountListViewHolder : BaseAdapterViewHolder
    {
        [BindView(Resource.Id.txtSupplyAccountName)]
        public TextView txtSupplyAccountName;

        [BindView(Resource.Id.imageActionIcon)]
        public ImageView imageActionIcon;

        public AccountListViewHolder(View itemView) : base(itemView)
        {
            TextViewUtils.SetMuseoSans300Typeface(txtSupplyAccountName);
        }
    }

}