using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Components.UsageView
{
    public class UsageFooterViewComponent : BaseComponent
    {
        UIView _parentView, _containerView;
        public UIButton _btnViewBill, _btnPay;
        UILabel _lblPaymentTitle, _lblAmount, _lblDate;
        nfloat _width;
        nfloat _viewHeight;

        public UsageFooterViewComponent(UIView view, nfloat viewHeight)
        {
            _parentView = view;
            _width = _parentView.Frame.Width;
            _viewHeight = viewHeight;
        }

        private void CreateComponent()
        {
            _containerView = new UIView(new CGRect(0, 0, _width, _viewHeight))
            {
                BackgroundColor = UIColor.White
            };
            CreatePaymentLabels();
            CreatePaymentButtons();
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        private void CreatePaymentLabels()
        {
            _lblPaymentTitle = new UILabel(new CGRect(BaseMarginWidth16, BaseMarginWidth16, (_width / 2) - BaseMarginWidth16, GetScaledHeight(20f)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Left,
                Text = "I need to pay"
            };
            _containerView.AddSubview(_lblPaymentTitle);

            _lblDate = new UILabel(new CGRect(BaseMarginWidth16, _lblPaymentTitle.Frame.GetMaxY(), (_width / 2) - BaseMarginWidth16, GetScaledHeight(20f)))
            {
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Left,
                Text = "by 24 Sep 2019"
            };
            _containerView.AddSubview(_lblDate);

            _lblAmount = new UILabel(new CGRect(_width / 2, GetScaledHeight(20f), (_width / 2) - BaseMarginWidth16, GetScaledHeight(32f)))
            {
                Font = TNBFont.MuseoSans_24_300,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Right,
                Text = "RM 420.00"
            };
            _containerView.AddSubview(_lblAmount);
        }

        private void CreatePaymentButtons()
        {
            nfloat yPos = GetScaledHeight(72f);
            _btnViewBill = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(BaseMarginWidth16, yPos, (_width / 2) - GetScaledWidth(18f), GetScaledHeight(48f))
            };
            _btnViewBill.Layer.CornerRadius = GetScaledHeight(4f);
            _btnViewBill.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnViewBill.Layer.BorderWidth = GetScaledHeight(1f);
            _btnViewBill.SetTitle("Component_CurrentBill".Translate(), UIControlState.Normal);
            _btnViewBill.Font = TNBFont.MuseoSans_16_500;
            _btnViewBill.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _containerView.AddSubview(_btnViewBill);

            _btnPay = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(_btnViewBill.Frame.GetMaxX() + GetScaledWidth(4f), yPos, (_width / 2) - GetScaledWidth(18f), GetScaledHeight(48f))
            };
            _btnPay.Layer.CornerRadius = GetScaledHeight(4f);
            _btnPay.Layer.BackgroundColor = MyTNBColor.FreshGreen.CGColor;
            _btnPay.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnPay.Layer.BorderWidth = GetScaledHeight(1f);
            _btnPay.SetTitle("Common_Pay".Translate(), UIControlState.Normal);
            _btnPay.Font = TNBFont.MuseoSans_16_500;
            _containerView.AddSubview(_btnPay);
        }
    }
}
