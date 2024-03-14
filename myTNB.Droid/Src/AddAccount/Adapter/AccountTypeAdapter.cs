using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.Android.Src.AddAccount.Models;
using myTNB.Android.Src.Base.Adapter;
using myTNB.Android.Src.Utils;
using System.Collections.Generic;

namespace myTNB.Android.Src.AddAccount.Adapter
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
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.SelectAccountTypeRow, parent, false);
                vh = new AccountTypeViewHolder(convertView);
                convertView.Tag = vh;
            }
            else
            {
                vh = convertView.Tag as AccountTypeViewHolder;

            }

            AccountType item = GetItemObject(position);
            vh.txtSupplyAccountName.Text = item.Type;
            TextViewUtils.SetTextSize16(vh.txtSupplyAccountName);

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

    public class AccountTypeViewHolder : BaseAdapterViewHolder
    {
        [BindView(Resource.Id.txtSupplyAccountName)]
        public TextView txtSupplyAccountName;

        [BindView(Resource.Id.imageActionIcon)]
        public ImageView imageActionIcon;

        public AccountTypeViewHolder(View itemView) : base(itemView)
        {
            TextViewUtils.SetMuseoSans300Typeface(txtSupplyAccountName);
            TextViewUtils.SetTextSize16(txtSupplyAccountName);
        }
    }
}