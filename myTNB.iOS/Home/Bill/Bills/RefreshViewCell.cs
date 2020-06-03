using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class RefreshViewCell : UITableViewCell
    {
        public UIView _view;
        private nfloat _cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;

        public RefreshViewCell(IntPtr handle) : base(handle)
        {
            _view = new UIView(new CGRect(0, 0, _cellWidth, 0));

            AddSubview(_view);

            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void Rescale()
        {
            if (_view != null)
            {
                _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
                _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
                _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
                _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            }
        }
    }
}
