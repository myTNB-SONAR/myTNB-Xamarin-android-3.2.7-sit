using System;
using System.Drawing;
using System.Text.RegularExpressions;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class TextFieldHelper
    {
        /// <summary>
        /// Create and add Done button in UITextField keyboard
        /// </summary>
        /// <param name="textField">Text field.</param>
        public void CreateDoneButton(UITextField textField)
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                textField.ResignFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            textField.InputAccessoryView = toolbar;
        }
        /// <summary>
        /// Creates image in left of the text field.
        /// </summary>
        /// <param name="textField">Text field.</param>
        /// <param name="imageName">Image name.</param>
		public void CreateTextFieldLeftView(UITextField textField, string imageName)
        {
            var leftView = new UIImageView(UIImage.FromBundle(imageName));
            leftView.Frame = new CGRect(leftView.Frame.X, leftView.Frame.Y, leftView.Frame.Width + ScaleUtility.GetScaledWidth(6F), leftView.Frame.Height);
            leftView.ContentMode = UIViewContentMode.Left;
            textField.LeftView = leftView;
            textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
        }
        /// <summary>
        /// Creates image in right of the text field.
        /// </summary>
        /// <param name="textField">Text field.</param>
        /// <param name="imageName">Image name.</param>
        public void CreateTextFieldRightView(UITextField textField, string imageName)
        {
            var rightView = new UIImageView(UIImage.FromBundle(imageName));
            rightView.Frame = new CGRect(rightView.Frame.X, rightView.Frame.Y, rightView.Frame.Width + 15, rightView.Frame.Height);
            rightView.ContentMode = UIViewContentMode.Center;
            textField.RightView = rightView;
            textField.RightViewMode = UITextFieldViewMode.Always;
        }
        /// <summary>
        /// Validates the text field.
        /// </summary>
        /// <returns><c>true</c>, if text field was validated, <c>false</c> otherwise.</returns>
        /// <param name="text">Text.</param>
        /// <param name="regexPattern">Regex pattern.</param>
		public bool ValidateTextField(string text, string regexPattern)
        {
            Regex regex = new Regex(regexPattern);
            Match match = regex.Match(text);
            return match.Success;
        }
        /// <summary>
        /// Validates the CA text field.
        /// </summary>
        /// <returns><c>true</c>, if CA text field was validated, <c>false</c> otherwise.</returns>
        /// <param name="text">Text.</param>
        public bool ValidateTextFieldWithLength(string text, int length)
        {
            return (text.Length == length);
        }

        /// <summary>
        /// Validates the mobile number length.
        /// </summary>
        /// <returns><c>true</c>, if mobile number was validated, <c>false</c> otherwise.</returns>
        /// <param name="text">Text.</param>
        public bool ValidateMobileNumberLength(string text)
        {
            var textStr = TrimAllSpaces(text);

            // lengths assume +60 is included. valid mobile number lengths are 9 and 10 + 3 country code length
            return (text.Length == 12 || text.Length == TNBGlobal.MobileNumberMaxCharCount);
        }

        /// <summary>
        /// Validates the length of the account number when multiple lengths are allowed. For single length, use ValidateTextFieldWithLength
        /// </summary>
        /// <returns><c>true</c>, if account number length was validated, <c>false</c> otherwise.</returns>
        /// <param name="text">Text.</param>
        public bool ValidateAccountNumberLength(string text)
        {
            var textStr = TrimAllSpaces(text);

            return (text.Length == TNBGlobal.AccountNumberLowCharLimit || text.Length == TNBGlobal.AccountNumberHighCharLimit);
        }

        /// <summary>
        /// Trims all spaces.
        /// </summary>
        /// <returns>The mobile number spaces.</returns>
        /// <param name="text">Text.</param>
        public string TrimAllSpaces(string text)
        {
            return text?.Trim()?.Replace(" ", string.Empty) ?? string.Empty;
        }

        /// <summary>
        /// Sets the keyboard.
        /// </summary>
        /// <param name="textField">Text field.</param>
        public void SetKeyboard(UITextField textField, UIReturnKeyType returnKeyType = UIReturnKeyType.Done)
        {
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.ReturnKeyType = returnKeyType;
        }

        /// <summary>
        /// Formats the mobile no.
        /// </summary>
        /// <returns>The mobile no.</returns>
        /// <param name="mobileNo">Mobile no.</param>
        public string FormatMobileNo(string mobileNo)
        {
            if (string.IsNullOrEmpty(mobileNo))
                return string.Empty;

            string formattedMobileNo = string.Empty;
            string prefix = TNBGlobal.MobileNoPrefix;

            if (mobileNo.StartsWith("+60", StringComparison.InvariantCulture))
            {
                formattedMobileNo = mobileNo;
            }
            else if (mobileNo.StartsWith("+0", StringComparison.InvariantCulture))
            {
                formattedMobileNo = prefix + mobileNo.Substring(2);
            }
            else if (mobileNo.StartsWith("+1", StringComparison.InvariantCulture) || mobileNo.StartsWith("0", StringComparison.InvariantCulture))
            {
                formattedMobileNo = prefix + mobileNo.Substring(1);
            }
            else if (mobileNo.StartsWith("60", StringComparison.InvariantCulture) && Array.Exists(TNBGlobal.MobileNumberLimits, s => s == mobileNo.Substring(2).Length))
            {
                formattedMobileNo = prefix + mobileNo.Substring(2);
            }
            else
            {
                formattedMobileNo = prefix + mobileNo;
            }
            return formattedMobileNo;
        }

        public string RemoveCountryCode(string mobileNo)
        {
            if (string.IsNullOrEmpty(mobileNo))
            {
                return string.Empty;
            }

            string formattedMobileNo = string.Empty;
            string prefix = TNBGlobal.MobileNoPrefix;

            if (mobileNo.StartsWith("+60", StringComparison.InvariantCulture))
            {
                formattedMobileNo = mobileNo;
            }
            else if (mobileNo.StartsWith("+0", StringComparison.InvariantCulture))
            {
                formattedMobileNo = prefix + mobileNo.Substring(2);
            }
            else if (mobileNo.StartsWith("+1", StringComparison.InvariantCulture) || mobileNo.StartsWith("0", StringComparison.InvariantCulture))
            {
                formattedMobileNo = prefix + mobileNo.Substring(1);
            }
            else if (mobileNo.StartsWith("60", StringComparison.InvariantCulture) && Array.Exists(TNBGlobal.MobileNumberLimits, s => s == mobileNo.Substring(2).Length))
            {
                formattedMobileNo = prefix + mobileNo.Substring(2);
            }
            else
            {
                formattedMobileNo = prefix + mobileNo;
            }
            return formattedMobileNo;
        }
    }
}