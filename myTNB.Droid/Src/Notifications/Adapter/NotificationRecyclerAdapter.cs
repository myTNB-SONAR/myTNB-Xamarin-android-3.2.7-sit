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
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Base.Adapter;
using CheeseBind;

using myTNB_Android.Src.Utils;

using Java.Text;
using Java.Util;
using myTNB_Android.Src.Notifications.MVP;
using static Android.Widget.CompoundButton;
using myTNB_Android.Src.Base;
using System.Text.RegularExpressions;
using AndroidX.RecyclerView.Widget;
using AndroidX.Core.Content;
using myTNB_Android.Src.Database.Model;

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

        public NotificationRecyclerAdapter(Context context, NotificationContract.IView notificatonListener, bool notify) : base(notify)
        {
            notifyContext = context;
            mNotificatonListener = notificatonListener;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SimpleDateFormat simpleDateTimeParser = new SimpleDateFormat("MM/dd/yyyy hh:mm:ss a", LocaleUtils.GetDefaultLocale());
            SimpleDateFormat simpleDateTimeFormat = new SimpleDateFormat("dd MMM yyyy", LocaleUtils.GetCurrentLocale());
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
                    viewHolder.txtNotificationDate.CompoundDrawablePadding = (int)DPUtils.ConvertDPToPx(4f);
                    viewHolder.txtNotificationDate.SetCompoundDrawablesWithIntrinsicBounds(null, null, ContextCompat.GetDrawable(notifyContext, Resource.Drawable.ic_notifications_unread), null);
                    TextViewUtils.SetMuseoSans500Typeface(viewHolder.txtNotificationTitle, viewHolder.txtNotificationContent, viewHolder.txtNotificationDate);
                    viewHolder.txtNotificationTitle.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(notifyContext, Resource.Color.tunaGrey)));
                    viewHolder.txtNotificationDate.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(notifyContext, Resource.Color.tunaGrey)));
                    viewHolder.txtNotificationContent.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(notifyContext, Resource.Color.silverChalice)));
                }
                else
                {
                    viewHolder.txtNotificationDate.CompoundDrawablePadding = 0;
                    viewHolder.txtNotificationDate.SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
                    TextViewUtils.SetMuseoSans300Typeface(viewHolder.txtNotificationTitle, viewHolder.txtNotificationContent, viewHolder.txtNotificationDate);
                    viewHolder.txtNotificationTitle.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(notifyContext, Resource.Color.tunaGrey)));
                    viewHolder.txtNotificationDate.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(notifyContext, Resource.Color.silverChalice)));
                    viewHolder.txtNotificationContent.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(notifyContext, Resource.Color.silverChalice)));
                }

                if (notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_NEW_BILL_ID))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_new_bill));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_BILL_DUE_ID))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_bill_due));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_DISCONNECT_NOTICE_ID))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_dunning_disconnection));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_DISCONNECTED_ID))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_disconnection));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_RECONNECTED_ID))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_reconnection));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_METER_READING_OPEN_ID) ||
                    notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_METER_READING_REMIND_ID) ||
                    notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_SMR_DISABLED_ID) ||
                    notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_SMR_APPLY_SUCCESS_ID) ||
                    notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_SMR_APPLY_FAILED_ID) ||
                    notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_SMR_DISABLED_SUCCESS_ID) ||
                    notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_SMR_DISABLED_FAILED_ID))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_smr));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_MAINTENANCE_ID))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_settings));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_PAYMENT_FAILED_ID))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_payment_failed));
                }
                else if (notificationData.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_PAYMENT_SUCCESS_ID))
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_payment_success));
                }
                else
                {
                    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_generic));
                }

                viewHolder.txtNotificationTitle.Text = notificationData.Title;

                string notificationAccountName = MyTNBAccountManagement.GetInstance().GetNotificationAccountName(notificationData.AccountNum);
                viewHolder.txtNotificationContent.Text = Regex.Replace(notificationData.Message, Constants.ACCOUNT_NICKNAME_PATTERN, notificationAccountName);

                if (notificationData.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_NEW_ACCOUNT_ADDED)
                {
                    string message = Regex.Replace(notificationData.Message, Constants.ACCOUNT_FULLNAME_PATTERN, UserEntity.GetActive().DisplayName + "/" + UserEntity.GetActive().Email);
                    message = Regex.Replace(message, Constants.ACCOUNT_NICKNAME_PATTERN, notificationAccountName);
                    viewHolder.txtNotificationContent.Text = message;

                }

                if (notificationData.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_NEW_ACCESS_ADDED)
                {
                    CustomerBillingAccount account = CustomerBillingAccount.FindByAccNum(notificationData.AccountNum);
                    string address = Utility.StringSpaceMasking(Utility.Masking.Address, account.AccountStAddress);
                    string message = Regex.Replace(notificationData.Message, Constants.ACCOUNT_FULLNAME_PATTERN, UserEntity.GetActive().DisplayName);
                    message = Regex.Replace(message, Constants.ACCOUNT_ADDRESS_PATTERN, address);

                    viewHolder.txtNotificationContent.Text = message;

                }

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
                TextViewUtils.SetMuseoSans500Typeface(txtNotificationTitle);
                TextViewUtils.SetMuseoSans300Typeface(txtNotificationContent, txtNotificationDate);
                txtNotificationTitle.TextSize = TextViewUtils.GetFontSize(14f);
                txtNotificationContent.TextSize = TextViewUtils.GetFontSize(10f);
                txtNotificationDate.TextSize = TextViewUtils.GetFontSize(10f);
                itemView.SetOnLongClickListener(new NotificationLongItemClickListener(this, adapter));
                itemView.SetOnClickListener(new NotificationItemClickListener(this, adapter));
                selectItemCheckbox.SetOnCheckedChangeListener(new NotificationItemSelectedListener(this, adapter));
            }
        }

        class NotificationLongItemClickListener : Java.Lang.Object, View.IOnLongClickListener
        {
            NotificationRecyclerViewHolder mViewHolder;
            NotificationRecyclerAdapter mAdapter;
            public NotificationLongItemClickListener(NotificationRecyclerViewHolder viewHolder, NotificationRecyclerAdapter adapter)
            {
                mViewHolder = viewHolder;
                mAdapter = adapter;
            }

            public bool OnLongClick(View v)
            {
                if (isClickable)
                {
                    mNotificatonListener.ShowEditMode();
                }
                return true;
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
