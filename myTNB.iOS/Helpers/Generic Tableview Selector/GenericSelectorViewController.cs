using System;
using System.Collections.Generic;
using UIKit;

namespace myTNB
{
    public partial class GenericSelectorViewController : UITableViewController
    {
        public Action OnSelect;
        public List<string> Items;

        public GenericSelectorViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            SetNavigationBar();
            genericTableView.Source = new GenericSelectorDataSource(this);
            genericTableView.ReloadData();
            genericTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            genericTableView.RowHeight = 56F;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle("Back-White")
                , UIBarButtonItemStyle.Done, (sender, e) =>
            {
                this.DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }
    }
}