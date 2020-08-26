using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.MVP;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.Adapter;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.WhatsNewDetail.MVP;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.MVP
{
    [Activity(Label = "Application Status", Theme = "@style/Theme.AppointmentScheduler")]
    public class ApplicationStatusLandingActivity : BaseActivityCustom, ApplicationStatusLandingContract.IView
    {
        [BindView(Resource.Id.rootview)]
        LinearLayout rootview;

        [BindView(Resource.Id.applicationStatusLandingTitleLayout)]
        RelativeLayout applicationStatusLandingTitleLayout;

        [BindView(Resource.Id.projectStatusLandingTitleLayout)]
        RelativeLayout projectStatusLandingTitleLayout;

        [BindView(Resource.Id.txtApplicationStatusLandingTitle)]
        TextView txtApplicationStatusLandingTitle;


        [BindView(Resource.Id.applicationStatusLandingFilterImg)]
        ImageView applicationStatusLandingFilterImg;

        [BindView(Resource.Id.applicationStatusLandingEmptyLayout)]
        LinearLayout applicationStatusLandingEmptyLayout;

        [BindView(Resource.Id.applicationStatusLandingEmptyImg)]
        ImageView applicationStatusLandingEmptyImg;

        [BindView(Resource.Id.txtApplicationStatusLandingEmpty)]
        TextView txtApplicationStatusLandingEmpty;

        [BindView(Resource.Id.applicationStatusLandingShimmerLayout)]
        LinearLayout applicationStatusLandingShimmerLayout;

        [BindView(Resource.Id.applicationStatusLandingListContentShimmer)]
        ShimmerFrameLayout applicationStatusLandingListContentShimmer;

        [BindView(Resource.Id.applicationStatusLandingRecyclerView)]
        RecyclerView applicationStatusLandingRecyclerView;

        [BindView(Resource.Id.viewMoreContainer)]
        LinearLayout viewMoreContainer;

        [BindView(Resource.Id.viewMoreLabel)]
        TextView viewMoreLabel;

        [BindView(Resource.Id.viewMoreImg)]
        ImageView viewMoreImg;

        [BindView(Resource.Id.applicationStatusLandingBottomLayout)]
        LinearLayout applicationStatusLandingBottomLayout;

        [BindView(Resource.Id.btnSearchApplicationStatus)]
        Button btnSearchApplicationStatus;

        [BindView(Resource.Id.applicationStatusLandingNestedScrollView)]
        NestedScrollView applicationStatusLandingNestedScrollView;

        ApplicationStatusLandingPresenter mPresenter;

        ApplicationStatusLandingAdapter applicationStatusLandingAdapter;

        RecyclerView.LayoutManager layoutManager;

        ApplicationStatusLandingContract.IPresenter presenter;

        private bool isFiltered = false;

        IMenuItem applicationFilterMenuItem;

        const string PAGE_ID = "ApplicationStatus";

        private int currentIndex = 0;

        private int maxIndex = 1;

        private string filterApplicationType = "";
        private string filterStatus = "";
        private string filterDate = "";

        private bool isApplicationStatusScrolled = false;
        private bool isProjectStatusScrolled = false;
        private bool isOnlyHaveOneList = true;

        List<ApplicationStatusModel> applicationStatusList = new List<ApplicationStatusModel>();
        List<ApplicationStatusModel> projectStatusList = new List<ApplicationStatusModel>();
        List<ApplicationStatusColorCodeModel> applicationStatusColorList = new List<ApplicationStatusColorCodeModel>();
        List<ApplicationStatusCodeModel> statusCodeList = new List<ApplicationStatusCodeModel>();
        List<ApplicationStatusTypeModel> typeList = new List<ApplicationStatusTypeModel>();

        public override int ResourceId()
        {
            return Resource.Layout.ApplicationStatusLandingLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }
        public int GetTopHeight()
        {
            int i = 0;

            try
            {
                int[] location = new int[2];
                applicationStatusLandingRecyclerView.GetLocationOnScreen(location);
                i = location[1];

              
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }
        public int GetTopSearchHeight()
        {
            int i = 0;

            try
            {
                Rect offsetViewBounds = new Rect();

                //int[] location = new int[2];
                //applicationStatusLandingRecyclerView.GetLocationOnScreen(location);
                //i = location[1];


                //returns the visible bounds
                btnSearchApplicationStatus.GetDrawingRect(offsetViewBounds);
                // calculates the relative coordinates to the parent

                rootview.OffsetDescendantRectToMyCoords(btnSearchApplicationStatus, offsetViewBounds);

                i = offsetViewBounds.Top + (int)DPUtils.ConvertDPToPx(14f);


            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }
       

        public int GetHighlightedHeight()
        {
            int i = 0;

            try
            {
                int aheight;
                aheight = applicationStatusLandingRecyclerView.Height / applicationStatusLandingAdapter.ItemCount;
                i = applicationStatusLandingAdapter.ItemCount > 1 ? i = aheight * 2 : aheight;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }
        public int    GetSearchButtonHeight()
        {
            int i = 0;

            try
            {

                i = btnSearchApplicationStatus.Height;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_notification:
                    if (isProjectStatusScrolled)
                    {

                    }
                    else
                    {
                        OnNavigateToApplicationStatusFilter();
                    }
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void OnNavigateToApplicationStatusFilter()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent filterIntent = new Intent(this, typeof(ApplicationStatusFilterActivity));
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_TYPE_KEY, filterApplicationType);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_STATUS_KEY, filterStatus);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_DATE_KEY, filterDate);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_STATUS_LIST_KEY, JsonConvert.SerializeObject(statusCodeList));
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_TYPE_LIST_KEY, JsonConvert.SerializeObject(typeList));
                StartActivityForResult(filterIntent, Constants.APPLICATION_STATUS_FILTER_REQUEST_CODE);
            }
        }

        [OnClick(Resource.Id.applicationStatusLandingFilterImg)]
        void OnApplicationStatusFilter(object sender, EventArgs eventArgs)
        {
            OnNavigateToApplicationStatusFilter();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.DashboardToolbarMenu, menu);
            applicationFilterMenuItem = menu.FindItem(Resource.Id.action_notification);
            applicationFilterMenuItem.SetIcon(ContextCompat.GetDrawable(this, Resource.Drawable.bill_screen_filter_icon));
            if (!isOnlyHaveOneList)
            {
                applicationFilterMenuItem.SetVisible(false);
            }
            else
            {
                applicationFilterMenuItem.SetVisible(true);
                UpdateFilterIcon();
            }
            return base.OnCreateOptionsMenu(menu);
        }

        public void ShowApplicationFilterToolbar(bool isApplicationShow, bool isProjectShow)
        {
            if (!isOnlyHaveOneList)
            {
                if (isProjectShow)
                {
                    // ApplicationStatus TODO: Multilingual
                    SetToolBarTitle("My Project Status");
                    isProjectStatusScrolled = true;
                    applicationFilterMenuItem.SetVisible(true);
                    UpdateFilterIcon();
                }
                else if (isApplicationShow)
                {
                    // ApplicationStatus TODO: Multilingual
                    SetToolBarTitle("My Application Status");
                    isProjectStatusScrolled = false;
                    isApplicationStatusScrolled = true;
                    applicationFilterMenuItem.SetVisible(true);
                    UpdateFilterIcon();
                }
                else
                {
                    // ApplicationStatus TODO: Multilingual
                    SetToolBarTitle("Application Status");
                    isProjectStatusScrolled = false;
                    isApplicationStatusScrolled = false;
                    applicationFilterMenuItem.SetVisible(false);
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new ApplicationStatusLandingPresenter(this);

            TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusLandingEmpty);
            TextViewUtils.SetMuseoSans500Typeface(btnSearchApplicationStatus, txtApplicationStatusLandingTitle, viewMoreLabel);

            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            applicationStatusLandingRecyclerView.SetLayoutManager(layoutManager);

            ApplicationOnScrollChangeListener applicationOnScrollChangeListener = new ApplicationOnScrollChangeListener(ShowApplicationFilterToolbar, applicationStatusLandingTitleLayout, projectStatusLandingTitleLayout);
            applicationStatusLandingNestedScrollView.SetOnScrollChangeListener(applicationOnScrollChangeListener);

            try
            {
                presenter = new ApplicationStatusLandingPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            // ApplicationStatus TODO: Multilingual
            SetToolBarTitle("Application Status");

            ScaleEmptyStateImage();
            SetupEmptyLandingText();

            // ApplicationStatis TODO: Stub
            UpdateUI();
        }

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
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

        public void HideProgressDialog()
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

        private void ScaleEmptyStateImage()
        {
            try
            {
                ViewGroup.LayoutParams currentImg = applicationStatusLandingEmptyImg.LayoutParameters;

                currentImg.Height = GetDeviceVerticalScaleInPixel(0.141f);
                currentImg.Width = GetDeviceHorizontalScaleInPixel(0.26f);

                applicationStatusLandingEmptyImg.RequestLayout();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public void OnShowApplicationStatusTutorial(int applicationCount)
        {
            if (!UserSessions.HasApplicationStatusShown(PreferenceManager.GetDefaultSharedPreferences(this)))
            {
                Handler h = new Handler();
                Action myAction = () =>
                {
                    if(applicationCount > 0)
                    {
                        NewAppTutorialUtils.OnShowNewAppTutorial(this, null, PreferenceManager.GetDefaultSharedPreferences(this), this.presenter.OnGeneraNewAppTutorialList(), true);
                    }
                    else
                    {
                        NewAppTutorialUtils.OnShowNewAppTutorial(this, null, PreferenceManager.GetDefaultSharedPreferences(this), this.presenter.OnGeneraNewAppTutorialEmptyList(), true);
                    }
                    
                };
                h.PostDelayed(myAction, 100);
            }
        }

        class ClickSpan : ClickableSpan
        {
            public Action<View> Click;
            public Color textColor { get; set; }
            public Typeface typeFace { get; set; }

            public override void OnClick(View widget)
            {
                if (Click != null)
                {
                    Click(widget);
                }
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                base.UpdateDrawState(ds);
                ds.Color = textColor;
                ds.SetTypeface(typeFace);
                ds.UnderlineText = false;
            }
        }

        private void SetupEmptyLandingText()
        {
            try
            {
                // ApplicationStatus TODO: Multilingual
                string input = "Looks like you have no applications at the moment.";
                txtApplicationStatusLandingEmpty.TextFormatted = GetFormattedText(input);

                if (!string.IsNullOrEmpty(input))
                {
                    SpannableString s = new SpannableString(txtApplicationStatusLandingEmpty.TextFormatted);
                    var urlSpans = s.GetSpans(0, s.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));

                    if (urlSpans != null && urlSpans.Length > 0)
                    {
                        for (int i = 0; i < urlSpans.Length; i++)
                        {
                            URLSpan URLItem = urlSpans[i] as URLSpan;
                            int startIndex = s.GetSpanStart(urlSpans[i]);
                            int endIndex = s.GetSpanEnd(urlSpans[i]);
                            s.RemoveSpan(urlSpans[i]);
                            ClickSpan clickableSpan = new ClickSpan()
                            {
                                textColor = new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.powerBlue)),
                                typeFace = Typeface.CreateFromAsset(this.Assets, "fonts/" + TextViewUtils.MuseoSans500)
                            };
                            clickableSpan.Click += v =>
                            {
                                OnClickSpan(URLItem.URL);
                            };
                            s.SetSpan(clickableSpan, startIndex, endIndex, SpanTypes.ExclusiveExclusive);
                        }
                        txtApplicationStatusLandingEmpty.TextFormatted = s;
                        txtApplicationStatusLandingEmpty.MovementMethod = new LinkMovementMethod();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnClickSpan(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);
                        if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[0])
                            || url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[1])
                            || url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[6]))
                        {
                            string uri = url;
                            if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[0]))
                            {
                                uri = url.Split(MyTNBAppToolTipBuilder.RedirectTypeList[0])[1];
                            }
                            else if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[1]))
                            {
                                uri = url.Split(MyTNBAppToolTipBuilder.RedirectTypeList[1])[1];
                            }

                            if (!uri.Contains("http"))
                            {
                                uri = "http://" + uri;
                            }

                            if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[1]))
                            {
                                Intent intent = new Intent(Intent.ActionView);
                                intent.SetData(Android.Net.Uri.Parse(uri));
                                this.StartActivity(intent);
                            }
                            else
                            {
                                if (uri.Contains(".pdf") && !uri.Contains("docs.google"))
                                {
                                    Intent webIntent = new Intent(this, typeof(BasePDFViewerActivity));
                                    webIntent.PutExtra(Constants.IN_APP_LINK, uri);
                                    webIntent.PutExtra(Constants.IN_APP_TITLE, Title);
                                    this.StartActivity(webIntent);
                                }
                                else
                                {
                                    Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
                                    webIntent.PutExtra(Constants.IN_APP_LINK, uri);
                                    webIntent.PutExtra(Constants.IN_APP_TITLE, Title);
                                    this.StartActivity(webIntent);
                                }
                            }
                        }
                        else if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[2])
                                    || url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[7]))
                        {
                            string phonenum = url;
                            if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[2]))
                            {
                                phonenum = url.Split(MyTNBAppToolTipBuilder.RedirectTypeList[2])[1];
                            }
                            if (!string.IsNullOrEmpty(phonenum))
                            {
                                if (!phonenum.Contains("tel:"))
                                {
                                    phonenum = "tel:" + phonenum;
                                }

                                var call = Android.Net.Uri.Parse(phonenum);
                                var callIntent = new Intent(Intent.ActionView, call);
                                this.StartActivity(callIntent);
                            }
                            else
                            {
                                this.SetIsClicked(false);
                            }
                        }
                        else if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[3])
                                    || url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[8]))
                        {
                            string whatsnewid = url;
                            if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[3]))
                            {
                                whatsnewid = url.Split(MyTNBAppToolTipBuilder.RedirectTypeList[3])[1];
                            }
                            else if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[8]))
                            {
                                whatsnewid = url.Split(MyTNBAppToolTipBuilder.RedirectTypeList[8])[1];
                            }

                            if (!string.IsNullOrEmpty(whatsnewid))
                            {
                                if (!whatsnewid.Contains("{"))
                                {
                                    whatsnewid = "{" + whatsnewid;
                                }

                                if (!whatsnewid.Contains("}"))
                                {
                                    whatsnewid = whatsnewid + "}";
                                }

                                WhatsNewEntity wtManager = new WhatsNewEntity();

                                WhatsNewEntity item = wtManager.GetItem(whatsnewid);

                                if (item != null)
                                {
                                    if (!item.Read)
                                    {
                                        this.mPresenter.UpdateWhatsNewRead(item.ID, true);
                                    }

                                    Intent activity = new Intent(this, typeof(WhatsNewDetailActivity));
                                    activity.PutExtra(Constants.WHATS_NEW_DETAIL_ITEM_KEY, whatsnewid);
                                    activity.PutExtra(Constants.WHATS_NEW_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "promotion"));
                                    this.StartActivity(activity);
                                }
                                else
                                {
                                    this.SetIsClicked(false);
                                }
                            }
                            else
                            {
                                this.SetIsClicked(false);
                            }
                        }
                        else if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[4])
                                    || url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[9]))
                        {
                            string faqid = url;
                            if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[4]))
                            {
                                faqid = url.Split(MyTNBAppToolTipBuilder.RedirectTypeList[4])[1];
                            }
                            else if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[9]))
                            {
                                faqid = url.Split(MyTNBAppToolTipBuilder.RedirectTypeList[9])[1];
                            }

                            if (!string.IsNullOrEmpty(faqid))
                            {
                                if (!faqid.Contains("{"))
                                {
                                    faqid = "{" + faqid;
                                }

                                if (!faqid.Contains("}"))
                                {
                                    faqid = faqid + "}";
                                }

                                Intent faqIntent = new Intent(this, typeof(FAQListActivity));
                                faqIntent.PutExtra(Constants.FAQ_ID_PARAM, faqid);
                                this.StartActivity(faqIntent);
                            }
                            else
                            {
                                this.SetIsClicked(false);
                            }
                        }
                        else if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[5])
                                    || url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[10]))
                        {
                            string rewardid = url;
                            if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[5]))
                            {
                                rewardid = url.Split(MyTNBAppToolTipBuilder.RedirectTypeList[5])[1];
                            }
                            else if (url.Contains(MyTNBAppToolTipBuilder.RedirectTypeList[10]))
                            {
                                rewardid = url.Split(MyTNBAppToolTipBuilder.RedirectTypeList[10])[1];
                            }

                            if (!string.IsNullOrEmpty(rewardid))
                            {
                                if (!rewardid.Contains("{"))
                                {
                                    rewardid = "{" + rewardid;
                                }

                                if (!rewardid.Contains("}"))
                                {
                                    rewardid = rewardid + "}";
                                }

                                RewardsEntity wtManager = new RewardsEntity();

                                RewardsEntity item = wtManager.GetItem(rewardid);

                                if (item != null)
                                {
                                    if (!item.Read)
                                    {
                                        this.mPresenter.UpdateRewardRead(item.ID, true);
                                    }

                                    Intent activity = new Intent(this, typeof(RewardDetailActivity));
                                    activity.PutExtra(Constants.REWARD_DETAIL_ITEM_KEY, rewardid);
                                    activity.PutExtra(Constants.REWARD_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "rewards"));
                                    this.StartActivity(activity);
                                }
                                else
                                {
                                    this.SetIsClicked(false);
                                }
                            }
                            else
                            {
                                this.SetIsClicked(false);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public int GetDeviceHorizontalScaleInPixel(float percentageValue)
        {
            var deviceWidth = Resources.DisplayMetrics.WidthPixels;
            return GetScaleInPixel(deviceWidth, percentageValue);
        }

        public int GetDeviceVerticalScaleInPixel(float percentageValue)
        {
            var deviceHeight = Resources.DisplayMetrics.HeightPixels;
            return GetScaleInPixel(deviceHeight, percentageValue);
        }

        public int GetScaleInPixel(int basePixel, float percentageValue)
        {
            int scaledInPixel = (int)((float)basePixel * percentageValue);
            return scaledInPixel;
        }

        /*[OnClick(Resource.Id.btnSubmitRegistration)]
        void SubmitRegistration(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {

            }
        }*/


        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public void EnableButton()
        {
            this.SetIsClicked(false);
        }

        /*[OnClick(Resource.Id.txtTermsAndCondition)]
        void OnTermsConditions(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                StartActivity(typeof(TermsAndConditionActivity));
            }
        }*/

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Application Status Landing");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            OnShowApplicationStatusTutorial(applicationStatusList.Count);
        }

        private void StopShimmer()
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        applicationStatusLandingShimmerLayout.Visibility = ViewStates.Gone;

                        if (applicationStatusLandingListContentShimmer.IsShimmerStarted)
                        {
                            applicationStatusLandingListContentShimmer.StopShimmer();
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void OnItemClick(object sender, int position)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    Intent DetailIntent = new Intent(this, typeof(ApplicationStatusDetailActivity));
                    StartActivity(DetailIntent);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void UpdateFilterIcon()
        {
            if (isFiltered)
            {
                applicationFilterMenuItem.SetIcon(Resource.Drawable.filter_filled);
                applicationStatusLandingFilterImg.SetImageResource(Resource.Drawable.filter_blue);
            }
            else
            {
                applicationFilterMenuItem.SetIcon(Resource.Drawable.filter_white);
                applicationStatusLandingFilterImg.SetImageResource(Resource.Drawable.bill_screen_filter_icon);
            }
        }

        class ApplicationOnScrollChangeListener : Java.Lang.Object, NestedScrollView.IOnScrollChangeListener
        {
            RelativeLayout mApplicationLandingTitle;
            RelativeLayout mProjectStatusLandingTitleLayout;
            Action<bool, bool> mOnScrollMethod;
            public ApplicationOnScrollChangeListener(Action<bool, bool> onScrollMethod, RelativeLayout applicationLandingTitle, RelativeLayout projectLandingTitle)
            {
                mApplicationLandingTitle = applicationLandingTitle;
                mProjectStatusLandingTitleLayout = projectLandingTitle;
                mOnScrollMethod = onScrollMethod;
            }
            public void OnScrollChange(NestedScrollView v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
            {
                try
                {
                    bool IsApplicationWidgetVisible = isViewVisible(v, mApplicationLandingTitle);
                    bool IsProjectWidgetVisible = isViewVisible(v, mProjectStatusLandingTitleLayout);
                    mOnScrollMethod(IsApplicationWidgetVisible, IsProjectWidgetVisible);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            private bool isViewVisible(NestedScrollView v, View view)
            {
                Rect scrollBounds = new Rect();
                v.GetDrawingRect(scrollBounds);

                if (view.Height <= 0)
                {
                    return false;
                }
                else
                {
                    float top = view.GetY() + view.Height;
                    float bottom = top + view.Height;

                    if (scrollBounds.Top < top && scrollBounds.Bottom > bottom)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        [OnClick(Resource.Id.viewMoreContainer)]
        internal void OnViewMorelick(object sender, EventArgs e)
        {
            // ApplicationStatus TODO: Api Calling
            int maxCount = maxIndex * Constants.APPLICATION_STATUS_LISTING_LIMIT;
            // this.mPresenter.DoLoadMoreAccount();
            OnProcessApplicationStatusListing();
        }

        private void OnProcessApplicationStatusListing()
        {
            try
            {
                RunOnUiThread(() =>
                {
                    try
                    {
                        if ((currentIndex + 1) == maxIndex)
                        {
                            List<ApplicationStatusModel> innerList = new List<ApplicationStatusModel>();
                            if (applicationStatusList.Count > 5)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    innerList.Add(applicationStatusList[i]);
                                }
                            }
                            else
                            {
                                innerList = applicationStatusList;
                            }

                            applicationStatusLandingAdapter.Clear();
                            applicationStatusLandingAdapter.UpdateAddList(innerList);

                            currentIndex = 0;

                            // ApplicationStatus TODO: Multilingual
                            viewMoreLabel.Text = "View More";
                            AnimationSet animSet = new AnimationSet(true);
                            animSet.Interpolator = new DecelerateInterpolator();
                            animSet.FillAfter = true;
                            animSet.FillEnabled = true;

                            RotateAnimation animRotate = new RotateAnimation(-180.0f, 0,
                                Dimension.RelativeToSelf, 0.5f,
                                Dimension.RelativeToSelf, 0.5f);

                            animRotate.Duration = 500;
                            animRotate.FillAfter = true;
                            animSet.AddAnimation(animRotate);

                            viewMoreImg.StartAnimation(animSet);

                        }
                        else
                        {
                            currentIndex++;
                            int counter = (currentIndex + 1) * Constants.APPLICATION_STATUS_LISTING_LIMIT;
                            if (counter > applicationStatusList.Count)
                            {
                                counter = applicationStatusList.Count;
                            }

                            if ((currentIndex + 1) == maxIndex)
                            {
                                // ApplicationStatus TODO: Multilingual
                                viewMoreLabel.Text = "View Less";
                                AnimationSet animSet = new AnimationSet(true);
                                animSet.Interpolator = new DecelerateInterpolator();
                                animSet.FillAfter = true;
                                animSet.FillEnabled = true;

                                RotateAnimation animRotate = new RotateAnimation(0.0f, -180.0f,
                                    Dimension.RelativeToSelf, 0.5f,
                                    Dimension.RelativeToSelf, 0.5f);

                                animRotate.Duration = 500;
                                animRotate.FillAfter = true;
                                animSet.AddAnimation(animRotate);

                                viewMoreImg.StartAnimation(animSet);
                            }

                            List<ApplicationStatusModel> innerList = new List<ApplicationStatusModel>();
                            for (int i = 0; i < counter; i++)
                            {
                                innerList.Add(applicationStatusList[i]);
                            }

                            applicationStatusLandingAdapter.Clear();
                            applicationStatusLandingAdapter.UpdateAddList(innerList);
                        }
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateUI()
        {
            try
            {
                StopShimmer();

                // ApplicationStatus TODO: Stub
                applicationStatusList = JsonConvert.DeserializeObject<List<ApplicationStatusModel>>("[{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}}]");
                applicationStatusColorList = JsonConvert.DeserializeObject<List<ApplicationStatusColorCodeModel>>("[{\"code\":\"completed\",\"rgb\":{\"r\":32,\"g\":189,\"b\":76}}]");
                statusCodeList = JsonConvert.DeserializeObject<List<ApplicationStatusCodeModel>>("[{\"status\":\"Completed\",\"statusCode\":\"completed\"}]");
                typeList = JsonConvert.DeserializeObject<List<ApplicationStatusTypeModel>>("[{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\"}]");

                bool isApplicationAvailable = false;
                bool isProjectAvailable = false;

                if (applicationStatusList != null && applicationStatusList.Count > 0)
                {
                    RunOnUiThread(() =>
                    {
                        try
                        {
                            isApplicationAvailable = true;
                            viewMoreContainer.Visibility = ViewStates.Gone;
                            applicationStatusLandingRecyclerView.Visibility = ViewStates.Visible;
                            applicationStatusLandingFilterImg.Visibility = ViewStates.Visible;
                            applicationStatusLandingEmptyLayout.Visibility = ViewStates.Gone;
                            applicationStatusLandingBottomLayout.Visibility = ViewStates.Visible;
                            List<ApplicationStatusModel> innerList = new List<ApplicationStatusModel>();
                            if (applicationStatusList.Count > 5)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    innerList.Add(applicationStatusList[i]);
                                }
                                maxIndex = (int) Math.Ceiling((double)applicationStatusList.Count / 5);
                                viewMoreContainer.Visibility = ViewStates.Visible;
                            }
                            else
                            {
                                innerList = applicationStatusList;
                            }
                            applicationStatusLandingAdapter = new ApplicationStatusLandingAdapter(this, innerList, applicationStatusColorList);
                            applicationStatusLandingRecyclerView.SetAdapter(applicationStatusLandingAdapter);
                            applicationStatusLandingAdapter.ItemClick += OnItemClick;
                            applicationStatusLandingAdapter.NotifyDataSetChanged();

                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    });
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        try
                        {
                            viewMoreContainer.Visibility = ViewStates.Gone;
                            applicationStatusLandingRecyclerView.Visibility = ViewStates.Gone;
                            applicationStatusLandingFilterImg.Visibility = ViewStates.Gone;
                            applicationStatusLandingEmptyLayout.Visibility = ViewStates.Visible;
                            applicationStatusLandingBottomLayout.Visibility = ViewStates.Visible;
                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    });
                }

                // ApplicationStatus TODO: Project List handling
                if (projectStatusList != null && projectStatusList.Count > 0)
                {
                    RunOnUiThread(() =>
                    {
                        try
                        {
                            isProjectAvailable = true;

                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    });
                }
                else
                {

                }

                if (isApplicationAvailable && !isProjectAvailable)
                {
                    applicationStatusLandingTitleLayout.Visibility = ViewStates.Gone;
                    // ApplicationStatus TODO: Multilingual
                    SetToolBarTitle("My Application Status");
                    isProjectStatusScrolled = false;
                    isApplicationStatusScrolled = true;
                    applicationFilterMenuItem.SetVisible(true);
                    UpdateFilterIcon();
                }
                else
                {
                    isOnlyHaveOneList = false;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
