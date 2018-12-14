using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Home.More
{
    public class MoreDataSource : UITableViewSource
    {
        MoreViewController _controller;
        Dictionary<string, List<string>> _data = new Dictionary<string, List<string>>();
        List<string> _keys = new List<string>();
        public MoreDataSource(MoreViewController controller, Dictionary<string, List<string>> data)
        {
            _controller = controller;
            _data = data;
            _keys = new List<string>(_data.Keys);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _keys.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _data[_keys[(int)section]].Count;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, 48));
            view.BackgroundColor = myTNBColor.SectionGrey();

            var lblSectionTitle = new UILabel(new CGRect(18, 16, tableView.Frame.Width, 18));
            lblSectionTitle.Text = _keys[(int)section];
            lblSectionTitle.Font = myTNBFont.MuseoSans16();
            lblSectionTitle.TextColor = myTNBColor.PowerBlue();
            view.Add(lblSectionTitle);

            return view;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("moreCell", indexPath);
            cell.Frame = new CGRect(cell.Frame.X, cell.Frame.Y, tableView.Frame.Width, 50);
            List<string> items = _data[_keys[(int)indexPath.Section]];
            cell.TextLabel.TextColor = myTNBColor.TunaGrey();
            cell.TextLabel.Frame = new CGRect(18, 16, cell.Frame.Width - 36, 18);
            cell.TextLabel.Font = myTNBFont.MuseoSans14_300();
            cell.TextLabel.Text = items[indexPath.Row];

            if (indexPath.Row < items.Count - 1)
            {
                UIView viewLine = new UIView(new CGRect(0, cell.Frame.Height - 1, cell.Frame.Width, 1));
                viewLine.BackgroundColor = myTNBColor.PlatinumGrey();
                cell.AddSubview(viewLine);
            }

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            _controller.RenderSettingsScreen(indexPath.Section, indexPath.Row);
        }
    }
}