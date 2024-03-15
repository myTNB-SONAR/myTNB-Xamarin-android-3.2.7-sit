using System;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Widget;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.DigitalSignature.IdentityVerification.Fragment
{
    public class DSIdentityVerificationListItemComponent : LinearLayout
    {
        private readonly Context mContext;
        TextView itemTitleText, itemDescText;
        ImageView itemIconView;

        public DSIdentityVerificationListItemComponent(Context context) : base(context)
        {
            mContext = context;
            Init(mContext);
        }

        public DSIdentityVerificationListItemComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            Init(mContext);
        }

        public DSIdentityVerificationListItemComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            mContext = context;
            Init(mContext);
        }

        public void Init(Context context)
        {
            Inflate(context, Resource.Layout.DSIdentityVerificationListItemView, this);
            itemTitleText = FindViewById<TextView>(Resource.Id.identityVerListItemTitle);
            itemDescText = FindViewById<TextView>(Resource.Id.identityVerListItemDesc);
            itemIconView = FindViewById<ImageView>(Resource.Id.identityVerListItemImage);

            SetUpViews();
        }

        private void SetUpViews()
        {
            if (itemTitleText != null)
            {
                TextViewUtils.SetMuseoSans500Typeface(itemTitleText);
                TextViewUtils.SetTextSize12(itemTitleText);
            }

            if (itemDescText != null)
            {
                TextViewUtils.SetMuseoSans300Typeface(itemDescText);
                TextViewUtils.SetTextSize12(itemDescText);
            }
        }

        public void SetItemTitleText(string itemTitle)
        {
            try
            {
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    itemTitleText.TextFormatted = Html.FromHtml(itemTitle, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    itemTitleText.TextFormatted = Html.FromHtml(itemTitle);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetItemDescText(string itemDesc)
        {
            try
            {
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    itemDescText.TextFormatted = Html.FromHtml(itemDesc, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    itemDescText.TextFormatted = Html.FromHtml(itemDesc);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetItemIcon(int resImage)
        {
            itemIconView.SetImageResource(resImage);
        }
    }
}
