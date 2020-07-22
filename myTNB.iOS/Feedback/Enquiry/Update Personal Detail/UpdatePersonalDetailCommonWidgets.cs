//using System;
//using CoreGraphics;
//using UIKit;

//namespace myTNB.Feedback.Enquiry.UpdatePersonalDetail
//{
//    public class UpdatePersonalDetailCommonWidgets
//    {
//        readonly TextFieldHelper _textFieldHelper = new TextFieldHelper();
//        private Action _validateFunction;

//        const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

//        private UIView View, _viewFullName, _viewLineFullName, _viewMobileNo
//            , _viewEmail, _viewLineEmail, _parentView;
//        private UILabel _lblFullNameTitle, _lblFullNameError, _lblEmailTitle, _lblEmailError;
//        private UITextField _txtFieldFullName, _txtFieldEmail;

//        public UpdatePersonalDetailCommonWidgets(UIView view)
//        {
//            View = view;
//        }

//        private void CreateCommonContainer()
//        {
//            _parentView = new UIView(new CGRect(0, 0, View.Frame.Width, (18 + 51) * 3));
//            _parentView.AddSubviews(new UIView[] { _viewFullName, _viewMobileNo, _viewEmail });
//        }

//        private void CreateCommonWidgets()
//        {
//            //FullName
//            _viewFullName = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 51)))
//            {
//                BackgroundColor = UIColor.Clear
//            };

//            _lblFullNameTitle = new UILabel
//            {
//                Frame = new CGRect(0, 0, _viewFullName.Frame.Width, 12),
//                AttributedText = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_Fullname)
//                    , AttributedStringUtility.AttributedStringType.Title),
//                TextAlignment = UITextAlignment.Left,
//                Hidden = true
//            };

//            _lblFullNameError = new UILabel
//            {
//                Frame = new CGRect(0, 37, _viewFullName.Frame.Width, 14),
//                AttributedText = AttributedStringUtility.GetAttributedString(GetErrorI18NValue(Constants.Error_InvalidFullname)
//                    , AttributedStringUtility.AttributedStringType.Error),
//                TextAlignment = UITextAlignment.Left,
//                Hidden = true
//            };

//            _txtFieldFullName = new UITextField
//            {
//                Frame = new CGRect(0, 12, _viewFullName.Frame.Width, 24),
//                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_Fullname)
//                    , AttributedStringUtility.AttributedStringType.Value),
//                TextColor = MyTNBColor.TunaGrey()
//            };

//            _viewLineFullName = GenericLine.GetLine(new CGRect(0, 36, _viewFullName.Frame.Width, 1));

//            _viewFullName.AddSubviews(new UIView[] { _lblFullNameTitle, _lblFullNameError
//                , _txtFieldFullName, _viewLineFullName });

//            //Mobile no.
//            _viewMobileNo = GetMobileNumberComponent();//GetMobileNumberWidget();

//            //Email
//            _viewEmail = new UIView((new CGRect(18, 150, View.Frame.Width - 36, 51)))
//            {
//                BackgroundColor = UIColor.Clear
//            };

//            _lblEmailTitle = new UILabel
//            {
//                Frame = new CGRect(0, 0, _viewEmail.Frame.Width, 12),
//                AttributedText = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_Email)
//                    , AttributedStringUtility.AttributedStringType.Title),
//                TextAlignment = UITextAlignment.Left,
//                Hidden = true
//            };
//            _viewEmail.AddSubview(_lblEmailTitle);

//            _lblEmailError = new UILabel
//            {
//                Frame = new CGRect(0, 37, _viewEmail.Frame.Width, 14),
//                AttributedText = AttributedStringUtility.GetAttributedString(GetErrorI18NValue(Constants.Error_InvalidEmailAddress)
//                    , AttributedStringUtility.AttributedStringType.Error),
//                TextAlignment = UITextAlignment.Left,
//                Hidden = true
//            };
//            _viewEmail.AddSubview(_lblEmailError);

//            _txtFieldEmail = new UITextField
//            {
//                Frame = new CGRect(0, 12, _viewEmail.Frame.Width, 24),
//                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_Email)
//                    , AttributedStringUtility.AttributedStringType.Value),
//                TextColor = MyTNBColor.TunaGrey()
//            };
//            _txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;
//            _viewEmail.AddSubview(_txtFieldEmail);

//            _viewLineEmail = GenericLine.GetLine(new CGRect(0, 36, _viewEmail.Frame.Width, 1));
//            _viewEmail.AddSubviews(new UIView[] { _lblEmailTitle, _lblEmailError
//                , _txtFieldEmail, _viewLineEmail });

//            _textFieldHelper.CreateTextFieldLeftView(_txtFieldFullName, "Name");
//            _textFieldHelper.CreateTextFieldLeftView(_txtFieldEmail, "Email");
//            SetValidationEvents();
//        }

//        private void SetValidationEvents()
//        {
//            SetTextFieldEvents(_txtFieldFullName, _lblFullNameTitle, _lblFullNameError
//                , _viewLineFullName, null, TNBGlobal.CustomerNamePattern);
//            SetTextFieldEvents(_txtFieldEmail, _lblEmailTitle, _lblEmailError
//                , _viewLineEmail, null, EMAIL_PATTERN);
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
//                Validate();
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
//                Validate();
//                return true;
//            };
//            textField.ShouldReturn = (sender) =>
//            {
//                sender.ResignFirstResponder();
//                return false;
//            };
//            textField.ShouldChangeCharacters += (txtField, range, replacementString) =>
//            {
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

//        public UIView GetMobileNumberComponent()
//        {
//            UIView viewMobileNumber = new UIView((new CGRect(0, 83, View.Frame.Width, ScaleUtility.GetScaledHeight(51))))
//            {
//                BackgroundColor = UIColor.Clear
//            };
//            if (ViewController != null)
//            {
//                ViewController._mobileNumberComponent = new MobileNumberComponent(viewMobileNumber, 0, false)
//                {
//                    OnDone = OnDone,
//                    CountryCode = CountryCode,
//                    OnSelect = OnSelect

//                };
//                UIView viewMobileNoComponent = ViewController._mobileNumberComponent.GetUI();
//                viewMobileNumber.AddSubview(viewMobileNoComponent);
//            }
//            return viewMobileNumber;
//        }

//        public UIView GetCommonWidgets()
//        {
//            CreateCommonWidgets();
//            CreateCommonContainer();
//            return _parentView;
//        }

//        public bool IsValidEntry()
//        {
//            bool isValidFullName = _textFieldHelper.ValidateTextField(_txtFieldFullName.Text
//                , TNBGlobal.CustomerNamePattern) && !string.IsNullOrWhiteSpace(_txtFieldFullName.Text);
//            bool isValidMobileNo = IsValidMobileNumber();
//            bool isValidEmail = _textFieldHelper.ValidateTextField(_txtFieldEmail.Text, EMAIL_PATTERN);
//            return isValidFullName && isValidMobileNo && isValidEmail;
//        }

//        public void SetValidationMethod(Action fn)
//        {
//            _validateFunction = fn;
//        }

//        private void Validate()
//        {
//            _validateFunction?.Invoke();
//        }

//        public bool IsValidMobileNumber()
//        {
//            if (ViewController != null && ViewController._mobileNumberComponent != null)
//            {
//                return ViewController._mobileNumberComponent.MobileNumber.IsValid();
//            }
//            return true;
//        }

//        public string GetEmail()
//        {
//            return _txtFieldEmail.Text ?? string.Empty;
//        }

//        public string GetMobileNumber()
//        {
//            if (ViewController != null && ViewController._mobileNumberComponent != null)
//            {
//                return ViewController._mobileNumberComponent.FullMobileNumber;
//            }
//            return string.Empty;
//        }

//        public string GetFullName()
//        {
//            return _txtFieldFullName.Text ?? string.Empty;
//        }

//        private string GetCommonI18NValue(string key)
//        {
//            return LanguageUtility.GetCommonI18NValue(key);
//        }

//        private string GetHintI18NValue(string key)
//        {
//            return LanguageUtility.GetHintI18NValue(key);
//        }

//        private string GetErrorI18NValue(string key)
//        {
//            return LanguageUtility.GetErrorI18NValue(key);
//        }

//        private void OnDone()
//        {
//            if (ViewController != null)
//            {
//                //ViewController.SetButtonEnable();
//            }
//        }

//        private string CountryCode
//        {
//            get
//            {
//                string defaultCountry = TNBGlobal.APP_COUNTRY;
//                CountryModel countryInfo = CountryManager.Instance.GetCountryInfo(defaultCountry);
//                return countryInfo != null ? countryInfo.CountryISDCode : string.Empty;
//            }
//        }

//        private Action OnSelect
//        {
//            get
//            {
//                return () =>
//                {
//                    if (ViewController != null)
//                    {
//                        SelectCountryViewController viewController = new SelectCountryViewController();
//                        UINavigationController navController = new UINavigationController(viewController)
//                        {
//                            ModalPresentationStyle = UIModalPresentationStyle.FullScreen
//                        };
//                        ViewController.NavigationController.PushViewController(viewController, true);
//                    }
//                };
//            }
//        }

//        public UpdatePersonalDetail2ViewController ViewController { set; private get; }
//    }
//}
