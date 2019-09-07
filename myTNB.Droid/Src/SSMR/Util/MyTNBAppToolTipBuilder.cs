using System;
using AFollestad.MaterialDialogs;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Text;
using myTNB_Android.Src.Utils;
using Android.Support.V7.Widget;

namespace myTNB_Android.Src.SSMR.Util
{
    public class MyTNBAppToolTipBuilder
    {
        public enum ToolTipType
        {
            IMAGE_HEADER,
            NORMAL_WITH_HEADER,
            NORMAL_WITH_HEADER_TWO_BUTTON,
            LISTVIEW_WITH_INDICATOR_AND_HEADER
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
        private Context mContext;

        private MyTNBAppToolTipBuilder()
        {

        }

        public static MyTNBAppToolTipBuilder Create(Context context, ToolTipType mToolTipType)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = new MyTNBAppToolTipBuilder();
            tooltipBuilder.toolTipType = mToolTipType;
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

        public MyTNBAppToolTipBuilder SetSecondaryCTAaction(Action ctaFunc)
        {
            this.secondaryCTAAction = ctaFunc;
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
                tooltipCTA.Text = this.ctaLabel;

                TextViewUtils.SetMuseoSans300Typeface(tooltipMessage);
                TextViewUtils.SetMuseoSans500Typeface(tooltipTitle, tooltipCTA);
            }
            else if (this.toolTipType == ToolTipType.LISTVIEW_WITH_INDICATOR_AND_HEADER)
            {
                RecyclerView recyclerView = this.dialog.FindViewById<RecyclerView>(Resource.Id.dialogRecyclerView);
                TextView tooltipCTA = this.dialog.FindViewById<TextView>(Resource.Id.txtToolTipCTA);
                LinearLayout indicatorContainer = this.dialog.FindViewById<LinearLayout>(Resource.Id.dialoagListViewIndicatorContainer);

                RecyclerView.LayoutManager layoutManager = new LinearLayoutManager(this.mContext, LinearLayoutManager.Horizontal, false);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(this.adapter);

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
            return this;
        }

        public void Show()
        {
            this.dialog.Show();
        }
    }
}
