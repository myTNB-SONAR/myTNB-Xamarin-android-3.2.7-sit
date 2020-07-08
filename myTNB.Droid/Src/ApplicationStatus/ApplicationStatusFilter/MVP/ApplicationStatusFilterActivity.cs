using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.Adapter;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.Models;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.WhatsNewDetail.MVP;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.MVP
{
    [Activity(Label = "Select Filter", Theme = "@style/Theme.RegisterForm")]
    public class ApplicationStatusFilterActivity : BaseActivityCustom, ApplicationStatusLandingContract.IView
    {
        [BindView(Resource.Id.rootview)]
        LinearLayout rootview;

        [BindView(Resource.Id.applicationStatusLandingTitleLayout)]
        RelativeLayout applicationStatusLandingTitleLayout;

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

        private bool isFiltered = false;

        IMenuItem applicationFilterMenuItem;

        const string PAGE_ID = "ApplicationStatus";

        List<ApplicationStatusModel> applicationStatusList = new List<ApplicationStatusModel>();
        List<ApplicationStatusColorCodeModel> applicationStatusColorList = new List<ApplicationStatusColorCodeModel>();

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

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_notification:
                    // ApplicationStatus TODO: function
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.DashboardToolbarMenu, menu);
            applicationFilterMenuItem = menu.FindItem(Resource.Id.action_notification);
            applicationFilterMenuItem.SetIcon(ContextCompat.GetDrawable(this, Resource.Drawable.bill_screen_filter_icon));
            applicationFilterMenuItem.SetVisible(false);
            return base.OnCreateOptionsMenu(menu);
        }

        public void ShowApplicationFilterToolbar(bool isShow)
        {
            if (isShow)
            {
                // ApplicationStatus TODO: Multilingual
                SetToolBarTitle("My Application Status");
                applicationFilterMenuItem.SetVisible(true);
                UpdateFilterIcon();
            }
            else
            {
                // ApplicationStatus TODO: Multilingual
                SetToolBarTitle("Check Status");
                applicationFilterMenuItem.SetVisible(false);
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

            ApplicationOnScrollChangeListener applicationOnScrollChangeListener = new ApplicationOnScrollChangeListener(ShowApplicationFilterToolbar, applicationStatusLandingTitleLayout);
            applicationStatusLandingNestedScrollView.SetOnScrollChangeListener(applicationOnScrollChangeListener);

            // ApplicationStatus TODO: Multilingual
            SetToolBarTitle("Check Status");

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
            Action<bool> mOnScrollMethod;
            public ApplicationOnScrollChangeListener(Action<bool> onScrollMethod, RelativeLayout applicationLandingTitle)
            {
                mApplicationLandingTitle = applicationLandingTitle;
                mOnScrollMethod = onScrollMethod;
            }
            public void OnScrollChange(NestedScrollView v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
            {
                try
                {
                    bool IsWidgetVisible = isViewVisible(v, mApplicationLandingTitle);
                    mOnScrollMethod(IsWidgetVisible);
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

        public void UpdateUI()
        {
            try
            {
                StopShimmer();

                applicationStatusList = JsonConvert.DeserializeObject<List<ApplicationStatusModel>>("[{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}},{\"type\":\"Change Tariff\",\"typeCode\":\"changeTariff\",\"status\":\"Completed\",\"statusCode\":\"completed\",\"refCode\":\"SR\",\"accountNumber\":\"212112345678\",\"applicationNumber\":\"NC-100-456-7800\",\"srNumber\":\"SR: 10045678\",\"isUpdated\":true,\"applicationDate\":{\"month\":6,\"year\":\"2019\",\"formattedDate\":\"16 Jul 2019\"}}]");
                applicationStatusColorList = JsonConvert.DeserializeObject<List<ApplicationStatusColorCodeModel>>("[{\"code\":\"completed\",\"rgb\":{\"r\":32,\"g\":189,\"b\":76}}]");

                if (applicationStatusList != null && applicationStatusList.Count > 0)
                {
                    RunOnUiThread(() =>
                    {
                        try
                        {
                            viewMoreContainer.Visibility = ViewStates.Gone;
                            applicationStatusLandingRecyclerView.Visibility = ViewStates.Visible;
                            applicationStatusLandingFilterImg.Visibility = ViewStates.Visible;
                            applicationStatusLandingEmptyLayout.Visibility = ViewStates.Gone;
                            applicationStatusLandingAdapter = new ApplicationStatusLandingAdapter(this, applicationStatusList, applicationStatusColorList);
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
                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
