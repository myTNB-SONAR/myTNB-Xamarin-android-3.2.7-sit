﻿using System;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Billing.MVP;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.NotificationDetails.MVP;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.NotificationDetails.Activity
{
    [Activity(Label = "Notification Detail", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class UserNotificationDetailActivity : BaseToolbarAppCompatActivity, UserNotificationDetailContract.IView
    {
        [BindView(Resource.Id.notificationDetailBannerImg)]
        ImageView notificationDetailBannerImg;

        [BindView(Resource.Id.notificationDetailTitle)]
        TextView notificationDetailTitle;

        [BindView(Resource.Id.notificationDetailMessage)]
        TextView notificationDetailMessage;

        [BindView(Resource.Id.rootView)]
        ViewGroup rootView;

        Models.NotificationDetails notificationDetails;
        UserNotificationData userNotificationData;
        int position;
        UserNotificationDetailPresenter mPresenter;
        AlertDialog removeDialog;
        private LoadingOverlay loadingOverlay;

        public override int ResourceId()
        {
            return Resource.Layout.UserNotificationDetailLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.NotificationDetailMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_delete_notification:
                    removeDialog = new AlertDialog.Builder(this)

                        .SetTitle(Resource.String.notification_detail_remove_notification_dialog_title)
                        .SetMessage(GetString(Resource.String.notification_detail_remove_notification_dialog_content))
                        .SetNegativeButton(Resource.String.notification_detail_remove_notification_negative_btn,
                        delegate
                        {
                            removeDialog.Dismiss();
                        })
                        .SetPositiveButton(Resource.String.notification_detail_remove_notification_positive_btn,
                        delegate
                        {
                            //this.userActionsListener.OnRemoveNotification(notificationDetails);
                        })
                        .Show();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                mPresenter = new UserNotificationDetailPresenter(this);
                base.OnCreate(savedInstanceState);
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM))
                    {
                        notificationDetails = DeSerialze<NotificationDetails.Models.NotificationDetails>(extras.GetString(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM));
                    }

                    if (extras.ContainsKey(Constants.SELECTED_NOTIFICATION_LIST_ITEM))
                    {
                        userNotificationData = DeSerialze<UserNotificationData>(extras.GetString(Constants.SELECTED_NOTIFICATION_LIST_ITEM));
                    }

                    position = extras.GetInt(Constants.SELECTED_NOTIFICATION_ITEM_POSITION);
                }
                SetStatusBarBackground(Resource.Drawable.dashboard_fluid_background);
                //SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                TextViewUtils.SetMuseoSans500Typeface(notificationDetailTitle);
                TextViewUtils.SetMuseoSans300Typeface(notificationDetailMessage);
                mPresenter.EvaluateDetail(notificationDetails);
                RenderUI();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void RenderUI()
        {
            try
            {
                NotificationDetailModel detailModel = mPresenter.GetNotificationDetailModel();
                notificationDetailBannerImg.SetImageResource(detailModel.imageResourceBanner);
                notificationDetailTitle.Text = detailModel.title;
                notificationDetailMessage.TextFormatted = GetFormattedText(detailModel.message);
                NotificationDetailCTAComponent ctaComponent = FindViewById<NotificationDetailCTAComponent>(Resource.Id.notificationCTAComponent);
                if (detailModel.ctaList.Count > 0)
                {
                    ctaComponent.Visibility = ViewStates.Visible;
                }
                else
                {
                    ctaComponent.Visibility = ViewStates.Gone;
                }
                ctaComponent.SetCTAButton(detailModel.ctaList);
            }
            catch(Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_detail_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_detail_cancelled_exception_btn_close), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();

            }
            );
            mCancelledExceptionSnackBar.Show();

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_detail_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_detail_api_exception_btn_close), delegate
            {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            mApiExcecptionSnackBar.Show();

        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_detail_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_detail_unknown_exception_btn_close), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            mUknownExceptionSnackBar.Show();

        }

        public void ShowLoadingScreen()
        {
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideLoadingScreen()
        {
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ViewBill()
        {
        }

        public void PayNow(AccountData mSelectedAccountData)
        {
            Intent payment_activity = new Intent(this, typeof(SelectAccountsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
            StartActivity(payment_activity);
        }

        public void ContactUs(WeblinkEntity entity)
        {
            if (entity.OpenWith.Equals("PHONE"))
            {
                var uri = Android.Net.Uri.Parse("tel:" + entity.Url);
                var intent = new Intent(Intent.ActionDial, uri);
                StartActivity(intent);
            }
        }

        public void ViewUsage()
        {
            
        }

        public void ViewDetails(AccountData mSelectedAccountData, AccountChargeModel accountChargeModel)
        {
            Intent intent = new Intent(this, typeof(BillingDetailsActivity));
            intent.PutExtra("SELECTED_ACCOUNT", JsonConvert.SerializeObject(mSelectedAccountData));
            intent.PutExtra("SELECTED_BILL_DETAILS", JsonConvert.SerializeObject(accountChargeModel));
            StartActivity(intent);
        }
    }
}
