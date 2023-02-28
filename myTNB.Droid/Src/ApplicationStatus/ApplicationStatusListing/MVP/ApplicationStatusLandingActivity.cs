using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Facebook.Shimmer;
using Google.Android.Material.Snackbar;
using myTNB;
using myTNB.Mobile;
using myTNB.Mobile.SessionCache;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusFilter.MVP;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.Adapter;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.Models;
using myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.FindUs.Activity;
using myTNB_Android.Src.MyHome;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Deeplink;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.MVP
{
    [Activity(Label = "Application Status", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.AppointmentScheduler")]
    public class ApplicationStatusLandingActivity : BaseActivityCustom, ApplicationStatusLandingContract.IView
    {
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

        [BindView(Resource.Id.btnRefresh)]
        Button btnRefresh;

        [BindView(Resource.Id.applicationStatusLandingNestedScrollView)]
        NestedScrollView applicationStatusLandingNestedScrollView;

        [BindView(Resource.Id.applicationStatusRefreshContainer)]
        LinearLayout applicationStatusRefreshContainer;

        [BindView(Resource.Id.mappicatinPopup)]
        LinearLayout mappicationPopup;

        [BindView(Resource.Id.ApplicationStatusTooltip)]
        LinearLayout ApplicationStatusTooltip;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txtApplicationStatsTooltip)]
        TextView txtApplicationStatsTooltip;

        [BindView(Resource.Id.refreshMsg)]
        TextView refreshMsg;

        bool isFilter = false;
        bool isFilterVisible = false;
        [OnClick(Resource.Id.btnRefresh)]
        void OnDetailRefresh(object sender, EventArgs eventArgs)
        {
            IsRefresh = true;
            UpdateUI();
        }

        [OnClick(Resource.Id.btnSearchApplicationStatus)]
        void OnClickSearchApplicationStatus(object sender, EventArgs eventArgs)
        {
            GetSearchApplicationTypeAsync();
        }
        private void GetSearchApplicationTypeAsync()
        {
            try
            {
                if (ConnectionUtils.HasInternetConnection(this))
                {
                    SearchApplicationTypeResponse searchApplicationTypeResponse = SearchApplicationTypeCache.Instance.GetData();
                    Intent applicationLandingIntent = new Intent(this, typeof(SearchApplicationStatusActivity));
                    applicationLandingIntent.PutExtra("searchApplicationType", JsonConvert.SerializeObject(searchApplicationTypeResponse.Content));
                    applicationStatusLandingEmptyLayout.Visibility = ViewStates.Gone;
                    StartActivityForResult(applicationLandingIntent, Constants.APPLICATION_STATUS_SEARCH_DETAILS_REQUEST_CODE);
                }
                else
                {
                    ShowNoInternetSnackbar();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        internal bool IsEmpty = false;
        internal bool IsLoading = true;
        internal bool IsRefresh = false;
        internal bool IsViewMoreDisplayed = false;
        internal bool IsViewMore = false;
        internal int ResultCount = 0;
        internal string filterApplicationType = string.Empty;
        internal string filterStatus = string.Empty;
        internal string filterDate = string.Empty;
        internal string filterFromDate = string.Empty;
        internal string filterToDate = string.Empty;
        internal GetAllApplicationsResponse AllApplicationResponse;
        internal List<ApplicationModel> GetAllApplications;
        ApplicationStatusLandingPresenter mPresenter;

        ApplicationStatusLandingAdapter applicationStatusLandingAdapter;

        internal List<SelectorModel> _statusList;
        internal List<SelectorModel> _typeList;
        internal List<string> _statusDisplayList;
        internal List<string> _typeDisplayList;
        RecyclerView.LayoutManager layoutManager;

        ApplicationStatusLandingContract.IPresenter presenter;

        private bool isFiltered = false;

        IMenuItem applicationFilterMenuItem;

        const string PAGE_ID = "ApplicationStatus";

        private int currentIndex = 0;

        private int maxIndex = 1;

        private bool isApplicationStatusScrolled = false;

        private bool isOnlyHaveOneList = true;

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
                //returns the visible bounds
                btnSearchApplicationStatus.GetDrawingRect(offsetViewBounds);
                // calculates the relative coordinates to the parent
                rootView.OffsetDescendantRectToMyCoords(btnSearchApplicationStatus, offsetViewBounds);
                i = offsetViewBounds.Top;
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
        public int GetSearchButtonHeight()
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
                    OnNavigateToApplicationStatusFilter();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void OnNavigateToApplicationStatusFilter()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                _statusList = Utility.GetSelectoryByKey("SelectApplicationStatus", "status");
                _statusDisplayList = _statusList.Select(x => x.Value).ToList();

                List<ApplicationStatusTypeModel> applicationStatusTypeModel = new List<ApplicationStatusTypeModel>();
                List<ApplicationStatusCodeModel> applicationStatusCodeModel = new List<ApplicationStatusCodeModel>();
                _typeList = SearchApplicationTypeCache.Instance.GetApplicationTypeList();
                _typeDisplayList = _typeList.Select(x => x.Value).ToList();

                if (_typeList != null && _typeList.Count > 0)
                {
                    foreach (var searchTypeItem in _typeList)
                    {
                        applicationStatusTypeModel.Add(new ApplicationStatusTypeModel
                        {
                            Type = searchTypeItem.Value,
                            TypeCode = searchTypeItem.Key,
                            isChecked = false

                        });
                    }
                }

                if (_statusList != null && _statusList.Count > 0)
                {
                    foreach (var searchTypeItem in _statusList)
                    {
                        applicationStatusCodeModel.Add(new ApplicationStatusCodeModel
                        {
                            StateCode = searchTypeItem.Key,
                            Status = searchTypeItem.Value,
                            isChecked = false

                        });
                    }
                }

                Intent filterIntent = new Intent(this, typeof(ApplicationStatusFilterActivity));
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_TYPE_KEY, filterApplicationType);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_STATUS_KEY, filterStatus);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_DATE_KEY, filterDate);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_FROM_DATE_KEY, filterFromDate);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_FILTER_TO_DATE_KEY, filterToDate);
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_STATUS_LIST_KEY, JsonConvert.SerializeObject(applicationStatusCodeModel));
                filterIntent.PutExtra(Constants.APPLICATION_STATUS_TYPE_LIST_KEY, JsonConvert.SerializeObject(applicationStatusTypeModel));
                StartActivityForResult(filterIntent, Constants.APPLICATION_STATUS_FILTER_REQUEST_CODE);
            }
        }
        [OnClick(Resource.Id.ApplicationStatusTooltip)]
        void OnApplicationStatusTooltipClick(object sender, EventArgs eventArgs)
        {
            try
            {
                string textTitle = Utility.GetLocalizedLabel("ApplicationStatusLanding", "allApplicationsTitle");
                string textMessage = Utility.GetLocalizedLabel("ApplicationStatusLanding", "allApplicationsMessage");
                string btnLabel = Utility.GetLocalizedCommonLabel("gotIt");

                if (textTitle != "" && textMessage != "" && btnLabel != "")
                {
                    MyTNBAppToolTipBuilder whatIsThisTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                        .SetTitle(textTitle)
                        .SetMessage(textMessage)
                        .SetCTALabel(btnLabel)
                        .Build();
                    whatIsThisTooltip.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            ShowProgressDialog();
            base.OnActivityResult(requestCode, resultCode, data);

            if (data != null && data.Extras != null)
            {
                Bundle extras = data.Extras;
                if (extras != null)
                {
                    applicationStatusLandingRecyclerView.Visibility = ViewStates.Gone;
                    viewMoreContainer.Visibility = ViewStates.Gone;
                    mappicationPopup.Visibility = ViewStates.Gone;
                    applicationStatusLandingShimmerLayout.Visibility = ViewStates.Visible;
                    applicationStatusLandingEmptyLayout.Visibility = ViewStates.Gone;
                    applicationStatusLandingListContentShimmer.StartShimmer();

                    if (extras.ContainsKey(Constants.APPLICATION_STATUS_FILTER_TYPE_KEY))
                    {
                        try
                        {
                            currentIndex = 0;
                            filterApplicationType = JsonConvert.DeserializeObject<string>(extras.GetString(Constants.APPLICATION_STATUS_FILTER_TYPE_KEY));
                            filterStatus = JsonConvert.DeserializeObject<string>(extras.GetString(Constants.APPLICATION_STATUS_FILTER_STATUS_KEY));
                            filterFromDate = JsonConvert.DeserializeObject<string>(extras.GetString(Constants.APPLICATION_STATUS_FILTER_FROM_DATE_KEY));
                            filterToDate = JsonConvert.DeserializeObject<string>(extras.GetString(Constants.APPLICATION_STATUS_FILTER_TO_DATE_KEY));
                            filterDate = JsonConvert.DeserializeObject<string>(extras.GetString(Constants.APPLICATION_STATUS_FILTER_DATE_KEY));
                            AllApplicationResponse = JsonConvert.DeserializeObject<GetAllApplicationsResponse>(extras.GetString("ApplicaitonFlilterList"));
                            if (filterApplicationType != string.Empty || filterStatus != string.Empty || filterFromDate != string.Empty || filterToDate != string.Empty)
                            {
                                isFilter = true;

                            }
                            else
                            {

                                isFilter = false;
                                AllApplicationsCache.Instance.Clear();
                                AllApplicationsCache.Instance.Reset();
                            }
                            UpdateUI();


                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }

                }
            }
            if (requestCode == Constants.APPLICATION_STATUS_SEARCH_DETAILS_REQUEST_CODE || requestCode == Constants.APPLICATION_STATUS_DETAILS_REMOVE_REQUEST_CODE)
            {
                UpdateUI();
                if (resultCode == Result.Ok)
                {
                    string message = data.Extras.GetString(Constants.DELETE_DRAFT_MESSAGE);
                    if (message.IsValid())
                    {
                        ToastUtils.OnDisplayToast(this, message ?? string.Empty);
                    }
                }
            }
            HideProgressDialog();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.DashboardToolbarMenu, menu);
            applicationFilterMenuItem = menu.FindItem(Resource.Id.action_notification);
            applicationFilterMenuItem.SetIcon(ContextCompat.GetDrawable(this, Resource.Drawable.bill_screen_filter_icon));
            if (!isOnlyHaveOneList || !isFilterVisible)
            {
                applicationFilterMenuItem.SetVisible(false);
            }
            else
            {
                applicationFilterMenuItem.SetVisible(true);
                UpdateFilterIcon();
            }
            isFilterVisible = true;
            return base.OnCreateOptionsMenu(menu);
        }

        public void ShowApplicationFilterToolbar(bool isApplicationShow)
        {
            if (!isOnlyHaveOneList)
            {
                if (!isOnlyHaveOneList || !isFilterVisible)

                {
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                    {
                        SetToolBarTitle(Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusLanding", "title"), FromHtmlOptions.ModeLegacy).ToString());
                    }
                    else
                    {
                        SetToolBarTitle(Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusLanding", "title")).ToString());
                    }

                    isApplicationStatusScrolled = true;
                    applicationFilterMenuItem.SetVisible(true);
                    UpdateFilterIcon();
                }
                else
                {
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                    {
                        SetToolBarTitle(Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusLanding", "title"), FromHtmlOptions.ModeLegacy).ToString());
                    }
                    else
                    {
                        SetToolBarTitle(Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusLanding", "title")).ToString());
                    }

                    isApplicationStatusScrolled = false;
                    applicationFilterMenuItem.SetVisible(false);
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            isFilter = false;
            isFilterVisible = false;
            mPresenter = new ApplicationStatusLandingPresenter(this);
            // applicationStatusLandingEmptyLayout.Visibility = ViewStates.Visible;
            viewMoreContainer.Visibility = ViewStates.Gone;
            ApplicationStatusTooltip.Visibility = ViewStates.Gone;
            mappicationPopup.Visibility = ViewStates.Gone;
            applicationStatusLandingShimmerLayout.Visibility = ViewStates.Visible;
            applicationStatusLandingEmptyLayout.Visibility = ViewStates.Gone;
            applicationStatusLandingListContentShimmer.StartShimmer();

            TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusLandingEmpty, refreshMsg);
            TextViewUtils.SetMuseoSans500Typeface(btnSearchApplicationStatus, viewMoreLabel, txtApplicationStatsTooltip);
            TextViewUtils.SetTextSize12(viewMoreLabel, txtApplicationStatsTooltip);
            TextViewUtils.SetTextSize14(txtApplicationStatusLandingEmpty);
            TextViewUtils.SetTextSize16(refreshMsg, btnSearchApplicationStatus, btnRefresh);

            btnSearchApplicationStatus.Text = Utility.GetLocalizedLabel("ApplicationStatusLanding", "search");
            txtApplicationStatsTooltip.Text = Utility.GetLocalizedLabel("ApplicationStatusLanding", "allApplications");

            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            applicationStatusLandingRecyclerView.SetLayoutManager(layoutManager);

            ApplicationOnScrollChangeListener applicationOnScrollChangeListener = new ApplicationOnScrollChangeListener(ShowApplicationFilterToolbar);
            applicationStatusLandingNestedScrollView.SetOnScrollChangeListener(applicationOnScrollChangeListener);

            try
            {
                presenter = new ApplicationStatusLandingPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                SetToolBarTitle(Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusLanding", "title"), FromHtmlOptions.ModeLegacy).ToString());
            }
            else
            {
                SetToolBarTitle(Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusLanding", "title")).ToString());
            }
            ScaleEmptyStateImage();
            UpdateUI();
            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                if (extras.ContainsKey("SaveApplication"))
                {
                    StatusDetail statusDetails = new StatusDetail();
                    statusDetails = DeSerialze<StatusDetail>(extras.GetString("SaveApplication"));
                    if (statusDetails != null)
                    {
                        Snackbar mSaveSnackbar = Snackbar.Make(rootView,
                        statusDetails.Message,
                        Snackbar.LengthLong);
                        View v = mSaveSnackbar.View;
                        TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                        tv.SetMaxLines(5);
                        mSaveSnackbar.Show();
                    }
                }
                if (extras.ContainsKey("RemoveApplication"))
                {
                    StatusDetail statusDetails = new StatusDetail();
                    statusDetails = DeSerialze<StatusDetail>(extras.GetString("RemoveApplication"));
                    if (statusDetails != null)
                    {
                        Snackbar mRemoveSnackbar = Snackbar.Make(rootView,
                        statusDetails.Message,
                        Snackbar.LengthLong);
                        View v = mRemoveSnackbar.View;
                        TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                        tv.SetMaxLines(5);
                        mRemoveSnackbar.Show();
                    }
                }
                if (extras.ContainsKey(MyHomeConstants.CANCEL_TOAST_MESSAGE))
                {
                    string message = extras.GetString(MyHomeConstants.CANCEL_TOAST_MESSAGE);
                    if (message.IsValid())
                    {
                        ToastUtils.OnDisplayToast(this, message ?? string.Empty);
                    }
                }
            }

            if (ApplicationStatusSearchDetailCache.Instance.ShouldSave)
            {
                btnSearchApplicationStatus.Enabled = false;
                btnSearchApplicationStatus.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_outline);
                btnSearchApplicationStatus.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.silverChalice));
            }
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
                    if (applicationCount > 0)
                    {
                        NewAppTutorialUtils.OnShowNewAppTutorial(this, null, PreferenceManager.GetDefaultSharedPreferences(this)
                            , this.presenter.OnGeneraNewAppTutorialList(), true);
                    }
                    else
                    {
                        NewAppTutorialUtils.OnShowNewAppTutorial(this, null, PreferenceManager.GetDefaultSharedPreferences(this)
                            , this.presenter.OnGeneraNewAppTutorialEmptyList(), true);
                    }
                };
                h.PostDelayed(myAction, 100);
            }
        }

        private void SetupEmptyLandingText(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message))
                {
                    message = Utility.GetLocalizedLabel("ApplicationStatusLanding", "emptyApplications");
                }
                txtApplicationStatusLandingEmpty.TextFormatted = GetFormattedText(message);
                txtApplicationStatusLandingEmpty = LinkRedirectionUtils
                    .Create(this, Title)
                    .SetTextView(txtApplicationStatusLandingEmpty)
                    .SetMessage(message)
                    .Build()
                    .GetProcessedTextView();
            }
            catch (Exception e)
            {
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

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public void EnableButton()
        {
            this.SetIsClicked(false);
        }

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
            if (AllApplicationResponse != null && AllApplicationResponse.Content != null && AllApplicationResponse.Content.Applications != null)
            {
                OnShowApplicationStatusTutorial(AllApplicationResponse.Content.Applications.Count);
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
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    GetApplicationStatus(position);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        private void GetApplicationStatus(int position)
        {
            try
            {
                if (ConnectionUtils.HasInternetConnection(this))
                {
                    ShowProgressDialog();
                    ApplicationModel application = GetAllApplications[position];

                    Task.Run(() =>
                    {
                        _ = OnGetApplicationStatus(application);
                    });
                }
                else
                {
                    ShowNoInternetSnackbar();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async Task OnGetApplicationStatus(ApplicationModel application)
        {
            ApplicationDetailDisplay response = await ApplicationStatusManager.Instance.GetApplicationDetail(application.SavedApplicationId
                , application.ApplicationId
                , application.ApplicationType
                , application.System);

            this.RunOnUiThread(() =>
            {
                if (response.StatusDetail.IsSuccess)
                {
                    Intent applicationStatusDetailIntent = new Intent(this, typeof(ApplicationStatusDetailActivity));
                    applicationStatusDetailIntent.PutExtra("applicationStatusResponse", JsonConvert.SerializeObject(response.Content));
                    StartActivityForResult(applicationStatusDetailIntent, Constants.APPLICATION_STATUS_DETAILS_REMOVE_REQUEST_CODE);
                }
                else
                {
                    ShowApplicationPopupMessage(this, response.StatusDetail);
                }
                HideProgressDialog();
            });
        }

        public void ShowApplicationPopupMessage(Activity context, StatusDetail statusDetail)
        {
            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(statusDetail.Title)
                .SetMessage(statusDetail.Message)
                .SetCTALabel(statusDetail.PrimaryCTATitle)
                .Build();
            whereisMyacc.Show();

        }
        private void UpdateFilterIcon()
        {
            if (isFiltered)
            {
                applicationFilterMenuItem.SetIcon(Resource.Drawable.filter_filled);
            }
            else
            {
                applicationFilterMenuItem.SetIcon(Resource.Drawable.Icon_Bills_Filter_White);
            }
        }

        class ApplicationOnScrollChangeListener : Java.Lang.Object, NestedScrollView.IOnScrollChangeListener
        {
            RelativeLayout mApplicationLandingTitle;

            Action<bool> mOnScrollMethod;
            public ApplicationOnScrollChangeListener(Action<bool> onScrollMethod)
            {


                mOnScrollMethod = onScrollMethod;
            }
            public void OnScrollChange(NestedScrollView v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
            {
                try
                {
                    bool IsApplicationWidgetVisible = isViewVisible(v, mApplicationLandingTitle);
                    mOnScrollMethod(IsApplicationWidgetVisible);
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
            //  TODO: ApplicationStatus Api Calling
            OnProcessApplicationStatusListing();
        }

        private void OnProcessApplicationStatusListing()
        {
            try
            {
                viewMoreContainer.Visibility = ViewStates.Gone;

                RunOnUiThread(async () =>
                {
                    try
                    {
                        if (ConnectionUtils.HasInternetConnection(this))
                        {
                            applicationStatusLandingShimmerLayout.Visibility = ViewStates.Visible;
                            applicationStatusLandingEmptyLayout.Visibility = ViewStates.Gone;

                            applicationStatusLandingListContentShimmer.StartShimmer();

                            if (!AllApplicationResponse.Content.IsLastResults)
                            {
                                AllApplicationResponse = await ApplicationStatusManager.Instance.GetAllApplications(AllApplicationsCache.Instance.QueryPage
                                    , AllApplicationsCache.Instance.ApplicationType == null ? string.Empty : AllApplicationsCache.Instance.ApplicationType
                                    , AllApplicationsCache.Instance.StatusDescription
                                    , AllApplicationsCache.Instance.CreatedDateFrom
                                    , AllApplicationsCache.Instance.CreatedDateTo);

                            }
                            GetAllApplications = AllApplicationsCache.Instance.GetAllApplications();

                            StopShimmer();
                            if (AllApplicationResponse != null
                                && AllApplicationResponse.Content != null
                                && AllApplicationResponse.StatusDetail.IsSuccess)
                            {
                                applicationStatusLandingRecyclerView.Visibility = ViewStates.Visible;
                                ApplicationStatusTooltip.Visibility = ViewStates.Visible;
                                applicationStatusRefreshContainer.Visibility = ViewStates.Gone;
                                viewMoreContainer.Visibility = AllApplicationResponse.Content.IsShowMoreDisplayed ? ViewStates.Visible : ViewStates.Gone;
                                if ((currentIndex + 1) == maxIndex)
                                {
                                    List<ApplicationModel> innerList = new List<ApplicationModel>();
                                    if (GetAllApplications.Count > AllApplicationResponse.Content.Limit)
                                    {
                                        for (int i = 0; i < AllApplicationResponse.Content.Limit; i++)
                                        {
                                            innerList.Add(GetAllApplications[i]);
                                        }
                                    }
                                    else
                                    {
                                        innerList = GetAllApplications;
                                    }

                                    applicationStatusLandingAdapter.Clear();
                                    applicationStatusLandingAdapter.UpdateAddList(innerList);

                                    currentIndex = 0;

                                    viewMoreLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusLanding", "viewMore");
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
                                    int counter = GetAllApplications.Count;
                                    if (AllApplicationResponse.Content.IsLastResults)
                                    {
                                        //  TODO: ApplicationStatus Multilingual
                                        viewMoreLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusLanding", "viewLess");
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

                                    List<ApplicationModel> innerList = new List<ApplicationModel>();
                                    for (int i = 0; i < counter; i++)
                                    {
                                        innerList.Add(GetAllApplications[i]);
                                    }

                                    applicationStatusLandingAdapter.Clear();
                                    applicationStatusLandingAdapter.UpdateAddList(innerList);
                                }
                            }
                            else
                            {
                                StopShimmer();
                                viewMoreContainer.Visibility = ViewStates.Gone;
                                applicationStatusLandingRecyclerView.Visibility = ViewStates.Gone;

                                if (AllApplicationResponse != null && AllApplicationResponse.StatusDetail != null
                                    && AllApplicationResponse.StatusDetail.Code == "empty")
                                {
                                    SetupEmptyLandingText(AllApplicationResponse.StatusDetail.Message);
                                    applicationStatusLandingEmptyLayout.Visibility = ViewStates.Visible;
                                    applicationStatusLandingBottomLayout.Visibility = ViewStates.Visible;
                                    mappicationPopup.Visibility = ViewStates.Gone;
                                    applicationFilterMenuItem.SetVisible(false);
                                    OnShowApplicationStatusTutorial(0);
                                }
                                else
                                {
                                    applicationStatusRefreshContainer.Visibility = ViewStates.Visible;
                                    ApplicationStatusTooltip.Visibility = ViewStates.Gone;
                                }
                            }
                        }
                        else
                        {
                            ShowNoInternetSnackbar();
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
                bool isApplicationAvailable = false;
                RunOnUiThread(async () =>
                    {
                        try
                        {
                            if (ConnectionUtils.HasInternetConnection(this))
                            {

                                if (IsRefresh)
                                {
                                    //ShowProgressDialog();
                                }

                                if (ApplicationStatusSearchDetailCache.Instance.ShouldSave)
                                {
                                    GetApplicationStatusDisplay appDetails = ApplicationStatusSearchDetailCache.Instance.GetData();
                                    PostSaveApplicationResponse postSaveApplicationResponse = await ApplicationStatusManager.Instance.SaveApplication(
                                        appDetails.ApplicationDetail.ReferenceNo
                                        , appDetails.ApplicationDetail.ApplicationModuleId
                                        , appDetails.ApplicationTypeID
                                        , appDetails.ApplicationDetail.BackendReferenceNo
                                        , appDetails.ApplicationDetail.BackendApplicationType
                                        , appDetails.ApplicationDetail.BackendModule
                                        , appDetails.ApplicationDetail.StatusCode
                                        , appDetails.ApplicationDetail.CreatedDate.Value);
                                    if (postSaveApplicationResponse.StatusDetail.IsSuccess)
                                    {
                                        ToastUtils.OnDisplayToast(this, postSaveApplicationResponse.StatusDetail.Message ?? string.Empty);
                                    }
                                    else
                                    {
                                        MyTNBAppToolTipBuilder saveFailPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                                           .SetTitle(postSaveApplicationResponse.StatusDetail.Title)
                                           .SetMessage(postSaveApplicationResponse.StatusDetail.Message)
                                           .SetCTALabel(Utility.GetLocalizedCommonLabel("ok"))
                                           .Build();
                                        saveFailPopup.Show();
                                    }
                                    ApplicationStatusSearchDetailCache.Instance.Clear();
                                    btnSearchApplicationStatus.Enabled = true;
                                    btnSearchApplicationStatus.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_green_outline_button_background);
                                    btnSearchApplicationStatus.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.freshGreen));
                                }
                                if (!isFilter)
                                {
                                    applicationStatusRefreshContainer.Visibility = ViewStates.Gone;
                                    applicationStatusLandingRecyclerView.Visibility = ViewStates.Gone;
                                    mappicationPopup.Visibility = ViewStates.Gone;
                                    applicationStatusLandingShimmerLayout.Visibility = ViewStates.Visible;
                                    applicationStatusLandingEmptyLayout.Visibility = ViewStates.Gone;

                                    applicationStatusLandingListContentShimmer.StartShimmer();
                                    AllApplicationsCache.Instance.Clear();
                                    AllApplicationResponse = await ApplicationStatusManager.Instance.GetAllApplications(AllApplicationsCache.Instance.QueryPage
                                       , AllApplicationsCache.Instance.ApplicationType
                                       , AllApplicationsCache.Instance.StatusDescription
                                       , AllApplicationsCache.Instance.CreatedDateFrom
                                       , AllApplicationsCache.Instance.CreatedDateTo);
                                }


                                if (AllApplicationResponse != null
                                    && AllApplicationResponse.Content != null
                                    && AllApplicationResponse.StatusDetail.IsSuccess)
                                {
                                    applicationStatusRefreshContainer.Visibility = ViewStates.Gone;
                                    mappicationPopup.Visibility = ViewStates.Gone;
                                    ApplicationStatusTooltip.Visibility = ViewStates.Gone;

                                    GetAllApplications = AllApplicationsCache.Instance.GetAllApplications();
                                    StopShimmer();
                                    if (GetAllApplications != null && GetAllApplications.Count > 0)
                                    {
                                        ApplicationStatusTooltip.Visibility = ViewStates.Visible;
                                        isApplicationAvailable = true;
                                        viewMoreContainer.Visibility = AllApplicationResponse.Content.IsShowMoreDisplayed ? ViewStates.Visible : ViewStates.Gone;
                                        applicationStatusLandingRecyclerView.Visibility = ViewStates.Visible;

                                        applicationStatusLandingEmptyLayout.Visibility = ViewStates.Gone;
                                        applicationStatusLandingBottomLayout.Visibility = ViewStates.Visible;
                                        mappicationPopup.Visibility = ViewStates.Visible;
                                        List<ApplicationModel> innerList = new List<ApplicationModel>();
                                        if (GetAllApplications != null && AllApplicationResponse != null && AllApplicationResponse.Content.Total > AllApplicationResponse.Content.Limit)
                                        {

                                            for (int i = 0; i < AllApplicationResponse.Content.Limit; i++)
                                            {
                                                innerList.Add(GetAllApplications[i]);
                                            }
                                            maxIndex = (int)Math.Ceiling((double)AllApplicationResponse.Content.Total / AllApplicationResponse.Content.Limit);
                                        }
                                        else
                                        {
                                            innerList = GetAllApplications;
                                        }

                                        applicationStatusLandingAdapter = new ApplicationStatusLandingAdapter(this, innerList);
                                        applicationStatusLandingRecyclerView.SetAdapter(applicationStatusLandingAdapter);
                                        applicationStatusLandingAdapter.ItemClick += OnItemClick;
                                        applicationStatusLandingAdapter.NotifyDataSetChanged();
                                        OnShowApplicationStatusTutorial(AllApplicationResponse.Content.Total);

                                        if (AllApplicationResponse != null && GetAllApplications.Count < AllApplicationResponse.Content.Total)
                                        {
                                            viewMoreLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusLanding", "viewMore");
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
                                    }
                                    else
                                    {
                                        viewMoreContainer.Visibility = ViewStates.Gone;
                                        applicationStatusLandingRecyclerView.Visibility = ViewStates.Gone;
                                        applicationStatusLandingEmptyLayout.Visibility = ViewStates.Visible;
                                        applicationStatusLandingBottomLayout.Visibility = ViewStates.Visible;
                                        mappicationPopup.Visibility = ViewStates.Gone;
                                        applicationFilterMenuItem.SetVisible(false);
                                        OnShowApplicationStatusTutorial(0);
                                    }
                                }
                                else
                                {
                                    StopShimmer();
                                    viewMoreContainer.Visibility = ViewStates.Gone;
                                    applicationStatusLandingRecyclerView.Visibility = ViewStates.Gone;

                                    if (AllApplicationResponse != null && AllApplicationResponse.StatusDetail != null
                                        && AllApplicationResponse.StatusDetail.Code == "empty")
                                    {
                                        SetupEmptyLandingText(AllApplicationResponse.StatusDetail.Message);
                                        applicationStatusLandingEmptyLayout.Visibility = ViewStates.Visible;
                                        applicationStatusLandingBottomLayout.Visibility = ViewStates.Visible;
                                        mappicationPopup.Visibility = ViewStates.Gone;
                                        applicationFilterMenuItem.SetVisible(false);
                                        OnShowApplicationStatusTutorial(0);
                                    }
                                    else
                                    {
                                        applicationStatusRefreshContainer.Visibility = ViewStates.Visible;
                                        ApplicationStatusTooltip.Visibility = ViewStates.Gone;
                                    }
                                }

                                if (isApplicationAvailable)
                                {
                                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                                    {
                                        SetToolBarTitle(Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusLanding", "title"), FromHtmlOptions.ModeLegacy).ToString());
                                    }
                                    else
                                    {
                                        SetToolBarTitle(Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusLanding", "title")).ToString());
                                    }

                                    isApplicationStatusScrolled = true;
                                    applicationFilterMenuItem.SetVisible(true);
                                    UpdateFilterIcon();
                                }
                                else
                                {
                                    isOnlyHaveOneList = false;
                                    applicationFilterMenuItem.SetVisible(false);
                                }
                                if (IsRefresh)
                                {
                                    HideProgressDialog();
                                }
                                IsRefresh = false;
                            }
                            else
                            {
                                ShowNoInternetSnackbar();
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
        private Snackbar mNoInternetSnackbar;
        public void ShowNoInternetSnackbar()
        {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mNoInternetSnackbar.Dismiss();
            }
            );
            View v = mNoInternetSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mNoInternetSnackbar.Show();
            this.SetIsClicked(false);
        }
    }
}