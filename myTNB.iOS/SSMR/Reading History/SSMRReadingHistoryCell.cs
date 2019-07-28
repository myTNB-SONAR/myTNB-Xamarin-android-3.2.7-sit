using System;
using CoreGraphics;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class SSMRReadingHistoryCell : UITableViewCell
    {
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        nfloat cellHeight = 67;
        nfloat padding = 16f;
        UIView _containerView;
        public UILabel _dateLabel, _descLabel, _kwhLabel, _monthYearLabel;
        public SSMRReadingHistoryCell(IntPtr handle) : base(handle)
        {
            _containerView = new UIView(new CGRect(0, 0, cellWidth, cellHeight))
            {
                BackgroundColor = UIColor.White
            };
            UIView line = new UIView(new CGRect(0, cellHeight - 1f, cellWidth, 1))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };
            _dateLabel = new UILabel(new CGRect(padding, padding, cellWidth / 2, 16f))
            {
                Font = MyTNBFont.MuseoSans12_500,
                TextColor = MyTNBColor.CharcoalGrey
            };
            _descLabel = new UILabel(new CGRect(padding, _dateLabel.Frame.GetMaxY() + 4f, cellWidth / 2, 14f))
            {
                Font = MyTNBFont.MuseoSans10_300,
                TextColor = MyTNBColor.Grey
            };
            _kwhLabel = new UILabel(new CGRect(cellWidth - (cellWidth / 2) - padding, padding, cellWidth / 2, 16f))
            {
                Font = MyTNBFont.MuseoSans12_500,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Right
            };
            _monthYearLabel = new UILabel(new CGRect(cellWidth - (cellWidth / 2) - padding, _kwhLabel.Frame.GetMaxY() + 4f, cellWidth / 2, 14f))
            {
                Font = MyTNBFont.MuseoSans10_300,
                TextColor = MyTNBColor.Grey,
                TextAlignment = UITextAlignment.Right
            };
            _containerView.AddSubviews(new UIView { _dateLabel, _descLabel, _kwhLabel, _monthYearLabel, line });
            AddSubview(_containerView);
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }
    }
}
