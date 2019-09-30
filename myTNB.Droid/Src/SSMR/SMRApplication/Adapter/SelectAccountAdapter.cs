using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMR.SMRApplication.Adapter
{
    public class SelectAccountAdapter : BaseCustomAdapter<SMRAccount>
    {
        public SelectAccountAdapter(Context context, List<SMRAccount> itemList) : base(context, itemList)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            AccountTypeViewHolder vh = null;
            if (position == (itemList.Count - 1))
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.SelectSMRAccountInfoLayout, parent, false);
            }
            else
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.SelectSupplyAccountItemLayout, parent, false);
                vh = new AccountTypeViewHolder(convertView);

                SMRAccount item = GetItemObject(position);
                vh.txtSupplyAccountName.Text = item.accountName;

                if (item.accountSelected)
                {
                    vh.imageActionIcon.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.imageActionIcon.Visibility = ViewStates.Gone;
                }

                if (item.isTaggedSMR)
                {
                    vh.accountIcon.SetImageResource(Resource.Drawable.smr_48_x_48);
                }
                else
                {
                    vh.accountIcon.SetImageResource(Resource.Drawable.ic_display_normal_meter);
                }
            }
            return convertView;
        }

        public class AccountTypeViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtSupplyAccountName)]
            public TextView txtSupplyAccountName;

            [BindView(Resource.Id.imageActionIcon)]
            public ImageView imageActionIcon;

            [BindView(Resource.Id.imageLeaf)]
            public ImageView accountIcon;

            public AccountTypeViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtSupplyAccountName);
            }
        }
    }
}
