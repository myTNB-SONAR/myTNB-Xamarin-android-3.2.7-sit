using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Home.Bill
{
    public class BillHistorySource : UITableViewSource
    {
        public EventHandler OnTableViewScroll;

        public BillHistorySource()
        {
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 10;
        }

        /*public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            nfloat scaled16 = ScaleUtility.GetScaledWidth(16);
            UIView sectionView = new UIView(new CGRect(0, 0, tableView.Frame.Width, ScaleUtility.GetScaledHeight(60)));
            sectionView.BackgroundColor = MyTNBColor.LightGrayBG;
            UILabel lblTitle = new UILabel(new CGRect(scaled16, ScaleUtility.GetScaledHeight(16), sectionView.Frame.Width, ScaleUtility.GetScaledHeight(24)))
            {
                Text = "My History",
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue
            };

            UIView viewFilter = new UIView(new CGRect(tableView.Frame.Width - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(20)
                , scaled16, scaled16));
            UIImageView imgFilter = new UIImageView(new CGRect(0, 0, scaled16, scaled16))
            {
                Image = UIImage.FromBundle("IC-Action-Filter")
            };
            viewFilter.AddGestureRecognizer(new UITapGestureRecognizer(()=> {
                Debug.WriteLine("Filter");
            }));
            viewFilter.AddSubview(imgFilter);

            sectionView.AddSubview(lblTitle);
            sectionView.AddSubview(viewFilter);
            return sectionView;
        }*/
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                BillSectionViewCell cell = tableView.DequeueReusableCell(BillConstants.Cell_BillSection) as BillSectionViewCell;

                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                return cell;
            }
            else
            {
                BillHistoryViewCell cell = tableView.DequeueReusableCell(BillConstants.Cell_BillHistory) as BillHistoryViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.ClipsToBounds = false;
                //cell.Layer.BorderColor = UIColor.Green.CGColor;
                //cell.Layer.BorderWidth = 1;

            cell.Layer.ZPosition = 10 + indexPath.Row;
                return cell;
            }
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return ScaleUtility.GetScaledHeight(60);
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            OnTableViewScroll?.Invoke(scrollView, null);
        }
    }
}
