using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class SSMRReadingHistoryCell : UITableViewCell
    {
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        nfloat cellHeight = 67;
        nfloat padding = 16f;
        UIView _containerView;
        UILabel _dateLabel, descLabel, _kwhLabel, monthYearLabel;
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
                TextColor = MyTNBColor.CharcoalGrey,
                Text = "08 Aug 2019"
            };
            descLabel = new UILabel(new CGRect(padding, _dateLabel.Frame.GetMaxY() + 4f, cellWidth / 2, 14f))
            {
                Font = MyTNBFont.MuseoSans10_300,
                TextColor = MyTNBColor.Grey,
                Text = "via Self Reading"
            };
            _containerView.AddSubviews(new UIView { _dateLabel, descLabel, line });
            AddSubview(_containerView);
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }
    }
}
