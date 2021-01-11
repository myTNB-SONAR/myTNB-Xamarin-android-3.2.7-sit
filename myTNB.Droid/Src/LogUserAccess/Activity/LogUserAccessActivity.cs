using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.LogUserAccess.Adapter;
using myTNB_Android.Src.LogUserAccess.Models;
using myTNB_Android.Src.LogUserAccess.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.LogUserAccess.Activity
{
    [Activity(Label = "@string/notification_filter_activity_title"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class LogUserAccessActivity : BaseActivityCustom, LogUserAccessContract.IView
    {

        [BindView(Resource.Id.text_title_this_week)]
        TextView texttitleThisWeek;

        [BindView(Resource.Id.this_week_list_recycler_view)]
        ListView ThisWeeklistview;

        [BindView(Resource.Id.last_week_list_recycler_view)]
        ListView LastWeeklistview;

        [BindView(Resource.Id.text_title_last_week)]
        TextView texttitleLastWeek;

        [BindView(Resource.Id.text_title_last_month)]
        TextView texttitleLastMonth;

        [BindView(Resource.Id.month_list_recycler_view)]
        ListView LastMonthlistview;

        [BindView(Resource.Id.log_activity_layout)]
        LinearLayout log_activity_layout;

        [BindView(Resource.Id.ActivityLog_layout_empty)]
        FrameLayout empty_layout;

        LogUserAccessAdapter adapter;

        LogUserAccessContract.IUserActionsListener userActionsListener;
        LogUserAccessPresenter mPresenter;
        List<LogUserAccessNewData> LogListData;
        const string PAGE_ID = "";

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.ActivityLogUserAccess;
        }

        public void SetPresenter(LogUserAccessContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowLogList(List<LogUserAccessNewData> logUserData)
        {
            //adapter.AddAll(logUserData);
            adapter = new LogUserAccessAdapter(this, logUserData);
            LastMonthlistview.Adapter = adapter;
            LastMonthlistview.SetNoScroll();
        }

        public void ShowLogListThisWeek(List<LogUserAccessNewData> logUserDataThisWeek)
        {
            //adapter.AddAll(logUserData);
            adapter = new LogUserAccessAdapter(this, logUserDataThisWeek);
            ThisWeeklistview.Adapter = adapter;
            ThisWeeklistview.SetNoScroll();
        }

        public void ShowLogListLastWeek(List<LogUserAccessNewData> logUserDataLastWeek)
        {
            //adapter.AddAll(logUserData);
            adapter = new LogUserAccessAdapter(this, logUserDataLastWeek);
            LastWeeklistview.Adapter = adapter;
            LastWeeklistview.SetNoScroll();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                    {
                        LogListData = DeSerialze<List<LogUserAccessNewData>>(extras.GetString(Constants.SELECTED_ACCOUNT));
                    }
                }

                SetToolBarTitle(Utility.GetLocalizedLabel("PushNotificationList", "selectNotification"));

                this.mPresenter = new LogUserAccessPresenter(this);

                if (LogListData.Count == 0)
                {
                    log_activity_layout.Visibility = ViewStates.Gone;
                    empty_layout.Visibility = ViewStates.Visible;
                }
                else
                {
                    this.userActionsListener.SortLogListDataByDate(LogListData);
                }

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
                FirebaseAnalyticsUtils.SetScreenName(this, "Log User Access");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void emptyThisWeekList()
        {
            texttitleThisWeek.Visibility = ViewStates.Gone;
            ThisWeeklistview.Visibility = ViewStates.Gone;
        }

        public void emptyLastWeekList()
        {
            texttitleLastWeek.Visibility = ViewStates.Gone;
            LastWeeklistview.Visibility = ViewStates.Gone;
        }

        public void emptyLastMonthList()
        {
            texttitleLastMonth.Visibility = ViewStates.Gone;
            LastMonthlistview.Visibility = ViewStates.Gone;
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