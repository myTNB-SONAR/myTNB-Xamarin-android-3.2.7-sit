//using System;
//using System.Collections.Generic;
//using CoreGraphics;
//using Foundation;
//using myTNB.Model;
//using UIKit;

//namespace myTNB.Feedback.Enquiry.UpdatePersonalDetail
//{
//    public class UpdatePersonalDetailComponent
//    {
//        public UpdatePersonalDetailComponent()
//        {
//        }

//        private readonly UpdatePersonalDetail2ViewController _controller;
//        private readonly TextFieldHelper _textFieldHelper = new TextFieldHelper();

//        private UIView _mainContainer, _commonWidgets, _viewAccountNo, _viewLineAccountNo;
//        private UILabel _lblAccountNoTitle, _lblAccountNoError, _lblAccountNumber;
//        private UITextField _txtFieldAccountNo;

//        private UpdatePersonalDetailCommonWidgets _feedbackCommonWidgets;

//        public UpdatePersonalDetailComponent(UpdatePersonalDetail2ViewController viewController)
//        {
//            _controller = viewController;
//        }

//        private void ConstructOtherFeedbackWidget()
//        {
//            _feedbackCommonWidgets = new UpdatePersonalDetailCommonWidgets(_controller.View) { ViewController = _controller };
//            //_feedbackCommonWidgets.SetValidationMethod(_controller.SetButtonEnable);
//            _mainContainer = new UIView(new CGRect(0, 0, _controller.View.Frame.Width, 0));
//            if (_controller.IsLoggedIn)
//            {
//                ConstructLoginComponent();
//            }
//            else
//            {
//                ConstructNonLoginComponent();
//            }
//        }

//        private void ConstructLoginComponent()
//        {
//            ConstructAccountNumberSelector();
//            _mainContainer.AddSubview(_viewAccountNo);
//            nfloat mobileNumberHeight = 0.0f;
//            if (!_controller.isMobileNumberAvailable)
//            {
//                _commonWidgets = _feedbackCommonWidgets.GetMobileNumberComponent();
//                mobileNumberHeight = _commonWidgets.Frame.Height;
//                _commonWidgets.Frame = new CGRect(_commonWidgets.Frame.X, _viewAccountNo.Frame.Height + _viewAccountNo.Frame.X + 14
//                    , _commonWidgets.Frame.Width, _commonWidgets.Frame.Height);
//                _mainContainer.AddSubview(_commonWidgets);
//            }
//            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width, 18 + 51 + mobileNumberHeight);
//        }

//        private void ConstructNonLoginComponent()
//        {
//            _commonWidgets = _feedbackCommonWidgets.GetCommonWidgets();
//            ConstructAccountNumberField();
//            _mainContainer.AddSubviews(new UIView[] { _commonWidgets, _viewAccountNo });
//            _mainContainer.Frame = new CGRect(0, 0, _controller.View.Frame.Width, (18 + 51) * 4);
//        }

//        private void ConstructAccountNumberSelector()
//        {
//            _viewAccountNo = new UIView((new CGRect(18, 16, _controller.View.Frame.Width - 36, 51)))
//            {
//                BackgroundColor = UIColor.Clear
//            };

//            _lblAccountNoTitle = new UILabel
//            {
//                Frame = new CGRect(0, 0, _viewAccountNo.Frame.Width, 12),
//                AttributedText = AttributedStringUtility.GetAttributedString(_controller.GetCommonI18NValue(Constants.Common_AccountNo)
//                , AttributedStringUtility.AttributedStringType.Title),
//                TextAlignment = UITextAlignment.Left,
//                Hidden = true
//            };

//            _lblAccountNoError = new UILabel
//            {
//                Frame = new CGRect(0, 37, _viewAccountNo.Frame.Width, 14),
//                AttributedText = AttributedStringUtility.GetAttributedString(_controller.GetErrorI18NValue(Constants.Error_AccountLength)
//                    , AttributedStringUtility.AttributedStringType.Error),
//                TextAlignment = UITextAlignment.Left,
//                Hidden = true
//            };

//            UIImageView imgViewAccountNumber = new UIImageView(new CGRect(0, 12, 24, 24))
//            {
//                Image = UIImage.FromBundle("Account-Number")
//            };
//            _viewAccountNo.AddSubview(imgViewAccountNumber);

//            _lblAccountNumber = new UILabel(new CGRect(30, 12, _viewAccountNo.Frame.Width - 60, 24))
//            {
//                Font = MyTNBFont.MuseoSans16_300,
//                TextColor = MyTNBColor.TunaGrey(),
//                AttributedText = new NSAttributedString(_controller.GetCommonI18NValue(Constants.Common_AccountNo)
//                , font: MyTNBFont.MuseoSans16
//                , foregroundColor: MyTNBColor.SilverChalice
//                , strokeWidth: 0
//            )
//            };
//            _viewAccountNo.AddSubview(_lblAccountNumber);

//            UIImageView imgDropDown = new UIImageView(new CGRect(_viewAccountNo.Frame.Width - 30, 12, 24, 24))
//            {
//                Image = UIImage.FromBundle("IC-Action-Dropdown")
//            };
//            _viewAccountNo.AddSubview(imgDropDown);

//            _viewLineAccountNo = GenericLine.GetLine(new CGRect(0, 36, _viewAccountNo.Frame.Width, 1));

//            _viewAccountNo.AddSubviews(new UIView[] { _lblAccountNoTitle, _lblAccountNoError
//                ,imgViewAccountNumber, _lblAccountNumber, _viewLineAccountNo });

//            _viewAccountNo?.AddGestureRecognizer(new UITapGestureRecognizer(() =>
//            {
//                UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
//                GenericSelectorViewController viewController = (GenericSelectorViewController)storyBoard
//                    .InstantiateViewController("GenericSelectorViewController");
//                viewController.Title = _controller.GetCommonI18NValue(Constants.Common_SelectElectricityAccount);
//                viewController.Items = GetAccountList();
//                viewController.OnSelect = OnSelectAction;
//                viewController.SelectedIndex = DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex;
//                UINavigationController navController = new UINavigationController(viewController);
//                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
//                _controller.PresentViewController(navController, true, null);
//            }));
//        }

//        private void OnSelectAction(int index)
//        {
//            DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex = index;
//        }

//        private List<string> GetAccountList()
//        {
//            if (DataManager.DataManager.SharedInstance.AccountRecordsList.d != null
//                && DataManager.DataManager.SharedInstance.AccountRecordsList.d?.Count > 0)
//            {
//                List<string> accountList = new List<string>();
//                foreach (CustomerAccountRecordModel item in DataManager.DataManager.SharedInstance.AccountRecordsList?.d)
//                {
//                    accountList.Add(string.Format("{0} - {1}", item?.accNum ?? string.Empty, item?.accDesc ?? string.Empty));
//                }
//                return accountList;
//            }
//            return new List<string>();
//        }

//        private void ConstructAccountNumberField()
//        {
//            _viewAccountNo = new UIView((new CGRect(18, 217, _controller.View.Frame.Width - 36, 51)))
//            {
//                BackgroundColor = UIColor.Clear
//            };

//            _lblAccountNoTitle = new UILabel
//            {
//                Frame = new CGRect(0, 0, _viewAccountNo.Frame.Width, 12),
//                AttributedText = AttributedStringUtility.GetAttributedString(_controller.GetCommonI18NValue(Constants.Common_AccountNo)
//                    , AttributedStringUtility.AttributedStringType.Title),
//                TextAlignment = UITextAlignment.Left,
//                Hidden = true
//            };

//            _lblAccountNoError = new UILabel
//            {
//                Frame = new CGRect(0, 37, _viewAccountNo.Frame.Width, 14),
//                AttributedText = AttributedStringUtility.GetAttributedString(_controller.GetErrorI18NValue(Constants.Error_AccountLength)
//                    , AttributedStringUtility.AttributedStringType.Error),
//                TextAlignment = UITextAlignment.Left,
//                Hidden = true
//            };

//            _txtFieldAccountNo = new UITextField
//            {
//                Frame = new CGRect(0, 12, _viewAccountNo.Frame.Width, 24),
//                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(_controller.GetCommonI18NValue(Constants.Common_AccountNo)
//                    , AttributedStringUtility.AttributedStringType.Value),
//                TextColor = MyTNBColor.TunaGrey()
//            };
//            _txtFieldAccountNo.KeyboardType = UIKeyboardType.NumberPad;
//            _textFieldHelper.CreateDoneButton(_txtFieldAccountNo);

//            _viewLineAccountNo = GenericLine.GetLine(new CGRect(0, 36, _viewAccountNo.Frame.Width, 1));
//            _viewAccountNo.AddSubviews(new UIView[] { _lblAccountNoTitle, _lblAccountNoError
//                , _txtFieldAccountNo, _viewLineAccountNo });

//            _textFieldHelper.CreateTextFieldLeftView(_txtFieldAccountNo, "Account-Number");
//            SetTextFieldEvents(_txtFieldAccountNo, _lblAccountNoTitle, _lblAccountNoError
//                , _viewLineAccountNo, null, TNBGlobal.ACCOUNT_NO_PATTERN);
//        }

//        private void SetTextFieldEvents(UITextField textField, UILabel lblTitle, UILabel lblError
//            , UIView viewLine, UILabel lblHint, string pattern)
//        {
//            if (lblHint == null)
//            {
//                lblHint = new UILabel();
//            }
//            _textFieldHelper.SetKeyboard(textField);
//            textField.EditingChanged += (sender, e) =>
//            {
//                UITextField txtField = sender as UITextField;
//                lblHint.Hidden = !lblError.Hidden || textField.Text.Length == 0;
//                lblTitle.Hidden = textField.Text.Length == 0;
//                //_controller.SetButtonEnable();
//            };
//            textField.EditingDidBegin += (sender, e) =>
//            {
//                lblHint.Hidden = !lblError.Hidden || textField.Text.Length == 0;
//                lblTitle.Hidden = textField.Text.Length == 0;
//                textField.LeftViewMode = UITextFieldViewMode.Never;
//                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
//            };
//            textField.ShouldEndEditing = (sender) =>
//            {
//                bool isValid = true;
//                bool isEmptyAllowed = true;
//                lblTitle.Hidden = textField.Text.Length == 0;
//                isValid = isValid && _textFieldHelper.ValidateTextField(textField.Text, pattern);

//                bool isNormal = isValid || (textField.Text.Length == 0 && isEmptyAllowed);
//                lblError.Hidden = isNormal;
//                lblHint.Hidden = true;
//                viewLine.BackgroundColor = isNormal ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
//                textField.TextColor = isNormal ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
//                //_controller.SetButtonEnable();
//                return true;
//            };
//            textField.ShouldReturn = (sender) =>
//            {
//                sender.ResignFirstResponder();
//                return false;
//            };
//            textField.ShouldChangeCharacters += (txtField, range, replacementString) =>
//            {
//                if (txtField == _txtFieldAccountNo)
//                {
//                    return !(range.Location == 12);
//                }
//                return true;
//            };
//            textField.EditingDidEnd += (sender, e) =>
//            {
//                if (textField.Text.Length == 0)
//                {
//                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
//                }
//            };
//        }

//        public UIView GetComponent()
//        {
//            ConstructOtherFeedbackWidget();
//            return _mainContainer;
//        }

//        public bool IsValidEntry()
//        {
//            if (_controller.IsLoggedIn)
//            {
//                return _controller.isMobileNumberAvailable || _feedbackCommonWidgets.IsValidMobileNumber();
//            }
//            else
//            {
//                bool isValidAccountNo = _textFieldHelper.ValidateTextField(_txtFieldAccountNo.Text, TNBGlobal.ACCOUNT_NO_PATTERN)
//                    && _textFieldHelper.ValidateAccountNumberLength(_txtFieldAccountNo.Text);
//                return _feedbackCommonWidgets.IsValidEntry() && isValidAccountNo;
//            }
//        }

//        public void SetSelectedAccountNumber()
//        {
//            int index = DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex;
//            if (DataManager.DataManager.SharedInstance.AccountRecordsList?.d == null
//                || DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count == 0)
//            {
//                return;
//            }

//            _lblAccountNumber.Text = string.Format("{0} - {1}", DataManager.DataManager.SharedInstance.AccountRecordsList?.d[index]?.accNum
//                , DataManager.DataManager.SharedInstance.AccountRecordsList?.d[index]?.accDesc);
//            _lblAccountNoTitle.Hidden = false;
//        }

//        public string GetAccountNumber()
//        {
//            if (_controller.IsLoggedIn)
//            {
//                int index = DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex;
//                if (DataManager.DataManager.SharedInstance.AccountRecordsList?.d.Count > 0)
//                {
//                    return DataManager.DataManager.SharedInstance.AccountRecordsList?.d[index]?.accNum ?? string.Empty;
//                }
//                return string.Empty;
//            }
//            else
//            {
//                return _txtFieldAccountNo.Text ?? string.Empty;
//            }
//        }

//        public string GetEmail()
//        {
//            return _feedbackCommonWidgets.GetEmail();
//        }

//        public string GetMobileNumber()
//        {
//            return _feedbackCommonWidgets.GetMobileNumber();
//        }

//        public string GetFullName()
//        {
//            return _feedbackCommonWidgets.GetFullName();
//        }
//    }
//}
