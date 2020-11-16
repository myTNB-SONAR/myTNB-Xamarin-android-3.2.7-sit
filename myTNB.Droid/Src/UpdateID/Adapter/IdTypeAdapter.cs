using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.UpdateID.Models;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Utils;
using System.Collections.Generic;

namespace myTNB_Android.Src.UpdateID.Adapter
{
    public class IdTypeAdapter : BaseCustomAdapter<IdType>
    {
        public IdTypeAdapter(Android.Content.Context context) : base(context)
        {
        }

        public IdTypeAdapter(Android.Content.Context context, bool notify) : base(context, notify)
        {
        }

        public IdTypeAdapter(Android.Content.Context context, List<IdType> itemList) : base(context, itemList)
        {
        }

        public IdTypeAdapter(Android.Content.Context context, List<IdType> itemList, bool notify) : base(context, itemList, notify)
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

            IdType item = GetItemObject(position);
            vh.txtSupplyAccountName.Text = item.Type;

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
        }
    }
}