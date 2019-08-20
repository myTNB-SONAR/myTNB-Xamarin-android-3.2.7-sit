using System;
using CoreGraphics;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB
{
    public class DisconnectionComponent : BaseComponent
    {
        CustomUIView _containerView;
        UIView _parentView;
        AccountStatusDataModel _accountStatusData = new AccountStatusDataModel();

        public DisconnectionComponent(UIView parentView, AccountStatusDataModel accountStatusData)
        {
            _parentView = parentView;
            _accountStatusData = accountStatusData;
        }

        private void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width - (BaseMarginHeight16 * 2);
            nfloat height = GetScaledHeight(24f);
            _containerView = new CustomUIView(new CGRect(BaseMarginHeight16, 0, width, height))
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
                Text = _accountStatusData.AccountStatusMessage ?? string.Empty // TO DO: assign fallback text 
            };
            _containerView.AddSubview(label);
        }

        public CustomUIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        public void SetGestureRecognizer(UITapGestureRecognizer recognizer)
        {
            _containerView.AddGestureRecognizer(recognizer);
        }
    }
}
