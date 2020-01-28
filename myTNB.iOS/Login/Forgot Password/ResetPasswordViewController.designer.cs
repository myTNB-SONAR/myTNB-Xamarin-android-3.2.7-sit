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

namespace myTNB.Login.ForgotPassword
{
    [Register ("ResetPasswordViewController")]
    partial class ResetPasswordViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSubmit { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnSubmit != null) {
                btnSubmit.Dispose ();
                btnSubmit = null;
            }
        }
    }
}