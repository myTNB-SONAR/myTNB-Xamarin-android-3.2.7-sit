using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.PushNotification
{
    public class SelectNotificationDataSource : UITableViewSource
    {
        SelectNotificationViewController _selectNotificationVC;
        List<NotificationPreferenceModel> _data = new List<NotificationPreferenceModel>();

        public SelectNotificationDataSource(SelectNotificationViewController controller, List<NotificationPreferenceModel> data)
        {
            _selectNotificationVC = controller;
            _data = data;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _data.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {

            var cell = tableView.DequeueReusableCell("selectNotificationCell", indexPath);
            cell.TextLabel.Text = _data[indexPath.Row].Title;
            cell.TextLabel.TextColor = myTNBColor.TunaGrey();
            cell.TextLabel.Font = myTNBFont.MuseoSans16();

            if (indexPath.Row == DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex)
            {
                cell.Accessory = UITableViewCellAccessory.None;
                cell.AccessoryView = new UIView(new CGRect(0, 0, 24, 24));
                UIImageView imgViewTick = new UIImageView(new CGRect(0, 0, 24, 24));
                imgViewTick.Image = UIImage.FromBundle("Table-Tick");
                cell.AccessoryView.AddSubview(imgViewTick);
            }

            UIView viewLine = new UIView(new CGRect(0, cell.Frame.Height - 1, cell.Frame.Width, 1));
            viewLine.BackgroundColor = myTNBColor.PlatinumGrey();
            cell.AddSubview(viewLine);

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedNotificationTypeIndex = indexPath.Row;
            _selectNotificationVC.DismissVC();
        }
    }
}