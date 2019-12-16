using System;
using CoreAnimation;
using CoreGraphics;
using myTNB.Home.Components;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class RewardsCell : CustomUITableViewCell
    {
        private UIView _viewContainer, _readIndicator;
        public UIImageView RewardImageView;
        public UILabel Title, _usedLbl;
        public UIImageView SaveIcon;
        public UIView UsedView;
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

            RewardImageView = new UIImageView(new CGRect(0, 0, _viewContainer.Frame.Width, GetScaledHeight(112F)))
            {
                ClipsToBounds = true,
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            ActivityIndicator = new ActivityIndicatorComponent(RewardImageView);
            rewardImgView.AddSubview(RewardImageView);
            _viewContainer.AddSubview(rewardImgView);

            nfloat shadowViewHeight = GetScaledHeight(40F);
            UIView shadowView = new UIView(new CGRect(0, RewardImageView.Frame.Height - shadowViewHeight, _viewContainer.Frame.Width, shadowViewHeight))
            {
                BackgroundColor = UIColor.Clear
            };
            var topColor = MyTNBColor.Black0;
            var bottomColor = MyTNBColor.Black20;
            CAGradientLayer gradientLayer = new CAGradientLayer
            {
                Colors = new[] { topColor.CGColor, bottomColor.CGColor }
            };
            gradientLayer.StartPoint = new CGPoint(x: 0.5, y: 0.0);
            gradientLayer.EndPoint = new CGPoint(x: 0.5, y: 1.0);
            gradientLayer.Frame = shadowView.Bounds;
            gradientLayer.Opacity = 1f;
            shadowView.Layer.InsertSublayer(gradientLayer, 0);
            _viewContainer.AddSubview(shadowView);

            Title = new UILabel(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(RewardImageView.Frame, 16F)
                , _viewContainer.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _viewContainer.AddSubview(Title);
            nfloat iconWidth = GetScaledWidth(24F);
            nfloat iconHeight = GetScaledHeight(21F);
            nfloat iconWidthPadding = GetScaledWidth(8F);
            nfloat iconHeightPadding = GetScaledHeight(8F);
            SaveIcon = new UIImageView(new CGRect(RewardImageView.Frame.Width - iconWidth - iconWidthPadding
                , RewardImageView.Frame.Height - iconHeight - iconHeightPadding, iconWidth, iconHeight))
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

            nfloat usedWidth = GetScaledWidth(52F);
            nfloat usedHeight = GetScaledHeight(24F);
            UsedView = new UIView(new CGRect(_viewContainer.Frame.Width - usedWidth - GetScaledWidth(12F), GetScaledHeight(12F), usedWidth, usedHeight))
            {
                BackgroundColor = MyTNBColor.GreyishBrown,
                Hidden = true
            };
            UsedView.Layer.CornerRadius = GetScaledHeight(5F);

            _usedLbl = new UILabel(new CGRect(0, GetYLocationToCenterObject(GetScaledHeight(16F), UsedView), 0, GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White
            };

            UsedView.AddSubview(_usedLbl);
            _viewContainer.AddSubview(UsedView);

            AddSubview(_viewContainer);
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void SetAccountCell(RewardsModel model)
        {
            if (model != null)
            {
                _usedLbl.Text = GetI18NValue(RewardsConstants.I18N_Used);
                CGSize lblSize = _usedLbl.SizeThatFits(new CGSize(_cellWidth - (BaseMarginWidth16 * 2), _usedLbl.Frame.Height));
                ViewHelper.AdjustFrameSetWidth(UsedView, lblSize.Width + (GetScaledWidth(12F) * 2));
                ViewHelper.AdjustFrameSetWidth(_usedLbl, lblSize.Width);
                ViewHelper.AdjustFrameSetX(_usedLbl, GetXLocationToCenterObject(lblSize.Width, UsedView));
                ViewHelper.AdjustFrameSetX(UsedView, _viewContainer.Frame.Width - UsedView.Frame.Width - GetScaledWidth(12F));
                Title.Text = model.TitleOnListing;
                _readIndicator.Hidden = model.IsRead;
                if (!model.IsRead)
                {
                    ViewHelper.AdjustFrameSetWidth(Title, _viewContainer.Frame.Width - BaseMarginWidth16 - _readIndicator.Frame.Width - GetScaledWidth(18F) - GetScaledWidth(12F));
                }
            }
        }
    }
}