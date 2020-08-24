using Android.Content;
using Android.Graphics;

using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB.SitecoreCM.Models;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Utils;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Promotions.Adapter
{
    public class PromotionPagerAdapter : PagerAdapter
    {
        private Context mContext;
        private List<PromotionsModelV2> promotions = new List<PromotionsModelV2>();
        public event EventHandler<int> DetailsClicked;
        public event EventHandler<int> CloseClicked;
        public event EventHandler<int> RefreshIndicator;

        public PromotionPagerAdapter(Context ctx, List<PromotionsModelV2> items)
        {
            this.mContext = ctx;
            this.promotions = items;
        }

        public PromotionPagerAdapter()
        {

        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            ViewGroup rootView = (ViewGroup)LayoutInflater.From(mContext).Inflate(Resource.Layout.promotion_pager_item, container, false);
            Button btnSkip = (Button)rootView.FindViewById(Resource.Id.btnPromoSkip);
            Button btnDetails = (Button)rootView.FindViewById(Resource.Id.btnPromoDetails);
            ImageButton btnClose = (ImageButton)rootView.FindViewById(Resource.Id.btnPromoClose);
            ImageView imgPromo = (ImageView)rootView.FindViewById(Resource.Id.image_promotion);

            TextViewUtils.SetMuseoSans300Typeface(btnSkip, btnDetails);

            PromotionsModelV2 model = promotions[position];

            if (model.PortraitImage != null)
            {
                if (model.PortraitImage.Contains("jpeg"))
                {
                    Log.Debug("Promotion Adapter", "Image path saved");
                    Picasso.With(imgPromo.Context)
                   .Load(new Java.IO.File(model.PortraitImage))
                   .Fit()
                   .Into(imgPromo);
                }
                else
                {
                    GetImageAsync(imgPromo, model);
                }
            }

            btnSkip.Text = Utility.GetLocalizedCommonLabel("skip");
            btnDetails.Text = Utility.GetLocalizedCommonLabel("details");

            btnClose.Click += delegate
            {
                SkipPromotion(position);
            };
            btnSkip.Click += delegate
            {
                SkipPromotion(position);
            };


            btnDetails.Click += delegate
            {
                OnDetailsClick(position);
                this.promotions.RemoveAt(position);
                this.NotifyDataSetChanged();
                OnRefreshIndicator(position);
                if (this.promotions.Count == 0 || this.AllPromoShown())
                {
                    OnCloseClick(position);
                }
            };

            container.AddView(rootView);
            return rootView;
        }

        private void SkipPromotion(int pos)
        {
            UpdatePromotionAsSeen(pos);
            this.promotions.RemoveAt(pos);
            this.NotifyDataSetChanged();
            OnRefreshIndicator(pos);
            if (this.promotions.Count == 0)
            {
                OnCloseClick(pos);
            }
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

        public override int Count => promotions.Count;

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((View)@object);
        }

        public void UpdatePromotionAsSeen(int position)
        {
            PromotionsModelV2 model = promotions[position];
            PromotionsEntityV2 entity = new PromotionsEntityV2()
            {
                ID = model.ID,
                GeneralLinkUrl = model.GeneralLinkUrl,
                Text = model.Text,
                Title = model.Title,
                HeaderContent = model.HeaderContent,
                BodyContent = model.BodyContent,
                FooterContent = model.FooterContent,
                PortraitImage = model.PortraitImage.Replace(" ", "%20"),
                LandscapeImage = model.LandscapeImage.Replace(" ", "%20"),
                PromoStartDate = model.PromoStartDate,
                PromoEndDate = model.PromoEndDate,
                PublishedDate = model.PublishedDate,
                IsPromoExpired = model.IsPromoExpired,
                Read = model.Read
            };
            PromotionsEntityV2 wtManger = new PromotionsEntityV2();
            wtManger.UpdateItemAsShown(entity);
        }

        public async Task GetImageAsync(ImageView icon, PromotionsModelV2 item)
        {
            //progressBar.Visibility = ViewStates.Visible;
            CancellationTokenSource cts = new CancellationTokenSource();
            Bitmap imageBitmap = null;
            await Task.Run(() =>
            {
                imageBitmap = GetImageBitmapFromUrl(icon, item.PortraitImage);
            }, cts.Token);

            if (imageBitmap != null)
            {
                icon.SetImageBitmap(imageBitmap);
                string filepath = await FileUtils.SaveAsync(imageBitmap, FileUtils.PROMO_IMAGE_FOLDER, string.Format("{0}.jpeg", item.Title));
                PromotionsEntityV2 wtManager = new PromotionsEntityV2()
                {
                    ID = item.ID,
                    GeneralLinkUrl = item.GeneralLinkUrl,
                    Text = item.Text,
                    Title = item.Title,
                    HeaderContent = item.HeaderContent,
                    BodyContent = item.BodyContent,
                    FooterContent = item.FooterContent,
                    PortraitImage = filepath,
                    LandscapeImage = item.LandscapeImage.Replace(" ", "%20"),
                    PromoStartDate = item.PromoStartDate,
                    PromoEndDate = item.PromoEndDate,
                    PublishedDate = item.PublishedDate,
                    IsPromoExpired = item.IsPromoExpired
                };
                wtManager.UpdateItem(wtManager);
            }

            //progressBar.Visibility = ViewStates.Gone;
        }

        private Android.Graphics.Bitmap GetImageBitmapFromUrl(ImageView icon, string url)
        {
            Bitmap image = null;
            using (WebClient webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    image = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            return image;
        }

        public bool AllPromoShown()
        {
            bool flag = false;

            foreach (PromotionsModelV2 item in promotions)
            {
                if (item.PromoShown)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                    break;
                }
            }

            return flag;
        }

    }
}