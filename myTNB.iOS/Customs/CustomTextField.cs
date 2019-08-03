using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class CustomTextField
    {
        public UIColor TextColor { set; get; } = MyTNBColor.CharcoalGrey;
        public CGSize Size { set; get; }
        public NSAttributedString PlaceHolder { set; get; }
        public UIKeyboardType KeyboardType { set; get; } = UIKeyboardType.Default;
        public UIReturnKeyType ReturnKeyType { set; get; } = UIReturnKeyType.Done;
        public Type TextFieldType { set; get; }
        public string Title { set; get; }
        public string Error { set; get; }
        public string Hint { set; get; }
        public string LeftIcon { set; get; }
        public string RightIcon { set; get; }
        public string Value { set; get; }
        public bool IsSecureEntry { set; get; } = false;
        public bool IsFieldValid { get { return _isFieldValid; } }
        public bool OnCreateValidation { set; get; }
        public Action TypingEndAction { set; get; }
        public Action TypingBeginAction { set; get; }

        public UIView ViewContainer;
        public UILabel LblTitle, LblError, LblHint;
        public UITextField TextField;

        private UIView _parentView;
        private UIView _viewLine;
        private CGPoint _location { set; get; }
        private TextFieldHelper _txtFieldHelper;
        protected bool _isFieldValid;

        public enum Type
        {
            MobileNumber,
            EmailAddress
        }

        private const string _regexMobileNumber = @"^[0-9 \+]+$";
        private const string _regexEmailAddress = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

        public CustomTextField(UIView view, CGPoint location)
        {
            _parentView = view;
            _location = location;
            _txtFieldHelper = new TextFieldHelper();
        }

        private void CreateUI()
        {
            CGSize containerSize = new CGSize(_parentView.Frame.Width - 32, 51);
            ViewContainer = new UIView(new CGRect(_location, containerSize))
            {
                BackgroundColor = UIColor.Clear
            };

            LblTitle = new UILabel(new CGRect(0, 0, ViewContainer.Frame.Width, 12))
            {
                AttributedText = AttributedStringUtility.GetAttributedStringV2(Title, AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = !HasValue
            };

            TextField = new UITextField
            {
                Frame = new CGRect(0, 12, ViewContainer.Frame.Width - 30, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedStringV2(Title, AttributedStringUtility.AttributedStringType.Value),
                TextColor = TextColor,
                SecureTextEntry = IsSecureEntry,
                KeyboardType = KeyboardType,
                Text = Value ?? string.Empty
            };
            _txtFieldHelper.CreateTextFieldLeftView(TextField, LeftIcon);
            _txtFieldHelper.SetKeyboard(TextField, ReturnKeyType);
            if (HasValue) { TextField.LeftViewMode = UITextFieldViewMode.Never; }
            if (KeyboardType == UIKeyboardType.NumberPad || KeyboardType == UIKeyboardType.PhonePad)
            {
                _txtFieldHelper.CreateDoneButton(TextField);
            }

            _viewLine = GenericLine.GetLine(new CGRect(0, 36, ViewContainer.Frame.Width, 1));

            LblError = new UILabel(new CGRect(0, 37, ViewContainer.Frame.Width, 14))
            {
                AttributedText = AttributedStringUtility.GetAttributedStringV2(Error, AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true,
            };

            LblHint = new UILabel(new CGRect(0, 37, ViewContainer.Frame.Width, 14))
            {
                AttributedText = AttributedStringUtility.GetAttributedStringV2(Hint, AttributedStringUtility.AttributedStringType.Hint),
                TextAlignment = UITextAlignment.Left,
                Hidden = string.IsNullOrEmpty(Hint) || string.IsNullOrWhiteSpace(Hint)
            };

            ViewContainer.AddSubviews(new UIView[] { LblTitle, TextField, _viewLine, LblError, LblHint });
        }

        public UIView GetUI()
        {
            CreateUI();
            SetEvents();
            if (OnCreateValidation) { ValidateField(); }
            return ViewContainer;
        }

        public void ValidateField()
        {
            _isFieldValid = _txtFieldHelper.ValidateTextField(TextField.Text, GetRegexPattern());
            if (TextFieldType == Type.MobileNumber)
            {
                _isFieldValid = _isFieldValid && _txtFieldHelper.ValidateMobileNumberLength(TextField.Text);
            }
        }

        public void SetValue(string value)
        {
            bool hasValue = false;
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                value = string.Empty;
                hasValue = true;
                TextField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
            }
            else
            {
                TextField.LeftViewMode = UITextFieldViewMode.Never;
            }
            if (TextFieldType == Type.MobileNumber)
            {
                value = _txtFieldHelper.FormatMobileNo(value);
            }
            TextField.Text = value;
            LblTitle.Hidden = hasValue;
            ValidateField();
        }

        public string GetTextFieldValue()
        {
            return TextField.Text ?? string.Empty;
        }

        public string GetNonFormattedValue()
        {
            string value = GetTextFieldValue();
            try
            {
                if (TextFieldType == Type.MobileNumber)
                {
                    if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && value.Length > 3)
                    {
                        int countryCodeIndex = value.IndexOf(@"+60", 0, StringComparison.CurrentCulture);
                        if (countryCodeIndex > -1)
                        {
                            return value.Substring(2);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in GetNonFormattedValue: " + e.Message);
            }
            return value;
        }

        private bool HasValue
        {
            get
            {
                return !string.IsNullOrEmpty(Value) && !string.IsNullOrWhiteSpace(Value);
            }
        }

        private string GetRegexPattern()
        {
            if (TextFieldType == Type.EmailAddress)
            {
                return _regexEmailAddress;
            }
            else if (TextFieldType == Type.MobileNumber)
            {
                return _regexMobileNumber;
            }
            return TNBGlobal.ACCOUNT_NAME_PATTERN;
        }

        private void SetEvents()
        {
            TextField.EditingChanged += (sender, e) =>
            {
                LblHint.Hidden = !LblError.Hidden || TextField.Text.Length == 0;
                LblTitle.Hidden = TextField.Text.Length == 0;
                if (TypingEndAction != null) { TypingEndAction.Invoke(); }
            };
            TextField.EditingDidBegin += (sender, e) =>
            {
                LblHint.Hidden = !LblError.Hidden || TextField.Text.Length == 0;
                LblTitle.Hidden = TextField.Text.Length == 0;
                if (TextFieldType == Type.MobileNumber && TextField.Text.Length == 0)
                {
                    TextField.Text += TNBGlobal.MobileNoPrefix;
                }
                _viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                TextField.LeftViewMode = UITextFieldViewMode.Never;
                if (TypingBeginAction != null) { TypingBeginAction.Invoke(); }
            };
            TextField.ShouldEndEditing = (sender) =>
            {
                LblTitle.Hidden = TextField.Text.Length == 0;
                _isFieldValid = _txtFieldHelper.ValidateTextField(TextField.Text, GetRegexPattern());
                if (TextFieldType == Type.MobileNumber)
                {
                    _isFieldValid = _isFieldValid && _txtFieldHelper.ValidateMobileNumberLength(TextField.Text);
                }

                LblError.Hidden = _isFieldValid;
                LblHint.Hidden = true;
                _viewLine.BackgroundColor = _isFieldValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                TextField.TextColor = _isFieldValid ? TextColor : MyTNBColor.Tomato;
                if (TypingEndAction != null) { TypingEndAction.Invoke(); }
                return true;
            };
            TextField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            TextField.ShouldChangeCharacters += (sender, range, replacementString) =>
            {
                if (TextFieldType == Type.MobileNumber)
                {
                    bool isCharValid = _txtFieldHelper.ValidateTextField(replacementString, TNBGlobal.MobileNoPattern);
                    if (!isCharValid) { return false; }

                    if (range.Location >= TNBGlobal.MobileNoPrefix.Length)
                    {
                        string content = _txtFieldHelper.TrimAllSpaces(TextField.Text);
                        nint count = content.Length + replacementString.Length - range.Length;
                        return count <= TNBGlobal.MobileNumberMaxCharCount;
                    }
                    return false;
                }
                return true;
            };
            TextField.EditingDidEnd += (sender, e) =>
            {
                if (TextField.Text.Length == 0)
                {
                    TextField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
                }
            };
        }
    }
}