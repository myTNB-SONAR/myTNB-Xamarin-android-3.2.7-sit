using System;
using CoreGraphics;
using Foundation;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public class SSMRReadingHistoryDataSource : UITableViewSource
    {
        private SSMRReadingHistoryViewController _controller;

        public SSMRReadingHistoryDataSource(SSMRReadingHistoryViewController controller)
        {
            _controller = controller;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            SSMRReadingHistoryCell cell = tableView.DequeueReusableCell(SSMRConstants.Cell_ReadingHistory) as SSMRReadingHistoryCell;
            return cell;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 20;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 67f;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 48.0f;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            nfloat padding = 13f;
            UIView sectionView = new UIView(new CGRect(0, 0, tableView.Bounds.Width, 48.0f));
            sectionView.BackgroundColor = MyTNBColor.LightGrayBG;
            UILabel lblTitle = new UILabel
            {
                Frame = new CGRect(padding, 0, sectionView.Frame.Width, sectionView.Frame.Height),
                Text = "My Meter Reading History",
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.WaterBlue
            };

            sectionView.AddSubviews(new UIView[] { lblTitle });
            return sectionView;
        }
    }
}
