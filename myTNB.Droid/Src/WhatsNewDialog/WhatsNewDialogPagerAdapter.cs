using Android.Content;
using Android.Graphics;
using Android.OS;



using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.Core.Content;
using AndroidX.ViewPager.Widget;
using Facebook.Shimmer;
using Java.Util.Regex;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.WhatsNewDetail.MVP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.WhatsNewDialog
{
    public class WhatsNewDialogPagerAdapter : PagerAdapter
    {
        private Android.App.Activity mContext;
        private List<WhatsNewModel> whatsnew = new List<WhatsNewModel>();
        public event EventHandler<int> DetailsClicked;
        public event EventHandler<int> CloseClicked;
        public event EventHandler<int> RefreshIndicator;
        private bool isTextOnly = false;
        private bool isPhotoOnly = true;

        public WhatsNewDialogPagerAdapter(Android.App.Activity ctx, List<WhatsNewModel> items)
        {
            this.mContext = ctx;
            this.whatsnew = items;
        }

        public WhatsNewDialogPagerAdapter()
        {

        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            WhatsNewModel model = whatsnew[position];

            ViewGroup rootView = (ViewGroup)LayoutInflater.From(mContext).Inflate(Resource.Layout.WhatsNewPagerItemLayout, container, false);

            if (!string.IsNullOrEmpty(model.PortraitImage_PopUp))
            {
                FrameLayout whatsNewDialogCardView = (FrameLayout)rootView.FindViewById(Resource.Id.layout_image_holder);
                Button btnGotIt = (Button)rootView.FindViewById(Resource.Id.btnWhatsNewGotIt);
                ImageView imgWhatsNew = (ImageView)rootView.FindViewById(Resource.Id.image_whatsnew);
                LinearLayout whatsNewCheckBoxLayout = (LinearLayout)rootView.FindViewById(Resource.Id.whatsNewCheckBoxLayout);
                CheckBox chkDontShow = (CheckBox)rootView.FindViewById(Resource.Id.chk_remember_me);

                LinearLayout whatsNewMainImgLayout = (LinearLayout)rootView.FindViewById(Resource.Id.whatsNewMainShimmerImgLayout);
                ShimmerFrameLayout shimmerWhatsNewImageLayout = (ShimmerFrameLayout)rootView.FindViewById(Resource.Id.shimmerWhatsNewImageLayout);

                int maxHeight = GetDeviceVerticalScaleInPixel(0.732f);

                if (mContext.Resources.DisplayMetrics.HeightPixels >= 1920)
                {
                    maxHeight = GetDeviceVerticalScaleInPixel(0.632f);
                }

                try
                {
                    float scaledWidth = mContext.Resources.DisplayMetrics.WidthPixels / mContext.Resources.DisplayMetrics.Density;

                    if (scaledWidth >= 430)
                    {
                        maxHeight = GetDeviceVerticalScaleInPixel(0.532f);
                    }
                }
                catch (Exception ex)
                {
                    Utility.LoggingNonFatalError(ex);
                }

                whatsNewDialogCardView.LayoutParameters.Height = maxHeight;
                whatsNewDialogCardView.RequestLayout();
                btnGotIt.RequestLayout();
                rootView.RequestLayout();

                TextViewUtils.SetMuseoSans500Typeface(btnGotIt, chkDontShow);
                chkDontShow.Text = Utility.GetLocalizedCommonLabel("dontShowThisAgain");

                if (shimmerWhatsNewImageLayout.IsShimmerStarted)
                {
                    shimmerWhatsNewImageLayout.StopShimmer();
                }
                var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                if (shimmerBuilder != null)
                {
                    shimmerWhatsNewImageLayout.SetShimmer(shimmerBuilder?.Build());
                }
                shimmerWhatsNewImageLayout.StartShimmer();

                if (!string.IsNullOrEmpty(model.PortraitImage_PopUpB64))
                {
                    Bitmap localBitmap = Base64ToBitmap(model.PortraitImage_PopUpB64);
                    if (localBitmap != null)
                    {
                        model.PortraitImage_PopUpBitmap = localBitmap;
                        SetWhatsNewDialogImage(localBitmap, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                    }
                    else
                    {
                        // WhatsNew TODO: set default img
                    }
                }
                else if (!string.IsNullOrEmpty(model.PortraitImage_PopUp))
                {
                    _ = GetImageAsync(model, position, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                }
                else
                {
                    // WhatsNew TODO: set default img
                }

                btnGotIt.Text = Utility.GetLocalizedCommonLabel("gotIt");

                btnGotIt.Click += delegate
                {
                    OnCloseClick(position);
                };

                if (!model.Donot_Show_In_WhatsNew)
                {
                    imgWhatsNew.Click += delegate
                    {
                        OnDetailsClick(position);
                        OnCloseClick(position);
                    };
                }

                if (model.Disable_DoNotShow_Checkbox)
                {
                    whatsNewCheckBoxLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    whatsNewCheckBoxLayout.Visibility = ViewStates.Visible;
                    chkDontShow.Checked = model.SkipShowOnAppLaunch;
                    chkDontShow.Click += delegate
                    {
                        SkipWhatsNew(position, chkDontShow.Checked);
                    };
                }
            }
            else
            {
                rootView = (ViewGroup)LayoutInflater.From(mContext).Inflate(Resource.Layout.WhatsNewPagerTextItemLayout, container, false);

                CardView whatsNewCardView = (CardView)rootView.FindViewById(Resource.Id.whatsNewDialogCardView);

                
                LinearLayout whatsNewDialogMainView = (LinearLayout)rootView.FindViewById(Resource.Id.whatsNewDialogMainView);
                LinearLayout whatsNewMainImgLayout = (LinearLayout)rootView.FindViewById(Resource.Id.whatsNewMainShimmerImgLayout);
                ShimmerFrameLayout shimmerWhatsNewImageLayout = (ShimmerFrameLayout)rootView.FindViewById(Resource.Id.shimmerWhatsNewImageLayout);

                ImageView imgWhatsNew = (ImageView)rootView.FindViewById(Resource.Id.image_whatsnew);

                TextView txtWhatsNewTitle = (TextView)rootView.FindViewById(Resource.Id.txtWhatsNewTitle);
                TextView txtWhatsNewMessage = (TextView)rootView.FindViewById(Resource.Id.txtWhatsNewMessage);

                LinearLayout whatsNewCheckBoxLayout = (LinearLayout)rootView.FindViewById(Resource.Id.whatsNewCheckBoxLayout);
                CheckBox chkDontShow = (CheckBox)rootView.FindViewById(Resource.Id.chk_remember_me);

                Button btnGotIt = (Button)rootView.FindViewById(Resource.Id.btnWhatsNewGotIt);

                if (model.PopUp_Text_Only)
                {
                    whatsNewMainImgLayout.Visibility = ViewStates.Gone;
                    imgWhatsNew.Visibility = ViewStates.Visible;
                    imgWhatsNew.SetScaleType(ImageView.ScaleType.CenterCrop);

                    int photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - GetDeviceHorizontalScaleInPixel(0.096f);
                    if (mContext.Resources.DisplayMetrics.HeightPixels >= 2500)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 8 * GetDeviceHorizontalScaleInPixel(0.006f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 2200)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 6 * GetDeviceHorizontalScaleInPixel(0.016f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 1920)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 4 * GetDeviceHorizontalScaleInPixel(0.036f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 1080)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 2 * GetDeviceHorizontalScaleInPixel(0.006f);
                    }
                    float photoRatio = 0.4929f;
                    int photoHeight = (int)(photoWidth * photoRatio);

                    imgWhatsNew.SetImageResource(Resource.Drawable.ic_banner_whatsnewdialog);
                    imgWhatsNew.LayoutParameters.Height = photoHeight;

                    imgWhatsNew.RequestLayout();
                }
                else
                {
                    whatsNewMainImgLayout.Visibility = ViewStates.Visible;
                    imgWhatsNew.Visibility = ViewStates.Gone;

                    if (shimmerWhatsNewImageLayout.IsShimmerStarted)
                    {
                        shimmerWhatsNewImageLayout.StopShimmer();
                    }
                    var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                    if (shimmerBuilder != null)
                    {
                        shimmerWhatsNewImageLayout.SetShimmer(shimmerBuilder?.Build());
                    }
                    shimmerWhatsNewImageLayout.StartShimmer();

                    // WhatsNew TODO: To handle header image
                    if (!string.IsNullOrEmpty(model.PopUp_HeaderImageB64))
                    {
                        Bitmap localBitmap = Base64ToBitmap(model.PopUp_HeaderImageB64);
                        if (localBitmap != null)
                        {
                            model.PopUp_HeaderImageBitmap = localBitmap;
                            SetWhatsNewDialogTextImage(localBitmap, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                        }
                        else
                        {
                            SetWhatsNewDialogTextImage(null, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.PopUp_HeaderImage))
                    {
                        _ = GetTextImageAsync(model, position, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                    }
                    else
                    {
                        SetWhatsNewDialogTextImage(null, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                    }
                }

                txtWhatsNewTitle.Visibility = ViewStates.Gone;

                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtWhatsNewMessage.TextFormatted = Html.FromHtml(model.PopUp_Text_Content, FromHtmlOptions.ModeCompact);
                }
                else
                {
                    txtWhatsNewMessage.TextFormatted = Html.FromHtml(model.PopUp_Text_Content);
                }

                txtWhatsNewMessage = LinkRedirectionUtils
                    .Create(this.mContext, "")
                    .SetTextView(txtWhatsNewMessage)
                    .SetMessage(model.PopUp_Text_Content)
                    .Build()
                    .GetProcessedTextView();

                btnGotIt.RequestLayout();
                rootView.RequestLayout();
                whatsNewCardView.RequestLayout();

                TextViewUtils.SetMuseoSans300Typeface(txtWhatsNewMessage);
                TextViewUtils.SetMuseoSans500Typeface(txtWhatsNewTitle);
                TextViewUtils.SetMuseoSans500Typeface(btnGotIt, chkDontShow);
                chkDontShow.Text = Utility.GetLocalizedCommonLabel("dontShowThisAgain");

                btnGotIt.Text = Utility.GetLocalizedCommonLabel("gotIt");

                if (!model.Donot_Show_In_WhatsNew)
                {
                    whatsNewDialogMainView.Click += delegate
                    {
                        OnDetailsClick(position);
                        OnCloseClick(position);
                    };
                }

                btnGotIt.Click += delegate
                {
                    OnCloseClick(position);
                };

                if (model.Disable_DoNotShow_Checkbox)
                {
                    whatsNewCheckBoxLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    whatsNewCheckBoxLayout.Visibility = ViewStates.Visible;
                    chkDontShow.Checked = model.SkipShowOnAppLaunch;
                    chkDontShow.Click += delegate
                    {
                        SkipWhatsNew(position, chkDontShow.Checked);
                    };
                }
            }

            container.AddView(rootView);
            return rootView;
        }

        private async Task GetTextImageAsync(WhatsNewModel item, int position, ShimmerFrameLayout shimmerWhatsNewImageLayout, LinearLayout whatsNewMainImgLayout, ImageView imgWhatsNew)
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                Bitmap imageBitmap = null;
                await Task.Run(() =>
                {
                    imageBitmap = GetImageBitmapFromUrl(item.PopUp_HeaderImage);
                }, cts.Token);

                if (imageBitmap != null)
                {
                    item.PopUp_HeaderImageBitmap = imageBitmap;
                    item.PopUp_HeaderImageB64 = BitmapToBase64(imageBitmap);
                    this.whatsnew[position].PopUp_HeaderImageBitmap = item.PopUp_HeaderImageBitmap;
                    this.whatsnew[position].PopUp_HeaderImageB64 = item.PopUp_HeaderImageB64;
                    WhatsNewEntity wtManager = new WhatsNewEntity();
                    wtManager.UpdateCachePopupHeaderImage(item.ID, item.PopUp_HeaderImageB64);
                    SetWhatsNewDialogTextImage(imageBitmap, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                }
                else
                {
                    SetWhatsNewDialogTextImage(null, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SetWhatsNewDialogTextImage(Bitmap imgSrc, ShimmerFrameLayout shimmerWhatsNewImageLayout, LinearLayout whatsNewMainImgLayout, ImageView imgWhatsNew)
        {
            try
            {
                if (imgSrc == null)
                {
                    int photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - GetDeviceHorizontalScaleInPixel(0.096f);
                    if (mContext.Resources.DisplayMetrics.HeightPixels >= 2500)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 8 * GetDeviceHorizontalScaleInPixel(0.006f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 2200)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 6 * GetDeviceHorizontalScaleInPixel(0.016f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 1920)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 4 * GetDeviceHorizontalScaleInPixel(0.036f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 1080)
                    {
                        photoWidth = mContext.Resources.DisplayMetrics.WidthPixels - 2 * GetDeviceHorizontalScaleInPixel(0.006f);
                    }
                    float photoRatio = 0.4929f;
                    int photoHeight = (int)(photoWidth * photoRatio);

                    imgWhatsNew.SetScaleType(ImageView.ScaleType.CenterCrop);
                    imgWhatsNew.SetImageResource(Resource.Drawable.ic_banner_whatsnewdialog);
                    imgWhatsNew.LayoutParameters.Height = photoHeight;

                    imgWhatsNew.RequestLayout();
                }
                else if (imgSrc != null)
                {
                    float currentImgWidth = mContext.Resources.DisplayMetrics.WidthPixels - GetDeviceHorizontalScaleInPixel(0.096f);
                    if (mContext.Resources.DisplayMetrics.HeightPixels >= 2500)
                    {
                        currentImgWidth = mContext.Resources.DisplayMetrics.WidthPixels - 8 * GetDeviceHorizontalScaleInPixel(0.006f);
                    }
                    else if(mContext.Resources.DisplayMetrics.HeightPixels >= 2200)
                    {
                        currentImgWidth = mContext.Resources.DisplayMetrics.WidthPixels - 6 * GetDeviceHorizontalScaleInPixel(0.016f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 1920)
                    {
                        currentImgWidth = mContext.Resources.DisplayMetrics.WidthPixels - 4 * GetDeviceHorizontalScaleInPixel(0.036f);
                    }
                    else if (mContext.Resources.DisplayMetrics.HeightPixels >= 1080)
                    {
                        currentImgWidth = mContext.Resources.DisplayMetrics.WidthPixels - 2 * GetDeviceHorizontalScaleInPixel(0.006f);
                    }
                    float calImgRatio = currentImgWidth / imgSrc.Width;
                    int currentImgHeight = (int)(imgSrc.Height * calImgRatio);

                    imgWhatsNew.SetScaleType(ImageView.ScaleType.CenterCrop);
                    imgWhatsNew.SetImageBitmap(imgSrc);
                    imgWhatsNew.LayoutParameters.Height = currentImgHeight;
                    imgWhatsNew.RequestLayout();
                }

                whatsNewMainImgLayout.Visibility = ViewStates.Gone;
                if (shimmerWhatsNewImageLayout.IsShimmerStarted)
                {
                    shimmerWhatsNewImageLayout.StopShimmer();
                }

                imgWhatsNew.Visibility = ViewStates.Visible;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public int GetDeviceHorizontalScaleInPixel(float percentageValue)
        {
            var deviceWidth = mContext.Resources.DisplayMetrics.WidthPixels;
            return GetScaleInPixel(deviceWidth, percentageValue);
        }

        public int GetDeviceVerticalScaleInPixel(float percentageValue)
        {
            var deviceHeight = mContext.Resources.DisplayMetrics.HeightPixels;
            return GetScaleInPixel(deviceHeight, percentageValue);
        }

        public int GetScaleInPixel(int basePixel, float percentageValue)
        {
            int scaledInPixel = (int)((float)basePixel * percentageValue);
            return scaledInPixel;
        }

        private void SkipWhatsNew(int pos, bool isCheck)
        {
            string id = this.whatsnew[pos].ID;
            this.whatsnew[pos].SkipShowOnAppLaunch = isCheck;
            this.NotifyDataSetChanged();
            WhatsNewEntity wtManager = new WhatsNewEntity();
            wtManager.UpdateDialogSkipItem(id, isCheck);
        }

        private async Task GetImageAsync(WhatsNewModel item, int position, ShimmerFrameLayout shimmerWhatsNewImageLayout, LinearLayout whatsNewMainImgLayout, ImageView imgWhatsNew)
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                Bitmap imageBitmap = null;
                await Task.Run(() =>
                {
                    imageBitmap = GetImageBitmapFromUrl(item.PortraitImage_PopUp);
                }, cts.Token);

                if (imageBitmap != null)
                {
                    item.PortraitImage_PopUpBitmap = imageBitmap;
                    item.PortraitImage_PopUpB64 = BitmapToBase64(imageBitmap);
                    this.whatsnew[position].PortraitImage_PopUpBitmap = item.PortraitImage_PopUpBitmap;
                    this.whatsnew[position].PortraitImage_PopUpB64 = item.PortraitImage_PopUpB64;
                    WhatsNewEntity wtManager = new WhatsNewEntity();
                    wtManager.UpdateCachePopupImage(item.ID, item.PortraitImage_PopUpB64);
                    SetWhatsNewDialogImage(imageBitmap, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                }
                else
                {
                    // WhatsNew TODO: set default img
                    // SetWhatsNewDialogImage(null, shimmerWhatsNewImageLayout, whatsNewMainImgLayout, imgWhatsNew);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private string BitmapToBase64(Bitmap bitmap)
        {
            string B64Output = "";
            try
            {
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();
                B64Output = Android.Util.Base64.EncodeToString(byteArray, Base64Flags.Default);
            }
            catch (Exception e)
            {
                B64Output = "";
                Utility.LoggingNonFatalError(e);
            }

            return B64Output;
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

        private void SetWhatsNewDialogImage(Bitmap imgSrc, ShimmerFrameLayout shimmerWhatsNewImageLayout, LinearLayout whatsNewMainImgLayout, ImageView imgWhatsNew)
        {
            try
            {
                if (imgSrc == null)
                {
                    /*BitmapFactory.Options opt = new BitmapFactory.Options();
                    opt.InMutable = true;

                    Bitmap mDefaultBitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.promotions_default_image, opt);

                    whatsNewImg.SetImageBitmap(mDefaultBitmap);*/
                    // WhatsNew TODO: set default img
                }
                else if (imgSrc != null)
                {
                    imgWhatsNew.SetImageBitmap(imgSrc);
                }

                whatsNewMainImgLayout.Visibility = ViewStates.Gone;
                if (shimmerWhatsNewImageLayout.IsShimmerStarted)
                {
                    shimmerWhatsNewImageLayout.StopShimmer();
                }

                imgWhatsNew.Visibility = ViewStates.Visible;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap = null;
            try
            {
                byte[] imageAsBytes = Android.Util.Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
        }

        void OnDetailsClick(int position)
        {
            if (DetailsClicked != null)
                DetailsClicked(this, position);
        }

        void OnCloseClick(int position)
        {
            this.whatsnew.RemoveAt(position);
            this.NotifyDataSetChanged();
            OnRefreshIndicator(position);
            if (this.whatsnew.Count == 0)
            {
                if (CloseClicked != null)
                    CloseClicked(this, position);
            }
        }

        void OnRefreshIndicator(int position)
        {
            if (RefreshIndicator != null)
            {
                RefreshIndicator(this, position);
            }
        }

        public override int GetItemPosition(Java.Lang.Object @object)
        {
            return PagerAdapter.PositionNone;
        }

        public override int Count => whatsnew.Count;

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((View)@object);
        }

    }
}