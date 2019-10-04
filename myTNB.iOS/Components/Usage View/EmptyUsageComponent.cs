using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class EmptyUsageComponent : BaseComponent
    {
        CustomUIView _parentView, _containerView;
        UILabel _messageLbl;
        public EmptyUsageComponent(CustomUIView parentView)
        {
            _parentView = parentView;
        }

        private void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width;
            nfloat height = _parentView.Frame.Height;
            _containerView = new CustomUIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat iconWidth = GetScaledWidth(96);
            nfloat iconHeight = GetScaledHeight(98);
            UIImageView icon = new UIImageView(new CGRect(GetXLocationToCenterObject(iconWidth, _containerView), GetScaledHeight(80), iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(UsageConstants.IMG_EmpyUsage)
            };
            _containerView.AddSubview(icon);
            _messageLbl = new UILabel(new CGRect(GetScaledWidth(32), icon.Frame.GetMaxY() + GetScaledHeight(24), _containerView.Frame.Width - (GetScaledWidth(32) * 2), 0))
            {
                Font = TNBFont.MuseoSans_14_300,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Lines = 0
            };
            _containerView.AddSubview(_messageLbl);
        }

        public CustomUIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        public void SetMessage(string msg)
        {
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrWhiteSpace(msg))
            {
                if (_messageLbl != null)
                {
                    _messageLbl.Text = msg;
                    CGSize lblSize = _messageLbl.SizeThatFits(new CGSize(_messageLbl.Frame.Width, 1000F));
                    ViewHelper.AdjustFrameSetHeight(_messageLbl, lblSize.Height);
                }
            }
        }
    }
}
