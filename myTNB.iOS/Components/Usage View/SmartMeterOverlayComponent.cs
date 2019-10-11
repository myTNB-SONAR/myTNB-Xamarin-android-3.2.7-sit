using System;
using Airbnb.Lottie;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class SmartMeterOverlayComponent : BaseComponent
    {
        private UIView _parentView, _contentView;
        private CustomUIView _btnView;
        private UILabel _title, _description;
        private nfloat _yPos;

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
            _title = new UILabel(new CGRect(0, _yPos, width, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Text = GetI18NValue(UsageConstants.I18N_SMOverlayTitle)
            };
            _contentView.AddSubview(_title);
            nfloat animationWidth = GetScaledWidth(76F);
            nfloat animationHeight = GetScaledWidth(76F);
            LOTAnimationView pinchAnimation = LOTAnimationView.AnimationNamed(UsageConstants.STR_PinchInOutFilename);
            pinchAnimation.Frame = new CGRect(GetXLocationToCenterObject(animationWidth, _contentView), _title.Frame.GetMaxY() + GetScaledHeight(32F),
                                         animationWidth, animationHeight);
            pinchAnimation.ContentMode = UIViewContentMode.ScaleAspectFill;
            pinchAnimation.LoopAnimation = true;             pinchAnimation.Play();
            _contentView.AddSubview(pinchAnimation);

            nfloat descWidth = width - (GetScaledWidth(34F) * 2);
            _description = new UILabel(new CGRect(GetScaledWidth(34F), pinchAnimation.Frame.GetMaxY() + GetScaledHeight(24F), descWidth, 0))
            {
                Font = TNBFont.MuseoSans_14_300,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Text = GetI18NValue(UsageConstants.I18N_SMOverlayMsg),
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
                Text = GetI18NValue(UsageConstants.I18N_SMOverlayBtnTxt)
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