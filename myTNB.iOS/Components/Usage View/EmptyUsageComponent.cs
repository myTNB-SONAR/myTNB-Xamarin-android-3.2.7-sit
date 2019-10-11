using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class EmptyUsageComponent : BaseComponent
    {
        CustomUIView _parentView, _containerView;
        UIImageView _icon;
        UILabel _messageLbl;

        public EmptyUsageComponent(CustomUIView parentView)
        {
            _parentView = parentView;
        }

        private void CreateComponent(string msg)
        {
            nfloat width = _parentView.Frame.Width;
            nfloat height = _parentView.Frame.Height;
            _containerView = new CustomUIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat iconWidth = GetScaledWidth(96);
            nfloat iconHeight = GetScaledHeight(98);
            _icon = new UIImageView(new CGRect(GetXLocationToCenterObject(iconWidth, _containerView), 0, iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(UsageConstants.IMG_EmpyUsage)
            };
            _containerView.AddSubview(_icon);

            string message = GetI18NValue(UsageConstants.I18N_EmptyDataMessage);
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrWhiteSpace(msg))
            {
                message = msg;
            }

            _messageLbl = new UILabel(new CGRect(GetScaledWidth(32), GetYLocationFromFrame(_icon.Frame, 24F), _containerView.Frame.Width - (GetScaledWidth(32) * 2), 0))
            {
                Font = TNBFont.MuseoSans_14_300,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                Text = message
            };
            _containerView.AddSubview(_messageLbl);

            CGSize lblSize = _messageLbl.SizeThatFits(new CGSize(_messageLbl.Frame.Width, 1000F));
            ViewHelper.AdjustFrameSetHeight(_messageLbl, lblSize.Height);
            nfloat totalHeight = iconHeight + GetScaledHeight(24) + lblSize.Height;
            ViewHelper.AdjustFrameSetHeight(_containerView, totalHeight);
            ViewHelper.AdjustFrameSetY(_containerView, GetYLocationToCenterObject(totalHeight, _parentView));
        }

        public CustomUIView GetUI(string msg)
        {
            CreateComponent(msg);
            return _containerView;
        }
    }
}