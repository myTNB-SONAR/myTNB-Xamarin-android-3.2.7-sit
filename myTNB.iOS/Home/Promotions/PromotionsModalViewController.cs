using Carousels;
using CoreGraphics;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using System;
using System.Collections.Generic;
using UIKit;

namespace myTNB
{
    public partial class PromotionsModalViewController : UIViewController
    {
        iCarousel promoCarousel;
        private List<PromotionsModelV2> promoCache = new List<PromotionsModelV2>();
        private List<PromotionsModelV2> promos;

        public Action OnModalDone { get; set; }

        public List<PromotionsModelV2> Promotions
        {
            get
            {
                return promos;
            }
            set
            {
                promos = value;
                if (promos != null)
                {
                    promoCache.AddRange(promos);
                }
            }
        }

        public PromotionsModalViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBar.Hidden = true;
            View.BackgroundColor = MyTNBColor.TunaGrey(0.6f);

            double carHeight = View.Frame.Width > 512 ? 840 : 420;
            var locY = (View.Bounds.Height / 2) - (carHeight / 2);
            promoCarousel = new iCarousel(new CGRect(0, locY, View.Bounds.Width, carHeight))
            {
                Type = iCarouselType.Linear,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                Bounces = false,
                BackgroundColor = UIColor.Clear
            };

            promoCarousel.GetValue = (sender, option, value) =>
            {
                if (option == iCarouselOption.Spacing)
                {
                    return value * 1.1f;
                }
                return value;
            };

            promoCarousel.CurrentItemIndexChanged += (sender, e) =>
            {
                UpdateShownDate();
            };

            var promoDataSource = new PromotionsModalDataSource(View, Promotions);
            promoDataSource.OnTapSkip = OnItemSkipTapped;
            promoDataSource.OnTapDetails = OnItemDetailsTapped;
            promoDataSource.OnTapExit = HandleSkip;
            promoCarousel.DataSource = promoDataSource;
            UpdateShownDate();
            View.AddSubview(promoCarousel);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            DataManager.DataManager.SharedInstance.UpdatePromosDb(promoCache);
        }

        /// <summary>
        /// Handles the item skip button tapped event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        private void OnItemSkipTapped(object sender, EventArgs args)
        {
            HandleSkip();
        }

        /// <summary>
        /// Handles the item details button tapped event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        private void OnItemDetailsTapped(object sender, EventArgs args)
        {
            UpdateShownDate();
            if (promoCarousel?.CurrentItemIndex < Promotions?.Count)
            {
                var promo = Promotions[(int)promoCarousel.CurrentItemIndex];
                promo.IsRead = true;
                var entity = promo.ToEntity();
                PromotionsEntity.UpdateItem(entity);

                var item = promoCache.Find(x => x.ID == promo.ID);
                if (item != null)
                {
                    item.IsRead = promo.IsRead;
                }
                ShowDetails(promo);
                RemoveTappedPromoItem();
            }
        }

        /// <summary>
        /// Handler for on done.
        /// </summary>
        public void OnDone()
        {
            DataManager.DataManager.SharedInstance.UpdatePromosDb(promoCache);
            if (Promotions.Count == 0)
            {
                DismissViewController(true, null);
                OnModalDone?.Invoke();
            }
        }

        /// <summary>
        /// Handles the skip.
        /// </summary>
        private void HandleSkip()
        {
            RemoveTappedPromoItem();
            if (Promotions.Count == 0)
            {
                OnDone();
            }
            else
            {
                UpdateShownDate();
            }
        }

        /// <summary>
        /// Handles the promotion item select.
        /// </summary>
        /// <param name="promotion">Promotion.</param>
        private void ShowDetails(PromotionsModelV2 promotion)
        {
            ActivityIndicator.Show();
            UIStoryboard storyBoard = UIStoryboard.FromName("PromotionDetails", null);
            PromotionDetailsViewController viewController =
                storyBoard.InstantiateViewController("PromotionDetailsViewController") as PromotionDetailsViewController;
            if (viewController != null)
            {
                viewController.Promotion = promotion;
                viewController.OnDone = OnDone;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            }
            ActivityIndicator.Hide();
        }

        /// <summary>
        /// Updates the shown date.
        /// </summary>
        private void UpdateShownDate()
        {
            if (promoCarousel?.CurrentItemIndex < Promotions?.Count)
            {
                var key = Promotions[(int)promoCarousel.CurrentItemIndex].ID;
                var item = promoCache.Find(x => x.ID == key);
                if (item != null)
                {
                    var nowStr = DateTime.Now.Date.ToString("yyyyMMdd");
                    item.PromoShownDate = nowStr;
                }
            }
        }

        /// <summary>
        /// Removes the tapped promo item.
        /// </summary>
        /// <param name="sender">Sender.</param>
        private void RemoveTappedPromoItem()
        {
            if (promoCarousel?.CurrentItemIndex < Promotions?.Count)
            {
                Promotions.RemoveAt((int)promoCarousel.CurrentItemIndex);

                if (Promotions.Count > 0)
                {
                    promoCarousel.ReloadData();
                }
            }
        }
    }
}