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
        private ClickableSpan clickableSpan;
        private Context mContext;
        private GravityFlags mGravityFlag;

        private MyTNBAppToolTipBuilder()
        {

        }

        public static MyTNBAppToolTipBuilder Create(Context context, ToolTipType mToolTipType)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = new MyTNBAppToolTipBuilder();
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
            tooltipBuilder.dialog = new MaterialDialog.Builder(context)
                .CustomView(layoutResource, false)
                .Cancelable(false)
                .CanceledOnTouchOutside(false)
                .Build();

            View dialogView = tooltipBuilder.dialog.Window.DecorView;
            dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);
            WindowManagerLayoutParams wlp = tooltipBuilder.dialog.Window.Attributes;
            wlp.Gravity = GravityFlags.Center;
            wlp.Width = ViewGroup.LayoutParams.MatchParent;
            wlp.Height = ViewGroup.LayoutParams.WrapContent;
            tooltipBuilder.dialog.Window.Attributes = wlp;

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

        public MyTNBAppToolTipBuilder SetMessage(string message)
        {
            this.message = message;
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

        public MyTNBAppToolTipBuilder SetContext(Context context)
        {
            this.mContext = context;
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

        public MyTNBAppToolTipBuilder SetClickableSpan(ClickableSpan clickSpan)
        {
            this.clickableSpan = clickSpan;
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

        public MyTNBAppToolTipBuilder Build()
        {
            if (this.toolTipType == ToolTipType.IMAGE_HEADER)
            {
                ImageView tooltipImageHeader = this.dialog.FindViewById<ImageView>(Resource.Id.imgToolTipHeader);
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);

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
                tooltipImageHeader.SetImageResource(this.imageResource);
                tooltipCTA.Text = this.ctaLabel;

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipCTA);
            }
            else if(this.toolTipType == ToolTipType.NORMAL_WITH_HEADER)
            {
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);

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
                if (this.clickableSpan != null)
                {
                    tooltipMessage.TextFormatted = Utility.GetFormattedURLString(this.clickableSpan, tooltipMessage.TextFormatted);
                    tooltipMessage.MovementMethod = new LinkMovementMethod();
                }
                tooltipCTA.Text = this.ctaLabel;

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipCTA);
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
                    if (this.ctaAction != null)
                    {
                        this.ctaAction();
                    }
                };
            }
            else if (this.toolTipType == ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
            {
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnPrimary);
                TextView tooltipSecondaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnSecondary);

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
                tooltipPrimaryCTA.Text = this.ctaLabel;
                tooltipSecondaryCTA.Text = this.secondaryCTALabel;

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipPrimaryCTA, tooltipSecondaryCTA);
            }
            else if (this.toolTipType == ToolTipType.IMAGE_HEADER_TWO_BUTTON)
            {
                ImageView tooltipImageHeader = this.dialog.FindViewById<ImageView>(Resource.Id.imgToolTipHeader);
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipPrimaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnPrimary);
                TextView tooltipSecondaryCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtBtnSecondary);

                tooltipImageHeader.SetImageResource(this.imageResource);

                tooltipPrimaryCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    if (ctaAction != null)
                    {
                        this.ctaAction();
                    }
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
                tooltipPrimaryCTA.Text = this.ctaLabel;
                tooltipSecondaryCTA.Text = this.secondaryCTALabel;

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipPrimaryCTA, tooltipSecondaryCTA);


            }
            else if (this.toolTipType == ToolTipType.NORMAL)
            {
                TextView tooltipTitle = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
                TextView tooltipMessage = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipMessage);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);

                tooltipCTA.Click += delegate
                {
                    this.dialog.Dismiss();
                    if (this.ctaAction != null)
                    {
                        this.ctaAction();
                    }
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
                if (this.clickableSpan != null)
                {
                    tooltipMessage.TextFormatted = Utility.GetFormattedURLString(this.clickableSpan, tooltipMessage.TextFormatted);
                    tooltipMessage.MovementMethod = new LinkMovementMethod();
                }
                tooltipCTA.Text = this.ctaLabel;

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipCTA);
            }
            return this;
        }

        public void Show()
        {
            this.dialog.Show();
        }

        class ClickSpan : ClickableSpan
        {
            public Action<View> Click;
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
