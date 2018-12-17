using System;
using UIKit;
using System.Text.RegularExpressions;
using System.Drawing;
using CoreGraphics;
namespace myTNB.Customs
{
    public class FeedbackTextView : UITextView
    {
        public UILabel placeholder;

        /// <summary>
        /// Create and add Placeholder in UITextView
        /// </summary>
        /// <param name="text">Placeholder.</param>
        public void SetPlaceholder(string text)
        {
            placeholder = new UILabel(new CGRect(3, 0, this.Frame.Width, this.Frame.Height))
            {
                Text = text,
                TextColor = myTNBColor.SilverChalice(),
                Font = myTNBFont.MuseoSans18_300(),
            };
            AddSubview(placeholder);
        }

        /// <summary>
        /// Hide or Show Placeholder in UITextView
        /// </summary>
        /// <param name="flag">flag.</param>
        public void SetPlaceholderHidden(bool flag)
        {
            placeholder.Hidden = flag;
        }

        /// <summary>
        /// Create and add Done button in UITextView keyboard
        /// </summary>
        /// <param name="textView">Text view.</param>
        public void CreateDoneButton()
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                this.ResignFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            this.InputAccessoryView = toolbar;
        }
        /// <summary>
        /// Validates the text view.
        /// </summary>
        /// <returns><c>true</c>, if text view was validated, <c>false</c> otherwise.</returns>
        /// <param name="text">Text.</param>
        /// <param name="regexPattern">Regex pattern.</param>
        public bool ValidateTextView(string text, string regexPattern)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            Regex regex = new Regex(regexPattern);
            Match match = regex.Match(text);
            return match.Success;
        }
        /// <summary>
        /// Sets the keyboard.
        /// </summary>
        /// <param name="textView">Text view.</param>
        public void SetKeyboard()
        {
            this.AutocorrectionType = UITextAutocorrectionType.No;
            this.AutocapitalizationType = UITextAutocapitalizationType.None;
            this.SpellCheckingType = UITextSpellCheckingType.No;
        }
    }
}
