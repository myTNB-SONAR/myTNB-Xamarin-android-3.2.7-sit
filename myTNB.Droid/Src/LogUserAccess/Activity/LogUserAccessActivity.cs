using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.RecyclerView.Widget;
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
        RecyclerView ThisWeeklistview;

        [BindView(Resource.Id.last_week_list_recycler_view)]
        RecyclerView LastWeeklistview;

        [BindView(Resource.Id.text_title_last_week)]
        TextView texttitleLastWeek;

        [BindView(Resource.Id.text_title_last_month)]
        TextView texttitleLastMonth;

        [BindView(Resource.Id.txtEmptyActivityLog)]
        TextView txtEmptyActivityLog;

        [BindView(Resource.Id.month_list_recycler_view)]
        RecyclerView LastMonthlistview;

        [BindView(Resource.Id.log_activity_layout)]
        LinearLayout log_activity_layout;

        [BindView(Resource.Id.ActivityLog_layout_empty)]
        FrameLayout empty_layout;

        [BindView(Resource.Id.scrollviewLog)]
        ScrollView scrollviewLog;

        [BindView(Resource.Id.progressbarlayout)]
        LinearLayout progressbarlayout;

        LogUserAccessAdapter adapter;

        LogUserAccessRecyclerviewAdapter logUserAccessRecyclerviewAdapter;
        LogUserAccessContract.IUserActionsListener userActionsListener;
        LogUserAccessPresenter mPresenter;
        List<LogUserAccessNewData> LogListData;

        private int firstdata;
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

        public void ShowLogListThisWeek(List<LogUserAccessNewData> logUserDataThisWeek)
        {
            DividerItemDecoration mDividerItemDecoration = new DividerItemDecoration(ThisWeeklistview.Context,
            DividerItemDecoration.Vertical);
            ThisWeeklistview.AddItemDecoration(mDividerItemDecoration);

            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            ThisWeeklistview.SetLayoutManager(linearLayoutManager);
            logUserAccessRecyclerviewAdapter = new LogUserAccessRecyclerviewAdapter(this, logUserDataThisWeek);

            ThisWeeklistview.SetAdapter(logUserAccessRecyclerviewAdapter);
            logUserAccessRecyclerviewAdapter.NotifyDataSetChanged();
            logUserAccessRecyclerviewAdapter.OnAttachedToRecyclerView(ThisWeeklistview);

            //LogUserAccessRecyclerviewAdapter.NotifyDataSetChanged();
            //logUserAccessRecyclerviewAdapter.(logUserDataThisWeek);
            //ThisWeeklistview.Adapter = adapter;
            //ThisWeeklistview.SetNoScroll();
            //ThisWeeklistview.SetScrollContainer(false);
        }

        public void ShowLogListLastWeek(List<LogUserAccessNewData> logUserDataLastWeek)
        {
            DividerItemDecoration mDividerItemDecoration = new DividerItemDecoration(LastWeeklistview.Context,
            DividerItemDecoration.Vertical);
            LastWeeklistview.AddItemDecoration(mDividerItemDecoration);

            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            LastWeeklistview.SetLayoutManager(linearLayoutManager);
            logUserAccessRecyclerviewAdapter = new LogUserAccessRecyclerviewAdapter(this, logUserDataLastWeek);

            LastWeeklistview.SetAdapter(logUserAccessRecyclerviewAdapter);
            logUserAccessRecyclerviewAdapter.NotifyDataSetChanged();
            logUserAccessRecyclerviewAdapter.OnAttachedToRecyclerView(LastWeeklistview);

            //logUserAccessRecyclerviewAdapter = new LogUserAccessRecyclerviewAdapter(this, this, true);
            //logUserAccessRecyclerviewAdapter.NotifyDataSetChanged();
            //logUserAccessRecyclerviewAdapter.AddAll(logUserDataLastWeek);

            //LastWeeklistview.Adapter = adapter;
            //LastWeeklistview.SetNoScroll();
            //LastWeeklistview.SetScrollContainer(false);
        }

        public void ShowLogList(List<LogUserAccessNewData> logUserData)
        {
            DividerItemDecoration mDividerItemDecoration = new DividerItemDecoration(LastMonthlistview.Context,
            DividerItemDecoration.Vertical);
            LastMonthlistview.AddItemDecoration(mDividerItemDecoration);

            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            LastMonthlistview.SetLayoutManager(linearLayoutManager);
            logUserAccessRecyclerviewAdapter = new LogUserAccessRecyclerviewAdapter(this, logUserData);

            LastMonthlistview.SetAdapter(logUserAccessRecyclerviewAdapter);
            logUserAccessRecyclerviewAdapter.NotifyDataSetChanged();
            logUserAccessRecyclerviewAdapter.OnAttachedToRecyclerView(LastMonthlistview);

            //logUserAccessRecyclerviewAdapter = new LogUserAccessRecyclerviewAdapter(this, this, true);
            //logUserAccessRecyclerviewAdapter.NotifyDataSetChanged();
            //logUserAccessRecyclerviewAdapter.AddAll(logUserData);

            //LastMonthlistview.Adapter = adapter;
            //LastMonthlistview.SetNoScroll();
            //LastMonthlistview.SetScrollContainer(false);
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

                TextViewUtils.SetMuseoSans300Typeface(txtEmptyActivityLog);
                TextViewUtils.SetMuseoSans500Typeface(texttitleThisWeek, texttitleLastWeek, texttitleLastMonth);
                TextViewUtils.SetTextSize14(txtEmptyActivityLog);
                TextViewUtils.SetTextSize16(texttitleThisWeek, texttitleLastWeek, texttitleLastMonth);

                texttitleThisWeek.Text = Utility.GetLocalizedLabel("UserAccess", "thisWeekTitle");
                texttitleLastWeek.Text = Utility.GetLocalizedLabel("UserAccess", "lastWeekTitle");
                texttitleLastMonth.Text = Utility.GetLocalizedLabel("UserAccess", "lastMonthTitle");
                txtEmptyActivityLog.Text = Utility.GetLocalizedLabel("UserAccess", "emptyLogText");

                SetToolBarTitle(Utility.GetLocalizedLabel("UserAccess", "title_activityLog"));

                this.mPresenter = new LogUserAccessPresenter(this);

                if (LogListData == null || LogListData.Count == 0)
                {
                    log_activity_layout.Visibility = ViewStates.Gone;
                    empty_layout.Visibility = ViewStates.Visible;
                }
                else
                {
                    this.userActionsListener.SortLogListDataByDate(LogListData);
                    //progressbarlayout.Visibility = ViewStates.Visible;
                }

                //scrollviewLog.ScrollChange += ScrollviewLog_ScrollChange;
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void FirstLoadData(int datatotal)
        {
            firstdata = datatotal;
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
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