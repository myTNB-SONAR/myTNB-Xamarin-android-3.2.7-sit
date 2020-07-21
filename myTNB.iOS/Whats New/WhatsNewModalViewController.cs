using Carousels;
using CoreGraphics;
using myTNB.SitecoreCMS.Model;
using System;
using System.Collections.Generic;
using UIKit;

namespace myTNB
{
    public partial class WhatsNewModalViewController : CustomUIViewController
    {
        iCarousel whatsNewCarousel;
        private List<WhatsNewModel> whatsNewCache = new List<WhatsNewModel>();
        private List<WhatsNewModel> whatsnews;

        public Action OnWhatsNewClick { get; set; }
        public Action OnDismissWhatsNew { get; set; }

        public List<WhatsNewModel> WhatsNews
        {
            get
            {
                return whatsnews;
            }
            set
            {
                whatsnews = value;
                if (whatsnews != null)
                {
                    whatsNewCache.AddRange(whatsnews);
                }

            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBar.Hidden = true;
            View.BackgroundColor = MyTNBColor.Black75;

            double carHeight = View.Bounds.Height;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                carHeight = carHeight - (ScaleUtility.GetScaledHeight(68F) + (UIScreen.MainScreen.Bounds.Height * 0.202F));
            }
            else
            {
                carHeight = carHeight - (ScaleUtility.GetScaledHeight(18F) + (UIScreen.MainScreen.Bounds.Height * 0.102F));
            }
            var locY = (View.Bounds.Height / 2) - (carHeight / 2);
            whatsNewCarousel = new iCarousel(new CGRect(0, locY, View.Bounds.Width, carHeight))
            {
                Type = iCarouselType.Linear,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                Bounces = false,
                BackgroundColor = UIColor.Clear
            };

            whatsNewCarousel.GetValue = (sender, option, value) =>
            {

                if (option == iCarouselOption.Spacing)
                {
                    return value * 1.1f;
                }

                return value;
            };

            var whatsNewDataSource = new WhatsNewModalDataSource(WhatsNews)
            {
                OnTapDetails = OnItemDetailsTapped,
                OnTapExit = HandleSkip
            };
            whatsNewCarousel.DataSource = whatsNewDataSource;
            View.AddSubview(whatsNewCarousel);

        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        /// <summary>
        /// Handles the item skip button tapped event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        private void OnItemSkipTapped(object sender, EventArgs args)
        {
            HandleSkip(sender, args);
        }

        /// <summary>
        /// Handles the item details button tapped event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        private void OnItemDetailsTapped()
        {
            if (whatsNewCarousel?.CurrentItemIndex < WhatsNews?.Count)
            {
                var whatnew = WhatsNews[(int)whatsNewCarousel.CurrentItemIndex];
                whatnew.IsRead = true;
                WhatsNewServices.SetIsRead(whatnew.ID);

                var item = whatsNewCache.Find(x => x.ID == whatnew.ID);
                if (item != null)
                {
                    item.IsRead = whatnew.IsRead;
                }

                DataManager.DataManager.SharedInstance.WhatsNewModalNavigationId = whatnew.ID;
                RemoveTappedWhatsNewItem();
                if (WhatsNews.Count > 0)
                {
                    OnWhatsNewClick?.Invoke();
                }
            }
        }

        /// <summary>
        /// Handles the skip.
        /// </summary>
        private void HandleSkip(object sender, EventArgs args)
        {
            RemoveTappedWhatsNewItem();
        }

        /// <summary>
        /// Removes the tapped whatsnew item.
        /// </summary>
        /// <param name="sender">Sender.</param>
        private void RemoveTappedWhatsNewItem()
        {
            if (whatsNewCarousel?.CurrentItemIndex < WhatsNews?.Count)
            {
                WhatsNews.RemoveAt((int)whatsNewCarousel.CurrentItemIndex);

                if (WhatsNews.Count > 0)
                {
                    whatsNewCarousel.ReloadData();
                }
                else if (WhatsNews.Count == 0)
                {
                    OnDismissWhatsNew?.Invoke();
                }

            }
        }

    }
}