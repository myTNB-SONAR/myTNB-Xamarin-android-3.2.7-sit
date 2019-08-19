using System;
using AFollestad.MaterialDialogs;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Text;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMR.Util
{
    public class MyTNBAppToolTipBuilder
    {
        public enum ToolTipType
        {
            IMAGE_HEADER,
            NORMAL_WITH_HEADER
        }

        private ToolTipType toolTipType;
        private int imageResource;
        private string title;
        private string message;
        private string ctaLabel;
        private Action ctaAction;
        private MaterialDialog dialog;

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

        public MyTNBAppToolTipBuilder SetCTAaction(Action ctaFunc)
        {
            this.ctaAction = ctaFunc;
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
            return this;
        }

        public void Show()
        {
            this.dialog.Show();
        }
    }
}
