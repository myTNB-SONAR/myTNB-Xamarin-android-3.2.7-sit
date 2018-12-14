using System;
using System.Drawing;
using System.Globalization;
using CoreAnimation;
using CoreGraphics;
using myTNB.Home.Components;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class BillAndPaymentComponent
    {
        UIView _parentView;
        UIView _viewPaymentContainer;
        public UIButton _btnViewBill;
        public UIButton _btnPay;
        UILabel _lblPaymentTitle;
        UIView _viewAmount;
        UILabel _lblCurrency;
        UILabel _lblAmount;
        UILabel _lblDate;
        UIImageView _mask;
        CGRect origViewFrame;
        int paymentViewHiddenState = -1;
        float height = 136;

        const string CURRENCY = "RM";

        public ActivityIndicatorComponent _activity;

        public BillAndPaymentComponent(UIView view)
        {
            _parentView = view;
        }

        internal void CreateComponent(double yLocation)
        {
            _viewPaymentContainer = new UIView(new CGRect(0, yLocation, _parentView.Frame.Width, height));
            _viewPaymentContainer.BackgroundColor = UIColor.White;
            _viewPaymentContainer.Alpha = 1f;
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
            _lblPaymentTitle = new UILabel(new CGRect(17, 16, _viewPaymentContainer.Frame.Width - 20, 18));
            _lblPaymentTitle.Font = myTNBFont.MuseoSans14();
            _lblPaymentTitle.TextColor = myTNBColor.TunaGrey();
            _lblPaymentTitle.TextAlignment = UITextAlignment.Left;
            _lblPaymentTitle.Text = "Total Amount Due";
            _viewPaymentContainer.AddSubview(_lblPaymentTitle);

            _lblDate = new UILabel(new CGRect(17, 34, _viewPaymentContainer.Frame.Width - 20, 14));
            _lblDate.Font = myTNBFont.MuseoSans9();
            _lblDate.TextColor = myTNBColor.SilverChalice();
            _lblDate.TextAlignment = UITextAlignment.Left;
            _viewPaymentContainer.AddSubview(_lblDate);

            _viewAmount = new UIView(new CGRect(0, 20, 0, 24));
            _lblCurrency = new UILabel(new CGRect(0, 6, 24, 18));
            _lblCurrency.Font = myTNBFont.MuseoSans14();
            _lblCurrency.TextColor = myTNBColor.TunaGrey();
            _lblCurrency.TextAlignment = UITextAlignment.Right;
            _lblCurrency.Text = CURRENCY;
            _viewAmount.AddSubview(_lblCurrency);

            _lblAmount = new UILabel(new CGRect(24, 0, 75, 24));
            _lblAmount.Font = myTNBFont.MuseoSans24();
            _lblAmount.TextColor = myTNBColor.TunaGrey();
            _lblAmount.TextAlignment = UITextAlignment.Right;
            _lblAmount.Text = "0.00";
            _viewAmount.AddSubview(_lblAmount);

            _viewPaymentContainer.AddSubview(_viewAmount);

            AdjustFrames();
        }

        internal void CreatePaymentButtons()
        {
            _btnViewBill = new UIButton(UIButtonType.Custom);
            _btnViewBill.Frame = new CGRect(17, 64, (_viewPaymentContainer.Frame.Width / 2) - 19, 48);
            _btnViewBill.Layer.CornerRadius = 4;
            _btnViewBill.Layer.BorderColor = myTNBColor.FreshGreen().CGColor;
            _btnViewBill.Layer.BorderWidth = 1;
            _btnViewBill.SetTitle("View Bill", UIControlState.Normal);
            _btnViewBill.Font = myTNBFont.MuseoSans16();
            _btnViewBill.SetTitleColor(myTNBColor.FreshGreen(), UIControlState.Normal);
            _viewPaymentContainer.AddSubview(_btnViewBill);

            _btnPay = new UIButton(UIButtonType.Custom);
            _btnPay.Frame = new CGRect(_btnViewBill.Frame.Width + 21, 64, (_viewPaymentContainer.Frame.Width / 2) - 19, 48);
            _btnPay.Layer.CornerRadius = 4;
            _btnPay.Layer.BackgroundColor = myTNBColor.FreshGreen().CGColor;
            _btnPay.Layer.BorderColor = myTNBColor.FreshGreen().CGColor;
            _btnPay.Layer.BorderWidth = 1;
            _btnPay.SetTitle("Pay", UIControlState.Normal);
            _btnPay.Font = myTNBFont.MuseoSans16();
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
            _btnPay.Frame = new CGRect(x, 58, width, 48);
        }

        public void SetREAccountButton()
        {
            _btnPay.Hidden = true;
            _btnViewBill.SetTitle("View Payment Advice", UIControlState.Normal);
            _btnViewBill.Frame = new CGRect(18, 64, _viewPaymentContainer.Frame.Width - 36, 48);
        }

        public void SetPayButtonEnable(bool isEnable)
        {
            _btnPay.Enabled = isEnable;
            _btnPay.BackgroundColor = isEnable ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
            _btnPay.Layer.BorderColor = isEnable ? myTNBColor.FreshGreen().CGColor : myTNBColor.SilverChalice().CGColor;
        }

        public UIView GetUI()
        {
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

        public void SetAmount(string amount)
        {
            if (!string.IsNullOrEmpty(amount))
            {
                double value = Double.Parse(amount, CultureInfo.InvariantCulture);
                string valueForDisplay = value.ToString("N2", CultureInfo.InvariantCulture);
                _lblAmount.Text = valueForDisplay;
                AdjustFrames();
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
                return;

            paymentViewHiddenState = isHiddenInt;

            UIView.Animate(0.3, 0, UIViewAnimationOptions.ShowHideTransitionViews
                , () =>
                {
                    if (isHidden)
                    {
                        var temp = origViewFrame;
                        temp.Y = _parentView.Frame.Height;

                        _viewPaymentContainer.Frame = temp;
                    }
                    else
                    {
                        _viewPaymentContainer.Frame = origViewFrame;
                    }
                        
                }
                , () =>
                {
                }
            );

        }

        internal void AdjustFrames()
        {
            CGSize newSize = LabelHelper.GetLabelSize(_lblAmount, _viewPaymentContainer.Frame.Width / 2, _lblAmount.Frame.Height);
            double newWidth = Math.Ceiling(newSize.Width);
            _lblAmount.Frame = new CGRect(24, 0, newWidth, _lblAmount.Frame.Height);
            _viewAmount.Frame = new CGRect(_viewPaymentContainer.Frame.Width - (newWidth + 24 + 17), 23, newWidth + 24, 24);
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
    }
}