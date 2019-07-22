using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class AccountsTableViewCell : UITableViewCell
    {
        DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private UIView _contentView;
        public AccountsTableViewCell(IntPtr handle) : base(handle)
        {
            _contentView = new UIView(new CGRect(0, 0, cellWidth, _dashboardHomeHelper.GetHeightForAccountCards() + DashboardHomeConstants.SearchViewHeight + DashboardHomeConstants.PageControlHeight))
            {
                BackgroundColor = UIColor.Clear
            };
            AddSubview(_contentView);
            BackgroundColor = UIColor.Clear;
            _contentView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            _contentView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void AddViewsToContainers(UIViewController accountsCardViewController)
        {
            _contentView.AddSubview(accountsCardViewController.View);
        }

        public void UpdateCell(nfloat updatedHeight)
        {
            CGRect frame = _contentView.Frame;
            frame.Height = updatedHeight + DashboardHomeConstants.SearchViewHeight + DashboardHomeConstants.PageControlHeight;
            _contentView.Frame = frame;
        }
    }
}
