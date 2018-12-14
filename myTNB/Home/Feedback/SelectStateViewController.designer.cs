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
    [Register ("SelectStateViewController")]
    partial class SelectStateViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView StateTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (StateTableView != null) {
                StateTableView.Dispose ();
                StateTableView = null;
            }
        }
    }
}