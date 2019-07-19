using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class AccountsTableViewCell : UITableViewCell
    {
        DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private UIView _headerViewContainer, _pageViewContainer;
        public AccountsTableViewCell(IntPtr handle) : base(handle)
        {
            nfloat headerHeight = 110f;
            _headerViewContainer = new UIView(new CGRect(0, 0, cellWidth, headerHeight))
            {
                BackgroundColor = UIColor.Clear
            };
            _pageViewContainer = new UIView(new CGRect(0, _headerViewContainer.Frame.GetMaxY(), cellWidth, _dashboardHomeHelper.GetHeightForAccountCards()))
            {
                BackgroundColor = UIColor.Clear
            };
            AddSubview(_headerViewContainer);
            AddSubview(_pageViewContainer);
            BackgroundColor = UIColor.Clear;
            _pageViewContainer.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            _pageViewContainer.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            _pageViewContainer.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            _pageViewContainer.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void AddViewsToContainers(UIPageViewController pageViewController, UIView headerView)
        {
            _headerViewContainer.AddSubview(headerView);
            _pageViewContainer.AddSubview(pageViewController.View);
        }

        public void UpdateCell(nfloat updatedHeight)
        {
            CGRect frame = _pageViewContainer.Frame;
            frame.Height = updatedHeight;
            _pageViewContainer.Frame = frame;
        }
    }
}
