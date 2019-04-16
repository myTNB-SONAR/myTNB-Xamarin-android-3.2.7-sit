using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Home.Feedback.FeedbackEntry
{
    public class StreetLampFeedbackComponent
    {
        readonly FeedbackEntryViewController _controller;

        NonLoginCommonWidget _nonLoginCommonWidgets;
        UIView _mainContainer, _bannerContainer, _nonLoginWidgets;

        nfloat imgViewFaultyLampHeight;

        public StreetLampFeedbackComponent(FeedbackEntryViewController viewController)
        {
            _controller = viewController;
        }

        void ConstructOtherFeedbackWidget()
        {
            _mainContainer = new UIView(new CGRect(0, 0, _controller.View.Frame.Width, 0));
            ConstructBanner();
            if (_controller.IsLoggedIn)
            {
                ConstructLoginComponent();
            }
            else
            {
                ConstructNonLoginComponent();
            }
        }

        void ConstructLoginComponent()
        {
            _mainContainer.AddSubviews(new UIView[] { _bannerContainer });
            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width, _bannerContainer.Frame.Height); //(18 + 51));
        }

        void ConstructNonLoginComponent()
        {
            _nonLoginCommonWidgets = new NonLoginCommonWidget(_controller.View);
            _nonLoginWidgets = _nonLoginCommonWidgets.GetCommonWidgets();
            _nonLoginCommonWidgets.SetValidationMethod(_controller.SetButtonEnable);
            _mainContainer.AddSubviews(new UIView[] { _bannerContainer, _nonLoginWidgets });
            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width, ((18 + 51) * 3) + _bannerContainer.Frame.Height);
        }

        void ConstructBanner()
        {
            //Photo Header
            imgViewFaultyLampHeight = (_controller.View.Frame.Width * 121) / 320;
            _bannerContainer = new UIView(new CGRect(0, 0, _controller.View.Frame.Width, (imgViewFaultyLampHeight + 23 + 18 + 47 + 78)));
            UIImageView imgViewFaultyLamp = new UIImageView(new CGRect(0, 0, _controller.View.Frame.Width, imgViewFaultyLampHeight))
            {
                Image = UIImage.FromBundle("Faulty-TNB-Lamp")
            };

            //Label
            var lblReport = new UILabel(new CGRect(18, 23 + imgViewFaultyLampHeight, _controller.View.Frame.Width - 36, 18))
            {
                Text = "Feedback_FaultyLampTitle".Translate(),
                TextColor = myTNBColor.PowerBlue(),
                Font = myTNBFont.MuseoSans16_500()
            };

            //Terms Details
            UITextView txtViewDetails = new UITextView(new CGRect(18, 47 + imgViewFaultyLampHeight, _controller.View.Frame.Width - 36, 78));
            NSMutableAttributedString attributedString = new NSMutableAttributedString("Feedback_FaultyLampMessage".Translate());
            UIStringAttributes firstAttributes = new UIStringAttributes
            {
                ForegroundColor = myTNBColor.TunaGrey(),
                Font = myTNBFont.MuseoSans14_300()
            };
            UIStringAttributes secondAttributes = new UIStringAttributes
            {
                ForegroundColor = myTNBColor.TunaGrey(),
                Font = UIFont.BoldSystemFontOfSize(14)

            };
            attributedString.SetAttributes(firstAttributes.Dictionary, new NSRange(0, 159));
            //Todo: Get Index for Lamp Number
            attributedString.SetAttributes(secondAttributes.Dictionary, new NSRange(128, 10));

            txtViewDetails.AttributedText = attributedString;
            txtViewDetails.Editable = false;
            txtViewDetails.Selectable = false;
            txtViewDetails.ScrollEnabled = false;
            txtViewDetails.TextContainerInset = new UIEdgeInsets(0, -4, 0, 0);

            _bannerContainer.AddSubviews(new UIView[] { imgViewFaultyLamp, lblReport, txtViewDetails });
        }

        public UIView GetComponent()
        {
            ConstructOtherFeedbackWidget();
            return _mainContainer;
        }

        public bool IsValidEntry()
        {
            if (_controller.IsLoggedIn)
            {
                return true;
            }
            else
            {
                return _nonLoginCommonWidgets.IsValidEntry();
            }
        }
    }
}