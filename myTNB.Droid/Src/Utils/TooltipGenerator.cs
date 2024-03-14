using System;
using System.Collections.Generic;
using AFollestad.MaterialDialogs;
using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace myTNB.Android.Src.Utils
{
    public class TooltipGenerator
    {
        Context mContext;
        MaterialDialog materialDialog;
        List<TooltipAction> tooltipActions;
        string tooltipContent;

        public TooltipGenerator(Context context)
        {
            this.mContext = context;
        }

        public void Create(string content)
        {
            this.materialDialog = null;
            this.tooltipActions = new List<TooltipAction>();
            tooltipContent = content;
        }

        public void AddAction(string actionLabel, View.IOnClickListener onClickListener)
        {
            this.tooltipActions.Add(new TooltipAction(actionLabel, onClickListener));
        }

        public void AddAction(string actionLabel)
        {
            this.tooltipActions.Add(new TooltipAction(actionLabel));
        }

        public void Show()
        {
            int tooltipViewLayout = Resource.Layout.ToolTipSingleButtonView;
            switch (this.tooltipActions.Count)
            {
                case 1:
                    tooltipViewLayout = Resource.Layout.ToolTipSingleButtonView;
                    break;
                default:
                    break;
            }

            this.materialDialog = new MaterialDialog.Builder(this.mContext)
                .CustomView(tooltipViewLayout, false)
                .Cancelable(false)
                .Build();

            TextView toolTipContentView = materialDialog.FindViewById<TextView>(Resource.Id.txtToolTipContent);
            FormatContent(toolTipContentView, this.tooltipContent);
            TextView toolTipActionView = materialDialog.FindViewById<TextView>(Resource.Id.txtToolTipAction);
            TextViewUtils.SetMuseoSans500Typeface(toolTipActionView);
            toolTipActionView.Text = this.tooltipActions[0].GetLabel();
            TextViewUtils.SetTextSize14(toolTipContentView);
            TextViewUtils.SetTextSize16(toolTipActionView);

            toolTipActionView.Click += delegate
            {
                this.materialDialog.Dismiss();
            };

            this.materialDialog.Show();
        }

        private void FormatContent(TextView contentView, string content)
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                contentView.TextFormatted = Html.FromHtml(content, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                contentView.TextFormatted = Html.FromHtml(content);
            }

            if (contentView != null)
            {
                TextViewUtils.SetMuseoSans300Typeface(contentView);
            }
        }

        public class TooltipAction
        {
            private string actionLabel;
            private View.IOnClickListener onClickListener;
            public TooltipAction(string label, View.IOnClickListener listener)
            {
                actionLabel = label;
                onClickListener = listener;
            }

            public TooltipAction(string label)
            {
                actionLabel = label;
            }

            public string GetLabel()
            {
                return actionLabel;
            }

            public View.IOnClickListener GetClickListener()
            {
                return onClickListener;
            }
        }
    }
}
