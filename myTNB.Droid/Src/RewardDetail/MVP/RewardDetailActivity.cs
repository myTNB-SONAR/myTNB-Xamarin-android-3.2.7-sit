using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Icu.Text;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime;
using System.Timers;

namespace myTNB_Android.Src.RewardDetail.MVP
{
    [Activity(Label = "Rewards"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Dashboard")]
    public class RewardDetailActivity : BaseToolbarAppCompatActivity, RewardDetailContract.IRewardDetailView
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.rewardMainImg)]
        RelativeLayout rewardMainImg;

        [BindView(Resource.Id.rewardImg)]
        ImageView rewardImg;

        [BindView(Resource.Id.rewardUsedLayout)]
        LinearLayout rewardUsedLayout;

        [BindView(Resource.Id.txtRewardUsed)]
        TextView txtRewardUsed;

        [BindView(Resource.Id.rewardMainShimmerImgLayout)]
        LinearLayout rewardMainShimmerImgLayout;

        [BindView(Resource.Id.txtTitle)]
        TextView txtTitle;

        [BindView(Resource.Id.txtRewardPeriodTitle)]
        TextView txtRewardPeriodTitle;

        [BindView(Resource.Id.txtRewardPeriodContent)]
        TextView txtRewardPeriodContent;

        [BindView(Resource.Id.txtRewardLocationTitle)]
        TextView txtRewardLocationTitle;

        [BindView(Resource.Id.txtRewardLocationContent)]
        TextView txtRewardLocationContent;

        [BindView(Resource.Id.txtRewardConditionTitle)]
        TextView txtRewardConditionTitle;

        [BindView(Resource.Id.txtRewardConditionContent)]
        TextView txtRewardConditionContent;

        [BindView(Resource.Id.btnRewardSave)]
        LinearLayout btnRewardSave;

        [BindView(Resource.Id.imgBtnRewardSave)]
        ImageView imgBtnRewardSave;

        [BindView(Resource.Id.txtBtnRewardSave)]
        TextView txtBtnRewardSave;

        [BindView(Resource.Id.btnRewardUse)]
        Button btnRewardUse;

        [BindView(Resource.Id.shimmerRewardImageLayout)]
        ShimmerFrameLayout shimmerRewardImageLayout;

        [BindView(Resource.Id.btnUseSaveLayout)]
        LinearLayout btnUseSaveLayout;

        [BindView(Resource.Id.rewardRedeemedLayout)]
        LinearLayout rewardRedeemedLayout;

        [BindView(Resource.Id.btnRewardRedeemed)]
        Button btnRewardRedeemed;

        [BindView(Resource.Id.rewardCountDownLayout)]
        LinearLayout rewardCountDownLayout;

        [BindView(Resource.Id.txtTimeCounter)]
        TextView txtTimeCounter;

        [BindView(Resource.Id.txtRewardRedeemedWord)]
        TextView txtRewardRedeemedWord;

        [BindView(Resource.Id.txtRewardUsedDateTime)]
        TextView txtRewardUsedDateTime;

        [BindView(Resource.Id.imgRewardPeriod)]
        ImageView imgRewardPeriod;

        [BindView(Resource.Id.imgRewardLocation)]
        ImageView imgRewardLocation;

        [BindView(Resource.Id.imgRewardCondition)]
        ImageView imgRewardCondition;

        RewardDetailContract.IRewardDetailPresenter presenter;

        private RewardsModel LocalItem = new RewardsModel();

        private string ItemID = "";

        private string Title = "Rewards";

        private int CountDownTimerMinutes = 5;

        private int CountDownTimerSecond;

        private System.Timers.Timer _timer;

        private IMenu menu;

        private bool isPendingRewardConfirm = false;

        private LoadingOverlay loadingOverlay;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                TextViewUtils.SetMuseoSans500Typeface(txtRewardUsed, txtTitle, txtRewardPeriodTitle, txtRewardLocationTitle,
                    txtRewardConditionTitle, txtBtnRewardSave, btnRewardUse, btnRewardRedeemed, txtRewardRedeemedWord);
                TextViewUtils.SetMuseoSans300Typeface(txtRewardPeriodContent, txtRewardLocationContent, txtRewardConditionContent,
                    txtTimeCounter, txtRewardUsedDateTime);
                btnRewardSave.Clickable = true;
                isPendingRewardConfirm = false;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                presenter = new RewardDetailPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.REWARD_DETAIL_TITLE_KEY) && !string.IsNullOrEmpty(extras.GetString(Constants.REWARD_DETAIL_TITLE_KEY)))
                    {
                        SetToolBarTitle(extras.GetString(Constants.REWARD_DETAIL_TITLE_KEY));
                        Title = extras.GetString(Constants.REWARD_DETAIL_TITLE_KEY);
                    }

                    if (extras.ContainsKey(Constants.REWARD_DETAIL_ITEM_KEY) && !string.IsNullOrEmpty(extras.GetString(Constants.REWARD_DETAIL_ITEM_KEY)))
                    {
                        ItemID = extras.GetString(Constants.REWARD_DETAIL_ITEM_KEY);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            try
            {
                RunOnUiThread(() =>
                {
                    if (!isPendingRewardConfirm)
                    {
                        if (string.IsNullOrEmpty(ItemID))
                        {
                            this.Finish();
                        }
                        else
                        {
                            SetupImageParam();
                            this.presenter.GetActiveReward(ItemID);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            // Check if isPendingRewardClaim then update the flag isUsed
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.RewardDetailLayout;
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, Title);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.PromotionDetailMenu, menu);
            this.menu = menu;
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_share_promotion:
                    
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void SetRewardDetail(RewardsModel item)
        {
            try
            {
                LocalItem = item;
                txtTitle.Text = item.Title;
                txtRewardPeriodContent.TextFormatted = GetFormattedText(item.PeriodLabel);
                txtRewardLocationContent.TextFormatted = GetFormattedText(item.LocationLabel);
                txtRewardConditionContent.TextFormatted = GetFormattedText(item.TandCLabel);

                if (item.LocationLabel != null && (item.LocationLabel.Contains("http") || item.LocationLabel.Contains("www.")))
                {
                    SpannableString s = new SpannableString(txtRewardLocationContent.TextFormatted);

                    var urlSpans = s.GetSpans(0, s.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));

                    List<string> extractedUrls = this.presenter.ExtractUrls(item.LocationLabel);

                    if (urlSpans.Length == extractedUrls.Count)
                    {
                        for (int i = 0; i < urlSpans.Length; i++)
                        {
                            int startIndex = s.GetSpanStart(urlSpans[i]);
                            int endIndex = s.GetSpanEnd(urlSpans[i]);
                            s.RemoveSpan(urlSpans[i]);
                            ClickSpan clickableSpan = new ClickSpan()
                            {
                                textColor = Resources.GetColor(Resource.Color.powerBlue),
                                typeFace = Typeface.CreateFromAsset(this.Assets, "fonts/" + TextViewUtils.MuseoSans500)
                            };
                            string url = extractedUrls[i];
                            clickableSpan.Click += v =>
                            {
                                OnClickSpan(url);
                            };
                            s.SetSpan(clickableSpan, startIndex, endIndex, SpanTypes.ExclusiveExclusive);
                        }
                    }
                    txtRewardLocationContent.TextFormatted = s;
                    txtRewardLocationContent.MovementMethod = new LinkMovementMethod();
                }

                if (item.TandCLabel != null && (item.TandCLabel.Contains("http") || item.TandCLabel.Contains("www.")))
                {
                    SpannableString se = new SpannableString(txtRewardConditionContent.TextFormatted);

                    var seUrlSpans = se.GetSpans(0, se.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));

                    List<string> extractedUrls = this.presenter.ExtractUrls(item.TandCLabel);

                    if (seUrlSpans.Length == extractedUrls.Count)
                    {
                        for (int i = 0; i < seUrlSpans.Length; i++)
                        {
                            int startIndex = se.GetSpanStart(seUrlSpans[i]);
                            int endIndex = se.GetSpanEnd(seUrlSpans[i]);
                            se.RemoveSpan(seUrlSpans[i]);
                            ClickSpan clickableSpan = new ClickSpan()
                            {
                                textColor = Resources.GetColor(Resource.Color.powerBlue),
                                typeFace = Typeface.CreateFromAsset(this.Assets, "fonts/" + TextViewUtils.MuseoSans500)
                            };
                            string url = extractedUrls[i];
                            clickableSpan.Click += v =>
                            {
                                OnClickSpan(url);
                            };
                            se.SetSpan(clickableSpan, startIndex, endIndex, SpanTypes.ExclusiveExclusive);
                        }
                    }

                    txtRewardConditionContent.TextFormatted = se;
                    txtRewardConditionContent.MovementMethod = new LinkMovementMethod();
                }

                if (!item.IsUsed)
                {
                    btnUseSaveLayout.Visibility = ViewStates.Visible;
                    rewardRedeemedLayout.Visibility = ViewStates.Gone;
                    rewardCountDownLayout.Visibility = ViewStates.Gone;
                    if (item.IsSaved)
                    {
                        imgBtnRewardSave.SetImageResource(Resource.Drawable.ic_button_reward_save);
                        txtBtnRewardSave.Text = "Unsave";
                    }
                    else
                    {
                        imgBtnRewardSave.SetImageResource(Resource.Drawable.ic_button_reward_unsave);
                        txtBtnRewardSave.Text = "Save";
                    }
                }
                else
                {
                    btnUseSaveLayout.Visibility = ViewStates.Gone;
                    rewardRedeemedLayout.Visibility = ViewStates.Visible;
                    rewardCountDownLayout.Visibility = ViewStates.Gone;

                    string dateTime = "Reward used ";

                    try
                    {
                        DateTime dateTimeParse = DateTime.ParseExact(item.IsUsedDateTime, "MM/d/yyyy H:mm:ss tt",
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                        TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kuala_Lumpur");
                        DateTime dateTimeMalaysia = TimeZoneInfo.ConvertTimeFromUtc(dateTimeParse, tzi);
                        dateTime += dateTimeMalaysia.ToString("dd MMM yyyy, h:mm tt");
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                    dateTime += ".";

                    dateTime = "<i>" + dateTime + "</i>";

                    txtRewardUsedDateTime.TextFormatted = GetFormattedText(dateTime);

                    txtTitle.SetTextColor(Resources.GetColor(Resource.Color.charcoalGrey));
                    txtRewardPeriodTitle.SetTextColor(Resources.GetColor(Resource.Color.charcoalGrey));
                    txtRewardLocationTitle.SetTextColor(Resources.GetColor(Resource.Color.charcoalGrey));
                    txtRewardConditionTitle.SetTextColor(Resources.GetColor(Resource.Color.charcoalGrey));

                    imgRewardPeriod.SetImageResource(Resource.Drawable.ic_reward_time_used);
                    imgRewardLocation.SetImageResource(Resource.Drawable.ic_reward_locate_used);
                    imgRewardCondition.SetImageResource(Resource.Drawable.ic_reward_t_c_used);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetRewardImage(Bitmap imgSrc)
        {
            try
            {
                if (imgSrc == null)
                {
                    rewardImg.SetImageResource(Resource.Drawable.ic_image_reward_empty);
                }
                else if (imgSrc != null)
                {
                    LocalItem.ImageBitmap = imgSrc;
                    rewardImg.SetImageBitmap(imgSrc);
                }
                else
                {
                    this.presenter.FetchRewardImage(LocalItem);
                    return;
                }

                if (LocalItem.IsUsed)
                {
                    rewardUsedLayout.Visibility = ViewStates.Visible;
                }

                rewardMainShimmerImgLayout.Visibility = ViewStates.Gone;
                if (shimmerRewardImageLayout.IsShimmerStarted)
                {
                    shimmerRewardImageLayout.StopShimmer();
                }

                rewardMainImg.Visibility = ViewStates.Visible;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SetupImageParam()
        {
            try
            {
                LinearLayout.LayoutParams currentShimmerImg = rewardMainShimmerImgLayout.LayoutParameters as LinearLayout.LayoutParams;
                LinearLayout.LayoutParams currentMainImgLayout = rewardMainImg.LayoutParameters as LinearLayout.LayoutParams;
                RelativeLayout.LayoutParams currentRewardLayout = rewardImg.LayoutParameters as RelativeLayout.LayoutParams;
                int currentImgWidth = this.Resources.DisplayMetrics.WidthPixels;
                float currentImgRatio = 180f / 320f;
                int currentImgHeight = (int)(currentImgWidth * currentImgRatio);
                currentShimmerImg.Height = currentImgHeight;
                currentMainImgLayout.Height = currentImgHeight;
                currentRewardLayout.Height = currentImgHeight;

                if (shimmerRewardImageLayout.IsShimmerStarted)
                {
                    shimmerRewardImageLayout.StopShimmer();
                }
                var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                if (shimmerBuilder != null)
                {
                    shimmerRewardImageLayout.SetShimmer(shimmerBuilder?.Build());
                }
                shimmerRewardImageLayout.StartShimmer();
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

        public void OnClickSpan(string url)
        {
            if (!string.IsNullOrEmpty(url) && (url.Contains("http") || url.Contains("www.")))
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);

                    if (!url.Contains("http"))
                    {
                        url = "http://" + url;
                    }

                    Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
                    webIntent.PutExtra(Constants.IN_APP_LINK, url);
                    webIntent.PutExtra(Constants.IN_APP_TITLE, Title);
                    StartActivity(webIntent);
                }
            }
        }

        [OnClick(Resource.Id.btnRewardSave)]
        void OnRewardSave(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);

                if (!LocalItem.IsSaved)
                {
                    LocalItem.IsSaved = true;
                    this.presenter.UpdateRewardSave(LocalItem.ID, LocalItem.IsSaved);
                    imgBtnRewardSave.SetImageResource(Resource.Drawable.ic_button_reward_save);
                    txtBtnRewardSave.Text = "Unsave";
                }
                else
                {
                    LocalItem.IsSaved = false;
                    this.presenter.UpdateRewardSave(LocalItem.ID, LocalItem.IsSaved);
                    imgBtnRewardSave.SetImageResource(Resource.Drawable.ic_button_reward_unsave);
                    txtBtnRewardSave.Text = "Save";
                }

                this.SetIsClicked(false);
            }

        }

        [OnClick(Resource.Id.btnRewardUse)]
        void OnRewardUse(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER_TWO_BUTTON)
                    .SetTitle(!string.IsNullOrEmpty(LocalItem.RewardUseTitle) ? LocalItem.RewardUseTitle : "Please make sure you’re at the shop.")
                    .SetMessage(!string.IsNullOrEmpty(LocalItem.RewardUseDescription) ? LocalItem.RewardUseDescription : "This reward can only be used once, please confirm you’re at the shop / merchant.")
                    .SetHeaderImage(Resource.Drawable.img_tooltip_reward_confirm)
                    .SetCTALabel("Use Later")
                    .SetCTAaction(() => {
                        this.SetIsClicked(false);
                    })
                    .SetSecondaryCTAaction(() =>
                    {
                        this.SetIsClicked(false);
                        IMenuItem item = this.menu.FindItem(Resource.Id.action_share_promotion);
                        if (item != null)
                        {
                            item.SetVisible(false);
                        }
                        this.presenter.UpdateRewardUsed(LocalItem.ID);
                    })
                    .SetSecondaryCTALabel("Confirm")
                    .Build().Show();
            }
        }

        public void ShowProgressDialog()
        {
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

        public void HideProgressDialog()
        {
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

        public void OnCountDownReward()
        {
            try
            {
                LocalItem.IsUsed = true;
                if (!string.IsNullOrEmpty(LocalItem.RewardUseWithinTime))
                {
                    try
                    {
                        CountDownTimerMinutes = int.Parse(LocalItem.RewardUseWithinTime);
                    }
                    catch (Exception ne)
                    {
                        CountDownTimerMinutes = 5;
                        Utility.LoggingNonFatalError(ne);
                    }
                }
                else
                {
                    CountDownTimerMinutes = 5;
                }

                CountDownTimerSecond = CountDownTimerMinutes * 60;

                RunOnUiThread(() =>
                {
                    btnUseSaveLayout.Visibility = ViewStates.Gone;
                    rewardRedeemedLayout.Visibility = ViewStates.Gone;
                    rewardCountDownLayout.Visibility = ViewStates.Visible;
                    txtRewardRedeemedWord.Text = "Reward redeemed. Please show the merchant this screen before the timer runs out.";
                });

                isPendingRewardConfirm = true;

                OnUpdateCountDownView();

                _timer = new System.Timers.Timer();
                _timer.Interval = 1000;
                _timer.Elapsed += OnTimedEvent;

                _timer.Enabled = true;

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            CountDownTimerSecond--;

            OnUpdateCountDownView();

            if (CountDownTimerSecond == 0)
            {
                _timer.Stop();
                _timer.Enabled = false;
                RunOnUiThread(() =>
                {
                    btnUseSaveLayout.Visibility = ViewStates.Gone;
                    rewardRedeemedLayout.Visibility = ViewStates.Visible;
                    rewardCountDownLayout.Visibility = ViewStates.Gone;
                    IMenuItem item = this.menu.FindItem(Resource.Id.action_share_promotion);
                    if (item != null)
                    {
                        item.SetVisible(true);
                    }
                    isPendingRewardConfirm = false;
                    this.presenter.GetActiveReward(ItemID);
                });
            }
        }

        private void OnUpdateCountDownView()
        {
            RunOnUiThread(() =>
            {
                TimeSpan time = TimeSpan.FromSeconds(CountDownTimerSecond);

                //here backslash is must to tell that colon is
                //not the part of format, it just a character that we want in output
                string str = time.ToString(@"mm\:ss");

                txtTimeCounter.Text = str;
            });
        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException()
        {
            try
            {
                if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
                {
                    mApiExcecptionSnackBar.Dismiss();
                }

                mApiExcecptionSnackBar = Snackbar.Make(rootView, "Something went wrong! Please try again later", Snackbar.LengthIndefinite)
                .SetAction("Close", delegate
                {

                    mApiExcecptionSnackBar.Dismiss();
                }
                );
                mApiExcecptionSnackBar.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}