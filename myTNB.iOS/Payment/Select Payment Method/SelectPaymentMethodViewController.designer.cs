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
    [Register ("SelectPaymentMethodViewController")]
    partial class SelectPaymentMethodViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView selectPaymentTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel toastMessage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView toastView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (selectPaymentTableView != null) {
                selectPaymentTableView.Dispose ();
                selectPaymentTableView = null;
            }

            if (toastMessage != null) {
                toastMessage.Dispose ();
                toastMessage = null;
            }

            if (toastView != null) {
                toastView.Dispose ();
                toastView = null;
            }
        }
    }
}