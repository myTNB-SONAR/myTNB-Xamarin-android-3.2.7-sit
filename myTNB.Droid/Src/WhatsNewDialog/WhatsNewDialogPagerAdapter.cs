using Android.Content;
using Android.Graphics;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Facebook.Shimmer;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.WhatsNewDialog
{
    public class WhatsNewDialogPagerAdapter : PagerAdapter
    {
        private Context mContext;
        private List<WhatsNewModel> whatsnew = new List<WhatsNewModel>();
        public event EventHandler<int> DetailsClicked;
        public event EventHandler<int> CloseClicked;
        public event EventHandler<int> RefreshIndicator;

        public WhatsNewDialogPagerAdapter(Context ctx, List<WhatsNewModel> items)
        {
            this.mContext = ctx;
            this.whatsnew = items;
        }

        public WhatsNewDialogPagerAdapter()
        {

        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            ViewGroup rootView = (ViewGroup)LayoutInflater.From(mContext).Inflate(Resource.Layout.WhatsNewPagerItemLayout, container, false);
            Button btnSkip = (Button)rootView.FindViewById(Resource.Id.btnWhatsNewSkip);
            Button btnDetails = (Button)rootView.FindViewById(Resource.Id.btnWhatsNewDetails);
            ImageButton btnClose = (ImageButton)rootView.FindViewById(Resource.Id.btnWhatsNewClose);
            ImageView imgWhatsNew = (ImageView)rootView.FindViewById(Resource.Id.image_whatsnew);
            LinearLayout whatsNewMainImgLayout = (LinearLayout)rootView.FindViewById(Resource.Id.whatsNewMainShimmerImgLayout);
            ShimmerFrameLayout shimmerWhatsNewImageLayout = (ShimmerFrameLayout)rootView.FindViewById(Resource.Id.shimmerWhatsNewImageLayout);

            TextViewUtils.SetMuseoSans300Typeface(btnSkip, btnDetails);

            WhatsNewModel model = whatsnew[position];

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

            btnSkip.Text = Utility.GetLocalizedCommonLabel("skip");
            btnDetails.Text = Utility.GetLocalizedCommonLabel("details");

            btnClose.Click += delegate
            {
                OnCloseClick(position);
            };
            btnSkip.Click += delegate
            {
                SkipWhatsNew(position);
            };


            btnDetails.Click += delegate
            {
                OnDetailsClick(position);
                this.whatsnew.RemoveAt(position);
                this.NotifyDataSetChanged();
                OnRefreshIndicator(position);
                if (this.whatsnew.Count == 0)
                {
                    OnCloseClick(position);
                }
            };

            container.AddView(rootView);
            return rootView;
        }

        private void SkipWhatsNew(int pos)
        {
            string id = this.whatsnew[pos].ID;
            WhatsNewEntity wtManager = new WhatsNewEntity();
            wtManager.UpdateDialogSkipItem(id, true);

            this.whatsnew.RemoveAt(pos);
            this.NotifyDataSetChanged();
            OnRefreshIndicator(pos);
            if (this.whatsnew.Count == 0)
            {
                OnCloseClick(pos);
            }
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
            if (CloseClicked != null)
                CloseClicked(this, position);
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