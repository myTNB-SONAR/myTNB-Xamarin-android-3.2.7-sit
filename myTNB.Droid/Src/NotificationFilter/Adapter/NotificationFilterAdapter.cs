using Android.Content;

using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.NotificationFilter.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

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
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.NotificationFilterRow, parent, false);
                viewHolder = new NotificationFilterViewHolder(convertView);
                convertView.Tag = viewHolder;
            }
            else
            {
                viewHolder = convertView.Tag as NotificationFilterViewHolder;
            }
            try
            {
                viewHolder.txtNotificationTitle.Text = data.Title;
                if (data.IsSelected)
                {
                    viewHolder.notificationActionIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_action_tick));
                }
                else
                {
                    viewHolder.notificationActionIcon.SetImageDrawable(null);

                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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
                txtNotificationTitle.TextSize = TextViewUtils.GetFontSize(16f);

            }
        }
    }
}