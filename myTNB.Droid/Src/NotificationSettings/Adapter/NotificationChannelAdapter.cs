
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.SelectNotification.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.NotificationSettings.Adapter
{
    public class NotificationChannelAdapter : BaseRecyclerAdapter<NotificationChannelUserPreference>
    {
        public NotificationChannelAdapter(bool notify) : base(notify)
        {
        }

        public NotificationChannelAdapter(List<NotificationChannelUserPreference> itemList) : base(itemList)
        {
        }

        public NotificationChannelAdapter(List<NotificationChannelUserPreference> itemList, bool notify) : base(itemList, notify)
        {
        }

        public event EventHandler<int> ClickEvent;



        public void OnClickEvent(int position)
        {
            if (ClickEvent != null)
            {
                ClickEvent(this, position);
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                var userPref = GetItemObject(position);
                var viewHolder = holder as NotificationChannelViewHolder;
                viewHolder.txtNotificationTitle.Text = userPref.Title;

                if (userPref.IsOpted)
                {
                    viewHolder.notificationActionSwitch.Checked = true;
                }
                else
                {
                    viewHolder.notificationActionSwitch.Checked = false;

                }

                if (userPref.PreferenceMode.Equals("M")) // MANDATORY
                {
                    viewHolder.notificationActionSwitch.Enabled = false;
                }
                else
                {
                    viewHolder.notificationActionSwitch.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new NotificationChannelViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.NotificationChannelUserPreferenceRow, parent, false), OnClickEvent);

        }

        class NotificationChannelViewHolder : BaseRecyclerViewHolder
        {
            [BindView(Resource.Id.txtNotificationTitle)]
            public TextView txtNotificationTitle;

            [BindView(Resource.Id.notificationActionSwitch)]
            public SwitchCompat notificationActionSwitch;

            public NotificationChannelViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtNotificationTitle);
                txtNotificationTitle.TextSize = TextViewUtils.GetFontSize(14f);
                notificationActionSwitch.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}