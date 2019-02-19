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
using myTNB_Android.Src.Base.Activity;
using Android.Content.PM;
using CheeseBind;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using myTNB_Android.Src.NotificationDetails.MVP;
using Refit;
using AFollestad.MaterialDialogs;
using Android.Support.Design.Widget;
using myTNB_Android.Src.NotificationDetails.Activity.Base;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.NotificationNewBill.Activity;
using myTNB_Android.Src.MakePayment.Activity;
using myTNB_Android.Src.Database.Model;
using Android.Support.V4.Content;
using myTNB_Android.Src.AddAccount.Fragment;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Promotions.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;

namespace myTNB_Android.Src.NotificationDetails.Activity
{

    [Activity(Label = "@string/notification_detail_activity_title"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Notification")]
    public class NotificationDetailActivity : BaseNotificationDetailActivity, NotificationDetailPayableViewableContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtNotificationTitle)]
        TextView txtNotificationTitle;

        [BindView(Resource.Id.txtNotificationContent)]
        TextView txtNotificationContent;

        [BindView(Resource.Id.imageDetails)]
        ImageView imageDetails;

        [BindView(Resource.Id.btnViewDetails)]
        Button btnViewDetails;

        [BindView(Resource.Id.btnPay)]
        Button btnPay;

        NotificationDetailPayableViewableContract.IUserActionsListener userActionsListener;
        NotificationDetailPayableViewablePresenter mPresenter;

        MaterialDialog retrievalDialog;
        private LoadingOverlay loadingOverlay;

        public override View GetRootView()
        {
            return rootView;
        }


        public override int ResourceId()
        {
            return Resource.Layout.NotificationDetailsView;
        }

        public void SetPresenter(NotificationDetailPayableViewableContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }



        public void ShowDetails(AccountData selectedAccount)
        {
            Intent payment_activity = new Intent(this, typeof(NotificationNewBillViewDetailsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            StartActivity(payment_activity);
        }

        public void ShowPayment(AccountData selectedAccount)
        {
            //Intent payment_activity = new Intent(this, typeof(MakePaymentActivity));
            Intent payment_activity = new Intent(this, typeof(SelectAccountsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            StartActivity(payment_activity);
        }

        public void ShowRetrievalProgress()
        {
            //if (retrievalDialog != null && !retrievalDialog.IsShowing)
            //{
            //    retrievalDialog.Show();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
        }

        public void HideRetrievalProgress()
        {
            //if (retrievalDialog != null && retrievalDialog.IsShowing)
            //{
            //    retrievalDialog.Dismiss();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            retrievalDialog = new MaterialDialog.Builder(this)
            .Title(GetString(Resource.String.notification_detail_retrieval_progress_title))
            .Content(GetString(Resource.String.notification_detail_retrieval_progress_content))
            .Cancelable(false)
            .Progress(true, 0)
            .Build();

            TextViewUtils.SetMuseoSans500Typeface(txtNotificationTitle,
                btnViewDetails,
                btnPay);

            TextViewUtils.SetMuseoSans300Typeface(txtNotificationContent);
            // Create your application here

            txtNotificationTitle.Text = notificationDetails.Title;
            txtNotificationContent.Text = notificationDetails.Message;

            mPresenter = new NotificationDetailPayableViewablePresenter(this);

            int count = UserNotificationEntity.Count();
            if (count == 0)
            {
                ME.Leolin.Shortcutbadger.ShortcutBadger.RemoveCount(this.ApplicationContext);
            }
            else
            {
                ME.Leolin.Shortcutbadger.ShortcutBadger.ApplyCount(this.ApplicationContext, count);
            }

            this.userActionsListener.Start();

            if (userNotificationData != null)
            {

                if (userNotificationData.BCRMNotificationTypeId.Equals("01"))
                {
                    imageDetails.SetImageDrawable(ContextCompat.GetDrawable(this , Resource.Drawable.img_notifications_new_bill));
                    btnPay.Visibility = ViewStates.Visible;
                    btnViewDetails.Visibility = ViewStates.Visible;
                }
                else if (userNotificationData.BCRMNotificationTypeId.Equals("02"))
                {
                    imageDetails.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.img_notifications_bill_due));
                    btnPay.Visibility = ViewStates.Visible;
                    btnViewDetails.Visibility = ViewStates.Visible;
                }
                else if (userNotificationData.BCRMNotificationTypeId.Equals("03"))
                {
                    imageDetails.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.img_notifications_dunning));
                    btnPay.Visibility = ViewStates.Visible;
                    btnViewDetails.Visibility = ViewStates.Visible;
                }
                else if (userNotificationData.BCRMNotificationTypeId.Equals("04"))
                {
                    imageDetails.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.img_notifications_disconnection));
                    btnPay.Visibility = ViewStates.Visible;
                    btnViewDetails.Visibility = ViewStates.Visible;
                }
                else if (userNotificationData.BCRMNotificationTypeId.Equals("05"))
                {
                    imageDetails.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.img_notifications_reconnection));
                    btnPay.Visibility = ViewStates.Gone;
                }
                else if (userNotificationData.BCRMNotificationTypeId.Equals("97"))
                {
                    imageDetails.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.img_notification_promotions));
                    btnPay.Visibility = ViewStates.Gone;
                    btnViewDetails.Text = "View Promotion";
                }
                else if (userNotificationData.BCRMNotificationTypeId.Equals("98"))
                {
                    imageDetails.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.img_notification_news));
                    btnPay.Visibility = ViewStates.Gone;
                    btnViewDetails.Visibility = ViewStates.Gone;
                }
                else if (userNotificationData.BCRMNotificationTypeId.Equals("99"))
                {
                    imageDetails.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.img_notification_maintenance));
                    btnPay.Visibility = ViewStates.Gone;
                    btnViewDetails.Visibility = ViewStates.Gone;
                }

            }
        }


        [OnClick(Resource.Id.btnPay)]
        void OnPay(object sender, EventArgs eventArgs)
        {
            this.userActionsListener.OnPayment(notificationDetails);
        }

        [OnClick(Resource.Id.btnViewDetails)]
        void OnViewDetails(object sender, EventArgs eventArgs)
        {
            if (notificationDetails.BCRMNotificationTypeId.Equals("97"))
            {
                this.userActionsListener.OnViewPromotion(notificationDetails);
            }
            else
            {
                this.userActionsListener.OnViewDetails(notificationDetails);
            }
        }

        public void ShowToolbarTitle(string title)
        {
            this.SetToolBarTitle(title);
        }

        public override string ToolbarTitle()
        {
            return userNotificationData.Title;
        }

        public void ShowPromotion(NotificationDetails.Models.NotificationDetails notificationDetails)
        {
            if (notificationDetails != null && !string.IsNullOrEmpty(notificationDetails.Target))
            {
                PromotionsEntityV2 wtManger = new PromotionsEntityV2();
                PromotionsEntityV2 entity = wtManger.GetItemById(notificationDetails.Target);
                wtManger.UpdateItemAsShown(entity);
                Intent details_activity = new Intent(this, typeof(PromotionsActivity));
                details_activity.PutExtra("Promotion", JsonConvert.SerializeObject(entity));
                StartActivity(details_activity);
            }
            else
            {
                Intent dashbaord_activity = new Intent(this, typeof(DashboardActivity));
                dashbaord_activity.PutExtra(Constants.PROMOTION_NOTIFICATION_VIEW, true);
                dashbaord_activity.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(dashbaord_activity);
            }
        }
    }
}