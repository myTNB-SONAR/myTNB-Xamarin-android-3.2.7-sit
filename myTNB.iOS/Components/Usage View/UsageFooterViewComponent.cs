using System;
using System.Globalization;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class UsageFooterViewComponent : BaseComponent
    {
        UIView _parentView, _containerView, _shimmerParent, _shimmerContent, _viewTitle, _viewAmount, _viewDate;
        public UIButton _btnViewBill, _btnPay;
        UILabel _lblPaymentTitle, _lblAmount, _lblDate, _lblCommon;
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
            _shimmerParent = new UIView(new CGRect(0, 0, _width, GetScaledHeight(72f)))
            {
                BackgroundColor = UIColor.Clear
            };
            _shimmerContent = new UIView(new CGRect(0, 0, _width, GetScaledHeight(72f)))
            {
                BackgroundColor = UIColor.Clear
            };
            _containerView.AddSubviews(new UIView[] { _shimmerParent, _shimmerContent });
            CustomShimmerView shimmeringView = new CustomShimmerView();
            _shimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = _shimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();

            CreatePaymentLabels();
            CreatePaymentButtons();
            UpdateUI(true);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        private void CreatePaymentLabels()
        {
            nfloat labelWidth = (_width / 2) - BaseMarginWidth16;
            _lblPaymentTitle = new UILabel(new CGRect(BaseMarginWidth16, BaseMarginWidth16, labelWidth, GetScaledHeight(20f)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Left,
                Hidden = false
            };
            _containerView.AddSubview(_lblPaymentTitle);

            _viewTitle = new UIView(new CGRect(BaseMarginWidth16, BaseMarginWidth16, labelWidth * 0.8F, _lblPaymentTitle.Frame.Height * 0.8F))
            {
                BackgroundColor = MyTNBColor.PaleGreyThree
            };
            _viewTitle.Layer.CornerRadius = GetScaledHeight(4f);

            _lblDate = new UILabel(new CGRect(BaseMarginWidth16, _lblPaymentTitle.Frame.GetMaxY(), labelWidth, GetScaledHeight(20f)))
            {
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Left,
                Hidden = false
            };
            _containerView.AddSubview(_lblDate);

            _viewDate = new UIView(new CGRect(BaseMarginWidth16, _lblPaymentTitle.Frame.GetMaxY(), labelWidth * 0.7F, _lblDate.Frame.Height * 0.8F))
            {
                BackgroundColor = MyTNBColor.PaleGreyThree
            };
            _viewDate.Layer.CornerRadius = GetScaledHeight(4f);

            _lblCommon = new UILabel(new CGRect(BaseMarginWidth16, GetScaledHeight(26f), labelWidth, GetScaledHeight(20f)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };
            _containerView.AddSubview(_lblCommon);

            _lblAmount = new UILabel(new CGRect(_width / 2, GetScaledHeight(20f), labelWidth, GetScaledHeight(32f)))
            {
                Font = TNBFont.MuseoSans_24_300,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Right
            };
            _containerView.AddSubview(_lblAmount);

            _viewAmount = new UIView(new CGRect(_width / 2 + labelWidth * 0.2F, GetScaledHeight(20f), labelWidth * 0.8F, _lblAmount.Frame.Height * 0.8F))
            {
                BackgroundColor = MyTNBColor.PaleGreyThree
            };
            _viewAmount.Layer.CornerRadius = GetScaledHeight(4f);

            _shimmerContent.AddSubviews(new UIView[] { _viewTitle, _viewAmount, _viewDate });
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

        public void UpdateUI(bool isUpdating)
        {
            _lblPaymentTitle.Hidden = isUpdating;
            _lblDate.Hidden = isUpdating;
            _lblAmount.Hidden = isUpdating;
            _shimmerParent.Hidden = !isUpdating;

            _btnViewBill.Enabled = !isUpdating;
            _btnViewBill.Layer.BorderColor = isUpdating ? MyTNBColor.SilverChalice.CGColor : MyTNBColor.FreshGreen.CGColor;
            _btnViewBill.SetTitleColor(isUpdating ? MyTNBColor.SilverChalice : MyTNBColor.FreshGreen, UIControlState.Normal);
            _btnPay.Enabled = !isUpdating;
            _btnPay.Layer.BackgroundColor = isUpdating ? MyTNBColor.SilverChalice.CGColor : MyTNBColor.FreshGreen.CGColor;
            _btnPay.Layer.BorderColor = isUpdating ? MyTNBColor.SilverChalice.CGColor : MyTNBColor.FreshGreen.CGColor;
        }

        public void SetAmount(double amount)
        {
            if (amount >= 0)
            {
                _lblAmount.AttributedText = TextHelper.CreateValuePairString(amount.ToString("N2", CultureInfo.InvariantCulture)
                        , TNBGlobal.UNIT_CURRENCY + " ", true, TNBFont.MuseoSans_24_300
                        , MyTNBColor.CharcoalGrey, TNBFont.MuseoSans_12_500, MyTNBColor.CharcoalGrey);
            }
            else if (amount < 0)
            {

                _lblAmount.AttributedText = TextHelper.CreateValuePairString(Math.Abs(amount).ToString("N2", CultureInfo.InvariantCulture)
                        , TNBGlobal.UNIT_CURRENCY + " ", true, TNBFont.MuseoSans_24_300
                        , MyTNBColor.FreshGreen, TNBFont.MuseoSans_12_500, MyTNBColor.FreshGreen);
            }
            AdjustLabels(amount);
        }
        private void AdjustLabels(double amount)
        {
            if (amount > 0)
            {
                _lblPaymentTitle.Hidden = false;
                _lblPaymentTitle.Text = "I need to pay";
                _lblDate.Hidden = false;
                _lblCommon.Hidden = true;
            }
            else
            {
                _lblPaymentTitle.Hidden = true;
                _lblPaymentTitle.Text = string.Empty;
                _lblDate.Hidden = true;
                _lblCommon.Hidden = false;
                _lblCommon.Text = amount < 0 ? "I’ve paid extra" : "I’ve cleared all bills";
            }
        }

        public void SetDate(string date)
        {
            if (!string.IsNullOrEmpty(date) && !string.IsNullOrWhiteSpace(date))
            {
                string formattedDate = DateHelper.GetFormattedDate(date, "dd MMM yyyy");
                _lblDate.Text = formattedDate;
            }
        }
    }
}
