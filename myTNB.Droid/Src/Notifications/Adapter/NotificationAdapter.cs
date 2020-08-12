using Android.Content;

using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.Notifications.Adapter
{
    public class NotificationAdapter : BaseCustomAdapter<UserNotificationData>
    {



        public NotificationAdapter(Context context) : base(context)
        {
        }

        public NotificationAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public NotificationAdapter(Context context, List<UserNotificationData> itemList) : base(context, itemList)
        {
        }

        public NotificationAdapter(Context context, List<UserNotificationData> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            SimpleDateFormat simpleDateTimeParser = new SimpleDateFormat("MM/dd/yyyy hh:mm:ss a");
            SimpleDateFormat simpleDateTimeFormat = new SimpleDateFormat("dd MMM yyyy");
            NotificationViewHolder viewHolder = null;
            UserNotificationData notificationData = GetItemObject(position);
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.NotificationRow, parent, false);
                viewHolder = new NotificationViewHolder(convertView);

                convertView.Tag = viewHolder;
            }
            else
            {
                viewHolder = convertView.Tag as NotificationViewHolder;
            }
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
                    viewHolder.txtNotificationDate.SetCompoundDrawablesWithIntrinsicBounds(null, null, ContextCompat.GetDrawable(context, Resource.Drawable.ic_notifications_unread), null);
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

                if (notificationData.BCRMNotificationTypeId.Equals("01"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_notification_billing));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals("02"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_notification_billing_due));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals("03"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_notification_power));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals("04"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_notification_connection));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals("05"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_notification_connection));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals("97"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_notification_promo));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals("98"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_notification_news));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals("99"))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_notification_maintenance));
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
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return convertView;
        }


        class NotificationViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.notificationIcon)]
            public ImageView notificationIcon;

            [BindView(Resource.Id.txtNotificationTitle)]
            public TextView txtNotificationTitle;

            [BindView(Resource.Id.txtNotificationContent)]
            public TextView txtNotificationContent;

            [BindView(Resource.Id.txtNotificationDate)]
            public TextView txtNotificationDate;


            public NotificationViewHolder(View itemView) : base(itemView)
            {

                TextViewUtils.SetMuseoSans300Typeface(txtNotificationTitle);
                TextViewUtils.SetMuseoSans300Typeface(txtNotificationContent, txtNotificationDate);

            }
        }
    }
}