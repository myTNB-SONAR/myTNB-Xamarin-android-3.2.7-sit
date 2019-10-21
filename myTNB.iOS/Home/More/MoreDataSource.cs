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
            UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, 48))
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };

            var lblSectionTitle = new UILabel(new CGRect(18, 16, tableView.Frame.Width, 18))
            {
                Text = _keys[(int)section],
                Font = MyTNBFont.MuseoSans16,
                TextColor = MyTNBColor.PowerBlue
            };
            view.Add(lblSectionTitle);

            return view;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("moreCell", indexPath);
            cell.Frame = new CGRect(cell.Frame.X, cell.Frame.Y, tableView.Frame.Width, 50);
            List<string> items = _data[_keys[(int)indexPath.Section]];
            cell.TextLabel.TextColor = MyTNBColor.TunaGrey();
            cell.TextLabel.Frame = new CGRect(18, 16, cell.Frame.Width - 36, 18);
            cell.TextLabel.Font = MyTNBFont.MuseoSans14_300;
            cell.TextLabel.Text = items[indexPath.Row];

            if (indexPath.Row < items.Count - 1)
            {
                UIView viewLine = new UIView(new CGRect(0, cell.Frame.Height - 1, cell.Frame.Width, 1))
                {
                    BackgroundColor = MyTNBColor.PlatinumGrey
                };
                cell.AddSubview(viewLine);
            }

            //Language
            /* if (indexPath.Section == 0 && indexPath.Row == 2)
             {
                 if (cell?.Subviews?.Length > 1)
                 {
                     cell?.Subviews[1]?.RemoveFromSuperview();
                 }
                 UILabel lang = new UILabel(new CGRect(cell.Frame.Width - 56, 0, 56, 56))
                 {
                     TextAlignment = UITextAlignment.Center,
                     Font = MyTNBFont.MuseoSans14_300,
                     TextColor = MyTNBColor.TunaGrey(),
                     Text = LanguageSettings.SupportedLanguageCode[LanguageSettings.SelectedLangugageIndex]
                 };
                 cell.AddSubview(lang);
             }*/

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            _controller.RenderSettingsScreen(indexPath.Section, indexPath.Row);
        }
    }
}