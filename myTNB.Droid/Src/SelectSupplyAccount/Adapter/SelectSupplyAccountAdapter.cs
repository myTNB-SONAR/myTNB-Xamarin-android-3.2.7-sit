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
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Base.Adapter;
using CheeseBind;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;

namespace myTNB_Android.Src.Dashboard.Adapter
{
    public class SelectSupplyAccountAdapter : BaseCustomAdapter<CustomerBillingAccount>
    {
        public SelectSupplyAccountAdapter(Context context) : base(context)
        {
        }

        public SelectSupplyAccountAdapter(Context context , bool notify) : base(context , notify)
        {

        }



        public SelectSupplyAccountAdapter(Context context, List<CustomerBillingAccount> itemList) : base(context, itemList)
        {
        }



        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            AccountListViewHolder vh = null;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.SelectSupplyAccountRow , parent , false) ;
                vh = new AccountListViewHolder(convertView);
                convertView.Tag = vh;
            }
            else
            {
                vh = convertView.Tag as AccountListViewHolder;

            }
            try {
            CustomerBillingAccount item = GetItemObject(position);
            vh.txtSupplyAccountName.Text = item.AccDesc;

            if (item.AccountCategoryId.Equals("2"))
            {
                vh.imageLeaf.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.imageLeaf.Visibility = ViewStates.Invisible;
            }

            if (item.IsSelected)
            {
                vh.imageActionIcon.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.imageActionIcon.Visibility = ViewStates.Invisible;

            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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

        [BindView(Resource.Id.imageLeaf)]
        public ImageView imageLeaf;

        public AccountListViewHolder(View itemView) : base(itemView)
        {
            TextViewUtils.SetMuseoSans300Typeface(txtSupplyAccountName);
        }
    }
}