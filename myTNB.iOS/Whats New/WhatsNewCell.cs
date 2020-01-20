using System;
using CoreGraphics;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class WhatsNewCell : CustomUITableViewCell
    {
        private UIView _viewContainer, _readIndicator, _imgLoadingView, _imgView;
        public UIImageView BannerImageView;
        public UILabel Title, Date;

        public WhatsNewCell(IntPtr handle) : base(handle)
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

            _imgView = new UIView(_viewContainer.Bounds)
            {
                BackgroundColor = UIColor.Clear,
                ClipsToBounds = true
            };
            _imgView.Layer.CornerRadius = GetScaledHeight(5F);

            BannerImageView = new UIImageView(new CGRect(0, 0, _viewContainer.Frame.Width, GetScaledHeight(112F)))
            {
                ClipsToBounds = true,
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            _imgView.AddSubview(BannerImageView);
            _viewContainer.AddSubview(_imgView);

            Title = new UILabel(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(BannerImageView.Frame, 16F)
                , _viewContainer.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _viewContainer.AddSubview(Title);

            Date = new UILabel(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(BannerImageView.Frame, 16F)
                , _viewContainer.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_10_500,
                TextColor = MyTNBColor.CharcoalGrey,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _viewContainer.AddSubview(Date);

            nfloat dotWidth = GetScaledWidth(8F);
            nfloat dotHeight = GetScaledHeight(8F);
            _readIndicator = new UIView(new CGRect(_viewContainer.Frame.Width - dotWidth - GetScaledWidth(12F)
                , GetScaledHeight(133F), dotWidth, dotHeight))
            {
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _readIndicator.Layer.CornerRadius = GetScaledHeight(4F);
            _viewContainer.AddSubview(_readIndicator);

            AddSubview(_viewContainer);
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void SetAccountCell(WhatsNewModel model)
        {
            if (model != null)
            {
                Title.Text = model.TitleOnListing;
                Date.TextColor = model.IsRead ? MyTNBColor.WarmGrey : MyTNBColor.CharcoalGrey;
                Date.Font = model.IsRead ? TNBFont.MuseoSans_10_300 : TNBFont.MuseoSans_10_500;
                Date.Text = WhatsNewServices.GetPublishedDate(model.StartDate);
                CGSize dateSize = Date.SizeThatFits(new CGSize(_cellWidth, GetScaledHeight(16F)));
                ViewHelper.AdjustFrameSetWidth(Date, dateSize.Width);
                _readIndicator.Hidden = model.IsRead;
                if (!model.IsRead)
                {
                    ViewHelper.AdjustFrameSetX(Date, _readIndicator.Frame.GetMinX() - Date.Frame.Width - GetScaledWidth(8F));
                }
                else
                {
                    ViewHelper.AdjustFrameSetX(Date, _viewContainer.Frame.Width - Date.Frame.Width - BaseMarginWidth16);
                }
                ViewHelper.AdjustFrameSetWidth(Title, Date.Frame.GetMinX() - GetScaledWidth(8F) - BaseMarginWidth16);
            }
        }

        public void SetLoadingImageView()
        {
            if (_imgView != null)
            {
                _imgView.Hidden = true;
            }

            if (_imgLoadingView != null)
            {
                _imgLoadingView.RemoveFromSuperview();
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

        public void ShowDowloadedImage()
        {
            if (_imgView != null)
            {
                _imgView.Hidden = false;
            }

            if (_imgLoadingView != null)
            {
                _imgLoadingView.RemoveFromSuperview();
            }
        }
    }
}
