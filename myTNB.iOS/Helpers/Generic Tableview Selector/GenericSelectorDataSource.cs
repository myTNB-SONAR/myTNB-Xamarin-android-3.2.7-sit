using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class GenericSelectorDataSource : UITableViewSource
    {
        readonly GenericSelectorViewController _controller;

        public GenericSelectorDataSource(GenericSelectorViewController controller)
        {
            _controller = controller;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 0;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return new UITableViewCell();
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {

        }
    }
}