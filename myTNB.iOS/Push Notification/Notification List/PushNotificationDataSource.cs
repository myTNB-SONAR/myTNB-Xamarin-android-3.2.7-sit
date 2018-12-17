using System;
using System.Collections.Generic;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.PushNotification
{
    public class PushNotificationDataSource : UITableViewSource
    {
        PushNotificationViewController _controller;
        List<UserNotificationDataModel> _data = new List<UserNotificationDataModel>();
        public PushNotificationDataSource(PushNotificationViewController controller, List<UserNotificationDataModel> data)
        {
            _controller = controller;
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
            UserNotificationDataModel notification = _data[indexPath.Row];
            var cell = tableView.DequeueReusableCell("pushNotificationCell", indexPath) as NotificationViewCell;
            cell.ClearsContextBeforeDrawing = true;

            cell.imgIcon.Image = UIImage.FromBundle(GetIcon(notification.BCRMNotificationTypeId));
            cell.lblTitle.Text = notification.Title;
            cell.lblDetails.Text = notification.Message;
            cell.lblDate.Text = GetDate(notification.CreatedDate);
            cell.imgUnread.Hidden = notification.IsRead.ToLower() != "false";

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            _controller.ExecuteGetNotificationDetailedInfoCall(_data[indexPath.Row]);
        }

        internal string GetDate(string createdDate)
        {
            try
            {
                string date = createdDate.Split(' ')[0];
                return DateHelper.GetFormattedDate(date, "dd MMM yyyy", true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in date parsing: " + e.Message);
            }
            return string.Empty;
        }

        internal string GetIcon(string id)
        {
            if (id == "01")
            {
                return "Notification-New-Bill";
            }
            else if (id == "02")
            {
                return "Notification-Bill-Due";
            }
            else if (id == "03")
            {
                return "Notification-Dunning";
            }
            else if (id == "04")
            {
                return "Notification-Disconnection";
            }
            else if (id == "05")
            {
                return "Notification-Reconnection";
            }
            else if (id == "97")
            {
                return "Notification-Promotion";
            }
            else if (id == "98")
            {
                return "Notification-News";
            }
            else if (id == "99")
            {
                return "Notification-Maintenance";
            }
            else
            {
                return string.Empty;
            }

        }
    }
}