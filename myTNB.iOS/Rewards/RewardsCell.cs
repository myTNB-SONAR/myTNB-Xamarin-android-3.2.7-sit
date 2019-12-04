using System;
using CoreGraphics;
using myTNB.Home.Components;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class RewardsCell : CustomUITableViewCell
    {
        private UIView _viewContainer, _readIndicator;
        public UIImageView RewardImageVIew;
        private UILabel _title;
        public UIImageView SaveIcon;
        public ActivityIndicatorComponent ActivityIndicator;

        public RewardsCell(IntPtr handle) : base(handle)
        {
            nfloat topPadding = GetScaledHeight(17F);
            _viewContainer = new UIView(new CGRect(BaseMarginWidth16, topPadding, _cellWidth - (BaseMarginWidth16 * 2), GetScaledHeight(160F)))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };
            _viewContainer.Layer.CornerRadius = GetScaledHeight(5F);
            _viewContainer.Layer.MasksToBounds = false;
            _viewContainer.Layer.ShadowColor = MyTNBColor.BabyBlue60.CGColor;
            _viewContainer.Layer.ShadowOpacity = 0.5F;
            _viewContainer.Layer.ShadowOffset = new CGSize(-4, 8);
            _viewContainer.Layer.ShadowRadius = 8;
            _viewContainer.Layer.ShadowPath = UIBezierPath.FromRect(_viewContainer.Bounds).CGPath;

            UIView rewardImgView = new UIImageView(_viewContainer.Bounds)
            {
                BackgroundColor = UIColor.Clear,
                ClipsToBounds = true
            };
            rewardImgView.Layer.CornerRadius = GetScaledHeight(5F);

            RewardImageVIew = new UIImageView(new CGRect(0, 0, _viewContainer.Frame.Width, GetScaledHeight(112F)))
            {
                ClipsToBounds = true,
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            ActivityIndicator = new ActivityIndicatorComponent(RewardImageVIew);
            rewardImgView.AddSubview(RewardImageVIew);
            _viewContainer.AddSubview(rewardImgView);
            _title = new UILabel(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(RewardImageVIew.Frame, 16F)
                , _viewContainer.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _viewContainer.AddSubview(_title);
            nfloat iconWidth = GetScaledWidth(24F);
            nfloat iconHeight = GetScaledHeight(21F);
            nfloat iconWidthPadding = GetScaledWidth(8F);
            nfloat iconHeightPadding = GetScaledHeight(8F);
            SaveIcon = new UIImageView(new CGRect(RewardImageVIew.Frame.Width - iconWidth - iconWidthPadding
                , RewardImageVIew.Frame.Height - iconHeight - iconHeightPadding, iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(RewardsConstants.Img_HeartUnsaveIcon),
                UserInteractionEnabled = true
            };
            nfloat dotWidth = GetScaledWidth(8F);
            nfloat dotHeight = GetScaledHeight(8F);
            _readIndicator = new UIView(new CGRect(_viewContainer.Frame.Width - dotWidth - GetScaledWidth(12F)
                , GetScaledHeight(133F), dotWidth, dotHeight))
            {
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _readIndicator.Layer.CornerRadius = GetScaledHeight(4F);
            _viewContainer.AddSubview(_readIndicator);

            _viewContainer.AddSubview(SaveIcon);
            AddSubview(_viewContainer);
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void SetAccountCell(RewardsModel model)
        {
            if (model != null)
            {
                _title.Text = model.TitleOnListing;
                _readIndicator.Hidden = model.IsRead;
                if (!model.IsRead)
                {
                    ViewHelper.AdjustFrameSetWidth(_title, _viewContainer.Frame.Width - BaseMarginWidth16 - _readIndicator.Frame.Width - GetScaledWidth(18F) - GetScaledWidth(12F));
                }
            }
        }
    }
}