using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Bill
{
    public class BillHistoryViewCell : UITableViewCell
    {
        private UIView _view;
        private nfloat _cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private nfloat _baseHMargin = ScaleUtility.GetScaledWidth(16);
        private nfloat _baseVMargin = ScaleUtility.GetScaledHeight(16);

        public BillHistoryViewCell(IntPtr handle) : base(handle)
        {
            _view = new UIView(new CGRect(0, 0, _cellWidth, ScaleUtility.GetScaledHeight(68))) { ClipsToBounds = false };
            AddViews();
            AddSubview(_view);
            if (_view != null)
            {
                _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
                _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
                _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
                _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            }
        }

        private UIView _viewLine;
        private UILabel _lblDate, _lblSource, _lblAmount;
        private UIImageView _imgArrow;

        private void AddViews()
        {
            _lblDate = new UILabel(new CGRect(_baseHMargin, _baseVMargin, ScaleUtility.GetScaledWidth(200), _baseVMargin))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_500,
                Text = "26 Aug - Payment"
            };

            _lblSource = new UILabel(new CGRect(_baseHMargin, ScaleUtility.GetYLocationFromFrame(_lblDate.Frame, 4)
                , ScaleUtility.GetScaledWidth(200), _baseVMargin))
            {
                TextColor = MyTNBColor.Grey,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_300,
                Text = "23 Jul - 24 Aug 2019"
            };

            _lblAmount = new UILabel(new CGRect(_cellWidth - ScaleUtility.GetScaledWidth(148), ScaleUtility.GetScaledHeight(26)
                , ScaleUtility.GetScaledWidth(100), _baseVMargin))
            {
                TextColor = MyTNBColor.FreshGreen,
                TextAlignment = UITextAlignment.Right,
                Font = TNBFont.MuseoSans_12_500,
                Text = "RM 201.80"
            };

            _imgArrow = new UIImageView(new CGRect(_cellWidth - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(26)
                , _baseHMargin, _baseHMargin))
            {
                Image = UIImage.FromBundle("Arrow-Expand")
            };

            _viewLine = new UIView(new CGRect(_baseHMargin, _view.Frame.Height - ScaleUtility.GetScaledHeight(1)
               , _cellWidth - (_baseHMargin * 2), ScaleUtility.GetScaledHeight(1)))
            { BackgroundColor = MyTNBColor.VeryLightPinkThree };

            _view.AddSubviews(new UIView[] { _lblDate, _lblSource, _lblAmount, _imgArrow, _viewLine });


            UIView viewGroupedDate = new UIView(new CGRect(ScaleUtility.GetScaledWidth(16), 0 - ScaleUtility.GetScaledHeight(12)
                   , ScaleUtility.GetScaledWidth(70), ScaleUtility.GetScaledHeight(24)))
            {
                ClipsToBounds = true,
                Tag = 101
            };
            UILabel _lblGroupedDate = new UILabel(new CGRect(new CGPoint(0, 0), viewGroupedDate.Frame.Size))
            {
                BackgroundColor = MyTNBColor.WaterBlue,
                TextColor = UIColor.White,
                Font = TNBFont.MuseoSans_12_500,
                TextAlignment = UITextAlignment.Center,
                Text = "Aug 2019"
            };
            viewGroupedDate.AddSubview(_lblGroupedDate);
            viewGroupedDate.Layer.CornerRadius = ScaleUtility.GetScaledHeight(12);
            viewGroupedDate.Layer.ZPosition = 99;

            _view.AddSubview(viewGroupedDate);
        }
    }
}
