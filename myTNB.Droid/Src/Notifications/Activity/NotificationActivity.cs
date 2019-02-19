using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.NotificationFilter.Activity;
using myTNB_Android.Src.Notifications.Adapter;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Notifications.MVP;
using Refit;
using System;
using System.Collections.Generic;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.NotificationDetails.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Android.Runtime;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;

namespace myTNB_Android.Src.Notifications.Activity
{
    [Activity(Label = "@string/notification_activity_title"
      //, MainLauncher = true
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Notification")]
    public class NotificationActivity : BaseToolbarAppCompatActivity  , NotificationContract.IView
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.notification_listview)]
        ListView notificationListView;

        [BindView(Resource.Id.txt_notification_name)]
        TextView txtNotificationName;

        [BindView(Resource.Id.txtNotificationsContent)]
        TextView txtNotificationsContent;

        [BindView(Resource.Id.emptyLayout)]
        LinearLayout emptyLayout;

        NotificationAdapter notificationAdapter;

        NotificationContract.IUserActionsListener userActionsListener;
        NotificationPresenter mPresenter;

        MaterialDialog mProgressDialog , mQueryProgressDialog;
        private LoadingOverlay loadingOverlay;

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.NotificationView;
        }

        public void SetPresenter(NotificationContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }
        [OnClick(Resource.Id.txt_notification_name)]
        void OnNotificationFilter(object sender , EventArgs eventArgs)
        {
            this.userActionsListener.OnShowNotificationFilter();
        }

        public void ShowNotificationFilter()
        {
            Intent notificationFilter = new Intent(this, typeof(NotificationFilterActivity));
            StartActivityForResult(notificationFilter , Constants.NOTIFICATION_FILTER_REQUEST_CODE);
        }

        public void ShowNotificationsList(List<UserNotificationData> userNotificationList)
        {
            notificationAdapter.AddAll(userNotificationList);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);



            mProgressDialog = new MaterialDialog.Builder(this)
                .Title(GetString(Resource.String.notification_activity_progress_title))
                .Content(GetString(Resource.String.notification_activity_progress_content))
                .Cancelable(false)
                .Progress(true, 0)
                .Build();

            mQueryProgressDialog = new MaterialDialog.Builder(this)
                .Title(GetString(Resource.String.notification_activity_query_progress_title))
                .Content(GetString(Resource.String.notification_activity_query_progress_content))
                .Cancelable(false)
                .Progress(true, 0)
                .Build();

            TextViewUtils.SetMuseoSans500Typeface(txtNotificationName);
            TextViewUtils.SetMuseoSans300Typeface(txtNotificationsContent);

            this.mPresenter = new NotificationPresenter(this);
            notificationAdapter = new NotificationAdapter(this, true);
            notificationListView.Adapter = notificationAdapter;
            notificationListView.EmptyView = emptyLayout;

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

            Bundle extras = Intent.Extras;
            if (extras != null && extras.ContainsKey(Constants.HAS_NOTIFICATION) && extras.GetBoolean(Constants.HAS_NOTIFICATION))
            {
                this.userActionsListener.QueryOnLoad(this.DeviceId());
            }
   

        }
        [OnItemClick(Resource.Id.notification_listview)]
        void OnItemClick(object sender , AbsListView.ItemClickEventArgs args)
        {
            UserNotificationData data = notificationAdapter.GetItemObject(args.Position);
            this.userActionsListener.OnSelectedNotificationItem(data , args.Position);
        }

        public void ShowProgress()
        {
            //if (mProgressDialog != null && !mProgressDialog.IsShowing)
            //{
            //    mProgressDialog.Show();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
        }

        public void HideProgress()
        {
            //if (mProgressDialog != null && mProgressDialog.IsShowing)
            //{
            //    mProgressDialog.Dismiss();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }
        }


        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_activity_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_activity_cancelled_exception_btn_close), delegate {

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

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_activity_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_activity_api_exception_btn_close), delegate {

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

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_activity_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_activity_unknown_exception_btn_close), delegate {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            mUknownExceptionSnackBar.Show();

        }



        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.userActionsListener.OnActivityResult(requestCode , resultCode , data);
        }

        public void UpdateIsReadNotificationItem(int position , bool isRead )
        {
            UserNotificationData userNotificationData = notificationAdapter.GetItemObject(position);
            userNotificationData.IsRead = isRead;
            notificationAdapter.Update(position, userNotificationData);
        }

        public void UpdateIsDeleteNotificationItem(int position, bool isDelete)
        {
            UserNotificationData userNotificationData = notificationAdapter.GetItemObject(position);
            notificationAdapter.Remove(position);


        }
        private Snackbar mNotificationRemoved;
        public void ShowNotificationRemoved()
        {
            if (mNotificationRemoved != null && mNotificationRemoved.IsShown)
            {
                mNotificationRemoved.Dismiss();
            }

            mNotificationRemoved = Snackbar.Make(rootView, GetString(Resource.String.notification_activity_notification_removed), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_activity_notification_removed_btn_close), delegate {

                mNotificationRemoved.Dismiss();

            }
            );
            mNotificationRemoved.Show();
        }

        public void ClearAdapter()
        {
            notificationAdapter.Clear();
        }

        public void ShowNotificationFilterName(string filterName)
        {
            txtNotificationName.Text = filterName;
        }

        public void ShowDetails(NotificationDetails.Models.NotificationDetails details, UserNotificationData notificationData, int position)
        {
            Intent notificationDetails = new Intent(this, typeof(NotificationDetailActivity));
            notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_LIST_ITEM, JsonConvert.SerializeObject(notificationData));
            notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM, JsonConvert.SerializeObject(details));
            notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
            StartActivityForResult(notificationDetails, Constants.NOTIFICATION_DETAILS_REQUEST_CODE);
        }

        //public void ShowSelectedNotificationNewBillItem(NotificationDetails.Models.NotificationDetails details, UserNotificationData notificationData, int position)
        //{
        //    Intent notificationDetails = new Intent(this, typeof(NotificationDetailsNewBillActivity));
        //    notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM, JsonConvert.SerializeObject(details));
        //    notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
        //    StartActivityForResult(notificationDetails, Constants.NOTIFICATION_DETAILS_REQUEST_CODE);
        //}

        //public void ShowSelectedNotificationDunningDisconnection(NotificationDetails.Models.NotificationDetails details, UserNotificationData notificationData, int position)
        //{
        //    Intent notificationDetails = new Intent(this, typeof(NotificationDetailAccountRelatedMattersActivity));
        //    notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM, JsonConvert.SerializeObject(details));
        //    notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
        //    StartActivityForResult(notificationDetails, Constants.NOTIFICATION_DETAILS_REQUEST_CODE);
        //}

        public void ShowQueryProgress()
        {
            //if (mQueryProgressDialog != null && !mQueryProgressDialog.IsShowing)
            //{
            //    mQueryProgressDialog.Show();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
        }

        public void HideQueryProgress()
        {
            //if (mQueryProgressDialog != null && mQueryProgressDialog.IsShowing)
            //{
            //    mQueryProgressDialog.Dismiss();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }
    }
}