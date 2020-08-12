using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;


using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.NotificationSettings.Adapter;
using myTNB_Android.Src.NotificationSettings.MVP;
using myTNB_Android.Src.SelectNotification.Models;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.NotificationSettings.Activity
{
    [Activity(Label = "@string/notification_settings_activity_title"
      //, MainLauncher = true
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Notification")]
    public class NotificationSettingsActivity : BaseActivityCustom, NotificationSettingsContract.IView
    {

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtNotificationTypeTitle)]
        TextView txtNotificationTypeTitle;

        [BindView(Resource.Id.txtNotificationChannelTitle)]
        TextView txtNotificationChannelTitle;

        [BindView(Resource.Id.notificationTypeRecyclerView)]
        RecyclerView notificationTypeRecyclerView;

        NotificationTypeAdapter typeAdapter;

        [BindView(Resource.Id.notificationChannelRecyclerView)]
        RecyclerView notificationChannelRecyclerView;

        NotificationChannelAdapter channelAdapter;

        NotificationSettingsContract.IUserActionsListener userActionsListener;
        NotificationSettingsPresenter mPresenter;

        LinearLayoutManager notificationChannelLayoutManager, notificationTypeLayoutManager;


        MaterialDialog progressUpdateType, progressUpdateChannel;

        const string PAGE_ID = "NotificationSettings";

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.NotificationSettingsView;
        }

        public void SetPresenter(NotificationSettingsContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowNotificationChannelList(List<NotificationChannelUserPreference> channelPreferenceList)
        {
            channelAdapter.AddAll(channelPreferenceList);
            //notificationChannelListView.SetNoScroll();
        }

        public void ShowNotificationTypesList(List<NotificationTypeUserPreference> typePreferenceList)
        {
            typeAdapter.AddAll(typePreferenceList);
            //notificationTypeListView.SetNoScroll();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                // Create your application here
                Console.WriteLine("NotificationSettingsActivity OnCreate");

                TextViewUtils.SetMuseoSans500Typeface(txtNotificationTypeTitle, txtNotificationChannelTitle);

                txtNotificationTypeTitle.Text = GetLabelByLanguage("typeDescription");
                txtNotificationChannelTitle.Text = GetLabelByLanguage("modeDescription");

                notificationChannelLayoutManager = new LinearLayoutManager(this);
                notificationTypeLayoutManager = new LinearLayoutManager(this);
                notificationTypeRecyclerView.SetLayoutManager(notificationTypeLayoutManager);
                notificationChannelRecyclerView.SetLayoutManager(notificationChannelLayoutManager);

                typeAdapter = new NotificationTypeAdapter(true);
                typeAdapter.ClickEvent += TypeAdapter_ClickEvent;
                notificationTypeRecyclerView.SetAdapter(typeAdapter);
                notificationTypeRecyclerView.NestedScrollingEnabled = (false);

                //notificationTypeListView.SetNoScroll();

                channelAdapter = new NotificationChannelAdapter(true);
                channelAdapter.ClickEvent += ChannelAdapter_ClickEvent;
                notificationChannelRecyclerView.SetAdapter(channelAdapter);
                notificationTypeRecyclerView.NestedScrollingEnabled = (false);
                //notificationChannelListView.SetNoScroll();

                mPresenter = new NotificationSettingsPresenter(this);
                this.userActionsListener.Start();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }

        }

        private void ChannelAdapter_ClickEvent(object sender, int e)
        {
            try
            {
                NotificationChannelUserPreference userPreference = channelAdapter.GetItemObject(e);
                this.userActionsListener.OnChannelItemClick(userPreference, e);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void TypeAdapter_ClickEvent(object sender, int e)
        {
            try
            {
                NotificationTypeUserPreference userPreference = typeAdapter.GetItemObject(e);
                this.userActionsListener.OnTypeItemClick(userPreference, e, this.DeviceId());
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        //[OnItemClick(Resource.Id.notificationTypeListView)]
        //void OnItemClickTypeListView(object sender, AdapterView.ItemClickEventArgs e)
        //{
        //    NotificationTypeUserPreference userPreference = typeAdapter.GetItemObject(e.Position);
        //    this.userActionsListener.OnTypeItemClick(userPreference , e.Position , this.DeviceId());
        //}

        //[OnItemClick(Resource.Id.notificationChannelListView)]
        //void OnItemClickChannelListView(object sender, AdapterView.ItemClickEventArgs e)
        //{
        //    NotificationChannelUserPreference userPreference = channelAdapter.GetItemObject(e.Position);
        //    this.userActionsListener.OnChannelItemClick(userPreference , e.Position);
        //}

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Notification Prefences");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private Snackbar mCancelledExceptionSnackBar;
        //public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        //{
        //    if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
        //    {
        //        mCancelledExceptionSnackBar.Dismiss();
        //    }

        //    mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_cancelled_exception_error), Snackbar.LengthIndefinite)
        //    .SetAction(GetString(Resource.String.notification_settings_cancelled_exception_btn_close), delegate {

        //        mCancelledExceptionSnackBar.Dismiss();
        //    }
        //    );
        //    mCancelledExceptionSnackBar.Show();

        //}

        private Snackbar mApiExcecptionSnackBar;
        //public void ShowRetryOptionsApiException(ApiException apiException)
        //{
        //    if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
        //    {
        //        mApiExcecptionSnackBar.Dismiss();
        //    }

        //    mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_api_exception_error), Snackbar.LengthIndefinite)
        //    .SetAction(GetString(Resource.String.notification_settings_api_exception_btn_close), delegate {

        //        mApiExcecptionSnackBar.Dismiss();

        //    }
        //    );
        //    mApiExcecptionSnackBar.Show();

        //}
        private Snackbar mUknownExceptionSnackBar;
        //public void ShowRetryOptionsUnknownException(Exception exception)
        //{
        //    if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
        //    {
        //        mUknownExceptionSnackBar.Dismiss();

        //    }

        //    mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_unknown_exception_error), Snackbar.LengthIndefinite)
        //    .SetAction(GetString(Resource.String.notification_settings_unknown_exception_btn_close), delegate {

        //        mUknownExceptionSnackBar.Dismiss();

        //    }
        //    );
        //    mUknownExceptionSnackBar.Show();

        //}

        public void ShowSuccessUpdatedNotificationType(NotificationTypeUserPreference typePreference, int position)
        {
            this.typeAdapter.Update(position, typePreference);
        }

        public void ShowSuccessUpdatedNotificationChannel(NotificationChannelUserPreference channelPreference, int position)
        {
            this.channelAdapter.Update(position, channelPreference);
        }

        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException, NotificationTypeUserPreference typePreference, int position)
        {
            this.typeAdapter.Update(position, typePreference);
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
            }
            );
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCancelledExceptionSnackBar.Show();
        }

        public void ShowRetryOptionsApiException(ApiException apiException, NotificationTypeUserPreference typePreference, int position)
        {
            this.typeAdapter.Update(position, typePreference);
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();
        }

        public void ShowRetryOptionsUnknownException(Exception exception, NotificationTypeUserPreference typePreference, int position)
        {
            this.typeAdapter.Update(position, typePreference);
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();
        }

        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException, NotificationChannelUserPreference channelPreference, int position)
        {
            this.channelAdapter.Update(position, channelPreference);
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
            }
            );
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCancelledExceptionSnackBar.Show();
        }

        public void ShowRetryOptionsApiException(ApiException apiException, NotificationChannelUserPreference channelPreference, int position)
        {
            this.channelAdapter.Update(position, channelPreference);
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();
        }

        public void ShowRetryOptionsUnknownException(Exception exception, NotificationChannelUserPreference channelPreference, int position)
        {
            this.channelAdapter.Update(position, channelPreference);
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.notification_settings_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
