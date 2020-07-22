using System;
using AFollestad.MaterialDialogs;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Text;
using myTNB_Android.Src.Utils;
using Android.Text.Style;
using Android.Text.Method;
using Android.Support.V7.Widget;
using Android.Graphics;
using Android.Support.V4.Content;
using System.Collections.Generic;
using Java.Util.Regex;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.WhatsNewDetail.MVP;
using System.Globalization;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB_Android.Src.Base.Models;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Utils
{
    public class MyTNBAppToolTipBuilder
    {
        public enum ToolTipType
        {
            IMAGE_HEADER,
            NORMAL_WITH_HEADER,
            NORMAL_WITH_HEADER_TWO_BUTTON,
            LISTVIEW_WITH_INDICATOR_AND_HEADER,
            IMAGE_HEADER_TWO_BUTTON,
            NORMAL,
            NORMAL_STRETCHABLE,
        }

        private ToolTipType toolTipType;
        private int imageResource;
        private string title;
        private string message;
        private string ctaLabel;
        private string secondaryCTALabel;
        private RecyclerView.Adapter adapter;
        private Action ctaAction;
        private Action secondaryCTAAction;
        private MaterialDialog dialog;
        private ClickSpan clickableSpan;
        private Color mClickSpanColor;
        private Typeface mTypeface;
        private Android.App.Activity mContext;
        private GravityFlags mGravityFlag;
        private Bitmap imageResourceBitmap;
        private RewardServiceImpl mApi;

        public static List<string> RedirectTypeList = new List<string> {
            "inAppBrowser=",
            "externalBrowser=",
            "tel=",
            "whatsnew=",
            "faq=",
            "reward=",
            "http",
            "tel:",
            "whatsnewid=",
            "faqid=",
            "rewardid="
        };

        private MyTNBAppToolTipBuilder(Android.App.Activity context)
        {
            this.mContext = context;
            this.mApi = new RewardServiceImpl();
        }

        public static MyTNBAppToolTipBuilder Create(Android.App.Activity context, ToolTipType mToolTipType)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = new MyTNBAppToolTipBuilder(context);
            tooltipBuilder.toolTipType = mToolTipType;
            tooltipBuilder.mGravityFlag = GravityFlags.Left;
            int layoutResource = 0;
            if (mToolTipType == ToolTipType.IMAGE_HEADER)
            {
                layoutResource = Resource.Layout.CustomDialogWithImageHeader;
            }else if (mToolTipType == ToolTipType.NORMAL_WITH_HEADER)
            {
                layoutResource = Resource.Layout.CustomToolTipWithHeaderLayout;
            }else if (mToolTipType == ToolTipType.LISTVIEW_WITH_INDICATOR_AND_HEADER)
            {
                layoutResource = Resource.Layout.CustomDialogWithListViewLayout;
            }else if (mToolTipType == ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
            {
                layoutResource = Resource.Layout.CustomToolTipWithHeaderTwoButtonLayout;
            }
            else if (mToolTipType == ToolTipType.IMAGE_HEADER_TWO_BUTTON)
            {
                layoutResource = Resource.Layout.CustomDialogWithImageHeaderTwoButton;
            }
            else if (mToolTipType == ToolTipType.NORMAL)
            {
                layoutResource = Resource.Layout.CustomToolTipWithHeaderLayout;
            }
            else if (mToolTipType == ToolTipType.NORMAL_STRETCHABLE)
            {
                layoutResource = Resource.Layout.WhatIsThisDialogView;
            }
            tooltipBuilder.dialog = new MaterialDialog.Builder(context)
                .CustomView(layoutResource, false)
                .Cancelable(false)
                .CanceledOnTouchOutside(false)
                .Build();

            View dialogView = tooltipBuilder.dialog.Window.DecorView;
            if (mToolTipType != ToolTipType.NORMAL_STRETCHABLE)
            {
                dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);
                WindowManagerLayoutParams wlp = tooltipBuilder.dialog.Window.Attributes;
                wlp.Gravity = GravityFlags.Center;
                wlp.Width = ViewGroup.LayoutParams.MatchParent;
                wlp.Height = ViewGroup.LayoutParams.WrapContent;
                tooltipBuilder.dialog.Window.Attributes = wlp;
            }

            return tooltipBuilder;
        }

        public MyTNBAppToolTipBuilder SetHeaderImage(int imageResource)
        {
            this.imageResource = imageResource;
            return this;
        }

        public MyTNBAppToolTipBuilder SetTitle(string title)
        {
            this.title = title;
            return this;
        }

        public MyTNBAppToolTipBuilder SetMessage(string message, Color? color = null, Typeface? typeface = null)
        {
            this.message = message;

            this.mClickSpanColor = color ?? new Android.Graphics.Color(ContextCompat.GetColor(mContext, Resource.Color.powerBlue));
            this.mTypeface = typeface ?? Typeface.CreateFromAsset(mContext.Assets, "fonts/" + TextViewUtils.MuseoSans500);
            return this;
        }

        public MyTNBAppToolTipBuilder SetCTALabel(string ctaLabel)
        {
            this.ctaLabel = ctaLabel;
            return this;
        }

        public MyTNBAppToolTipBuilder SetSecondaryCTALabel(string secondaryCTALabel)
        {
            this.secondaryCTALabel = secondaryCTALabel;
            return this;
        }

        public MyTNBAppToolTipBuilder SetAdapter(RecyclerView.Adapter adapter)
        {
            this.adapter = adapter;
            return this;
        }

        public MyTNBAppToolTipBuilder SetCTAaction(Action ctaFunc)
        {
            this.ctaAction = ctaFunc;
            return this;
        }

        public MyTNBAppToolTipBuilder SetSecondaryCTAaction(Action ctaFunc)
        {
            this.secondaryCTAAction = ctaFunc;
            return this;
        }

        public MyTNBAppToolTipBuilder SetContentGravity(GravityFlags gravityFlags)
        {
            this.mGravityFlag = gravityFlags;
            return this;
        }

        public void DismissDialog()
        {
            this.dialog.Dismiss();
        }

          public MyTNBAppToolTipBuilder SetHeaderImageBitmap(Bitmap imageResource)
        {
            this.imageResourceBitmap = imageResource;
            return this;
        }

        public MyTNBAppToolTipBuilder Build()
        {
            if (this.toolTipType == ToolTipType.IMAGE_HEADER)
            {
                ImageView tooltipImageHeader = this.dialog.FindViewById<ImageView>(Resource.Id.imgToolTipHeader);
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipCTA);

                tooltipCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    if (this.ctaAction != null)
                    {
                        this.ctaAction();
                    }
                };

                tooltipTitle.Text = this.title;
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                }

                tooltipMessage = ProcessClickableSpan(tooltipMessage, this.message);
               // tooltipImageHeader.SetImageResource(this.imageResource);
                 if (this.imageResourceBitmap != null)
                {
                    tooltipImageHeader.SetImageBitmap(this.imageResourceBitmap);
                }
                else
                {
                    tooltipImageHeader.SetImageResource(this.imageResource);
                }
                tooltipCTA.Text = this.ctaLabel;
            }
            else if(this.toolTipType == ToolTipType.NORMAL_WITH_HEADER)
            {
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipCTA);

                tooltipTitle.Gravity = this.mGravityFlag;
                tooltipMessage.Gravity = this.mGravityFlag;

                tooltipCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                tooltipTitle.Text = this.title;
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                }

                tooltipMessage = ProcessClickableSpan(tooltipMessage, this.message);

                tooltipCTA.Text = this.ctaLabel;
            }
            else if (this.toolTipType == ToolTipType.LISTVIEW_WITH_INDICATOR_AND_HEADER)
            {
                RecyclerView recyclerView = this.dialog.FindViewById<RecyclerView>(Resource.Id.dialogRecyclerView);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);
                tooltipCTA.Text = this.ctaLabel;
                LinearLayout indicatorContainer = this.dialog.FindViewById<LinearLayout>(Resource.Id.dialoagListViewIndicatorContainer);

                LinearSnapHelper snapTooltipHelper = new LinearSnapHelper();
                LinearLayoutManager layoutManager = new LinearLayoutManager(this.mContext, LinearLayoutManager.Horizontal, false);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(this.adapter);
                snapTooltipHelper.AttachToRecyclerView(recyclerView);
                recyclerView.AddOnScrollListener(new ToolTipRecyclerViewOnScrollListener(layoutManager, indicatorContainer));

                try
                {
                    for (int i = 0; i < this.adapter.ItemCount; i++)
                    {
                        ImageView image = new ImageView(this.mContext);
                        image.Id = i;
                        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                        layoutParams.RightMargin = 8;
                        layoutParams.LeftMargin = 8;
                        image.LayoutParameters = layoutParams;
                        if (i == 0)
                        {
                            image.SetImageResource(Resource.Drawable.onboarding_circle_active);
                        }
                        else
                        {
                            image.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                        }
                        indicatorContainer.AddView(image, i);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                tooltipCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };
            }
            else if (this.toolTipType == ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
            {
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnPrimary);
                TextView tooltipSecondaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnSecondary);

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipPrimaryCTA, tooltipSecondaryCTA);

                tooltipTitle.Gravity = this.mGravityFlag;
                tooltipMessage.Gravity = this.mGravityFlag;

                tooltipPrimaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                tooltipSecondaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    if (secondaryCTAAction != null)
                    {
                        this.secondaryCTAAction();
                    }
                };

                tooltipTitle.Text = this.title;
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                }
                tooltipMessage = ProcessClickableSpan(tooltipMessage, this.message);
                tooltipPrimaryCTA.Text = this.ctaLabel;
                tooltipSecondaryCTA.Text = this.secondaryCTALabel;
            }
            else if (this.toolTipType == ToolTipType.IMAGE_HEADER_TWO_BUTTON)
            {
                ImageView tooltipImageHeader = this.dialog.FindViewById<ImageView>(Resource.Id.imgToolTipHeader);
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnPrimary);
                TextView tooltipSecondaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnSecondary);

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipPrimaryCTA, tooltipSecondaryCTA);

                if (this.imageResourceBitmap != null)
                {
                    float currentImgWidth = DPUtils.ConvertDPToPx(284f);
                    float calImgRatio = currentImgWidth / this.imageResourceBitmap.Width;
                    int currentImgHeight = (int)(this.imageResourceBitmap.Height * calImgRatio);

                    tooltipImageHeader.SetImageBitmap(this.imageResourceBitmap);
                    tooltipImageHeader.LayoutParameters.Height = currentImgHeight;
                    tooltipImageHeader.RequestLayout();
                }
                else
                {
                    tooltipImageHeader.SetImageResource(this.imageResource);
                }

                tooltipPrimaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                tooltipSecondaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    if (secondaryCTAAction != null)
                    {
                        this.secondaryCTAAction();
                    }
                };

                tooltipTitle.Text = this.title;
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                }
                tooltipMessage = ProcessClickableSpan(tooltipMessage, this.message);

                tooltipPrimaryCTA.Text = this.ctaLabel;
                tooltipSecondaryCTA.Text = this.secondaryCTALabel;
            }
            else if (this.toolTipType == ToolTipType.NORMAL)
            {
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipCTA);

                tooltipCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                tooltipTitle.Visibility = ViewStates.Gone;
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                }
                tooltipMessage = ProcessClickableSpan(tooltipMessage, this.message);

                tooltipCTA.Text = this.ctaLabel;

            }
            else if (this.toolTipType == ToolTipType.NORMAL_STRETCHABLE)
            {
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtDialogMessage);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnLabel);

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipCTA);

                tooltipCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                }
                tooltipMessage = ProcessClickableSpan(tooltipMessage, this.message);

                tooltipCTA.Text = this.ctaLabel;

            }
            return this;
        }

        public void Show()
        {
            this.dialog.Show();
        }

        private TextView ProcessClickableSpan(TextView mTextView, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                int whileCount = 0;
                bool isContained = false;
                for (int i = 0; i < RedirectTypeList.Count; i++)
                {
                    if (message.Contains(RedirectTypeList[i]))
                    {
                        whileCount = i;
                        isContained = true;
                        break;
                    }
                }

                if (isContained)
                {
                    SpannableString s = new SpannableString(mTextView.TextFormatted);
                    this.clickableSpan = new ClickSpan()
                    {
                        textColor = this.mClickSpanColor,
                        typeFace = this.mTypeface
                    };

                    this.clickableSpan.Click += v =>
                    {
                        if (RedirectTypeList[whileCount] == RedirectTypeList[0]
                            || RedirectTypeList[whileCount] == RedirectTypeList[1]
                            || RedirectTypeList[whileCount] == RedirectTypeList[6])
                        {
                            List<string> extractedUrls = ExtractUrls(message);
                            if (extractedUrls.Count > 0)
                            {
                                if (!extractedUrls[0].Contains("http"))
                                {
                                    extractedUrls[0] = "http://" + extractedUrls[0];
                                }

                                if (RedirectTypeList[whileCount] == RedirectTypeList[0] || RedirectTypeList[whileCount] == RedirectTypeList[6])
                                {
                                    if (extractedUrls[0].Contains(".pdf") && !extractedUrls[0].Contains("docs.google"))
                                    {
                                        Intent webIntent = new Intent(this.mContext, typeof(BasePDFViewerActivity));
                                        webIntent.PutExtra(Constants.IN_APP_LINK, extractedUrls[0]);
                                        webIntent.PutExtra(Constants.IN_APP_TITLE, "");
                                        this.mContext.StartActivity(webIntent);
                                    }
                                    else
                                    {
                                        Intent webIntent = new Intent(this.mContext, typeof(BaseWebviewActivity));
                                        webIntent.PutExtra(Constants.IN_APP_LINK, extractedUrls[0]);
                                        webIntent.PutExtra(Constants.IN_APP_TITLE, "");
                                        this.mContext.StartActivity(webIntent);
                                    }
                                }
                                else
                                {
                                    Intent intent = new Intent(Intent.ActionView);
                                    intent.SetData(Android.Net.Uri.Parse(extractedUrls[0]));
                                    this.mContext.StartActivity(intent);
                                }
                            }
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[2])
                        {
                            int startIndex = message.LastIndexOf("=") + 1;
                            int lastIndex = message.LastIndexOf("\">") - 1;
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < message.Length)
                            {
                                string phonenum = message.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(phonenum))
                                {
                                    if (!phonenum.Contains("tel:"))
                                    {
                                        phonenum = "tel:" + phonenum;
                                    }

                                    var call = Android.Net.Uri.Parse(phonenum);
                                    var callIntent = new Intent(Intent.ActionView, call);
                                    this.mContext.StartActivity(callIntent);
                                }
                            }
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[3]
                                    || RedirectTypeList[whileCount] == RedirectTypeList[8])
                        {
                            int startIndex = message.LastIndexOf("=") + 1;
                            int lastIndex = message.LastIndexOf("}");
                            if (lastIndex < 0)
                            {
                                lastIndex = message.LastIndexOf("\">") - 1;
                            }
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < message.Length)
                            {
                                string whatsnewid = message.Substring(startIndex, lengthOfId);
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
                                            UpdateWhatsNewRead(item.ID, true);
                                        }

                                        Intent activity = new Intent(this.mContext, typeof(WhatsNewDetailActivity));
                                        activity.PutExtra(Constants.WHATS_NEW_DETAIL_ITEM_KEY, whatsnewid);
                                        activity.PutExtra(Constants.WHATS_NEW_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "promotion"));
                                        this.mContext.StartActivity(activity);
                                    }
                                }
                            }
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[4]
                                    || RedirectTypeList[whileCount] == RedirectTypeList[9])
                        {
                            int startIndex = message.LastIndexOf("=") + 1;
                            int lastIndex = message.LastIndexOf("}");
                            if (lastIndex < 0)
                            {
                                lastIndex = message.LastIndexOf("\">") - 1;
                            }
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < message.Length)
                            {
                                string faqid = message.Substring(startIndex, lengthOfId);
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

                                    Intent faqIntent = new Intent(this.mContext, typeof(FAQListActivity));
                                    faqIntent.PutExtra(Constants.FAQ_ID_PARAM, faqid);
                                    this.mContext.StartActivity(faqIntent);
                                }
                            }
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[5]
                                    || RedirectTypeList[whileCount] == RedirectTypeList[10])
                        {
                            int startIndex = message.LastIndexOf("=") + 1;
                            int lastIndex = message.LastIndexOf("}");
                            if (lastIndex < 0)
                            {
                                lastIndex = message.LastIndexOf("\">") - 1;
                            }
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < message.Length)
                            {
                                string rewardid = message.Substring(startIndex, lengthOfId);
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
                                            UpdateRewardRead(item.ID, true);
                                        }

                                        Intent activity = new Intent(this.mContext, typeof(RewardDetailActivity));
                                        activity.PutExtra(Constants.REWARD_DETAIL_ITEM_KEY, rewardid);
                                        activity.PutExtra(Constants.REWARD_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "rewards"));
                                        this.mContext.StartActivity(activity);
                                    }
                                }
                            }
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[7])
                        {
                            int startIndex = message.LastIndexOf("\"tel") + 1;
                            int lastIndex = message.LastIndexOf("\">") - 1;
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < message.Length)
                            {
                                string phonenum = message.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(phonenum))
                                {
                                    if (!phonenum.Contains("tel:"))
                                    {
                                        phonenum = "tel:" + phonenum;
                                    }

                                    var call = Android.Net.Uri.Parse(phonenum);
                                    var callIntent = new Intent(Intent.ActionView, call);
                                    this.mContext.StartActivity(callIntent);
                                }
                            }
                        }
                    };

                    var urlSpans = s.GetSpans(0, s.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
                    int startLink = s.GetSpanStart(urlSpans[0]);
                    int endLink = s.GetSpanEnd(urlSpans[0]);
                    s.RemoveSpan(urlSpans[0]);
                    s.SetSpan(clickableSpan, startLink, endLink, SpanTypes.ExclusiveExclusive);
                    mTextView.TextFormatted = s;
                    mTextView.MovementMethod = new LinkMovementMethod();
                }
            }

            return mTextView;
        }

        private List<string> ExtractUrls(string text)
        {
            List<string> containedUrls = new List<string>();
            string urlRegex = "\\(?\\b(https://|http://|www[.])[-A-Za-z0-9+&amp;@#/%?=~_()|!:,.;]*[-A-Za-z0-9+&amp;@#/%=~_()|]";
            Java.Util.Regex.Pattern pattern = Java.Util.Regex.Pattern.Compile(urlRegex);
            Matcher urlMatcher = pattern.Matcher(text);

            try
            {
                while (urlMatcher.Find())
                {
                    string urlStr = urlMatcher.Group();
                    if (urlStr.StartsWith("(") && urlStr.EndsWith(")"))
                    {
                        urlStr = urlStr.Substring(1, urlStr.Length - 1);
                    }

                    if (!containedUrls.Contains(urlStr))
                    {
                        containedUrls.Add(urlStr);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return containedUrls;
        }

        private void UpdateWhatsNewRead(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            WhatsNewEntity wtManager = new WhatsNewEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateReadItem(itemID, flag, formattedDate);
        }

        private void UpdateRewardRead(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            RewardsEntity wtManager = new RewardsEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateReadItem(itemID, flag, formattedDate);

            _ = OnUpdateReward(itemID);
        }

        private async Task OnUpdateReward(string itemID)
        {
            try
            {
                // Update api calling
                RewardsEntity wtManager = new RewardsEntity();
                RewardsEntity currentItem = wtManager.GetItem(itemID);

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = UserEntity.GetActive().DeviceId,
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                string rewardId = currentItem.ID;
                rewardId = rewardId.Replace("{", "");
                rewardId = rewardId.Replace("}", "");

                AddUpdateRewardModel currentReward = new AddUpdateRewardModel()
                {
                    Email = UserEntity.GetActive().Email,
                    RewardId = rewardId,
                    Read = currentItem.Read,
                    ReadDate = !string.IsNullOrEmpty(currentItem.ReadDateTime) ? currentItem.ReadDateTime + " +00:00" : "",
                    Favourite = currentItem.IsSaved,
                    FavUpdatedDate = !string.IsNullOrEmpty(currentItem.IsSavedDateTime) ? currentItem.IsSavedDateTime + " +00:00" : "",
                    Redeemed = currentItem.IsUsed,
                    RedeemedDate = !string.IsNullOrEmpty(currentItem.IsUsedDateTime) ? currentItem.IsUsedDateTime + " +00:00" : ""
                };

                AddUpdateRewardRequest request = new AddUpdateRewardRequest()
                {
                    usrInf = currentUsrInf,
                    reward = currentReward
                };

                AddUpdateRewardResponse response = await this.mApi.AddUpdateReward(request, new System.Threading.CancellationTokenSource().Token);

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

        public class ToolTipRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
        {
            private LinearLayoutManager mLinearLayoutManager;
            private LinearLayout mIndicatorContainer;
            public ToolTipRecyclerViewOnScrollListener(LinearLayoutManager layoutManager, LinearLayout indicatorContainer)
            {
                mLinearLayoutManager = layoutManager;
                mIndicatorContainer = indicatorContainer;
            }
            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                base.OnScrolled(recyclerView, dx, dy);
                int currentPosition = mLinearLayoutManager.FindFirstCompletelyVisibleItemPosition();
                if (currentPosition >= 0)
                {
                    ImageView imageView;
                    for (int i = 0; i < mIndicatorContainer.ChildCount; i++)
                    {
                        imageView = (ImageView)mIndicatorContainer.GetChildAt(i);
                        if (i == currentPosition)
                        {
                            imageView.SetImageResource(Resource.Drawable.circle_active);
                        }
                        else
                        {
                            imageView.SetImageResource(Resource.Drawable.circle);
                        }
                    }
                }
            }
        }
    }
}
