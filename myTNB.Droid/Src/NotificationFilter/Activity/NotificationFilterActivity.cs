using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.NotificationFilter.Adapter;
using myTNB.AndroidApp.Src.NotificationFilter.Models;
using myTNB.AndroidApp.Src.NotificationFilter.MVP;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB.AndroidApp.Src.NotificationFilter.Activity
{
    [Activity(Label = "@string/notification_filter_activity_title"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Notification")]
    public class NotificationFilterActivity : BaseActivityCustom, NotificationFilterContract.IView
    {

        [BindView(Resource.Id.notification_listview)]
        ListView notificationListView;

        NotificationFilterAdapter adapter;

        NotificationFilterContract.IUserActionsListener userActionsListener;
        NotificationFilterPresenter mPresenter;
        const string PAGE_ID = "";

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.NotificationFilterView;
        }

        public void SetPresenter(NotificationFilterContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowNotificationList(List<NotificationFilterData> notificationFilters)
        {
            adapter.AddAll(notificationFilters);
        }

        [OnItemClick(Resource.Id.notification_listview)]
        void OnSelectItem(object sender, AbsListView.ItemClickEventArgs e)
        {
            try
            {
                NotificationFilterData data = adapter.GetItemObject(e.Position);
                this.userActionsListener.OnSelectFilterItem(data, e.Position);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void ShowSelectedFilterItem(NotificationFilterData notificationFilterData, int position)
        {
            try
            {
                adapter.DisableAll();
                notificationFilterData.IsSelected = true;
                adapter.Update(position, notificationFilterData);
                SetResult(Result.Ok);
                Finish();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetToolBarTitle(Utility.GetLocalizedLabel("PushNotificationList", "selectNotification"));
                adapter = new NotificationFilterAdapter(this, true);
                notificationListView.Adapter = adapter;
                // Create your application here
                this.mPresenter = new NotificationFilterPresenter(this);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Notification Filter");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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