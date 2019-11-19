using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CoreGraphics;
using Foundation;
using myTNB.Enums;
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
            _data = data.FindAll(x => x.IsValidNotification);
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
            NotificationViewCell cell = tableView.DequeueReusableCell("pushNotificationCell") as NotificationViewCell;
            if (cell == null)
            {
                cell = new NotificationViewCell("pushNotificationCell");
            }
            cell.UpdateCell(_controller._isSelectionMode, notification.IsReadNotification);
            cell.ClearsContextBeforeDrawing = true;
            cell.imgIcon.Image = UIImage.FromBundle(GetIcon(notification.BCRMNotificationType));
            cell.lblTitle.Text = notification.Title;

            string message = notification.Message;
            int accountIndex = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindIndex(x => x.accNum == notification.AccountNum);
            if (accountIndex > -1)
            {
                string accountNickname = DataManager.DataManager.SharedInstance.AccountRecordsList.d[accountIndex].accountNickName ?? string.Empty;
                if (string.IsNullOrEmpty(accountNickname) || string.IsNullOrWhiteSpace(accountNickname))
                {
                    accountNickname = string.Format(_controller.GetCommonI18NValue(PushNotificationConstants.I18N_CustomerAccountNumber), notification.AccountNum);
                }
                message = Regex.Replace(message, PushNotificationConstants.REGEX_AccountNickname, accountNickname);
            }
            else
            {
                message = Regex.Replace(message, PushNotificationConstants.REGEX_AccountNickname
                    , string.Format(_controller.GetCommonI18NValue(PushNotificationConstants.I18N_CustomerAccountNumber), notification.AccountNum));
            }
            cell.lblDetails.Text = message;
            cell.lblDate.Text = GetDate(notification.CreatedDate);
            cell.IsRead = notification.IsReadNotification;
            cell.imgCheckbox.Image = UIImage.FromBundle(notification.IsSelected
                ? PushNotificationConstants.IMG_ChkActive : PushNotificationConstants.IMG_ChkInactive);
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (_controller._isSelectionMode)
            {
                UserNotificationDataModel notification = _data[indexPath.Row];
                notification.IsSelected = !notification.IsSelected;
                _controller.UpdateTitleRightIconImage(notification);
                NSIndexPath[] rowsToReload = {
                    indexPath
                };
                _controller.UpdateSectionHeaderWidget();
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

        private string GetIcon(BCRMNotificationEnum type)
        {
            if (PushNotificationConstants.IconDictionary.ContainsKey(type))
            {
                return PushNotificationConstants.IconDictionary[type];
            }
            return string.Empty;
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
                deleteAction.BackgroundColor = UIColor.FromPatternImage(RowActionImage(MyTNBColor.HarleyDavidsonOrange.CGColor, PushNotificationConstants.IMG_Delete));
                if (!notification.IsReadNotification)
                {
                    UITableViewRowAction readAction = UITableViewRowAction.Create(UITableViewRowActionStyle.Default, "        ", delegate
                    {
                        ReadNotification(indexPath);
                    });
                    readAction.BackgroundColor = UIColor.FromPatternImage(RowActionImage(MyTNBColor.Denim.CGColor, PushNotificationConstants.IMG_MarkAsRead));
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

        public override UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
        {
            if (_controller._isSelectionMode)
            {
                return null;
            }
#pragma warning disable XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
            UserNotificationDataModel notification = _data[indexPath.Row];
            UIContextualAction contextualReadAction = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal
             , string.Empty
             , (action, sourceView, completionHandler) =>
             {
                 ReadNotification(indexPath);
             });
            contextualReadAction.Image = UIImage.FromBundle(PushNotificationConstants.IMG_MarkAsRead);
            contextualReadAction.BackgroundColor = MyTNBColor.Denim;
            UIContextualAction contextualDeleteAction = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal
                , string.Empty
                , (action, sourceView, completionHandler) =>
                {
                    DeleteNotification(indexPath);
                });
            UIContextualAction[] contextualAction;
            if (notification.IsReadNotification)
            {
                contextualAction = new UIContextualAction[] { contextualDeleteAction };
            }
            else
            {
                contextualAction = new UIContextualAction[] { contextualDeleteAction, contextualReadAction };
            }
            contextualDeleteAction.Image = UIImage.FromBundle(PushNotificationConstants.IMG_Delete);
            contextualDeleteAction.BackgroundColor = MyTNBColor.HarleyDavidsonOrange;
            UISwipeActionsConfiguration trailingSwipe = UISwipeActionsConfiguration.FromActions(contextualAction);
            trailingSwipe.PerformsFirstActionWithFullSwipe = false;
            return trailingSwipe;
#pragma warning restore XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
        }
    }
}