using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class SSMRReadingHistoryCell : UITableViewCell
    {
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        nfloat cellHeight = ScaleUtility.GetScaledHeight(68);
        nfloat padding = ScaleUtility.GetScaledWidth(16);
        UIView _containerView;
        public UILabel _dateLabel, _descLabel, _kwhLabel, _monthYearLabel;
        public SSMRReadingHistoryCell(IntPtr handle) : base(handle)
        {
            _containerView = new UIView(new CGRect(0, 0, cellWidth, cellHeight))
            {
                BackgroundColor = UIColor.White
            };
            UIView line = new UIView(new CGRect(0, cellHeight - ScaleUtility.GetScaledHeight(1)
                , cellWidth, ScaleUtility.GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };
            _dateLabel = new UILabel(new CGRect(padding, padding, cellWidth / 2, ScaleUtility.GetScaledHeight(16)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.CharcoalGrey
            };
            _descLabel = new UILabel(new CGRect(padding, _dateLabel.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(4)
                , cellWidth / 2, ScaleUtility.GetScaledHeight(14)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.Grey
            };
            _kwhLabel = new UILabel(new CGRect(cellWidth - (cellWidth / 2) - padding
                , padding, cellWidth / 2, ScaleUtility.GetScaledHeight(16)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Right
            };
            _monthYearLabel = new UILabel(new CGRect(cellWidth - (cellWidth / 2) - padding
                , _kwhLabel.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(4), cellWidth / 2, ScaleUtility.GetScaledHeight(14)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.Grey,
                TextAlignment = UITextAlignment.Right
            };
            _containerView.AddSubviews(new UIView { _dateLabel, _descLabel, _kwhLabel, _monthYearLabel, line });
            AddSubview(_containerView);
            if (_containerView != null)
            {
                _containerView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
                _containerView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
                _containerView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
                _containerView.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            }
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void UpdateCell(bool flag)
        {
            _descLabel.TextColor = flag ? MyTNBColor.Tomato : MyTNBColor.Grey;
        }
    }
}