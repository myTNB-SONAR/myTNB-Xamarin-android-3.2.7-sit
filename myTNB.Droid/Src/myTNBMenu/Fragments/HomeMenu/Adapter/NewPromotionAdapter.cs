
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
    public class NewPromotionAdapter : RecyclerView.Adapter
    {

        List<NewPromotion> promotionList = new List<NewPromotion>();

        public event EventHandler<int> ClickChanged;

        private Android.App.Activity mActivity;

        public NewPromotionAdapter(List<NewPromotion> data, Android.App.Activity Activity)
        {
            if (data == null)
            {
                this.promotionList.Clear();
            }
            else
            {
                this.promotionList = data;
            }
            this.mActivity = Activity;
        }

        public override int ItemCount => promotionList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            NewPromotionViewHolder vh = holder as NewPromotionViewHolder;

            NewPromotion model = promotionList[position];

            try
            {
                vh.Title.Text = model.Title;

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    vh.Message.TextFormatted = Html.FromHtml(model.Description, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    vh.Message.TextFormatted = Html.FromHtml(model.Description);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                int currentCount = position % 2;

                switch (currentCount)
                {
                    case 0:
                        vh.PromoImage.SetImageResource(Resource.Drawable.image_1);
                        break;
                    case 1:
                        vh.PromoImage.SetImageResource(Resource.Drawable.image_2);
                        break;

                }

                TextViewUtils.SetMuseoSans500Typeface(vh.Title);
                TextViewUtils.SetMuseoSans300Typeface(vh.Message);

                ViewGroup.LayoutParams currentCard = vh.cardView.LayoutParameters;

                int cardWidth = (int)((this.mActivity.Resources.DisplayMetrics.WidthPixels / 1.35) - DPUtils.ConvertDPToPx(6f));
                currentCard.Width = cardWidth;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.PromotionCardLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new NewPromotionViewHolder(itemView, OnClick);
        }

        void OnClick(NewPromotionViewHolder sender, int position)
        {
            try
            {
                // ClickChanged(this, position);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public class NewPromotionViewHolder : RecyclerView.ViewHolder
        {

            public TextView Message { get; private set; }

            public TextView Title { get; private set; }

            public CardView cardView { get; private set; }

            public ImageView PromoImage { get; private set; }

            public NewPromotionViewHolder(View itemView, Action<NewPromotionViewHolder, int> listener) : base(itemView)
            {
                Message = itemView.FindViewById<TextView>(Resource.Id.txtMessage);
                Title = itemView.FindViewById<TextView>(Resource.Id.txtTitle);
                cardView = itemView.FindViewById<CardView>(Resource.Id.card_view_click);
                PromoImage = itemView.FindViewById<ImageView>(Resource.Id.promotion_img);

                cardView.Click += (s, e) => listener((this), base.LayoutPosition);
            }
        }

    }
}
