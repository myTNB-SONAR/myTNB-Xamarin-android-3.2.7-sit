using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class DashboardHomeDataSource : UITableViewSource
    {
        DashboardHomeViewController _controller;
        public DashboardHomeDataSource(DashboardHomeViewController controller)
        {
            _controller = controller;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 3;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == 0)
            {
                return 2;
            }
            else if (section == 1)
            {
                return 1;
            }
            else
            {
                return 1;
            }
        }
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            return GetViewForSectionHeader(tableView, (int)section);
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 36.0F;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 1)
            {
                ServicesTableViewCell cell = tableView.DequeueReusableCell("servicesTableViewCell") as ServicesTableViewCell;
                cell.AddCards();
                return cell;
            }
            if (indexPath.Section == 2)
            {
                HelpTableViewCell cell = tableView.DequeueReusableCell("helpTableViewCell") as HelpTableViewCell;
                cell.AddCards();
                return cell;
            }
            return new UITableViewCell() { BackgroundColor = UIColor.Clear };
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {

        }

        private string GetSectionTitle(int sectionIndex)
        {
            string key = "needHelp";
            if (sectionIndex == 0)
            {
                key = "myAccounts";
            }
            else if (sectionIndex == 1)
            {
                key = "myServices";
            }
            return _controller.I18NDictionary[key];
        }

        private UIView GetViewForSectionHeader(UITableView tableView, int sectionIndex)
        {
            UIView viewSection = new UIView(new CGRect(0, 0, tableView.Frame.Width, 36.0F)) { BackgroundColor = UIColor.Clear };
            UILabel lblTitle = new UILabel(new CGRect(16, 8, viewSection.Frame.Width, 20.0F))
            {
                TextColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans14_500,
                Text = GetSectionTitle(sectionIndex)
            };
            CGSize newSize = _controller.GetLabelSize(lblTitle, viewSection.Frame.Width / 2, 20.0F);
            lblTitle.Frame = new CGRect(lblTitle.Frame.X, lblTitle.Frame.Y, newSize.Width, lblTitle.Frame.Height);
            viewSection.AddSubview(lblTitle);
            return viewSection;
        }
    }
}