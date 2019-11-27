using System;
using CoreGraphics;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class RewardsCell : CustomUITableViewCell
    {
        private UIView _viewContainer;
        public UIView ViewImage;
        public UIImageView RewardImageVIew;
        private UILabel Title;

        public RewardsCell(IntPtr handle) : base(handle)
        {
            nfloat topPadding = GetScaledHeight(17F);
            _viewContainer = new UIView(new CGRect(BaseMarginWidth16, topPadding, _cellWidth - (BaseMarginWidth16 * 2), GetScaledHeight(160F)))
            {
                BackgroundColor = UIColor.White
            };
            _viewContainer.Layer.CornerRadius = GetScaledHeight(5F);
            _viewContainer.Layer.MasksToBounds = false;
            _viewContainer.Layer.ShadowColor = MyTNBColor.BabyBlue60.CGColor;
            _viewContainer.Layer.ShadowOpacity = 0.5F;
            _viewContainer.Layer.ShadowOffset = new CGSize(-4, 8);
            _viewContainer.Layer.ShadowRadius = 8;
            _viewContainer.Layer.ShadowPath = UIBezierPath.FromRect(_viewContainer.Bounds).CGPath;
            ViewImage = new UIView(new CGRect(0, 0, _viewContainer.Frame.Width, GetScaledHeight(112F)))
            {
                BackgroundColor = UIColor.Clear
            };
            RewardImageVIew = new UIImageView(ViewImage.Bounds)
            {
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            Title = new UILabel(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(RewardImageVIew.Frame, 16F), _viewContainer.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _viewContainer.AddSubviews(new UIView { ViewImage, RewardImageVIew, Title });
            AddSubview(_viewContainer);
        }

        public void SetAccountCell(RewardsModel model)
        {
            if (model != null)
            {
                Title.Text = model.RewardName;
            }
            //if (CellIndex > -1 && CellIndex == 0)
            //{
            //    ViewHelper.AdjustFrameSetY(_viewContainer, GetScaledHeight(17F));
            //}
        }
    }
}
