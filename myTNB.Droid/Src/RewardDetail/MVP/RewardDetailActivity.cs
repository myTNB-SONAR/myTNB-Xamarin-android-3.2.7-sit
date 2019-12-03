using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
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
using System.Runtime;

namespace myTNB_Android.Src.RewardDetail.MVP
{
    [Activity(Label = "Rewards"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.RewardDetail")]
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

        RewardDetailContract.IRewardDetailPresenter presenter;

        private RewardsModel LocalItem = new RewardsModel();

        private string ItemID = "";

        private string Title = "Rewards";

        private ClickSpan clickableSpan;

        private ClickSpan seClickableSpan;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                TextViewUtils.SetMuseoSans500Typeface(txtRewardUsed, txtTitle, txtRewardPeriodTitle, txtRewardLocationTitle,
                    txtRewardConditionTitle, txtBtnRewardSave, btnRewardUse);
                TextViewUtils.SetMuseoSans300Typeface(txtRewardPeriodContent, txtRewardLocationContent, txtRewardConditionContent);
                btnRewardSave.Clickable = true;
                clickableSpan = new ClickSpan()
                {
                    textColor = Resources.GetColor(Resource.Color.powerBlue),
                    typeFace = Typeface.CreateFromAsset(this.Assets, "fonts/" + TextViewUtils.MuseoSans500)
                };

                seClickableSpan = new ClickSpan()
                {
                    textColor = Resources.GetColor(Resource.Color.powerBlue),
                    typeFace = Typeface.CreateFromAsset(this.Assets, "fonts/" + TextViewUtils.MuseoSans500)
                };
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
                    if (string.IsNullOrEmpty(ItemID))
                    {
                        this.Finish();
                    }
                    else
                    {
                        SetupImageParam();
                        this.presenter.GetActiveReward(ItemID);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
                    clickableSpan.Click += v =>
                    {
                        OnClickSpan(item.LocationLabel);
                    };
                    var urlSpans = s.GetSpans(0, s.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
                    int startFAQLink = s.GetSpanStart(urlSpans[0]);
                    int endFAQLink = s.GetSpanEnd(urlSpans[0]);
                    s.RemoveSpan(urlSpans[0]);
                    s.SetSpan(clickableSpan, startFAQLink, endFAQLink, SpanTypes.ExclusiveExclusive);
                    txtRewardLocationContent.TextFormatted = s;
                    txtRewardLocationContent.MovementMethod = new LinkMovementMethod();
                }

                if (item.TandCLabel != null && (item.TandCLabel.Contains("http") || item.TandCLabel.Contains("www.")))
                {
                    SpannableString se = new SpannableString(txtRewardConditionContent.TextFormatted);
                    seClickableSpan.Click += v =>
                    {
                        OnClickSpan(item.TandCLabel);
                    };
                    var seUrlSpans = se.GetSpans(0, se.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
                    int seStartFAQLink = se.GetSpanStart(seUrlSpans[0]);
                    int seEndFAQLink = se.GetSpanEnd(seUrlSpans[0]);
                    se.RemoveSpan(seUrlSpans[0]);
                    se.SetSpan(seClickableSpan, seStartFAQLink, seEndFAQLink, SpanTypes.ExclusiveExclusive);
                    txtRewardConditionContent.TextFormatted = se;
                    txtRewardConditionContent.MovementMethod = new LinkMovementMethod();
                }

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
                    rewardImg.SetImageResource(0);
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

        public void OnClickSpan(string textMessage)
        {
            if (textMessage != null && (textMessage.Contains("http") || textMessage.Contains("www.")))
            {
                //Launch webview
                int startIndex = textMessage.LastIndexOf("=") + 2;
                int lastIndex = textMessage.LastIndexOf("\"");
                int lengthOfId = (lastIndex - startIndex);
                if (lengthOfId < textMessage.Length)
                {
                    string url = textMessage.Substring(startIndex, lengthOfId);
                    if (!string.IsNullOrEmpty(url))
                    {
                        if (!url.Contains("http"))
                        {
                            url = "http://" + url;
                        }
                        Intent intent = new Intent(Intent.ActionView);
                        intent.SetData(Android.Net.Uri.Parse(url));
                        StartActivity(intent);
                    }
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
    }
}