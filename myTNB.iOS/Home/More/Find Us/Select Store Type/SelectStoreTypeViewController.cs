using System;
using UIKit;
using myTNB.Home.More.FindUs.SelectStoreType;

namespace myTNB
{
    public partial class SelectStoreTypeViewController : UITableViewController
    {
        public SelectStoreTypeViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            selectStoreTypeTableView.Source = new SelectStoreTypeDataSource(this);
            selectStoreTypeTableView.ReloadData();
            selectStoreTypeTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            selectStoreTypeTableView.RowHeight = 56F;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        internal void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.IsSameStoreType = true;
                this.DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal void OnSelectStoreType()
        {
            if (DataManager.DataManager.SharedInstance.LocationTypes != null
                && DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex < DataManager.DataManager.SharedInstance.LocationTypes?.Count)
            {
                DataManager.DataManager.SharedInstance.SelectedLocationTypeID
                           = DataManager.DataManager.SharedInstance.LocationTypes
                    [DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex].Id;
                DataManager.DataManager.SharedInstance.SelectedLocationTypeTitle
                           = DataManager.DataManager.SharedInstance.LocationTypes
                    [DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex].Title;
            }
            DataManager.DataManager.SharedInstance.isLocationSearch = false;
            DismissViewController(true, null);
        }
    }
}