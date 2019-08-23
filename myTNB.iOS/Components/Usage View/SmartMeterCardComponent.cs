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
                BackgroundColor = UIColor.Yellow
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
            itemView.AddSubviews(new UIView { icon, title, dateRange });
            return itemView;
        }
    }
}
