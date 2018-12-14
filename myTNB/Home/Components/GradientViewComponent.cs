using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class GradientViewComponent
    {
        UIView _parentView;
        UIView _gradientView;
        float _screenPercentage;
        bool _isFixedHeight = false;
        float _viewHeight = 0;
        bool _isHorizontal = false;

        public GradientViewComponent(UIView view, float screenPercentage)
        {
            _parentView = view;
            _screenPercentage = screenPercentage;
        }

        public GradientViewComponent(UIView view, float screenPercentage, bool isHorizontal)
        {
            _parentView = view;
            _screenPercentage = screenPercentage;
            _isHorizontal = isHorizontal;
        }

        public GradientViewComponent(UIView view, bool isFixedHeight, float height)
        {
            _parentView = view;
            _isFixedHeight = isFixedHeight;
            _viewHeight = height;
        }

        public GradientViewComponent(UIView view, bool isFixedHeight, float height, bool isHorizontal)
        {
            _parentView = view;
            _isFixedHeight = isFixedHeight;
            _viewHeight = height;
            _isHorizontal = isHorizontal;
        }

        internal void CreateComponent()
        {
            if (DeviceHelper.IsIphoneX())
            {
                _viewHeight += 24;
            }
            float viewHeight = _isFixedHeight ? _viewHeight : ((float)_parentView.Frame.Height * _screenPercentage);
            _gradientView = new UIView(new CGRect(0, 0, _parentView.Frame.Width, viewHeight));
            var startColor = myTNBColor.GradientPurpleDarkElement();
            var endColor = myTNBColor.GradientPurpleLightElement();
            var gradientLayer = new CAGradientLayer();
            gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
            if (_isHorizontal)
            {
                gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
                gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);
            }
            else
            {
                gradientLayer.Locations = new NSNumber[] { 0, 1 };
            }
            gradientLayer.Frame = _gradientView.Bounds;
            _gradientView.Layer.InsertSublayer(gradientLayer, 0);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _gradientView;
        }
    }
}