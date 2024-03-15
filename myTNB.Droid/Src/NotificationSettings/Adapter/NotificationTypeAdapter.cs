
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Adapter;
using myTNB.AndroidApp.Src.SelectNotification.Models;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.NotificationSettings.Adapter
{
    public class NotificationTypeAdapter : BaseRecyclerAdapter<NotificationTypeUserPreference>
    {
        public NotificationTypeAdapter(bool notify) : base(notify)
        {
        }

        public NotificationTypeAdapter(List<NotificationTypeUserPreference> itemList) : base(itemList)
        {
        }

        public NotificationTypeAdapter(List<NotificationTypeUserPreference> itemList, bool notify) : base(itemList, notify)
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
                var viewHolder = holder as NotificationTypeViewHolder;
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
            return new NotificationTypeViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.NotificationTypeUserPreferenceRow, parent, false), OnClickEvent);
        }

        class NotificationTypeViewHolder : BaseRecyclerViewHolder
        {
            [BindView(Resource.Id.txtNotificationTitle)]
            public TextView txtNotificationTitle;

            [BindView(Resource.Id.notificationActionSwitch)]
            public SwitchCompat notificationActionSwitch;

            public NotificationTypeViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtNotificationTitle);
                TextViewUtils.SetTextSize14(txtNotificationTitle);
                notificationActionSwitch.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}