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
    [Register ("SelectBillsViewController")]
    partial class SelectBillsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView BottomContainerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BtnPayBill { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView SelectBillsTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BottomContainerView != null) {
                BottomContainerView.Dispose ();
                BottomContainerView = null;
            }

            if (BtnPayBill != null) {
                BtnPayBill.Dispose ();
                BtnPayBill = null;
            }

            if (SelectBillsTableView != null) {
                SelectBillsTableView.Dispose ();
                SelectBillsTableView = null;
            }
        }
    }
}