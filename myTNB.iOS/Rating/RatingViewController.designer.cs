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
    [Register ("RatingViewController")]
    partial class RatingViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tableViewRating { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tableViewRating != null) {
                tableViewRating.Dispose ();
                tableViewRating = null;
            }
        }
    }
}