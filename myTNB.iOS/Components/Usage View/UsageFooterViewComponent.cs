using System;
using System.Globalization;
using CoreGraphics;
using Foundation;
using UIKit;

//Created by Syahmi ICS 05052020
using myTNB.Home.Bill;
using myTNB.Model;

namespace myTNB
{
    public class UsageFooterViewComponent : BaseComponent
    {
        UIView _parentView, _containerView, _shimmerParent, _shimmerContent, _viewTitle, _viewAmount, _viewDate;
        public UIButton _btnViewBill, _btnPay;
        UILabel _lblPaymentTitle, _lblAmount, _lblDate, _lblCommon;
        nfloat _width, _viewHeight, _yPos;

        //Created by Syahmi ICS 05052020
        public CustomUIView _eppToolTipsView;
        UIView _containerEppView;
        DueAmountDataModel dueData;
        public bool ShowEppToolTip;

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
            CreateTooltipsView();   //Created by Syahmi ICS 05052020
            CreatePaymentButtons();
            UpdateUI(true);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        public UIView GetView()
        {
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
                Lines = 0,
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
            //nfloat yPos = GetScaledHeight(72f);

            //Created by Syahmi ICS 05052020
            nfloat yPos;

            if (dueData != null && dueData.ShowEppToolTip.Equals(true))
            {
                yPos = GetScaledHeight(96f);
            }
            else if (dueData != null && dueData.ShowEppToolTip.Equals(false))
            {
                yPos = GetScaledHeight(72f);
            }
            else
            {
                yPos = GetScaledHeight(72f);
            }

            _btnViewBill = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(BaseMarginWidth16, yPos, (_width / 2) - GetScaledWidth(18f), GetScaledHeight(48f))
            };
            _btnViewBill.Layer.CornerRadius = GetScaledHeight(4f);
            _btnViewBill.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnViewBill.Layer.BorderWidth = GetScaledHeight(1f);
            _btnViewBill.SetTitle(GetI18NValue(UsageConstants.I18N_ViewDetails), UIControlState.Normal);
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
            _btnPay.SetTitle(GetI18NValue(UsageConstants.I18N_Pay), UIControlState.Normal);
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

        public bool IsPayEnable
        {
            set
            {
                if (_btnPay != null)
                {
                    _btnPay.Enabled = value;
                    _btnPay.BackgroundColor = value ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
                    _btnPay.Layer.BorderColor = (value ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice).CGColor;
                }
            }
        }

        //Created by Syahmi ICS 05052020
        public bool IsShowEppToolTip
        {
            set
            {
                if (_containerEppView != null)
                {
                    _containerEppView.Hidden = !value;
                    ShowEppToolTip = value;
                }
            }
        }

        public void SetAmount(double amount, bool isPendingPayment = false)
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
            if (isPendingPayment)
            {
                _lblAmount.TextColor = MyTNBColor.LightOrange;
            }
            AdjustLabels(amount, isPendingPayment);
        }

        private void AdjustLabels(double amount, bool isPendingPayment = false)
        {
            if (_containerView != null)
            {
                if (amount > 0)
                {
                    _lblPaymentTitle.Hidden = false;
                    _lblPaymentTitle.Text = GetI18NValue(UsageConstants.I18N_NeedToPay);
                    _lblDate.Hidden = false;
                    _lblCommon.Hidden = true;
                }
                else
                {
                    _lblPaymentTitle.Hidden = true;
                    _lblPaymentTitle.Text = string.Empty;
                    _lblDate.Hidden = true;
                    _lblCommon.Hidden = false;
                    _lblCommon.Text = amount < 0 ? GetI18NValue(UsageConstants.I18N_PaidExtra) : GetI18NValue(UsageConstants.I18N_ClearedAllBills);
                    nfloat width = _lblCommon.GetLabelWidth(_width * .8F);
                    _lblCommon.Frame = new CGRect(_lblCommon.Frame.Location, new CGSize(width, _lblCommon.Frame.Height));
                }
            }

            if (isPendingPayment)
            {
                _lblPaymentTitle.Hidden = true;
                _lblPaymentTitle.Text = string.Empty;
                _lblDate.Hidden = true;
                _lblCommon.Hidden = false;
                UIStringAttributes stringAttributes = new UIStringAttributes
                {
                    Font = TNBFont.MuseoSans_14_500,
                    ForegroundColor = MyTNBColor.GreyishBrown,
                    ParagraphStyle = new NSMutableParagraphStyle() { LineSpacing = 3.0f, Alignment = UITextAlignment.Left }
                };
                var text = GetCommonI18NValue(Constants.Common_PaymentPendingMsg);
                var AttributedText = new NSMutableAttributedString(text);
                AttributedText.AddAttributes(stringAttributes, new NSRange(0, text.Length));
                _lblCommon.AttributedText = AttributedText;

                nfloat width = _containerView.Frame.Width / 2 - (BaseMarginWidth16 * 2);
                nfloat height = GetScaledHeight(40F);
                nfloat xPos = BaseMarginWidth16;
                nfloat yPos = BaseMarginHeight16;
                _lblCommon.Frame = new CGRect(xPos, yPos, width, height);
            }
        }

        public void SetDate(string date)
        {
            if (!string.IsNullOrEmpty(date) && !string.IsNullOrWhiteSpace(date))
            {
                if (_lblDate != null)
                {
                    string formattedDate = DateHelper.GetFormattedDate(date, "dd MMM yyyy");
                    _lblDate.Text = GetI18NValue(UsageConstants.I18N_By) + " " + formattedDate;
                }
            }
        }

        public void SetRefreshState()
        {
            if (_containerView != null)
            {
                _lblPaymentTitle.Hidden = false;
                _lblPaymentTitle.Text = GetI18NValue(UsageConstants.I18N_NeedToPay);
                _lblDate.Hidden = false;
                _lblCommon.Hidden = true;
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

        //Created by Syahmi ICS 05052020
        public void CreateTooltipsView()
        {
            dueData = AmountDueCache.GetDues(DataManager.DataManager.SharedInstance.SelectedAccount?.accNum);
            if (dueData != null && dueData.ShowEppToolTip.Equals(true))
            {
                _containerEppView = new UIView(new CGRect(0, (GetYLocationFromFrame(_lblDate.Frame, 8)), _width, GetScaledHeight(24)))
                {
                    BackgroundColor = UIColor.White,
                    Hidden = true
                };
                _containerEppView.AddSubview(GetEPPTooltipView(0));
                _containerView.AddSubview(_containerEppView);
            }
        }

        public CustomUIView GetEPPTooltipView(nfloat yLoc)
        {
            _eppToolTipsView = new CustomUIView(new CGRect(0, yLoc, _width, GetScaledHeight(24)))
            {
                BackgroundColor = UIColor.White,
            };
            CustomUIView viewInfo = new CustomUIView(new CGRect(BaseMarginWidth16, 0, _width - (GetScaledWidth(16) * 2), GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            UIImageView imgView = new UIImageView(new CGRect(GetScaledWidth(4)
                , GetScaledHeight(4), GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_InfoBlue)
            };
            UILabel lblDescription = new UILabel(new CGRect(GetScaledWidth(28)
                , GetScaledHeight(4), _eppToolTipsView.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_11_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetCommonI18NValue("eppToolTipTitle")

            };
            viewInfo.Layer.CornerRadius = GetScaledHeight(12);
            viewInfo.AddSubviews(new UIView[] { imgView, lblDescription });
            _eppToolTipsView.AddSubview(viewInfo);

            return _eppToolTipsView;
        }
    }
}