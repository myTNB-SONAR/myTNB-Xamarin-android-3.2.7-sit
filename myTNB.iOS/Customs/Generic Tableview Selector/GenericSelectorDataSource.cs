using System;
using CoreGraphics;
using Foundation;
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

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (_controller.HasSectionTitle
                && !string.IsNullOrEmpty(_controller.SectionTitle)
                && !string.IsNullOrWhiteSpace(_controller.SectionTitle))
            {
                return ScaleUtility.GetScaledHeight(44);
            }
            return 0;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView view = new UIView();
            if (_controller.HasSectionTitle
                && !string.IsNullOrEmpty(_controller.SectionTitle)
                && !string.IsNullOrWhiteSpace(_controller.SectionTitle))
            {
                view = new UIView(new CGRect(0, 0, tableView.Frame.Width, ScaleUtility.GetScaledHeight(44)))
                {
                    BackgroundColor = MyTNBColor.SectionGrey
                };

                UILabel lblSectionTitle = new UILabel(new CGRect(ScaleUtility.GetScaledWidth(16)
                    , ScaleUtility.GetScaledHeight(16), tableView.Frame.Width, ScaleUtility.GetScaledHeight(20)))
                {
                    Text = _controller.SectionTitle ?? string.Empty,
                    Font = TNBFont.MuseoSans_16_500,
                    TextColor = MyTNBColor.WaterBlue
                };
                view.Add(lblSectionTitle);
            }
            return view;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return (nint)(_controller?.Items != null ? _controller.Items?.Count : 0);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell("genericViewCell", indexPath);
            string text = _controller.Items[indexPath.Row] ?? string.Empty;
            cell.TextLabel.Text = text;
            cell.TextLabel.Font = TNBFont.MuseoSans_16_500;
            cell.TextLabel.TextColor = MyTNBColor.CharcoalGrey;
            cell.TextLabel.Lines = 0;
            cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
            cell.TextLabel.BackgroundColor = UIColor.Clear;
            nfloat accWidth = ScaleUtility.GetScaledWidth(24);
            if (_controller.SelectedIndex > -1 && indexPath.Row == _controller.SelectedIndex)
            {
                cell.Accessory = UITableViewCellAccessory.None;
                cell.AccessoryView = new UIView(new CGRect(0, 0, accWidth, accWidth));
                UIImageView imgViewTick = new UIImageView(new CGRect(0, 0, accWidth, accWidth))
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
            UIView viewLine = new UIView(new CGRect(0, cell.Frame.Height - ScaleUtility.GetScaledHeight(1)
                , tableView.Frame.Width, ScaleUtility.GetScaledHeight(1)))
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
                if (_controller.IsRootPage && _controller.NavigationController != null)
                {
                    _controller.NavigationController.PopViewController(true);
                }
                else
                {
                    _controller?.DismissViewController(true, null);
                }
            }
        }
    }
}