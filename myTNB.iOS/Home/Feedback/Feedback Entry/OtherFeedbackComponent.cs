﻿using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Feedback.FeedbackEntry
{
    public class OtherFeedbackComponent
    {
        UIView _mainContainer, _commonWidgets, _viewFeedbackType, _viewLineFeedbackType;
        UILabel _lblFeedbackTypeTitle, _lblFeedbackTypeError, _lblFeedbackType;
        UIImageView imgViewAccountNumber;

        FeedbackCommonWidgets _feedbackCommonWidgets;

        readonly FeedbackEntryViewController _controller;

        public OtherFeedbackComponent(FeedbackEntryViewController viewController)
        {
            _controller = viewController;
        }

        void ConstructOtherFeedbackWidget()
        {
            _feedbackCommonWidgets = new FeedbackCommonWidgets(_controller.View);
            _feedbackCommonWidgets.SetValidationMethod(_controller.SetButtonEnable);
            _mainContainer = new UIView(new CGRect(0, 0, _controller.View.Frame.Width, 0));
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
            ConstructFeedbackType();
            _mainContainer.AddSubview(_viewFeedbackType);
            nfloat mobileNumberHeight = 0.0f;
            if (!_controller.isMobileNumberAvailable)
            {
                _commonWidgets = _feedbackCommonWidgets.GetMobileNumberComponent();
                mobileNumberHeight = _commonWidgets.Frame.Height;
                _commonWidgets.Frame = new CGRect(_commonWidgets.Frame.X, _viewFeedbackType.Frame.Height + _viewFeedbackType.Frame.X + 14
                    , _commonWidgets.Frame.Width, _commonWidgets.Frame.Height);
                _mainContainer.AddSubview(_commonWidgets);
            }
            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width, 18 + 51 + mobileNumberHeight);
        }

        void ConstructNonLoginComponent()
        {
            _commonWidgets = _feedbackCommonWidgets.GetCommonWidgets();
            ConstructFeedbackType();
            _mainContainer.AddSubviews(new UIView[] { _commonWidgets, _viewFeedbackType });
            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width, (18 + 51) * 4);
        }

        void ConstructFeedbackType()
        {
            //Feedback Type
            _viewFeedbackType = new UIView((new CGRect(18, _controller.IsLoggedIn ? 16 : 217
                , _controller.View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblFeedbackTypeTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFeedbackType.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString("Feedback_Type", AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left
            };

            _lblFeedbackTypeError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewFeedbackType.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString("Invalid_FeedbackType", AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            imgViewAccountNumber = new UIImageView(new CGRect(0, 12, 24, 24))
            {
                Image = UIImage.FromBundle("IC-FieldFeedback")
            };

            _lblFeedbackType = new UILabel(new CGRect(30, 12, _viewFeedbackType.Frame.Width, 24))
            {
                AttributedText = AttributedStringUtility.GetAttributedString("Feedback_Type", AttributedStringUtility.AttributedStringType.Value),
                //Noted: Temp Number, will create a list for this later.
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

        public bool IsValidEntry()
        {
            if (_controller.IsLoggedIn)
            {
                return _controller.isMobileNumberAvailable || _feedbackCommonWidgets.IsValidMobileNumber();
            }
            else
            {
                return _feedbackCommonWidgets.IsValidEntry();
            }
        }

        public string GetEmail()
        {
            return _feedbackCommonWidgets.GetEmail();
        }

        public string GetMobileNumber()
        {
            return _feedbackCommonWidgets.GetMobileNumber();
        }

        public string GetFullName()
        {
            return _feedbackCommonWidgets.GetFullName();
        }
    }
}