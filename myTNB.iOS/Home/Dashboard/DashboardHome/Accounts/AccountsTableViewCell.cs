using System;
using myTNB.Home.Components;
using UIKit;

namespace myTNB
{
    public class AccountsTableViewCell : UITableViewCell
    {
        public AccountsTableViewCell(IntPtr handle) : base(handle)
        {
            BackgroundColor = UIColor.Clear;
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void AddViewsToContainers(UIViewController accountListViewController)
        {
            if (accountListViewController != null)
            {
                ViewHelper.AdjustFrameSetY(accountListViewController.View, 0);
                AddSubview(accountListViewController.View);
            }
        }

        public void AddRefreshViewToContainer(RefreshScreenComponent refreshScreenComponent)
        {
            if (refreshScreenComponent != null)
            {
                AddSubview(refreshScreenComponent.GetView());
            }
        }
    }
}
