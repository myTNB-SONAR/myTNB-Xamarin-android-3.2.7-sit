using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.XDetailRegistrationForm.Models;
using myTNB.AndroidApp.Src.Base.Adapter;
using myTNB.AndroidApp.Src.Utils;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.XDetailRegistrationForm.Adapter
{
    public class IdentificationTypeAdapter : BaseCustomAdapter<IdentificationType>
    {
        public IdentificationTypeAdapter(Android.Content.Context context) : base(context)
        {
        }

        public IdentificationTypeAdapter(Android.Content.Context context, bool notify) : base(context, notify)
        {
        }

        public IdentificationTypeAdapter(Android.Content.Context context, List<IdentificationType> itemList) : base(context, itemList)
        {
        }

        public IdentificationTypeAdapter(Android.Content.Context context, List<IdentificationType> itemList, bool notify) : base(context, itemList, notify)
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

            IdentificationType item = GetItemObject(position);
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
            TextViewUtils.SetTextSize16(txtSupplyAccountName);
        }
    }
}