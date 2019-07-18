using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class AccountsTableViewCell : UITableViewCell
    {
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private UIView _view;
        public AccountsTableViewCell(IntPtr handle) : base(handle)
        {
            _view = new UIView(new CGRect(0, 0, cellWidth, 395.0F))
            {
                BackgroundColor = UIColor.Clear
            };
            AddSubview(_view);
            BackgroundColor = UIColor.Clear;
            _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void AddCards(UIPageViewController pageViewController)
        {
            _view.AddSubview(pageViewController.View);
        }
    }
}
