using System;
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

        public void AddViewsToContainers(UIViewController accountsCardViewController)
        {
            AddSubview(accountsCardViewController.View);
        }
    }
}
