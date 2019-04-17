using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Home.Feedback.FeedbackEntry
{
    public class BillRelatedFeedbackComponent
    {
        readonly FeedbackEntryViewController _controller;
        readonly TextFieldHelper _textFieldHelper = new TextFieldHelper();

        const string ACCOUNT_NO_PATTERN = @"^[0-9]{12,14}$";

        UIView _mainContainer, _nonLoginWidgets, _viewAccountNo, _viewLineAccountNo;
        UILabel _lblAccountNoTitle, _lblAccountNoError, _lblAccountNumber;
        UITextField _txtFieldAccountNo;

        FeedbackCommonWidgets _nonLoginCommonWidgets;

        public BillRelatedFeedbackComponent(FeedbackEntryViewController viewController)
        {
            _controller = viewController;
        }

        void ConstructOtherFeedbackWidget()
        {
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
            ConstructAccountNumberSelector();
            _mainContainer.AddSubviews(new UIView[] { _viewAccountNo });
            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width, (18 + 51));
        }

        void ConstructNonLoginComponent()
        {
            _nonLoginCommonWidgets = new FeedbackCommonWidgets(_controller.View);
            _nonLoginWidgets = _nonLoginCommonWidgets.GetCommonWidgets();
            _nonLoginCommonWidgets.SetValidationMethod(_controller.SetButtonEnable);
            ConstructAccountNumberField();
            _mainContainer.AddSubviews(new UIView[] { _nonLoginWidgets, _viewAccountNo });
            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width, (18 + 51) * 4);
        }

        void ConstructAccountNumberSelector()
        {
            _viewAccountNo = new UIView((new CGRect(18, 16, _controller.View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblAccountNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewAccountNo.Frame.Width, 12),
                AttributedText = new NSAttributedString("Common_AccountNo".Translate().ToUpper()
                    , font: myTNBFont.MuseoSans11_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _lblAccountNoError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewAccountNo.Frame.Width, 14),
                AttributedText = new NSAttributedString("Invalid_AccountLength".Translate()
                    , font: myTNBFont.MuseoSans11_300()
                    , foregroundColor: myTNBColor.Tomato()
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            UIImageView imgViewAccountNumber = new UIImageView(new CGRect(0, 12, 24, 24))
            {
                Image = UIImage.FromBundle("Account-Number")
            };
            _viewAccountNo.AddSubview(imgViewAccountNumber);

            _lblAccountNumber = new UILabel(new CGRect(30, 12, _viewAccountNo.Frame.Width - 60, 24))
            {
                Font = myTNBFont.MuseoSans16_300(),
                TextColor = myTNBColor.TunaGrey(),
                AttributedText = new NSAttributedString("Common_AccountNo".Translate()
                , font: myTNBFont.MuseoSans16()
                , foregroundColor: myTNBColor.SilverChalice()
                , strokeWidth: 0
            )
            };
            _viewAccountNo.AddSubview(_lblAccountNumber);

            UIImageView imgDropDown = new UIImageView(new CGRect(_viewAccountNo.Frame.Width - 30, 12, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Action-Dropdown")
            };
            _viewAccountNo.AddSubview(imgDropDown);

            _viewLineAccountNo = new UIView((new CGRect(0, 36, _viewAccountNo.Frame.Width, 1)))
            {
                BackgroundColor = myTNBColor.PlatinumGrey()
            };

            _viewAccountNo.AddSubviews(new UIView[] { _lblAccountNoTitle, _lblAccountNoError
                ,imgViewAccountNumber, _lblAccountNumber, _viewLineAccountNo });

            _viewAccountNo?.AddGestureRecognizer(_controller.GetAccountNumberGestureRecognizer());
        }

        void ConstructAccountNumberField()
        {
            _viewAccountNo = new UIView((new CGRect(18, 217, _controller.View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblAccountNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewAccountNo.Frame.Width, 12),
                AttributedText = new NSAttributedString("Common_AccountNo".Translate().ToUpper()
                    , font: myTNBFont.MuseoSans11_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _lblAccountNoError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewAccountNo.Frame.Width, 14),
                AttributedText = new NSAttributedString("Invalid_AccountLength".Translate()
                    , font: myTNBFont.MuseoSans11_300()
                    , foregroundColor: myTNBColor.Tomato()
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _txtFieldAccountNo = new UITextField
            {
                Frame = new CGRect(0, 12, _viewAccountNo.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString("Common_AccountNo".Translate()
                    , font: myTNBFont.MuseoSans18_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0),
                TextColor = myTNBColor.TunaGrey()
            };
            _txtFieldAccountNo.KeyboardType = UIKeyboardType.NumberPad;
            _textFieldHelper.CreateDoneButton(_txtFieldAccountNo);

            _viewLineAccountNo = new UIView((new CGRect(0, 36, _viewAccountNo.Frame.Width, 1)))
            {
                BackgroundColor = myTNBColor.PlatinumGrey()
            };

            _viewAccountNo.AddSubviews(new UIView[] { _lblAccountNoTitle, _lblAccountNoError
                , _txtFieldAccountNo, _viewLineAccountNo });

            _textFieldHelper.CreateTextFieldLeftView(_txtFieldAccountNo, "Account-Number");
            SetTextFieldEvents(_txtFieldAccountNo, _lblAccountNoTitle, _lblAccountNoError
                , _viewLineAccountNo, null, ACCOUNT_NO_PATTERN);
        }

        void SetTextFieldEvents(UITextField textField, UILabel lblTitle, UILabel lblError
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
                viewLine.BackgroundColor = myTNBColor.PowerBlue();
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
                viewLine.BackgroundColor = isNormal ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                textField.TextColor = isNormal ? myTNBColor.TunaGrey() : myTNBColor.Tomato();
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
                {
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
                }
            };
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
                bool isValidAccountNo = _textFieldHelper.ValidateTextField(_txtFieldAccountNo.Text, ACCOUNT_NO_PATTERN)
                    && _textFieldHelper.ValidateAccountNumberLength(_txtFieldAccountNo.Text);
                return _nonLoginCommonWidgets.IsValidEntry() && isValidAccountNo;
            }
        }

        public void SetSelectedAccountNumber()
        {
            var index = DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex;
            if (DataManager.DataManager.SharedInstance.AccountRecordsList?.d == null)
            {
                return;
            }
            _lblAccountNumber.Text = string.Format("{0} - {1}", DataManager.DataManager.SharedInstance.AccountRecordsList?.d[index]?.accNum
                , DataManager.DataManager.SharedInstance.AccountRecordsList?.d[index]?.accDesc);
        }
    }
}
