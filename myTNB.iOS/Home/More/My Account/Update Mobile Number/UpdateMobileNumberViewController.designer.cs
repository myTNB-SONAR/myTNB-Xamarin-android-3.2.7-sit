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
    [Register ("UpdateMobileNumberViewController")]
    partial class UpdateMobileNumberViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView toastView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (toastView != null) {
                toastView.Dispose ();
                toastView = null;
            }
        }
    }
}