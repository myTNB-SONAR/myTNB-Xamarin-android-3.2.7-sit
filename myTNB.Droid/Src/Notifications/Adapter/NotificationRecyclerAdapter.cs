﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Base.Adapter;
using CheeseBind;
using Android.Support.V4.Content;
using myTNB_Android.Src.Utils;
using Android.Support.V7.Widget;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Notifications.MVP;
using static Android.Widget.CompoundButton;

namespace myTNB_Android.Src.Notifications.Adapter
{
    public class NotificationRecyclerAdapter : BaseRecyclerAdapter<UserNotificationData>
    {
        Context notifyContext;
        private static NotificationContract.IView mNotificatonListener;
        private static bool isClickable = true;
        public NotificationRecyclerAdapter(bool notify) : base(notify)
        {
        }

        public NotificationRecyclerAdapter(List<UserNotificationData> itemList) : base(itemList)
        {
        }

        public NotificationRecyclerAdapter(List<UserNotificationData> itemList, bool notify) : base(itemList, notify)
        {
        }

        //public NotificationRecyclerAdapter(Context context, bool notify) : base(notify)
        //{
        //    notifyContext = context;
        //}

        public NotificationRecyclerAdapter(Context context, NotificationContract.IView notificatonListener, bool notify) : base(notify)
        {
            notifyContext = context;
            mNotificatonListener = notificatonListener;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SimpleDateFormat simpleDateTimeParser = new SimpleDateFormat("MM/dd/yyyy hh:mm:ss a");
            SimpleDateFormat simpleDateTimeFormat = new SimpleDateFormat("dd MMM yyyy");
            NotificationRecyclerViewHolder viewHolder = holder as NotificationRecyclerViewHolder;
            UserNotificationData notificationData = GetItemObject(position);
            try
            {
                Date d = null;
                try
                {
                    d = simpleDateTimeParser.Parse(notificationData.CreatedDate);
                }
                catch (Java.Text.ParseException e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                if (d != null)
                {
                    viewHolder.txtNotificationDate.Text = simpleDateTimeFormat.Format(d);
                }
                else
                {
                    viewHolder.txtNotificationDate.Text = notificationData.CreatedDate;
                }

                if (!notificationData.IsRead)
                {
                    viewHolder.txtNotificationDate.SetCompoundDrawablesWithIntrinsicBounds(null, null, ContextCompat.GetDrawable(notifyContext, Resource.Drawable.ic_notifications_unread), null);
                }
                else
                {
                    viewHolder.txtNotificationDate.SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);

                }

                //01 New Bill
                //02 Bill Due
                //03 Dunning Disconnection Notice
                //04 Disconnection
                //05 Reconnection
                //06 SMR

                if (notificationData.BCRMNotificationTypeId.Equals("01"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_new_bill));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals("02"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_bill_due));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals("03"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_dunning_disconnection));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals("04"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_disconnection));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals("05"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_reconnection));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals("06"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_smr));
                }
                //else if (notificationData.BCRMNotificationTypeId.Equals("97"))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.ic_notification_promo));
                //}
                //else if (notificationData.BCRMNotificationTypeId.Equals("98"))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.ic_notification_news));
                //}
                else if (notificationData.BCRMNotificationTypeId.Equals("99"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_settings));
                }

                //if (notificationData.Code.Equals(Constants.NOTIFICATION_CODE_BP))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context , Resource.Drawable.ic_notification_billing));
                //}
                //else if (notificationData.Code.Equals(Constants.NOTIFICATION_CODE_ACC))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_notification_power));
                //}
                //else if (notificationData.Code.Equals(Constants.NOTIFICATION_CODE_PO))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_notification_power));
                //}
                //else if (notificationData.Code.Equals(Constants.NOTIFICATION_CODE_PRO))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_notification_promo));

                //}
                //else if (notificationData.Code.Equals(Constants.NOTIFICATION_CODE_REW))
                //{
                //    //viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_notification_rew));

                //}

                viewHolder.txtNotificationTitle.Text = notificationData.Title;

                viewHolder.txtNotificationContent.Text = notificationData.Message;

                if (notificationData.ShowSelectButton)
                {
                    viewHolder.selectItemCheckbox.Visibility = ViewStates.Visible;
                    viewHolder.txtNotificationContent.SetEms(18);
                }
                else
                {
                    viewHolder.selectItemCheckbox.Visibility = ViewStates.Gone;
                    viewHolder.txtNotificationContent.SetEms(20);
                }

                viewHolder.selectItemCheckbox.Checked = notificationData.IsSelected;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new NotificationRecyclerViewHolder(this, LayoutInflater.From(parent.Context).Inflate(Resource.Layout.NotificationRow, parent, false));
        }

        public void RemoveItem(int position)
        {
            Remove(position);
        }

        public void ShowSelectButtons(bool show)
        {
            foreach(UserNotificationData userNotificationData in this.itemList)
            {
                userNotificationData.ShowSelectButton = show;
            }
            NotifyDataSetChanged();
        }

        public void SelectAllNotifications(bool show)
        {
            for (int i=0; i < this.itemList.Count; i++)
            {
                this.itemList[i].IsSelected = show;
            }
            NotifyDataSetChanged();
        }

        public List<UserNotificationData> GetAllNotifications()
        {
            return this.itemList;
        }

        class NotificationItemSelectedListener : Java.Lang.Object, IOnCheckedChangeListener
        {
            NotificationRecyclerViewHolder mViewHolder;
            NotificationRecyclerAdapter mAdapter;

            public NotificationItemSelectedListener(NotificationRecyclerViewHolder viewHolder, NotificationRecyclerAdapter adapter)
            {
                mViewHolder = viewHolder;
                mAdapter = adapter;
            }

            public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
            {
                mAdapter.itemList[mViewHolder.AdapterPosition].IsSelected = isChecked;
                mNotificatonListener.UpdatedSelectedNotifications();
            }
        }

        class NotificationRecyclerViewHolder : BaseRecyclerViewHolder
        {
            [BindView(Resource.Id.notificationIcon)]
            public ImageView notificationIcon;

            [BindView(Resource.Id.txtNotificationTitle)]
            public TextView txtNotificationTitle;

            [BindView(Resource.Id.txtNotificationContent)]
            public TextView txtNotificationContent;

            [BindView(Resource.Id.txtNotificationDate)]
            public TextView txtNotificationDate;

            [BindView(Resource.Id.selectItemCheckBox)]
            public CheckBox selectItemCheckbox;

            public NotificationRecyclerViewHolder(NotificationRecyclerAdapter adapter, View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtNotificationTitle);
                TextViewUtils.SetMuseoSans300Typeface(txtNotificationContent, txtNotificationDate);
                itemView.SetOnClickListener(new NotificationItemClickListener(this, adapter));
                selectItemCheckbox.SetOnCheckedChangeListener(new NotificationItemSelectedListener(this, adapter));
            }
        }

        class NotificationItemClickListener : Java.Lang.Object, View.IOnClickListener
        {
            NotificationRecyclerViewHolder mViewHolder;
            NotificationRecyclerAdapter mAdapter;
            public NotificationItemClickListener(NotificationRecyclerViewHolder viewHolder, NotificationRecyclerAdapter adapter)
            {
                mViewHolder = viewHolder;
                mAdapter = adapter;
            }
            public void OnClick(View v)
            {
                if (isClickable)
                {
                    int position = mViewHolder.AdapterPosition;
                    mNotificatonListener.ShowNotificationDetails(position);
                }
            }
        }

        public void SetClickable(bool isClick)
        {
            isClickable = isClick;
        }
    }
}
