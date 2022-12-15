using System;
using AFollestad.MaterialDialogs;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Text;
using Android.Graphics;
using AndroidX.RecyclerView.Widget;
using AndroidX.Core.Content;
using Android.Text.Style;
using System.Text.RegularExpressions;
using myTNB.Mobile.Constants.DS;

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
            THREE_PART_WITH_HEADER_TWO_BUTTON,
            DIALOGBOX_WITH_CHECKBOX,
            DIALOGBOX_WITH_IMAGE_ONE_BUTTON,
            MYTNB_DIALOG_IMAGE_BUTTON,
            MYTNB_DIALOG_ICON_ONE_BUTTON,
            MYTNB_DIALOG_ICON_TWO_BUTTON,
            MYTNB_DIALOG_ICON_DROPDOWN_TWO_BUTTON,
            MYTNB_DIALOG_IMAGE_BUTTON_WITH_HEADER
        }

        private ToolTipType toolTipType;
        private int imageResource;
        private string headertitle;
        private string title;
        private string subtitle;
        private string dropdownTitle;
        private string message;
        private string dropdownMessage;
        private string ctaLabel;
        private string secondaryCTALabel;
        private string CheckboxTitle;
        private RecyclerView.Adapter adapter;
        private Action ctaAction;
        private Action secondaryCTAAction;
        private Action checkedBoxAction;
        private Action uncheckedBoxAction;
        private MaterialDialog dialog;
        private Color mClickSpanColor;
        private Typeface mTypeface;
        private Android.App.Activity mContext;
        private GravityFlags mGravityFlag;
        private Bitmap imageResourceBitmap;
        private bool isIconImage = false;

        private MyTNBAppToolTipBuilder(Android.App.Activity context)
        {
            this.mContext = context;
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
            else if (mToolTipType == ToolTipType.THREE_PART_WITH_HEADER_TWO_BUTTON)
            {
                layoutResource = Resource.Layout.CustomToolTipWithHeaderSubheaderTwoButtonLayout;
            }
            else if (mToolTipType == ToolTipType.DIALOGBOX_WITH_CHECKBOX)
            {
                layoutResource = Resource.Layout.CustomToolTipWithIDDialogTwoButtonLayout;
            }
            else if (mToolTipType == ToolTipType.DIALOGBOX_WITH_IMAGE_ONE_BUTTON)
            {
                layoutResource = Resource.Layout.CustomToolTipWithIDDialogWithButtonLayout;
            }
            else if (mToolTipType == ToolTipType.MYTNB_DIALOG_IMAGE_BUTTON)
            {
                layoutResource = Resource.Layout.MyTNBDialogWithImageAndButton;
            }
            else if (mToolTipType == ToolTipType.MYTNB_DIALOG_ICON_TWO_BUTTON)
            {
                layoutResource = Resource.Layout.MyTNBDialogWithIconAndTwoButtons;
            }
            else if (mToolTipType == ToolTipType.MYTNB_DIALOG_ICON_ONE_BUTTON)
            {
                layoutResource = Resource.Layout.MyTNBDialogWithIconAndOneButton;
            }
            else if (mToolTipType == ToolTipType.MYTNB_DIALOG_ICON_DROPDOWN_TWO_BUTTON)
            {
                layoutResource = Resource.Layout.MyTNBDialogWithIconDropdownAndTwoButtons;
            }
            else if (mToolTipType == ToolTipType.MYTNB_DIALOG_IMAGE_BUTTON_WITH_HEADER)
            {
                layoutResource = Resource.Layout.MyTNBDialogWithHeaderImageAndButton;
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

        public MyTNBAppToolTipBuilder SetHeaderTitle(string headertitle)
        {
            this.headertitle = headertitle;
            return this;
        }

        public MyTNBAppToolTipBuilder SetSubTitle(string subtitle)
        {
            this.subtitle = subtitle;
            return this;
        }

        public MyTNBAppToolTipBuilder SetDropdownTitle(string dropdownTitle)
        {
            this.dropdownTitle = dropdownTitle;
            return this;
        }

        public MyTNBAppToolTipBuilder SetMessage(string message, Color? color = null, Typeface? typeface = null)
        {
            this.message = message;

            this.mClickSpanColor = color ?? new Android.Graphics.Color(ContextCompat.GetColor(mContext, Resource.Color.powerBlue));
            this.mTypeface = typeface ?? Typeface.CreateFromAsset(mContext.Assets, "fonts/" + TextViewUtils.MuseoSans500);
            return this;
        }

        public MyTNBAppToolTipBuilder SetDropdownMessage(string dropdownMessage)
        {
            this.dropdownMessage = dropdownMessage;
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

        public MyTNBAppToolTipBuilder SetCheckBoxCTaction(Action ctaFunc)
        {
            this.checkedBoxAction = ctaFunc;
            return this;
        }

        public MyTNBAppToolTipBuilder SetUnCheckBoxCTaction(Action ctaFunc)
        {
            this.uncheckedBoxAction = ctaFunc;
            return this;
        }

        public MyTNBAppToolTipBuilder SetTitleCheckBox(string checkboxtitle)
        {
            this.CheckboxTitle = checkboxtitle;
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

        public MyTNBAppToolTipBuilder IsIconImage(bool isIcon)
        {
            this.isIconImage = isIcon;
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
                TextViewUtils.SetTextSize14(tooltipTitle, tooltipMessage);
                TextViewUtils.SetTextSize16(tooltipCTA);

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

                tooltipMessage = LinkRedirectionUtils
                    .Create(this.mContext, string.Empty)
                    .SetTextView(tooltipMessage)
                    .SetMessage(this.message, this.mClickSpanColor, this.mTypeface)
                    .Build()
                    .GetProcessedTextView();

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
                tooltipCTA.Text = this.ctaLabel;
            }
            else if (this.toolTipType == ToolTipType.NORMAL_WITH_HEADER)
            {
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipCTA);
                TextViewUtils.SetTextSize14(tooltipMessage, tooltipTitle);
                TextViewUtils.SetTextSize16(tooltipCTA);

                tooltipTitle.Gravity = this.mGravityFlag;
                tooltipMessage.Gravity = this.mGravityFlag;

                tooltipCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                tooltipTitle.Text = this.title;
                tooltipTitle.Visibility = string.IsNullOrEmpty(this.title) || string.IsNullOrWhiteSpace(this.title)
                    ? ViewStates.Gone
                    : ViewStates.Visible;
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                }

                tooltipMessage = LinkRedirectionUtils
                    .Create(this.mContext, string.Empty)
                    .SetTextView(tooltipMessage)
                    .SetMessage(this.message, this.mClickSpanColor, this.mTypeface)
                    .Build()
                    .GetProcessedTextView();

                tooltipCTA.Text = this.ctaLabel;
            }
            else if (this.toolTipType == ToolTipType.LISTVIEW_WITH_INDICATOR_AND_HEADER)
            {
                RecyclerView recyclerView = this.dialog.FindViewById<RecyclerView>(Resource.Id.dialogRecyclerView);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);
                TextViewUtils.SetMuseoSans500Typeface(tooltipCTA);
                TextViewUtils.SetTextSize16(tooltipCTA);
                tooltipCTA.Text = this.ctaLabel;
                LinearLayout indicatorContainer = this.dialog.FindViewById<LinearLayout>(Resource.Id.dialoagListViewIndicatorContainer);

                LinearSnapHelper snapTooltipHelper = new LinearSnapHelper();
                LinearLayoutManager layoutManager = new LinearLayoutManager(this.mContext, LinearLayoutManager.Horizontal, false);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(this.adapter);
                snapTooltipHelper.AttachToRecyclerView(recyclerView);
                recyclerView.AddOnScrollListener(new ToolTipRecyclerViewOnScrollListener(layoutManager, indicatorContainer));
                TextViewUtils.SetTextSize16(tooltipCTA);

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
                TextViewUtils.SetTextSize14(tooltipTitle, tooltipMessage);
                TextViewUtils.SetTextSize16(tooltipPrimaryCTA, tooltipSecondaryCTA);

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
                tooltipMessage = LinkRedirectionUtils
                    .Create(this.mContext, string.Empty)
                    .SetTextView(tooltipMessage)
                    .SetMessage(this.message, this.mClickSpanColor, this.mTypeface)
                    .Build()
                    .GetProcessedTextView();
                tooltipPrimaryCTA.Text = this.ctaLabel;
                tooltipSecondaryCTA.Text = this.secondaryCTALabel;
            }
            else if (this.toolTipType == ToolTipType.THREE_PART_WITH_HEADER_TWO_BUTTON)
            {
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipSubTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipSubTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnPrimary);
                TextView tooltipSecondaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnSecondary);

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipSubTitle, tooltipPrimaryCTA, tooltipSecondaryCTA);
                TextViewUtils.SetTextSize14(tooltipTitle, tooltipSubTitle, tooltipMessage);
                TextViewUtils.SetTextSize16(tooltipPrimaryCTA, tooltipSecondaryCTA);

                tooltipTitle.Gravity = this.mGravityFlag;
                tooltipSubTitle.Gravity = this.mGravityFlag;
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
                tooltipSubTitle.Text = this.subtitle;
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                }
                tooltipMessage = LinkRedirectionUtils
                    .Create(this.mContext, "")
                    .SetTextView(tooltipMessage)
                    .SetMessage(this.message, this.mClickSpanColor, this.mTypeface)
                    .Build()
                    .GetProcessedTextView();
                tooltipPrimaryCTA.Text = this.ctaLabel;
                tooltipSecondaryCTA.Text = this.secondaryCTALabel;
            }
            else if (this.toolTipType == ToolTipType.DIALOGBOX_WITH_CHECKBOX)
            {
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnPrimary);
                TextView tooltipSecondaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnSecondary);
                CheckBox tooltipCheckBoxCTA = this.dialog.FindViewById<CheckBox>(Resource.Id.ToolTipCheckBox);
                TextView tooltipCheckBoxText = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitleCheckBox);

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipPrimaryCTA, tooltipCheckBoxText);
                TextViewUtils.SetTextSize14(tooltipTitle, tooltipCheckBoxText, tooltipMessage);
                TextViewUtils.SetTextSize16(tooltipPrimaryCTA);

                tooltipTitle.Gravity = this.mGravityFlag;
                tooltipMessage.Gravity = this.mGravityFlag;

                tooltipPrimaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                tooltipCheckBoxCTA.CheckedChange += (sender, e) =>
                {
                    if (e.IsChecked)
                    {
                        this.checkedBoxAction();
                    }
                    else
                    {
                        this.uncheckedBoxAction();
                    }
                };

                tooltipSecondaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    if (ctaAction != null)
                    {
                        this.ctaAction();
                    }
                };

                tooltipTitle.Text = this.title;
                tooltipCheckBoxText.Text = this.CheckboxTitle;
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                }
                tooltipMessage = LinkRedirectionUtils
                    .Create(this.mContext, "")
                    .SetTextView(tooltipMessage)
                    .SetMessage(this.message, this.mClickSpanColor, this.mTypeface)
                    .Build()
                    .GetProcessedTextView();
                tooltipPrimaryCTA.Text = this.ctaLabel;
                tooltipSecondaryCTA.Text = this.secondaryCTALabel;
            }
            else if (this.toolTipType == ToolTipType.DIALOGBOX_WITH_IMAGE_ONE_BUTTON)
            {
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnPrimary);

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipPrimaryCTA);
                TextViewUtils.SetTextSize14(tooltipTitle, tooltipMessage);
                TextViewUtils.SetTextSize16(tooltipPrimaryCTA);

                tooltipTitle.Gravity = this.mGravityFlag;
                tooltipMessage.Gravity = this.mGravityFlag;

                tooltipPrimaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    if (ctaAction != null)
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
                tooltipMessage = LinkRedirectionUtils
                    .Create(this.mContext, "")
                    .SetTextView(tooltipMessage)
                    .SetMessage(this.message, this.mClickSpanColor, this.mTypeface)
                    .Build()
                    .GetProcessedTextView();
                tooltipPrimaryCTA.Text = this.ctaLabel;
                //tooltipSecondaryCTA.Text = this.secondaryCTALabel;
            }
            else if (this.toolTipType == ToolTipType.IMAGE_HEADER_TWO_BUTTON)
            {
                ImageView tooltipImageHeader = this.dialog.FindViewById<ImageView>(Resource.Id.imgToolTipHeader);
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnPrimary);
                TextView tooltipSecondaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnSecondary);
                TextViewUtils.SetTextSize14(tooltipTitle, tooltipMessage);
                TextViewUtils.SetTextSize16(tooltipPrimaryCTA, tooltipPrimaryCTA, tooltipSecondaryCTA);
                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipPrimaryCTA, tooltipSecondaryCTA);
                tooltipTitle.Gravity = this.mGravityFlag;
                tooltipMessage.Gravity = this.mGravityFlag;

                if (this.isIconImage)
                {
                    tooltipImageHeader.Visibility = ViewStates.Gone;
                    ImageView tooltipImageHeaderIcon = this.dialog.FindViewById<ImageView>(Resource.Id.imgToolTipHeaderIcon);
                    tooltipImageHeaderIcon.SetImageResource(this.imageResource);
                    tooltipImageHeaderIcon.Visibility = ViewStates.Visible;
                }
                else if (this.imageResourceBitmap != null)
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
                tooltipMessage = LinkRedirectionUtils
                    .Create(this.mContext, string.Empty)
                    .SetTextView(tooltipMessage)
                    .SetMessage(this.message, this.mClickSpanColor, this.mTypeface)
                    .Build()
                    .GetProcessedTextView();

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
                TextViewUtils.SetTextSize14(tooltipTitle, tooltipMessage);
                TextViewUtils.SetTextSize16(tooltipCTA);
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
                tooltipMessage = LinkRedirectionUtils
                    .Create(this.mContext, string.Empty)
                    .SetTextView(tooltipMessage)
                    .SetMessage(this.message, this.mClickSpanColor, this.mTypeface)
                    .Build()
                    .GetProcessedTextView();

                tooltipCTA.Text = this.ctaLabel;

            }
            else if (this.toolTipType == ToolTipType.NORMAL_STRETCHABLE)
            {
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtDialogMessage);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnLabel);

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipCTA);
                TextViewUtils.SetTextSize14(tooltipMessage);
                TextViewUtils.SetTextSize16(tooltipCTA);

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
                tooltipMessage = LinkRedirectionUtils
                    .Create(this.mContext, string.Empty)
                    .SetTextView(tooltipMessage)
                    .SetMessage(this.message, this.mClickSpanColor, this.mTypeface)
                    .Build()
                    .GetProcessedTextView();

                tooltipCTA.Text = this.ctaLabel;

            }
            else if (this.toolTipType == ToolTipType.MYTNB_DIALOG_IMAGE_BUTTON)
            {
                ImageView tooltipImageHeader = this.dialog.FindViewById<ImageView>(Resource.Id.dialogHeaderImg);
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.dialogTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.dialogMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.dialogPrimaryBtn);
                TextView tooltipSecondaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.dialogSecondaryBtn);

                TextViewUtils.SetTextSize14(tooltipMessage);
                TextViewUtils.SetTextSize16(tooltipTitle, tooltipPrimaryCTA);
                TextViewUtils.SetTextSize12(tooltipSecondaryCTA);
                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipPrimaryCTA, tooltipSecondaryCTA);
                tooltipTitle.Gravity = this.mGravityFlag;
                tooltipMessage.Gravity = this.mGravityFlag;

                if (this.imageResourceBitmap != null)
                {
                    tooltipImageHeader.SetImageBitmap(this.imageResourceBitmap);
                }
                else if (this.imageResource > 0)
                {
                    tooltipImageHeader.SetImageResource(this.imageResource);
                }
                else
                {
                    tooltipImageHeader.Visibility = ViewStates.Gone;
                }

                tooltipPrimaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                tooltipSecondaryCTA.Visibility = this.secondaryCTALabel.IsValid() ? ViewStates.Visible : ViewStates.Gone;

                if (this.secondaryCTALabel.IsValid())
                {
                    tooltipSecondaryCTA.Click += delegate
                    {
                        this.dialog.Dismiss();
                        if (secondaryCTAAction != null)
                        {
                            this.secondaryCTAAction();
                        }
                    };
                }
                else
                {
                    LinearLayout.LayoutParams primaryButtonParams = tooltipPrimaryCTA.LayoutParameters as LinearLayout.LayoutParams;
                    primaryButtonParams.BottomMargin = (int)DPUtils.ConvertDPToPx(24f);
                }

                tooltipTitle.Text = this.title;
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                }
                tooltipMessage = LinkRedirectionUtils
                    .Create(this.mContext, string.Empty)
                    .SetTextView(tooltipMessage)
                    .SetMessage(this.message, this.mClickSpanColor, this.mTypeface)
                    .Build()
                    .GetProcessedTextView();

                tooltipPrimaryCTA.Text = this.ctaLabel;
                tooltipSecondaryCTA.Text = this.secondaryCTALabel;
            }
            else if (this.toolTipType == ToolTipType.MYTNB_DIALOG_ICON_TWO_BUTTON)
            {
                ImageView tooltipImageHeader = this.dialog.FindViewById<ImageView>(Resource.Id.dialogHeaderImg);
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.dialogTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.dialogMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnPrimary);
                TextView tooltipSecondaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnSecondary);

                TextViewUtils.SetTextSize12(tooltipMessage);
                TextViewUtils.SetTextSize14(tooltipTitle, tooltipPrimaryCTA, tooltipSecondaryCTA);
                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage, tooltipPrimaryCTA);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipSecondaryCTA);

                tooltipTitle.Gravity = GravityFlags.Center;
                tooltipMessage.Gravity = GravityFlags.Center;

                if (this.imageResourceBitmap != null)
                {
                    tooltipImageHeader.SetImageBitmap(this.imageResourceBitmap);
                }
                else if (this.imageResource > 0)
                {
                    tooltipImageHeader.SetImageResource(this.imageResource);
                }
                else
                {
                    tooltipImageHeader.Visibility = ViewStates.Gone;
                }

                tooltipPrimaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                tooltipSecondaryCTA.Visibility = this.secondaryCTALabel.IsValid() ? ViewStates.Visible : ViewStates.Gone;

                if (this.secondaryCTALabel.IsValid())
                {
                    tooltipSecondaryCTA.Click += delegate
                    {
                        this.dialog.Dismiss();
                        if (secondaryCTAAction != null)
                        {
                            this.secondaryCTAAction();
                        }
                    };
                }

                tooltipTitle.Text = this.title;

                try
                {
                    if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                tooltipPrimaryCTA.Text = this.ctaLabel;
                tooltipSecondaryCTA.Text = this.secondaryCTALabel;
            }
            else if (this.toolTipType == ToolTipType.MYTNB_DIALOG_ICON_ONE_BUTTON)
            {
                ImageView tooltipImageHeader = this.dialog.FindViewById<ImageView>(Resource.Id.dialogHeaderImg);
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.dialogTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.dialogMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnPrimary);

                TextViewUtils.SetTextSize12(tooltipMessage);
                TextViewUtils.SetTextSize14(tooltipTitle, tooltipPrimaryCTA);
                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipPrimaryCTA);

                tooltipTitle.Gravity = GravityFlags.Center;
                tooltipMessage.Gravity = GravityFlags.Center;

                if (this.imageResourceBitmap != null)
                {
                    tooltipImageHeader.SetImageBitmap(this.imageResourceBitmap);
                }
                else if (this.imageResource > 0)
                {
                    tooltipImageHeader.SetImageResource(this.imageResource);
                }
                else
                {
                    tooltipImageHeader.Visibility = ViewStates.Gone;
                }

                tooltipPrimaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                tooltipTitle.Text = this.title;

                try
                {
                    if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                tooltipPrimaryCTA.Text = this.ctaLabel;
            }
            else if (this.toolTipType == ToolTipType.MYTNB_DIALOG_ICON_DROPDOWN_TWO_BUTTON)
            {
                ImageView tooltipImageHeader = this.dialog.FindViewById<ImageView>(Resource.Id.dialogHeaderImg);
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.dialogTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.dialogMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnPrimary);
                TextView tooltipSecondaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnSecondary);
                LinearLayout dropdownContentLayout = this.dialog.FindViewById<LinearLayout>(Resource.Id.dropdownContentLayout);
                TextView tooltipDropdownTitle = this.dialog.FindViewById<TextView>(Resource.Id.dialogDropdownTitle);
                TextView tooltipDropdownMessage = this.dialog.FindViewById<TextView>(Resource.Id.dialogDropdownMessage);

                TextViewUtils.SetTextSize12(tooltipMessage, tooltipDropdownTitle, tooltipDropdownMessage);
                TextViewUtils.SetTextSize14(tooltipTitle, tooltipPrimaryCTA, tooltipSecondaryCTA);
                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage, tooltipPrimaryCTA, tooltipDropdownTitle, tooltipDropdownMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipSecondaryCTA);

                tooltipTitle.Gravity = GravityFlags.Center;
                tooltipMessage.Gravity = GravityFlags.Center;
                tooltipDropdownTitle.Gravity = GravityFlags.Center;
                tooltipDropdownMessage.Gravity = GravityFlags.Center;

                tooltipDropdownMessage.Visibility = ViewStates.Gone;

                if (this.imageResourceBitmap != null)
                {
                    tooltipImageHeader.SetImageBitmap(this.imageResourceBitmap);
                }
                else if (this.imageResource > 0)
                {
                    tooltipImageHeader.SetImageResource(this.imageResource);
                }
                else
                {
                    tooltipImageHeader.Visibility = ViewStates.Gone;
                }

                tooltipPrimaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                tooltipSecondaryCTA.Visibility = this.secondaryCTALabel.IsValid() ? ViewStates.Visible : ViewStates.Gone;

                if (this.secondaryCTALabel.IsValid())
                {
                    tooltipSecondaryCTA.Click += delegate
                    {
                        this.dialog.Dismiss();
                        if (secondaryCTAAction != null)
                        {
                            this.secondaryCTAAction();
                        }
                    };
                }

                tooltipTitle.Text = this.title;
                tooltipPrimaryCTA.Text = this.ctaLabel;
                tooltipSecondaryCTA.Text = this.secondaryCTALabel;

                string dropdownTitleBase = Regex.Replace(this.dropdownTitle, DSConstants.DropDown, string.Empty);

                try
                {
                    if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                        tooltipDropdownTitle.TextFormatted = Html.FromHtml(this.dropdownTitle, FromHtmlOptions.ModeLegacy);
                        tooltipDropdownMessage.TextFormatted = Html.FromHtml(this.dropdownMessage, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                        tooltipDropdownTitle.TextFormatted = Html.FromHtml(this.dropdownTitle);
                        tooltipDropdownMessage.TextFormatted = Html.FromHtml(this.dropdownMessage);
                    }

                    ImageSpan imageSpan = new ImageSpan(this.mContext, Resource.Drawable.Icon_DS_Dropdown_Expand, SpanAlign.Bottom);
                    SpannableString imageString = new SpannableString(tooltipDropdownTitle.TextFormatted);

                    imageString.SetSpan(imageSpan, dropdownTitleBase.Length, this.dropdownTitle.Length, SpanTypes.ExclusiveExclusive);
                    tooltipDropdownTitle.TextFormatted = imageString;
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                var expanded = false;
                tooltipDropdownTitle.Click += delegate
                {
                    expanded = !expanded;
                    tooltipDropdownMessage.Visibility = expanded ? ViewStates.Visible : ViewStates.Gone;

                    ImageSpan imageSpan = new ImageSpan(this.mContext, expanded ? Resource.Drawable.Icon_DS_Dropdown_Collapse : Resource.Drawable.Icon_DS_Dropdown_Expand, SpanAlign.Bottom);
                    SpannableString imageString = new SpannableString(tooltipDropdownTitle.TextFormatted);

                    imageString.SetSpan(imageSpan, dropdownTitleBase.Length, this.dropdownTitle.Length, SpanTypes.ExclusiveExclusive);
                    tooltipDropdownTitle.TextFormatted = imageString;
                };
            }
            else if (this.toolTipType == ToolTipType.MYTNB_DIALOG_IMAGE_BUTTON_WITH_HEADER)
            {
                ImageView tooltipImageHeader = this.dialog.FindViewById<ImageView>(Resource.Id.dialogHeaderImg);
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.dialogTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.dialogMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.dialogPrimaryBtn);
                TextView tooltipHeaderTitle = this.dialog.FindViewById<TextView>(Resource.Id.dialogHeaderTitle);

                TextViewUtils.SetTextSize14(tooltipMessage);
                TextViewUtils.SetTextSize16(tooltipTitle, tooltipHeaderTitle, tooltipPrimaryCTA);
                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipHeaderTitle, tooltipPrimaryCTA);
                tooltipTitle.Gravity = this.mGravityFlag;
                tooltipMessage.Gravity = this.mGravityFlag;
                tooltipHeaderTitle.Gravity = this.mGravityFlag;

                if (this.imageResourceBitmap != null)
                {
                    tooltipImageHeader.SetImageBitmap(this.imageResourceBitmap);
                }
                else if (this.imageResource > 0)
                {
                    tooltipImageHeader.SetImageResource(this.imageResource);
                }
                else
                {
                    tooltipImageHeader.Visibility = ViewStates.Gone;
                }

                tooltipPrimaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    this.ctaAction?.Invoke();
                };

                tooltipTitle.Text = this.title;
                tooltipHeaderTitle.Text = this.headertitle;
                if (string.IsNullOrEmpty(this.title))
                {
                    tooltipTitle.Visibility = ViewStates.Gone;
                }
                if (string.IsNullOrEmpty(this.headertitle))
                {
                    tooltipHeaderTitle.Visibility = ViewStates.Gone;
                }
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    tooltipMessage.TextFormatted = Html.FromHtml(this.message);
                }
                tooltipMessage = LinkRedirectionUtils
                    .Create(this.mContext, string.Empty)
                    .SetTextView(tooltipMessage)
                    .SetMessage(this.message, this.mClickSpanColor, this.mTypeface)
                    .Build()
                    .GetProcessedTextView();

                tooltipPrimaryCTA.Text = this.ctaLabel;
            }
            return this;
        }

        public void Show()
        {
            this.dialog.Show();
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