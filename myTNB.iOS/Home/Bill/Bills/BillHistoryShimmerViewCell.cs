using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Bill
{
    public class BillHistoryShimmerViewCell : UITableViewCell
    {
        private CustomUIView _view;
        private nfloat _cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private nfloat _baseHMargin = ScaleUtility.GetScaledWidth(16);

        public BillHistoryShimmerViewCell(IntPtr handle) : base(handle)
        {
            _view = new CustomUIView(new CGRect(0, 0, _cellWidth, ScaleUtility.GetScaledHeight(100))) { ClipsToBounds = false };
            AddShimmerView();
            AddSubview(_view);
            if (_view != null)
            {
                _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
                _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
                _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
                _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            }
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        private void AddShimmerView()
        {
            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewParent = new UIView(new CGRect(new CGPoint(0, 0), _view.Frame.Size)) { BackgroundColor = UIColor.White };
            UIView viewShimmerParent = new UIView(new CGRect(new CGPoint(0, 0), _view.Frame.Size)) { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(new CGPoint(0, 0), _view.Frame.Size)) { BackgroundColor = UIColor.Clear };
            viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

            UIView viewGroupedData = new UIView(new CGRect(_baseHMargin, 0
                  , ScaleUtility.GetScaledWidth(80), ScaleUtility.GetScaledHeight(12)))
            {
                BackgroundColor = MyTNBColor.PaleGrey,
            };

            UIView viewDate = new UIView(new CGRect(_baseHMargin, ScaleUtility.GetScaledHeight(28)
                  , ScaleUtility.GetScaledWidth(120), ScaleUtility.GetScaledHeight(16)))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };

            UIView viewSource = new UIView(new CGRect(_baseHMargin, ScaleUtility.GetYLocationFromFrame(viewDate.Frame, 8)
                  , ScaleUtility.GetScaledWidth(80), ScaleUtility.GetScaledHeight(14)))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };

            UIView viewAmount = new UIView(new CGRect(_cellWidth - ScaleUtility.GetScaledWidth(128), ScaleUtility.GetScaledHeight(28)
                  , ScaleUtility.GetScaledWidth(80), ScaleUtility.GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };

            UIImageView imgArrow = new UIImageView(new CGRect(_cellWidth - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(32)
                , _baseHMargin, _baseHMargin))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_ArrowExpand)
            };

            UIView viewLine = new UIView(new CGRect(_baseHMargin, _view.Frame.Height - ScaleUtility.GetScaledHeight(21)
              , _cellWidth - (_baseHMargin * 2), ScaleUtility.GetScaledHeight(1)))
            { BackgroundColor = MyTNBColor.VeryLightPinkThree };

            viewShimmerContent.AddSubviews(new UIView[] { viewGroupedData, viewDate, viewSource, viewAmount, imgArrow, viewLine });

            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();

            _view.AddSubview(viewParent);
        }
    }
}
