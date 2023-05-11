
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Java.Text;
using myTNB.Mobile;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.MyHome.Model;
using myTNB_Android.Src.MyHome.MVP;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.CompoundView.ExpandableTextViewComponent;

namespace myTNB_Android.Src.MyHome.Activity
{
	[Activity(Label = "MyHomePaymentDetailsActivity", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class MyHomePaymentDetailsActivity : BaseActivityCustom, MyHomePaymentDetailsContract.IView
    {
        [BindView(Resource.Id.myHomePaymentDetailsAccountName)]
        TextView myHomePaymentDetailsAccountName;

        [BindView(Resource.Id.myHomePaymentDetailsAccountAddress)]
        TextView myHomePaymentDetailsAccountAddress;

        [BindView(Resource.Id.myHomePaymentDetailsMyChargesTitle)]
        TextView myHomePaymentDetailsMyChargesTitle;

        [BindView(Resource.Id.myHomePaymentDetailsAccountChargeLabel)]
        TextView myHomePaymentDetailsAccountChargeLabel;

        [BindView(Resource.Id.myHomePaymentDetailsAccountChargeValue)]
        TextView myHomePaymentDetailsAccountChargeValue;

        [BindView(Resource.Id.myHomePaymentDetailsAccountBillThisMonthLabel)]
        TextView myHomePaymentDetailsAccountBillThisMonthLabel;

        [BindView(Resource.Id.myHomePaymentDetailsAccountBillThisMonthValue)]
        TextView myHomePaymentDetailsAccountBillThisMonthValue;

        [BindView(Resource.Id.myHomePaymentDetailsRoundingAdjustmentLayout)]
        LinearLayout myHomePaymentDetailsRoundingAdjustmentLayout;

        [BindView(Resource.Id.myHomePaymentDetailsRoundingAdjustmentLabel)]
        TextView myHomePaymentDetailsRoundingAdjustmentLabel;

        [BindView(Resource.Id.myHomePaymentDetailsRoundingAdjustmentValue)]
        TextView myHomePaymentDetailsRoundingAdjustmentValue;

        [BindView(Resource.Id.myHomePaymentDetailsOtherChargesExpandableView)]
        ExpandableTextViewComponent myHomePaymentDetailsOtherChargesExpandableView;

        [BindView(Resource.Id.myHomePaymentDetailsAccountPayAmountLabel)]
        TextView myHomePaymentDetailsAccountPayAmountLabel;

        [BindView(Resource.Id.myHomePaymentDetailsAccountPayAmountDate)]
        TextView myHomePaymentDetailsAccountPayAmountDate;

        [BindView(Resource.Id.myHomePaymentDetailsAccountPayAmountCurrency)]
        TextView myHomePaymentDetailsAccountPayAmountCurrency;

        [BindView(Resource.Id.myHomePaymentDetailsAccountPayAmountValue)]
        TextView myHomePaymentDetailsAccountPayAmountValue;

        [BindView(Resource.Id.myHomePaymentDetailsAccountMinChargeLabelContainer)]
        LinearLayout myHomePaymentDetailsAccountMinChargeLabelContainer;

        [BindView(Resource.Id.myHomePaymentDetailsAccountMinChargeLabel)]
        TextView myHomePaymentDetailsAccountMinChargeLabel;

        [BindView(Resource.Id.myHomePaymentDetailsInfoLabelContainerDetailEPP)]
        LinearLayout myHomePaymentDetailsInfoLabelContainerDetailEPP;

        [BindView(Resource.Id.myHomePaymentDetailsInfoLabelDetailEPP)]
        TextView myHomePaymentDetailsInfoLabelDetailEPP;

        [BindView(Resource.Id.myHomePaymentDetailsCreditDebitCardTitle)]
        TextView myHomePaymentDetailsCreditDebitCardTitle;

        [BindView(Resource.Id.myHomePaymentDetailsListAddedCards)]
        ListView myHomePaymentDetailsListAddedCards;

        [BindView(Resource.Id.myHomePaymentDetailsBtnAddCard)]
        Button myHomePaymentDetailsBtnAddCard;

        [BindView(Resource.Id.myHomePaymentDetailsOtherPaymentTitle)]
        TextView myHomePaymentDetailsOtherPaymentTitle;

        [BindView(Resource.Id.myHomePaymentDetailsBtnFPXPayment)]
        Button myHomePaymentDetailsBtnFPXPayment;

        [BindView(Resource.Id.myHomePaymentDetailsTNGPaymentTitle)]
        TextView myHomePaymentDetailsTNGPaymentTitle;

        [BindView(Resource.Id.myHomePaymentDetailsBtnTNGPayment)]
        Button myHomePaymentDetailsBtnTNGPayment;

        private MyHomePaymentDetailsContract.IUserActionsListener presenter;

        const string PAGE_ID = "PaymentDetailsActivity";

        protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

            try
            {
                _ = new MyHomePaymentDetailsPresenter(this, this, this);

                SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

                //Lang STUB
                SetToolBarTitle("Details");
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    this.presenter?.OnInitialize(extras);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.MyHomePaymentDetailsView;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public void SetPresenter(MyHomePaymentDetailsContract.IUserActionsListener userActionListener)
        {
            this.presenter = userActionListener;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public void SetUpViews()
        {
            TextViewUtils.SetMuseoSans500Typeface(myHomePaymentDetailsAccountName, myHomePaymentDetailsMyChargesTitle,
                myHomePaymentDetailsAccountChargeLabel,
                myHomePaymentDetailsAccountBillThisMonthLabel,
                myHomePaymentDetailsRoundingAdjustmentLabel,
                myHomePaymentDetailsAccountPayAmountLabel,
                myHomePaymentDetailsAccountMinChargeLabel,
                myHomePaymentDetailsAccountChargeValue,
                myHomePaymentDetailsAccountBillThisMonthValue,
                myHomePaymentDetailsRoundingAdjustmentValue,
                myHomePaymentDetailsAccountPayAmountValue,
                myHomePaymentDetailsAccountPayAmountCurrency,
                myHomePaymentDetailsCreditDebitCardTitle,
                myHomePaymentDetailsBtnAddCard,
                myHomePaymentDetailsOtherPaymentTitle,
                myHomePaymentDetailsBtnFPXPayment,
                myHomePaymentDetailsTNGPaymentTitle,
                myHomePaymentDetailsBtnTNGPayment);

            TextViewUtils.SetMuseoSans300Typeface(myHomePaymentDetailsAccountAddress,
                myHomePaymentDetailsAccountPayAmountDate);

            TextViewUtils.SetTextSize24(myHomePaymentDetailsAccountPayAmountValue);

            TextViewUtils.SetTextSize16(myHomePaymentDetailsMyChargesTitle,
                myHomePaymentDetailsCreditDebitCardTitle,
                myHomePaymentDetailsOtherPaymentTitle,
                myHomePaymentDetailsTNGPaymentTitle);

            TextViewUtils.SetTextSize14(myHomePaymentDetailsAccountName,
                myHomePaymentDetailsAccountChargeLabel,
                myHomePaymentDetailsAccountChargeValue,
                myHomePaymentDetailsAccountBillThisMonthLabel,
                myHomePaymentDetailsAccountBillThisMonthValue,
                myHomePaymentDetailsRoundingAdjustmentLabel,
                myHomePaymentDetailsRoundingAdjustmentValue,
                myHomePaymentDetailsAccountPayAmountLabel,
                myHomePaymentDetailsAccountPayAmountDate,
                myHomePaymentDetailsBtnAddCard,
                myHomePaymentDetailsBtnFPXPayment,
                myHomePaymentDetailsBtnTNGPayment);

            TextViewUtils.SetTextSize12(myHomePaymentDetailsAccountAddress,
                myHomePaymentDetailsAccountPayAmountCurrency,
                myHomePaymentDetailsAccountMinChargeLabel);

            if (TextViewUtils.IsLargeFonts)
            {
                TextViewUtils.SetMuseoSans500Typeface(myHomePaymentDetailsAccountPayAmountValue);
                TextViewUtils.SetTextSize14(myHomePaymentDetailsAccountPayAmountValue, myHomePaymentDetailsAccountPayAmountCurrency);
            }
            else
            {
                TextViewUtils.SetMuseoSans300Typeface(myHomePaymentDetailsAccountPayAmountValue);
                TextViewUtils.SetTextSize24(myHomePaymentDetailsAccountPayAmountValue);
                TextViewUtils.SetTextSize12(myHomePaymentDetailsAccountPayAmountCurrency);
            }
        }

        public void UpdateAccountInfoUI()
        {
            MyHomePaymentDetailsModel model = this.presenter?.GetMyHomePaymentDetailsModel();
            if (model != null)
            {
                myHomePaymentDetailsAccountName.Text = model.AccountNickName;
                myHomePaymentDetailsAccountAddress.Text = model.AccountAddress;
            }
        }

        public void UpdateMyChargesUI()
        {
            MyHomePaymentDetailsModel model = this.presenter?.GetMyHomePaymentDetailsModel();
            AccountChargeModel accountChargeModel = this.presenter?.GetAccountChargeModel();

            SimpleDateFormat dateParser = new SimpleDateFormat("yyyyMMdd", LocaleUtils.GetDefaultLocale());
            SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy", LocaleUtils.GetCurrentLocale());

            myHomePaymentDetailsMyChargesTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.BILL_DETAILS, LanguageConstants.BillDetails.MY_CHARGES);
            var billThisMonthString = BillRedesignUtility.Instance.IsCAEligible(model.AccountNumber) ? Utility.GetLocalizedLabel(LanguageConstants.BILL_DETAILS, LanguageConstants.BillDetails.BILL_THIS_MONTH_V2)
                : Utility.GetLocalizedLabel(LanguageConstants.BILL_DETAILS, LanguageConstants.BillDetails.BILL_THIS_MONTH);
            myHomePaymentDetailsAccountBillThisMonthLabel.Text = billThisMonthString;
            myHomePaymentDetailsAccountPayAmountCurrency.Text = "RM";

            EnableEppTooltip(accountChargeModel.ShowEppToolTip);

            bool hasEPPToolTip = accountChargeModel.ShowEppToolTip;
            bool hasOneTimeCharges = accountChargeModel.MandatoryCharges.TotalAmount > 0f;

            myHomePaymentDetailsAccountMinChargeLabelContainer.Visibility = hasOneTimeCharges && !hasEPPToolTip ? ViewStates.Visible : ViewStates.Gone;

            if (hasOneTimeCharges)
            {
                myHomePaymentDetailsOtherChargesExpandableView.Visibility = ViewStates.Visible;
                myHomePaymentDetailsOtherChargesExpandableView.SetExpandableType(ExpandableTextViewType.APPLICATION_CHARGES);
                myHomePaymentDetailsOtherChargesExpandableView.SetApplicationChargesLabel(Utility.GetLocalizedLabel(LanguageConstants.BILL_DETAILS, LanguageConstants.BillDetails.APPLICATION_CHARGES));
                myHomePaymentDetailsOtherChargesExpandableView.SetOtherCharges(accountChargeModel.MandatoryCharges.TotalAmount, accountChargeModel.MandatoryCharges.ChargeModelList);
                myHomePaymentDetailsOtherChargesExpandableView.RequestLayout();
            }
            else
            {
                myHomePaymentDetailsOtherChargesExpandableView.Visibility = ViewStates.Gone;
            }

            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            if (accountChargeModel.OutstandingCharges < 0f)
            {
                myHomePaymentDetailsAccountChargeValue.Text = "- RM " + (Math.Abs(accountChargeModel.OutstandingCharges) * -1).ToString("#,##0.00", currCult);
            }
            else
            {
                myHomePaymentDetailsAccountChargeValue.Text = "RM " + (Math.Abs(accountChargeModel.OutstandingCharges)).ToString("#,##0.00", currCult);
            }

            if (accountChargeModel.OutstandingCharges < 0f)
            {
                myHomePaymentDetailsAccountChargeLabel.Text = Utility.GetLocalizedLabel(LanguageConstants.BILL_DETAILS, LanguageConstants.BillDetails.PAID_EXTRA);
                myHomePaymentDetailsAccountChargeValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));
            }
            else
            {
                var accountChargeString = BillRedesignUtility.Instance.IsCAEligible(model.AccountNumber) ? Utility.GetLocalizedLabel(LanguageConstants.BILL_DETAILS, LanguageConstants.BillDetails.OUTSTANDING_CHARGES_V2)
                    : Utility.GetLocalizedLabel(LanguageConstants.BILL_DETAILS, LanguageConstants.BillDetails.OUTSTANDING_CHARGES);
                myHomePaymentDetailsAccountChargeLabel.Text = accountChargeString;
                myHomePaymentDetailsAccountChargeValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
            }

            if (BillRedesignUtility.Instance.IsCAEligible(model.AccountNumber) && accountChargeModel.ShouldShowRoundingAdjustment)
            {
                if (accountChargeModel.CurrentCharges < 0f)
                {
                    myHomePaymentDetailsAccountBillThisMonthValue.Text = "- RM " + (accountChargeModel.ActualCurrentCharges * -1).ToString("#,##0.00", currCult);
                }
                else
                {
                    myHomePaymentDetailsAccountBillThisMonthValue.Text = "RM " + accountChargeModel.ActualCurrentCharges.ToString("#,##0.00", currCult);
                }

                myHomePaymentDetailsRoundingAdjustmentLayout.Visibility = ViewStates.Visible;
                myHomePaymentDetailsRoundingAdjustmentLabel.Text = Utility.GetLocalizedLabel(LanguageConstants.BILL_DETAILS, LanguageConstants.BillDetails.ROUNDING_ADJUSTMENT);

                if (accountChargeModel.RoundingAmount < 0f)
                {
                    myHomePaymentDetailsRoundingAdjustmentValue.Text = "- RM " + (accountChargeModel.RoundingAmount * -1).ToString("#,##0.00", currCult);
                }
                else
                {
                    myHomePaymentDetailsRoundingAdjustmentValue.Text = "RM " + accountChargeModel.RoundingAmount.ToString("#,##0.00", currCult);
                }
            }
            else
            {
                if (accountChargeModel.CurrentCharges < 0f)
                {
                    myHomePaymentDetailsAccountBillThisMonthValue.Text = "- RM " + (accountChargeModel.CurrentCharges * -1).ToString("#,##0.00", currCult);
                }
                else
                {
                    myHomePaymentDetailsAccountBillThisMonthValue.Text = "RM " + accountChargeModel.CurrentCharges.ToString("#,##0.00", currCult);
                }
            }

            myHomePaymentDetailsAccountPayAmountValue.Text = accountChargeModel.AmountDue.ToString("#,##0.00", currCult);
            if (accountChargeModel.IsNeedPay)
            {
                myHomePaymentDetailsAccountPayAmountLabel.Visibility = ViewStates.Visible;
                myHomePaymentDetailsAccountPayAmountLabel.Text = Utility.GetLocalizedLabel(LanguageConstants.BILL_DETAILS, LanguageConstants.BillDetails.NEED_TO_PAY);
                myHomePaymentDetailsAccountPayAmountDate.Visibility = ViewStates.Visible;
                myHomePaymentDetailsAccountPayAmountDate.Text = Utility.GetLocalizedLabel(LanguageConstants.BILL_DETAILS, LanguageConstants.BillDetails.BY) + " " + dateFormatter.Format(dateParser.Parse(accountChargeModel.DueDate));

                myHomePaymentDetailsAccountPayAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
                myHomePaymentDetailsAccountPayAmountValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
            }
            else if (accountChargeModel.IsPaidExtra)
            {
                myHomePaymentDetailsAccountPayAmountLabel.Visibility = ViewStates.Visible;
                myHomePaymentDetailsAccountPayAmountDate.Visibility = ViewStates.Gone;

                myHomePaymentDetailsAccountPayAmountLabel.Text = Utility.GetLocalizedLabel(LanguageConstants.BILL_DETAILS, LanguageConstants.BillDetails.PAID_EXTRA);
                myHomePaymentDetailsAccountPayAmountValue.Text = (Math.Abs(accountChargeModel.AmountDue)).ToString("#,##0.00", currCult);
                myHomePaymentDetailsAccountPayAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));
                myHomePaymentDetailsAccountPayAmountValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));
            }
            else if (accountChargeModel.IsCleared)
            {
                myHomePaymentDetailsAccountPayAmountLabel.Visibility = ViewStates.Visible;
                myHomePaymentDetailsAccountPayAmountDate.Visibility = ViewStates.Gone;

                myHomePaymentDetailsAccountPayAmountLabel.Text = Utility.GetLocalizedLabel(LanguageConstants.BILL_DETAILS, LanguageConstants.BillDetails.CLEARED_BILLS);
                myHomePaymentDetailsAccountPayAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
                myHomePaymentDetailsAccountPayAmountValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
            }
        }

        public void UpdatePaymentsMethodUI()
        {
            myHomePaymentDetailsCreditDebitCardTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.CARDS);
            myHomePaymentDetailsOtherPaymentTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SELECT_PAYMENT_METHOD, LanguageConstants.SelectPaymentMethod.OTHER_PAYMENT_METHODS);
            myHomePaymentDetailsTNGPaymentTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SELECT_PAYMENT_METHOD, LanguageConstants.SelectPaymentMethod.E_WALLET_PAYMENT_METHODS);
            myHomePaymentDetailsBtnTNGPayment.Text = Utility.GetLocalizedLabel(LanguageConstants.SELECT_PAYMENT_METHOD, LanguageConstants.SelectPaymentMethod.TNG_TITLE);
            myHomePaymentDetailsBtnAddCard.Text = Utility.GetLocalizedLabel(LanguageConstants.SELECT_PAYMENT_METHOD, LanguageConstants.SelectPaymentMethod.ADD_CARD);
            myHomePaymentDetailsBtnFPXPayment.Text = Utility.GetLocalizedLabel(LanguageConstants.SELECT_PAYMENT_METHOD, LanguageConstants.SelectPaymentMethod.FPX_TITLE);
        }

        private void EnableEppTooltip(bool isTooltipShown)
        {
            if (isTooltipShown == true)
            {
                myHomePaymentDetailsInfoLabelDetailEPP.Text = Utility.GetLocalizedCommonLabel(LanguageConstants.Common.EPP_TOOLTIP_TITLE);
                myHomePaymentDetailsInfoLabelContainerDetailEPP.Visibility = ViewStates.Visible;
            }
            else
            {
                myHomePaymentDetailsInfoLabelContainerDetailEPP.Visibility = ViewStates.Gone;
            }
        }
    }
}

