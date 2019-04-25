using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class GenericSelectorDataSource : UITableViewSource
    {
        readonly GenericSelectorViewController _controller;

        public GenericSelectorDataSource(GenericSelectorViewController controller)
        {
            _controller = controller;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return (nint)(_controller?.Items != null ? _controller.Items?.Count : 0);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell("genericViewCell", indexPath);
            cell.TextLabel.Text = _controller.Items[indexPath.Row];
            cell.TextLabel.TextColor = MyTNBColor.TunaGrey();
            cell.TextLabel.Font = MyTNBFont.MuseoSans16;
            if (_controller.SelectedIndex > -1 && indexPath.Row == _controller.SelectedIndex)
            {
                cell.Accessory = UITableViewCellAccessory.None;
                cell.AccessoryView = new UIView(new CGRect(0, 0, 24, 24));
                UIImageView imgViewTick = new UIImageView(new CGRect(0, 0, 24, 24))
                {
                    Image = UIImage.FromBundle("Table-Tick")
                };
                cell.AccessoryView.AddSubview(imgViewTick);
            }
            else
            {
                if (cell != null && cell.AccessoryView != null && cell.AccessoryView.Subviews != null)
                {
                    foreach (var subView in cell.AccessoryView.Subviews)
                    {
                        subView.RemoveFromSuperview();
                    }
                }
            }
            UIView viewLine = new UIView(new CGRect(0, cell.Frame.Height - 1, tableView.Frame.Width, 1))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };
            cell.AddSubview(viewLine);
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (_controller?.OnSelect != null)
            {
                _controller?.OnSelect(indexPath.Row);
                _controller?.DismissViewController(true, null);
            }
        }
    }
}