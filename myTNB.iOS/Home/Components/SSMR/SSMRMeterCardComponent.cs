using System;
using System.Diagnostics;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class SSMRMeterCardComponent
    {
        private readonly UIView _parentView;
        UIView _containerView, _prevReadingView, _viewBoxContainer, _viewBox;
        UIImageView _iconView;
        nfloat containerRatio = 112.0f / 288.0f;
        nfloat viewBoxContainerRatio = 40.0f / 256.0f;
        float imgHeight = 20.0f;
        float imgWidth = 52.0f;
        nfloat padding = 16f;
        nfloat halfPadding = 8f;
        int boxMaxCount = 9;
        nfloat _yLocation;

        public SSMRMeterCardComponent(UIView parentView, nfloat yLocation)
        {
            _parentView = parentView;
            _yLocation = yLocation;
        }

        private void CreateComponent()
        {
            nfloat containerHeight = _parentView.Frame.Width * containerRatio;
            _containerView = new UIView(new CGRect(padding, _yLocation, _parentView.Frame.Width - (padding * 2), containerHeight))
            {
                BackgroundColor = UIColor.White
            };
            _containerView.Layer.CornerRadius = 5.0f;

            nfloat viewBoxContainerHeight = _containerView.Frame.Width * viewBoxContainerRatio;
            nfloat viewBoxContainerWidth = _containerView.Frame.Width - (padding * 2);
            _viewBoxContainer = new UIView(new CGRect(padding, DeviceHelper.GetCenterYWithObjHeight((float)viewBoxContainerHeight, _containerView), viewBoxContainerWidth, viewBoxContainerHeight))
            {
                BackgroundColor = UIColor.Clear
            };
            for (int i = 0; i < boxMaxCount; i++)
            {
                _viewBoxContainer.AddSubview(CreateViewBox(_viewBoxContainer, i));
            }
            _containerView.AddSubview(_viewBoxContainer);

            nfloat prevReadingHeight = _viewBoxContainer.Frame.GetMinY() - (halfPadding * 2);
            _prevReadingView = new UIView(new CGRect(padding, halfPadding, _viewBoxContainer.Frame.Width, prevReadingHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            _containerView.AddSubview(_prevReadingView);

            nfloat iconHeight = DeviceHelper.GetScaledHeight(imgHeight);
            nfloat iconWidth = DeviceHelper.GetScaledWidth(imgWidth);
            nfloat yPos = _viewBoxContainer.Frame.GetMaxY() + (containerHeight - _viewBoxContainer.Frame.GetMaxY()) / 2 - (iconHeight / 2);
            _iconView = new UIImageView(new CGRect(_containerView.Frame.Width - padding - iconWidth, yPos, iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle("kWh-Icon")
            };

            _containerView.AddSubview(_iconView);

            AddCardShadow(ref _containerView);

            ShowPreviousReading("1234567");
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

        public UIView CreateViewBox(UIView containerView, int index)
        {
            var width = (containerView.Frame.Width / boxMaxCount) - 1;
            var height = containerView.Frame.Height;
            var xPos = (containerView.Frame.Width / boxMaxCount * index) + 1;
            UIView viewBox = new UIView(new CGRect(xPos, 0, width, height))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkFour,
                Tag = index
            };
            return viewBox;
        }

        public UIView CreateViewBoxForPrevReading(UIView containerView, int index, char digit)
        {
            var width = (containerView.Frame.Width / boxMaxCount) - 1;
            var height = containerView.Frame.Height;
            var xPos = (containerView.Frame.Width / boxMaxCount * index) + 1;
            UIView viewBox = new UIView(new CGRect(xPos, 0, width, height))
            {
                BackgroundColor = UIColor.Clear,
                Tag = index
            };

            UILabel digitLabel = new UILabel(new CGRect(0, 0, width, height))
            {
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.Grey,
                TextAlignment = UITextAlignment.Center,
                Text = digit.ToString()
            };
            viewBox.AddSubview(digitLabel);

            return viewBox;
        }

        private void ShowPreviousReading(string text)
        {
            int startIndx = boxMaxCount - text.Length;
            for (int i = 0; i < text.Length; i++)
            {
                if (i < startIndx)
                {
                    continue;
                }
                _prevReadingView.AddSubview(CreateViewBoxForPrevReading(_prevReadingView, i, text[i]));
            }
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 5f;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue.CGColor;
            view.Layer.ShadowOpacity = 1;
            view.Layer.ShadowOffset = new CGSize(0, 0);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
    }
}
