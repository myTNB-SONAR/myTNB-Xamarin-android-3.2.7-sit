using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class SSMRMeterFooterComponent
    {
        private readonly UIView _parentView;
        UIView _containerView;
        public UIButton _submitBtn;
        nfloat containerHeight = ScaleUtility.GetScaledHeight(80.0f);
        nfloat buttonHeight = ScaleUtility.GetScaledHeight(48.0f);
        nfloat _parentHeight;
        nfloat padding = ScaleUtility.GetScaledWidth(16f);

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

            _submitBtn = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(padding, padding, _containerView.Frame.Width - (padding * 2), buttonHeight)
            };
            _submitBtn.Layer.CornerRadius = 4;
            _submitBtn.Layer.BorderColor = MyTNBColor.SilverChalice.CGColor;
            _submitBtn.Layer.BorderWidth = 1;
            _submitBtn.BackgroundColor = MyTNBColor.SilverChalice;
            _submitBtn.Font = MyTNBFont.MuseoSans16_500;
            _submitBtn.SetTitleColor(UIColor.White, UIControlState.Normal);
            _submitBtn.SetTitle("Submit Reading", UIControlState.Normal);
            _submitBtn.Enabled = false;

            _containerView.AddSubview(_submitBtn);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        public UIView GetView()
        {
            return _containerView;
        }

        public void SetSubmitButtonEnabled(bool isEnable)
        {
            _submitBtn.Enabled = isEnable;
            _submitBtn.BackgroundColor = isEnable ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
            _submitBtn.Layer.BorderColor = isEnable ? MyTNBColor.FreshGreen.CGColor : MyTNBColor.SilverChalice.CGColor;
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.SilverChalice.CGColor;
            view.Layer.ShadowOpacity = 0.5f;
            view.Layer.ShadowOffset = new CGSize(0, 1);
            view.Layer.ShadowRadius = 5;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
    }
}
