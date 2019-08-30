using System;
using Airbnb.Lottie;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class SmartMeterOverlayComponent : BaseComponent
    {
        UIView _parentView, _contentView;
        CustomUIView _btnView;
        UILabel _title, _description;
        UIImageView _imageView;
        nfloat _yPos;
        public SmartMeterOverlayComponent(UIView parentView, nfloat yPos)
        {
            _parentView = parentView;
            _yPos = yPos;
        }

        private void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width;
            _contentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            ViewHelper.AdjustFrameSetY(_contentView, _yPos);
            _title = new UILabel(new CGRect(0, GetScaledHeight(148F), width, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Text = "How do I view my daily usage?"
            };
            _contentView.AddSubview(_title);
            //nfloat animationWidth = GetScaledWidth(76F);
            //nfloat animationHeight = GetScaledWidth(76F);
            //LOTAnimationView pinchAnimation = LOTAnimationView.AnimationNamed("DashboardZoom.json");
            //pinchAnimation.Frame = new CGRect(GetXLocationToCenterObject(animationWidth, _contentView), _title.Frame.GetMaxY() + GetScaledHeight(32F),
            //                             animationWidth, animationHeight);
            //pinchAnimation.LoopAnimation = true;             //pinchAnimation.Play();
            //_contentView.AddSubview(pinchAnimation);

            nfloat imageWidth = GetScaledWidth(76F);
            nfloat imageHeight = GetScaledWidth(76F);
            nfloat imageXPos = GetXLocationToCenterObject(imageWidth, _contentView);
            _imageView = new UIImageView(new CGRect(imageXPos, _title.Frame.GetMaxY() + GetScaledHeight(32F), imageWidth, imageHeight))
            {
                Image = UIImage.FromBundle("Pinch-Icon")
            };
            _contentView.AddSubview(_imageView);

            nfloat descWidth = width - (GetScaledWidth(34F) * 2);
            _description = new UILabel(new CGRect(GetScaledWidth(34F), _imageView.Frame.GetMaxY() + GetScaledHeight(24F), descWidth, 0))
            {
                Font = TNBFont.MuseoSans_14_300,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Text = "Pinch to zoom in or out! Zoom in to view your daily breakdown.",
                Lines = 0
            };
            _contentView.AddSubview(_description);
            CGSize descLabel = _description.SizeThatFits(new CGSize(descWidth, 1000f));
            ViewHelper.AdjustFrameSetHeight(_description, descLabel.Height);
            _btnView = new CustomUIView(new CGRect(BaseMarginWidth16, _description.Frame.GetMaxY() + BaseMarginHeight16, width - (BaseMarginWidth16 * 2), GetScaledHeight(48F)))
            {
                BackgroundColor = UIColor.White,
            };
            _btnView.Layer.CornerRadius = GetScaledHeight(4F);
            UILabel btnLabel = new UILabel(_btnView.Bounds)
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Center,
                Text = "Got It!"
            };
            _btnView.AddSubview(btnLabel);
            _contentView.AddSubview(_btnView);
        }

        public void SetGestureForButton(UITapGestureRecognizer recognizer)
        {
            if (_btnView != null)
            {
                _btnView.AddGestureRecognizer(recognizer);
            }
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _contentView;
        }
    }
}
