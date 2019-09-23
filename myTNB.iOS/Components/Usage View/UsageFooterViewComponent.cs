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
        nfloat _width, _viewHeight, _yPos;

        public UsageFooterViewComponent(UIView view, nfloat viewHeight, nfloat yPos)
        {
            _parentView = view;
            _width = _parentView.Frame.Width;
            _viewHeight = viewHeight;
            _yPos = yPos;
        }

        private void CreateComponent()
        {
            _containerView = new UIView(new CGRect(0, _yPos, _width, _viewHeight))
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
                BackgroundColor = MyTNBColor.PaleGrey
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
                BackgroundColor = MyTNBColor.PaleGrey
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
                BackgroundColor = MyTNBColor.PaleGrey
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
            _btnViewBill.SetTitle(LanguageUtility.GetCommonI18NValue(Constants.I18N_ViewDetails), UIControlState.Normal);
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
            _btnPay.SetTitle(LanguageUtility.GetCommonI18NValue(Constants.I18N_Pay), UIControlState.Normal);
            _btnPay.Font = TNBFont.MuseoSans_16_500;
            _containerView.AddSubview(_btnPay);
        }

        public void UpdateUI(bool isUpdating)
        {
            if (_containerView != null)
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
        }

        public void SetAmount(double amount)
        {
            if (amount >= 0)
            {
                if (_lblAmount != null)
                {
                    _lblAmount.AttributedText = TextHelper.CreateValuePairString(amount.ToString("N2", CultureInfo.InvariantCulture)
                        , TNBGlobal.UNIT_CURRENCY + " ", true, TNBFont.MuseoSans_24_300
                        , MyTNBColor.CharcoalGrey, TNBFont.MuseoSans_12_500, MyTNBColor.CharcoalGrey);
                }
            }
            else if (amount < 0)
            {
                if (_lblAmount != null)
                {
                    _lblAmount.AttributedText = TextHelper.CreateValuePairString(Math.Abs(amount).ToString("N2", CultureInfo.InvariantCulture)
                    , TNBGlobal.UNIT_CURRENCY + " ", true, TNBFont.MuseoSans_24_300
                    , MyTNBColor.FreshGreen, TNBFont.MuseoSans_12_500, MyTNBColor.FreshGreen);
                }
            }

            AdjustLabels(amount);
        }
        private void AdjustLabels(double amount)
        {
            if (_containerView != null)
            {
                if (amount > 0)
                {
                    _lblPaymentTitle.Hidden = false;
                    _lblPaymentTitle.Text = LanguageUtility.GetCommonI18NValue(Constants.I18N_NeedToPay);
                    _lblDate.Hidden = false;
                    _lblCommon.Hidden = true;
                }
                else
                {
                    _lblPaymentTitle.Hidden = true;
                    _lblPaymentTitle.Text = string.Empty;
                    _lblDate.Hidden = true;
                    _lblCommon.Hidden = false;
                    _lblCommon.Text = amount < 0 ? LanguageUtility.GetCommonI18NValue(Constants.I18N_PaidExtra) : LanguageUtility.GetCommonI18NValue(Constants.I18N_ClearedAllBills);
                }
            }
        }

        public void SetDate(string date)
        {
            if (!string.IsNullOrEmpty(date) && !string.IsNullOrWhiteSpace(date))
            {
                if (_lblDate != null)
                {
                    string formattedDate = DateHelper.GetFormattedDate(date, "dd MMM yyyy");
                    _lblDate.Text = formattedDate;
                }
            }
        }

        public void SetRefreshState()
        {
            if (_containerView != null)
            {
                _lblPaymentTitle.Hidden = false;
                _lblPaymentTitle.Text = LanguageUtility.GetCommonI18NValue(Constants.I18N_NeedToPay);
                _lblDate.Hidden = false;
                _lblAmount.Hidden = false;
                _shimmerParent.Hidden = true;

                _lblDate.Text = "- -";
                _lblAmount.AttributedText = TextHelper.CreateValuePairString("- -"
                            , TNBGlobal.UNIT_CURRENCY + " ", true, TNBFont.MuseoSans_24_300
                            , MyTNBColor.CharcoalGrey, TNBFont.MuseoSans_12_500, MyTNBColor.CharcoalGrey);

                _btnViewBill.Enabled = false;
                _btnViewBill.Layer.BorderColor = MyTNBColor.SilverChalice.CGColor;
                _btnViewBill.SetTitleColor(MyTNBColor.SilverChalice, UIControlState.Normal);
                _btnPay.Enabled = false;
                _btnPay.Layer.BackgroundColor = MyTNBColor.SilverChalice.CGColor;
                _btnPay.Layer.BorderColor = MyTNBColor.SilverChalice.CGColor;
            }
        }
    }
}
