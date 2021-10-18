using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class ExpandableTextViewComponent : LinearLayout
    {
        Context mContext;
        Drawable dropdown;
        TextView myApplicationChargesLabel;
        TextView myApplicationChargesValue;
        LinearLayout expandableContainer, expandedGSLContent;
        LinearLayout expandableBillDetailsLayout;
        LinearLayout expandableGSLLayout;
        TextView expandableGSLTitle;

        ExpandableTextViewType expandableType;
        BaseAppCompatActivity mActivity;

        public enum ExpandableTextViewType
        {
            APPLICATION_CHARGES,
            APPLICATION_STATUS,
            GSL_INFO,
            NONE
        }

        public ExpandableTextViewComponent(Context context, BaseAppCompatActivity activity) : base(context)
        {
            mContext = context;
            mActivity = activity;
            Init();
        }

        public ExpandableTextViewComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            Init();
        }

        public ExpandableTextViewComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            mContext = context;
            Init();
        }

        private void Init()
        {
            Inflate(mContext, Resource.Layout.ExpandingTextViewLayout, this);

            expandableBillDetailsLayout = FindViewById<LinearLayout>(Resource.Id.expandableBillDetailsLayout);
            expandableGSLLayout = FindViewById<LinearLayout>(Resource.Id.expandableGSLLayout);
            expandableGSLTitle = FindViewById<TextView>(Resource.Id.expandableGSLTitle);

            dropdown = ContextCompat.GetDrawable(mContext, Resource.Drawable.ic_action_expand_down);

            myApplicationChargesLabel = FindViewById<TextView>(Resource.Id.myApplicationChargesLabel);
            myApplicationChargesValue = FindViewById<TextView>(Resource.Id.myApplicationChargesValue);
            expandableContainer = FindViewById<LinearLayout>(Resource.Id.expandedContent);
            expandedGSLContent = FindViewById<LinearLayout>(Resource.Id.expandedGSLContent);

            myApplicationChargesLabel.SetCompoundDrawablesWithIntrinsicBounds(null, null, dropdown, null);
            myApplicationChargesLabel.CompoundDrawablePadding = (int)DPUtils.ConvertDPToPx(4f);

            TextViewUtils.SetMuseoSans500Typeface(myApplicationChargesLabel, myApplicationChargesValue);
            TextViewUtils.SetTextSize14(myApplicationChargesLabel, myApplicationChargesValue);

            TextViewUtils.SetMuseoSans300Typeface(expandableGSLTitle);
            TextViewUtils.SetTextSize14(expandableGSLTitle);

            SetOnClickListener(new OnExpandListener(this));
            expandableContainer.Visibility = ViewStates.Gone;
            expandableContainer.RemoveAllViews();

            expandedGSLContent.Visibility = ViewStates.Gone;
            expandedGSLContent.RemoveAllViews();
        }

        public void SetExpandableType(ExpandableTextViewType type)
        {
            this.expandableType = type;
            if (type == ExpandableTextViewType.APPLICATION_CHARGES || type == ExpandableTextViewType.APPLICATION_STATUS)
            {
                expandableBillDetailsLayout.Visibility = ViewStates.Visible;
            }
            else if (type == ExpandableTextViewType.GSL_INFO)
            {
                expandableGSLLayout.Visibility = ViewStates.Visible;
            }
        }

        public void SetApplicationChargesLabel(string label)
        {
            myApplicationChargesLabel.Text = label;
        }

        public void OnClickExpand(bool isExpand)
        {
            switch (expandableType)
            {
                case ExpandableTextViewType.APPLICATION_CHARGES:
                    {
                        if (isExpand)
                        {
                            dropdown = ContextCompat.GetDrawable(mContext, Resource.Drawable.ic_action_expand_down_up);
                        }
                        else
                        {
                            dropdown = ContextCompat.GetDrawable(mContext, Resource.Drawable.ic_action_expand_down);
                        }
                        myApplicationChargesLabel.SetCompoundDrawablesWithIntrinsicBounds(null, null, dropdown, null);
                        myApplicationChargesLabel.CompoundDrawablePadding = (int)DPUtils.ConvertDPToPx(4f);

                        expandableContainer.Visibility = isExpand ? ViewStates.Visible : ViewStates.Gone;
                    }
                    break;
                case ExpandableTextViewType.APPLICATION_STATUS:
                    {
                        myApplicationChargesLabel.SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
                    }
                    break;
                case ExpandableTextViewType.GSL_INFO:
                    {
                        dropdown = ContextCompat.GetDrawable(mContext, isExpand ? Resource.Drawable.ic_action_expand_down_up : Resource.Drawable.ic_action_expand_down);
                        expandableGSLTitle.SetCompoundDrawablesWithIntrinsicBounds(null, null, dropdown, null);
                        expandableGSLTitle.CompoundDrawablePadding = (int)DPUtils.ConvertDPToPx(4f);

                        expandedGSLContent.Visibility = isExpand ? ViewStates.Visible : ViewStates.Gone;
                    }
                    break;
                default:
                    break;
            }
        }

        public void SetOtherCharges(double totalAmount, List<ChargeModel> chargeList)
        {
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            myApplicationChargesValue.Text = "RM " + totalAmount.ToString("#,##0.00", currCult);
            chargeList.ForEach(charge =>
            {
                if (charge.Amount > 0.00)
                {
                    LinearLayout item = (LinearLayout)LayoutInflater.From(mContext).Inflate(Resource.Layout.MyOtherChargesItemLayout, this, false);
                    TextView textView = item.FindViewById<TextView>(Resource.Id.otherChargeItem);
                    TextView textValue = item.FindViewById<TextView>(Resource.Id.otherChargeValue);

                    textView.Text = charge.Title;
                    textValue.Text = "RM" + charge.Amount.ToString("#,##0.00", currCult);
                    TextViewUtils.SetMuseoSans300Typeface(textView, textValue);
                    TextViewUtils.SetTextSize14(textView, textValue);
                    expandableContainer.AddView(item);
                }
            });
            expandableContainer.Invalidate();
            expandableContainer.RequestLayout();
        }

        public void SetApplicationOtherCharges(string oneTimeCharges, string totalAmount, List<ChargeModel> chargeList)
        {
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            myApplicationChargesValue.Text = totalAmount;
            myApplicationChargesLabel.Text = oneTimeCharges;
            chargeList.ForEach(charge =>
            {
                if (charge.AmountDisplay != string.Empty)
                {
                    LinearLayout item = (LinearLayout)LayoutInflater.From(mContext).Inflate(Resource.Layout.MyOtherChargesItemLayout, this, false);
                    TextView textView = item.FindViewById<TextView>(Resource.Id.otherChargeItem);
                    TextView textValue = item.FindViewById<TextView>(Resource.Id.otherChargeValue);

                    textView.Text = charge.Title;
                    textValue.Text = charge.AmountDisplay;
                    TextViewUtils.SetMuseoSans300Typeface(textView, textValue);
                    TextViewUtils.SetTextSize14(textView, textValue);
                    expandableContainer.AddView(item);
                }
            });
            expandableContainer.Invalidate();
            expandableContainer.RequestLayout();
            expandableContainer.Visibility = ViewStates.Visible;
            myApplicationChargesLabel.SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
        }

        public void SetExpandableGSLTitle(string title)
        {
            expandableGSLTitle.Text = title;
        }

        public void SetExpandableGSLContent(string content)
        {
            LinearLayout item = (LinearLayout)LayoutInflater.From(mContext).Inflate(Resource.Layout.ExpandableContentView, this, false);
            TextView textView = item.FindViewById<TextView>(Resource.Id.txtExpandableContent);

            TextViewUtils.SetMuseoSans300Typeface(textView);
            TextViewUtils.SetTextSize14(textView);

            try
            {
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    textView.TextFormatted = Html.FromHtml(content, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    textView.TextFormatted = Html.FromHtml(content);
                }

                textView = LinkRedirectionUtils
                    .Create(this.mActivity, Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_TITLE))
                    .SetTextView(textView)
                    .SetMessage(content)
                    .Build()
                    .GetProcessedTextView();

                dropdown = ContextCompat.GetDrawable(mContext, Resource.Drawable.ic_action_expand_down);
                expandableGSLTitle.SetCompoundDrawablesWithIntrinsicBounds(null, null, dropdown, null);
                expandableGSLTitle.CompoundDrawablePadding = (int)DPUtils.ConvertDPToPx(4f);
                expandedGSLContent.AddView(item);
                expandedGSLContent.Invalidate();
                expandedGSLContent.RequestLayout();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public class OnExpandListener : Java.Lang.Object, IOnClickListener
        {
            ExpandableTextViewComponent ownerView;
            bool isVisible = true;
            public OnExpandListener(ExpandableTextViewComponent view)
            {
                ownerView = view;
            }
            public void OnClick(View v)
            {
                if (!isVisible)
                {
                    ownerView.OnClickExpand(false);
                }
                else
                {
                    ownerView.OnClickExpand(true);
                }
                isVisible = !isVisible;
            }
        }
    }
}
