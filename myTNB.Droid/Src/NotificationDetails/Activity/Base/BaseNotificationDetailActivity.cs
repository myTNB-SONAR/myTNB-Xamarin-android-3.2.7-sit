using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.NotificationDetails.MVP;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Refit;
using System;
using System.Runtime;

namespace myTNB_Android.Src.NotificationDetails.Activity.Base
{
    [Activity(Label = "BaseNotificationDetailActivity")]
    public abstract class BaseNotificationDetailActivity : BaseToolbarAppCompatActivity, NotificationDetailContract.IView
    {



        protected NotificationDetails.Models.NotificationDetails notificationDetails;
        protected UserNotificationData userNotificationData;
        protected int position;

        protected NotificationDetailContract.IUserActionsListener userActionsListener;
        protected NotificationDetailPresenter mPresenter;

        MaterialDialog mProgressDialog;

        private LoadingOverlay loadingOverlay;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    //notificationDetails = JsonConvert.DeserializeObject<NotificationDetails.Models.NotificationDetails>(extras.GetString(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM));
                    //userNotificationData = JsonConvert.DeserializeObject<UserNotificationData>(extras.GetString(Constants.SELECTED_NOTIFICATION_LIST_ITEM));

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


                base.OnCreate(savedInstanceState);

                mProgressDialog = new MaterialDialog.Builder(this)
                .Title(GetString(Resource.String.notification_detail_remove_progress_title))
                .Content(GetString(Resource.String.notification_detail_remove_progress_content))
                .Cancelable(false)
                .Progress(true, 0)
                .Build();



                this.mPresenter = new NotificationDetailPresenter(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            this.userActionsListener.Start();
        }

        public abstract View GetRootView();

        public void ShowToolbarTitle(int resourceString)
        {
            this.SetToolBarTitle(GetString(resourceString));
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.NotificationDetailMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override void OnBackPressed()
        {
            Intent result = new Intent();
            result.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
            result.PutExtra(Constants.ACTION_IS_READ, true);
            SetResult(Result.Ok, result);
            base.OnBackPressed();
        }

        AlertDialog removeDialog;
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
                            this.userActionsListener.OnRemoveNotification(notificationDetails);
                        })
                        .Show()
                        ;

                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void ShowRemovingProgress()
        {
            //if (mProgressDialog != null && !mProgressDialog.IsShowing)
            //{
            //    mProgressDialog.Show();
            //}
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

        public void HideRemovingProgress()
        {
            //if (mProgressDialog != null && mProgressDialog.IsShowing)
            //{
            //    mProgressDialog.Dismiss();
            //}
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

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void SetPresenter(NotificationDetailContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }


        public void ShowNotificationListAsDeleted()
        {
            Intent result = new Intent();
            result.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
            result.PutExtra(Constants.ACTION_IS_DELETE, true);
            SetResult(Result.Ok, result);
            Finish();
        }



        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(GetRootView(), Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
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

            mApiExcecptionSnackBar = Snackbar.Make(GetRootView(), Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
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

            mUknownExceptionSnackBar = Snackbar.Make(GetRootView(), Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.notification_detail_unknown_exception_btn_close), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            mUknownExceptionSnackBar.Show();

        }

        public virtual void ShowAccountNumber()
        {
            // TODO : TO BE OVERIDDED BY SUBCLASS
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}