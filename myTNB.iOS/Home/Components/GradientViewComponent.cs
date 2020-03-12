using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class GradientViewComponent
    {
        readonly UIView _parentView;
        private UIView _gradientView;
        private float _screenPercentage, _viewHeight;
        private bool _isHorizontal, _isFixedHeight;
        private CAGradientLayer _gradientLayer;

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
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                _viewHeight += 24;
            }
            float viewHeight = _isFixedHeight ? _viewHeight : ((float)_parentView.Frame.Height * _screenPercentage);
            _gradientView = new UIView(new CGRect(0, 0, _parentView.Frame.Width, viewHeight));
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            _gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor }
            };
            if (_isHorizontal)
            {
                _gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
                _gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);
            }
            else
            {
                _gradientLayer.Locations = new NSNumber[] { 0, 1 };
            }
            _gradientLayer.Frame = _gradientView.Bounds;
            _gradientView.Layer.InsertSublayer(_gradientLayer, 0);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _gradientView;
        }

        public void SetOpacity(float opacity)
        {
            _gradientLayer.Opacity = opacity;
        }
    }
}