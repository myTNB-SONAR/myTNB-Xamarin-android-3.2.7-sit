﻿// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace myTNB.Helpers.GenericTableviewSelector
{
    [Register ("GenericSelectorViewController")]
    partial class GenericSelectorViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView selectStoreTypeTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (selectStoreTypeTableView != null) {
                selectStoreTypeTableView.Dispose ();
                selectStoreTypeTableView = null;
            }
        }
    }
}