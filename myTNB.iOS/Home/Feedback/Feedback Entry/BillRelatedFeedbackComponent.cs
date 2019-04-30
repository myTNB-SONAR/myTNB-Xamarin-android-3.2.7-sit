using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Home.Feedback.FeedbackEntry
{
    public class BillRelatedFeedbackComponent
    {
        readonly FeedbackEntryViewController _controller;
        readonly TextFieldHelper _textFieldHelper = new TextFieldHelper();

        UIView _mainContainer, _commonWidgets, _viewAccountNo, _viewLineAccountNo;
        UILabel _lblAccountNoTitle, _lblAccountNoError, _lblAccountNumber;
        UITextField _txtFieldAccountNo;

        FeedbackCommonWidgets _feedbackCommonWidgets;

        public BillRelatedFeedbackComponent(FeedbackEntryViewController viewController)
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
            ConstructAccountNumberSelector();
            _mainContainer.AddSubview(_viewAccountNo);
            nfloat mobileNumberHeight = 0.0f;
            if (!_controller.isMobileNumberAvailable)
            {
                _commonWidgets = _feedbackCommonWidgets.GetMobileNumberComponent();
                mobileNumberHeight = _commonWidgets.Frame.Height;
                _commonWidgets.Frame = new CGRect(_commonWidgets.Frame.X, _viewAccountNo.Frame.Height + _viewAccountNo.Frame.X + 14
                    , _commonWidgets.Frame.Width, _commonWidgets.Frame.Height);
                _mainContainer.AddSubview(_commonWidgets);
            }
            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width, 18 + 51 + mobileNumberHeight);
        }

        void ConstructNonLoginComponent()
        {
            _commonWidgets = _feedbackCommonWidgets.GetCommonWidgets();
            ConstructAccountNumberField();
            _mainContainer.AddSubviews(new UIView[] { _commonWidgets, _viewAccountNo });
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
                AttributedText = AttributedStringUtility.GetAttributedString("Common_AccountNo", AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _lblAccountNoError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewAccountNo.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString("Invalid_AccountLength", AttributedStringUtility.AttributedStringType.Error),
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
                Font = MyTNBFont.MuseoSans16_300,
                TextColor = MyTNBColor.TunaGrey(),
                AttributedText = new NSAttributedString("Common_AccountNo".Translate()
                , font: MyTNBFont.MuseoSans16
                , foregroundColor: MyTNBColor.SilverChalice
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
                BackgroundColor = MyTNBColor.PlatinumGrey
            };

            _viewAccountNo.AddSubviews(new UIView[] { _lblAccountNoTitle, _lblAccountNoError
                ,imgViewAccountNumber, _lblAccountNumber, _viewLineAccountNo });

            _viewAccountNo?.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
                GenericSelectorViewController viewController = (GenericSelectorViewController)storyBoard
                    .InstantiateViewController("GenericSelectorViewController");
                viewController.Title = "Feedback_SelectAccountNumber".Translate();
                viewController.Items = GetAccountList();
                viewController.OnSelect = OnSelectAction;
                viewController.SelectedIndex = DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex;
                var navController = new UINavigationController(viewController);
                _controller.PresentViewController(navController, true, null);
            }));
        }

        void OnSelectAction(int index)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex = index;
        }

        List<string> GetAccountList()
        {
            if (DataManager.DataManager.SharedInstance.AccountRecordsList.d != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d?.Count > 0)
            {
                List<string> accountList = new List<string>();
                foreach (CustomerAccountRecordModel item in DataManager.DataManager.SharedInstance.AccountRecordsList?.d)
                {
                    accountList.Add(string.Format("{0} - {1}", item?.accNum ?? string.Empty, item?.accDesc ?? string.Empty));
                }
                return accountList;
            }
            return new List<string>();
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
                AttributedText = AttributedStringUtility.GetAttributedString("Common_AccountNo", AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _lblAccountNoError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewAccountNo.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString("Invalid_AccountLength", AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _txtFieldAccountNo = new UITextField
            {
                Frame = new CGRect(0, 12, _viewAccountNo.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString("Common_AccountNo", AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };
            _txtFieldAccountNo.KeyboardType = UIKeyboardType.NumberPad;
            _textFieldHelper.CreateDoneButton(_txtFieldAccountNo);

            _viewLineAccountNo = new UIView((new CGRect(0, 36, _viewAccountNo.Frame.Width, 1)))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };

            _viewAccountNo.AddSubviews(new UIView[] { _lblAccountNoTitle, _lblAccountNoError
                , _txtFieldAccountNo, _viewLineAccountNo });

            _textFieldHelper.CreateTextFieldLeftView(_txtFieldAccountNo, "Account-Number");
            SetTextFieldEvents(_txtFieldAccountNo, _lblAccountNoTitle, _lblAccountNoError
                , _viewLineAccountNo, null, TNBGlobal.ACCOUNT_NO_PATTERN);
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
                return _controller.isMobileNumberAvailable || _feedbackCommonWidgets.IsValidMobileNumber();
            }
            else
            {
                bool isValidAccountNo = _textFieldHelper.ValidateTextField(_txtFieldAccountNo.Text, TNBGlobal.ACCOUNT_NO_PATTERN)
                    && _textFieldHelper.ValidateAccountNumberLength(_txtFieldAccountNo.Text);
                return _feedbackCommonWidgets.IsValidEntry() && isValidAccountNo;
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

        public string GetAccountNumber()
        {
            if (_controller.IsLoggedIn)
            {
                var index = DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex;
                return DataManager.DataManager.SharedInstance.AccountRecordsList?.d[index]?.accNum ?? string.Empty;
            }
            else
            {
                return _txtFieldAccountNo.Text ?? string.Empty;
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