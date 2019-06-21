﻿using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class CustomBillUIViewController : CustomUIViewController
    {
        nfloat _frameWidth, _contentWidth, childWidth, childWidthTitle, childWidthValue;
        public CustomBillUIViewController(IntPtr handle) : base(handle)
        {
            _frameWidth = UIScreen.MainScreen.Bounds.Width;
            _contentWidth = _frameWidth - 36.0F;
            childWidth = _frameWidth - 36.0F;
            childWidthTitle = childWidth * 0.60F;
            childWidthValue = childWidth * 0.40F;
        }
        //Parent Container
        public UIView _viewAccountDetails, _viewCharges, _viewHistory                               //Parent Container
            , _viewBreakdownTitle, _viewCurrentCharges, _viewOutstandingCharges                     //Field Container
            , _viewItemizedBilling, _viewTotalAmountDue, _viewHistoryHeader;

        public UILabel _lblAccountName, _lblAccountNumber, _lblAddress                              // Value Account Details
            , _lblBreakdownHeader, _lblCurrentChargesTitle, _lblOutstandingChargesTitle             //Title Charges
            , _lblTotalDueAmountTitle, _lblHistoryHeader
            , _lblMandatoryPayments, _lblSecurityDeposit                                            //Title Itemized Billing
            , _lblDisconnection, _lblReconnection, _lblStampDuty
            , _lblCurrentChargesValue, _lblOutstandingChargesValue                                  //Value Charges
            , _lblDueDateTitle, _lblAmount
            , _lblMandatoryPaymentsValue, _lblSecurityDepositValue                                  //Value Itemized Billing
            , _lblDisconnectionValue, _lblReconnectionValue, _lblStampDutyValue;
        //Button
        public UIButton _btnPay, _btnBills, _btnPayments;
        //Child Container
        UIView _viewAmount;
        //UIImage
        public UIImageView _imgLeaf;

        //For Normal and Smart Metre
        public void CreateNormalView(bool isItemizedBilling = false)
        {
            CreateAccountDetailsSection();
            CreateChargesSection(false, isItemizedBilling);
            CreateHistorySection();
        }

        public void CreateREView()
        {
            CreateAccountDetailsSection(true);
            CreateChargesSection(true);
            CreateHistorySection();
        }

        public void SetChargesValues(string currentCharge, string outstandingCharge, string amount)
        {
            if (_lblCurrentChargesValue != null)
            {
                _lblCurrentChargesValue.Text = currentCharge;
            }
            if (_lblOutstandingChargesValue != null)
            {
                _lblOutstandingChargesValue.Text = outstandingCharge;
            }
            if (_lblAmount != null)
            {
                _lblAmount.Text = amount;
            }
            RefitAmountToWidget();
        }

        public void SetItemizedBillingValues(string mandatoryPayments, string securityDeposit
            , string disconnectionCharges, string reconnectionCharges, string stampDuty)
        {
            _lblMandatoryPaymentsValue.Text = mandatoryPayments ?? string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, TNBGlobal.DEFAULT_VALUE);
            _lblSecurityDepositValue.Text = securityDeposit ?? string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, TNBGlobal.DEFAULT_VALUE);
            _lblDisconnectionValue.Text = disconnectionCharges ?? string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, TNBGlobal.DEFAULT_VALUE);
            _lblReconnectionValue.Text = reconnectionCharges ?? string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, TNBGlobal.DEFAULT_VALUE);
            _lblStampDutyValue.Text = stampDuty ?? string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, TNBGlobal.DEFAULT_VALUE);
        }

        public CGRect GetHeaderFrame()
        {
            nfloat height = _viewAccountDetails.Frame.Height + _viewCharges.Frame.Height + _viewHistory.Frame.Height;
            return new CGRect(0, 0, _frameWidth, height);
        }

        public void RefitAmountToWidget()
        {
            CGSize newSize = GetLabelSize(_lblAmount, (_frameWidth - 36.0F) * 0.40F, _lblAmount.Frame.Height);
            double newWidth = Math.Ceiling(newSize.Width);
            _lblAmount.Frame = new CGRect(_lblAmount.Frame.X, _lblAmount.Frame.Y, newWidth, _lblAmount.Frame.Height);
            nfloat newContainerWidth = _lblAmount.Frame.Width + _lblAmount.Frame.X;
            _viewAmount.Frame = new CGRect(_viewTotalAmountDue.Frame.Width - newContainerWidth
                , _viewAmount.Frame.Y, newContainerWidth, _viewAmount.Frame.Height);
        }

        public void RefitAccountDetailsToWidget()
        {
            CGSize newSize = GetLabelSize(_lblAddress, _lblAddress.Frame.Width, 64);
            double newHeight = Math.Ceiling(newSize.Height);
            _lblAddress.Frame = new CGRect(_lblAddress.Frame.X, _lblAddress.Frame.Y, _lblAddress.Frame.Width, newHeight);
            nfloat newContainerHeight = newHeight > 32 ? _viewAccountDetails.Frame.Height - 32 + (nfloat)newHeight : _viewAccountDetails.Frame.Height;
            _viewAccountDetails.Frame = new CGRect(_viewAccountDetails.Frame.X, _viewAccountDetails.Frame.Y
                , _viewAccountDetails.Frame.Width, newContainerHeight);
            _viewCharges.Frame = new CGRect(_viewCharges.Frame.X, _viewAccountDetails.Frame.GetMaxY()
                , _viewCharges.Frame.Width, _viewCharges.Frame.Height);
        }

        private void CreateAccountDetailsSection(bool isReAccount = false)
        {
            nfloat widgetY = 16.0F;
            _viewAccountDetails = new UIView(new CGRect(0, 0, _frameWidth, 122))
            {
                BackgroundColor = UIColor.White
            };
            _lblAccountName = GetUILabelField(new CGRect(18, widgetY, _contentWidth, 18)
                , string.Empty, MyTNBFont.MuseoSans14_500, MyTNBColor.TunaGrey());
            widgetY += 18;
            _lblAccountNumber = GetUILabelField(new CGRect(18, widgetY, _contentWidth, 16)
                , string.Empty, MyTNBFont.MuseoSans12_300, MyTNBColor.TunaGrey());
            widgetY += 32;
            _lblAddress = GetUILabelField(new CGRect(18, widgetY, _contentWidth, 32)
                , string.Empty, MyTNBFont.MuseoSans12_300, MyTNBColor.TunaGrey());
            _lblAddress.Lines = 0;

            _viewAccountDetails.AddSubviews(new UIView[] { _lblAccountName, _lblAccountNumber, _lblAddress });

            if (isReAccount)
            {
                _imgLeaf = new UIImageView(new CGRect(_frameWidth - 42, 16, 24, 24))
                {
                    Image = UIImage.FromBundle("IC-RE-Leaf-Green")
                };
                _viewAccountDetails.AddSubview(_imgLeaf);
            }
        }

        private void CreateChargesSection(bool isREAccount = false, bool isItemizedBilling = false)
        {
            nfloat widgetY = 0.0F;
            _viewCharges = new UIView()
            {
                BackgroundColor = UIColor.White
            };
            _viewBreakdownTitle = new UIView(new CGRect(0, widgetY, _frameWidth, 48))
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };
            if (isREAccount)
            {
                widgetY += 48;
                _viewTotalAmountDue = new UIView(new CGRect(18, widgetY, _contentWidth, 64));
                _viewCharges.Frame = new CGRect(0, _viewAccountDetails.Frame.GetMaxY(), _frameWidth, _viewTotalAmountDue.Frame.GetMaxY());
                _viewCharges.AddSubviews(new UIView[] { _viewBreakdownTitle, _viewTotalAmountDue });
            }
            else
            {
                widgetY += 64;
                _viewCurrentCharges = new UIView(new CGRect(18, widgetY, _contentWidth, 16));
                widgetY += 32;
                _viewItemizedBilling = new UIView()
                {
                    // BackgroundColor = UIColor.Cyan
                };
                if (isItemizedBilling)
                {
                    _viewItemizedBilling.Frame = new CGRect(18, widgetY, _contentWidth, 88);
                    AddItemizedBilling();
                    widgetY += 104;
                }
                _viewOutstandingCharges = new UIView(new CGRect(18, widgetY, _contentWidth, 16));
                widgetY += 32;
                UIView viewLine = GenericLine.GetLine(new CGRect(18, widgetY, _contentWidth, 1));
                widgetY += 1;
                _viewTotalAmountDue = new UIView(new CGRect(18, widgetY, _contentWidth, 64));
                widgetY += 64;
                _btnPay = GetUIButton(new CGRect(18, widgetY, _contentWidth, 48), "Bill_Pay");
                _btnPay.Enabled = false;

                nfloat containerViewHeight = widgetY + _btnPay.Frame.Height + 24;
                _viewCharges.Frame = new CGRect(0, _viewAccountDetails.Frame.GetMaxY(), _frameWidth, containerViewHeight);
                _viewCharges.AddSubviews(new UIView[] { _viewBreakdownTitle, _viewCurrentCharges, _viewItemizedBilling
                , _viewOutstandingCharges, viewLine, _viewTotalAmountDue, _btnPay });
            }
            AddChildrenViews(isREAccount, isItemizedBilling);
        }

        private void AddChildrenViews(bool isREAccount, bool isItemizedBilling)
        {
            _lblBreakdownHeader = GetUILabelField(new CGRect(18, 24, _frameWidth - 36, 18), "Bill_BillDetails"
                , MyTNBFont.MuseoSans16, MyTNBColor.PowerBlue);

            if (!isREAccount)
            {
                _lblCurrentChargesTitle = GetUILabelField(new CGRect(0, 0, childWidthTitle, 16), "Bill_CurrentCharges");
                _lblCurrentChargesValue = GetUILabelField(new CGRect(_lblCurrentChargesTitle.Frame.Width, 0
                    , childWidthValue, 16), string.Empty, UITextAlignment.Right);

                _lblOutstandingChargesTitle = GetUILabelField(new CGRect(0, 0, childWidthTitle, 16), "Bill_OutstandingCharges");
                _lblOutstandingChargesValue = GetUILabelField(new CGRect(_lblCurrentChargesTitle.Frame.Width, 0
                    , childWidthValue, 16), string.Empty, UITextAlignment.Right);

                _viewCurrentCharges.AddSubviews(new UIView[] { _lblCurrentChargesTitle, _lblCurrentChargesValue });
                _viewOutstandingCharges.AddSubviews(new UIView[] { _lblOutstandingChargesTitle, _lblOutstandingChargesValue });
            }

            _lblTotalDueAmountTitle = GetUILabelField(new CGRect(0, 16, childWidthTitle, 18), "Common_TotalAmountDue"
                , MyTNBFont.MuseoSans14_500, MyTNBColor.TunaGrey());
            _lblDueDateTitle = GetUILabelField(new CGRect(0, 34, childWidthTitle, 14), TNBGlobal.EMPTY_DATE
                , MyTNBFont.MuseoSans11_300, MyTNBColor.SilverChalice);
            _viewAmount = new UIView(new CGRect(childWidthTitle, 0, childWidthValue, 48));
            UILabel lblCurrency = GetUILabelField(new CGRect(0, 30, 24, 18), string.Format("{0} "
                , TNBGlobal.UNIT_CURRENCY), MyTNBFont.MuseoSans14, MyTNBColor.TunaGrey(), UITextAlignment.Right);
            _lblAmount = GetUILabelField(new CGRect(24, 24, 75, 24), TNBGlobal.DEFAULT_VALUE
                , MyTNBFont.MuseoSans24, MyTNBColor.TunaGrey(), UITextAlignment.Right);
            _lblAmount.Lines = 0;

            _viewAmount.AddSubviews(new UIView[] { lblCurrency, _lblAmount });

            _viewBreakdownTitle.AddSubview(_lblBreakdownHeader);

            _viewTotalAmountDue.AddSubviews(new UIView[] { _lblTotalDueAmountTitle, _lblDueDateTitle, _viewAmount });

            RefitAmountToWidget();
        }

        private void AddItemizedBilling()
        {
            nfloat widgetY = 0;
            _lblMandatoryPayments = GetUILabelField(new CGRect(0, widgetY, childWidthTitle, 16), "Bill_MandatoryPayments");
            _lblMandatoryPaymentsValue = GetUILabelField(new CGRect(_lblMandatoryPayments.Frame.Width, widgetY
                , childWidthValue, 16), string.Empty, UITextAlignment.Right);
            widgetY += 32;
            _lblSecurityDeposit = GetSubKeyValueLabel(new CGRect(0, widgetY, childWidthTitle, 14), "Bill_SecurityDeposit");
            _lblSecurityDepositValue = GetSubKeyValueLabel(new CGRect(_lblSecurityDeposit.Frame.Width, widgetY
                , childWidthValue, 14), string.Empty, UITextAlignment.Right);
            widgetY += 14;
            _lblDisconnection = GetSubKeyValueLabel(new CGRect(0, widgetY, childWidthTitle, 14), "Bill_DisconnectionCharges");
            _lblDisconnectionValue = GetSubKeyValueLabel(new CGRect(_lblDisconnection.Frame.Width, widgetY
                , childWidthValue, 14), string.Empty, UITextAlignment.Right);
            widgetY += 14;
            _lblReconnection = GetSubKeyValueLabel(new CGRect(0, widgetY, childWidthTitle, 14), "Bill_ReconnectionCharges");
            _lblReconnectionValue = GetSubKeyValueLabel(new CGRect(_lblReconnection.Frame.Width, widgetY
                , childWidthValue, 14), string.Empty, UITextAlignment.Right);
            widgetY += 14;
            _lblStampDuty = GetSubKeyValueLabel(new CGRect(0, widgetY, childWidthTitle, 14), "Bill_StampDuty");
            _lblStampDutyValue = GetSubKeyValueLabel(new CGRect(_lblStampDuty.Frame.Width, widgetY
                , childWidthValue, 14), string.Empty, UITextAlignment.Right);

            _viewItemizedBilling.AddSubviews(new UIView[] { _lblMandatoryPayments, _lblSecurityDeposit
                , _lblDisconnection, _lblReconnection, _lblStampDuty,_lblMandatoryPaymentsValue
                , _lblSecurityDepositValue, _lblDisconnectionValue, _lblReconnectionValue, _lblStampDutyValue  });
        }

        private void CreateHistorySection()
        {
            _viewHistoryHeader = new UIView(new CGRect(0, 0, _frameWidth, 48))
            {
                BackgroundColor = MyTNBColor.SectionGrey
            };

            _lblHistoryHeader = GetUILabelField(new CGRect(18, 24, _frameWidth - 36, 18), string.Empty
              , MyTNBFont.MuseoSans16, MyTNBColor.PowerBlue);

            UIView viewToggle = new UIView(new CGRect(0, _viewHistoryHeader.Frame.GetMaxY(), _frameWidth, 50));

            nfloat btnWidth = _frameWidth / 2;
            _btnBills = GetUIButton(new CGRect(0, 0, btnWidth, 48), "Bill_Bills");
            _btnBills.BackgroundColor = UIColor.White;
            _btnBills.Tag = 0;
            MakeTopCornerRadius(_btnBills);

            _btnPayments = GetUIButton(new CGRect(_btnBills.Frame.GetMaxX(), 0, btnWidth, 48), "Common_Payment");
            _btnPayments.BackgroundColor = MyTNBColor.SelectionGrey;
            _btnPayments.Tag = 1;
            MakeTopCornerRadius(_btnPayments);

            _viewHistory = new UIView(new CGRect(0, _viewCharges.Frame.GetMaxY(), _frameWidth, 98))
            {
                BackgroundColor = UIColor.White
            };

            _viewHistoryHeader.AddSubview(_lblHistoryHeader);
            viewToggle.AddSubviews(new UIView[] { _btnBills, _btnPayments });
            _viewHistory.AddSubviews(new UIView[] { _viewHistoryHeader, viewToggle });
        }

        private UILabel GetSubKeyValueLabel(CGRect cgRect, string key, UITextAlignment textAlignment = UITextAlignment.Left)
        {
            return GetUILabelField(cgRect, key, MyTNBFont.MuseoSans11_300, MyTNBColor.SilverChalice, textAlignment);
        }
    }
}