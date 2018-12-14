using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Home.Feedback
{
    public class SelectFeedbackTypeDataSource: UITableViewSource
    {

        FeedbackTypeViewController _controller;
        List<OtherFeedbackTypeDataModel>  _feedbackTypeList = new List<OtherFeedbackTypeDataModel>();

        public SelectFeedbackTypeDataSource(FeedbackTypeViewController controller, List<OtherFeedbackTypeDataModel> feedbackTypeList)
        {
            _controller = controller;
            _feedbackTypeList = feedbackTypeList;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("FeedbackTypeCell", indexPath) as FeedbackTypeCell;

            cell.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            cell.LayoutMargins = new UIEdgeInsets(0, 0, 0, 0);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            cell.FeedbackTypeLabel.Text = _feedbackTypeList[indexPath.Row].FeedbackTypeName;

            if (indexPath.Row == DataManager.DataManager.SharedInstance.CurrentSelectedFeedbackTypeIndex)
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
            return _feedbackTypeList != null && _feedbackTypeList.Count != 0 ? _feedbackTypeList.Count : 0;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedFeedbackTypeIndex = indexPath.Row;
            _controller.NavigationController.PopViewController(true);
        }


    }
}
