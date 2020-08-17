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
            EmailAddress,
            NameUser
        }

        private const string _regexMobileNumber = @"^[0-9 \+]+$";
        private const string _regexEmailAddress = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

        public CustomTextField(UIView view, CGPoint location)
        {
            _parentView = view;
            _location = location;
            _txtFieldHelper = new TextFieldHelper();
        }

        private nfloat GetScaledWidth(nfloat w)
        {
            return ScaleUtility.GetScaledWidth(w);
        }

        private nfloat GetScaledHeight(nfloat h)
        {
            return ScaleUtility.GetScaledHeight(h);
        }

        private void CreateUI()
        {
            CGSize containerSize = new CGSize(_parentView.Frame.Width - GetScaledWidth(32), GetScaledHeight(51));
            ViewContainer = new UIView(new CGRect(_location, containerSize))
            {
                BackgroundColor = UIColor.Clear
            };

            LblTitle = new UILabel(new CGRect(0, 0, ViewContainer.Frame.Width, GetScaledHeight(12)))
            {
                AttributedText = AttributedStringUtility.GetAttributedString(Title, AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = !HasValue ? false : true
            };

            TextField = new UITextField
            {
                Frame = new CGRect(0, GetScaledHeight(12), ViewContainer.Frame.Width - GetScaledWidth(30), GetScaledHeight(24)),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(Title, AttributedStringUtility.AttributedStringType.Value),
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

            _viewLine = GenericLine.GetLine(new CGRect(0, GetScaledHeight(36), ViewContainer.Frame.Width, GetScaledHeight(1)));

            LblError = new UILabel(new CGRect(0, GetScaledHeight(37), ViewContainer.Frame.Width, GetScaledHeight(14)))
            {
                AttributedText = AttributedStringUtility.GetAttributedString(Error, AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true,
            };

            LblHint = new UILabel(new CGRect(0, GetScaledHeight(37), ViewContainer.Frame.Width, GetScaledHeight(14)))
            {
                AttributedText = AttributedStringUtility.GetAttributedString(Hint, AttributedStringUtility.AttributedStringType.Hint),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
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

        public bool IsFieldValid
        {
            get
            {
                ValidateField();
                return _isFieldValid;
            }
        }

        public void SetState(bool isValid)
        {
            LblError.Hidden = isValid;
            _viewLine.BackgroundColor = isValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
            TextField.TextColor = isValid ? TextColor : MyTNBColor.Tomato;
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

        public bool SetEnable
        {
            set { TextField.Enabled = value; }
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

        private bool IsEmpty(string value)
        {
            if (TextFieldType == Type.MobileNumber)
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)) { return true; }
                int countryCodeIndex = value.IndexOf(@"+60", 0, StringComparison.CurrentCulture);
                return countryCodeIndex > -1 && value.Length == 3;
            }
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

        private void SetEvents()
        {
            TextField.EditingChanged += (sender, e) =>
            {
                string value = TextField.Text;
                LblHint.Hidden = !IsEmpty(value) && !LblError.Hidden || value.Length == 0;
                bool hidden;
                if (TextFieldType == Type.MobileNumber)
                {
                    hidden = value.Length != 3 && IsEmpty(value) || value.Length == 0;
                }
                else
                {
                    hidden = IsEmpty(value) || value.Length == 0;
                }
                LblTitle.Hidden = hidden;
                LblError.Hidden = true;
                _viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;
                TextField.TextColor = TextColor;
                if (TypingEndAction != null) { TypingEndAction.Invoke(); }
            };
            TextField.EditingDidBegin += (sender, e) =>
            {
                if (TextFieldType == Type.MobileNumber && TextField.Text.Length == 0)
                {
                    TextField.Text += TNBGlobal.MobileNoPrefix;
                }
                string value = TextField.Text;
                bool hidden;
                if (TextFieldType == Type.MobileNumber)
                {
                    hidden = value.Length != 3 && IsEmpty(value) || value.Length == 0;
                }
                else
                {
                    hidden = IsEmpty(value) || value.Length == 0;
                }
                LblTitle.Hidden = hidden;
                LblHint.Hidden = !IsEmpty(value) && !LblError.Hidden || value.Length == 0;
                _viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                TextField.LeftViewMode = UITextFieldViewMode.Never;
                if (TypingBeginAction != null) { TypingBeginAction.Invoke(); }
            };
            TextField.ShouldEndEditing = (sender) =>
            {
                string value = TextField.Text;
                bool hidden;
                if (TextFieldType == Type.MobileNumber)
                {
                    hidden = value.Length != 3 && IsEmpty(value) || value.Length == 0;
                }
                else
                {
                    hidden = IsEmpty(value) || value.Length == 0;
                }
                LblTitle.Hidden = hidden;
                _isFieldValid = _txtFieldHelper.ValidateTextField(value, GetRegexPattern());
                if (TextFieldType == Type.MobileNumber)
                {
                    _isFieldValid = _isFieldValid && _txtFieldHelper.ValidateMobileNumberLength(value);
                }

                LblError.Hidden = IsEmpty(value) || _isFieldValid;
                LblHint.Hidden = true;
                _viewLine.BackgroundColor = IsEmpty(value) || _isFieldValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                TextField.TextColor = IsEmpty(value) || _isFieldValid ? TextColor : MyTNBColor.Tomato;
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
                string value = TextField.Text;
                if (value.Length == 0)
                {
                    TextField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
                }
            };
        }
    }
}