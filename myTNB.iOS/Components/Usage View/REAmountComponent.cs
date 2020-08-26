using System;
using System.Globalization;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class REAmountComponent : BaseComponent
    {
        CustomUIView _containerView;
        UIView _parentView, _shimmerParent, _shimmerContent, _viewIcon, _viewTitle, _viewDate, _viewAmount;
        UIImageView _iconView;
        public UIButton _btnViewPaymentAdvice;
        UILabel _lblTitle, _lblDate, _lblAmount;
        nfloat _viewWidth;
        nfloat _viewHeight;

        public REAmountComponent(UIView parentView)
        {
            _parentView = parentView;
            _viewWidth = _parentView.Frame.Width - (BaseMarginWidth16 * 2);
            _viewHeight = _parentView.Frame.Height;
        }

        private void CreateComponent()
        {
            _containerView = new CustomUIView(new CGRect(BaseMarginWidth16, 0, _viewWidth, _viewHeight))
            {
                BackgroundColor = UIColor.White
            };
            _containerView.Layer.CornerRadius = GetScaledHeight(4f);
            AddContainerShadow(ref _containerView);
            _shimmerParent = new UIView(new CGRect(0, 0, _viewWidth, GetScaledHeight(62f)))
            {
                BackgroundColor = UIColor.Clear
            };
            _shimmerContent = new UIView(new CGRect(0, 0, _viewWidth, GetScaledHeight(62f)))
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

        public CustomUIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        private void CreatePaymentLabels()
        {
            _iconView = new UIImageView(new CGRect(BaseMarginWidth16, GetScaledHeight(16f), GetScaledWidth(32f), GetScaledHeight(32f)))
            {
                Image = UIImage.FromBundle(Constants.IMG_AcctREIcon)
            };
            _containerView.AddSubview(_iconView);

            _viewIcon = new UIView(new CGRect(BaseMarginWidth16, GetScaledHeight(16f), GetScaledWidth(32f), GetScaledHeight(32f)))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            _viewIcon.Layer.CornerRadius = GetScaledHeight(14f);

            nfloat labelWidth = (_viewWidth / 2) - BaseMarginWidth16;
            _lblTitle = new UILabel(new CGRect(_iconView.Frame.GetMaxX() + GetScaledWidth(8F), BaseMarginWidth16, labelWidth, GetScaledHeight(16f)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(UsageConstants.I18N_MyEarnings),
                Hidden = false
            };
            _containerView.AddSubview(_lblTitle);

            _viewTitle = new UIView(new CGRect(_iconView.Frame.GetMaxX() + GetScaledWidth(8F), BaseMarginWidth16, labelWidth * 0.8F, _lblTitle.Frame.Height * 0.8F))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            _viewTitle.Layer.CornerRadius = GetScaledHeight(4f);

            _lblDate = new UILabel(new CGRect(_iconView.Frame.GetMaxX() + GetScaledWidth(8F), _lblTitle.Frame.GetMaxY(), labelWidth, GetScaledHeight(16f)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.WarmGrey,
                TextAlignment = UITextAlignment.Left,
                Hidden = false
            };
            _containerView.AddSubview(_lblDate);

            _viewDate = new UIView(new CGRect(_iconView.Frame.GetMaxX() + GetScaledWidth(8F), _lblTitle.Frame.GetMaxY(), labelWidth * 0.7F, _lblDate.Frame.Height * 0.8F))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            _viewDate.Layer.CornerRadius = GetScaledHeight(4f);

            _lblAmount = new UILabel(new CGRect(_viewWidth / 2, GetScaledHeight(22f), labelWidth, GetScaledHeight(20f)))
            {
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Right
            };
            _containerView.AddSubview(_lblAmount);

            _viewAmount = new UIView(new CGRect(_viewWidth / 2 + labelWidth * 0.2F, GetScaledHeight(22f), labelWidth * 0.8F, _lblAmount.Frame.Height * 0.8F))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            _viewAmount.Layer.CornerRadius = GetScaledHeight(4f);

            _shimmerContent.AddSubviews(new UIView[] { _viewIcon, _viewTitle, _viewDate, _viewAmount });
        }

        private void CreatePaymentButtons()
        {
            nfloat yPos = GetScaledHeight(62f);
            _btnViewPaymentAdvice = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(BaseMarginWidth16, yPos, _viewWidth - (BaseMarginHeight16 * 2), GetScaledHeight(40f))
            };
            _btnViewPaymentAdvice.Layer.CornerRadius = GetScaledHeight(4f);
            _btnViewPaymentAdvice.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnViewPaymentAdvice.Layer.BorderWidth = GetScaledHeight(1f);
            _btnViewPaymentAdvice.SetTitle(GetI18NValue(UsageConstants.I18N_ViewPaymentAdvice), UIControlState.Normal);
            _btnViewPaymentAdvice.Font = TNBFont.MuseoSans_16_500;
            _btnViewPaymentAdvice.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _containerView.AddSubview(_btnViewPaymentAdvice);
        }

        private void AddContainerShadow(ref CustomUIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue35.CGColor;
            view.Layer.ShadowOpacity = .5f;
            view.Layer.ShadowOffset = new CGSize(0, 8);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }

        public void SetValues(string date, double amount)
        {
            double amnt = amount * -1;
            _lblAmount.AttributedText = TextHelper.CreateValuePairString(Math.Abs(amnt).ToString("N2", CultureInfo.InvariantCulture)
                        , TNBGlobal.UNIT_CURRENCY + " ", true, TNBFont.MuseoSans_16_300
                        , MyTNBColor.GreyishBrown, TNBFont.MuseoSans_10_300, MyTNBColor.GreyishBrown);

            if (amnt < 0)
            {
                _lblDate.Hidden = true;
                _lblTitle.Text = GetI18NValue(UsageConstants.I18N_BeenPaidExtra);
                ViewHelper.AdjustFrameSetY(_lblTitle, GetScaledHeight(24F));
            }
            else
            {
                _lblTitle.Text = GetI18NValue(UsageConstants.I18N_MyEarnings);
                if (amnt == 0)
                {
                    _lblDate.Hidden = true;
                    ViewHelper.AdjustFrameSetY(_lblTitle, GetScaledHeight(24F));
                }
                else
                {
                    ViewHelper.AdjustFrameSetY(_lblTitle, GetScaledHeight(16F));
                    if (!string.IsNullOrEmpty(date) && !string.IsNullOrWhiteSpace(date))
                    {
                        string formattedDate = DateHelper.GetFormattedDate(date, "dd MMM");

                        _lblDate.Hidden = false;
                        _lblDate.AttributedText = TextHelper.CreateValuePairString(formattedDate
                                , GetI18NValue(UsageConstants.I18N_IWillGetBy) + " ", true, TNBFont.MuseoSans_12_300
                                , MyTNBColor.WarmGrey, TNBFont.MuseoSans_12_300, MyTNBColor.WarmGrey);
                    }
                    else
                    {
                        _lblDate.Hidden = false;
                        _lblDate.Text = "- -";
                    }
                }
            }
        }

        public void UpdateUI(bool isUpdating)
        {
            _iconView.Hidden = isUpdating;
            _lblTitle.Hidden = isUpdating;
            _lblDate.Hidden = isUpdating;
            _lblAmount.Hidden = isUpdating;
            _shimmerParent.Hidden = !isUpdating;
            EnableDisableCTA(!isUpdating);
        }

        public void EnableDisableCTA(bool enable)
        {
            _btnViewPaymentAdvice.Enabled = enable;
            _btnViewPaymentAdvice.Layer.BorderColor = enable ? MyTNBColor.FreshGreen.CGColor : MyTNBColor.SilverChalice.CGColor;
            _btnViewPaymentAdvice.SetTitleColor(enable ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice, UIControlState.Normal);
        }

        public void SetRefreshState()
        {
            _iconView.Hidden = false;
            _lblTitle.Hidden = false;
            _lblDate.Hidden = false;
            _lblAmount.Hidden = false;
            _shimmerParent.Hidden = true;

            _lblDate.Text = GetI18NValue(UsageConstants.I18N_EmptyDateAmount);
            _lblAmount.AttributedText = TextHelper.CreateValuePairString(GetI18NValue(UsageConstants.I18N_EmptyDateAmount)
                        , TNBGlobal.UNIT_CURRENCY + " ", true, TNBFont.MuseoSans_16_300
                        , MyTNBColor.GreyishBrown, TNBFont.MuseoSans_10_300, MyTNBColor.GreyishBrown);

            _btnViewPaymentAdvice.Enabled = false;
            _btnViewPaymentAdvice.Layer.BorderColor = MyTNBColor.SilverChalice.CGColor;
            _btnViewPaymentAdvice.SetTitleColor(MyTNBColor.SilverChalice, UIControlState.Normal);
        }
    }
}