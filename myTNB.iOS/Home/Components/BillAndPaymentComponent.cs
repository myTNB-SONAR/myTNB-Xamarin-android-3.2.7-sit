﻿using System;
using System.Globalization;
using CoreGraphics;
using myTNB.Home.Components;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class BillAndPaymentComponent
    {
        readonly UIView _parentView;
        UIView _viewPaymentContainer, _viewAmount, _infoView;
        public UIButton _btnViewBill, _btnPay;
        UILabel _lblPaymentTitle, _lblCurrency, _lblAmount, _lblDate, _lblInfo;
        UIImageView _mask;
        CGRect origViewFrame;
        int paymentViewHiddenState = -1;
        float height = 136f;
        float adjustment;

        public ActivityIndicatorComponent _activity;

        public BillAndPaymentComponent(UIView view)
        {
            _parentView = view;
        }

        internal void CreateComponent(double yLocation)
        {

            _viewPaymentContainer = new UIView(new CGRect(0, yLocation, _parentView.Frame.Width, height))
            {
                BackgroundColor = UIColor.White,
                Alpha = 1f
            };
            origViewFrame = _viewPaymentContainer.Frame;
            paymentViewHiddenState = -1;

            //AddComponents
            CreatePaymentLabels();
            CreatePaymentButtons();

            int maskHeight = 48;
            _mask = new UIImageView(new CGRect(0, maskHeight * -1, _viewPaymentContainer.Frame.Width, maskHeight))
            {
                Image = UIImage.FromBundle("Mask-View"),
                BackgroundColor = UIColor.Clear
            };
            _mask.Hidden = true;
            _viewPaymentContainer.AddSubview(_mask);
            _parentView.AddSubview(_viewPaymentContainer);
        }

        internal void CreatePaymentLabels()
        {
            _lblPaymentTitle = new UILabel(new CGRect(17, 16 + adjustment, _viewPaymentContainer.Frame.Width - 20, 18))
            {
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Left,
                Text = "Common_AmountDue".Translate()
            };

            _lblDate = new UILabel(new CGRect(17, _lblPaymentTitle.Frame.GetMaxY() + 4, _viewPaymentContainer.Frame.Width - 20, 14))
            {
                Font = MyTNBFont.MuseoSans12_300,
                TextColor = MyTNBColor.SilverChalice,
                TextAlignment = UITextAlignment.Left
            };

            //_viewAmount = new UIView(new CGRect(0, 23 + adjustment, 0, 24));
            _viewAmount = new UIView(new CGRect(0, 10 + adjustment, 0, 24));
            _lblCurrency = new UILabel(new CGRect(0, 6, 24, 18))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Right,
                Text = TNBGlobal.UNIT_CURRENCY
            };

            _lblAmount = new UILabel(new CGRect(24, 0, 75, 24))
            {
                Font = MyTNBFont.MuseoSans24_300,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Right,
                Text = TNBGlobal.DEFAULT_VALUE
            };

            _infoView = new UIView(new CGRect(_viewPaymentContainer.Frame.Width - (_viewPaymentContainer.Frame.Width * .30) - 18
                , _lblPaymentTitle.Frame.GetMaxY() + 4, _viewPaymentContainer.Frame.Width * .30, 16))
            {
                Hidden = true
            };
            _lblInfo = new UILabel(new CGRect(0, 0, _infoView.Frame.Width, 16))
            {
                Font = MyTNBFont.MuseoSans12_500,
                TextColor = MyTNBColor.PowerBlue,
                TextAlignment = UITextAlignment.Right,
                Text = "Component_WhyThisAmount".Translate()
            };
            _infoView.AddSubview(_lblInfo);

            _viewAmount.AddSubviews(new UIView[] { _lblCurrency, _lblAmount });
            _viewPaymentContainer.AddSubviews(new UIView[] { _lblPaymentTitle, _lblDate, _viewAmount, _infoView });

            AdjustFrames();
        }

        internal void CreatePaymentButtons()
        {
            _btnViewBill = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(17, _lblDate.Frame.GetMaxY() + 18, (_viewPaymentContainer.Frame.Width / 2) - 19, 48)
            };
            _btnViewBill.Layer.CornerRadius = 4;
            _btnViewBill.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnViewBill.Layer.BorderWidth = 1;
            _btnViewBill.SetTitle("Component_CurrentBill".Translate(), UIControlState.Normal);
            _btnViewBill.Font = MyTNBFont.MuseoSans16_500;
            _btnViewBill.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _viewPaymentContainer.AddSubview(_btnViewBill);

            _btnPay = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(_btnViewBill.Frame.Width + 21, _lblDate.Frame.GetMaxY() + 18, (_viewPaymentContainer.Frame.Width / 2) - 19, 48)
            };
            _btnPay.Layer.CornerRadius = 4;
            _btnPay.Layer.BackgroundColor = MyTNBColor.FreshGreen.CGColor;
            _btnPay.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnPay.Layer.BorderWidth = 1;
            _btnPay.SetTitle("Common_Pay".Translate(), UIControlState.Normal);
            _btnPay.Font = MyTNBFont.MuseoSans16_500;
            _viewPaymentContainer.AddSubview(_btnPay);
        }

        public void ToggleBillAndPayVisibility(bool payOnly)
        {
            _btnViewBill.Hidden = payOnly;
            double x = _btnViewBill.Frame.Width + 21;
            double width = (_viewPaymentContainer.Frame.Width / 2) - 19;
            if (payOnly)
            {
                x = 17;
                width = _viewPaymentContainer.Frame.Width - 34;
            }
            _btnPay.Frame = new CGRect(x, _lblDate.Frame.GetMaxY() + 12, width, 48);
        }

        public void SetREAccountButton()
        {
            _btnPay.Hidden = true;
            _btnViewBill.SetTitle("Component_ViewPaymentAdvice".Translate(), UIControlState.Normal);
            _btnViewBill.Frame = new CGRect(18, _lblDate.Frame.GetMaxY() + 12, _viewPaymentContainer.Frame.Width - 36, 48);
        }

        /// <summary>
        /// Sets the pay button enabled property.
        /// </summary>
        /// <param name="isEnable">If set to <c>true</c> is enable.</param>
        public void SetPayButtonEnable(bool isEnable)
        {
            _btnPay.Enabled = isEnable;
            _btnPay.BackgroundColor = isEnable ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
            _btnPay.Layer.BorderColor = isEnable ? MyTNBColor.FreshGreen.CGColor : MyTNBColor.SilverChalice.CGColor;
        }

        /// <summary>
        /// Sets the bill button enabled property.
        /// </summary>
        /// <param name="isEnable">If set to <c>true</c> is enable.</param>
        public void SetBillButtonEnable(bool isEnable)
        {
            _btnViewBill.Enabled = isEnable;
            _btnViewBill.SetTitleColor(isEnable ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice, UIControlState.Normal);
            _btnViewBill.Layer.BorderColor = isEnable ? MyTNBColor.FreshGreen.CGColor : MyTNBColor.SilverChalice.CGColor;
        }

        public UIView GetUI()
        {
            adjustment = 0;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                adjustment = 20f;
            }
            else if (DeviceHelper.IsIphone6UpResolution())
            {
                adjustment = 10f;
            }

            height = height + (adjustment * 2);

            var yLocation = _parentView.Bounds.Height - height;
            return GetUI(yLocation);
        }

        public UIView GetUI(double yLocation)
        {
            CreateComponent(yLocation);
            _activity = new ActivityIndicatorComponent(_viewPaymentContainer);
            _activity.Show();
            return _viewPaymentContainer;
        }

        public void SetPaymentTitle(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                _lblPaymentTitle.Text = title;
            }
        }

        public void SetDateDueVisibility(bool isHidden)
        {
            _lblDate.Hidden = isHidden;
        }

        public void SetDateDue(string dateString)
        {
            _lblDate.Text = dateString;
        }

        public void SetCurrency(string currency)
        {
            if (!string.IsNullOrEmpty(currency))
            {
                _lblCurrency.Text = currency;
            }
        }

        /// <summary>
        /// Sets the amount.
        /// </summary>
        /// <param name="amount">Amount.</param>
        /// <param name="isREAccount">If set to <c>true</c> is REA ccount.</param>
        public void SetAmount(string amount, bool isREAccount = false)
        {
            if (!string.IsNullOrEmpty(amount))
            {
                if (double.TryParse(amount, out double value))
                {
                    if (isREAccount)
                    {
                        value = ChartHelper.UpdateValueForRE(value);
                    }

                    string valueForDisplay = value.ToString("N2", CultureInfo.InvariantCulture);
                    _lblAmount.Text = valueForDisplay;
                    AdjustFrames();
                }
            }
        }

        public void SetViewBillButtonTitle(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                _btnViewBill.SetTitle(title, UIControlState.Normal);
            }
        }

        public void SetPayButtonTitle(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                _btnPay.SetTitle(title, UIControlState.Normal);
            }
        }

        public void SetMaskHidden(bool isHidden)
        {
            _mask.Hidden = isHidden;
        }

        public void SetComponentHidden(bool isHidden)
        {
            int isHiddenInt = isHidden ? 0 : 1;
            if (paymentViewHiddenState != -1 && isHiddenInt == paymentViewHiddenState)
            {
                return;
            }

            paymentViewHiddenState = isHiddenInt;

            UIView.Animate(0.3, 0, UIViewAnimationOptions.ShowHideTransitionViews
                , () =>
                {
                    if (isHidden)
                    {
                        var temp = origViewFrame;
                        temp.Y = _parentView.Frame.Height;

                        _viewPaymentContainer.Frame = temp;
                        SetMaskHidden(true);
                    }
                    else
                    {
                        _viewPaymentContainer.Frame = origViewFrame;
                        SetMaskHidden(false);
                    }
                }
                , () => { }
            );
        }

        internal void AdjustFrames()
        {
            CGSize newSize = LabelHelper.GetLabelSize(_lblAmount, _viewPaymentContainer.Frame.Width / 2, _lblAmount.Frame.Height);
            double newWidth = Math.Ceiling(newSize.Width);
            _lblAmount.Frame = new CGRect(24, 0, newWidth, _lblAmount.Frame.Height);
            _viewAmount.Frame = new CGRect(_viewPaymentContainer.Frame.Width - (newWidth + 24 + 17), 23 + adjustment, newWidth + 24, 24);
        }

        /// <summary>
        /// Sets the frame based on meter type.
        /// </summary>
        /// <param name="isNormalMeter">If set to <c>true</c> is normal meter.</param>
        public void SetFrameByMeterType(bool isNormalMeter)
        {
            float yLocation = 0;

            if (isNormalMeter && DeviceHelper.IsIphoneXUpResolution())
            {
                yLocation = (float)_parentView.Bounds.Height - height - 80f;
            }
            else
            {
                yLocation = (float)_parentView.Bounds.Height - height;
            }

            var newFrame = _viewPaymentContainer.Frame;
            newFrame.Y = yLocation;
            _viewPaymentContainer.Frame = newFrame;
            origViewFrame = newFrame;
        }

        public void DisplayInfoToolTip(string message, Action action = null)
        {
            if (!string.IsNullOrEmpty(message) && !string.IsNullOrWhiteSpace(message))
            {
                _lblInfo.Text = message;
            }
            if (action != null)
            {
                _infoView.AddGestureRecognizer(new UITapGestureRecognizer(action));
            }
            _infoView.Hidden = false;
            _viewAmount.Frame = new CGRect(_viewAmount.Frame.X, 10 + adjustment, _viewAmount.Frame.Width, _viewAmount.Frame.Height);
        }
    }
}