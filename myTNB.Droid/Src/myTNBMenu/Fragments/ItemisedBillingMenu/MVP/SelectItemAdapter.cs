using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP
{
    public class SelectItemAdapter : BaseCustomAdapter<Item>
    {
        public SelectItemAdapter(Context context) : base(context)
        {
        }

        public SelectItemAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public SelectItemAdapter(Context context, List<Item> itemList) : base(context, itemList)
        {
        }

        public SelectItemAdapter(Context context, List<Item> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            SelectItemViewHolder vh;
            convertView = LayoutInflater.From(context).Inflate(Resource.Layout.SelectItemListLayout, parent, false);
            vh = new SelectItemViewHolder(convertView);
            Item item = GetItemObject(position);
            vh.txtItemTitle.Text = item.title;
            vh.imageActionIcon.Visibility = item.selected ? ViewStates.Visible : ViewStates.Gone;
            return convertView;
        }

        public class SelectItemViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtItemTitle)]
            public TextView txtItemTitle;

            [BindView(Resource.Id.imageActionIcon)]
            public ImageView imageActionIcon;

            public SelectItemViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtItemTitle);
            }
        }
    }
}
