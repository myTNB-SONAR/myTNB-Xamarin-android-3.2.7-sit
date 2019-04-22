// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace myTNB
{
    [Register ("FeedbackTypeCell")]
    partial class FeedbackTypeCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblFeedbackType { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblFeedbackType != null) {
                lblFeedbackType.Dispose ();
                lblFeedbackType = null;
            }
        }
    }
}