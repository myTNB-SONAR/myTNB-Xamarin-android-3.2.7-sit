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
    [Register ("SuccessfullyAddedAccountCell")]
    partial class SuccessfullyAddedAccountCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAccountNumber { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblNickName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtViewAddress { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblAccountNumber != null) {
                lblAccountNumber.Dispose ();
                lblAccountNumber = null;
            }

            if (lblNickName != null) {
                lblNickName.Dispose ();
                lblNickName = null;
            }

            if (txtViewAddress != null) {
                txtViewAddress.Dispose ();
                txtViewAddress = null;
            }
        }
    }
}