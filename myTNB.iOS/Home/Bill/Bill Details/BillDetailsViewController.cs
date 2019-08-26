using CoreGraphics;
using Foundation;
using myTNB.SSMR;
using System;
using UIKit;

namespace myTNB
{
    public partial class BillDetailsViewController : CustomUIViewController
    {
        private UIView _viewDetails, _viewTitleSection, _viewBreakdown, _viewLine, _viewPayment;

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
               });
            NavigationItem.LeftBarButtonItem = btnBack;
            NavigationItem.RightBarButtonItem = btnInfo;
            Title = "Bill Details";
        }

        private void AddDetails()
        {
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
            View.AddSubview(_viewDetails);
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
            View.AddSubview(_viewTitleSection);
        }

        private void AddBreakdown()
        {
            _viewBreakdown = new UIView { BackgroundColor = UIColor.White };
            UIView viewOutstanding = GetCommonLabelView(GetScaledHeight(16), "My outstanding charges", "RM 0.00");
            UIView viewMonthBill = GetCommonLabelView(GetYLocationFromFrame(viewOutstanding.Frame, 16), "My bill this month", "RM 0.00");

            _viewLine = new UIView(new CGRect(BaseMargin, GetYLocationFromFrame(viewMonthBill.Frame, 16), BaseMarginedWidth, GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };

            UIView viewPayment = GetPaymentDetails(GetYLocationFromFrame(_viewLine.Frame, 20));

            _viewBreakdown.AddSubviews(new UIView[] { viewOutstanding, viewMonthBill, _viewLine, viewPayment });
            _viewBreakdown.Frame = new CGRect(new CGPoint(0, _viewTitleSection.Frame.GetMaxY()), new CGSize(ViewWidth
                , viewPayment.Frame.GetMaxY() + GetScaledHeight(12)));

            View.AddSubview(_viewBreakdown);
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
            nfloat valueWidth = item.GetLabelWidth(ViewWidth);
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
    }
}