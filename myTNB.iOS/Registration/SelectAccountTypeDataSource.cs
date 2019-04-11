using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Registration
{
    public class SelectAccountTypeDataSource : UITableViewSource
    {
        List<String> _accountTypes = new List<String>();
        SelectAccountTypeViewController _controller;

        public SelectAccountTypeDataSource(SelectAccountTypeViewController controller)
        {
            _controller = controller;
            _accountTypes.Add("Registration_Residential".Translate());
            _accountTypes.Add("Registration_Commercial".Translate());
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("SelectAccountTypeCell", indexPath) as SelectAccountTypeCell;
            cell.AccountTypeLabel.Text = _accountTypes[indexPath.Row];
            if (indexPath.Row == DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex)
            {
                cell.Accessory = UITableViewCellAccessory.None;
                cell.AccessoryView = new UIView(new CGRect(0, 0, 24, 24));
                UIImageView imgViewTick = new UIImageView(new CGRect(0, 0, 24, 24));
                imgViewTick.Image = UIImage.FromBundle("Table-Tick");
                cell.AccessoryView.AddSubview(imgViewTick);
            }
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _accountTypes.Count;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex = indexPath.Row;
            _controller.NavigationController.PopViewController(true);
        }
    }
}