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
using myTNB_Android.Src.ManageAccess.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
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
using myTNB_Android.Src.MyAccount.Activity;
using Newtonsoft.Json;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageSupplyAccount.Activity;
using myTNB_Android.Src.ManageBillDelivery.MVP;
using myTNB.Mobile;

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

            //add yana
            Button btnTopUpdateNickname = rootView.FindViewById(Resource.Id.btnTopUpdateNickname) as Button;
            btnTopUpdateNickname.Click += BtnUpdate_Click;

            btnBottomGotIt.Click += BtnGotIt_Click;
            btnTopGotIt.Click += BtnGotIt_Click;

            TextViewUtils.SetMuseoSans300Typeface(txtBottomContent, txtTopContent);
            TextViewUtils.SetMuseoSans500Typeface(txtBottomTitle, txtTopTitle, btnBottomGotIt, btnTopGotIt);
            TextViewUtils.SetTextSize14(txtTopTitle, txtTopContent, txtBottomTitle, txtBottomContent);
            TextViewUtils.SetTextSize16(btnTopGotIt, btnBottomGotIt, btnTopUpdateNickname);

            btnTopGotIt.Text = Utility.GetLocalizedCommonLabel("gotIt");
            btnBottomGotIt.Text = Utility.GetLocalizedCommonLabel("gotIt");

            //yana
            btnTopUpdateNickname.Text = Utility.GetLocalizedLabel("DashboardHome", "tutorialUpdateNicknameBtn");

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
                try
                {
                    if (!(this.mFragment is ItemisedBillingMenuFragment))
                    {
                        txtBottomContentParam.Width = (int)((float)this.mContext.Resources.DisplayMetrics.WidthPixels * 0.705);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
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

                //yana
                LinearLayout.LayoutParams btnTopUpdateaNicknameParam = btnTopUpdateNickname.LayoutParameters as LinearLayout.LayoutParams;

                if (model.ContentShowPosition == ContentType.TopLeft)
                {
                    leftInnerTopLineLayout.Visibility = ViewStates.Visible;
                    rightInnerTopLineLayout.Visibility = ViewStates.Gone;
                    txtTopContentParam.Gravity = GravityFlags.Left;
                    txtTopTitleParam.Gravity = GravityFlags.Left;
                    btnTopGotItParam.Gravity = GravityFlags.Left;
                    //yana
                    btnTopUpdateaNicknameParam.Gravity = GravityFlags.Center;
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
                    //yana
                    btnTopUpdateaNicknameParam.Gravity = GravityFlags.Center;
                    innerMiddleTopLayoutParam.Gravity = GravityFlags.Right;
                    innerMiddleTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(0);
                    innerMiddleTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(12f);
                }

                txtTopContentParam.Width = (int)((float)this.mContext.Resources.DisplayMetrics.WidthPixels * 0.705);

                txtTopContent.RequestLayout();
                txtTopTitle.RequestLayout();
                btnTopGotIt.RequestLayout();
                //yana
                btnTopUpdateNickname.RequestLayout();
                innerMiddleTopLayout.RequestLayout();
                btnTopUpdateNickname.RequestLayout();

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

            int ncAcc = UserSessions.GetNCFlag(this.mPref);
            bool newNCFlag = UserSessions.GetNewNCFlag(this.mPref);

            //yana
            if (model.IsButtonUpdateShow && ncAcc > 0)
            {
                btnTopUpdateNickname.Visibility = ViewStates.Visible;
                //btnBottomGotIt.Visibility = ViewStates.Visible;
            }
            else
            {
                btnTopUpdateNickname.Visibility = ViewStates.Gone;
                //btnBottomGotIt.Visibility = ViewStates.Gone;
            }

            if (this.mFragment != null)
            {
                if (this.mFragment is HomeMenuFragment && ncAcc > 0 && newNCFlag == false)
                {
                    if (ncAcc == 1 || ncAcc > 1)
                    {
                        if (position == 0)
                        {
                            if (model.ItemCount > 1)
                            {
                                int topHeight;
                                if (UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify()
                                     && !MyTNBAccountManagement.GetInstance().COMCLandNEM())
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight();
                                }
                                else
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                }
                                //int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                int middleHeight = (int)DPUtils.ConvertDPToPx(160f);

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
                            else
                            {
                                int topHeight;
                                if (UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify()
                                     && !MyTNBAccountManagement.GetInstance().COMCLandNEM())
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight();
                                }
                                else
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                }
                                //int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
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
                        }
                    }
                }
                else if (this.mFragment is HomeMenuFragment && ncAcc > 0 && newNCFlag == true)
                {
                    if (ncAcc > 0)
                    {
                        if (model.ItemCount > 3)
                        {
                            if (position == 0)
                            {
                                if (model.ItemCount > 1)
                                {
                                    int topHeight;
                                    if (UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify()
                                         && !MyTNBAccountManagement.GetInstance().COMCLandNEM())
                                    {
                                        topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight();
                                    }
                                    else
                                    {
                                        topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                    }
                                    //int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                    //int middleHeight = (int)DPUtils.ConvertDPToPx(160f);
                                    int middleHeight;
                                    middleHeight = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(190f) : (int)DPUtils.ConvertDPToPx(160f);

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
                                else
                                {
                                    int topHeight;
                                    if (UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify()
                                         && !MyTNBAccountManagement.GetInstance().COMCLandNEM())
                                    {
                                        topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight();
                                    }
                                    else
                                    {
                                        topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                    }
                                    //int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
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
                            }
                            else if (position == 1)
                            {
                                int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(25f);
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
                            else if (position == 2)
                            {
                                int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(27f);

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
                                middleLayoutParam.Height = (int)DPUtils.ConvertDPToPx(50f);
                                middleLayout.RequestLayout();

                                int deviceWidth = this.mContext.Resources.DisplayMetrics.WidthPixels;
                                int rightAreaWidth = (int)DPUtils.ConvertDPToPx(8f);
                                int middleAreaWidth = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(160f) : (int)DPUtils.ConvertDPToPx(140f);
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
                            }
                            else if (position == 3)
                            {
                                int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(10f);

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
                                middleLayoutParam.Height = ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight();
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
                                    innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(164f) : (int)DPUtils.ConvertDPToPx(132f);
                                }
                                innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                                innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                                innerTopLayout.RequestLayout();
                            }
                            else if (model.Feature == FeatureType.MyHome)
                            {
                                int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(10f);

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

                                var padding = (int)DPUtils.ConvertDPToPx(16f);
                                var screenWidth = this.mContext.Resources.DisplayMetrics.WidthPixels;
                                var myHomeTopHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemTopPosition("1008");
                                var myHomeWidth = ((HomeMenuFragment)this.mFragment).GetMyServiceItemWidth();
                                var myHomeHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemHeight();
                                var myHomeLeftPosition = ((HomeMenuFragment)this.mFragment).GetMyServiceItemLeftPosition("1008");

                                LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                                topLayoutParam.Height = topHeight + myHomeTopHeight;
                                topLayout.RequestLayout();

                                LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                                middleLayoutParam.Height = myHomeHeight;
                                middleLayout.RequestLayout();

                                LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                                highlightedLeftLayoutParam.Width = padding + myHomeLeftPosition;
                                highlightedLeftLayout.RequestLayout();

                                LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                                highlightedLayoutParam.Width = myHomeWidth;
                                highlightedLayout.RequestLayout();

                                LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;

                                highlightedRightLayoutParam.Width = screenWidth - (padding + myHomeLeftPosition + myHomeWidth);
                                highlightedRightLayout.RequestLayout();

                                LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                                bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                                bottomLayout.RequestLayout();

                                RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                                if (model.NeedHelpHide)
                                {
                                    innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(150f);
                                }
                                else
                                {
                                    innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(164f) : (int)DPUtils.ConvertDPToPx(132f);
                                }
                                innerTopLayoutParam.LeftMargin = padding;
                                innerTopLayoutParam.RightMargin = padding + (myHomeWidth / 2) - 8;
                                innerTopLayout.RequestLayout();
                            }
                            else if (model.Feature == FeatureType.NeedHelp)
                            {
                                int middleHeight = 0;
                                int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight() + (int)DPUtils.ConvertDPToPx(30f);
                                middleHeight = ((HomeMenuFragment)this.mFragment).GetnewFAQContainerHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight()) - (int)DPUtils.ConvertDPToPx(10f);
                                if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                                {
                                    topHeight = (int)DPUtils.ConvertDPToPx(TextViewUtils.IsLargeFonts ? 385f : 345f);
                                    topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                                }
                                if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                                {
                                    topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                                }
                                if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                                {
                                    int belowHeight = ((myTNB_Android.Src.myTNBMenu.Activity.DashboardHomeActivity)this.mContext).BottomNavigationViewHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                    middleHeight += ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
                                    topHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight) - middleHeight - ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
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
                                if (model.ItemCount > 1)
                                {
                                    int topHeight;
                                    if (UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify()
                                         && !MyTNBAccountManagement.GetInstance().COMCLandNEM())
                                    {
                                        topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight();
                                    }
                                    else
                                    {
                                        topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                    }
                                    //int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                    int middleHeight = (int)DPUtils.ConvertDPToPx(160f);

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
                                else
                                {
                                    int topHeight;
                                    if (UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify()
                                         && !MyTNBAccountManagement.GetInstance().COMCLandNEM())
                                    {
                                        topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight();
                                    }
                                    else
                                    {
                                        topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                    }
                                    //int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
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
                            }
                            else if (position == 1) //lala
                            {
                                int topHeight;
                                int middleHeight;

                                if (model.ItemCount == 3)
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                    middleHeight = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(255f) : (int)DPUtils.ConvertDPToPx(235f);
                                }
                                else if (model.ItemCount == 2)
                                {
                                    if (UserSessions.GetEnergyBudgetList().Count > 0 && MyTNBAccountManagement.GetInstance().IsEBUserVerify()
                                         && !MyTNBAccountManagement.GetInstance().COMCLandNEM())
                                    {
                                        topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight();
                                    }
                                    else
                                    {
                                        topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                    }
                                    middleHeight = (int)DPUtils.ConvertDPToPx(160f);
                                }
                                else if (model.ItemCount == 1)
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                    middleHeight = (int)DPUtils.ConvertDPToPx(120f);
                                }
                                else
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                    middleHeight = (int)DPUtils.ConvertDPToPx(118f);
                                }

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
                            else if (position == 2)
                            {
                                int topHeight;
                                if (model.ItemCount == 3)
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(20f) - ((HomeMenuFragment)this.mFragment).GetloadMoreContainerHeight();
                                }
                                else if (model.ItemCount == 2)
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(20f);
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
                                else if (model.ItemCount == 1)
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(27f) + ((HomeMenuFragment)this.mFragment).GetloadMoreContainerHeight();
                                }
                                else
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(27f);
                                }

                                LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                                topLayoutParam.Height = topHeight;
                                topLayout.RequestLayout();
                                LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                                middleLayoutParam.Height = ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight();
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
                                    innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(164f) : (int)DPUtils.ConvertDPToPx(132f);
                                }
                                innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                                innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                                innerTopLayout.RequestLayout();

                            }
                            else if (model.Feature == FeatureType.MyHome)
                            {
                                int topHeight;
                                if (model.ItemCount == 3)
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(20f) - ((HomeMenuFragment)this.mFragment).GetloadMoreContainerHeight();
                                }
                                else if (model.ItemCount == 2)
                                {
                                    //topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(20f);
                                    topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(20f) - ((HomeMenuFragment)this.mFragment).GetloadMoreContainerHeight();
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
                                else if (model.ItemCount == 1)
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(27f) + ((HomeMenuFragment)this.mFragment).GetloadMoreContainerHeight();
                                }
                                else
                                {
                                    topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(27f);
                                }

                                var padding = (int)DPUtils.ConvertDPToPx(16f);
                                var screenWidth = this.mContext.Resources.DisplayMetrics.WidthPixels;
                                var myHomeTopHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemTopPosition("1008");
                                var myHomeWidth = ((HomeMenuFragment)this.mFragment).GetMyServiceItemWidth();
                                var myHomeHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemHeight();
                                var myHomeLeftPosition = ((HomeMenuFragment)this.mFragment).GetMyServiceItemLeftPosition("1008");

                                LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                                topLayoutParam.Height = topHeight + myHomeTopHeight;
                                topLayout.RequestLayout();

                                LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                                middleLayoutParam.Height = myHomeHeight;
                                middleLayout.RequestLayout();

                                LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                                highlightedLeftLayoutParam.Width = padding + myHomeLeftPosition;
                                highlightedLeftLayout.RequestLayout();

                                LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                                highlightedLayoutParam.Width = myHomeWidth;
                                highlightedLayout.RequestLayout();

                                LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;

                                highlightedRightLayoutParam.Width = screenWidth - (padding + myHomeLeftPosition + myHomeWidth);
                                highlightedRightLayout.RequestLayout();

                                LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                                bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                                bottomLayout.RequestLayout();

                                RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                                if (model.NeedHelpHide)
                                {
                                    innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(150f);
                                }
                                else
                                {
                                    innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(164f) : (int)DPUtils.ConvertDPToPx(132f);
                                }
                                innerTopLayoutParam.LeftMargin = padding;
                                innerTopLayoutParam.RightMargin = padding + (myHomeWidth / 2) - 8;
                                innerTopLayout.RequestLayout();
                            }
                            else
                            {
                                if (model.ItemCount == 3)
                                {
                                    int middleHeight = 0;
                                    int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                                    middleHeight = ((HomeMenuFragment)this.mFragment).GetnewFAQContainerHeight() + (TextViewUtils.IsLargeFonts ? 20 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                    if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                                    {
                                        topHeight = (int)DPUtils.ConvertDPToPx(TextViewUtils.IsLargeFonts ? 385f : 345f);
                                        topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                                    }
                                    if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                                    {
                                        topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                                    }
                                    if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                                    {
                                        int belowHeight = ((myTNB_Android.Src.myTNBMenu.Activity.DashboardHomeActivity)this.mContext).BottomNavigationViewHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                        middleHeight += ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
                                        if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                        {
                                            belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                        }
                                        topHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight) - middleHeight - ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
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
                                else if (model.ItemCount == 2)
                                {
                                    int middleHeight = 0;
                                    int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight() + (int)DPUtils.ConvertDPToPx(30f);
                                    middleHeight = ((HomeMenuFragment)this.mFragment).GetnewFAQContainerHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                    if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                                    {
                                        topHeight = (int)DPUtils.ConvertDPToPx(TextViewUtils.IsLargeFonts ? 385f : 345f);
                                        topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                                    }
                                    if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                                    {
                                        topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                                    }
                                    if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                                    {
                                        int belowHeight = ((myTNB_Android.Src.myTNBMenu.Activity.DashboardHomeActivity)this.mContext).BottomNavigationViewHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                        middleHeight += ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
                                        if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                        {
                                            belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                        }
                                        topHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight) - middleHeight - ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
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
                                else if (model.ItemCount == 1)
                                {
                                    int middleHeight = 0;
                                    int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight() + (int)DPUtils.ConvertDPToPx(40f) + ((HomeMenuFragment)this.mFragment).GetloadMoreContainerHeight();
                                    middleHeight = ((HomeMenuFragment)this.mFragment).GetnewFAQContainerHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                    if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                                    {
                                        topHeight = (int)DPUtils.ConvertDPToPx(TextViewUtils.IsLargeFonts ? 385f : 345f);
                                        topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                                    }
                                    if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                                    {
                                        topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                                    }
                                    if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                                    {
                                        int belowHeight = ((myTNB_Android.Src.myTNBMenu.Activity.DashboardHomeActivity)this.mContext).BottomNavigationViewHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                        middleHeight += ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
                                        if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                        {
                                            belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                        }
                                        topHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight) - middleHeight - ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();

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
                                else
                                {
                                    int middleHeight = 0;
                                    int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight() + ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight() + ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight() + (int)DPUtils.ConvertDPToPx(9f);
                                    middleHeight = ((HomeMenuFragment)this.mFragment).GetnewFAQContainerHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                    if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                                    {
                                        topHeight = (int)DPUtils.ConvertDPToPx(TextViewUtils.IsLargeFonts ? 385f : 345f);
                                        topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                                    }
                                    if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                                    {
                                        topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                                    }
                                    if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                                    {
                                        int belowHeight = ((myTNB_Android.Src.myTNBMenu.Activity.DashboardHomeActivity)this.mContext).BottomNavigationViewHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                        middleHeight += ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
                                        if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                        {
                                            belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                        }
                                        topHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight) - middleHeight - ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
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
                    }
                }
                else if (this.mFragment is HomeMenuFragment)
                {
                    if (model.ItemCount == 0) //NO ELECTRICITY ACCOUNT
                    {
                        if (model.Feature == FeatureType.Accounts)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
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
                        else if (model.Feature == FeatureType.QuickActions)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(27f);

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight();
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
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(150f);
                            }
                            else
                            {
                                innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(144f) : (int)DPUtils.ConvertDPToPx(122f);
                            }
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else if (model.Feature == FeatureType.MyHome)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(27f);

                            var padding = (int)DPUtils.ConvertDPToPx(16f);
                            var screenWidth = this.mContext.Resources.DisplayMetrics.WidthPixels;
                            var myHomeTopHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemTopPosition("1008");
                            var myHomeWidth = ((HomeMenuFragment)this.mFragment).GetMyServiceItemWidth();
                            var myHomeHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemHeight();
                            var myHomeLeftPosition = ((HomeMenuFragment)this.mFragment).GetMyServiceItemLeftPosition("1008");

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight + myHomeTopHeight;
                            topLayout.RequestLayout();

                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = myHomeHeight;
                            middleLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = padding + myHomeLeftPosition;
                            highlightedLeftLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = myHomeWidth;
                            highlightedLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;

                            highlightedRightLayoutParam.Width = screenWidth - (padding + myHomeLeftPosition + myHomeWidth);
                            highlightedRightLayout.RequestLayout();

                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            if (model.NeedHelpHide)
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(150f);
                            }
                            else
                            {
                                innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(164f) : (int)DPUtils.ConvertDPToPx(132f);
                            }
                            innerTopLayoutParam.LeftMargin = padding;
                            innerTopLayoutParam.RightMargin = padding + (myHomeWidth / 2) - 8;
                            innerTopLayout.RequestLayout();
                        }
                        else if (model.Feature == FeatureType.NeedHelp)
                        {
                            int middleHeight = 0;
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight() + ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight() + ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight() + (int)DPUtils.ConvertDPToPx(9f);
                            middleHeight = ((HomeMenuFragment)this.mFragment).GetnewFAQContainerHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = (int)DPUtils.ConvertDPToPx(TextViewUtils.IsLargeFonts ? 385f : 345f);
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = ((myTNB_Android.Src.myTNBMenu.Activity.DashboardHomeActivity)this.mContext).BottomNavigationViewHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                middleHeight += ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
                                if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                {
                                    belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                }
                                topHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight) - middleHeight - ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
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
                    else if (model.ItemCount == 1) //ONE ELECTRICITY ACCOUNT
                    {
                        if (model.Feature == FeatureType.Accounts)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
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
                        else if (model.Feature == FeatureType.QuickActions)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(27f);

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight;
                            topLayout.RequestLayout();
                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight();
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
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(150f);
                            }
                            else
                            {
                                innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(144f) : (int)DPUtils.ConvertDPToPx(122f);
                            }
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else if (model.Feature == FeatureType.MyHome)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(27f);

                            var padding = (int)DPUtils.ConvertDPToPx(16f);
                            var screenWidth = this.mContext.Resources.DisplayMetrics.WidthPixels;
                            var myHomeTopHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemTopPosition("1008");
                            var myHomeWidth = ((HomeMenuFragment)this.mFragment).GetMyServiceItemWidth();
                            var myHomeHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemHeight();
                            var myHomeLeftPosition = ((HomeMenuFragment)this.mFragment).GetMyServiceItemLeftPosition("1008");

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight + myHomeTopHeight;
                            topLayout.RequestLayout();

                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = myHomeHeight;
                            middleLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = padding + myHomeLeftPosition;
                            highlightedLeftLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = myHomeWidth;
                            highlightedLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;

                            highlightedRightLayoutParam.Width = screenWidth - (padding + myHomeLeftPosition + myHomeWidth);
                            highlightedRightLayout.RequestLayout();

                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            if (model.NeedHelpHide)
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(150f);
                            }
                            else
                            {
                                innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(164f) : (int)DPUtils.ConvertDPToPx(132f);
                            }
                            innerTopLayoutParam.LeftMargin = padding;
                            innerTopLayoutParam.RightMargin = padding + (myHomeWidth / 2) - 8;
                            innerTopLayout.RequestLayout();
                        }
                        else if (model.Feature == FeatureType.NeedHelp)
                        {
                            int middleHeight = 0;
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight() + ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight() + ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight() + (int)DPUtils.ConvertDPToPx(9f);
                            middleHeight = ((HomeMenuFragment)this.mFragment).GetnewFAQContainerHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = (int)DPUtils.ConvertDPToPx(TextViewUtils.IsLargeFonts ? 385f : 345f);
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = ((myTNB_Android.Src.myTNBMenu.Activity.DashboardHomeActivity)this.mContext).BottomNavigationViewHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                middleHeight += ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
                                if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                {
                                    belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                }
                                topHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight) - middleHeight - ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
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
                    else if (model.ItemCount == 2) //TWO ELECTRICITY ACCOUNTS
                    {
                        if (model.Feature == FeatureType.Accounts)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                            int middleHeight = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(195f) : (int)DPUtils.ConvertDPToPx(175f);
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
                        else if (model.Feature == FeatureType.QuickActions)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(20f);
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
                            middleLayoutParam.Height = ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight();
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
                                innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(144f) : (int)DPUtils.ConvertDPToPx(122f);
                            }
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else if (model.Feature == FeatureType.MyHome)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(20f) - ((HomeMenuFragment)this.mFragment).GetloadMoreContainerHeight();
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

                            var padding = (int)DPUtils.ConvertDPToPx(16f);
                            var screenWidth = this.mContext.Resources.DisplayMetrics.WidthPixels;
                            var myHomeTopHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemTopPosition("1008");
                            var myHomeWidth = ((HomeMenuFragment)this.mFragment).GetMyServiceItemWidth();
                            var myHomeHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemHeight();
                            var myHomeLeftPosition = ((HomeMenuFragment)this.mFragment).GetMyServiceItemLeftPosition("1008");

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight + myHomeTopHeight;
                            topLayout.RequestLayout();

                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = myHomeHeight;
                            middleLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = padding + myHomeLeftPosition;
                            highlightedLeftLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = myHomeWidth;
                            highlightedLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;

                            highlightedRightLayoutParam.Width = screenWidth - (padding + myHomeLeftPosition + myHomeWidth);
                            highlightedRightLayout.RequestLayout();

                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            if (model.NeedHelpHide)
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(150f);
                            }
                            else
                            {
                                innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(164f) : (int)DPUtils.ConvertDPToPx(132f);
                            }
                            innerTopLayoutParam.LeftMargin = padding;
                            innerTopLayoutParam.RightMargin = padding + (myHomeWidth / 2) - 8;
                            innerTopLayout.RequestLayout();
                        }
                        else if (model.Feature == FeatureType.NeedHelp)
                        {
                            int middleHeight = 0;
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight() + (int)DPUtils.ConvertDPToPx(30f);
                            middleHeight = ((HomeMenuFragment)this.mFragment).GetnewFAQContainerHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = (int)DPUtils.ConvertDPToPx(TextViewUtils.IsLargeFonts ? 385f : 345f);
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = ((myTNB_Android.Src.myTNBMenu.Activity.DashboardHomeActivity)this.mContext).BottomNavigationViewHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                middleHeight += ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
                                if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                {
                                    belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                }
                                topHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight) - middleHeight - ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
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
                    else if (model.ItemCount == 3) //THREE ELECTRICITY ACCOUNTS
                    {
                        if (model.Feature == FeatureType.Accounts)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                            int middleHeight = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(255f) : (int)DPUtils.ConvertDPToPx(235f);
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
                        else if (model.Feature == FeatureType.QuickActions)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(20f) - ((HomeMenuFragment)this.mFragment).GetloadMoreContainerHeight();
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
                            middleLayoutParam.Height = ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight();
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
                                innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(144f) : (int)DPUtils.ConvertDPToPx(122f);
                            }
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else if (model.Feature == FeatureType.MyHome)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(20f) - ((HomeMenuFragment)this.mFragment).GetloadMoreContainerHeight();
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

                            var padding = (int)DPUtils.ConvertDPToPx(16f);
                            var screenWidth = this.mContext.Resources.DisplayMetrics.WidthPixels;
                            var myHomeTopHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemTopPosition("1008");
                            var myHomeWidth = ((HomeMenuFragment)this.mFragment).GetMyServiceItemWidth();
                            var myHomeHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemHeight();
                            var myHomeLeftPosition = ((HomeMenuFragment)this.mFragment).GetMyServiceItemLeftPosition("1008");

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight + myHomeTopHeight;
                            topLayout.RequestLayout();

                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = myHomeHeight;
                            middleLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = padding + myHomeLeftPosition;
                            highlightedLeftLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = myHomeWidth;
                            highlightedLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;

                            highlightedRightLayoutParam.Width = screenWidth - (padding + myHomeLeftPosition + myHomeWidth);
                            highlightedRightLayout.RequestLayout();

                            LinearLayout.LayoutParams bottomLayoutParam = bottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            bottomLayoutParam.Height = ViewGroup.LayoutParams.MatchParent;
                            bottomLayout.RequestLayout();

                            RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                            if (model.NeedHelpHide)
                            {
                                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(150f);
                            }
                            else
                            {
                                innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(164f) : (int)DPUtils.ConvertDPToPx(132f);
                            }
                            innerTopLayoutParam.LeftMargin = padding;
                            innerTopLayoutParam.RightMargin = padding + (myHomeWidth / 2) - 8;
                            innerTopLayout.RequestLayout();
                        }
                        else if (model.Feature == FeatureType.NeedHelp)
                        {
                            int middleHeight = 0;
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight() + (int)DPUtils.ConvertDPToPx(35f);
                            middleHeight = ((HomeMenuFragment)this.mFragment).GetnewFAQContainerHeight() + (TextViewUtils.IsLargeFonts ? 20 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = (int)DPUtils.ConvertDPToPx(TextViewUtils.IsLargeFonts ? 385f : 345f);
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = ((myTNB_Android.Src.myTNBMenu.Activity.DashboardHomeActivity)this.mContext).BottomNavigationViewHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                middleHeight += ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
                                if (!((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible() && this.mContext.Resources.DisplayMetrics.HeightPixels <= 800)
                                {
                                    belowHeight = (int)DPUtils.ConvertDPToPx(135);
                                }
                                topHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight) - middleHeight - ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
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
                    else //MORE THAN THREE ELECTRICITY ACCOUNTS
                    {
                        if (model.Feature == FeatureType.Accounts)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(25f);
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
                        else if (model.Feature == FeatureType.QuickAccess)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GetAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(27f);
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
                            middleLayoutParam.Height = (int)DPUtils.ConvertDPToPx(50f);
                            middleLayout.RequestLayout();

                            int deviceWidth = this.mContext.Resources.DisplayMetrics.WidthPixels;
                            int rightAreaWidth = (int)DPUtils.ConvertDPToPx(8f);
                            int middleAreaWidth = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(160f) : (int)DPUtils.ConvertDPToPx(140f);
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
                        else if (model.Feature == FeatureType.QuickActions)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(10f);
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
                            middleLayoutParam.Height = ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight();
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
                                innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(164f) : (int)DPUtils.ConvertDPToPx(132f);
                            }
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else if (model.Feature == FeatureType.MyHome)
                        {
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + (int)DPUtils.ConvertDPToPx(10f);
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

                            var padding = (int)DPUtils.ConvertDPToPx(16f);
                            var screenWidth = this.mContext.Resources.DisplayMetrics.WidthPixels;
                            var myHomeTopHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemTopPosition("1008");
                            var myHomeWidth = ((HomeMenuFragment)this.mFragment).GetMyServiceItemWidth();
                            var myHomeHeight = ((HomeMenuFragment)this.mFragment).GetMyServiceItemHeight();
                            var myHomeLeftPosition = ((HomeMenuFragment)this.mFragment).GetMyServiceItemLeftPosition("1008");

                            LinearLayout.LayoutParams topLayoutParam = topLayout.LayoutParameters as LinearLayout.LayoutParams;
                            topLayoutParam.Height = topHeight + myHomeTopHeight;
                            topLayout.RequestLayout();

                            LinearLayout.LayoutParams middleLayoutParam = middleLayout.LayoutParameters as LinearLayout.LayoutParams;
                            middleLayoutParam.Height = myHomeHeight;
                            middleLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedLeftLayoutParam = highlightedLeftLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLeftLayoutParam.Width = padding + myHomeLeftPosition;
                            highlightedLeftLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedLayoutParam = highlightedLayout.LayoutParameters as LinearLayout.LayoutParams;
                            highlightedLayoutParam.Width = myHomeWidth;
                            highlightedLayout.RequestLayout();

                            LinearLayout.LayoutParams highlightedRightLayoutParam = highlightedRightLayout.LayoutParameters as LinearLayout.LayoutParams;

                            highlightedRightLayoutParam.Width = screenWidth - (padding + myHomeLeftPosition + myHomeWidth);
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
                                innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(164f) : (int)DPUtils.ConvertDPToPx(132f);
                            }
                            innerTopLayoutParam.LeftMargin = padding;
                            innerTopLayoutParam.RightMargin = padding + (myHomeWidth / 2) - 8;
                            innerTopLayout.RequestLayout();
                        }
                        else if (model.Feature == FeatureType.NeedHelp)
                        {
                            int middleHeight = 0;
                            int topHeight = ((HomeMenuFragment)this.mFragment).GettopRootViewHeight() + ((HomeMenuFragment)this.mFragment).GetMyServiceContainerHeight() + (int)DPUtils.ConvertDPToPx(30f);
                            middleHeight = ((HomeMenuFragment)this.mFragment).GetnewFAQContainerHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight()) - (int)DPUtils.ConvertDPToPx(10f);
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = (int)DPUtils.ConvertDPToPx(TextViewUtils.IsLargeFonts ? 385f : 345f);
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                            if (((HomeMenuFragment)this.mFragment).IsMyServiceLoadMoreVisible())
                            {
                                topHeight = topHeight + (int)DPUtils.ConvertDPToPx(38f);
                            }
                            if (((HomeMenuFragment)this.mFragment).CheckIsScrollable())
                            {
                                int belowHeight = ((myTNB_Android.Src.myTNBMenu.Activity.DashboardHomeActivity)this.mContext).BottomNavigationViewHeight() + (TextViewUtils.IsLargeFonts ? 0 : ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight());
                                middleHeight += ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
                                topHeight = (this.mContext.Resources.DisplayMetrics.HeightPixels - belowHeight) - middleHeight - ((HomeMenuFragment)this.mFragment).GetnewFAQTitleHeight();
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
                    if (list.Count == 3)
                    {
                        if (position == 0)
                        {
                            float h1 = 55f;
                            int topHeight = (int)DPUtils.ConvertDPToPx(h1);
                            int middleHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetchargeAvailableNoCTAContainerHeight();
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
                            int topHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetchargeAvailableNoCTAContainerHeight() + ((ItemisedBillingMenuFragment)this.mFragment).GetButtonHeight() + (int)DPUtils.ConvertDPToPx(85f);
                            int middleHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetDigitalContainerHeight() - (int)DPUtils.ConvertDPToPx(5f);
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
                                        float h2 = TextViewUtils.IsLargeFonts ? 2f : 20f;
                                        int diff = (topHeight + middleHeight) - (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)) + (int)DPUtils.ConvertDPToPx(h2);
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
                            innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(130f);
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else
                        {
                            int topHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetchargeAvailableNoCTAContainerHeight() + ((ItemisedBillingMenuFragment)this.mFragment).GetButtonHeight() + ((ItemisedBillingMenuFragment)this.mFragment).GetDigitalContainerHeight() + (TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(85f) : (int)DPUtils.ConvertDPToPx(75f));
                            int middleHeight = (int)DPUtils.ConvertDPToPx(130f);


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
                                        float h2 = TextViewUtils.IsLargeFonts ? 20f : 20f;
                                        int diff = (topHeight + middleHeight) - (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)) + (int)DPUtils.ConvertDPToPx(h2);
                                        topHeight = (topHeight + +((ItemisedBillingMenuFragment)this.mFragment).GetDigitalContainerHeight()) - diff;
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
                    else if (list.Count == 2)
                    {
                        if (position == 0)
                        {
                            float h1 = 55f;
                            int topHeight = (int)DPUtils.ConvertDPToPx(h1);
                            int middleHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetchargeAvailableNoCTAContainerHeight();
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
                            int topHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetchargeAvailableNoCTAContainerHeight() + ((ItemisedBillingMenuFragment)this.mFragment).GetButtonHeight() + ((ItemisedBillingMenuFragment)this.mFragment).GetDigitalContainerHeight() + (TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(85f) : (int)DPUtils.ConvertDPToPx(75f));
                            int middleHeight = (int)DPUtils.ConvertDPToPx(130f);


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
                                        float h2 = TextViewUtils.IsLargeFonts ? 20f : 20f;
                                        int diff = (topHeight + middleHeight) - (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)) + (int)DPUtils.ConvertDPToPx(h2);
                                        topHeight = (topHeight + +((ItemisedBillingMenuFragment)this.mFragment).GetDigitalContainerHeight()) - diff;
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
                            float h1 = 55f;
                            int topHeight = (int)DPUtils.ConvertDPToPx(h1);
                            int middleHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetchargeAvailableNoCTAContainerHeight();
                            int checkPoint = (int)DPUtils.ConvertDPToPx(200f);

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
                            innerUpperBottomLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(45f) : (int)DPUtils.ConvertDPToPx(40f);
                            innerUpperBottomLayoutParam.LeftMargin = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(8f) : (int)DPUtils.ConvertDPToPx(8f);
                            innerUpperBottomLayoutParam.RightMargin = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(0f) : (int)DPUtils.ConvertDPToPx(8f);
                            innerUpperBottomLayout.LayoutParameters = innerUpperBottomLayoutParam;
                            innerUpperBottomLayout.RequestLayout();

                            LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                            innerTxtBtnBottomLayoutParam.LeftMargin = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(0f) : (int)DPUtils.ConvertDPToPx(8f);
                            innerTxtBtnBottomLayoutParam.RightMargin = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(0f) : (int)DPUtils.ConvertDPToPx(8f);
                            innerTxtBtnBottomLayout.RequestLayout();
                        }
                        else if (position == 1)
                        {
                            int topHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetchargeAvailableNoCTAContainerHeight() + (int)DPUtils.ConvertDPToPx(55f);
                            int middleHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetButtonHeight() + (int)DPUtils.ConvertDPToPx(8f);

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
                            int topHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetchargeAvailableNoCTAContainerHeight() + (int)DPUtils.ConvertDPToPx(55f);
                            int middleHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetButtonHeight() + (int)DPUtils.ConvertDPToPx(8f);

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
                        else if (((ItemisedBillingMenuFragment)this.mFragment).IsDigitalContainerVisible() && position == 3)
                        {
                            int topHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetchargeAvailableNoCTAContainerHeight() + ((ItemisedBillingMenuFragment)this.mFragment).GetButtonHeight() + (int)DPUtils.ConvertDPToPx(95f);
                            int middleHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetDigitalContainerHeight() - (int)DPUtils.ConvertDPToPx(5f);

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
                                        //int diff = (topHeight + middleHeight) - (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)) + (int)DPUtils.ConvertDPToPx(20f);
                                        float h2 = TextViewUtils.IsLargeFonts ? 2f : 20f;
                                        int diff = (topHeight + middleHeight) - (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)) + (int)DPUtils.ConvertDPToPx(h2);
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
                            innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(130f);
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                            innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayout.RequestLayout();
                        }
                        else if ((((ItemisedBillingMenuFragment)this.mFragment)._isBillStatement && position == 4 && ((ItemisedBillingMenuFragment)this.mFragment).IsDigitalContainerVisible()) || (((ItemisedBillingMenuFragment)this.mFragment)._isBillStatement && position == 3))
                        {
                            int topHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetchargeAvailableNoCTAContainerHeight() + ((ItemisedBillingMenuFragment)this.mFragment).GetButtonHeight() + ((ItemisedBillingMenuFragment)this.mFragment).GetDigitalContainerHeight() + (int)DPUtils.ConvertDPToPx(95f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(35f);
                            int middleWidth = (int)DPUtils.ConvertDPToPx(35f);
                            float h2 = 39f;
                            int rightWidth = (int)DPUtils.ConvertDPToPx(h2);

                            int leftWidth = (this.mContext.Resources.DisplayMetrics.WidthPixels - rightWidth - middleWidth);// (int)DPUtils.ConvertDPToPx(h1);

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
                            var margin = rightWidth + (int)DPUtils.ConvertDPToPx((35f / 2) - 4);
                            var msgHeight = TextViewUtils.IsLargeFonts ? 220f : 150f;
                            innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(msgHeight);
                            innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(0f);
                            innerTopLayoutParam.RightMargin = margin;
                            innerTopLayout.RequestLayout();
                        }
                        else
                        {
                            int topHeight = ((ItemisedBillingMenuFragment)this.mFragment).GetchargeAvailableNoCTAContainerHeight() + ((ItemisedBillingMenuFragment)this.mFragment).GetButtonHeight() + ((ItemisedBillingMenuFragment)this.mFragment).GetDigitalContainerHeight() + (int)DPUtils.ConvertDPToPx(85f);
                            int middleHeight = (int)DPUtils.ConvertDPToPx(130f);

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
                                        float h2 = TextViewUtils.IsLargeFonts ? 2f : 20f;
                                        int diff = (topHeight + middleHeight) - (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)) + (int)DPUtils.ConvertDPToPx(h2);
                                        topHeight -= diff;
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
            else if (this.mContext is DashboardHomeActivity)
            {
                int middleHeight = ((DashboardHomeActivity)this.mContext).GetViewBillButtonHeight() + (int)DPUtils.ConvertDPToPx(15f);
                int topHeight = ((DashboardHomeActivity)this.mContext).GetTopHeight() - (int)DPUtils.ConvertDPToPx(7f);


                int rightWidth = (int)DPUtils.ConvertDPToPx(5f);
                int middleWidth = (int)DPUtils.ConvertDPToPx(40f);
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

                LinearLayout.LayoutParams rightInnerUpperBottomLineLayoutParam = rightInnerUpperBottomLineLayout.LayoutParameters as LinearLayout.LayoutParams;
                rightInnerUpperBottomLineLayoutParam.Height = (int)DPUtils.ConvertDPToPx(70f); ;
                rightInnerUpperBottomLineLayout.RequestLayout();

                RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(160f);
                innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                innerTopLayout.RequestLayout();


                LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(70f);
                innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(50f);
                innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(20f);
                innerUpperBottomLayout.RequestLayout();

                /* LinearLayout.LayoutParams innerTxtBtnBottomLayoutParam = innerTxtBtnBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                 innerTxtBtnBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(70f);
                 innerTxtBtnBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(50f);
                 innerTxtBtnBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(60f);
                 innerTxtBtnBottomLayout.RequestLayout();*/


            }
            else if (this.mContext is ManageAccessActivity)
            {
                if (list.Count == 2)
                {
                    if (position == 0)
                    {
                        int middleHeight = ((ManageAccessActivity)this.mContext).GetViewBillButtonHeight() + (int)DPUtils.ConvertDPToPx(15f);
                        int topHeight = ((ManageAccessActivity)this.mContext).GetTopHeight() - (int)DPUtils.ConvertDPToPx(7f);


                        int rightWidth = (int)DPUtils.ConvertDPToPx(5f);
                        int middleWidth = (int)DPUtils.ConvertDPToPx(40f);
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

                        LinearLayout.LayoutParams rightInnerUpperBottomLineLayoutParam = rightInnerUpperBottomLineLayout.LayoutParameters as LinearLayout.LayoutParams;
                        rightInnerUpperBottomLineLayoutParam.Height = (int)DPUtils.ConvertDPToPx(70f); ;
                        rightInnerUpperBottomLineLayout.RequestLayout();

                        RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(160f);
                        innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                        innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTopLayout.RequestLayout();


                        LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(70f);
                        innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(50f);
                        innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(20f);
                        innerUpperBottomLayout.RequestLayout();
                    }
                    else
                    {
                        int topHeight = (((ManageAccessActivity)this.mContext).GetViewBillButtonHeight()) + (int)DPUtils.ConvertDPToPx(30f);
                        int middleHeight = (((ManageAccessActivity)this.mContext).OnGetEndOfScrollView());

                        if (model.ItemCount == 0)
                        {
                            if ((topHeight + middleHeight) > (this.mContext.Resources.DisplayMetrics.HeightPixels - (int)DPUtils.ConvertDPToPx(62f)))
                            {
                                if (((ManageAccessActivity)this.mContext).CheckIsScrollable())
                                {
                                    int bottomHeight = (int)DPUtils.ConvertDPToPx(90f);
                                    topHeight = this.mContext.Resources.DisplayMetrics.HeightPixels - bottomHeight - middleHeight;
                                }
                            }
                        }
                        else if (model.ItemCount == 1)
                        {
                            middleHeight = (((ManageAccessActivity)this.mContext).OnGetEndOfScrollView());
                        }
                        else if (model.ItemCount == 2)
                        {
                            middleHeight = (((ManageAccessActivity)this.mContext).OnGetEndOfScrollView2());
                        }
                        else
                        {
                            middleHeight = (((ManageAccessActivity)this.mContext).OnGetEndOfScrollView2());
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

                        //LinearLayout.LayoutParams leftInnerUpperBottomLineLayoutParam = leftInnerUpperBottomLineLayout.LayoutParameters as LinearLayout.LayoutParams;
                        //leftInnerUpperBottomLineLayoutParam.Height = (int)DPUtils.ConvertDPToPx(70f); ;
                        //leftInnerUpperBottomLineLayout.RequestLayout();

                        //LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        //innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(70f);
                        //innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(50f);
                        //innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(20f);
                        //innerUpperBottomLayout.RequestLayout();



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
                }
                else
                {
                    if (list.Count == 1)
                    {
                        int middleHeight = ((ManageAccessActivity)this.mContext).GetViewBillButtonHeight() + (int)DPUtils.ConvertDPToPx(15f);
                        int topHeight = ((ManageAccessActivity)this.mContext).GetTopHeight() - (int)DPUtils.ConvertDPToPx(7f);


                        int rightWidth = (int)DPUtils.ConvertDPToPx(5f);
                        int middleWidth = (int)DPUtils.ConvertDPToPx(40f);
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

                        LinearLayout.LayoutParams rightInnerUpperBottomLineLayoutParam = rightInnerUpperBottomLineLayout.LayoutParameters as LinearLayout.LayoutParams;
                        rightInnerUpperBottomLineLayoutParam.Height = (int)DPUtils.ConvertDPToPx(70f); ;
                        rightInnerUpperBottomLineLayout.RequestLayout();

                        RelativeLayout.LayoutParams innerTopLayoutParam = innerTopLayout.LayoutParameters as RelativeLayout.LayoutParams;
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(160f);
                        innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                        innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTopLayout.RequestLayout();


                        LinearLayout.LayoutParams innerUpperBottomLayoutParam = innerUpperBottomLayout.LayoutParameters as LinearLayout.LayoutParams;
                        innerUpperBottomLayoutParam.Height = (int)DPUtils.ConvertDPToPx(70f);
                        innerUpperBottomLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(50f);
                        innerUpperBottomLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(20f);
                        innerUpperBottomLayout.RequestLayout();
                    }
                }
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
                    int middleHeight = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(220f) : (int)DPUtils.ConvertDPToPx(200f);

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
                        int middleHeight = ((ApplicationStatusLandingActivity)this.mContext).GetSearchButtonHeight() + (int)DPUtils.ConvertDPToPx(35f);
                        int topHeight = ((ApplicationStatusLandingActivity)this.mContext).GetTopSearchHeight() + (int)DPUtils.ConvertDPToPx(-15f);

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
                        innerTopLayoutParam.Height = TextViewUtils.IsLargeFonts ? (int)DPUtils.ConvertDPToPx(250f) : (int)DPUtils.ConvertDPToPx(110f);
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
                    int topHeight;
                    int middleHeight;
                    if (((ApplicationStatusDetailActivity)this.mContext).GetRecyclerViewHeight() != 0)
                    {
                        topHeight = ((ApplicationStatusDetailActivity)this.mContext).GetHighlightedHeight() - (int)DPUtils.ConvertDPToPx(60);
                        middleHeight = (int)DPUtils.ConvertDPToPx(185);
                    }
                    else
                    {
                        topHeight = ((ApplicationStatusDetailActivity)this.mContext).GetHighlightedHeight() - (int)DPUtils.ConvertDPToPx(-80);
                        middleHeight = (int)DPUtils.ConvertDPToPx(170);
                    }



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
                    float innerHeight = TextViewUtils.IsLargeFonts ? 130f : 100f;
                    innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(innerHeight);
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
            else if (this.mContext is ManageSupplyAccountActivityEdit)
            {

                int middleHeight = ((ManageSupplyAccountActivityEdit)this.mContext).GetLayoutManageBillHeight();
                int topHeight = ((ManageSupplyAccountActivityEdit)this.mContext).GetAccountLayoutHeight() + ((ManageSupplyAccountActivityEdit)this.mContext).GetLayoutNickNameHeight() + (int)DPUtils.ConvertDPToPx(85f);
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
                innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(130f);
                innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                innerTopLayout.RequestLayout();

            }
            else if (DBRUtility.Instance.IsAccountEligible && this.mContext is ManageBillDeliveryActivity)
            {
                int middleHeight = 0;
                int topHeight = 0;
                if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.Email)
                {
                    float h1 = 130f;
                    float h2 = TextViewUtils.IsLargeFonts ? 110 : 105f;
                    middleHeight = (int)DPUtils.ConvertDPToPx(h2);
                    topHeight = ((ManageBillDeliveryActivity)this.mContext).GetEmail_layoutrHeight() + ((ManageBillDeliveryActivity)this.mContext).GetdigitalBillLabelHeight() +
                                             ((ManageBillDeliveryActivity)this.mContext).GetSelectAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(h1);
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
                    innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(130f);
                    innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                    innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                    innerTopLayout.RequestLayout();
                }
                else if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.EBillWithCTA)
                {
                    middleHeight = ((ManageBillDeliveryActivity)this.mContext).GetBtnUpdateDigitalBillHeight() + (int)DPUtils.ConvertDPToPx(20f);
                    topHeight = ((ManageBillDeliveryActivity)this.mContext).GetTopHeight();
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
                    innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(130f);
                    innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                    innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                    innerTopLayout.RequestLayout();
                }
                else if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.EmailWithCTA)
                {
                    if (position == 0)
                    {
                        float h1 = 130f;
                        float h2 = TextViewUtils.IsLargeFonts ? 110 : 105f;
                        middleHeight = (int)DPUtils.ConvertDPToPx(h2); ;
                        topHeight = ((ManageBillDeliveryActivity)this.mContext).GetEmail_layoutrHeight() + ((ManageBillDeliveryActivity)this.mContext).GetdigitalBillLabelHeight() +
                                                 ((ManageBillDeliveryActivity)this.mContext).GetSelectAccountContainerHeight() + (int)DPUtils.ConvertDPToPx(h1);

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
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(130f);
                        innerTopLayoutParam.LeftMargin = (int)DPUtils.ConvertDPToPx(32f);
                        innerTopLayoutParam.RightMargin = (int)DPUtils.ConvertDPToPx(0f);
                        innerTopLayout.RequestLayout();
                    }
                    else
                    {
                        middleHeight = ((ManageBillDeliveryActivity)this.mContext).GetBtnUpdateDigitalBillHeight() + (int)DPUtils.ConvertDPToPx(20f);
                        topHeight = ((ManageBillDeliveryActivity)this.mContext).GetTopHeight();
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
                        innerTopLayoutParam.Height = (int)DPUtils.ConvertDPToPx(130f);
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
                if (this.mContext is ManageSupplyAccountActivityEdit)
                {
                    UserSessions.DoManageSupplyAccountTutorialShown(this.mPref);
                }
                else if (this.mContext is ManageBillDeliveryActivity)
                {
                    if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.EBill)
                    {
                        UserSessions.DoManageEBillDeliveryTutorialShown(this.mPref);
                    }
                    else if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.EBillWithCTA)
                    {
                        UserSessions.DoManagepoptedEBillDeliveryTutorialShown(this.mPref);
                    }
                    else if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.Email)
                    {
                        UserSessions.DoManageEmailBillDeliveryTutorialShown(this.mPref);
                    }
                    else if (UserSessions.ManageBillDelivery == MobileEnums.DBRTypeEnum.EmailWithCTA)
                    {
                        UserSessions.DoManageParallelEmailBillDeliveryTutorialShown(this.mPref);
                    }
                }
            }
        }

        ////yana
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                Intent nextIntent = new Intent(this.mContext, typeof(MyAccountActivity));
                nextIntent.PutExtra("fromDashboard", true);
                this.mContext.StartActivityForResult(nextIntent, Constants.MANAGE_SUPPLY_ACCOUNT_REQUEST);
                UserSessions.DoHomeTutorialShown(this.mPref);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
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