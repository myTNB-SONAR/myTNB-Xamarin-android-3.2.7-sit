using System;
using System.Collections.Generic;
using Carousels;
using CoreGraphics;
using Foundation;
using myTNB.Home.Components;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class PromotionsModalDataSource : iCarouselDataSource
    {
        List<PromotionsModelV2> promotions;
        UIView parentView;

        public EventHandler OnTapSkip { get; set; }
        public EventHandler OnTapDetails { get; set; }
        public Action OnTapExit { get; set; }

        public PromotionsModalDataSource(UIView parent, List<PromotionsModelV2> promos)
        {
            parentView = parent;
            promotions = promos;
        }

        public override nint GetNumberOfItems(iCarousel carousel)
        {
            return promotions?.Count ?? 0;
        }

        public override UIView GetViewForItem(iCarousel carousel, nint index, UIView view)
        {
            double buttonHeight = 44;
            double itemHeight = parentView.Frame.Width > 512 ? 840 : 420;
            double carWidth = parentView.Frame.Width > 512 ? 512 : 256;
            double carHeight = itemHeight + buttonHeight;
            double buttonWidth = Math.Floor(carWidth / 2);

            var promotion = promotions[(int)index];

            view = new UIView(new CGRect(0, 0, carWidth, carHeight))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };

            view.Layer.CornerRadius = 6.0f;

            UIImageView imgView = new UIImageView(new CGRect(0, 0, carWidth, itemHeight));
            
            view.AddSubview(imgView);

            if (promotion.PortraitImage != null)
            {
                ActivityIndicatorComponent _activityIndicator = new ActivityIndicatorComponent(view);
                _activityIndicator.Show();
                NSUrl url = new NSUrl(promotion.PortraitImage);
                NSUrlSession session = NSUrlSession
                    .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
                NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
                {
                    if (error == null && response != null && data != null)
                    {
                        InvokeOnMainThread(() =>
                        {
                            imgView.Image = UIImage.LoadFromData(data);
                            imgView.ContentMode = UIViewContentMode.ScaleAspectFit;
                            _activityIndicator.Hide();
                        });
                    }
                });
                dataTask.Resume();
            }

            UIButton btnSkip = new UIButton(UIButtonType.Custom);
            btnSkip.Frame = new CGRect(0, view.Frame.Height - buttonHeight, buttonWidth, buttonHeight);
            btnSkip.SetAttributedTitle(LabelHelper.CreateAttributedString("Skip", myTNBFont.MuseoSans14_300(), myTNBColor.PowerBlue()), UIControlState.Normal);
            btnSkip.BackgroundColor = UIColor.White;
            btnSkip.TouchDown += OnTapSkip;
            view.AddSubview(btnSkip);

            UIButton btnDetails = new UIButton(UIButtonType.Custom);
            btnDetails.Frame = new CGRect(view.Frame.Width - buttonWidth, view.Frame.Height - buttonHeight, buttonWidth , buttonHeight);
            btnDetails.SetAttributedTitle(LabelHelper.CreateAttributedString("Details", myTNBFont.MuseoSans14_300(), myTNBColor.PowerBlue()), UIControlState.Normal);
            btnDetails.BackgroundColor = UIColor.White;
            btnDetails.TouchDown += OnTapDetails;
            view.AddSubview(btnDetails);

            double exitWidth = 24;
            double exitContainerWidth = 60;
            UIView exitViewContainer = new UIView(new CGRect(view.Frame.Width - exitContainerWidth, 0, exitContainerWidth, exitContainerWidth));
            exitViewContainer.BackgroundColor = UIColor.Clear;
            UIImageView exitView = new UIImageView(new CGRect(24, 12, exitWidth, exitWidth))
            {
                Image = UIImage.FromBundle("IC-X-Gray"),
                BackgroundColor = UIColor.Clear
            };
            exitViewContainer.AddSubview(exitView);

            var exitTap = new UITapGestureRecognizer(OnTapExit);
            exitTap.NumberOfTapsRequired = 1;
            exitViewContainer.AddGestureRecognizer(exitTap);
            view.AddSubview(exitViewContainer);
            view.BringSubviewToFront(exitViewContainer);

            UIView lineView = new UIView(new CGRect(btnSkip.Frame.Width + 1, view.Frame.Height - buttonHeight, 1, buttonHeight));
            lineView.BackgroundColor = myTNBColor.LinesGray();
            view.AddSubview(lineView);

            return view;
        }
    }
}
