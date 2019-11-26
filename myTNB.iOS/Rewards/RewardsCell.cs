using System;
using CoreGraphics;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class RewardsCell : CustomUITableViewCell
    {
        private UIView _viewContainer;
        private UIImageView _imageView;
        private UILabel _title;

        public RewardsCell(IntPtr handle) : base(handle)
        {
            nfloat topPadding = GetScaledHeight(17F);
            _viewContainer = new UIView(new CGRect(BaseMarginWidth16, topPadding, _cellWidth, RewardsConstants.RewardsCellHeight - topPadding))
            {
                BackgroundColor = UIColor.White
            };
            _imageView = new UIImageView(new CGRect(0, 0, _viewContainer.Frame.Width, GetScaledHeight(112F)))
            {
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            _title = new UILabel(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(_imageView.Frame, 16F), _cellWidth - (BaseMarginWidth16 * 2), GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _viewContainer.AddSubviews(new UIView { _imageView, _title });
            AddSubview(_viewContainer);
        }

        public void SetAccountCell(RewardsChildModel model)
        {
            if (model != null)
            {
                _title.Text = model.RewardName;
            }
        }
    }
}
