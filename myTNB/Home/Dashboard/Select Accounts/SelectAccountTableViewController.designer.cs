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
    [Register ("SelectAccountTableViewController")]
    partial class SelectAccountTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView accountRecordsTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (accountRecordsTableView != null) {
                accountRecordsTableView.Dispose ();
                accountRecordsTableView = null;
            }
        }
    }
}