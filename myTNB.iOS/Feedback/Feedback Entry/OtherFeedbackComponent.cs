using System;
using System.Collections.Generic;
using CoreGraphics;
using myTNB.Feedback;
using myTNB.Model;
using UIKit;

namespace myTNB.Home.Feedback.FeedbackEntry
{
    public class OtherFeedbackComponent
    {
        private UIView _mainContainer, _commonWidgets, _viewFeedbackType, _viewLineFeedbackType;
        private UILabel _lblFeedbackTypeTitle, _lblFeedbackTypeError, _lblFeedbackType;
        private UIImageView imgViewAccountNumber;

        private FeedbackCommonWidgets _feedbackCommonWidgets;

        private readonly FeedbackEntryViewController _controller;

        public OtherFeedbackComponent(FeedbackEntryViewController viewController)
        {
            _controller = viewController;
        }

        private void ConstructOtherFeedbackWidget()
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

        private void ConstructLoginComponent()
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

        private void ConstructNonLoginComponent()
        {
            _commonWidgets = _feedbackCommonWidgets.GetCommonWidgets();
            ConstructFeedbackType();
            _mainContainer.AddSubviews(new UIView[] { _commonWidgets, _viewFeedbackType });
            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width, (18 + 51) * 4);
        }

        private void ConstructFeedbackType()
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
                AttributedText = AttributedStringUtility.GetAttributedString(_controller.GetI18NValue(FeedbackConstants.I18N_FeedbackType)
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left
            };

            _lblFeedbackTypeError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewFeedbackType.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString(_controller.GetI18NValue(FeedbackConstants.I18N_InvalidFeedbackType)
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            imgViewAccountNumber = new UIImageView(new CGRect(0, 12, 24, 24))
            {
                Image = UIImage.FromBundle("IC-FieldFeedback")
            };

            _lblFeedbackType = new UILabel(new CGRect(30, 12, _viewFeedbackType.Frame.Width, 24))
            {
                AttributedText = AttributedStringUtility.GetAttributedString(_controller.GetI18NValue(FeedbackConstants.I18N_FeedbackType), AttributedStringUtility.AttributedStringType.Value),
                //Noted: Temp Number, will create a list for this later.
                Font = MyTNBFont.MuseoSans18_300,
                TextColor = MyTNBColor.TunaGrey()
            };

            UIImageView imgDropDown = new UIImageView(new CGRect(_viewFeedbackType.Frame.Width - 30, 12, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Action-Dropdown")
            };

            _viewLineFeedbackType = GenericLine.GetLine(new CGRect(0, 36, _viewFeedbackType.Frame.Width, 1));

            _viewFeedbackType.AddSubviews(new UIView[] { _lblFeedbackTypeTitle
                , _lblFeedbackTypeError, imgViewAccountNumber, _lblFeedbackType
                , imgDropDown, _viewLineFeedbackType });

            _viewFeedbackType.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
                GenericSelectorViewController viewController = (GenericSelectorViewController)storyBoard
                    .InstantiateViewController("GenericSelectorViewController");
                viewController.Title = _controller.GetI18NValue(FeedbackConstants.I18N_SelectFeedbackType);
                viewController.Items = GetFeedbackTypeList();
                viewController.OnSelect = OnSelectAction;
                viewController.SelectedIndex = DataManager.DataManager.SharedInstance.CurrentSelectedFeedbackTypeIndex;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                _controller.PresentViewController(navController, true, null);
            }));
        }

        private void OnSelectAction(int index)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedFeedbackTypeIndex = index;
        }

        private List<string> GetFeedbackTypeList()
        {
            if (DataManager.DataManager.SharedInstance.OtherFeedbackType != null
                && DataManager.DataManager.SharedInstance.OtherFeedbackType.Count > 0)
            {
                List<string> feedbackList = new List<string>();
                foreach (OtherFeedbackTypeDataModel item in DataManager.DataManager.SharedInstance.OtherFeedbackType)
                {
                    feedbackList.Add(item?.FeedbackTypeName ?? string.Empty);
                }
                return feedbackList;
            }
            return new List<string>();
        }

        public UIView GetComponent()
        {
            ConstructOtherFeedbackWidget();
            return _mainContainer;
        }

        public void SetFeedbackType()
        {
            int currIndex = DataManager.DataManager.SharedInstance.CurrentSelectedFeedbackTypeIndex;
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