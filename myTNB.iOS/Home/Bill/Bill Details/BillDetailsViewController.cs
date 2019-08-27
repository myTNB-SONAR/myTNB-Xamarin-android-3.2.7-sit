using CoreGraphics;
using Foundation;
using myTNB.SSMR;
using System;
using System.Diagnostics;
using UIKit;

namespace myTNB
{
    public partial class BillDetailsViewController : CustomUIViewController
    {
        private UIView _viewDetails, _viewTitleSection, _viewBreakdown, _viewLine, _toolTipParentView;
        CustomUIView _viewMandatory;
        private UIScrollView _uiScrollView;

        public BillDetailsViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
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
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(SSMRConstants.IMG_BackIcon)
            , UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            UIBarButtonItem btnInfo = new UIBarButtonItem(UIImage.FromBundle(SSMRConstants.IMG_Info)
               , UIBarButtonItemStyle.Done, (sender, e) =>
               {
                   Debug.WriteLine("btnInfo tapped");
                   PrepareToolTipView();
               });
            NavigationItem.LeftBarButtonItem = btnBack;
            NavigationItem.RightBarButtonItem = btnInfo;
            Title = "Bill Details";
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
                Text = "My House"
            };
            UILabel lblAddress = new UILabel(new CGRect(BaseMargin, GetYLocationFromFrame(lblAccountName.Frame, 8)
                , BaseMarginedWidth, GetScaledHeight(32)))
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_12_500,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = "No. 3 Jalan Melur, 12 Taman Melur, 68000 Ampang, Selangor"
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
                Text = "My Bill Details"
            };
            _viewTitleSection.AddSubview(lblSectionTitle);
            _uiScrollView.AddSubview(_viewTitleSection);
        }

        private void AddBreakdown()
        {
            _viewBreakdown = new UIView { BackgroundColor = UIColor.White };
            UIView viewOutstanding = GetCommonLabelView(GetScaledHeight(16), "My outstanding charges", "RM 0.00");
            UIView viewMonthBill = GetCommonLabelView(GetYLocationFromFrame(viewOutstanding.Frame, 16), "My bill this month", "RM 0.00");
            _viewMandatory = GetMandatoryView(GetYLocationFromFrame(viewMonthBill.Frame, 16));
            _viewLine = new UIView(new CGRect(BaseMargin, GetYLocationFromFrame(_viewMandatory.Frame, 16), BaseMarginedWidth, GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };

            UIView viewPayment = GetPaymentDetails(GetYLocationFromFrame(_viewLine.Frame, 20));
            CustomUIView viewTooltip = GetTooltipView(GetYLocationFromFrame(viewPayment.Frame, 16));

            _viewBreakdown.AddSubviews(new UIView[] { viewOutstanding, viewMonthBill, _viewMandatory, _viewLine, viewPayment, viewTooltip });
            _viewBreakdown.Frame = new CGRect(new CGPoint(0, _viewTitleSection.Frame.GetMaxY()), new CGSize(ViewWidth
                , viewTooltip.Frame.GetMaxY() + GetScaledHeight(16)));

            _uiScrollView.AddSubview(_viewBreakdown);
            _uiScrollView.ContentSize = new CGSize(ViewWidth, _viewBreakdown.Frame.GetMaxY());
        }

        private UIView GetCommonLabelView(nfloat yLocation, string itemString, string valueString)
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
                TextColor = MyTNBColor.CharcoalGrey,
                Text = valueString
            };
            nfloat valueWidth = value.GetLabelWidth(ViewWidth);
            value.Frame = new CGRect(new CGPoint(ViewWidth - BaseMargin - valueWidth, value.Frame.Y), new CGSize(valueWidth, value.Frame.Height));
            view.AddSubviews(new UIView[] { item, value });
            return view;
        }

        private UIView GetPaymentDetails(nfloat yLoc)
        {
            UIView viewPayment = new UIView(new CGRect(0, yLoc, ViewWidth, GetScaledHeight(32)));

            UILabel lblStatus = new UILabel(new CGRect(BaseMargin, 0, BaseMarginedWidth / 2, GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = "I need to pay"
            };
            nfloat statusWidth = lblStatus.GetLabelWidth(ViewWidth);
            lblStatus.Frame = new CGRect(lblStatus.Frame.Location, new CGSize(statusWidth, lblStatus.Frame.Height));

            UILabel lblDue = new UILabel(new CGRect(BaseMargin, lblStatus.Frame.GetMaxY(), BaseMarginedWidth / 2, GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = "by 24 Sep 2019"
            };
            nfloat dueWidth = lblDue.GetLabelWidth(ViewWidth);
            lblDue.Frame = new CGRect(lblDue.Frame.Location, new CGSize(dueWidth, lblDue.Frame.Height));

            UIView viewAmount = new UIView();
            UILabel lblCurrency = new UILabel(new CGRect(0, GetScaledHeight(11), GetScaledWidth(100), GetScaledHeight(18)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_12_500,
                TextAlignment = UITextAlignment.Right,
                Text = TNBGlobal.UNIT_CURRENCY
            };

            nfloat currencyWidth = lblCurrency.GetLabelWidth(ViewWidth);
            lblCurrency.Frame = new CGRect(lblCurrency.Frame.Location, new CGSize(currencyWidth, lblCurrency.Frame.Height));

            UILabel lblAmount = new UILabel(new CGRect(lblCurrency.Frame.GetMaxX() + GetScaledWidth(6), 0, GetScaledWidth(100), GetScaledHeight(32)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_24_300,
                TextAlignment = UITextAlignment.Left,
                Text = "100.00"
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
            CustomUIView view = new CustomUIView(new CGRect(0, yLoc, ViewWidth, GetScaledHeight(20))) { ClipsToBounds = true };
            UILabel item = new UILabel(new CGRect(BaseMargin, 0, BaseMarginedWidth / 2, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = "My other charges"
            };
            nfloat itemWidth = item.GetLabelWidth(ViewWidth);
            item.Frame = new CGRect(item.Frame.Location, new CGSize(itemWidth, item.Frame.Height));

            UIImageView imgIndicator = new UIImageView(new CGRect(item.Frame.GetMaxX() + GetScaledWidth(4)
                , 0, GetScaledWidth(20), GetScaledWidth(20)))
            {
                Image = UIImage.FromBundle("Arrow-Expand-Down")
            };

            UILabel value = new UILabel(new CGRect(BaseMargin + (BaseMarginedWidth / 2), 0, BaseMarginedWidth / 2, GetScaledHeight(20)))
            {
                TextAlignment = UITextAlignment.Right,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = "RM 200.00"
            };
            nfloat valueWidth = value.GetLabelWidth(ViewWidth);
            value.Frame = new CGRect(new CGPoint(ViewWidth - BaseMargin - valueWidth, value.Frame.Y), new CGSize(valueWidth, value.Frame.Height));
            view.AddSubviews(new UIView[] { item, imgIndicator, value });

            UIView mandatoryView = new UIView(new CGRect(GetScaledWidth(24), GetYLocationFromFrame(item.Frame, 4), ViewWidth - GetScaledWidth(40), 0));
            nfloat subYLoc = 0;
            int itemCount = 0;
            for (int i = 0; i < 4; i++)
            {
                UILabel subItem = new UILabel(new CGRect(0, subYLoc, BaseMarginedWidth / 2, GetScaledHeight(20)))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = TNBFont.MuseoSans_14_300,
                    TextColor = MyTNBColor.GreyishBrown,
                    Text = "My other charges"
                };
                nfloat subItemWidth = subItem.GetLabelWidth(ViewWidth);
                subItem.Frame = new CGRect(subItem.Frame.Location, new CGSize(subItemWidth, subItem.Frame.Height));

                UILabel subValue = new UILabel(new CGRect(BaseMargin + (BaseMarginedWidth / 2), subYLoc, BaseMarginedWidth / 2, GetScaledHeight(20)))
                {
                    TextAlignment = UITextAlignment.Right,
                    Font = TNBFont.MuseoSans_14_300,
                    TextColor = MyTNBColor.GreyishBrown,
                    Text = "RM 200.00"
                };
                nfloat subValueWidth = subValue.GetLabelWidth(ViewWidth);
                subValue.Frame = new CGRect(new CGPoint(mandatoryView.Frame.Width - valueWidth, subValue.Frame.Y)
                    , new CGSize(subValueWidth, subValue.Frame.Height));
                mandatoryView.AddSubviews(new UIView[] { subItem, subValue });
                itemCount++;
                subYLoc += GetScaledHeight(20);
            }

            mandatoryView.Frame = new CGRect(mandatoryView.Frame.Location, new CGSize(mandatoryView.Frame.Width, itemCount * GetScaledHeight(20)));

            view.AddSubview(mandatoryView);
            CGRect newFrame = view.Frame;
            newFrame.Height = mandatoryView.Frame.GetMaxY();
            view.Frame = newFrame;
            return view;
        }

        private CustomUIView GetTooltipView(nfloat yLoc)
        {
            CustomUIView view = new CustomUIView(new CGRect(0, yLoc, ViewWidth, GetScaledHeight(24)))
            {
                BackgroundColor = UIColor.White
            };
            CustomUIView viewInfo = new CustomUIView(new CGRect(BaseMargin, 0, BaseMarginedWidth, GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            UIImageView imgView = new UIImageView(new CGRect(GetScaledWidth(4)
                , GetScaledHeight(4), GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle("IC-Info-Blue")
            };
            UILabel lblDescription = new UILabel(new CGRect(GetScaledWidth(28)
                , GetScaledHeight(4), view.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = "This account has a minimum charge."
            };
            viewInfo.Layer.CornerRadius = GetScaledHeight(12);
            viewInfo.AddSubviews(new UIView[] { imgView, lblDescription });
            view.AddSubview(viewInfo);
            view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                /* Model.PopupModel popUpContent = SSMRAccounts.GetPopupDetailsByType(SelectAccountConstants.Popup_NoSSMRCA);
                 DisplayCustomAlert(popUpContent.Title, popUpContent.Description
                     , new Dictionary<string, Action> { { popUpContent.CTA, null } }
                     , false);*/
            }));
            return view;
        }

        private void SetEvents()
        {

        }
    }
}