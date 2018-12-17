using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Home.Feedback
{
    public class SelectAccountNoDataSource : UITableViewSource
    {
        SelectAccountNoViewController _controller;

        public SelectAccountNoDataSource(SelectAccountNoViewController controller)
        {
            _controller = controller;

        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("FeedbackAccountCell", indexPath) as FeedbackAccountCell;

            cell.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            cell.LayoutMargins = new UIEdgeInsets(0, 0, 0, 0);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            cell.AccountNumber.Text = DataManager.DataManager.SharedInstance.AccountRecordsList.d[indexPath.Row].accNum + " - " + DataManager.DataManager.SharedInstance.AccountRecordsList.d[indexPath.Row].accDesc;

            if (indexPath.Row == DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex)
            {
                cell.Accessory = UITableViewCellAccessory.None;
                cell.AccessoryView = new UIView(new CGRect(0, 0, 24, 24));
                UIImageView imgViewTick = new UIImageView(new CGRect(0, 0, 24, 24));
                imgViewTick.Image = UIImage.FromBundle("Table-Tick");
                cell.AccessoryView.AddSubview(imgViewTick);
            } else {
                if (cell != null && cell.AccessoryView != null && cell.AccessoryView.Subviews != null)
                {
                    foreach (var subView in cell.AccessoryView.Subviews)
                    {
                        subView.RemoveFromSuperview();
                    }
                }
            }

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
            {
                return DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count;
            }
            return 0;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex = indexPath.Row;
            _controller.NavigationController.PopViewController(true);
        }
    }
}