using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Views;
using Android.Widget;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Base.Adapter;
using CheeseBind;
using System.Runtime.Remoting.Contexts;
using Android.Content;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.AddAccount.Adapter
{
    public class AccountTypeAdapter : BaseCustomAdapter<AccountType>
    {
        public AccountTypeAdapter(Android.Content.Context context) : base(context)
        {
        }

        public AccountTypeAdapter(Android.Content.Context context, bool notify) : base(context, notify)
        {
        }

        public AccountTypeAdapter(Android.Content.Context context, List<AccountType> itemList) : base(context, itemList)
        {
        }

        public AccountTypeAdapter(Android.Content.Context context, List<AccountType> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            AccountTypeViewHolder vh = null;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.SelectSupplyAccountRow, parent, false);
                vh = new AccountTypeViewHolder(convertView);
                convertView.Tag = vh;
            }
            else
            {
                vh = convertView.Tag as AccountTypeViewHolder;

            }

            AccountType item = GetItemObject(position);
            vh.txtSupplyAccountName.Text = item.Type;

            if (item.IsSelected)
            {
                //vh.txtSupplyAccountName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.ic_action_tick, 0);
                vh.imageActionIcon.Visibility = ViewStates.Visible;
            }
            else
            {
                //vh.txtSupplyAccountName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                vh.imageActionIcon.Visibility = ViewStates.Gone;
            }

            return convertView;
        }

    }

    public class AccountTypeViewHolder : BaseAdapterViewHolder
    {
        [BindView(Resource.Id.txtSupplyAccountName)]
        public TextView txtSupplyAccountName;

        [BindView(Resource.Id.imageActionIcon)]
        public ImageView imageActionIcon;

        public AccountTypeViewHolder(View itemView) : base(itemView)
        {
            TextViewUtils.SetMuseoSans300Typeface(txtSupplyAccountName);
        }
    }
}