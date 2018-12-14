using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Home.Feedback
{
    public class SelectStateDataSource: UITableViewSource
    {
        SelectStateViewController _controller;
        List<StatesForFeedbackDataModel> _statesForFeedbackList = new List<StatesForFeedbackDataModel>();

        public SelectStateDataSource(SelectStateViewController controller, List<StatesForFeedbackDataModel> statesForFeedbackList)
        {
            _controller = controller;
            _statesForFeedbackList = statesForFeedbackList;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("StateCell", indexPath) as StateCell;

            cell.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            cell.LayoutMargins = new UIEdgeInsets(0, 0, 0, 0);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            cell.StateForFeedbackLabel.Text = _statesForFeedbackList[indexPath.Row].StateName;

            if (indexPath.Row == DataManager.DataManager.SharedInstance.CurrentSelectedStateForFeedbackIndex) 
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
            return _statesForFeedbackList != null && _statesForFeedbackList.Count != 0 ? _statesForFeedbackList.Count : 0;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedStateForFeedbackIndex = indexPath.Row;
            _controller.NavigationController.PopViewController(true);
        }
    }
}
