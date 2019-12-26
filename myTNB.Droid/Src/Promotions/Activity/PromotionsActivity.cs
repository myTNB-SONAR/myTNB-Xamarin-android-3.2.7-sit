using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.SitecoreCM.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using Square.Picasso;
using System;
using System.Net;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Promotions.Activity
{
    [Activity(Label = "@string/promotion_menu_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Promotions")]
    public class PromotionsActivity : BaseToolbarAppCompatActivity
    {
        private PromotionsModelV2 model;

        [BindView(Resource.Id.promotion_title)]
        public TextView textPromotionTitle;

        [BindView(Resource.Id.promotion_description)]
        public TextView textPromotionDes;

        [BindView(Resource.Id.promotion_campaign_period_label)]
        public TextView textCampaignLabel;

        [BindView(Resource.Id.promotion_campaign_period)]
        public TextView textCampaign;


        [BindView(Resource.Id.promotion_prizes_label)]
        public TextView textPrizesLabel;

        [BindView(Resource.Id.promotion_prizes)]
        public TextView textPrizes;

        [BindView(Resource.Id.promotion_more_info)]
        public TextView textPromotionInfo;

        [BindView(Resource.Id.promotion_img)]
        public ImageView imgPromotion;

        [BindView(Resource.Id.progressBar)]
        public ProgressBar mProgressBar;

        public override int ResourceId()
        {
            return Resource.Layout.PromotionDetailsView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            try
            {
                // Create your application here
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey("Promotion"))
                    {
                        //model = JsonConvert.DeserializeObject<PromotionsModelV2>(Intent.Extras.GetString("Promotion"));
                        model = DeSerialze<PromotionsModelV2>(extras.GetString("Promotion"));
                    }
                }

                TextViewUtils.SetMuseoSans300Typeface(textPromotionTitle, textPromotionDes, textCampaign, textPrizes, textPromotionInfo);
                TextViewUtils.SetMuseoSans500Typeface(textPromotionTitle, textCampaignLabel, textPrizesLabel);

                if (model != null)
                {
                    textPromotionTitle.Text = model.Title;
                    textPromotionDes.Text = model.SubText;
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                    {
                        textCampaign.TextFormatted = Html.FromHtml(model.HeaderContent, FromHtmlOptions.ModeLegacy);
                        textPrizes.TextFormatted = Html.FromHtml(model.BodyContent, FromHtmlOptions.ModeLegacy);
                        textPromotionInfo.TextFormatted = Html.FromHtml(model.FooterContent, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        textCampaign.TextFormatted = Html.FromHtml(model.HeaderContent);
                        textPrizes.TextFormatted = Html.FromHtml(model.BodyContent);
                        textPromotionInfo.TextFormatted = Html.FromHtml(model.FooterContent);
                    }

                    textCampaign.MovementMethod = Android.Text.Method.LinkMovementMethod.Instance;
                    textPrizes.MovementMethod = Android.Text.Method.LinkMovementMethod.Instance;
                    textPromotionInfo.MovementMethod = Android.Text.Method.LinkMovementMethod.Instance;

                    if (!string.IsNullOrEmpty(model.LandscapeImage))
                    {
                        if (model.LandscapeImage.Contains("jpeg"))
                        {
                            Picasso.With(imgPromotion.Context)
                           .Load(new Java.IO.File(model.LandscapeImage))
                           .Error(Resource.Drawable.promotions_default_image)
                           .Fit()
                           .Into(imgPromotion);
                            mProgressBar.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            GetImageAsync(imgPromotion, mProgressBar, model);
                        }
                    }
                    else
                    {
                        imgPromotion.SetImageResource(Resource.Drawable.promotions_default_image);
                        mProgressBar.Visibility = ViewStates.Gone;
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.PromotionDetailMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_share_promotion:
                    if (model != null)
                    {
                        Intent shareIntent = new Intent(Intent.ActionSend);
                        shareIntent.SetType("text/plain");
                        shareIntent.PutExtra(Intent.ExtraSubject, model.Title);
                        shareIntent.PutExtra(Intent.ExtraText, model.GeneralLinkUrl);
                        StartActivity(Intent.CreateChooser(shareIntent, Utility.GetLocalizedLabel("Profile", "share")));
                    }
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Promotion Detailed Info");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async Task GetImageAsync(ImageView icon, ProgressBar progressBar, PromotionsModelV2 item)
        {
            try
            {
                progressBar.Visibility = ViewStates.Visible;
                CancellationTokenSource cts = new CancellationTokenSource();
                Bitmap imageBitmap = null;
                await Task.Run(() =>
                {
                    imageBitmap = GetImageBitmapFromUrl(icon, item.LandscapeImage);
                }, cts.Token);

                if (imageBitmap != null)
                {
                    icon.SetImageBitmap(imageBitmap);
                }
                else
                {
                    icon.SetImageResource(Resource.Drawable.promotions_default_image);
                }

                progressBar.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Android.Graphics.Bitmap GetImageBitmapFromUrl(ImageView icon, string url)
        {
            Android.Graphics.Bitmap image = null;
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

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            SetResult(Result.Ok);
        }


        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

    }
}