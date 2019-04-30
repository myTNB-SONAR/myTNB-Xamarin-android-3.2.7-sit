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
    [Register ("AddAccountSuccessViewController")]
    partial class AddAccountSuccessViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView AccountsTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnStart { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AccountsTableView != null) {
                AccountsTableView.Dispose ();
                AccountsTableView = null;
            }

            if (btnStart != null) {
                btnStart.Dispose ();
                btnStart = null;
            }
        }
    }
}