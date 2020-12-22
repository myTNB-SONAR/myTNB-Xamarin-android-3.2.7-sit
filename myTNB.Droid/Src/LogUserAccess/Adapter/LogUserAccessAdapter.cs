using Android.Content;

using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.LogUserAccess.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.LogUserAccess.Adapter
{
    public class LogUserAccessAdapter : BaseCustomAdapter<LogUserAccessData>
    {
        public LogUserAccessAdapter(Context context) : base(context)
        {
        }

        public LogUserAccessAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public LogUserAccessAdapter(Context context, List<LogUserAccessData> itemList) : base(context, itemList)
        {
        }

        public LogUserAccessAdapter(Context context, List<LogUserAccessData> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public void DisableAll()
        {
            foreach (LogUserAccessData data in itemList)
            {
                //data.IsSelected = false;
            }
            NotifyDataSetChanged();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LogUserAccessData data = GetItemObject(position);
            LogUserAccessDataViewHolder viewHolder = null;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.NotificationFilterRow, parent, false);
                viewHolder = new LogUserAccessDataViewHolder(convertView);
                convertView.Tag = viewHolder;
            }
            else
            {
                viewHolder = convertView.Tag as LogUserAccessDataViewHolder;
            }
            try
            {
                /*viewHolder.txtNotificationTitle.Text = data.Title;
                if (data.IsSelected)
                {
                    viewHolder.notificationActionIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_action_tick));
                }
                else
                {
                    viewHolder.notificationActionIcon.SetImageDrawable(null);

                }*/
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return convertView;
        }

        class LogUserAccessDataViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtNotificationTitle)]
            public TextView txtNotificationTitle;

            [BindView(Resource.Id.notificationActionIcon)]
            public ImageView notificationActionIcon;

            public LogUserAccessDataViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtNotificationTitle);
            }
        }
    }
}