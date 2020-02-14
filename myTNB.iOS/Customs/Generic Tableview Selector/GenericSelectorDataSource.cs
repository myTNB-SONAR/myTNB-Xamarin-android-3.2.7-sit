using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class GenericSelectorDataSource : UITableViewSource
    {
        private readonly GenericSelectorViewController _controller;

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
            GenericSelectorCell cell = tableView.DequeueReusableCell("genericSelectorTableViewCell") as GenericSelectorCell;
            string text = _controller.Items[indexPath.Row] ?? string.Empty;
            cell.SetText(text);

            if (_controller.SelectedIndex > -1 && indexPath.Row == _controller.SelectedIndex)
            {
                cell.Accessory = UITableViewCellAccessory.None;
                cell.AccessoryView = new UIView(new CGRect(0, 0, cell.ImgViewTick.Frame.Width, cell.ImgViewTick.Frame.Height));
                cell.AccessoryView.AddSubview(cell.ImgViewTick);
            }
            else
            {
                RemoveAccessory(cell);
            }
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (_controller?.OnSelect != null && !_controller.HasCTA)
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
            else
            {
                _controller.SelectedIndex = indexPath.Row;
                int count = _controller.Items != null ? _controller.Items.Count : 0;
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        RemoveAccessory(tableView.CellAt(NSIndexPath.Create(0, i)));
                    }
                    catch (Exception e) { Debug.WriteLine("Error in RemoveAccessory: " + e.Message); }
                }
                UITableViewCell cell = tableView.CellAt(indexPath);
                if (_controller.SelectedIndex > -1 && indexPath.Row == _controller.SelectedIndex)
                {
                    nfloat accWidth = ScaleUtility.GetScaledWidth(24);
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
                    RemoveAccessory(cell);
                }
                _controller.SetCTAState();
            }
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            string text = _controller.Items[indexPath.Row] ?? string.Empty;
            UILabel lblTitle = new UILabel
            {
                Text = text,
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.CharcoalGrey,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                BackgroundColor = UIColor.Clear,
                Frame = new CGRect(0, 0, tableView.Frame.Width - ScaleUtility.GetScaledWidth(62), 1000)
            };
            nfloat newLblSize = lblTitle.GetLabelHeight(1000);
            return ScaleUtility.GetScaledHeight(32) + newLblSize;
        }

        private void RemoveAccessory(UITableViewCell cell)
        {
            if (cell != null && cell.AccessoryView != null && cell.AccessoryView.Subviews != null)
            {
                foreach (var subView in cell.AccessoryView.Subviews)
                {
                    subView.RemoveFromSuperview();
                }
            }
        }
    }
}