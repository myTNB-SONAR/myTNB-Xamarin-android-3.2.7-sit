using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Home.Feedback.FeedbackEntry
{
    public class OtherFeedbackComponent
    {
        UIView _mainContainer, _nonLoginWidgets, _viewFeedbackType, _viewLineFeedbackType;
        UILabel _lblFeedbackTypeTitle, _lblFeedbackTypeError, _lblFeedbackType;
        UIImageView imgViewAccountNumber;

        NonLoginCommonWidget _nonLoginCommonWidgets;

        readonly FeedbackEntryViewController _controller;

        public OtherFeedbackComponent(FeedbackEntryViewController viewController)
        {
            _controller = viewController;
        }

        void ConstructOtherFeedbackWidget()
        {
            _mainContainer = new UIView(new CGRect(0, 0, _controller.View.Frame.Width, 500));
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

        }

        void ConstructNonLoginComponent()
        {
            _nonLoginCommonWidgets = new NonLoginCommonWidget(_controller.View);
            _nonLoginWidgets = _nonLoginCommonWidgets.GetCommonWidgets();
            _nonLoginCommonWidgets.SetValidationMethod(_controller.SetButtonEnable);
            ConstructFeedbackType();
            _mainContainer.AddSubviews(new UIView[] { _nonLoginWidgets, _viewFeedbackType });
            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width, (18 + 51) * 4);
        }

        void ConstructFeedbackType()
        {
            //Feedback Type
            _viewFeedbackType = new UIView((new CGRect(18, 217, _controller.View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblFeedbackTypeTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFeedbackType.Frame.Width, 12),
                AttributedText = new NSAttributedString("Feedback_Type".Translate().ToUpper()
                    , font: myTNBFont.MuseoSans11_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left
            };

            _lblFeedbackTypeError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewFeedbackType.Frame.Width, 14),
                AttributedText = new NSAttributedString("Invalid_FeedbackType".Translate()
                    , font: myTNBFont.MuseoSans11_300()
                    , foregroundColor: myTNBColor.Tomato()
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            imgViewAccountNumber = new UIImageView(new CGRect(0, 12, 24, 24))
            {
                Image = UIImage.FromBundle("IC-FieldFeedback")
            };

            _lblFeedbackType = new UILabel(new CGRect(30, 12, _viewFeedbackType.Frame.Width, 24))
            {
                AttributedText = new NSAttributedString("Feedback_Type".Translate()
                    , font: myTNBFont.MuseoSans18_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0),//Noted: Temp Number, will create a list for this later.
                Font = myTNBFont.MuseoSans18_300(),
                TextColor = myTNBColor.TunaGrey()
            };

            UIImageView imgDropDown = new UIImageView(new CGRect(_viewFeedbackType.Frame.Width - 30, 12, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Action-Dropdown")
            };

            _viewLineFeedbackType = new UIView((new CGRect(0, 36, _viewFeedbackType.Frame.Width, 1)))
            {
                BackgroundColor = myTNBColor.PlatinumGrey()
            };

            _viewFeedbackType.AddSubviews(new UIView[] { _lblFeedbackTypeTitle
                , _lblFeedbackTypeError, imgViewAccountNumber, _lblFeedbackType
                , imgDropDown, _viewLineFeedbackType });

            _viewFeedbackType.AddGestureRecognizer(_controller.GetFeedbackTypeGestureRecognizer());
        }

        public UIView GetComponent()
        {
            ConstructOtherFeedbackWidget();
            return _mainContainer;
        }

        public void SetFeedbackType()
        {
            var currIndex = DataManager.DataManager.SharedInstance.CurrentSelectedFeedbackTypeIndex;
            if (DataManager.DataManager.SharedInstance.OtherFeedbackType != null
                && currIndex < DataManager.DataManager.SharedInstance.OtherFeedbackType.Count)
            {
                _lblFeedbackType.Text = DataManager.DataManager.SharedInstance?.OtherFeedbackType[currIndex]?.FeedbackTypeName;
            }
        }

        void SetButtonEnabled()
        {
            if (_controller.IsLoggedIn)
            {
            }
            else
            {
                _controller.SetButtonEnable();
            }
        }

        public bool IsValidEntry()
        {
            return _nonLoginCommonWidgets.IsValidEntry();
        }
    }
}