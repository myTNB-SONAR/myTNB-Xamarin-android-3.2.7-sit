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
    [Register ("LocationDetailsViewController")]
    partial class LocationDetailsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView locationDetailsTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (locationDetailsTableView != null) {
                locationDetailsTableView.Dispose ();
                locationDetailsTableView = null;
            }
        }
    }
}