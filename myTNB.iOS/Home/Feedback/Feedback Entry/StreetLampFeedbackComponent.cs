using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using CoreLocation;
using Foundation;
using Location;
using myTNB.Model;
using UIKit;

namespace myTNB.Home.Feedback.FeedbackEntry
{
    public class StreetLampFeedbackComponent
    {
        readonly FeedbackEntryViewController _controller;
        readonly TextFieldHelper _textFieldHelper = new TextFieldHelper();

        const string ANY_PATTERN = @".*";

        private LocationManager LocManager { get; set; }

        FeedbackCommonWidgets _feedbackCommonWidgets;
        UIView _mainContainer, _bannerContainer, _commonWidgets, _viewState, _viewLineState
            , _detailsContainer, _viewLocation, _viewLineLocation, _viewPole, _viewLinePole;
        UILabel _lblStateTitle, _lblStateError, _lblState, _lblLocationTitle, _lblLocationError
            , _lblPoleTitle, _lblPoleError;
        UIImageView imgViewState;
        UITextField _txtFieldLocation, _txtFieldPole;

        nfloat imgViewFaultyLampHeight;

        public StreetLampFeedbackComponent(FeedbackEntryViewController viewController)
        {
            _controller = viewController;
        }

        void ConstructOtherFeedbackWidget()
        {
            _feedbackCommonWidgets = new FeedbackCommonWidgets(_controller.View);
            _feedbackCommonWidgets.SetValidationMethod(_controller.SetButtonEnable);
            _mainContainer = new UIView(new CGRect(0, 0, _controller.View.Frame.Width, 0));
            ConstructBanner();
            _detailsContainer = new UIView(new CGRect(0, _bannerContainer.Frame.Height
                , _controller.View.Frame.Width, (18 + 51) * 3));
            ConstructDetailFields();
            _mainContainer.AddSubview(_bannerContainer);
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
            _mainContainer.AddSubview(_detailsContainer);
            nfloat mobileNumberHeight = 0.0f;
            if (!_controller.isMobileNumberAvailable)
            {
                _commonWidgets = _feedbackCommonWidgets.GetMobileNumberComponent();
                mobileNumberHeight = _commonWidgets.Frame.Height;
                _commonWidgets.Frame = new CGRect(_commonWidgets.Frame.X, _detailsContainer.Frame.Height
                        + _detailsContainer.Frame.X + 14 + _bannerContainer.Frame.Height
                    , _commonWidgets.Frame.Width, _commonWidgets.Frame.Height);
                _mainContainer.AddSubview(_commonWidgets);
            }
            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width
                , _bannerContainer.Frame.Height + _detailsContainer.Frame.Height + mobileNumberHeight);
        }

        void ConstructNonLoginComponent()
        {
            _commonWidgets = _feedbackCommonWidgets.GetCommonWidgets();
            _commonWidgets.Frame = new CGRect(0, _bannerContainer.Frame.Height
                , _commonWidgets.Frame.Width, _commonWidgets.Frame.Height);
            _detailsContainer.Frame = new CGRect(0, _bannerContainer.Frame.Height + _commonWidgets.Frame.Height
                , _detailsContainer.Frame.Width, _detailsContainer.Frame.Height);
            _mainContainer.AddSubviews(new UIView[] { _commonWidgets, _detailsContainer });
            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width
                , _commonWidgets.Frame.Height + _bannerContainer.Frame.Height + _detailsContainer.Frame.Height);
        }

        void ConstructBanner()
        {
            //Photo Header
            imgViewFaultyLampHeight = (_controller.View.Frame.Width * 121) / 320;
            _bannerContainer = new UIView(new CGRect(0, 0, _controller.View.Frame.Width, (imgViewFaultyLampHeight + 47 + 78)));
            UIImageView imgViewFaultyLamp = new UIImageView(new CGRect(0, 0, _controller.View.Frame.Width, imgViewFaultyLampHeight))
            {
                Image = UIImage.FromBundle("Faulty-TNB-Lamp")
            };

            //Label
            var lblReport = new UILabel(new CGRect(18, 23 + imgViewFaultyLampHeight, _controller.View.Frame.Width - 36, 18))
            {
                Text = "Feedback_FaultyLampTitle".Translate(),
                TextColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans16_500
            };

            //Terms Details
            UITextView txtViewDetails = new UITextView(new CGRect(18, 47 + imgViewFaultyLampHeight, _controller.View.Frame.Width - 36, 78));
            NSMutableAttributedString attributedString = new NSMutableAttributedString("Feedback_FaultyLampMessage".Translate());
            UIStringAttributes firstAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14_300
            };
            UIStringAttributes secondAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.TunaGrey(),
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

        void ConstructDetailFields()
        {
            //State
            _viewState = new UIView((new CGRect(18, 16, _controller.View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblStateTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewState.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString("Feedback_State", AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _lblStateError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewState.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString("Invalid_State", AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            imgViewState = new UIImageView(new CGRect(0, 12, 24, 24))
            {
                Image = UIImage.FromBundle("IC-FieldCoordinates")
            };

            _lblState = new UILabel(new CGRect(30, 12, _viewState.Frame.Width, 24))
            {
                AttributedText = AttributedStringUtility.GetAttributedString("Feedback_State", AttributedStringUtility.AttributedStringType.Value)
            };

            UIImageView imgDropDown = new UIImageView(new CGRect(_viewState.Frame.Width - 30, 12, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Action-Dropdown")
            };

            _viewLineState = GenericLine.GetLine(new CGRect(0, 36, _viewState.Frame.Width, 1));

            _viewState.AddSubviews(new UIView[] { _lblStateTitle, _lblStateError, imgViewState
                , _lblState, imgDropDown, _viewLineState });
            _viewState.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
                GenericSelectorViewController viewController = (GenericSelectorViewController)storyBoard
                    .InstantiateViewController("GenericSelectorViewController");
                viewController.Title = "Feedback_SelectState".Translate();
                viewController.Items = GetStateList();
                viewController.OnSelect = OnSelectAction;
                viewController.SelectedIndex = DataManager.DataManager.SharedInstance.CurrentSelectedStateForFeedbackIndex;
                var navController = new UINavigationController(viewController);
                _controller.PresentViewController(navController, true, null);
            }));

            //Location Street/Name
            _viewLocation = new UIView((new CGRect(18, 83, _controller.View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblLocationTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewLocation.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString("Feedback_Location", AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _lblLocationError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewLocation.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString("Invalid_Location", AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _txtFieldLocation = new UITextField
            {
                Frame = new CGRect(0, 12, _viewLocation.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString("Feedback_Location", AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };

            _viewLineLocation = GenericLine.GetLine(new CGRect(0, 36, _viewLocation.Frame.Width, 1));

            _viewLocation.AddSubviews(new UIView[] { _lblLocationTitle, _lblLocationError
                , _txtFieldLocation, _viewLineLocation });

            //Pole Number
            _viewPole = new UIView((new CGRect(18, 150, _controller.View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblPoleTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewPole.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString("Feedback_PoleNumber", AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _lblPoleError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewPole.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString("Invalid_PoleNumber", AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _txtFieldPole = new UITextField
            {
                Frame = new CGRect(0, 12, _viewPole.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString("Feedback_PoleNumber", AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };

            _viewLinePole = GenericLine.GetLine(new CGRect(0, 36, _viewPole.Frame.Width, 1));

            _viewPole.AddSubviews(new UIView[] { _lblPoleTitle, _lblPoleError, _txtFieldPole, _viewLinePole });
            _detailsContainer.AddSubviews(new UIView[] { _viewState, _viewLocation, _viewPole });

            _textFieldHelper.CreateTextFieldLeftView(_txtFieldLocation, "IC-FieldCoordinates");
            CreateTextFieldRightView(_txtFieldLocation, "IC-Action-Location");
            _textFieldHelper.CreateTextFieldLeftView(_txtFieldPole, "Account-Number");
            SetTextFieldEvents(_txtFieldLocation, _lblLocationTitle, _lblLocationError
                , _viewLineLocation, null, ANY_PATTERN);
            SetTextFieldEvents(_txtFieldPole, _lblPoleTitle, _lblPoleError, _viewLinePole
                , null, ANY_PATTERN);
        }

        void OnSelectAction(int index)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedStateForFeedbackIndex = index;
            _controller._streetLampRelatedFeedbackComponent.ValidateState();
        }

        List<string> GetStateList()
        {
            if (DataManager.DataManager.SharedInstance.StatesForFeedBack != null
                && DataManager.DataManager.SharedInstance.StatesForFeedBack.Count > 0)
            {
                List<string> stateList = new List<string>();
                foreach (StatesForFeedbackDataModel item in DataManager.DataManager.SharedInstance.StatesForFeedBack)
                {
                    stateList.Add(item?.StateName ?? string.Empty);
                }
                return stateList;
            }
            return new List<string>();
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel lblTitle, UILabel lblError
            , UIView viewLine, UILabel lblHint, string pattern)
        {
            if (lblHint == null)
            {
                lblHint = new UILabel();
            }
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                UITextField txtField = sender as UITextField;
                lblHint.Hidden = !lblError.Hidden || textField.Text.Length == 0;
                lblTitle.Hidden = textField.Text.Length == 0;
                _controller.SetButtonEnable();
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblHint.Hidden = !lblError.Hidden || textField.Text.Length == 0;
                lblTitle.Hidden = textField.Text.Length == 0;
                textField.LeftViewMode = UITextFieldViewMode.Never;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                bool isValid = true;
                bool isEmptyAllowed = true;
                lblTitle.Hidden = textField.Text.Length == 0;
                isValid = isValid && _textFieldHelper.ValidateTextField(textField.Text, pattern);
                bool isNormal = isValid || (textField.Text.Length == 0 && isEmptyAllowed);
                lblError.Hidden = isNormal;
                lblHint.Hidden = true;
                viewLine.BackgroundColor = isNormal ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isNormal ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
                _controller.SetButtonEnable();
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.ShouldChangeCharacters += (txtField, range, replacementString) =>
            {
                return true;
            };
            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
            };
        }

        public void ValidateState()
        {
            _lblStateError.Hidden = DataManager.DataManager.SharedInstance.CurrentSelectedStateForFeedbackIndex > -1;
            _viewLineState.BackgroundColor = DataManager.DataManager.SharedInstance.CurrentSelectedStateForFeedbackIndex > -1
                ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
        }

        void CreateTextFieldRightView(UITextField textField, string imageName)
        {
            UIView rightView = new UIView((new CGRect(0, 0, 24, 24)));

            UIImageView imgForRightView = new UIImageView(UIImage.FromBundle(imageName));
            rightView.AddSubview(imgForRightView);

            rightView.Frame = new CGRect(rightView.Frame.X, rightView.Frame.Y, rightView.Frame.Width, rightView.Frame.Height);
            rightView.ContentMode = UIViewContentMode.Center;
            textField.RightView = rightView;
            textField.RightViewMode = UITextFieldViewMode.Always;

            UITapGestureRecognizer tapLocation = new UITapGestureRecognizer(() =>
            {
                LocManager = new LocationManager();
                LocManager.StartLocationUpdates();
                bool isFirstCall = false;
                LocManager.LocMgr.AuthorizationChanged += (sender, e) =>
                {

                    if (e.Status == CLAuthorizationStatus.Authorized
                        || e.Status == CLAuthorizationStatus.AuthorizedWhenInUse
                        || e.Status == CLAuthorizationStatus.AuthorizedAlways)
                    {
                        CLLocation location = LocManager.LocMgr.Location;

                        CLGeocoder geoCoder = new CLGeocoder();
                        geoCoder.ReverseGeocodeLocation(location, (CLPlacemark[] placemarks, NSError error) =>
                        {
                            var reverseGeocodedAddress = string.Format("{0} {1} {2} {3} {4}"
                                , placemarks[0].Thoroughfare
                                , placemarks[0].Locality
                                , placemarks[0].AdministrativeArea
                                , placemarks[0].PostalCode
                                , placemarks[0].Country);
                            _txtFieldLocation.Text = reverseGeocodedAddress;
                            _controller.SetButtonEnable();
                        });
                    }
                    else if (e.Status == CLAuthorizationStatus.NotDetermined)
                    {
                        LocManager.LocMgr.RequestWhenInUseAuthorization();
                        isFirstCall = true;
                    }
                    else if (e.Status == CLAuthorizationStatus.Denied && !isFirstCall)
                    {
                        var alertTitle = "Feedback_AccessTitle".Translate();
                        var alertMessage = "Feedback_AccessMessage".Translate();
                        var okCancelAlertController = UIAlertController.Create(alertTitle, alertMessage
                            , UIAlertControllerStyle.Alert);
                        okCancelAlertController.AddAction(UIAlertAction.Create("Common_Cancel".Translate()
                            , UIAlertActionStyle.Cancel, alert => Debug.WriteLine("Cancel was clicked")));
                        okCancelAlertController.AddAction(UIAlertAction.Create("Feedback_GoToSettings".Translate()
                            , UIAlertActionStyle.Default, alert => NavigateToSettings()));
                        _controller.PresentViewController(okCancelAlertController, true, null);
                    }
                };
            });
            rightView.AddGestureRecognizer(tapLocation);
        }

        void NavigateToSettings()
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
        }

        public void SetState()
        {
            var currIndex = DataManager.DataManager.SharedInstance.CurrentSelectedStateForFeedbackIndex;
            if (currIndex > -1)
            {
                if (DataManager.DataManager.SharedInstance.StatesForFeedBack != null
                    && currIndex < DataManager.DataManager.SharedInstance.StatesForFeedBack?.Count
                    && _lblState != null && imgViewState != null)
                {
                    _lblState.Text = DataManager.DataManager.SharedInstance.StatesForFeedBack[currIndex].StateName;
                    _lblStateTitle.Hidden = false;
                    imgViewState.Hidden = true;
                    var frame = new CGRect();
                    frame = _lblState.Frame;
                    frame.X = 0;
                    _lblState.Frame = frame;
                }
            }
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
                return _textFieldHelper.ValidateTextField(_txtFieldLocation.Text, ANY_PATTERN)
                    && _txtFieldLocation.Text.Length != 0
                    && _textFieldHelper.ValidateTextField(_txtFieldPole.Text, ANY_PATTERN)
                    && _txtFieldPole.Text.Length != 0
                    && (_controller.isMobileNumberAvailable || _feedbackCommonWidgets.IsValidMobileNumber());
            }
            else
            {
                bool isValid = _textFieldHelper.ValidateTextField(_txtFieldLocation.Text, ANY_PATTERN)
                    && _txtFieldLocation.Text.Length != 0
                    && _textFieldHelper.ValidateTextField(_txtFieldPole.Text, ANY_PATTERN)
                    && _txtFieldPole.Text.Length != 0;
                return _feedbackCommonWidgets.IsValidEntry() && isValid;
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

        public string GetState()
        {
            if (DataManager.DataManager.SharedInstance.StatesForFeedBack != null && DataManager
                    .DataManager.SharedInstance.CurrentSelectedStateForFeedbackIndex > -1)
            {
                return DataManager.DataManager.SharedInstance.StatesForFeedBack?[DataManager
                    .DataManager.SharedInstance.CurrentSelectedStateForFeedbackIndex]?.StateId ?? string.Empty;
            }
            return string.Empty;
        }

        public string GetLocation()
        {

            return _txtFieldLocation?.Text ?? string.Empty;
        }

        public string GetPoleNumber()
        {
            return _txtFieldPole?.Text ?? string.Empty;
        }
    }
}