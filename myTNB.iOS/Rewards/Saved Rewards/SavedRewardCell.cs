using System;
using CoreGraphics;
using myTNB.Home.Components;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class SavedRewardCell : CustomUITableViewCell
    {
        private UIView _viewContainer;
        public UIImageView RewardImageVIew;
        public UILabel Title;
        public UIView UsedView;
        public ActivityIndicatorComponent ActivityIndicator;

        public SavedRewardCell(IntPtr handle) : base(handle)
        {
            nfloat topPadding = GetScaledHeight(17F);
            nfloat viewContainerWidth = _cellWidth - (BaseMarginWidth16 * 2);
            _viewContainer = new UIView(new CGRect(BaseMarginWidth16, topPadding, viewContainerWidth, GetScaledHeight(160F)))
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
            Title = new UILabel(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(RewardImageVIew.Frame, 16F), _viewContainer.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _viewContainer.AddSubview(Title);
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
                Text = "Used"
            };

            CGSize lblSize = usedLbl.SizeThatFits(new CGSize(viewContainerWidth, usedLbl.Frame.Height));
            ViewHelper.AdjustFrameSetWidth(UsedView, lblSize.Width + (GetScaledWidth(12F) * 2));
            ViewHelper.AdjustFrameSetWidth(usedLbl, lblSize.Width);
            ViewHelper.AdjustFrameSetX(usedLbl, GetXLocationToCenterObject(lblSize.Width, UsedView));
            UsedView.AddSubview(usedLbl);
            _viewContainer.AddSubview(UsedView);

            AddSubview(_viewContainer);
        }

        public void SetAccountCell(RewardsModel model)
        {
            if (model != null)
            {
                Title.Text = model.TitleOnListing;
            }
        }
    }
}
