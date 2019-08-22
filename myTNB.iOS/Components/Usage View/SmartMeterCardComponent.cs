using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class SmartMeterCardComponent : BaseComponent
    {
        CustomUIView _containerView;
        UIView _parentView;

        public SmartMeterCardComponent(UIView parentView)
        {
            _parentView = parentView;
        }

        private void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width - (BaseMarginWidth16 * 2);
            nfloat height = GetScaledHeight(169F);
            _containerView = new CustomUIView(new CGRect(BaseMarginWidth16, 0, width, height))
            {
                BackgroundColor = UIColor.White
            };
            _containerView.Layer.CornerRadius = GetScaledHeight(5F);
        }

        public CustomUIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }
    }
}
