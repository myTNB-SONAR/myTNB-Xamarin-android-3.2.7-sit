using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;



using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager.Widget;
using CheeseBind;
using Google.Android.Material.Tabs;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.WhatsNewMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.Fragments.WhatsNewMenu.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP
{
    public class WhatsNewMenuFragment : BaseFragment, WhatsNewMenuContract.IWhatsNewMenuView, ViewPager.IOnPageChangeListener
    {

        private bool isSiteCoreComplete = false;

        [BindView(Resource.Id.whatsNewMainLayout)]
        LinearLayout whatsNewMainLayout;

        [BindView(Resource.Id.whatsNewsSlidingTabs)]
        TabLayout whatsNewsSlidingTabs;

        [BindView(Resource.Id.whatsNewViewPager)]
        ViewPager whatsNewViewPager;

        [BindView(Resource.Id.whatsNewEmptyLayout)]
        LinearLayout whatsNewEmptyLayout;

        [BindView(Resource.Id.whatsNewEmptyImg)]
        ImageView whatsNewEmptyImg;

        [BindView(Resource.Id.txtEmptyWhatsNew)]
        TextView txtEmptyWhatsNew;

        [BindView(Resource.Id.whatsNewRefreshLayout)]
        LinearLayout whatsNewRefreshLayout;

        [BindView(Resource.Id.txtRefresh)]
        TextView txtRefresh;

        [BindView(Resource.Id.btnRefresh)]
        Button btnRefresh;

        private WhatsNewTabAdapter mAdapter;

        WhatsNewMenuContract.IWhatsNewMenuPresenter presenter;

        private List<WhatsNewMenuModel> mTabList = new List<WhatsNewMenuModel>();

        private string savedTimeStamp = "0000000";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.HasOptionsMenu = true;
            presenter = new WhatsNewMenuPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this.Activity));

            WhatsNewMenuUtils.OnSetTouchDisable(false);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            try
            {
                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "WhatsNews");
            }
            catch (Java.Lang.ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                WhatsNewMenuUtils.OnSetTouchDisable(true);

                InitializeView();

                if (mTabList != null && mTabList.Count > 0)
                {
                    SetupTabIndicator((int)DPUtils.ConvertDPToPx(16f), (int)DPUtils.ConvertDPToPx(10f));
                    MeasureTabScroll();
                    HighLightCurrentTab(0);
                    whatsNewViewPager.AddOnPageChangeListener(this);
                }
                whatsNewViewPager.OverScrollMode = OverScrollMode.Never;

                TextViewUtils.SetMuseoSans300Typeface(txtEmptyWhatsNew, txtRefresh);
                TextViewUtils.SetMuseoSans500Typeface(btnRefresh);
                TextViewUtils.SetTextSize14(txtEmptyWhatsNew);
                TextViewUtils.SetTextSize16(txtRefresh, btnRefresh);
                whatsNewMainLayout.Visibility = ViewStates.Visible;

                whatsNewEmptyLayout.Visibility = ViewStates.Gone;

                whatsNewRefreshLayout.Visibility = ViewStates.Gone;

                OnGetWhatsNewTimestamp();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                WhatsNewMenuUtils.OnResetUpdateList();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnGetWhatsNewTimestamp()
        {
            try
            {
                if (WhatsNewMenuUtils.GetWhatsNewLoading())
                {
                    _ = this.presenter.OnRecheckWhatsNewsStatus();
                }
                else
                {
                    this.presenter.GetWhatsNewsTimeStamp();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return IsAdded && IsVisible && !IsDetached && !IsRemoving;
        }

        public override void OnPause()
        {
            base.OnPause();
            NewAppTutorialUtils.ForceCloseNewAppTutorial();
        }

        public override void OnResume()
        {
            base.OnResume();

            try
            {
                var act = this.Activity as AppCompatActivity;

                var actionBar = act.SupportActionBar;
                actionBar.Show();
                ShowBackButton(false);

                ((DashboardHomeActivity)this.Activity).RemoveHeaderDropDown();
                ((DashboardHomeActivity)this.Activity).HideAccountName();
                ((DashboardHomeActivity)this.Activity).SetToolBarTitle(Utility.GetLocalizedLabel("Tabbar", "promotion"));
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                if (WhatsNewMenuUtils.GetRefreshAll())
                {
                    WhatsNewMenuUtils.OnResetUpdateList();
                    if (mTabList != null && mTabList.Count > 0)
                    {
                        for (int i = 0; i < mTabList.Count; i++)
                        {
                            if (mTabList[i].Fragment.IsActive())
                            {
                                mTabList[i].Fragment.Refresh();
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                if (mTabList != null && mTabList.Count > 0 && mTabList[0].FragmentListMode == Constants.WHATSNEWITEMLISTMODE.LOADED)
                {
                    if (!UserSessions.HasWhatsNewShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                    {
                        Handler h = new Handler();
                        Action myAction = () =>
                        {
                            NewAppTutorialUtils.ForceCloseNewAppTutorial();
                            OnShowWhatsNewMenuTutorial();
                        };
                        h.PostDelayed(myAction, 50);
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return base.OnCreateView(inflater, container, savedInstanceState);
        }


        public override int ResourceId()
        {
            return Resource.Layout.WhatsNewListView;
        }

        public void ShowBackButton(bool flag)
        {
            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(flag);
            actionBar.SetDisplayShowHomeEnabled(flag);
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        private void HighLightCurrentTab(int position)
        {
            try
            {
                for (int i = 0; i < whatsNewsSlidingTabs.TabCount; i++)
                {
                    if (i != position)
                    {
                        TabLayout.Tab currentTab = whatsNewsSlidingTabs.GetTabAt(i);
                        currentTab.SetCustomView(null);
                        currentTab.SetCustomView(mAdapter.GetTabView(i));
                    }
                }
                TabLayout.Tab currentHightlightTab = whatsNewsSlidingTabs.GetTabAt(position);
                currentHightlightTab.SetCustomView(null);
                currentHightlightTab.SetCustomView(mAdapter.GetSelectedTabView(position));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetupTabIndicator(int externalMargin, int internalMargin)
        {
            try
            {
                View TabStrip = whatsNewsSlidingTabs.GetChildAt(0);

                ViewGroup TabStripGroup = (ViewGroup)TabStrip;

                int childCount = ((ViewGroup)TabStrip).ChildCount;

                for (int i = 0; i < childCount; i++)
                {
                    View tabView = TabStripGroup.GetChildAt(i);
                    tabView.SetMinimumWidth(0);
                    tabView.SetPadding(0, tabView.PaddingTop, 0, tabView.PaddingBottom);
                    ViewGroup.MarginLayoutParams layoutParams = (ViewGroup.MarginLayoutParams)tabView.LayoutParameters;
                    if (i == 0)
                    {
                        // left
                        SettingMargin(layoutParams, externalMargin, internalMargin);
                    }
                    else if (i == childCount - 1)
                    {
                        // right
                        SettingMargin(layoutParams, internalMargin, externalMargin);
                    }
                    else
                    {
                        // internal
                        SettingMargin(layoutParams, internalMargin, internalMargin);
                    }
                }

                whatsNewsSlidingTabs.RequestLayout();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SettingMargin(ViewGroup.MarginLayoutParams layoutParams, int start, int end)
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBeanMr1)
            {
                layoutParams.MarginStart = start;
                layoutParams.MarginEnd = end;
                layoutParams.LeftMargin = start;
                layoutParams.RightMargin = end;
            }
            else
            {
                layoutParams.LeftMargin = start;
                layoutParams.RightMargin = end;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.presenter.OnCancelTask();
            mAdapter.ClearAll();
        }

        void ViewPager.IOnPageChangeListener.OnPageScrollStateChanged(int state)
        {

        }

        void ViewPager.IOnPageChangeListener.OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {

        }

        void ViewPager.IOnPageChangeListener.OnPageSelected(int position)
        {
            if (mTabList != null && mTabList.Count > 0)
            {
                HighLightCurrentTab(position);
                try
                {
                    WhatsNewMenuModel currentModel = mTabList[position];
                    if (currentModel.FragmentListMode == Constants.WHATSNEWITEMLISTMODE.LOADED
                        && WhatsNewMenuUtils.OnCheckIsUpdateNeed(currentModel.FragmentSearchString))
                    {
                        currentModel.Fragment.Refresh();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }

        private void InitializeView()
        {
            mAdapter = new WhatsNewTabAdapter(this.FragmentManager, this.Activity);

            mTabList = this.presenter.InitializeWhatsNewView();

            for (int i = 0; i < mTabList.Count; i++)
            {
                Bundle bundle = new Bundle();
                bundle.PutInt(Constants.WHATSNEW_ITEM_LIST_MODE, (int)mTabList[i].FragmentListMode);
                mTabList[i].Fragment.Arguments = bundle;
                mAdapter.AddFragment(mTabList[i].Fragment, mTabList[i].TabTitle);
            }

            whatsNewViewPager.Adapter = mAdapter;
            whatsNewsSlidingTabs.SetupWithViewPager(whatsNewViewPager);
        }

        public void OnSavedWhatsNewsTimeStamp(string mSavedTimeStamp)
        {
            if (mSavedTimeStamp != null)
            {
                this.savedTimeStamp = mSavedTimeStamp;
            }
            this.presenter.OnGetWhatsNewsTimeStamp();
        }

        public void OnSetResultTabView(List<WhatsNewMenuModel> list)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        mAdapter.ClearAll();

                        mTabList = list;

                        for (int i = 0; i < mTabList.Count; i++)
                        {
                            Bundle bundle = new Bundle();
                            bundle.PutInt(Constants.WHATSNEW_ITEM_LIST_MODE, (int)mTabList[i].FragmentListMode);
                            bundle.PutString(Constants.WHATSNEW_ITEM_LIST_SEARCH_STRING_KEY, mTabList[i].FragmentSearchString);

                            mTabList[i].Fragment.Arguments = bundle;
                            mAdapter.AddFragment(mTabList[i].Fragment, mTabList[i].TabTitle);
                        }

                        whatsNewViewPager.Adapter = mAdapter;
                        whatsNewsSlidingTabs.SetupWithViewPager(whatsNewViewPager);

                        if (mTabList != null && mTabList.Count > 1)
                        {
                            SetupTabIndicator((int)DPUtils.ConvertDPToPx(16f), (int)DPUtils.ConvertDPToPx(10f));
                            MeasureTabScroll();
                            HighLightCurrentTab(0);
                        }

                        if (mTabList == null || (mTabList != null && mTabList.Count <= 1))
                        {
                            whatsNewsSlidingTabs.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            whatsNewsSlidingTabs.Visibility = ViewStates.Visible;
                        }

                        try
                        {
                            if (WhatsNewEntity.HasUnread())
                            {
                                ((DashboardHomeActivity)this.Activity).ShowUnreadWhatsNew();
                            }
                            else
                            {
                                ((DashboardHomeActivity)this.Activity).HideUnreadWhatsNew();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }

                        WhatsNewMenuUtils.OnSetTouchDisable(false);

                        try
                        {
                            if (!UserSessions.HasWhatsNewShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                            {
                                Handler h = new Handler();
                                Action myAction = () =>
                                {
                                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                                    OnShowWhatsNewMenuTutorial();
                                };
                                h.PostDelayed(myAction, 50);
                            }
                        }
                        catch (System.Exception exp)
                        {
                            Utility.LoggingNonFatalError(exp);
                        }
                    }
                    catch (System.Exception err)
                    {
                        Utility.LoggingNonFatalError(err);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void MeasureTabScroll()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        Action myAction = () =>
                        {
                            try
                            {
                                Activity.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        int widthS = DPUtils.GetWidth();
                                        whatsNewsSlidingTabs.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);
                                        int widthT = whatsNewsSlidingTabs.MeasuredWidth;

                                        if (widthS > widthT)
                                        {
                                            int diff = widthS - widthT;
                                            int diffOnEachItem = diff / mTabList.Count;
                                            SetupTabIndicator((int)DPUtils.ConvertDPToPx(16f) + diffOnEachItem / 2, (int)DPUtils.ConvertDPToPx(10f) + diffOnEachItem / 2);
                                        }
                                    }
                                    catch (Exception er)
                                    {
                                        Utility.LoggingNonFatalError(er);
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }
                        };
                        whatsNewsSlidingTabs.Post(myAction);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void CheckWhatsNewsTimeStamp(string mTimeStamp)
        {
            try
            {
                if (mTimeStamp != null)
                {
                    if (!mTimeStamp.Equals(savedTimeStamp))
                    {
                        this.presenter.OnGetWhatsNews();
                    }
                    else
                    {
                        WhatsNewEntity wtItemManager = new WhatsNewEntity();
                        List<WhatsNewEntity> subItems = wtItemManager.GetAllItems();
                        if (subItems != null && subItems.Count > 0)
                        {
                            this.presenter.CheckWhatsNewsCache();
                        }
                        else
                        {
                            this.presenter.OnGetWhatsNews();
                        }
                    }
                }
                else
                {
                    this.presenter.OnGetWhatsNews();
                }
            }
            catch (System.Exception e)
            {
                this.presenter.OnGetWhatsNews();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetEmptyView()
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        whatsNewMainLayout.Visibility = ViewStates.Gone;

                        whatsNewEmptyLayout.Visibility = ViewStates.Visible;

                        whatsNewRefreshLayout.Visibility = ViewStates.Gone;

                        txtEmptyWhatsNew.Text = Utility.GetLocalizedLabel("WhatsNew", "noPromotions");

                        LinearLayout.LayoutParams whatsNewEmptyImgParams = whatsNewEmptyImg.LayoutParameters as LinearLayout.LayoutParams;
                        whatsNewEmptyImgParams.TopMargin = GetDeviceVerticalScaleInPixel(0.155f);
                        whatsNewEmptyImgParams.Width = GetDeviceHorizontalScaleInPixel(0.319f);
                        whatsNewEmptyImgParams.Height = GetDeviceVerticalScaleInPixel(0.165f);
                        whatsNewEmptyImg.RequestLayout();

                        WhatsNewMenuUtils.OnSetTouchDisable(false);

                        try
                        {
                            if (WhatsNewEntity.HasUnread())
                            {
                                ((DashboardHomeActivity)this.Activity).ShowUnreadWhatsNew();
                            }
                            else
                            {
                                ((DashboardHomeActivity)this.Activity).HideUnreadWhatsNew();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (Exception er)
            {
                Utility.LoggingNonFatalError(er);
            }
        }

        public void SetRefreshView(string buttonText, string messageText)
        {
            try
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        whatsNewMainLayout.Visibility = ViewStates.Gone;

                        whatsNewEmptyLayout.Visibility = ViewStates.Gone;

                        whatsNewRefreshLayout.Visibility = ViewStates.Visible;

                        if (!string.IsNullOrEmpty(buttonText))
                        {
                            btnRefresh.Text = buttonText;
                        }
                        else
                        {
                            btnRefresh.Text = Utility.GetLocalizedCommonLabel("refreshNow");
                        }

                        if (!string.IsNullOrEmpty(messageText))
                        {
                            txtRefresh.Text = messageText;
                        }
                        else
                        {
                            txtRefresh.Text = Utility.GetLocalizedCommonLabel("refreshDescription");
                        }

                        WhatsNewMenuUtils.OnSetTouchDisable(false);

                        try
                        {
                            if (WhatsNewEntity.HasUnread())
                            {
                                ((DashboardHomeActivity)this.Activity).ShowUnreadWhatsNew();
                            }
                            else
                            {
                                ((DashboardHomeActivity)this.Activity).HideUnreadWhatsNew();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (System.Exception err)
            {
                Utility.LoggingNonFatalError(err);
            }
        }

        public void OnShowWhatsNewMenuTutorial()
        {
            if (!UserSessions.HasWhatsNewShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
            {
                Handler h = new Handler();
                Action myAction = () =>
                {
                    NewAppTutorialUtils.OnShowNewAppTutorial(this.Activity, this, PreferenceManager.GetDefaultSharedPreferences(this.Activity), this.presenter.OnGeneraNewAppTutorialList());
                };
                h.PostDelayed(myAction, 100);
            }
        }

        public void StopScrolling()
        {
            try
            {
                if (mTabList != null && mTabList.Count > 0)
                {
                    mTabList[0].Fragment.StopScrolling();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public List<WhatsNewMenuModel> GetTabList()
        {
            return mTabList;
        }

        public bool CheckTabVisibility()
        {
            return whatsNewsSlidingTabs.Visibility == ViewStates.Visible;
        }

        public int GetTabRelativePosition()
        {
            int i = 0;

            try
            {
                int[] location = new int[2];
                whatsNewsSlidingTabs.GetLocationOnScreen(location);
                i = location[1];
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        public int GetTabHeight()
        {
            int i = 0;

            try
            {
                i = whatsNewsSlidingTabs.Height;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        [OnClick(Resource.Id.btnRefresh)]
        internal void OnRefresh(object sender, EventArgs e)
        {
            try
            {
                ((DashboardHomeActivity)Activity).ShowWhatsNewMenu();
            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
    }
}
