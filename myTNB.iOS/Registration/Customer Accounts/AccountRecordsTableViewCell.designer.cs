// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace myTNB.Registration.CustomerAccounts
{
    [Register ("AccountRecordsTableViewCell")]
    partial class AccountRecordsTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnDelete { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAccountNo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAddress { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblNicknameError { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblNickNameTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView lineView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtFieldNickname { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewSeparator { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnDelete != null) {
                btnDelete.Dispose ();
                btnDelete = null;
            }

            if (lblAccountNo != null) {
                lblAccountNo.Dispose ();
                lblAccountNo = null;
            }

            if (lblAddress != null) {
                lblAddress.Dispose ();
                lblAddress = null;
            }

            if (lblNicknameError != null) {
                lblNicknameError.Dispose ();
                lblNicknameError = null;
            }

            if (lblNickNameTitle != null) {
                lblNickNameTitle.Dispose ();
                lblNickNameTitle = null;
            }

            if (lineView != null) {
                lineView.Dispose ();
                lineView = null;
            }

            if (txtFieldNickname != null) {
                txtFieldNickname.Dispose ();
                txtFieldNickname = null;
            }

            if (viewSeparator != null) {
                viewSeparator.Dispose ();
                viewSeparator = null;
            }
        }
    }
}