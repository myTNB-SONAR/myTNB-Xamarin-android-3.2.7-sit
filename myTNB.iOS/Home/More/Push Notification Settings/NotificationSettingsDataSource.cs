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
        List<NotificationPreferenceModel> _preferenceItems = new List<NotificationPreferenceModel>();
        public NotificationSettingsDataSource(NotificationSettingsViewController controller, List<string> keys, List<NotificationPreferenceModel> preferenceItems)
        {
            _controller = controller;
            _keys = keys;
            if (preferenceItems != null)
            {
                _preferenceItems = preferenceItems;
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _keys.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == 0)
            {
                return _preferenceItems?.Count ?? 0;
            }
            else if (section == 1)
            {
                return DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d?.data?.Count ?? 0;
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
            UILabel lblTitle = new UILabel(new CGRect(18, 16, cell.Frame.Width - 36, 40))
            {
                TextColor = myTNBColor.PowerBlue(),
                Font = myTNBFont.MuseoSans16(),
                Text = _keys[(int)section],
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
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
                items = _preferenceItems;
            }
            else if (indexPath.Section == 1)
            {
                isNotificationType = false;
                items = DataManager.DataManager.SharedInstance.NotificationChannelResponse?.d?.data;
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
                if (_controller != null && index > -1)
                {
                    _controller.SelectedNotificationTypeList[index].IsOpted = cell.Tag == 1 ? "true" : "false";
                    _controller.ExecuteSaveUserNotificationPreferenceCall(isNotificationType, _controller.SelectedNotificationTypeList[index]);
                }
            }
            else
            {
                if (_controller != null && index > -1)
                {
                    _controller.SelectedNotificationChannelList[index].IsOpted = cell.Tag == 1 ? "true" : "false";
                    _controller.ExecuteSaveUserNotificationPreferenceCall(isNotificationType, _controller.SelectedNotificationChannelList[index]);
                }
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {

        }
    }
}