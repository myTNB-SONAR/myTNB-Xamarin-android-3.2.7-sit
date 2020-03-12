using System;
using Foundation;
using UIKit;

namespace myTNB
{
    public class TextViewDelegate : UITextViewDelegate
    {
        Action<NSUrl> URLAction;
        public bool InteractWithURL = true;
        public TextViewDelegate()
        {

        }
        public TextViewDelegate(Action<NSUrl> action)
        {
            URLAction = action;
        }
        public override bool ShouldInteractWithUrl(UITextView textView, NSUrl URL, NSRange characterRange)
        {
            if (URLAction != null)
            {
                URLAction?.Invoke(URL);
            }
            return InteractWithURL;
        }
    }
}