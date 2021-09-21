using Android.Widget;
using Android.Content;
using Android.Util;
using Android.OS;
using Android.Text;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Bills.NewBillRedesign.Fragment
{
    public class NBRDiscoverMoreListItemComponent : LinearLayout
    {
        private readonly Context mContext;
        TextView itemNumberText, itemTitleText, itemContentText;
        ImageView itemBannerView;

        public NBRDiscoverMoreListItemComponent(Context context) : base(context)
        {
            mContext = context;
            Init(mContext);
        }

        public NBRDiscoverMoreListItemComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            Init(mContext);
        }

        public NBRDiscoverMoreListItemComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            mContext = context;
            Init(mContext);
        }

        public void Init(Context context)
        {
            Inflate(context, Resource.Layout.NBRDiscoverMoreListItemView, this);
            itemNumberText = FindViewById<TextView>(Resource.Id.nbrDiscoverMoreNumber);
            itemTitleText = FindViewById<TextView>(Resource.Id.nbrDiscoverMoreTitle);
            itemBannerView = FindViewById<ImageView>(Resource.Id.nbrDiscoverMoreBanner);
            itemContentText = FindViewById<TextView>(Resource.Id.nbrDiscoverMoreContent);

            SetUpViews();
        }

        private void SetUpViews()
        {
            if (itemNumberText != null)
            {
                TextViewUtils.SetMuseoSans500Typeface(itemNumberText);
                TextViewUtils.SetTextSize12(itemNumberText);
            }

            if (itemTitleText != null)
            {
                TextViewUtils.SetMuseoSans500Typeface(itemTitleText);
                TextViewUtils.SetTextSize14(itemTitleText);
            }

            if (itemContentText != null)
            {
                TextViewUtils.SetMuseoSans300Typeface(itemContentText);
                TextViewUtils.SetTextSize12(itemContentText);
            }
        }

        public void SetItemNumber(string itemNo)
        {
            itemNumberText.Text = itemNo;
        }

        public void SetItemTitle(string itemTitle)
        {
            itemTitleText.Text = itemTitle;
        }

        public void SetItemContent(string itemContent)
        {
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                itemContentText.TextFormatted = Html.FromHtml(itemContent, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                itemContentText.TextFormatted = Html.FromHtml(itemContent);
            }
        }
        public void SetBannerImage(string imageName)
        {
            int resImage = Resources.GetIdentifier(imageName, "drawable", Context.PackageName);
            itemBannerView.SetImageResource(resImage);
        }
    }
}

