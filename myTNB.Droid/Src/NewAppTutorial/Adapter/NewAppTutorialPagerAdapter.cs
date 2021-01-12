using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.MVP;
using AndroidX.ViewPager.Widget;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Billing.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP;

namespace myTNB_Android.Src.NewAppTutorial.Adapter
{
    public class NewAppTutorialPagerAdapter : PagerAdapter
    {
        private Activity mContext;
        private List<NewAppModel> list = new List<NewAppModel>();
        private AndroidX.Fragment.App.DialogFragment mDialog;
        private AndroidX.Fragment.App.Fragment mFragment;
        private ISharedPreferences mPref;
       

        public NewAppTutorialPagerAdapter(Activity ctx, AndroidX.Fragment.App.Fragment fragment, ISharedPreferences pref, AndroidX.Fragment.App.DialogFragment dialog, List<NewAppModel> items)
        {
            this.mContext = ctx;
            this.list = items;
            this.mDialog = dialog;
            this.mFragment = fragment;
            this.mPref = pref;
           
        }

        public NewAppTutorialPagerAdapter()
        {

        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            ViewGroup rootView = (ViewGroup)LayoutInflater.From(mContext).Inflate(Resource.Layout.NewAppTutorialItemLayout, container, false);
            LinearLayout mainLayout = rootView.FindViewById(Resource.Id.mainLayout) as LinearLayout;
            RelativeLayout topLayout = rootView.FindViewById(Resource.Id.topLayout) as RelativeLayout;
            LinearLayout middleLayout = rootView.FindViewById(Resource.Id.middleLayout) as LinearLayout;
            LinearLayout highlightedLeftLayout = rootView.FindViewById(Resource.Id.highlightedLeftLayout) as LinearLayout;
            LinearLayout highlightedLayout = rootView.FindViewById(Resource.Id.highlightedLayout) as LinearLayout;
            LinearLayout highlightedRightLayout = rootView.FindViewById(Resource.Id.highlightedRightLayout) as LinearLayout;
            RelativeLayout bottomLayout = rootView.FindViewById(Resource.Id.bottomLayout) as RelativeLayout;
            LinearLayout mainBottomLayout = rootView.FindViewById(Resource.Id.mainBottomLayout) as LinearLayout;
            LinearLayout innerTopLayout = rootView.FindViewById(Resource.Id.innerTopLayout) as LinearLayout;
            LinearLayout innerUpperBottomLayout = rootView.FindViewById(Resource.Id.innerUpperBottomLayout) as LinearLayout;
            LinearLayout leftInnerUpperBottomLineLayout = rootView.FindViewById(Resource.Id.leftInnerUpperBottomLineLayout) as LinearLayout;
            LinearLayout rightInnerUpperBottomLineLayout = rootView.FindViewById(Resource.Id.rightInnerUpperBottomLineLayout) as LinearLayout;
            LinearLayout leftInnerTopLineLayout = rootView.FindViewById(Resource.Id.leftInnerTopLineLayout) as LinearLayout;
            LinearLayout rightInnerTopLineLayout = rootView.FindViewById(Resource.Id.rightInnerTopLineLayout) as LinearLayout;
            LinearLayout innerTxtBtnBottomLayout = rootView.FindViewById(Resource.Id.innerTxtBtnBottomLayout) as LinearLayout;
            LinearLayout innerMiddleTopLayout = rootView.FindViewById(Resource.Id.innerMiddleTopLayout) as LinearLayout;


            TextView txtBottomTitle = rootView.FindViewById(Resource.Id.txtBottomTitle) as TextView;
            TextView txtBottomContent = rootView.FindViewById(Resource.Id.txtBottomContent) as TextView;
            TextView txtTopTitle = rootView.FindViewById(Resource.Id.txtTopTitle) as TextView;
            TextView txtTopContent = rootView.FindViewById(Resource.Id.txtTopContent) as TextView;

            Button btnBottomGotIt = rootView.FindViewById(Resource.Id.btnBottomGotIt) as Button;
            Button btnTopGotIt = rootView.FindViewById(Resource.Id.btnTopGotIt) as Button;

            btnBottomGotIt.Click += BtnGotIt_Click;
            btnTopGotIt.Click += BtnGotIt_Click;

            TextViewUtils.SetMuseoSans300Typeface(txtBottomContent, txtTopContent);
            TextViewUtils.SetMuseoSans500Typeface(txtBottomTitle, txtTopTitle, btnBottomGotIt, btnTopGotIt);

            txtTopTitle.TextSize = TextViewUtils.GetFontSize(14f);
            txtTopContent.TextSize = TextViewUtils.GetFontSize(14f);
            txtBottomTitle.TextSize = TextViewUtils.GetFontSize(14f);
            txtBottomContent.TextSize = TextViewUtils.GetFontSize(14f);
            btnTopGotIt.TextSize = TextViewUtils.GetFontSize(16f);
            btnBottomGotIt.TextSize = TextViewUtils.GetFontSize(16f);

            btnTopGotIt.Text = Utility.GetLocalizedCommonLabel("gotIt");
            btnBottomGotIt.Text = Utility.GetLocalizedCommonLabel("gotIt");

            NewAppModel model = list[position];

            if (model.ContentShowPosition == ContentType.BottomLeft || model.ContentShowPosition == ContentType.BottomRight)
            {
                mainBottomLayout.Visibility = ViewStates.Visible;
                innerTopLayout.Visibility = ViewStates.Gone;
                LinearLayout.LayoutParams txtBottomContentParam = txtBottomContent.LayoutParameters as LinearLayout.LayoutParams;
                LinearLayout.LayoutParams btnBottomGotItParam = btnBottomGotIt.LayoutParameters as LinearLayout.LayoutParams;
                LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;

                if (model.ContentShowPosition == ContentType.BottomLeft)
                {
                    leftInnerUpperBottomLineLayout.Visibility = ViewStates.Visible;
                    rightInnerUpperBottomLineLayout.Visibility = ViewStates.Gone;
                    txtBottomContentParam.Gravity = GravityFlags.Left;
                    btnBottomGotItParam.Gravity = GravityFlags.Left;
                    innerUpperBottomLayoutParam.Gravity = GravityFlags.Left;
                    innerTxtBtnBottomLayoutParam.Gravity = GravityFlags.Left;
                }
                else
                {
                    leftInnerUpperBottomLineLayout.Visibility = ViewStates.Gone;
                    rightInnerUpperBottomLineLayout.Visibility = ViewStates.Visible;
                    txtBottomContentParam.Gravity = GravityFlags.Right;
                    btnBottomGotItParam.Gravity = GravityFlags.Right;
                    innerUpperBottomLayoutParam.Gravity = GravityFlags.Right;
                    innerTxtBtnBottomLayoutParam.Gravity = GravityFlags.Right;
                }

                txtBottomContentParam.Width = (int)((float)this.mContext.Resources.DisplayMetrics.WidthPixels * 0.705);

                txtBottomContent.RequestLayout();
                btnBottomGotIt.RequestLayout();
                innerUpperBottomLayout.RequestLayout();
                innerTxtBtnBottomLayout.RequestLayout();

                if (model.ContentShowPosition == ContentType.BottomLeft)
                {
                    txtBottomContent.Gravity = GravityFlags.Left;
                    innerUpperBottomLayout.SetGravity(GravityFlags.Left);
                    innerTxtBtnBottomLayout.SetGravity(GravityFlags.Left);
                }
                else
                {
                    txtBottomContent.Gravity = GravityFlags.Right;
                    innerUpperBottomLayout.SetGravity(GravityFlags.Right);
                    innerTxtBtnBottomLayout.SetGravity(GravityFlags.Right);
                }
                innerUpperBottomLayout.RequestLayout();
                innerTxtBtnBottomLayout.RequestLayout();
                txtBottomContent.RequestLayout();

                txtBottomTitle.Text = model.ContentTitle;

                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtBottomContent.TextFormatted = Html.FromHtml(model.ContentMessage, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtBottomContent.TextFormatted = Html.FromHtml(model.ContentMessage);
                }

                if (this.mFragment != null && this.mFragment is ItemisedBillingMenuFragment && position == 0)
                {
                    ImageSpan imageSpan = new ImageSpan(this.mContext, Resource.Drawable.ic_display_dropdown, SpanAlign.Baseline);
                    SpannableString firstPageString = new SpannableString(txtBottomContent.TextFormatted);
                    string searchText = model.ContentMessage;

                    int start = -1;
                    int end = -1;
                    if (searchText.Contains("“ #"))
                    {
                        start = searchText.LastIndexOf("“ #") + 1;
                    }
                    else if (searchText.Contains("\" #"))
                    {
                        start = searchText.LastIndexOf("\" #") + 1;
                    }
                    else if (searchText.Contains("\"#"))
                    {
                        start = searchText.LastIndexOf("\"#") + 1;
                    }
                    else if (searchText.Contains("“#"))
                    {
                        start = searchText.LastIndexOf("“#") + 1;
                    }

                    if (searchText.Contains("# ”"))
                    {
                        end = searchText.LastIndexOf("# ”") + 2;
                    }
                    else if (searchText.Contains("# \""))
                    {
                        end = searchText.LastIndexOf("# \"") + 2;
                    }
                    else if (searchText.Contains("#\""))
                    {
                        end = searchText.LastIndexOf("#\"") + 1;
                    }
                    else if (searchText.Contains("#”"))
                    {
                        end = searchText.LastIndexOf("#”") + 1;
                    }

                    if (start != -1 && end != -1)
                    {
                        firstPageString.SetSpan(imageSpan, start, end, SpanTypes.ExclusiveExclusive);
                    }

                    txtBottomContent.TextFormatted = firstPageString;
                }

            }
            else
            {
                mainBottomLayout.Visibility = ViewStates.Gone;
                innerTopLayout.Visibility = ViewStates.Visible;
                LinearLayout.LayoutParams txtTopContentParam = txtTopContent.LayoutParameters as LinearLayout.LayoutParams;
                LinearLayout.LayoutParams txtTopTitleParam = txtTopTitle.LayoutParameters as LinearLayout.LayoutParams;
                LinearLayout.LayoutParams btnTopGotItParam = btnTopGotIt.LayoutParameters as LinearLayout.LayoutParams;
                LinearLayout.LayoutParams innerMiddleTopLayoutParam = innerMiddleTopLayout.LayoutParameters as LinearLayout.LayoutParams;

                if (model.ContentShowPosition == ContentType.TopLeft)
                {
                    leftInnerTopLineLayout.Visibility = ViewStates.Visible;
                    rightInnerTopLineLayout.Visibility = ViewStates.Gone;
                    txtTopContentParam.Gravity = GravityFlags.Left;
                    txtTopTitleParam.Gravity = GravityFlags.Left;
                    btnTopGotItParam.Gravity = GravityFlags.Left;
                    innerMiddleTopLayoutParam.Gravity = GravityFlags.Left;
                    innerMiddleTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(12f);
                    innerMiddleTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                }
                else
                {
                    leftInnerTopLineLayout.Visibility = ViewStates.Gone;
                    rightInnerTopLineLayout.Visibility = ViewStates.Visible;
                    txtTopContentParam.Gravity = GravityFlags.Right;
                    txtTopTitleParam.Gravity = GravityFlags.Right;
                    btnTopGotItParam.Gravity = GravityFlags.Right;
                    innerMiddleTopLayoutParam.Gravity = GravityFlags.Right;
                    innerMiddleTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(0);
                    innerMiddleTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(12f);
                }

                txtTopContentParam.Width = (int)((float)this.mContext.Resources.DisplayMetrics.WidthPixels * 0.705);

                txtTopContent.RequestLayout();
                txtTopTitle.RequestLayout();
                btnTopGotIt.RequestLayout();
                innerMiddleTopLayout.RequestLayout();

                if (model.ContentShowPosition == ContentType.TopLeft)
                {
                    txtTopContent.Gravity = GravityFlags.Left;
                    txtTopTitle.Gravity = GravityFlags.Left;
                    innerTopLayout.SetGravity(GravityFlags.Left);
                    innerMiddleTopLayout.SetGravity(GravityFlags.Left);
                }
                else
                {
                    txtTopContent.Gravity = GravityFlags.Right;
                    txtTopTitle.Gravity = GravityFlags.Right;
                    innerTopLayout.SetGravity(GravityFlags.Right);
                    innerMiddleTopLayout.SetGravity(GravityFlags.Right);
                }
                txtTopContent.RequestLayout();
                txtTopTitle.RequestLayout();
                innerTopLayout.RequestLayout();

                txtTopTitle.Text = model.ContentTitle;

                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtTopContent.TextFormatted = Html.FromHtml(model.ContentMessage, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtTopContent.TextFormatted = Html.FromHtml(model.ContentMessage);
                }
            }

            if (model.IsButtonShow)
            {
                btnTopGotIt.Visibility = ViewStates.Visible;
                btnBottomGotIt.Visibility = ViewStates.Visible;
            }
            else
            {
                btnTopGotIt.Visibility = ViewStates.Gone;
                btnBottomGotIt.Visibility = ViewStates.Gone;
            }

            if (this.mFragment != null)
            {
                if (this.mFragment is HomeMenuFragment)
                {
                    if ((list.Count == 4 && !model.NeedHelpHide) || (list.Count == 3 && model.NeedHelpHide))
                    {
                        if (position == 0)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(65f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(275f);
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                int halfScroll = topHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    topHeight = topHeight / 2;
                                }
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(42f);
                            innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                            innerUpperBottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTxtBtnBottomLayout.RequestLayout();

                        }
                        else if (position == 1)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(65f);

                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                int halfScroll = topHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    topHeight = topHeight / 2;
                                }
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = (int)DPUtils.ConvertDPToPx(50f);
                            middleLayout.RequestLayout();

                            int deviceWidth = this.mContext.Resources.DisplayMetrics.WidthPixels;
                            int rightAreaWidth = (int)DPUtils.ConvertDPToPx(8f);
                            int middleAreaWidth = (int)DPUtils.ConvertDPToPx(140f);
                            int leftAreaWidth = deviceWidth - middleAreaWidth - rightAreaWidth;

                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = leftAreaWidth;
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = middleAreaWidth;
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = rightAreaWidth;
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(44f);
                            innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(40f);
                            innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                            innerUpperBottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(40f);
                            innerTxtBtnBottomLayout.RequestLayout();
                        }
                        else if (position == 2)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(345f);
                            int cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                            float heightRatio = 84f / 96f;
                            int cardHeight = (int)(cardWidth * (heightRatio));
                            if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                                cardHeight = cardWidth;
                            }
                            else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                                cardHeight = cardWidth;
                            }
                            int middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(2f);

                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int offsetHeight = (int)DPUtils.ConvertDPToPx(65f);
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                int halfScroll = offsetHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    topHeight = topHeight - (offsetHeight / 2);
                                }
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            if (model.NeedHelpHide)
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(192f);
                            }
                            else
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(122f);
                            }
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(345f);
                            int cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                            float heightRatio = 84f / 96f;
                            int cardHeight = (int)(cardWidth * (heightRatio));
                            if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                                cardHeight = cardWidth;
                            }
                            else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                                cardHeight = cardWidth;
                            }
                            int middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(2f);
                            topHeight = topHeight + middleHeight + (int)DPUtils.ConvertDPToPx(12f);
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }

                            cardWidth = (int)((this.mContext.Resources.DisplayMetrics.WidthPixels / 3.05) - DPUtils.ConvertDPToPx(16f));

                            heightRatio = 56f / 92f;
                            cardHeight = (int)(cardWidth * (heightRatio));

                            middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(42f);

                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = (int)DPUtils.ConvertDPToPx(110f);

                                if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                {
                                    belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                }

                                topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight - middleHeight;
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(175f);
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                    }
                    else if (model.ItemCount == 3)
                    {
                        if (position == 0)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(65f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(235f);
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                int halfScroll = topHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    topHeight = topHeight / 2;
                                }
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(42f);
                            innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                            innerUpperBottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTxtBtnBottomLayout.RequestLayout();
                        }
                        else if (position == 1)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(325f);
                            int cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                            float heightRatio = 84f / 96f;
                            int cardHeight = (int)(cardWidth * (heightRatio));
                            if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                                cardHeight = cardWidth;
                            }
                            else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                                cardHeight = cardWidth;
                            }
                            int middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(2f);

                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int offsetHeight = (int)DPUtils.ConvertDPToPx(65f);
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                int halfScroll = offsetHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    topHeight = topHeight - (offsetHeight / 2);
                                }
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            if (model.NeedHelpHide)
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(192f);
                            }
                            else
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(122f);
                            }
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(325f);
                            int cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                            float heightRatio = 84f / 96f;
                            int cardHeight = (int)(cardWidth * (heightRatio));
                            if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                                cardHeight = cardWidth;
                            }
                            else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                                cardHeight = cardWidth;
                            }
                            int middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(2f);
                            topHeight = topHeight + middleHeight + (int)DPUtils.ConvertDPToPx(12f);
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }

                            cardWidth = (int)((this.mContext.Resources.DisplayMetrics.WidthPixels / 3.05) - DPUtils.ConvertDPToPx(16f));

                            heightRatio = 56f / 92f;
                            cardHeight = (int)(cardWidth * (heightRatio));

                            middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(42f);

                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = (int)DPUtils.ConvertDPToPx(110f);

                                if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                {
                                    belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                }

                                topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight - middleHeight;
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(175f);
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                    }
                    else if (model.ItemCount == 2)
                    {
                        if (position == 0)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(65f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(175f);
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                int halfScroll = topHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    topHeight = topHeight / 2;
                                }
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(42f);
                            innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                            innerUpperBottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTxtBtnBottomLayout.RequestLayout();
                        }
                        else if (position == 1)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(268f);
                            int cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                            float heightRatio = 84f / 96f;
                            int cardHeight = (int)(cardWidth * (heightRatio));
                            if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                                cardHeight = cardWidth;
                            }
                            else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                                cardHeight = cardWidth;
                            }
                            int middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(2f);

                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int offsetHeight = (int)DPUtils.ConvertDPToPx(65f);
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                int halfScroll = offsetHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    topHeight = topHeight - (offsetHeight / 2);
                                }
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            if (model.NeedHelpHide)
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(192f);
                            }
                            else
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(122f);
                            }
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(268f);
                            int cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                            float heightRatio = 84f / 96f;
                            int cardHeight = (int)(cardWidth * (heightRatio));
                            if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                                cardHeight = cardWidth;
                            }
                            else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                                cardHeight = cardWidth;
                            }
                            int middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(2f);
                            topHeight = topHeight + middleHeight + (int)DPUtils.ConvertDPToPx(12f);
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }

                            cardWidth = (int)((this.mContext.Resources.DisplayMetrics.WidthPixels / 3.05) - DPUtils.ConvertDPToPx(16f));

                            heightRatio = 56f / 92f;
                            cardHeight = (int)(cardWidth * (heightRatio));

                            middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(42f);

                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = (int)DPUtils.ConvertDPToPx(110f);

                                if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                {
                                    belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                }

                                topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight - middleHeight;
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(175f);
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                    }
                    else if (model.ItemCount == 1)
                    {
                        if (position == 0)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(75f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(120f);

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(42f);
                            innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                            innerUpperBottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTxtBtnBottomLayout.RequestLayout();
                        }
                        else if (position == 1)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(228f);
                            int cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                            float heightRatio = 84f / 96f;
                            int cardHeight = (int)(cardWidth * (heightRatio));
                            if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                                cardHeight = cardWidth;
                            }
                            else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                                cardHeight = cardWidth;
                            }
                            int middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(2f);

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            if (model.NeedHelpHide)
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(192f);
                            }
                            else
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(122f);
                            }
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(228f);
                            int cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                            float heightRatio = 84f / 96f;
                            int cardHeight = (int)(cardWidth * (heightRatio));
                            if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                                cardHeight = cardWidth;
                            }
                            else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                                cardHeight = cardWidth;
                            }
                            int middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(2f);
                            topHeight = topHeight + middleHeight + (int)DPUtils.ConvertDPToPx(12f);
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }

                            cardWidth = (int)((this.mContext.Resources.DisplayMetrics.WidthPixels / 3.05) - DPUtils.ConvertDPToPx(16f));

                            heightRatio = 56f / 92f;
                            cardHeight = (int)(cardWidth * (heightRatio));

                            middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(42f);

                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = (int)DPUtils.ConvertDPToPx(110f);

                                if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                {
                                    belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                }

                                topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight - middleHeight;
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(175f);
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                    }
                    else
                    {
                        if (position == 0)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(75f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(118f);

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(42f);
                            innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                            innerUpperBottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTxtBtnBottomLayout.RequestLayout();
                        }
                        else if (position == 1)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(218f);
                            int cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                            float heightRatio = 84f / 96f;
                            int cardHeight = (int)(cardWidth * (heightRatio));
                            if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                                cardHeight = cardWidth;
                            }
                            else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                                cardHeight = cardWidth;
                            }
                            int middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(2f);

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            if (model.NeedHelpHide)
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(192f);
                            }
                            else
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(122f);
                            }
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(218f);
                            int cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);
                            float heightRatio = 84f / 96f;
                            int cardHeight = (int)(cardWidth * (heightRatio));
                            if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                                cardHeight = cardWidth;
                            }
                            else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                            {
                                cardWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                                cardHeight = cardWidth;
                            }
                            int middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(2f);
                            topHeight = topHeight + middleHeight + (int)DPUtils.ConvertDPToPx(12f);
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }

                            cardWidth = (int)((this.mContext.Resources.DisplayMetrics.WidthPixels / 3.05) - DPUtils.ConvertDPToPx(16f));

                            heightRatio = 56f / 92f;
                            cardHeight = (int)(cardWidth * (heightRatio));

                            middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(42f);

                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = (int)DPUtils.ConvertDPToPx(110f);

                                if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                {
                                    belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                }

                                topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight - middleHeight;
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(175f);
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                    }
                }
                else if (this.mFragment is ItemisedBillingMenuFragment)
                {
                    if (list.Count == 2)
                    {
                        if (position == 0)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(55f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(285f);
                            int checkPoint = (int)DPUtils.ConvertDPToPx(200f);
                            if (model.DisplayMode == "Extra")
                            {
                                middleHeight = (int)DPUtils.ConvertDPToPx(265f);
                                checkPoint = (int)DPUtils.ConvertDPToPx(180f);
                            }

                            if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                if (((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - checkPoint)))
                                {
                                    topHeight -= (int)DPUtils.ConvertDPToPx(15f);
                                }
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(40f);
                            innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                            innerUpperBottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTxtBtnBottomLayout.RequestLayout();
                        }
                        else
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(375f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(208f);
                            if (model.DisplayMode == "Extra")
                            {
                                topHeight = (int)DPUtils.ConvertDPToPx(350f);
                            }

                            if (model.ItemCount == 0)
                            {
                                if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
                                {
                                    if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                                    {
                                        int bottomHeight = (int)DPUtils.ConvertDPToPx(90f);
                                        topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - bottomHeight - middleHeight;
                                    }
                                }
                            }
                            else if (model.ItemCount == 1)
                            {
                                middleHeight = (int)DPUtils.ConvertDPToPx(130f);

                                if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
                                {
                                    if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                                    {
                                        int bottomHeight = (int)DPUtils.ConvertDPToPx(100f);
                                        if (model.DisplayMode == "Extra")
                                        {
                                            bottomHeight = (int)DPUtils.ConvertDPToPx(85f);
                                        }
                                        topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - bottomHeight - middleHeight;
                                    }
                                }
                            }
                            else if (model.ItemCount == 2)
                            {
                                if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
                                {
                                    if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                                    {
                                        int bottomHeight = (int)DPUtils.ConvertDPToPx(100f);
                                        if (model.DisplayMode == "Extra")
                                        {
                                            bottomHeight = (int)DPUtils.ConvertDPToPx(85f);
                                        }
                                        topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - bottomHeight - middleHeight;
                                    }
                                }
                            }
                            else
                            {
                                if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
                                {
                                    if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                                    {
                                        int diff = (topHeight + middleHeight) - (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)) + (int)DPUtils.ConvertDPToPx(20f);
                                        topHeight = topHeight - diff;
                                    }
                                }
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(200f);
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                    }
                    else
                    {
                        if (position == 0)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(55f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(285f);
                            int checkPoint = (int)DPUtils.ConvertDPToPx(200f);
                            if (model.DisplayMode == "Extra")
                            {
                                middleHeight = (int)DPUtils.ConvertDPToPx(265f);
                                checkPoint = (int)DPUtils.ConvertDPToPx(180f);
                            }

                            if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                if (((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - checkPoint)))
                                {
                                    topHeight -= (int)DPUtils.ConvertDPToPx(15f);
                                }
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(40f);
                            innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                            innerUpperBottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTxtBtnBottomLayout.RequestLayout();
                        }
                        else if (position == 1)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(359f);
                            int middleHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetButtonHeight() + (int)DPUtils.ConvertDPToPx(8f);
                            if (model.DisplayMode == "Extra")
                            {
                                topHeight = (int)DPUtils.ConvertDPToPx(335f);
                            }

                            int rightWidth = (int)DPUtils.ConvertDPToPx(12f);
                            int middleWidth = ((ItemisedBillingMenuFragment)this.mFragment).GetButtonWidth() + (int)DPUtils.ConvertDPToPx(8f);
                            int leftWidth = this.mContext.Resources.DisplayMetrics.WidthPixels - rightWidth - middleWidth;

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = leftWidth;
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = middleWidth;
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = rightWidth;
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(77f);
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(46f);
                            innerTopLayout.RequestLayout();
                        }
                        else if (position == 2)
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(359f);
                            int middleHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetButtonHeight() + (int)DPUtils.ConvertDPToPx(8f);
                            if (model.DisplayMode == "Extra")
                            {
                                topHeight = (int)DPUtils.ConvertDPToPx(335f);
                            }

                            int leftWidth = (int)DPUtils.ConvertDPToPx(12f);
                            int middleWidth = ((ItemisedBillingMenuFragment)this.mFragment).GetButtonWidth() + (int)DPUtils.ConvertDPToPx(8f);
                            int rightWidth = this.mContext.Resources.DisplayMetrics.WidthPixels - leftWidth - middleWidth;

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = leftWidth;
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = middleWidth;
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = rightWidth;
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(97f);
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(31f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else
                        {
                            int topHeight = (int)DPUtils.ConvertDPToPx(430f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(208f);
                            if (model.DisplayMode == "Extra")
                            {
                                topHeight = (int)DPUtils.ConvertDPToPx(405f);
                            }

                            if (model.ItemCount == 0)
                            {
                                if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
                                {
                                    if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                                    {
                                        int bottomHeight = (int)DPUtils.ConvertDPToPx(90f);
                                        topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - bottomHeight - middleHeight;
                                    }
                                }
                            }
                            else if (model.ItemCount == 1)
                            {
                                middleHeight = (int)DPUtils.ConvertDPToPx(130f);

                                if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
                                {
                                    if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                                    {
                                        int bottomHeight = (int)DPUtils.ConvertDPToPx(100f);
                                        if (model.DisplayMode == "Extra")
                                        {
                                            bottomHeight = (int)DPUtils.ConvertDPToPx(85f);
                                        }
                                        topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - bottomHeight - middleHeight;
                                    }
                                }
                            }
                            else if (model.ItemCount == 2)
                            {
                                if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
                                {
                                    if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                                    {
                                        int bottomHeight = (int)DPUtils.ConvertDPToPx(100f);
                                        if (model.DisplayMode == "Extra")
                                        {
                                            bottomHeight = (int)DPUtils.ConvertDPToPx(85f);
                                        }
                                        topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - bottomHeight - middleHeight;
                                    }
                                }
                            }
                            else
                            {
                                if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
                                {
                                    if (((ItemisedBillingMenuFragment)this.mFragment).CheckIsScrollable())
                                    {
                                        int diff = (topHeight + middleHeight) - (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)) + (int)DPUtils.ConvertDPToPx(20f);
                                        topHeight = topHeight - diff;
                                    }
                                }
                            }

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(200f);
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                    }
                }
                else if (this.mFragment is DashboardChartFragment)
                {
                    int topHeight = ((DashboardChartFragment)this.mFragment).GetSMRCardLocation() + (int)DPUtils.ConvertDPToPx(63f);
                    int middleHeight = ((DashboardChartFragment)this.mFragment).GetSMRCardHeight() - (int)DPUtils.ConvertDPToPx(13f);

                    LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                    topLayoutParam.Height = topHeight;
                    topLayout.RequestLayout();
                    LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                    middleLayoutParam.Height = middleHeight;
                    middleLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                    highlightedLeftLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f);
                    highlightedLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                    highlightedRightLayout.RequestLayout();
                    LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                    bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                    bottomLayout.RequestLayout();

                    RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                    innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(180f);
                    innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                    innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                    innerTopLayout.RequestLayout();

                }
                else if (this.mFragment is RewardMenuFragment)
                {
                    if (position == 0)
                    {
                        int middleHeight = ((RewardMenuFragment)this.mFragment).GetTabList()[0].Fragment.GetFirstItemHeight() - (int)DPUtils.ConvertDPToPx(10f);
                        int topHeight = ((RewardMenuFragment)this.mFragment).GetTabList()[0].Fragment.GetFirstItemRelativePosition() - GetStatusBarHeight() + (int)DPUtils.ConvertDPToPx(5f);

                        int leftWidth = (int)DPUtils.ConvertDPToPx(16f);
                        int rightWidth = (int)DPUtils.ConvertDPToPx(16f);
                        int middleWidth = this.mContext.Resources.DisplayMetrics.WidthPixels - leftWidth - rightWidth;

                        LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                        topLayoutParam.Height = topHeight;
                        topLayout.RequestLayout();
                        LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                        middleLayoutParam.Height = middleHeight;
                        middleLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLeftLayoutParam.Width = leftWidth;
                        highlightedLeftLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLayoutParam.Width = middleWidth;
                        highlightedLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedRightLayoutParam.Width = rightWidth;
                        highlightedRightLayout.RequestLayout();
                        LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                        bottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(42f);
                        innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                        innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                        innerUpperBottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                        innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTxtBtnBottomLayout.RequestLayout();

                    }
                    else if (list.Count == 2 && position == 1)
                    {
                        int middleHeight = (int)DPUtils.ConvertDPToPx(38f);
                        int topHeight = (int)DPUtils.ConvertDPToPx(10f);

                        int rightWidth = (int)DPUtils.ConvertDPToPx(5f);
                        int middleWidth = (int)DPUtils.ConvertDPToPx(38f);
                        int leftWidth = this.mContext.Resources.DisplayMetrics.WidthPixels - rightWidth - middleWidth;

                        LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                        topLayoutParam.Height = topHeight;
                        topLayout.RequestLayout();
                        LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                        middleLayoutParam.Height = middleHeight;
                        middleLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLeftLayoutParam.Width = leftWidth;
                        highlightedLeftLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLayoutParam.Width = middleWidth;
                        highlightedLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedRightLayoutParam.Width = rightWidth;
                        highlightedRightLayout.RequestLayout();
                        LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                        bottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(36f);
                        innerUpperBottomLayoutParam.LeftMargin = 0;
                        innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(19f);
                        innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                        innerUpperBottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerTxtBtnBottomLayoutParam.LeftMargin = 0;
                        innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(19f);
                        innerTxtBtnBottomLayout.RequestLayout();
                    }
                    else if (list.Count == 3 && position == 1)
                    {
                        int middleHeight = ((RewardMenuFragment)this.mFragment).GetTabHeight() + (int)DPUtils.ConvertDPToPx(6f);
                        int topHeight = ((RewardMenuFragment)this.mFragment).GetTabRelativePosition() - GetStatusBarHeight();

                        int leftWidth = 0;
                        int rightWidth = 0;
                        int middleWidth = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);

                        LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                        topLayoutParam.Height = topHeight;
                        topLayout.RequestLayout();
                        LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                        middleLayoutParam.Height = middleHeight;
                        middleLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLeftLayoutParam.Width = leftWidth;
                        highlightedLeftLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLayoutParam.Width = middleWidth;
                        highlightedLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedRightLayoutParam.Width = rightWidth;
                        highlightedRightLayout.RequestLayout();
                        LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                        bottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(44f);
                        innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                        innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                        innerUpperBottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                        innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTxtBtnBottomLayout.RequestLayout();
                    }
                    else if (list.Count == 3 && position == 2)
                    {
                        int middleHeight = (int)DPUtils.ConvertDPToPx(38f);
                        int topHeight = (int)DPUtils.ConvertDPToPx(10f);

                        int rightWidth = (int)DPUtils.ConvertDPToPx(5f);
                        int middleWidth = (int)DPUtils.ConvertDPToPx(38f);
                        int leftWidth = this.mContext.Resources.DisplayMetrics.WidthPixels - rightWidth - middleWidth;

                        LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                        topLayoutParam.Height = topHeight;
                        topLayout.RequestLayout();
                        LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                        middleLayoutParam.Height = middleHeight;
                        middleLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLeftLayoutParam.Width = leftWidth;
                        highlightedLeftLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLayoutParam.Width = middleWidth;
                        highlightedLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedRightLayoutParam.Width = rightWidth;
                        highlightedRightLayout.RequestLayout();
                        LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                        bottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(36f);
                        innerUpperBottomLayoutParam.LeftMargin = 0;
                        innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(19f);
                        innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                        innerUpperBottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerTxtBtnBottomLayoutParam.LeftMargin = 0;
                        innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(19f);
                        innerTxtBtnBottomLayout.RequestLayout();
                    }
                }
                else if (this.mFragment is WhatsNewMenuFragment)
                {
                    if (list.Count == 2)
                    {
                        if (position == 0)
                        {
                            int middleHeight = ((WhatsNewMenuFragment)this.mFragment).GetTabHeight() + (int)DPUtils.ConvertDPToPx(6f);
                            int topHeight = ((WhatsNewMenuFragment)this.mFragment).GetTabRelativePosition() - GetStatusBarHeight();

                            int leftWidth = 0;
                            int rightWidth = 0;
                            int middleWidth = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = leftWidth;
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = middleWidth;
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = rightWidth;
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(44f);
                            innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(46f);
                            innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                            innerUpperBottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(46f);
                            innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTxtBtnBottomLayout.RequestLayout();
                        }
                        else
                        {
                            int middleHeight = ((WhatsNewMenuFragment)this.mFragment).GetTabList()[0].Fragment.GetFirstItemHeight() - (int)DPUtils.ConvertDPToPx(10f);
                            int topHeight = ((WhatsNewMenuFragment)this.mFragment).GetTabList()[0].Fragment.GetFirstItemRelativePosition() - GetStatusBarHeight() + (int)DPUtils.ConvertDPToPx(5f);

                            int leftWidth = (int)DPUtils.ConvertDPToPx(16f);
                            int rightWidth = (int)DPUtils.ConvertDPToPx(16f);
                            int middleWidth = this.mContext.Resources.DisplayMetrics.WidthPixels - leftWidth - rightWidth;

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = middleHeight;
                            middleLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = leftWidth;
                            highlightedLeftLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = middleWidth;
                            highlightedLayout.RequestLayout();
                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedRightLayoutParam.Width = rightWidth;
                            highlightedRightLayout.RequestLayout();
                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(42f);
                            innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                            innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                            innerUpperBottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                            innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTxtBtnBottomLayout.RequestLayout();
                        }
                    }
                    else
                    {
                        int middleHeight = ((WhatsNewMenuFragment)this.mFragment).GetTabList()[0].Fragment.GetFirstItemHeight() - (int)DPUtils.ConvertDPToPx(10f);
                        int topHeight = ((WhatsNewMenuFragment)this.mFragment).GetTabList()[0].Fragment.GetFirstItemRelativePosition() - GetStatusBarHeight() + (int)DPUtils.ConvertDPToPx(5f);

                        int leftWidth = (int)DPUtils.ConvertDPToPx(16f);
                        int rightWidth = (int)DPUtils.ConvertDPToPx(16f);
                        int middleWidth = this.mContext.Resources.DisplayMetrics.WidthPixels - leftWidth - rightWidth;

                        LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                        topLayoutParam.Height = topHeight;
                        topLayout.RequestLayout();
                        LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                        middleLayoutParam.Height = middleHeight;
                        middleLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLeftLayoutParam.Width = leftWidth;
                        highlightedLeftLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLayoutParam.Width = middleWidth;
                        highlightedLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedRightLayoutParam.Width = rightWidth;
                        highlightedRightLayout.RequestLayout();
                        LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                        bottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(42f);
                        innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                        innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                        innerUpperBottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                        innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTxtBtnBottomLayout.RequestLayout();
                    }
                }
            }
            else if (this.mContext is BillingDetailsActivity)
            {
                int middleHeight = ((BillingDetailsActivity)this.mContext).GetViewBillButtonHeight() + (int)DPUtils.ConvertDPToPx(10f);
                int topHeight = ((BillingDetailsActivity)this.mContext).GetTopHeight();

                int leftWidth = (int)DPUtils.ConvertDPToPx(14f);
                int middleWidth = ((this.mContext.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(3f) - (int)DPUtils.ConvertDPToPx(24f)) / 2) + (int)DPUtils.ConvertDPToPx(2f);
                int rightWidth = this.mContext.Resources.DisplayMetrics.WidthPixels - leftWidth - middleWidth;

                LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                topLayoutParam.Height = topHeight;
                topLayout.RequestLayout();
                LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                middleLayoutParam.Height = middleHeight;
                middleLayout.RequestLayout();
                LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                highlightedLeftLayoutParam.Width = leftWidth;
                highlightedLeftLayout.RequestLayout();
                LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                highlightedLayoutParam.Width = middleWidth;
                highlightedLayout.RequestLayout();
                LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                highlightedRightLayoutParam.Width = rightWidth;
                highlightedRightLayout.RequestLayout();
                LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                bottomLayout.RequestLayout();

                RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(160f);
                innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                innerTopLayout.RequestLayout();

            }
            else if (this.mContext is SSMRMeterHistoryActivity)
            {
                int topHeight = (int)DPUtils.ConvertDPToPx(255f);
                int middleHeight = ((SSMRMeterHistoryActivity)this.mContext).GetSMRTopViewHeight();
                int checkPoint = (int)DPUtils.ConvertDPToPx(60f);
                if (model.DisplayMode == "NONSMR")
                {
                    checkPoint = (int)DPUtils.ConvertDPToPx(40f);
                }

                if (model.ItemCount == 1)
                {
                    checkPoint = 0;
                }

                if (((SSMRMeterHistoryActivity)this.mContext).CheckIsScrollable())
                {
                    if (((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - checkPoint)))
                    {
                        topHeight -= (int)DPUtils.ConvertDPToPx(30f);
                    }
                }

                LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                topLayoutParam.Height = topHeight;
                topLayout.RequestLayout();
                LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                middleLayoutParam.Height = middleHeight;
                middleLayout.RequestLayout();
                LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                highlightedLeftLayout.RequestLayout();
                LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);
                highlightedLayout.RequestLayout();
                LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(0f);
                highlightedRightLayout.RequestLayout();
                LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                bottomLayout.RequestLayout();

                RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(180f);
                innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                innerTopLayout.RequestLayout();
            }
            else if (this.mContext is SubmitMeterReadingActivity)
            {
                int topHeight = ((SubmitMeterReadingActivity)this.mContext).GetTopLocation() - (int)GetStatusBarHeight() + (int)DPUtils.ConvertDPToPx(6f);

                int middleHeight = (int)DPUtils.ConvertDPToPx(150f);


                if (model.ContentShowPosition == ContentType.TopLeft)
                {
                    RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                    innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(180f);
                    innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                    innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                    innerTopLayout.RequestLayout();
                }
                else
                {
                    LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                    innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(40f);
                    innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                    innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                    innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                    innerUpperBottomLayout.RequestLayout();

                    LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                    innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                    innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                    innerTxtBtnBottomLayout.RequestLayout();
                }


                LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                topLayoutParam.Height = topHeight;
                topLayout.RequestLayout();
                LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                middleLayoutParam.Height = middleHeight;
                middleLayout.RequestLayout();
                LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                highlightedLeftLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                highlightedLeftLayout.RequestLayout();
                LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                highlightedLayoutParam.Width = this.mContext.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(32f);
                highlightedLayout.RequestLayout();
                LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                highlightedRightLayoutParam.Width = (int)DPUtils.ConvertDPToPx(16f);
                highlightedRightLayout.RequestLayout();
                LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                bottomLayout.RequestLayout();
            }
            else if (this.mContext is RewardDetailActivity)
            {
                if (position == 0)
                {
                    int middleHeight = ((RewardDetailActivity)this.mContext).GetRewardSaveButtonHeight() + (int)DPUtils.ConvertDPToPx(12f);
                    int topHeight = ((RewardDetailActivity)this.mContext).GetSaveButtonRelativePosition()[1] - GetStatusBarHeight() - (int)DPUtils.ConvertDPToPx(6f);

                    int leftWidth = (int)DPUtils.ConvertDPToPx(14f);
                    int middleWidth = ((this.mContext.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(3f) - (int)DPUtils.ConvertDPToPx(24f)) / 2) + (int)DPUtils.ConvertDPToPx(2f);
                    int rightWidth = this.mContext.Resources.DisplayMetrics.WidthPixels - leftWidth - middleWidth;

                    LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                    topLayoutParam.Height = topHeight;
                    topLayout.RequestLayout();
                    LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                    middleLayoutParam.Height = middleHeight;
                    middleLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedLeftLayoutParam.Width = leftWidth;
                    highlightedLeftLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedLayoutParam.Width = middleWidth;
                    highlightedLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedRightLayoutParam.Width = rightWidth;
                    highlightedRightLayout.RequestLayout();
                    LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                    bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                    bottomLayout.RequestLayout();

                    RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                    innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(96f);
                    innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(46f);
                    innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                    innerTopLayout.RequestLayout();
                }
                else
                {
                    int middleHeight = ((RewardDetailActivity)this.mContext).GetRewardSaveButtonHeight() + (int)DPUtils.ConvertDPToPx(12f);
                    int topHeight = ((RewardDetailActivity)this.mContext).GetSaveButtonRelativePosition()[1] - GetStatusBarHeight() - (int)DPUtils.ConvertDPToPx(6f);

                    int rightWidth = (int)DPUtils.ConvertDPToPx(14f);
                    int middleWidth = ((this.mContext.Resources.DisplayMetrics.WidthPixels - (int)DPUtils.ConvertDPToPx(3f) - (int)DPUtils.ConvertDPToPx(24f)) / 2) + (int)DPUtils.ConvertDPToPx(2f);
                    int leftWidth = this.mContext.Resources.DisplayMetrics.WidthPixels - rightWidth - middleWidth;

                    LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                    topLayoutParam.Height = topHeight;
                    topLayout.RequestLayout();
                    LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                    middleLayoutParam.Height = middleHeight;
                    middleLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedLeftLayoutParam.Width = leftWidth;
                    highlightedLeftLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedLayoutParam.Width = middleWidth;
                    highlightedLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedRightLayoutParam.Width = rightWidth;
                    highlightedRightLayout.RequestLayout();
                    LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                    bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                    bottomLayout.RequestLayout();

                    RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                    innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(200f);
                    innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(0f);
                    innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(46f);
                    innerTopLayout.RequestLayout();
                }
            }
            else if (this.mContext is ApplicationStatusLandingActivity)
             {
                if (model.DisplayMode == "Extra")
                {
                    int topHeight = (int)DPUtils.ConvertDPToPx(55f);
                    int middleHeight = (int)DPUtils.ConvertDPToPx(200f);

                    int leftWidth = 0;
                    int rightWidth = 0;
                    int middleWidth = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);

                    LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                    topLayoutParam.Height = topHeight;
                    topLayout.RequestLayout();
                    LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                    middleLayoutParam.Height = middleHeight;
                    middleLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedLeftLayoutParam.Width = leftWidth;
                    highlightedLeftLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedLayoutParam.Width = middleWidth;
                    highlightedLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedRightLayoutParam.Width = rightWidth;
                    highlightedRightLayout.RequestLayout();
                    LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                    bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                    bottomLayout.RequestLayout();

                    LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                    innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(44f);
                    innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                    innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                    innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                    innerUpperBottomLayout.RequestLayout();

                    LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                    innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                    innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                    innerTxtBtnBottomLayout.RequestLayout();
                }
                else
                {
                    if (position == 0)
                    {

                        int topHeight = ((ApplicationStatusLandingActivity)this.mContext).GetTopHeight() - GetStatusBarHeight();
                        int middleHeight = ((ApplicationStatusLandingActivity)this.mContext).GetHighlightedHeight();

                        int leftWidth = 0;
                        int rightWidth = 0;
                        int middleWidth = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);

                        LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                        topLayoutParam.Height = topHeight;
                        topLayout.RequestLayout();
                        LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                        middleLayoutParam.Height = middleHeight;
                        middleLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLeftLayoutParam.Width = leftWidth;
                        highlightedLeftLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLayoutParam.Width = middleWidth;
                        highlightedLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedRightLayoutParam.Width = rightWidth;
                        highlightedRightLayout.RequestLayout();
                        LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                        bottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(44f);
                        innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                        innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                        innerUpperBottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                        innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTxtBtnBottomLayout.RequestLayout();
                    }
                    else
                    {
                        int middleHeight = ((ApplicationStatusLandingActivity)this.mContext).GetSearchButtonHeight() + (int)DPUtils.ConvertDPToPx(10f);
                        int topHeight = ((ApplicationStatusLandingActivity)this.mContext).GetTopSearchHeight() + (int)DPUtils.ConvertDPToPx(37f);

                        int leftWidth = 0;
                        int rightWidth = 0;
                        int middleWidth = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);

                        LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                        topLayoutParam.Height = topHeight;
                        topLayout.RequestLayout();
                        LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                        middleLayoutParam.Height = middleHeight;
                        middleLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLeftLayoutParam.Width = leftWidth;
                        highlightedLeftLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLayoutParam.Width = middleWidth;
                        highlightedLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedRightLayoutParam.Width = rightWidth;
                        highlightedRightLayout.RequestLayout();
                        LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                        bottomLayout.RequestLayout();


                        RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                        innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts? (int)DPUtils.ConvertDPToPx(150f) : (int)DPUtils.ConvertDPToPx(110f);
                        innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                        innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTopLayout.RequestLayout();
                    }

                }
            }
            else if (this.mContext is ApplicationStatusDetailActivity)
            {
                if (model.DisplayMode == "Extra")
                {
                    int progressDateCount = ((ApplicationStatusDetailActivity)this.mContext).GetProgressDateCount();
                    int pressView = ((ApplicationStatusDetailActivity)this.mContext).GetRecyclerViewHeight();

                    pressView = pressView < 500 ? pressView - 65 : pressView - (progressDateCount == 1 ? 65 : (progressDateCount == 2 ? 80 : (progressDateCount == 3 ? 120 : (progressDateCount == 4 ? 160 : (progressDateCount == 5 ? 200 : 0)))));
                    float h1 = 0;
                    float h2 = 180f;
                    int topHeight = ((ApplicationStatusDetailActivity)this.mContext).GetTopHeight() + pressView + (int)DPUtils.ConvertDPToPx(h1);
                    int middleHeight = (int)DPUtils.ConvertDPToPx(h2);

                    int leftWidth = 0;
                    int rightWidth = 0;
                    int middleWidth = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);

                    LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                    topLayoutParam.Height = topHeight;
                    topLayout.RequestLayout();
                    LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                    middleLayoutParam.Height = middleHeight;
                    middleLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedLeftLayoutParam.Width = leftWidth;
                    highlightedLeftLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedLayoutParam.Width = middleWidth;
                    highlightedLayout.RequestLayout();
                    LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                    highlightedRightLayoutParam.Width = rightWidth;
                    highlightedRightLayout.RequestLayout();
                    LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                    bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                    bottomLayout.RequestLayout();

                    RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                    innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(100f);
                    innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                    innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                    innerTopLayout.RequestLayout();
                }
                else
                {
                    if (position == 0)
                    {



                        int topHeight = ((ApplicationStatusDetailActivity)this.mContext).GetTopCtaHeight() + (int)DPUtils.ConvertDPToPx(45f);

                        int middleHeight = (int)DPUtils.ConvertDPToPx(180f);

                        int leftWidth = 0;
                        int rightWidth = 0;
                        int middleWidth = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);

                        LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                        topLayoutParam.Height = topHeight;
                        topLayout.RequestLayout();
                        LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                        middleLayoutParam.Height = middleHeight;
                        middleLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLeftLayoutParam.Width = leftWidth;
                        highlightedLeftLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLayoutParam.Width = middleWidth;
                        highlightedLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedRightLayoutParam.Width = rightWidth;
                        highlightedRightLayout.RequestLayout();
                        LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                        bottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(44f);
                        innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                        innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                        innerUpperBottomLayout.RequestLayout();

                        LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(24f);
                        innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTxtBtnBottomLayout.RequestLayout();
                    }
                    else
                    {

                        int middleHeight = ((ApplicationStatusDetailActivity)this.mContext).GetCtaButtonHeight() + (int)DPUtils.ConvertDPToPx(45f);
                        int topHeight = ((ApplicationStatusDetailActivity)this.mContext).GetTopCtaHeight() + (int)DPUtils.ConvertDPToPx(45f);

                        int leftWidth = 0;
                        int rightWidth = 0;
                        int middleWidth = this.mContext.Resources.DisplayMetrics.WidthPixels + (int)DPUtils.ConvertDPToPx(10f);

                        LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                        topLayoutParam.Height = topHeight;
                        topLayout.RequestLayout();
                        LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                        middleLayoutParam.Height = middleHeight;
                        middleLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLeftLayoutParam.Width = leftWidth;
                        highlightedLeftLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedLayoutParam.Width = middleWidth;
                        highlightedLayout.RequestLayout();
                        LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;
                        highlightedRightLayoutParam.Width = rightWidth;
                        highlightedRightLayout.RequestLayout();
                        LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                        bottomLayout.RequestLayout();


                        RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(100f);
                        innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                        innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTopLayout.RequestLayout();
                    }

                }
            }

            container.AddView(rootView);
            return rootView;
        }

        private void BtnGotIt_Click(object sender, EventArgs e)
        {
            this.mDialog.DismissAllowingStateLoss();
            NewAppTutorialUtils.CloseNewAppTutorial();
            if (this.mFragment != null)
            {
                if (this.mFragment is HomeMenuFragment)
                {
                    ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                    UserSessions.DoHomeTutorialShown(this.mPref);
                    ((HomeMenuFragment)this.mFragment).RestartHomeMenu();
                }
                else if (this.mFragment is ItemisedBillingMenuFragment)
                {
                    ((ItemisedBillingMenuFragment)this.mFragment).ItemizedBillingCustomScrolling(0);
                    if (list.Count == 2)
                    {
                        UserSessions.DoItemizedBillingRETutorialShown(this.mPref);
                    }
                    else
                    {
                        UserSessions.DoItemizedBillingNMSMTutorialShown(this.mPref);
                    }
                }
                else if (this.mFragment is DashboardChartFragment)
                {
                    ((DashboardChartFragment)this.mFragment).DashboardCustomScrolling(0);
                    ((DashboardChartFragment)this.mFragment).ShowBottomSheet();
                    UserSessions.DoSMRDashboardTutorialShown(this.mPref);
                }
                else if (this.mFragment is RewardMenuFragment)
                {
                    ((RewardMenuFragment)this.mFragment).StopScrolling();
                    UserSessions.DoRewardsShown(this.mPref);
                }
                else if (this.mFragment is WhatsNewMenuFragment)
                {
                    ((WhatsNewMenuFragment)this.mFragment).StopScrolling();
                    UserSessions.DoWhatsNewShown(this.mPref);
                }
            }
            else
            {
                if (this.mContext is BillingDetailsActivity)
                {
                    UserSessions.DoItemizedBillingDetailTutorialShown(this.mPref);
                }
                else if (this.mContext is SSMRMeterHistoryActivity)
                {
                    ((SSMRMeterHistoryActivity)this.mContext).MeterHistoryCustomScrolling(0);
                    UserSessions.DoSMRMeterHistoryTutorialShown(this.mPref);
                }
                else if (this.mContext is SubmitMeterReadingActivity)
                {
                    ((SubmitMeterReadingActivity)mContext).SubmitMeterCustomScrolling(0);
                    UserSessions.DoSMRSubmitMeterTutorialShown(this.mPref);
                }
                else if (this.mContext is RewardDetailActivity)
                {
                    UserSessions.DoRewardsDetailShown(this.mPref);
                }
                else if (this.mContext is ApplicationStatusLandingActivity)
                {
                    UserSessions.DoApplicationStatusShown(this.mPref);
                }
            }
        }

        public override int GetItemPosition(Java.Lang.Object @object)
        {
            return PagerAdapter.PositionNone;
        }

        public override int Count => list.Count;

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((View)@object);
        }

        private Bitmap Base64ToBitmap(String base64String)
        {
            byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
            return BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
        }

        private int GetStatusBarHeight()
        {
            int statusBarHeight = 0;

            try
            {
                Rect rectangle = new Rect();
                Window window = this.mContext.Window;
                window.DecorView.GetWindowVisibleDisplayFrame(rectangle);
                statusBarHeight = rectangle.Top;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return statusBarHeight;
        }

        private int GetTitleBarHeight()
        {
            int titleBarHeight = 0;

            try
            {
                Rect rectangle = new Rect();
                Window window = this.mContext.Window;
                window.DecorView.GetWindowVisibleDisplayFrame(rectangle);
                int statusBarHeight = rectangle.Top;

                int contentViewTop =
                    window.FindViewById(Window.IdAndroidContent).Top;
                titleBarHeight = contentViewTop - statusBarHeight;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return titleBarHeight;
        }
    }
}