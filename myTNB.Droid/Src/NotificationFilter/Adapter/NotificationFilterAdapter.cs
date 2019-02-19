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
using myTNB_Android.Src.NotificationFilter.Models;
using myTNB_Android.Src.Base.Adapter;
using CheeseBind;
using Android.Support.V4.Content;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.NotificationFilter.Adapter
{
    public class NotificationFilterAdapter : BaseCustomAdapter<NotificationFilterData>
    {
        public NotificationFilterAdapter(Context context) : base(context)
        {
        }

        public NotificationFilterAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public NotificationFilterAdapter(Context context, List<NotificationFilterData> itemList) : base(context, itemList)
        {
        }

        public NotificationFilterAdapter(Context context, List<NotificationFilterData> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public void DisableAll()
        { 
            foreach (NotificationFilterData data in itemList)
            {
                data.IsSelected = false;
            }
            NotifyDataSetChanged();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            NotificationFilterData data = GetItemObject(position);
            NotificationFilterViewHolder viewHolder = null;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.NotificationFilterRow , parent , false);
                viewHolder = new NotificationFilterViewHolder(convertView);
                convertView.Tag = viewHolder;
            }
            else
            {
                viewHolder = convertView.Tag as NotificationFilterViewHolder;
            }

            viewHolder.txtNotificationTitle.Text = data.Title;
            if (data.IsSelected)
            {
                viewHolder.notificationActionIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_action_tick));
            }
            else
            {
                viewHolder.notificationActionIcon.SetImageDrawable(null);

            }

            return convertView;
        }

        class NotificationFilterViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtNotificationTitle)]
            public TextView txtNotificationTitle;

            [BindView(Resource.Id.notificationActionIcon)]
            public ImageView notificationActionIcon;

            public NotificationFilterViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtNotificationTitle);
            }
        }
    }
}