using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class SmartMeterCardComponent : BaseComponent
    {
        CustomUIView _containerView, _toolTipView;
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
            _containerView.AddSubview(ItemDetailView(_containerView, 0, "Calendar-Icon"));
            UIView line = new UIView(new CGRect(BaseMarginWidth16, GetScaledHeight(64F), width - (BaseMarginHeight16 * 2), GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };
            _containerView.AddSubview(line);
            _containerView.AddSubview(ItemDetailView(_containerView, line.Frame.GetMaxY(), "Predict-Icon"));
            _toolTipView = new CustomUIView(new CGRect(BaseMarginWidth16, GetScaledHeight(64F * 2) + GetScaledHeight(1F), width - (BaseMarginHeight16 * 2), GetScaledHeight(24F)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            _toolTipView.Layer.CornerRadius = GetScaledHeight(12F);
            UIImageView toolTipIcon = new UIImageView(new CGRect(GetScaledWidth(4F), GetScaledHeight(4F), GetScaledWidth(16F), GetScaledHeight(16F)))
            {
                Image = UIImage.FromBundle("IC-Info-Blue")
            };
            UILabel toolTipLabel = new UILabel(new CGRect(toolTipIcon.Frame.GetMaxX() + GetScaledWidth(8F), GetScaledHeight(4F), _toolTipView.Frame.Width * .80F, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Left,
                Text = "What are these?"
            };
            _toolTipView.AddSubviews(new UIView { toolTipLabel, toolTipIcon });
            _containerView.AddSubview(_toolTipView);
        }

        public CustomUIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        private UIView ItemDetailView(UIView parentView, nfloat yPos, string imageName)
        {
            UIImageView icon;
            UILabel title, dateRange, amount;
            nfloat width = parentView.Frame.Width;
            nfloat height = GetScaledHeight(64F);
            nfloat iconWidth = GetScaledWidth(28F);
            nfloat iconHeight = GetScaledHeight(28F);
            UIView itemView = new UIView(new CGRect(0, yPos, width, height))
            {
                BackgroundColor = UIColor.Clear
            };
            icon = new UIImageView(new CGRect(BaseMarginWidth16, GetScaledHeight(18F), iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(imageName)
            };
            title = new UILabel(new CGRect(icon.Frame.GetMaxX() + GetScaledWidth(12), BaseMarginHeight16, width * .40F, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Left,
                Text = "My bill amount so far"
            };
            dateRange = new UILabel(new CGRect(icon.Frame.GetMaxX() + GetScaledWidth(12), title.Frame.GetMaxY(), width * .40F, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.WarmGrey,
                TextAlignment = UITextAlignment.Left,
                Text = "for 22 Jun - 21 Jul"
            };
            amount = new UILabel(new CGRect(width * .68F - BaseMarginWidth16, GetScaledHeight(22F), width * .32F, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.WarmGrey,
                TextAlignment = UITextAlignment.Right,
                Text = "RM 120.00"
            };
            itemView.AddSubviews(new UIView { icon, title, dateRange, amount });
            return itemView;
        }
    }
}
