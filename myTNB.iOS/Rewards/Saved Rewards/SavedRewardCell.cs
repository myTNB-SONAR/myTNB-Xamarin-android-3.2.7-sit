using System;
using CoreGraphics;
using myTNB.Home.Components;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class SavedRewardCell : CustomUITableViewCell
    {
        private UIView _viewContainer, _imgLoadingView, _rewardImgView, _shadowView;
        public UIImageView RewardImageView;
        public UILabel Title, _usedLbl;
        public UIView UsedView;

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

            _rewardImgView = new UIImageView(_viewContainer.Bounds)
            {
                BackgroundColor = UIColor.Clear,
                ClipsToBounds = true
            };
            _rewardImgView.Layer.CornerRadius = GetScaledHeight(5F);

            RewardImageView = new UIImageView(new CGRect(0, 0, _viewContainer.Frame.Width, GetScaledHeight(112F)))
            {
                ClipsToBounds = true,
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            _rewardImgView.AddSubview(RewardImageView);
            _viewContainer.AddSubview(_rewardImgView);
            Title = new UILabel(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(RewardImageView.Frame, 16F), _viewContainer.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(16F)))
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

            _usedLbl = new UILabel(new CGRect(0, GetYLocationToCenterObject(GetScaledHeight(16F), UsedView), 0, GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White
            };

            UsedView.AddSubview(_usedLbl);
            _viewContainer.AddSubview(UsedView);

            AddSubview(_viewContainer);
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
            }
        }

        public void SetLoadingImageView()
        {
            if (_rewardImgView != null)
            {
                _rewardImgView.Hidden = true;
            }

            if (_shadowView != null)
            {
                _shadowView.Hidden = true;
            }

            if (_imgLoadingView != null)
            {
                _imgLoadingView.RemoveFromSuperview();
            }

            if (UsedView != null)
            {
                UsedView.Hidden = true;
            }

            _imgLoadingView = new UIView(_viewContainer.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            AddSubview(_imgLoadingView);

            UIView viewImage = new UIView(new CGRect(BaseMarginWidth16, GetScaledHeight(17F), _viewContainer.Frame.Width, GetScaledHeight(112F)))
            {
                BackgroundColor = MyTNBColor.PaleGreyThree
            };

            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewShimmerParent = new UIView(new CGRect(0, 0, _cellWidth, _cellHeight))
            { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(0, 0, _cellWidth, _cellHeight))
            { BackgroundColor = UIColor.Clear };
            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();

            viewShimmerContent.AddSubview(viewImage);
            _imgLoadingView.AddSubview(viewShimmerParent);
        }

        public void ShowDowloadedImage(RewardsModel model)
        {
            if (_rewardImgView != null)
            {
                _rewardImgView.Hidden = false;
            }

            if (_shadowView != null)
            {
                _shadowView.Hidden = false;
            }

            if (_imgLoadingView != null)
            {
                _imgLoadingView.RemoveFromSuperview();
            }

            if (model != null)
            {
                if (UsedView != null)
                {
                    UsedView.Hidden = !model.IsUsed;
                }
            }
        }
    }
}
