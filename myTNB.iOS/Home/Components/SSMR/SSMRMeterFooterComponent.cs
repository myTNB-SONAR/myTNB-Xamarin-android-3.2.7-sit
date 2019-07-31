using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class SSMRMeterFooterComponent
    {
        private readonly UIView _parentView;
        UIView _containerView;
        UIButton _takePhotoBtn, _submitBtn;
        nfloat containerHeight = 136.0f;
        nfloat buttonHeight = 48.0f;
        nfloat _parentHeight;
        nfloat padding = 16f;

        public SSMRMeterFooterComponent(UIView parentView, nfloat parentHeight)
        {
            _parentView = parentView;
            _parentHeight = parentHeight;
        }

        private void CreateComponent()
        {
            nfloat containerYPos = _parentHeight - containerHeight;
            _containerView = new UIView(new CGRect(0, containerYPos, _parentView.Frame.Width, containerHeight))
            {
                BackgroundColor = UIColor.White
            };
            AddCardShadow(ref _containerView);

            _takePhotoBtn = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(padding, padding, _containerView.Frame.Width - (padding * 2), buttonHeight)
            };
            _takePhotoBtn.Layer.CornerRadius = 4;
            _takePhotoBtn.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _takePhotoBtn.Layer.BorderWidth = 1;
            _takePhotoBtn.Font = MyTNBFont.MuseoSans16_500;
            _takePhotoBtn.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _takePhotoBtn.SetTitle("Take Photos Instead", UIControlState.Normal);
            _takePhotoBtn.Enabled = true;

            _submitBtn = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(padding, _takePhotoBtn.Frame.GetMaxY() + 8f, _containerView.Frame.Width - (padding * 2), buttonHeight)
            };
            _submitBtn.Layer.CornerRadius = 4;
            _submitBtn.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _submitBtn.Layer.BorderWidth = 1;
            _submitBtn.BackgroundColor = MyTNBColor.FreshGreen;
            _submitBtn.Font = MyTNBFont.MuseoSans16_500;
            _submitBtn.SetTitleColor(UIColor.White, UIControlState.Normal);
            _submitBtn.SetTitle("Submit Reading", UIControlState.Normal);
            _submitBtn.Enabled = true;

            _containerView.AddSubview(_takePhotoBtn);
            _containerView.AddSubview(_submitBtn);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.SilverChalice.CGColor;
            view.Layer.ShadowOpacity = 0.1f;
            view.Layer.ShadowOffset = new CGSize(0, 0);
            view.Layer.ShadowRadius = 5;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
    }
}
