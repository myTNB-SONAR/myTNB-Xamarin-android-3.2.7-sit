using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Graphics.Drawables;
using Android.Graphics;
using myTNB_Android.Src.Promotions.Adapter;
using Android.Support.V4.View;
using myTNB.SitecoreCM.Models;
using Newtonsoft.Json;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Promotions.Activity;
using Me.Relex;

namespace myTNB_Android.Src.Promotions.Fragments
{
    public class PromotionDialogFragmnet : DialogFragment, ViewPager.IOnPageChangeListener
    {

        private Context mContext;
        private ViewPager pager;
        private PromotionPagerAdapter adapter;
        private List<PromotionsModelV2> promotions = new List<PromotionsModelV2>();
        private CircleIndicator indicator;

        public PromotionDialogFragmnet(Context ctx)
        {
            this.mContext = ctx;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Dialog.SetCancelable(false);
            Dialog.SetCanceledOnTouchOutside(false);
            Dialog.Window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.WrapContent);
            Dialog.Window.Attributes.Gravity = GravityFlags.Center;
            
            View rootView = inflater.Inflate(Resource.Layout.promotion_dialog, container, false);

            pager = (ViewPager) rootView.FindViewById(Resource.Id.promotionPager);
            indicator = (CircleIndicator)rootView.FindViewById(Resource.Id.indicator);

            promotions = JsonConvert.DeserializeObject<List<PromotionsModelV2>>(Arguments.GetString("promotions"));

            adapter = new PromotionPagerAdapter(mContext, promotions);
            pager.Adapter = adapter;
            adapter.DetailsClicked += OnDetailsClick;
            adapter.CloseClicked += OnCloseClick;
            adapter.RefreshIndicator += OnRefreshIndicator;
            adapter.NotifyDataSetChanged();

            pager.SetClipToPadding(false);
            pager.SetPadding(50, 0, 50, 0);
            pager.PageMargin = 50;
            pager.SetOnPageChangeListener(this);
            indicator.SetViewPager(pager);
            UpdatePromotionAsSeen(0);
            return rootView;
        }

        void OnDetailsClick(object sender, int position)
        {
            if (promotions.Count > 0)
            {
                PromotionsModelV2 model = promotions[position];
                PromotionsEntityV2 wtManager = new PromotionsEntityV2()
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
                    Read = true
                };
                wtManager.UpdateItem(wtManager);
                UpdatePromotionAsSeen(position);
                Intent details_activity = new Intent(mContext, typeof(PromotionsActivity));
                details_activity.PutExtra("Promotion", JsonConvert.SerializeObject(model));
                //Activity.StartActivity(details_activity);
                mContext.StartActivity(details_activity);
            }
            //this.Dismiss();
        }

        void OnCloseClick(object sender, int position)
        {
            this.Dismiss();
        }

        void OnRefreshIndicator(object sender, int position)
        {
            this.indicator.SetViewPager(this.pager);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public void OnPageScrollStateChanged(int state)
        {
            
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            
        }

        public void OnPageSelected(int position)
        {
            //Update all records as Shown
            if (promotions.Count > 0) {
                UpdatePromotionAsSeen(position);
                promotions[position].PromoShown = true;
            }
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
    }
}