using Android.Widget;
using Android.Content;
using Android.Util;
using Android.OS;
using Android.Text;
using myTNB.Android.Src.Utils;
using Android.Graphics;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Threading;
using Android.Views;

namespace myTNB.Android.Src.Bills.NewBillRedesign.Fragment
{
    public class NBRDiscoverMoreListItemComponent : LinearLayout
    {
        private readonly Context mContext;
        public TextView itemContentText;
        TextView itemNumberText, itemTitleText;
        ImageView itemBannerView;
        LinearLayout bannerShimmerLayout;
        private string placeholderImage;

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
            bannerShimmerLayout = FindViewById<LinearLayout>(Resource.Id.bannerShimmerContainer);

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
            try
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
            catch (Exception e)
            {
                ShowImagePlaceholder();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetBannerImage(string imageName)
        {
            if (imageName.IsValid())
            {
                _ = GetImageAsync(imageName);
            }
            else
            {
                HideBannerImage();
            }
        }

        public void SetBannerPlaceholder(string placeholderImg)
        {
            placeholderImage = placeholderImg;
        }

        public async Task GetImageAsync(string imageName)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Bitmap imageBitmap = null;
            try
            {
                await Task.Run(() =>
                {
                    imageBitmap = GetImageBitmapFromUrl(imageName);
                }, cts.Token);

                if (imageBitmap != null)
                {
                    itemBannerView.SetImageBitmap(imageBitmap);
                    bannerShimmerLayout.Visibility = ViewStates.Gone;
                    itemBannerView.Visibility = ViewStates.Visible;
                }
                else
                {
                    ShowImagePlaceholder();
                }
            }
            catch (Exception e)
            {
                ShowImagePlaceholder();
                Utility.LoggingNonFatalError(e);
            }
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap image = null;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        image = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return image;
        }

        private void ShowImagePlaceholder()
        {
            if (placeholderImage.IsValid())
            {
                try
                {
                    bannerShimmerLayout.Visibility = ViewStates.Gone;
                    itemBannerView.Visibility = ViewStates.Visible;
                    var resImage = (int)typeof(Resource.Drawable).GetField(placeholderImage).GetValue(null);
                    if (resImage > 0)
                    {
                        itemBannerView.SetImageResource(resImage);
                    }
                    else
                    {
                        HideBannerImage();
                    }
                }
                catch (Exception e)
                {
                    HideBannerImage();
                    Utility.LoggingNonFatalError(e);
                }
            }
            else
            {
                HideBannerImage();
            }
        }

        private void HideBannerImage()
        {
            bannerShimmerLayout.Visibility = ViewStates.Gone;
            itemBannerView.Visibility = ViewStates.Gone;
        }
    }
}