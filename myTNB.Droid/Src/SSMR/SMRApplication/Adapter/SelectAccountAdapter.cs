using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.Android.Src.Base.Adapter;
using myTNB.Android.Src.SSMR.SMRApplication.MVP;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.SSMR.SMRApplication.Adapter
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
                TextView textView = convertView.FindViewById<TextView>(Resource.Id.whyAccountsNotHere);
                TextViewUtils.SetMuseoSans500Typeface(textView);
                TextViewUtils.SetTextSize12(textView);
                textView.Text = Utility.GetLocalizedLabel("SelectElectricityAccounts", "accountsMissing");
            }
            else
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.SelectSupplyAccountItemLayout, parent, false);
                vh = new AccountTypeViewHolder(convertView);

                SMRAccount item = GetItemObject(position);
                vh.txtSupplyAccountName.Text = item.accountName;
                TextViewUtils.SetTextSize16(vh.txtSupplyAccountName);

                if (item.accountSelected)
                {
                    vh.imageActionIcon.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.imageActionIcon.Visibility = ViewStates.Gone;
                }

                vh.accountIcon.Visibility = ViewStates.Gone;
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
                TextViewUtils.SetTextSize16(txtSupplyAccountName);
            }
        }
    }
}
