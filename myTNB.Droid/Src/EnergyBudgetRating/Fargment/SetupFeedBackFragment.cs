using System;
using AFollestad.MaterialDialogs;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Text;
using Android.Graphics;
using AndroidX.RecyclerView.Widget;
using AndroidX.Core.Content;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.EnergyBudgetRating.Model;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.EnergyBudgetRating.Adapter;
using myTNB.AndroidApp.Src.Base;

namespace myTNB.AndroidApp.Src.EnergyBudgetRating.Fargment
{
    public class SetupFeedBackFragment
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
            FEEDBACK_WITH_IMAGES_STAR_RATING_BUTTON,
            NORMAL_WITH_THREE_BUTTON,
        }

        private ToolTipType toolTipType;
        private int imageResource;
        private string title;
        private string titleOtherOne;
        private string titleOtherTwo;
        private string titleOtherThree;
        private string message;
        private string ctaLabel;
        private string secondaryCTALabel;
        private RecyclerView.Adapter adapter;
        private Action ctaAction;
        private Action ctaYesAction;
        private Action ctaNoAction;
        private Action secondaryCTAAction;
        private MaterialDialog dialog;
        private Color mClickSpanColor;
        private Typeface mTypeface;
        private Android.App.Activity mContext;
        private GravityFlags mGravityFlag;
        private Bitmap imageResourceBitmap;
        private List<ImproveSelectModel> SelectStarPosition = new List<ImproveSelectModel>();
        private GridLayoutManager layoutManager;
        private RateUsStarsCustomAdapter adaptercustom;
        private int countClick = 0;
        private SetupFeedBackFragment(Android.App.Activity context)
        {
            this.mContext = context;
        }

        public static SetupFeedBackFragment Create(Android.App.Activity context, ToolTipType mToolTipType)
        {
            SetupFeedBackFragment tooltipBuilder = new SetupFeedBackFragment(context);
            tooltipBuilder.toolTipType = mToolTipType;
            tooltipBuilder.mGravityFlag = GravityFlags.Left;
            int layoutResource = 0;
            if (mToolTipType == ToolTipType.IMAGE_HEADER)
            {
                layoutResource = Resource.Layout.CustomDialogWithImageHeaderEBFeedback;
            }
            else if (mToolTipType == ToolTipType.NORMAL_WITH_HEADER)
            {
                layoutResource = Resource.Layout.CustomToolTipWithHeaderLayout;
            }
            else if (mToolTipType == ToolTipType.LISTVIEW_WITH_INDICATOR_AND_HEADER)
            {
                layoutResource = Resource.Layout.CustomDialogWithListViewLayout;
            }
            else if (mToolTipType == ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
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
            else if (mToolTipType == ToolTipType.FEEDBACK_WITH_IMAGES_STAR_RATING_BUTTON)
            {
                layoutResource = Resource.Layout.CustomDialogWithStarRatingLayout;
            }
            else if (mToolTipType == ToolTipType.NORMAL_WITH_THREE_BUTTON)
            {
                layoutResource = Resource.Layout.CustomDialogThreeButtonLayout;
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
                if (mToolTipType != ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON && mToolTipType != ToolTipType.NORMAL_WITH_HEADER)
                {
                    WindowManagerLayoutParams wlp = tooltipBuilder.dialog.Window.Attributes;
                    wlp.Gravity = GravityFlags.Center;
                    wlp.Width = ViewGroup.LayoutParams.MatchParent;
                    wlp.Height = ViewGroup.LayoutParams.WrapContent;
                    tooltipBuilder.dialog.Window.Attributes = wlp;
                }
            }

            return tooltipBuilder;
        }

        public SetupFeedBackFragment SetHeaderImage(int imageResource)
        {
            this.imageResource = imageResource;
            return this;
        }

        public SetupFeedBackFragment SetTitle(string title)
        {
            this.title = title;
            return this;
        }

        public SetupFeedBackFragment SetTitleOtherOne(string titleOtherOne)
        {
            this.titleOtherOne = titleOtherOne;
            return this;
        }

        public SetupFeedBackFragment SetTitleOtherTwo(string titleOtherTwo)
        {
            this.titleOtherTwo = titleOtherTwo;
            return this;
        }
        public SetupFeedBackFragment SetMessage(string message, Color? color = null, Typeface? typeface = null)
        {
            this.message = message;

            this.mClickSpanColor = color ?? new Android.Graphics.Color(ContextCompat.GetColor(mContext, Resource.Color.powerBlue));
            this.mTypeface = typeface ?? Typeface.CreateFromAsset(mContext.Assets, "fonts/" + TextViewUtils.MuseoSans500);
            return this;
        }

        public SetupFeedBackFragment SetCTALabel(string ctaLabel)
        {
            this.ctaLabel = ctaLabel;
            return this;
        }

        public SetupFeedBackFragment SetSecondaryCTALabel(string secondaryCTALabel)
        {
            this.secondaryCTALabel = secondaryCTALabel;
            return this;
        }

        public SetupFeedBackFragment SetAdapter(RecyclerView.Adapter adapter)
        {
            this.adapter = adapter;
            return this;
        }

        public SetupFeedBackFragment SetCTAaction(Action ctaFunc)
        {
            this.ctaAction = ctaFunc;
            return this;
        }

        public SetupFeedBackFragment SetSecondaryCTAaction(Action ctaFunc)
        {
            this.secondaryCTAAction = ctaFunc;
            return this;
        }

        public SetupFeedBackFragment SetYesBtnCTAaction(Action ctaFunc)
        {
            this.ctaYesAction = ctaFunc;
            return this;
        }

        public SetupFeedBackFragment SetNoBtnCTAaction(Action ctaFunc)
        {
            this.ctaNoAction = ctaFunc;
            return this;
        }

        public SetupFeedBackFragment SetContentGravity(GravityFlags gravityFlags)
        {
            this.mGravityFlag = gravityFlags;
            return this;
        }

        public void DismissDialog()
        {
            this.dialog.Dismiss();
        }

        public SetupFeedBackFragment SetHeaderImageBitmap(Bitmap imageResource)
        {
            this.imageResourceBitmap = imageResource;
            return this;
        }

        public SetupFeedBackFragment Build()
        {
            if (this.toolTipType == ToolTipType.IMAGE_HEADER)
            {
                ImageView tooltipImageHeader = this.dialog.FindViewById<ImageView>(Resource.Id.imgToolTipHeader);
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);

                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipCTA);
                TextViewUtils.SetTextSize20(tooltipTitle);
                TextViewUtils.SetTextSize16(tooltipCTA);

                tooltipTitle.Text = this.title;
                tooltipCTA.Text = this.ctaLabel;

                tooltipCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    if (this.ctaAction != null)
                    {
                        this.ctaAction();
                    }
                };
            }           
            else if (this.toolTipType == ToolTipType.FEEDBACK_WITH_IMAGES_STAR_RATING_BUTTON)
            {
                GridLayoutManager layoutManager;
                int selectedRating = 0;
                RecyclerView recyclerView = this.dialog.FindViewById<RecyclerView>(Resource.Id.dialogRecyclerView);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);
                TextView tooltipBlueTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitleBlue);
                LinearLayout indicatorContainer = this.dialog.FindViewById<LinearLayout>(Resource.Id.dialoagListViewIndicatorContainer);
                
                TextViewUtils.SetMuseoSans500Typeface(tooltipCTA, tooltipBlueTitle);

                TextViewUtils.SetTextSize18(tooltipBlueTitle);
                TextViewUtils.SetTextSize14(tooltipCTA);

                tooltipBlueTitle.Text = this.title;
                tooltipCTA.Text = this.ctaLabel;

                countClick = 0;
                injectData();
                layoutManager = new GridLayoutManager(this.mContext, 5);
                adaptercustom = new RateUsStarsCustomAdapter(this.mContext, SelectStarPosition, selectedRating);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(adaptercustom);
                adaptercustom.RatingUpdate += OnRatingUpdate;

                tooltipCTA.Click += delegate
                {
                    MyTNBAccountManagement.GetInstance().SetIsFromClickAdapter(0);
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };
            }
            else if (this.toolTipType == ToolTipType.NORMAL_WITH_THREE_BUTTON)
            {
                Button btnDontAsk = this.dialog.FindViewById<Button>(Resource.Id.btnNoTQ);
                LinearLayout btnYes = this.dialog.FindViewById<LinearLayout>(Resource.Id.btnYes_Layout);
                LinearLayout btnNo = this.dialog.FindViewById<LinearLayout>(Resource.Id.btnNo_Layout);
                TextView tooltipBlueTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtTitle);
                TextView titleYes = this.dialog.FindViewById<TextView>(Resource.Id.titleYes);
                TextView titleNo = this.dialog.FindViewById<TextView>(Resource.Id.titleNo);
                ImageView img_displayYes = this.dialog.FindViewById<ImageView>(Resource.Id.img_displayYes);
                ImageView img_displayNo = this.dialog.FindViewById<ImageView>(Resource.Id.img_displayNo);
                LinearLayout indicatorContainer = this.dialog.FindViewById<LinearLayout>(Resource.Id.dialoagListViewIndicatorContainer);

                TextViewUtils.SetMuseoSans500Typeface(tooltipBlueTitle, titleYes, titleNo);
                TextViewUtils.SetMuseoSans500Typeface(btnDontAsk);

                TextViewUtils.SetTextSize17(tooltipBlueTitle);
                TextViewUtils.SetTextSize16(btnDontAsk);
                TextViewUtils.SetTextSize10(titleYes, titleNo);

                tooltipBlueTitle.Text = this.title;
                titleYes.Text = this.titleOtherOne;
                titleNo.Text = this.titleOtherTwo;
                btnDontAsk.Text = this.ctaLabel;

                btnDontAsk.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                btnYes.Click += delegate
                {
                    btnYes.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.blue_out_line_thin);
                    img_displayYes.SetImageResource(Resource.Drawable.thumb_up_blue_yes);
                    titleYes.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                    Handler h = new Handler();
                    Action myAction = () =>
                    {
                        this.dialog.Dismiss();
                        this.ctaYesAction?.Invoke();
                    };
                    h.PostDelayed(myAction, 300);
                };

                btnNo.Click += delegate
                {
                    btnNo.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.blue_out_line_thin);
                    img_displayNo.SetImageResource(Resource.Drawable.thumb_down_no_blue);
                    titleNo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                    Handler h = new Handler();
                    Action myAction = () =>
                    {
                        this.dialog.Dismiss();
                        this.ctaNoAction?.Invoke();
                    };
                    h.PostDelayed(myAction, 300);
                };
            }
            return this;
        }

        public void Show()
        {
            this.dialog.Show();
        }

        public void injectData()
        {
            var data1 = new ImproveSelectModel
            {
                ModelCategories = "star",
                IconCategories = "1",
                IsSelected = false,
            };
            SelectStarPosition.Add(data1);

            var data2 = new ImproveSelectModel
            {
                ModelCategories = "star",
                IconCategories = "2",
                IsSelected = false,
            };
            SelectStarPosition.Add(data2);

            var data3 = new ImproveSelectModel
            {
                ModelCategories = "star",
                IconCategories = "3",
                IsSelected = false,
            };
            SelectStarPosition.Add(data3);

            var data4 = new ImproveSelectModel
            {
                ModelCategories = "star",
                IconCategories = "4",
                IsSelected = false,
            };
            SelectStarPosition.Add(data4);

            var data5 = new ImproveSelectModel
            {
                ModelCategories = "star",
                IconCategories = "5",
                IsSelected = false,
            };
            SelectStarPosition.Add(data5);
        }

        void OnRatingUpdate(object sender, int position)
        {
            try
            {
                countClick++;
                if (adaptercustom != null)
                {
                    if (MyTNBAccountManagement.GetInstance().IsFromClickAdapter() != position && countClick == 1)
                    {
                        RecyclerView recyclerView = this.dialog.FindViewById<RecyclerView>(Resource.Id.dialogRecyclerView);
                        MyTNBAccountManagement.GetInstance().SetIsFromClickAdapter(position);
                        List<ImproveSelectModel> activeStarSelectList = new List<ImproveSelectModel>();
                        int starSelect = position + 1;
                        foreach (ImproveSelectModel data in SelectStarPosition)
                        {
                            int NoStar = int.Parse(data.IconCategories);
                            if (NoStar < starSelect || NoStar == starSelect)
                            {
                                data.IsSelected = true;
                            }
                            else
                            {
                                data.IsSelected = false;
                            }
                            activeStarSelectList.Add(data);
                        }
                        SelectStarPosition = null;
                        SelectStarPosition = activeStarSelectList;
                        adaptercustom.NotifyDataSetChanged();
                        Handler h = new Handler();
                        Action myAction = () =>
                        {
                            this.dialog.Dismiss();
                            this.secondaryCTAAction();
                        };
                        h.PostDelayed(myAction, 300);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}