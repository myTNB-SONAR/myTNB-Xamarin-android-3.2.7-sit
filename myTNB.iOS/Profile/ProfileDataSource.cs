using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Profile
{
    public class ProfileDataSource : UITableViewSource
    {
        public Dictionary<string, List<string>> ProfileList { set; private get; } = new Dictionary<string, List<string>>();
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
                case 1:
                case 3:
                    { return 2; }
                case 2: { return 5; }
                default: { return 0; }
                    /* case 0: { return 7; }
                     case 2: { return 5; }
                     default: { return 2; }*/
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
            return new UITableViewCell();
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
        }
    }
}