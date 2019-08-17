using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Components.UsageView
{
    public class DisconnectionComponent : BaseComponent
    {
        UIView _parentView, _containerView;

        public DisconnectionComponent(UIView parentView)
        {
            _parentView = parentView;
        }

        private void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width - (BaseMarginHeight16 * 2);
            nfloat height = GetScaledHeight(24f);
            _containerView = new UIView(new CGRect(BaseMarginHeight16, 0, width, height))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            _containerView.Layer.CornerRadius = GetScaledHeight(12f);

            nfloat iconXPos = GetScaledWidth(4f);
            nfloat iconYPos = GetScaledHeight(4f);
            nfloat iconWidth = GetScaledWidth(16f);
            nfloat iconHeight = GetScaledHeight(16f);
            UIImageView iconView = new UIImageView(new CGRect(iconXPos, iconYPos, iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle("Info-Black-Icon")
            };
            _containerView.AddSubview(iconView);

            nfloat labelXPos = GetScaledWidth(28f);
            nfloat labelYPos = GetScaledHeight(4f);
            nfloat labelWidth = GetScaledWidth(231f);
            nfloat labelHeight = GetScaledHeight(16f);
            UILabel label = new UILabel(new CGRect(labelXPos, labelYPos, labelWidth, labelHeight))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.GreyishBrown,
                Text = "Your electricity is currently disconnected."
            };
            _containerView.AddSubview(label);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }
    }
}
