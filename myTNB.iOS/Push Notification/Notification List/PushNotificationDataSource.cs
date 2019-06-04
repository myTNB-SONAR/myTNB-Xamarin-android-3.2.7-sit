using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    ActivityIndicator.Show();
                    _controller.DeleteUserNotification(_data[indexPath.Row]).ContinueWith(task =>
                    {
                        InvokeOnMainThread(() =>
                        {
                            var deleteNotifResponse = _controller._deleteNotificationResponse;

                            if (deleteNotifResponse != null && deleteNotifResponse?.d != null
                                            && deleteNotifResponse?.d?.status?.ToLower() == "success"
                                            && deleteNotifResponse?.d?.didSucceed == true)
                            {
                                _data.RemoveAt(indexPath.Row);
                                tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                            }

                            ActivityIndicator.Hide();
                        });
                    });
                    break;
                case UITableViewCellEditingStyle.None:
                    break;
            }
        }
        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true; // return false if you wish to disable editing for a specific indexPath or for all rows
        }
        public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
        {   // Optional - default text is 'Delete'
            return "Delete";
        }

        void DeleteNotification(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            /* ActivityIndicator.Show();
             _controller.DeleteUserNotification(_data[indexPath.Row]).ContinueWith(task =>
             {
                 InvokeOnMainThread(() =>
                 {
                     var deleteNotifResponse = _controller._deleteNotificationResponse;

                     if (deleteNotifResponse != null && deleteNotifResponse?.d != null
                                     && deleteNotifResponse?.d?.status?.ToLower() == "success"
                                     && deleteNotifResponse?.d?.didSucceed == true)
                     {
                         _data.RemoveAt(indexPath.Row);
                         tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                     }

                     ActivityIndicator.Hide();
                 });
             });*/
        }

        public override UISwipeActionsConfiguration GetLeadingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
        {
            UserNotificationDataModel notification = _data[indexPath.Row];
            if (notification.IsRead.ToLower() != "false")
            {
                return null;
            }
#pragma warning disable XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
            UIContextualAction contextualAction = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal
                , string.Empty
                , (action, sourceView, completionHandler) =>
                {
                    //Todo: Call service
                });
            contextualAction.Image = UIImage.FromBundle("Notification-MarkAsRead");
            contextualAction.BackgroundColor = UIColor.Blue;
            UISwipeActionsConfiguration leadingSwipe = UISwipeActionsConfiguration.FromActions(new UIContextualAction[] { contextualAction });
            leadingSwipe.PerformsFirstActionWithFullSwipe = true;
            return leadingSwipe;
#pragma warning restore XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
        }

        public override UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
        {
#pragma warning disable XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
            UIContextualAction contextualAction = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal
                , string.Empty
                , (action, sourceView, completionHandler) =>
                {
                    DeleteNotification(tableView, indexPath);
                });
            contextualAction.Image = UIImage.FromBundle("Notification-Delete");
            contextualAction.BackgroundColor = UIColor.Red;
            UISwipeActionsConfiguration trailingSwipe = UISwipeActionsConfiguration.FromActions(new UIContextualAction[] { contextualAction });
            trailingSwipe.PerformsFirstActionWithFullSwipe = true;
            return trailingSwipe;
#pragma warning restore XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
        }

    }
}