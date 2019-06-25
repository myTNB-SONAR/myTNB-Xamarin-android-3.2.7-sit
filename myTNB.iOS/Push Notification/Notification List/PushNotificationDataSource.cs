using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
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

            var cell = tableView.DequeueReusableCell("pushNotificationCell") as NotificationViewCell;
            if (cell == null)
            {
                cell = new NotificationViewCell("pushNotificationCell");
            }
            cell.UpdateCell(_controller._isSelectionMode);
            cell.ClearsContextBeforeDrawing = true;
            cell.imgIcon.Image = UIImage.FromBundle(GetIcon(notification.BCRMNotificationTypeId));
            cell.lblTitle.Text = notification.Title;
            cell.lblDetails.Text = notification.Message;
            cell.lblDate.Text = GetDate(notification.CreatedDate);
            cell.imgUnread.Hidden = notification.IsRead.ToLower() != "false";
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.imgCheckbox.Image = UIImage.FromBundle(notification.IsSelected
                ? "Payment-Checkbox-Active" : "Payment-Checkbox-Inactive");
            cell.viewCheckBox.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                notification.IsSelected = !notification.IsSelected;
                _controller.UpdateTitleRightIconImage(notification);
                cell.imgCheckbox.Image = UIImage.FromBundle(notification.IsSelected
                    ? "Payment-Checkbox-Active" : "Payment-Checkbox-Inactive");

                _controller.UpdateSectionHeaderWidget();
            }));

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (_controller._isSelectionMode)
            {
                UserNotificationDataModel notification = _data[indexPath.Row];
                notification.IsSelected = !notification.IsSelected;
                _controller.UpdateTitleRightIconImage(notification);
                NSIndexPath[] rowsToReload = new NSIndexPath[] {
                    indexPath
                };
                tableView.ReloadRows(rowsToReload, UITableViewRowAnimation.None);
            }
            else
            {
                _controller.ExecuteGetNotificationDetailedInfoCall(_data[indexPath.Row]);
            }
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
                Debug.WriteLine("Error in date parsing: " + e.Message);
            }
            return string.Empty;
        }

        internal string GetIcon(string id)
        {
            switch (id)
            {
                case "01":
                    return "Notification-New-Bill";
                case "02":
                    return "Notification-Bill-Due";
                case "03":
                    return "Notification-Dunning";
                case "04":
                    return "Notification-Disconnection";
                case "05":
                    return "Notification-Reconnection";
                case "97":
                    return "Notification-Promotion";
                case "98":
                    return "Notification-News";
                case "99":
                    return "Notification-Maintenance";
                default:
                    return string.Empty;
            }
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return !_controller._isSelectionMode;
        }

        public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
        {
            UserNotificationDataModel notification = _data[indexPath.Row];
            if (!_controller._isSelectionMode)
            {
                UITableViewRowAction deleteAction = UITableViewRowAction.Create(UITableViewRowActionStyle.Default, "        ", delegate
                {
                    DeleteNotification(indexPath);
                });
                deleteAction.BackgroundColor = UIColor.FromPatternImage(RowActionImage(MyTNBColor.HarleyDavidsonOrange.CGColor, "Notification-Delete"));
                if (notification.IsRead.ToLower() == "false")
                {
                    UITableViewRowAction readAction = UITableViewRowAction.Create(UITableViewRowActionStyle.Default, "        ", delegate
                    {
                        //Todo: Read Notification
                    });
                    readAction.BackgroundColor = UIColor.FromPatternImage(RowActionImage(MyTNBColor.Denim.CGColor, "Notification-MarkAsRead"));
                    return new UITableViewRowAction[] { deleteAction, readAction };
                }
                else
                {
                    return new UITableViewRowAction[] { deleteAction };
                }
            }
            return null;
        }

        UIImage RowActionImage(CGColor bgColor, string imgKey)
        {
            CGRect frame = new CGRect(0, 0, 66, 66);
            UIGraphics.BeginImageContextWithOptions(new CGSize(66, 66), false, UIScreen.MainScreen.Scale);
            CGContext context = UIGraphics.GetCurrentContext();
            context.SetFillColor(bgColor);
            context.FillRect(frame);
            UIImage img = UIImage.FromBundle(imgKey);
            img.Draw(new CGRect((frame.Size.Width - 20) / 2, (frame.Size.Height - 20) / 2, 20, 20));
            UIImage newImg = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return newImg;
        }

        private void DeleteNotification(NSIndexPath indexPath)
        {
            List<UpdateNotificationModel> updateNotificationList = new List<UpdateNotificationModel>();
            updateNotificationList.Add(new UpdateNotificationModel()
            {
                NotificationType = _data[indexPath.Row]?.NotificationType,
                NotificationId = _data[indexPath.Row]?.Id
            });
            _controller.DeleteNotification(updateNotificationList, false, indexPath);
        }

        private void ReadNotification(NSIndexPath indexPath)
        {
            List<UpdateNotificationModel> updateNotificationList = new List<UpdateNotificationModel>();
            updateNotificationList.Add(new UpdateNotificationModel()
            {
                NotificationType = _data[indexPath.Row]?.NotificationType,
                NotificationId = _data[indexPath.Row]?.Id
            });
            _controller.ReadNotification(updateNotificationList, false, indexPath);
        }

        public override UISwipeActionsConfiguration GetLeadingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
        {
            UserNotificationDataModel notification = _data[indexPath.Row];
            if (notification.IsRead.ToLower() != "false" || _controller._isSelectionMode)
            {
                return null;
            }
#pragma warning disable XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
            UIContextualAction contextualAction = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal
                , string.Empty
                , (action, sourceView, completionHandler) =>
                {
                    ReadNotification(indexPath);
                });
            contextualAction.Image = UIImage.FromBundle("Notification-MarkAsRead");
            contextualAction.BackgroundColor = MyTNBColor.Denim;
            UISwipeActionsConfiguration leadingSwipe = UISwipeActionsConfiguration.FromActions(new UIContextualAction[] { contextualAction });
            leadingSwipe.PerformsFirstActionWithFullSwipe = true;
            return leadingSwipe;
#pragma warning restore XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
        }

        public override UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
        {
            if (_controller._isSelectionMode)
            {
                return null;
            }
#pragma warning disable XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
            UIContextualAction contextualAction = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal
                , string.Empty
                , (action, sourceView, completionHandler) =>
                {
                    DeleteNotification(indexPath);
                });
            contextualAction.Image = UIImage.FromBundle("Notification-Delete");
            contextualAction.BackgroundColor = MyTNBColor.HarleyDavidsonOrange;
            UISwipeActionsConfiguration trailingSwipe = UISwipeActionsConfiguration.FromActions(new UIContextualAction[] { contextualAction });
            trailingSwipe.PerformsFirstActionWithFullSwipe = true;
            return trailingSwipe;
#pragma warning restore XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
        }
    }
}