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
    [Register ("AddCardCell")]
    partial class AddCardCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAddCard { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblAddCard != null) {
                lblAddCard.Dispose ();
                lblAddCard = null;
            }
        }
    }
}