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
    [Register ("FeedbackViewController")]
    partial class FeedbackViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView feedbackTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (feedbackTableView != null) {
                feedbackTableView.Dispose ();
                feedbackTableView = null;
            }
        }
    }
}