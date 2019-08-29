using CoreGraphics;
using myTNB.Home.Bill;
using myTNB.Model;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using UIKit;

namespace myTNB
{
    public partial class BillDetailsViewController : CustomUIViewController
    {
        private UIView _viewDetails, _viewTitleSection, _viewBreakdown, _viewLine, _viewPayment, _toolTipParentView;
        private CustomUIView _viewMandatory, _viewTooltip;
        private UIScrollView _uiScrollView;
        private bool isMandatoryExpanded;

        public AccountChargesDataModel Charges { set; private get; }
        public BillDetailsViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = BillConstants.Pagename_BillDetails;
            NavigationController.SetNavigationBarHidden(false, true);
            base.ViewDidLoad();
            View.BackgroundColor = MyTNBColor.LightGrayBG;
            SetNavigation();
            AddDetails();
            AddSectionTitle();
            AddBreakdown();
            SetEvents();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        private void SetNavigation()
        {
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(BillConstants.IMG_BackIcon)
            , UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            UIBarButtonItem btnInfo = new UIBarButtonItem(UIImage.FromBundle(BillConstants.IMG_Info)
            , UIBarButtonItemStyle.Done, (sender, e) =>
            {
                PrepareToolTipView();
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            NavigationItem.RightBarButtonItem = btnInfo;
            Title = GetI18NValue(BillConstants.I18N_NavTitle);
        }

        #region
        private void PrepareToolTipView()
        {
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            nfloat height = currentWindow.Frame.Height;
            if (_toolTipParentView != null)
            {
                _toolTipParentView.RemoveFromSuperview();
            }
            _toolTipParentView = new UIView(new CGRect(0, 0, ViewWidth, height))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            currentWindow.AddSubview(_toolTipParentView);
            PaginatedTooltipComponent tooltipComponent = new PaginatedTooltipComponent(_toolTipParentView);
            //tooltipComponent.SetSSMRData(pageData);
            //tooltipComponent.SetPreviousMeterData(_previousMeterList);
            _toolTipParentView.AddSubview(tooltipComponent.GetBillDetailsTooltip());
            tooltipComponent.SetGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                MakeToolTipVisible(false);
            }));
            _toolTipParentView.Hidden = false;
        }

        private void MakeToolTipVisible(bool isVisible)
        {
            if (_toolTipParentView != null)
            {
                _toolTipParentView.Hidden = !isVisible;
            }
        }
        #endregion

        private void AddDetails()
        {
            _uiScrollView = new UIScrollView(new CGRect(0, 0, ViewWidth, ViewHeight));
            View.AddSubview(_uiScrollView);
            _viewDetails = new UIView { BackgroundColor = UIColor.White };
            UILabel lblAccountName = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_14_500,
                LineBreakMode = UILineBreakMode.TailTruncation,
                Text = DataManager.DataManager.SharedInstance.SelectedAccount.accountNickName
            };
            UILabel lblAddress = new UILabel(new CGRect(BaseMargin, GetYLocationFromFrame(lblAccountName.Frame, 8)
                , BaseMarginedWidth, GetScaledHeight(32)))
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_12_500,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = DataManager.DataManager.SharedInstance.SelectedAccount.accountStAddress
            };
            nfloat height = lblAddress.GetLabelHeight(GetScaledHeight(100));
            lblAddress.Frame = new CGRect(lblAddress.Frame.Location, new CGSize(lblAddress.Frame.Width, height));
            _viewDetails.AddSubviews(new UIView[] { lblAccountName, lblAddress });
            _viewDetails.Frame = new CGRect(0, 0, ViewWidth, lblAddress.Frame.GetMaxY() + GetScaledHeight(16));
            _uiScrollView.AddSubview(_viewDetails);
        }

        private void AddSectionTitle()
        {
            _viewTitleSection = new UIView(new CGRect(0, _viewDetails.Frame.GetMaxY(), ViewWidth, GetScaledHeight(48)));
            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(24)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(BillConstants.I18N_BillDetails)
            };
            _viewTitleSection.AddSubview(lblSectionTitle);
            _uiScrollView.AddSubview(_viewTitleSection);
        }

        private void AddBreakdown()
        {
            _viewBreakdown = new UIView { BackgroundColor = UIColor.White };
            bool isOutstandingOverpaid = Charges.AccountCharges[0].OutstandingCharges < 0;
            string outstandingTilte = isOutstandingOverpaid ? GetI18NValue(BillConstants.I18N_PaidExtra)
                : GetI18NValue(BillConstants.I18N_OutstandingCharges);
            UIView viewOutstanding = GetCommonLabelView(GetScaledHeight(16), outstandingTilte
                , Math.Abs(Charges.AccountCharges[0].OutstandingCharges).ToString("N2", CultureInfo.InvariantCulture), isOutstandingOverpaid);
            UIView viewMonthBill = GetCommonLabelView(GetYLocationFromFrame(viewOutstanding.Frame, 16), GetI18NValue(BillConstants.I18N_BillThisMonth)
                , Math.Abs(Charges.AccountCharges[0].CurrentCharges).ToString("N2", CultureInfo.InvariantCulture));

            _viewMandatory = GetMandatoryView(GetYLocationFromFrame(viewMonthBill.Frame, 16));

            nfloat lineYLoc = GetYLocationFromFrame(HasMandatory ? _viewMandatory.Frame : viewMonthBill.Frame, 16);
            _viewLine = new UIView(new CGRect(BaseMargin, lineYLoc, BaseMarginedWidth, GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };

            _viewPayment = GetPaymentDetails(GetYLocationFromFrame(_viewLine.Frame, 20));
            _viewTooltip = GetTooltipView(GetYLocationFromFrame(_viewPayment.Frame, 16));


            _viewBreakdown.AddSubviews(new UIView[] { viewOutstanding, viewMonthBill, _viewLine, _viewPayment });

            nfloat breakDownViewHeight = _viewPayment.Frame.GetMaxY();
            if (HasMandatory)
            {
                _viewBreakdown.AddSubviews(new UIView[] { _viewMandatory, _viewTooltip });
                breakDownViewHeight = _viewTooltip.Frame.GetMaxY();
            }

            _viewBreakdown.Frame = new CGRect(new CGPoint(0, _viewTitleSection.Frame.GetMaxY()), new CGSize(ViewWidth
                , breakDownViewHeight + GetScaledHeight(16)));

            _uiScrollView.AddSubview(_viewBreakdown);
            _uiScrollView.ContentSize = new CGSize(ViewWidth, _viewBreakdown.Frame.GetMaxY());
        }

        private UIView GetCommonLabelView(nfloat yLocation, string itemString, string valueString, bool isOverpaid = false)
        {
            UIView view = new UIView(new CGRect(0, yLocation, ViewWidth, GetScaledHeight(20)));
            UILabel item = new UILabel(new CGRect(BaseMargin, 0, BaseMarginedWidth / 2, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = itemString
            };
            nfloat itemWidth = item.GetLabelWidth(ViewWidth);
            item.Frame = new CGRect(item.Frame.Location, new CGSize(itemWidth, item.Frame.Height));
            UILabel value = new UILabel(new CGRect(BaseMargin + (BaseMarginedWidth / 2), 0, BaseMarginedWidth / 2, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Right,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = isOverpaid ? MyTNBColor.FreshGreen : MyTNBColor.CharcoalGrey,
                Text = string.Format(BillConstants.Format_Default, TNBGlobal.UNIT_CURRENCY, valueString)
            };
            nfloat valueWidth = value.GetLabelWidth(ViewWidth);
            value.Frame = new CGRect(new CGPoint(ViewWidth - BaseMargin - valueWidth, value.Frame.Y), new CGSize(valueWidth, value.Frame.Height));
            view.AddSubviews(new UIView[] { item, value });
            return view;
        }

        private UIView GetPaymentDetails(nfloat yLoc)
        {
            bool isOverPaid = Charges.AccountCharges[0].AmountDue < 0;
            UIView viewPayment = new UIView(new CGRect(0, yLoc, ViewWidth, GetScaledHeight(32)));
            nfloat statusYLoc = isOverPaid ? GetScaledHeight(8) : 0;
            UILabel lblStatus = new UILabel(new CGRect(BaseMargin, statusYLoc, BaseMarginedWidth / 2, GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = GetI18NValue(Charges.AccountCharges[0].AmountDue < 0 ? BillConstants.I18N_PaidExtra : BillConstants.I18N_NeedToPay)
            };
            nfloat statusWidth = lblStatus.GetLabelWidth(ViewWidth);
            lblStatus.Frame = new CGRect(lblStatus.Frame.Location, new CGSize(statusWidth, lblStatus.Frame.Height));

            string result = string.Empty;
            if (Charges.AccountCharges[0].DueDate != null)
            {
                result = DateTime.ParseExact(Charges.AccountCharges[0].DueDate
                   , BillConstants.Format_DateParse, CultureInfo.InvariantCulture).ToString(BillConstants.Format_Date);
            }
            UILabel lblDue = new UILabel(new CGRect(BaseMargin, lblStatus.Frame.GetMaxY(), BaseMarginedWidth / 2, GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = string.Format(BillConstants.Format_Default, GetI18NValue(BillConstants.I18N_By), result),
                Hidden = isOverPaid
            };
            nfloat dueWidth = lblDue.GetLabelWidth(ViewWidth);
            lblDue.Frame = new CGRect(lblDue.Frame.Location, new CGSize(dueWidth, lblDue.Frame.Height));

            UIView viewAmount = new UIView();
            UILabel lblCurrency = new UILabel(new CGRect(0, GetScaledHeight(11), GetScaledWidth(100), GetScaledHeight(18)))
            {
                TextColor = isOverPaid ? MyTNBColor.FreshGreen : MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_12_500,
                TextAlignment = UITextAlignment.Right,
                Text = TNBGlobal.UNIT_CURRENCY
            };

            nfloat currencyWidth = lblCurrency.GetLabelWidth(ViewWidth);
            lblCurrency.Frame = new CGRect(lblCurrency.Frame.Location, new CGSize(currencyWidth, lblCurrency.Frame.Height));

            UILabel lblAmount = new UILabel(new CGRect(lblCurrency.Frame.GetMaxX() + GetScaledWidth(6)
                , 0, GetScaledWidth(100), GetScaledHeight(32)))
            {
                TextColor = isOverPaid ? MyTNBColor.FreshGreen : MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_24_300,
                TextAlignment = UITextAlignment.Left,
                Text = Math.Abs(Charges.AccountCharges[0].AmountDue).ToString("N2", CultureInfo.InvariantCulture)
            };

            nfloat amountWidth = lblAmount.GetLabelWidth(ViewWidth);
            lblAmount.Frame = new CGRect(lblAmount.Frame.Location, new CGSize(amountWidth, lblAmount.Frame.Height));
            viewAmount.AddSubviews(new UIView[] { lblCurrency, lblAmount });

            viewAmount.Frame = new CGRect(ViewWidth - (BaseMargin + currencyWidth + GetScaledWidth(6) + amountWidth)
                , 0, currencyWidth + amountWidth + GetScaledWidth(6), GetScaledHeight(32));

            viewPayment.AddSubviews(new UIView[] { lblStatus, lblDue, viewAmount });

            return viewPayment;
        }

        private CustomUIView GetMandatoryView(nfloat yLoc)
        {
            if (!HasMandatory)
            {
                return new CustomUIView();
            }
            isMandatoryExpanded = false;
            CustomUIView view = new CustomUIView(new CGRect(0, yLoc, ViewWidth, GetScaledHeight(20)))
            {
                ClipsToBounds = true,
                PageName = PageName,
                EventName = BillConstants.Event_MandatoryDetails
            };
            UILabel item = new UILabel(new CGRect(BaseMargin, 0, BaseMarginedWidth / 2, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = GetI18NValue(BillConstants.I18N_ApplicationCharges)
            };
            nfloat itemWidth = item.GetLabelWidth(ViewWidth);
            item.Frame = new CGRect(item.Frame.Location, new CGSize(itemWidth, item.Frame.Height));

            UIImageView imgIndicator = new UIImageView(new CGRect(item.Frame.GetMaxX() + GetScaledWidth(4)
                , 0, GetScaledWidth(20), GetScaledWidth(20)))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_ArrowDown),
                Tag = 99
            };

            UILabel value = new UILabel(new CGRect(BaseMargin + (BaseMarginedWidth / 2), 0, BaseMarginedWidth / 2, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Right,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = string.Format(BillConstants.Format_Default, TNBGlobal.UNIT_CURRENCY
                , Charges.AccountCharges[0].MandatoryCharges.TotalAmount.ToString("N2", CultureInfo.InvariantCulture))
            };
            nfloat valueWidth = value.GetLabelWidth(ViewWidth);
            value.Frame = new CGRect(new CGPoint(ViewWidth - BaseMargin - valueWidth, value.Frame.Y), new CGSize(valueWidth, value.Frame.Height));
            view.AddSubviews(new UIView[] { item, imgIndicator, value });

            UIView mandatoryView = new UIView(new CGRect(GetScaledWidth(24), GetYLocationFromFrame(item.Frame, 4)
                , ViewWidth - GetScaledWidth(40), 0))
            { Tag = 98 };
            nfloat subYLoc = 0;
            int itemCount = 0;
            if (Charges != null && Charges.AccountCharges != null && Charges.AccountCharges.Count > 0
                && Charges.AccountCharges[0] != null && Charges.AccountCharges[0].MandatoryCharges != null
                && Charges.AccountCharges[0].MandatoryCharges.Charges != null)
            {
                for (int i = 0; i < Charges.AccountCharges[0].MandatoryCharges.Charges.Count; i++)
                {
                    ChargesModel chargeItem = Charges.AccountCharges[0].MandatoryCharges.Charges[i];
                    if (chargeItem.Amount < 0) { continue; }
                    UILabel subItem = new UILabel(new CGRect(0, subYLoc, BaseMarginedWidth / 2, GetScaledHeight(20)))
                    {
                        TextAlignment = UITextAlignment.Left,
                        Font = TNBFont.MuseoSans_14_300,
                        TextColor = MyTNBColor.GreyishBrown,
                        Text = chargeItem.Title
                    };
                    nfloat subItemWidth = subItem.GetLabelWidth(ViewWidth);
                    subItem.Frame = new CGRect(subItem.Frame.Location, new CGSize(subItemWidth, subItem.Frame.Height));

                    UILabel subValue = new UILabel(new CGRect(BaseMargin + (BaseMarginedWidth / 2)
                        , subYLoc, BaseMarginedWidth / 2, GetScaledHeight(20)))
                    {
                        TextAlignment = UITextAlignment.Right,
                        Font = TNBFont.MuseoSans_14_300,
                        TextColor = MyTNBColor.GreyishBrown,
                        Text = string.Format(BillConstants.Format_Default, TNBGlobal.UNIT_CURRENCY, chargeItem.Amount.ToString("N2", CultureInfo.InvariantCulture))
                    };
                    nfloat subValueWidth = subValue.GetLabelWidth(ViewWidth);
                    subValue.Frame = new CGRect(new CGPoint(mandatoryView.Frame.Width - valueWidth, subValue.Frame.Y)
                        , new CGSize(subValueWidth, subValue.Frame.Height));
                    mandatoryView.AddSubviews(new UIView[] { subItem, subValue });
                    itemCount++;
                    subYLoc += GetScaledHeight(20);
                }
            }

            mandatoryView.Frame = new CGRect(mandatoryView.Frame.Location, new CGSize(mandatoryView.Frame.Width, itemCount * GetScaledHeight(20)));

            view.AddSubview(mandatoryView);
            CGRect newFrame = view.Frame;
            newFrame.Height = mandatoryView.Frame.GetMaxY();
            return view;
        }

        private CustomUIView GetTooltipView(nfloat yLoc)
        {
            if (!HasMandatory)
            {
                return new CustomUIView();
            }
            CustomUIView view = new CustomUIView(new CGRect(0, yLoc, ViewWidth, GetScaledHeight(24)))
            {
                BackgroundColor = UIColor.White,
                PageName = PageName,
                EventName = BillConstants.Event_MandatoryChargesPopup
            };
            CustomUIView viewInfo = new CustomUIView(new CGRect(BaseMargin, 0, BaseMarginedWidth, GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            UIImageView imgView = new UIImageView(new CGRect(GetScaledWidth(4)
                , GetScaledHeight(4), GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_InfoBlue)
            };
            UILabel lblDescription = new UILabel(new CGRect(GetScaledWidth(28)
                , GetScaledHeight(4), view.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(BillConstants.I18N_MinimumChargeDescription)
            };
            viewInfo.Layer.CornerRadius = GetScaledHeight(12);
            viewInfo.AddSubviews(new UIView[] { imgView, lblDescription });
            view.AddSubview(viewInfo);
            view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (Charges != null && Charges.MandatoryChargesPopUpDetails != null)
                {
                    PopupModel popUpContent = Charges.MandatoryChargesPopUpDetails.Find(x => x.Type == "HasMandatoryCharges");
                    if (popUpContent != null)
                    {
                        DisplayCustomAlert(popUpContent.Title, popUpContent.Description
                            , new Dictionary<string, Action> { { popUpContent.CTA, null } }
                            , false);
                    }
                }
            }));
            return view;
        }

        private void SetEvents()
        {
            if (_viewMandatory == null) { return; }
            _viewMandatory.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                nfloat mandatoryViewHeight = GetScaledHeight(20);
                if (!isMandatoryExpanded)
                {
                    UIView view = _viewMandatory.ViewWithTag(98) as UIView;
                    if (view != null)
                    {
                        mandatoryViewHeight = view.Frame.GetMaxY();
                    }
                }
                isMandatoryExpanded = !isMandatoryExpanded;
                UIImageView imgView = _viewMandatory.ViewWithTag(99) as UIImageView;
                if (imgView != null)
                {
                    imgView.Image = UIImage.FromBundle(isMandatoryExpanded ? BillConstants.IMG_ArrowUp : BillConstants.IMG_ArrowDown);
                }
                UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseIn
                    , () =>
                    {
                        _viewMandatory.Frame = new CGRect(_viewMandatory.Frame.Location, new CGSize(_viewMandatory.Frame.Width
                            , mandatoryViewHeight));
                        _viewLine.Frame = new CGRect(new CGPoint(_viewLine.Frame.X
                            , GetYLocationFromFrame(_viewMandatory.Frame, 16)), _viewLine.Frame.Size);
                        _viewPayment.Frame = new CGRect(new CGPoint(_viewPayment.Frame.X
                            , GetYLocationFromFrame(_viewLine.Frame, 20)), _viewPayment.Frame.Size);
                        _viewTooltip.Frame = new CGRect(new CGPoint(_viewTooltip.Frame.X
                            , GetYLocationFromFrame(_viewPayment.Frame, 16)), _viewTooltip.Frame.Size);
                        _viewBreakdown.Frame = new CGRect(new CGPoint(0, _viewTitleSection.Frame.GetMaxY()), new CGSize(ViewWidth
                            , _viewTooltip.Frame.GetMaxY() + GetScaledHeight(16)));
                    }
                    , () =>
                    {
                        _uiScrollView.ContentSize = new CGSize(ViewWidth, _viewBreakdown.Frame.GetMaxY());
                    }
                );
            }));
        }

        private bool HasMandatory
        {
            get
            {
                if (Charges != null && Charges.AccountCharges != null && Charges.AccountCharges[0] != null
                    && Charges.AccountCharges[0].MandatoryCharges != null)
                {
                    return Charges.AccountCharges[0].MandatoryCharges.TotalAmount > 0;
                }
                return false;
            }
        }
    }
}