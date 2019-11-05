using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.NewAppTutorial.Adapter
{
    public class NewAppTutorialPagerAdapter : PagerAdapter
    {
        private Context mContext;
        private List<NewAppModel> list = new List<NewAppModel>();
        private Android.Support.V4.App.DialogFragment mDialog;
        private Android.App.Fragment mFragment;
        private ISharedPreferences mPref;

        public NewAppTutorialPagerAdapter(Context ctx, Android.App.Fragment fragment, ISharedPreferences pref, Android.Support.V4.App.DialogFragment dialog, List<NewAppModel> items)
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

            if (model.TargetPage == "HomeMenu")
            {
                if (list.Count == 4)
                {
                    if (position == 0)
                    {
                        int topHeight = (int)DPUtils.ConvertDPToPx(65f);
                        int middleHeight = (int)DPUtils.ConvertDPToPx(275f);
                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                int halfScroll = topHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    topHeight = topHeight / 2;
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

                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                int halfScroll = topHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    topHeight = topHeight / 2;
                                }
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

                        if (this.mFragment is HomeMenuFragment)
                        {
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
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(122f);
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
                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                        }

                        cardWidth = (int)((this.mContext.Resources.DisplayMetrics.WidthPixels / 3.05) - DPUtils.ConvertDPToPx(16f));

                        heightRatio = 56f / 92f;
                        cardHeight = (int)(cardWidth * (heightRatio));

                        middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(42f);

                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = (int)DPUtils.ConvertDPToPx(90f);

                                topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight - middleHeight;
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
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(175f);
                        innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                        innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTopLayout.RequestLayout();
                    }
                }
                else if (model.AccountCount == 3)
                {
                    if (position == 0)
                    {
                        int topHeight = (int)DPUtils.ConvertDPToPx(65f);
                        int middleHeight = (int)DPUtils.ConvertDPToPx(235f);
                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                int halfScroll = topHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    topHeight = topHeight / 2;
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

                        if (this.mFragment is HomeMenuFragment)
                        {
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
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(122f);
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
                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                        }

                        cardWidth = (int)((this.mContext.Resources.DisplayMetrics.WidthPixels / 3.05) - DPUtils.ConvertDPToPx(16f));

                        heightRatio = 56f / 92f;
                        cardHeight = (int)(cardWidth * (heightRatio));

                        middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(42f);

                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = (int)DPUtils.ConvertDPToPx(90f);

                                topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight - middleHeight;
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
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(175f);
                        innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                        innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTopLayout.RequestLayout();
                    }
                }
                else if (model.AccountCount == 2)
                {
                    if (position == 0)
                    {
                        int topHeight = (int)DPUtils.ConvertDPToPx(65f);
                        int middleHeight = (int)DPUtils.ConvertDPToPx(175f);
                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int diffHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - ((HomeMenuFragment)this.mFragment).OnGetEndOfScrollView());
                                int halfScroll = topHeight / 2;

                                if (diffHeight < halfScroll)
                                {
                                    topHeight = topHeight / 2;
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

                        if (this.mFragment is HomeMenuFragment)
                        {
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
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(122f);
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
                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                        }

                        cardWidth = (int)((this.mContext.Resources.DisplayMetrics.WidthPixels / 3.05) - DPUtils.ConvertDPToPx(16f));

                        heightRatio = 56f / 92f;
                        cardHeight = (int)(cardWidth * (heightRatio));

                        middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(42f);

                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = (int)DPUtils.ConvertDPToPx(90f);

                                if (this.mContext.Resources.DisplayMetrics.HeightPixels <= (int)DPUtils.ConvertDPToPx(800f))
                                {
                                    belowHeight = (int)DPUtils.ConvertDPToPx(115);
                                }

                                topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight - middleHeight;
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
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(175f);
                        innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                        innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTopLayout.RequestLayout();
                    }
                }
                else if (model.AccountCount == 1)
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
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(122f);
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
                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                        }

                        cardWidth = (int)((this.mContext.Resources.DisplayMetrics.WidthPixels / 3.05) - DPUtils.ConvertDPToPx(16f));

                        heightRatio = 56f / 92f;
                        cardHeight = (int)(cardWidth * (heightRatio));

                        middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(42f);

                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = (int)DPUtils.ConvertDPToPx(90f);

                                if (this.mContext.Resources.DisplayMetrics.HeightPixels <= (int)DPUtils.ConvertDPToPx(800f))
                                {
                                    belowHeight = (int)DPUtils.ConvertDPToPx(115);
                                }

                                topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight - middleHeight;
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
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(122f);
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
                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                        }

                        cardWidth = (int)((this.mContext.Resources.DisplayMetrics.WidthPixels / 3.05) - DPUtils.ConvertDPToPx(16f));

                        heightRatio = 56f / 92f;
                        cardHeight = (int)(cardWidth * (heightRatio));

                        middleHeight = cardHeight + (int)DPUtils.ConvertDPToPx(42f);

                        if (this.mFragment is HomeMenuFragment)
                        {
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = (int)DPUtils.ConvertDPToPx(90f);

                                if (this.mContext.Resources.DisplayMetrics.HeightPixels <= (int)DPUtils.ConvertDPToPx(800f))
                                {
                                    belowHeight = (int)DPUtils.ConvertDPToPx(115);
                                }

                                topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight - middleHeight;
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
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(175f);
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
            this.mDialog.Dismiss();
            if (this.mFragment is HomeMenuFragment)
            {
                ((HomeMenuFragment)this.mFragment).HomeMenuCustomScrolling(0);
                UserSessions.DoHomeTutorialShown(this.mPref);
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
    }
}