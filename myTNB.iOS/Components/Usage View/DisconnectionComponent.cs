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
                Image = UIImage.FromBundle(Constants.IMG_InfoBlackIcon)
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
                Text = !string.IsNullOrEmpty(_accountStatusData?.AccountStatusMessage) &&
                    !string.IsNullOrWhiteSpace(_accountStatusData?.AccountStatusMessage) ?
                    _accountStatusData?.AccountStatusMessage :
                    LanguageUtility.GetCommonI18NValue(Constants.I18N_DisconnectionMsg)
            };
            _containerView.AddSubview(label);
        }

        public CustomUIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        public virtual CustomUIView GetShimmerUI()
        {
            nfloat baseWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat baseHeight = GetScaledHeight(24f);
            CustomShimmerView shimmeringView = new CustomShimmerView();
            CustomUIView parentView = new CustomUIView(new CGRect(BaseMarginWidth16, 0
                , baseWidth - (BaseMarginWidth16 * 2), baseHeight))
            { BackgroundColor = UIColor.Clear };
            UIView viewShimmerParent = new UIView(new CGRect(new CGPoint(0, 0), parentView.Frame.Size)) { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(new CGPoint(0, 0), parentView.Frame.Size)) { BackgroundColor = UIColor.Clear };
            parentView.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

            UIView viewDisconnection = new UIView(new CGRect(GetXLocationToCenterObject(GetScaledWidth(231f), parentView), GetScaledHeight(4f)
                , GetScaledWidth(231f), GetScaledHeight(16f) * .80F))
            {
                BackgroundColor = new UIColor(red: 0.75f, green: 0.85f, blue: 0.95f, alpha: 0.25f)
            };
            viewDisconnection.Layer.CornerRadius = GetScaledHeight(2f);

            viewShimmerContent.AddSubview(viewDisconnection);
            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();

            return parentView;
        }

        public void SetGestureRecognizer(UITapGestureRecognizer recognizer)
        {
            _containerView.AddGestureRecognizer(recognizer);
        }
    }
}
