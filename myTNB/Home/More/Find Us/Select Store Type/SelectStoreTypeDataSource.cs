using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB.Home.More.FindUs.SelectStoreType
{
    public class SelectStoreTypeDataSource : UITableViewSource
    {
        SelectStoreTypeViewController _controller;
        List<LocationTypeDataModel> _storeTypeList = new List<LocationTypeDataModel>();
        public SelectStoreTypeDataSource(SelectStoreTypeViewController controller)
        {
            _controller = controller;
            if (DataManager.DataManager.SharedInstance.LocationTypes != null
               && DataManager.DataManager.SharedInstance.LocationTypes.d != null
               && DataManager.DataManager.SharedInstance.LocationTypes.d.data != null)
            {
                _storeTypeList = DataManager.DataManager.SharedInstance.LocationTypes.d.data;
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _storeTypeList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("selectStoreTypeViewCell", indexPath);
            cell.TextLabel.Text = _storeTypeList[indexPath.Row].Description;
            cell.TextLabel.TextColor = myTNBColor.TunaGrey();
            cell.TextLabel.Font = myTNBFont.MuseoSans16();
            if (indexPath.Row == DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex)
            {
                cell.Accessory = UITableViewCellAccessory.None;
                cell.AccessoryView = new UIView(new CGRect(0, 0, 24, 24));
                UIImageView imgViewTick = new UIImageView(new CGRect(0, 0, 24, 24));
                imgViewTick.Image = UIImage.FromBundle("Table-Tick");
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
            UIView viewLine = new UIView(new CGRect(0, cell.Frame.Height - 1, tableView.Frame.Width, 1));
            viewLine.BackgroundColor = myTNBColor.PlatinumGrey();
            cell.AddSubview(viewLine);

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex = indexPath.Row;
            if (DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex
                == DataManager.DataManager.SharedInstance.PreviousStoreTypeIndex)
            {
                DataManager.DataManager.SharedInstance.IsSameStoreType = true;
            }
            else
            {
                DataManager.DataManager.SharedInstance.PreviousStoreTypeIndex
                           = DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex;
                DataManager.DataManager.SharedInstance.IsSameStoreType = false;
            }
            if (DataManager.DataManager.SharedInstance.IsSameStoreType)
            {
                _controller.DismissViewController(true, null);
            }
            else
            {
                _controller.OnSelectStoreType();
            }
        }
    }
}