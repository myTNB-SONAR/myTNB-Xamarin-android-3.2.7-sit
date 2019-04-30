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
    [Register ("DashboardAccountCell")]
    partial class DashboardAccountCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgRe { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAccountSubTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAccountTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAmountSubTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAmountTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewLine { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (imgRe != null) {
                imgRe.Dispose ();
                imgRe = null;
            }

            if (lblAccountSubTitle != null) {
                lblAccountSubTitle.Dispose ();
                lblAccountSubTitle = null;
            }

            if (lblAccountTitle != null) {
                lblAccountTitle.Dispose ();
                lblAccountTitle = null;
            }

            if (lblAmountSubTitle != null) {
                lblAmountSubTitle.Dispose ();
                lblAmountSubTitle = null;
            }

            if (lblAmountTitle != null) {
                lblAmountTitle.Dispose ();
                lblAmountTitle = null;
            }

            if (viewLine != null) {
                viewLine.Dispose ();
                viewLine = null;
            }
        }
    }
}