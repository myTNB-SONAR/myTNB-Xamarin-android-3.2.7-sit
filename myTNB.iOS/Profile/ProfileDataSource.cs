using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using Foundation;
using myTNB.SQLite.SQLiteDataManager;
using UIKit;

namespace myTNB.Profile
{
    public class ProfileDataSource : UITableViewSource
    {
        public Dictionary<string, List<string>> ProfileList { set; private get; } = new Dictionary<string, List<string>>();
        public Action<int, int> OnRowSelect { set; private get; }
        public List<string> ProfileLabels { set; private get; }
        public ProfileDataSource()
        {

        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 4;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            switch (section)
            {
                case 0: { return 7; }
                case 2: { return 5; }
                default: { return 2; }
            }
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            if (ProfileList == null || ProfileList.Count < 1 || section > ProfileList.Count - 1)
            {
                return new UIView();
            }
            List<string> keys = ProfileList.Keys.ToList();

            nfloat padding = ScaleUtility.GetScaledWidth(16);
            UIView sectionView = new UIView(new CGRect(0, 0, tableView.Bounds.Width, ScaleUtility.GetScaledHeight(44)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            UILabel lblTitle = new UILabel
            {
                Frame = new CGRect(padding, 0, sectionView.Frame.Width, sectionView.Frame.Height),
                Text = keys[(int)section],
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue
            };

            sectionView.AddSubviews(new UIView[] { lblTitle });
            return sectionView;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            List<string> keys = ProfileList.Keys.ToList();
            List<string> labelList = new List<string>();
            if (indexPath.Section > 0)
            {
                labelList = ProfileList[keys[indexPath.Section]];
                UITableViewCell cell = new UITableViewCell();
                cell.Frame = new CGRect(0, 0, tableView.Frame.Width, ScaleUtility.GetScaledHeight(52));
                cell.TextLabel.Frame = new CGRect(ScaleUtility.GetScaledWidth(16), ScaleUtility.GetScaledHeight(16)
                    , cell.Frame.Width - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(20));
                cell.TextLabel.Font = TNBFont.MuseoSans_14_500;
                cell.TextLabel.TextColor = MyTNBColor.CharcoalGrey;
                cell.TextLabel.Text = labelList[indexPath.Row];
                if (indexPath.Row < labelList.Count - 1)
                {
                    cell.AddSubview(new UIView(new CGRect(0, cell.Frame.GetMaxY() - ScaleUtility.GetScaledHeight(1)
                        , cell.Frame.Width, ScaleUtility.GetScaledHeight(1)))
                    { BackgroundColor = MyTNBColor.VeryLightPinkThree });
                }
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                return cell;
            }
            else
            {
                ProfileCell cell = tableView.DequeueReusableCell(ProfileConstants.Cell_Profile, indexPath) as ProfileCell;
                cell.Title = ProfileLabels[indexPath.Row];
                cell.IsLineHidden = indexPath.Row == 6;

                ProfileConfig(ref cell, indexPath.Row);
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                return cell;
            }
        }

        private void ProfileConfig(ref ProfileCell cell, int row)
        {
            UserEntity userInfo = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                   ? DataManager.DataManager.SharedInstance.UserEntity[0] : new UserEntity();
            string value = string.Empty;
            string action = string.Empty;
            bool isActionEnabled = true;
            switch (row)
            {
                case 0:
                    {
                        value = userInfo?.displayName ?? string.Empty;
                        break;
                    }
                case 1:
                    {
                        string icNo = userInfo?.identificationNo;
                        if (!string.IsNullOrEmpty(icNo) && icNo.Length > 4)
                        {
                            string lastDigit = icNo.Substring(icNo.Length - 4);
                            icNo = "•••••• •• " + lastDigit;
                        }
                        string maskedICNo = icNo;
                        value = maskedICNo;
                        break;
                    }
                case 2:
                    {
                        value = userInfo?.email ?? string.Empty;
                        break;
                    }
                case 3:
                    {
                        value = userInfo?.mobileNo ?? string.Empty;
                        action = LanguageUtility.GetCommonI18NValue(Constants.Common_Update);
                        break;
                    }
                case 4:
                    {
                        value = "••••••••••••••";
                        action = LanguageUtility.GetCommonI18NValue(Constants.Common_Update);
                        break;
                    }
                case 5:
                    {
                        int cardCount = DataManager.DataManager.SharedInstance.RegisteredCards?.d?.data?.Count ?? 0;
                        value = cardCount.ToString();
                        action = LanguageUtility.GetCommonI18NValue(Constants.Common_Manage);
                        isActionEnabled = cardCount > 0;
                        break;
                    }
                case 6:
                    {
                        int accountCount = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count ?? 0;
                        value = accountCount.ToString();
                        action = LanguageUtility.GetCommonI18NValue(Constants.Common_Manage);
                        isActionEnabled = accountCount > 0;
                        break;
                    }
            }
            cell.Value = value;
            cell.Action = action;
            cell.IsActionEnabled = isActionEnabled;
        }

        public override nfloat EstimatedHeightForHeader(UITableView tableView, nint section)
        {
            return ScaleUtility.GetScaledHeight(44);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
            {
                return ScaleUtility.GetScaledHeight(65);
            }
            return ScaleUtility.GetScaledHeight(52);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            Debug.WriteLine("RowSelected: " + indexPath.Section + " " + indexPath.Row);
            bool isActionEnabled = true;
            if (indexPath.Section == 0 && indexPath.Row > 4)
            {
                ProfileCell cell = tableView.CellAt(indexPath) as ProfileCell;
                isActionEnabled = cell.IsActionEnabled;
            }
            if (OnRowSelect != null && isActionEnabled)
            {
                OnRowSelect.Invoke(indexPath.Section, indexPath.Row);
            }
        }
    }
}