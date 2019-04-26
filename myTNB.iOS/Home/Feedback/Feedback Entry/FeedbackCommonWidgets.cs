using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Feedback.FeedbackEntry
{
    public class FeedbackCommonWidgets
    {
        readonly TextFieldHelper _textFieldHelper = new TextFieldHelper();
        Action _validateFunction;

        const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        const string MOBILE_NO_PATTERN = @"^[0-9 \+]+$";

        UIView View, _viewFullName, _viewLineFullName, _viewMobileNo, _viewLineMobileNo
            , _viewEmail, _viewLineEmail, _parentView;
        UILabel _lblFullNameTitle, _lblFullNameError, _lblMobileNoTitle, _lblMobileNoError
            , _lblMobileNoHint, _lblEmailTitle, _lblEmailError;
        UITextField _txtFieldFullName, _txtFieldMobileNo, _txtFieldEmail;

        public FeedbackCommonWidgets(UIView view)
        {
            View = view;
        }

        private void CreateCommonContainer()
        {
            _parentView = new UIView(new CGRect(0, 0, View.Frame.Width, (18 + 51) * 3));
            _parentView.AddSubviews(new UIView[] { _viewFullName, _viewMobileNo, _viewEmail });
        }

        private void CreateCommonWidgets()
        {
            //FullName
            _viewFullName = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblFullNameTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFullName.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString("Common_Fullname"
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _lblFullNameError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewFullName.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString("Invalid_Fullname"
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _txtFieldFullName = new UITextField
            {
                Frame = new CGRect(0, 12, _viewFullName.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString("Common_Fullname"
                    , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };

            _viewLineFullName = GenericLine.GetLine(new CGRect(0, 36, _viewFullName.Frame.Width, 1));

            _viewFullName.AddSubviews(new UIView[] { _lblFullNameTitle, _lblFullNameError
                , _txtFieldFullName, _viewLineFullName });

            //Mobile no.
            _viewMobileNo = GetMobileNumberWidget();

            //Email
            _viewEmail = new UIView((new CGRect(18, 150, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblEmailTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewEmail.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString("Common_Email"
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };
            _viewEmail.AddSubview(_lblEmailTitle);

            _lblEmailError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewEmail.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString("Invalid_Email"
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };
            _viewEmail.AddSubview(_lblEmailError);

            _txtFieldEmail = new UITextField
            {
                Frame = new CGRect(0, 12, _viewEmail.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString("Common_Email"
                    , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };
            _txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;
            _viewEmail.AddSubview(_txtFieldEmail);

            _viewLineEmail = GenericLine.GetLine(new CGRect(0, 36, _viewEmail.Frame.Width, 1));
            _viewEmail.AddSubviews(new UIView[] { _lblEmailTitle, _lblEmailError
                , _txtFieldEmail, _viewLineEmail });

            _textFieldHelper.CreateTextFieldLeftView(_txtFieldFullName, "Name");
            _textFieldHelper.CreateTextFieldLeftView(_txtFieldEmail, "Email");
            SetValidationEvents();
        }

        void SetValidationEvents()
        {
            SetTextFieldEvents(_txtFieldFullName, _lblFullNameTitle, _lblFullNameError
                , _viewLineFullName, null, TNBGlobal.CustomerNamePattern);
            SetTextFieldEvents(_txtFieldEmail, _lblEmailTitle, _lblEmailError
                , _viewLineEmail, null, EMAIL_PATTERN);
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
                Validate();
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                if (textField == _txtFieldMobileNo)
                {
                    if (textField.Text.Length == 0)
                    {
                        textField.Text += TNBGlobal.MobileNoPrefix;
                    }
                }
                lblHint.Hidden = !lblError.Hidden || textField.Text.Length == 0;
                lblTitle.Hidden = textField.Text.Length == 0;
                textField.LeftViewMode = UITextFieldViewMode.Never;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                bool isValid = true;
                bool isEmptyAllowed = true;
                if (textField == _txtFieldMobileNo)
                {
                    if (textField.Text.Length < 4)
                    {
                        textField.Text = string.Empty;
                    }
                    isValid = _textFieldHelper.ValidateMobileNumberLength(textField.Text);
                    isEmptyAllowed = false;
                }
                lblTitle.Hidden = textField.Text.Length == 0;
                isValid = isValid && _textFieldHelper.ValidateTextField(textField.Text, pattern);

                bool isNormal = isValid || (textField.Text.Length == 0 && isEmptyAllowed);
                lblError.Hidden = isNormal;
                lblHint.Hidden = true;
                viewLine.BackgroundColor = isNormal ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isNormal ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
                Validate();
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                if (textField == _txtFieldMobileNo)
                {
                    if (textField.Text.Length < 4)
                    {
                        textField.Text = string.Empty;
                    }
                }
                sender.ResignFirstResponder();
                return false;
            };
            textField.ShouldChangeCharacters += (txtField, range, replacementString) =>
            {
                if (txtField == _txtFieldMobileNo)
                {
                    bool isCharValid = _textFieldHelper.ValidateTextField(replacementString, TNBGlobal.MobileNoPattern);
                    if (!isCharValid)
                        return false;

                    if (range.Location >= TNBGlobal.MobileNoPrefix.Length)
                    {
                        string content = _textFieldHelper.TrimAllSpaces(((UITextField)txtField).Text);
                        var count = content.Length + replacementString.Length - range.Length;
                        return count <= TNBGlobal.MobileNumberMaxCharCount;
                    }
                    return false;
                }
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

        UIView GetMobileNumberWidget()
        {
            //Mobile no.
            _viewMobileNo = new UIView((new CGRect(18, 83, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblMobileNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewMobileNo.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString("Common_MobileNumber"
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _lblMobileNoError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewMobileNo.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString("Invalid_MobileNumber"
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _lblMobileNoHint = new UILabel
            {
                Frame = new CGRect(0, 37, _viewMobileNo.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString("Hint_MobileNumber"
                    , AttributedStringUtility.AttributedStringType.Hint),
                TextAlignment = UITextAlignment.Left
            };
            _lblMobileNoHint.Hidden = true;

            _txtFieldMobileNo = new UITextField
            {
                Frame = new CGRect(0, 12, _viewMobileNo.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString("Common_MobileNumber"
                    , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };
            _txtFieldMobileNo.KeyboardType = UIKeyboardType.NumberPad;

            _viewLineMobileNo = GenericLine.GetLine(new CGRect(0, 36, _viewMobileNo.Frame.Width, 1));
            _viewMobileNo.AddSubviews(new UIView[] { _lblMobileNoTitle, _lblMobileNoError
                , _lblMobileNoHint, _txtFieldMobileNo , _viewLineMobileNo });

            _textFieldHelper.CreateDoneButton(_txtFieldMobileNo);
            _textFieldHelper.CreateTextFieldLeftView(_txtFieldMobileNo, "Mobile");
            SetTextFieldEvents(_txtFieldMobileNo, _lblMobileNoTitle, _lblMobileNoError
                , _viewLineMobileNo, _lblMobileNoHint, MOBILE_NO_PATTERN);
            return _viewMobileNo;
        }

        public UIView GetMobileNumberComponent()
        {
            UIView mobileNumberWidget = GetMobileNumberWidget();
            mobileNumberWidget.Frame = new CGRect(mobileNumberWidget.Frame.X, 18
                , mobileNumberWidget.Frame.Width, mobileNumberWidget.Frame.Height + 18);
            return mobileNumberWidget;
        }

        public UIView GetCommonWidgets()
        {
            CreateCommonWidgets();
            CreateCommonContainer();
            return _parentView;
        }

        public bool IsValidEntry()
        {
            bool isValidFullName = _textFieldHelper.ValidateTextField(_txtFieldFullName.Text
                , TNBGlobal.CustomerNamePattern) && !string.IsNullOrWhiteSpace(_txtFieldFullName.Text);
            bool isValidMobileNo = IsValidMobileNumber();
            bool isValidEmail = _textFieldHelper.ValidateTextField(_txtFieldEmail.Text, EMAIL_PATTERN);
            return isValidFullName && isValidMobileNo && isValidEmail;
        }

        public void SetValidationMethod(Action fn)
        {
            _validateFunction = fn;
        }

        void Validate()
        {
            _validateFunction?.Invoke();
        }

        public bool IsValidMobileNumber()
        {
            return _textFieldHelper.ValidateTextField(_txtFieldMobileNo.Text, MOBILE_NO_PATTERN)
                && _textFieldHelper.ValidateMobileNumberLength(_txtFieldMobileNo.Text);
        }

        public string GetEmail()
        {
            return _txtFieldEmail.Text ?? string.Empty;
        }

        public string GetMobileNumber()
        {
            return _txtFieldMobileNo.Text ?? string.Empty;
        }

        public string GetFullName()
        {
            return _txtFieldFullName.Text ?? string.Empty;
        }
    }
}