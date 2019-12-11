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
        public UIImageView RewardImageView;
        public UILabel Title;
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

            UILabel usedLbl = new UILabel(new CGRect(0, GetYLocationToCenterObject(GetScaledHeight(16F), UsedView), 0, GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                Text = LanguageUtility.GetCommonI18NValue(Constants.Common_Used)
            };

            CGSize lblSize = usedLbl.SizeThatFits(new CGSize(_cellWidth - (BaseMarginWidth16 * 2), usedLbl.Frame.Height));
            ViewHelper.AdjustFrameSetWidth(UsedView, lblSize.Width + (GetScaledWidth(12F) * 2));
            ViewHelper.AdjustFrameSetWidth(usedLbl, lblSize.Width);
            ViewHelper.AdjustFrameSetX(usedLbl, GetXLocationToCenterObject(lblSize.Width, UsedView));
            UsedView.AddSubview(usedLbl);
            _viewContainer.AddSubview(UsedView);

            AddSubview(_viewContainer);
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void SetAccountCell(RewardsModel model)
        {
            if (model != null)
            {
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