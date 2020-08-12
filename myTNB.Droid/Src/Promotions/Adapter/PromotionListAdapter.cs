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
    public class PromotionListAdapter : RecyclerView.Adapter
    {

        private Android.App.Activity mActicity;
        private List<PromotionsModelV2> promotionList = new List<PromotionsModelV2>();
        private List<PromotionsModelV2> orgPromotionList = new List<PromotionsModelV2>();
        public event EventHandler<int> ItemClick;

        public PromotionListAdapter(Android.App.Activity activity, List<PromotionsModelV2> data)
        {
            this.mActicity = activity;
            this.promotionList.AddRange(data);
            this.orgPromotionList.AddRange(data);
        }

        public override int ItemCount => promotionList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            PromotionListViewHolder vh = holder as PromotionListViewHolder;

            PromotionsModelV2 model = promotionList[position];
            vh.Title.Text = model.Title;
            vh.Description.Text = model.Text;
            if (model.PublishedDate != null)
            {
                vh.Date.Text = DateTime.ParseExact(model.PublishedDate, "yyyyMMdd", null).ToString("dd MMM yyyy");
            }

            TextViewUtils.SetMuseoSans500Typeface(vh.Title);
            TextViewUtils.SetMuseoSans300Typeface(vh.Description);
            TextViewUtils.SetMuseoSans300Typeface(vh.Date);

            if (!string.IsNullOrEmpty(model.LandscapeImage))
            {
                if (model.LandscapeImage.Contains("jpeg"))
                {
                    Log.Debug("Promotion Adapter", "Image path saved");
                    Picasso.With(vh.PromotionImgView.Context)
                   .Load(new Java.IO.File(model.LandscapeImage))
                   .Error(Resource.Drawable.promotions_default_image)
                   .Fit()
                   .Into(vh.PromotionImgView);
                    vh.PromotionImgProgress.Visibility = ViewStates.Gone;
                }
                else
                {
                    GetImageAsync(vh.PromotionImgView, vh.PromotionImgProgress, model);
                }
            }
            else
            {
                vh.PromotionImgView.SetImageResource(Resource.Drawable.promotions_default_image);
                vh.PromotionImgProgress.Visibility = ViewStates.Gone;
            }

            if (model.Read)
            {
                vh.PromotionReadImgView.SetImageResource(0);
            }
            else
            {
                vh.PromotionReadImgView.SetImageResource(Resource.Drawable.ic_notifications_unread);
            }

        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.PromotionListItemView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new PromotionListViewHolder(itemView, OnClick);
        }

        public class PromotionListViewHolder : RecyclerView.ViewHolder
        {
            public LinearLayout RootView { get; private set; }
            public TextView Title { get; private set; }
            public TextView Description { get; private set; }
            public TextView Date { get; private set; }
            public ImageView PromotionImgView { get; private set; }
            public ProgressBar PromotionImgProgress { get; private set; }
            public ImageView PromotionImgTagView { get; private set; }
            public ImageView PromotionReadImgView { get; private set; }

            public PromotionListViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                RootView = itemView.FindViewById<LinearLayout>(Resource.Id.rootView);
                Title = itemView.FindViewById<TextView>(Resource.Id.promotion_title);
                Description = itemView.FindViewById<TextView>(Resource.Id.promotion_description);
                Date = itemView.FindViewById<TextView>(Resource.Id.promotion_date);
                PromotionImgView = itemView.FindViewById<ImageView>(Resource.Id.promotion_img);
                PromotionImgTagView = itemView.FindViewById<ImageView>(Resource.Id.promotion_img_tag);
                PromotionImgProgress = itemView.FindViewById<ProgressBar>(Resource.Id.progressBar);
                PromotionReadImgView = itemView.FindViewById<ImageView>(Resource.Id.promotion_img_read);

                RootView.Click += (sender, e) => listener(base.LayoutPosition);

            }

        }

        public async Task GetImageAsync(ImageView icon, ProgressBar progressBar, PromotionsModelV2 item)
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
                    PortraitImage = item.PortraitImage.Replace(" ", "%20"),
                    LandscapeImage = filepath,
                    PromoStartDate = item.PromoStartDate,
                    PromoEndDate = item.PromoEndDate,
                    PublishedDate = item.PublishedDate,
                    IsPromoExpired = item.IsPromoExpired,
                    Read = item.Read
                };
                wtManager.UpdateItem(wtManager);
            }
            else
            {
                icon.SetImageResource(Resource.Drawable.promotions_default_image);
            }

            progressBar.Visibility = ViewStates.Gone;
        }

        private Android.Graphics.Bitmap GetImageBitmapFromUrl(ImageView icon, string url)
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
    }
}