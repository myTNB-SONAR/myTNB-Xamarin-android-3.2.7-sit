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
    [Register ("FeedbackDetailsViewController")]
    partial class FeedbackDetailsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView feedbackDetailsTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (feedbackDetailsTableView != null) {
                feedbackDetailsTableView.Dispose ();
                feedbackDetailsTableView = null;
            }
        }
    }
}