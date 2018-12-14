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
		public void CreateTextFieldLeftView(UITextField textField, String imageName)
        {
            var leftView = new UIImageView(UIImage.FromBundle(imageName));
            leftView.Frame = new CGRect(leftView.Frame.X, leftView.Frame.Y, leftView.Frame.Width + 10, leftView.Frame.Height);
            leftView.ContentMode = UIViewContentMode.Left;
            textField.LeftView = leftView;
            textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
        }
        /// <summary>
        /// Creates image in right of the text field.
        /// </summary>
        /// <param name="textField">Text field.</param>
        /// <param name="imageName">Image name.</param>
        public void CreateTextFieldRightView(UITextField textField, String imageName)
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
        /// Sets the keyboard.
        /// </summary>
        /// <param name="textField">Text field.</param>
        public void SetKeyboard(UITextField textField)
        {
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.ReturnKeyType = UIReturnKeyType.Done;
        }
    }
}