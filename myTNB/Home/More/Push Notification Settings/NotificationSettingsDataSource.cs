using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Home.More.PushNotificationSettings
{
    public class NotificationSettingsDataSource : UITableViewSource
    {

        NotificationSettingsViewController _controller;
        List<string> _keys = new List<string>();
        public NotificationSettingsDataSource(NotificationSettingsViewController controller, List<string> keys)
        {
            _controller = controller;
            _keys = keys;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _keys.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == 0)
            {
                return DataManager.DataManager.SharedInstance.NotificationTypeResponse.d.data.Count;
            }
            else if (section == 1)
            {
                return DataManager.DataManager.SharedInstance.NotificationChannelResponse.d.data.Count;
            }
            else
            {
                return 0;
            }
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UITableViewCell cell = new UITableViewCell(UITableViewCellStyle.Default, "SectionCell");
            cell.Frame = new CGRect(cell.Frame.X, cell.Frame.Y, cell.Frame.Width, 66);
            UILabel lblTitle = new UILabel(new CGRect(18, 16, cell.Frame.Width - 36, 40));
            lblTitle.TextColor = myTNBColor.PowerBlue();
            lblTitle.Font = myTNBFont.MuseoSans16();
            lblTitle.Text = _keys[(int)section];
            lblTitle.Lines = 0;
            lblTitle.LineBreakMode = UILineBreakMode.WordWrap;
            cell.AddSubview(lblTitle);
            cell.BackgroundColor = myTNBColor.SectionGrey();
            return cell;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            List<NotificationPreferenceModel> items = new List<NotificationPreferenceModel>();
            bool isNotificationType = true;
            if (indexPath.Section == 0)
            {
                items = DataManager.DataManager.SharedInstance.NotificationTypeResponse.d.data;
            }
            else if (indexPath.Section == 1)
            {
                isNotificationType = false;
                items = DataManager.DataManager.SharedInstance.NotificationChannelResponse.d.data;
            }

            var cell = tableView.DequeueReusableCell("notificationSettingsCell", indexPath) as NotificationSettingsViewCell;
            cell.Frame = new CGRect(cell.Frame.X, cell.Frame.Y, tableView.Frame.Width, 54);

            cell.lblTitle.Text = items[indexPath.Row].Title;
            cell.Tag = items[indexPath.Row].IsOpted.ToLower() == "true" ? 1 : 0;
            bool isOpted = items[indexPath.Row].IsOpted.ToLower().Equals("true");
            cell.switchToggle.SetState(isOpted, true);
            cell.switchToggle.TouchUpInside += (sender, e) =>
            {
                SelectPreference(cell, isNotificationType, indexPath.Row);
            };
            cell.viewLine.Hidden = !(indexPath.Row < items.Count - 1);

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }

        internal void SelectPreference(UITableViewCell cell, bool isNotificationType, int index)
        {
            cell.Tag = cell.Tag == 0 ? 1 : 0;
            if (isNotificationType)
            {
                _controller.SelectedNotificationTypeList[index].IsOpted = cell.Tag == 1 ? "true" : "false";
                _controller.ExecuteSaveUserNotificationPreferenceCall(isNotificationType, _controller.SelectedNotificationTypeList[index]);
            }
            else
            {
                _controller.SelectedNotificationChannelList[index].IsOpted = cell.Tag == 1 ? "true" : "false";
                _controller.ExecuteSaveUserNotificationPreferenceCall(isNotificationType, _controller.SelectedNotificationChannelList[index]);
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {

        }
    }
}