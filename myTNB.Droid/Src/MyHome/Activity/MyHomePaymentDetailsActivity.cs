
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
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Java.Text;
using myTNB.Mobile;
using myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails;
using myTNB.AndroidApp.Src.AddCard.Activity;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.CompoundView;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Adapter;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Model;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Models;
using myTNB.AndroidApp.Src.MyHome.Model;
using myTNB.AndroidApp.Src.MyHome.MVP;
using myTNB.AndroidApp.Src.MyTNBService.Model;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using static myTNB.AndroidApp.Src.CompoundView.ExpandableTextViewComponent;
using static myTNB.AndroidApp.Src.Utils.LanguageConstants;

namespace myTNB.AndroidApp.Src.MyHome.Activity
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

        [BindView(Resource.Id.myHomePaymentDetailsOverlay)]
        View myHomePaymentDetailsOverlay;

        [BindView(Resource.Id.myHomePaymentDetailsEnterCvvLayout)]
        LinearLayout myHomePaymentDetailsEnterCvvLayout;

        [BindView(Resource.Id.myHomePaymentDetailsLblBack)]
        TextView myHomePaymentDetailsLblBack;

        [BindView(Resource.Id.myHomePaymentDetailsLblCVVInfo)]
        TextView myHomePaymentDetailsLblCVVInfo;

        [BindView(Resource.Id.myHomePaymentDetailsTxtNumber_1)]
        EditText myHomePaymentDetailsTxtNumber_1;

        [BindView(Resource.Id.myHomePaymentDetailsTxtNumber_2)]
        EditText myHomePaymentDetailsTxtNumber_2;

        [BindView(Resource.Id.myHomePaymentDetailsTxtNumber_3)]
        EditText myHomePaymentDetailsTxtNumber_3;

        [BindView(Resource.Id.myHomePaymentDetailsTxtNumber_4)]
        EditText myHomePaymentDetailsTxtNumber_4;

        [BindView(Resource.Id.myHomePaymentDetailsMainLayout)]
        LinearLayout myHomePaymentDetailsMainLayout;

        private static MPCardDetails cardDetails;
        MPAddCardAdapter cardAdapter;
        private string selectedPaymentMethod;
        private CreditCard selectedCard;

        private MyHomePaymentDetailsContract.IUserActionsListener presenter;

        const string PAGE_ID = "PaymentDetailsActivity";
        private static string METHOD_CREDIT_CARD = "CC";
        private static string METHOD_FPX = "FPX";
        private static string METHOD_TNG = "TNG";

        private string enteredCVV;

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
                myHomePaymentDetailsBtnTNGPayment,
                myHomePaymentDetailsLblBack);

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

            cardAdapter = new MPAddCardAdapter(this, this.presenter?.GetRegisteredCards());
            myHomePaymentDetailsListAddedCards.Adapter = cardAdapter;
            cardAdapter.OnItemClick += OnItemClick;

            myHomePaymentDetailsOverlay.Visibility = ViewStates.Gone;
            myHomePaymentDetailsOverlay.Click += delegate
            {
                myHomePaymentDetailsEnterCvvLayout.Visibility = ViewStates.Gone;
                myHomePaymentDetailsOverlay.Visibility = ViewStates.Gone;
                ShowHideKeyboard(myHomePaymentDetailsTxtNumber_1, false);
            };

            myHomePaymentDetailsLblBack.Text = Utility.GetLocalizedCommonLabel(LanguageConstants.Common.BACK);
            myHomePaymentDetailsLblBack.Click += delegate
            {
                myHomePaymentDetailsEnterCvvLayout.Visibility = ViewStates.Gone;
                myHomePaymentDetailsOverlay.Visibility = ViewStates.Gone;
                ShowHideKeyboard(myHomePaymentDetailsTxtNumber_1, false);
            };

            myHomePaymentDetailsTxtNumber_1.TextChanged += TxtNumber_1_TextChanged;
            myHomePaymentDetailsTxtNumber_2.TextChanged += TxtNumber_2_TextChanged;
            myHomePaymentDetailsTxtNumber_3.TextChanged += TxtNumber_3_TextChanged;
            myHomePaymentDetailsTxtNumber_4.TextChanged += TxtNumber_4_TextChanged;

            cardAdapter.NotifyDataSetChanged();
            myHomePaymentDetailsListAddedCards.Visibility = ViewStates.Visible;

            UpdateCardListViewHeight();

            myHomePaymentDetailsBtnAddCard.Click += delegate
            {
                DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
                if (pgCCEntity.IsDown)
                {
                    Utility.ShowBCRMDOWNTooltip(this, pgCCEntity, () => { });
                }
                else
                {
                    AddNewCard();
                    DynatraceHelper.OnTrack(DynatraceConstants.WEBVIEW_PAYMENT_CC);
                }
            };

            myHomePaymentDetailsBtnFPXPayment.Click += delegate
            {
                DownTimeEntity pgFPXEntity = DownTimeEntity.GetByCode(Constants.PG_FPX_SYSTEM);
                if (pgFPXEntity.IsDown)
                {
                    Utility.ShowBCRMDOWNTooltip(this, pgFPXEntity, () => { });
                }
                else
                {
                    selectedPaymentMethod = METHOD_FPX;
                    selectedCard = null;
                    //InitiatePaymentRequest();
                    DynatraceHelper.OnTrack(DynatraceConstants.WEBVIEW_PAYMENT_FPX);
                }
            };

            myHomePaymentDetailsBtnTNGPayment.Click += delegate
            {
                DownTimeEntity pgTNGEntity = DownTimeEntity.GetByCode(Constants.PG_TNG_SYSTEM);
                if (pgTNGEntity != null && pgTNGEntity.IsDown)
                {
                    Utility.ShowBCRMDOWNTooltip(this, pgTNGEntity, () => { });
                }
                else
                {
                    selectedPaymentMethod = METHOD_TNG;
                    selectedCard = null;
                    //InitiatePaymentRequest();
                    DynatraceHelper.OnTrack(DynatraceConstants.WEBVIEW_PAYMENT_TNG);
                }
            };

            if (TNGUtility.Instance.IsAccountEligible)
            {
                if (!MyTNBAccountManagement.GetInstance().IsTNGEnableVerify())
                {
                    myHomePaymentDetailsBtnTNGPayment.Visibility = ViewStates.Gone;
                    myHomePaymentDetailsTNGPaymentTitle.Visibility = ViewStates.Gone;
                }
                else
                {
                    myHomePaymentDetailsBtnTNGPayment.Visibility = ViewStates.Visible;
                    myHomePaymentDetailsTNGPaymentTitle.Visibility = ViewStates.Visible;
                    myHomePaymentDetailsBtnTNGPayment.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.tng, 0, 0, 0);
                }
            }
            else
            {
                myHomePaymentDetailsBtnTNGPayment.Visibility = ViewStates.Gone;
                myHomePaymentDetailsTNGPaymentTitle.Visibility = ViewStates.Gone;
            }
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

        void OnItemClick(object sender, int position)
        {
            try
            {
                DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
                if (pgCCEntity.IsDown)
                {
                    Utility.ShowBCRMDOWNTooltip(this, pgCCEntity, () => { });
                }
                else
                {
                    selectedPaymentMethod = METHOD_CREDIT_CARD;
                    if (IsValidPayableAmount())
                    {
                        selectedCard = cardAdapter.GetCardDetailsAt(position);
                        EnterCVVNumber(selectedCard);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void UpdateCardListViewHeight()
        {
            if (cardAdapter == null)
            {
                return;
            }

            int totalHeight = 0;
            int adapterCount = cardAdapter.Count;
            for (int size = 0; size < adapterCount; size++)
            {
                View listItem = cardAdapter.GetView(size, null, myHomePaymentDetailsListAddedCards);
                listItem.Measure(0, 0);
                totalHeight += listItem.MeasuredHeight;
            }

            myHomePaymentDetailsListAddedCards.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, totalHeight);
        }

        private bool IsValidPayableAmount()
        {
            bool isValid = true;

            AccountChargeModel accountChargeModel = this.presenter?.GetAccountChargeModel();

            if (accountChargeModel != null)
            {
                if (accountChargeModel.AmountDue < 1)
                {
                    isValid = false;
                    ErrorDialog(Utility.GetLocalizedErrorLabel(LanguageConstants.Error.MIN_PAY_AMOUNT));
                }
                else if (accountChargeModel.AmountDue > 5000 && selectedPaymentMethod.Equals(METHOD_CREDIT_CARD))
                {
                    isValid = false;
                    ErrorDialog(Utility.GetLocalizedLabel(LanguageConstants.SELECT_PAYMENT_METHOD, LanguageConstants.SelectPaymentMethod.MAX_CC_AMT_MSG));
                }
            }

            return isValid;
        }

        private void ErrorDialog(string message)
        {
            MyTNBAppToolTipBuilder eppTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                    .SetTitle("")
                    .SetMessage(message)
                    .SetCTALabel(Utility.GetLocalizedCommonLabel(LanguageConstants.Common.OK))
                    .SetCTAaction(() => { })
                    .Build();
            eppTooltip.Show();
        }

        private void EnterCVVNumber(CreditCard card)
        {
            if (myHomePaymentDetailsEnterCvvLayout.Visibility != ViewStates.Visible)
            {
                myHomePaymentDetailsEnterCvvLayout.Visibility = ViewStates.Visible;
                myHomePaymentDetailsTxtNumber_1.RequestFocus();
                myHomePaymentDetailsOverlay.Visibility = ViewStates.Visible;

                ShowHideKeyboard(myHomePaymentDetailsTxtNumber_1, true);

                myHomePaymentDetailsTxtNumber_1.Text = "";
                myHomePaymentDetailsTxtNumber_2.Text = "";
                myHomePaymentDetailsTxtNumber_3.Text = "";
                myHomePaymentDetailsTxtNumber_4.Text = "";
                if (card != null)
                {
                    if (card.CardType.Equals("AMEX") || card.CardType.Equals("A"))
                    {
                        myHomePaymentDetailsLblCVVInfo.Text = Utility.GetLocalizedLabel("SelectPaymentMethod", "cvvFourDigitMessage");
                        myHomePaymentDetailsTxtNumber_4.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        myHomePaymentDetailsLblCVVInfo.Text = Utility.GetLocalizedLabel("SelectPaymentMethod", "cvvThreeDigitMessage");
                        myHomePaymentDetailsTxtNumber_4.Visibility = ViewStates.Gone;
                    }
                }
            }
        }

        private void ShowHideKeyboard(EditText edt, bool flag)
        {
            try
            {
                InputMethodManager inputMethodManager = this.GetSystemService(Context.InputMethodService) as InputMethodManager;
                if (flag)
                {
                    inputMethodManager.ShowSoftInput(edt, ShowFlags.Forced);
                    inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
                }
                else
                {
                    inputMethodManager.HideSoftInputFromWindow(myHomePaymentDetailsMainLayout.WindowToken, 0);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void TxtNumber_1_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (e.Text.Count() == 1)
                {
                    myHomePaymentDetailsTxtNumber_1.ClearFocus();
                    myHomePaymentDetailsTxtNumber_2.RequestFocus();
                }
                CheckValidPin();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void TxtNumber_2_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (e.Text.Count() == 1)
                {
                    myHomePaymentDetailsTxtNumber_2.ClearFocus();
                    myHomePaymentDetailsTxtNumber_3.RequestFocus();
                }
                CheckValidPin();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void TxtNumber_3_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (e.Text.Count() == 1)
                {
                    myHomePaymentDetailsTxtNumber_3.ClearFocus();
                    myHomePaymentDetailsTxtNumber_4.RequestFocus();
                }
                CheckValidPin();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void TxtNumber_4_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckValidPin();
        }

        private void CheckValidPin()
        {
            try
            {
                string txt_1 = myHomePaymentDetailsTxtNumber_1.Text;
                string txt_2 = myHomePaymentDetailsTxtNumber_2.Text;
                string txt_3 = myHomePaymentDetailsTxtNumber_3.Text;
                string txt_4 = myHomePaymentDetailsTxtNumber_4.Text;
                if (TextUtils.IsEmpty(txt_1) || !TextUtils.IsDigitsOnly(txt_1))
                {
                    return;
                }

                if (TextUtils.IsEmpty(txt_2) || !TextUtils.IsDigitsOnly(txt_2))
                {
                    return;
                }

                if (TextUtils.IsEmpty(txt_3) || !TextUtils.IsDigitsOnly(txt_3))
                {
                    return;
                }

                if (selectedCard != null)
                {
                    if (selectedCard.CardType.Equals("AMEX") || selectedCard.CardType.Equals("A"))
                    {
                        if (TextUtils.IsEmpty(txt_4) || !TextUtils.IsDigitsOnly(txt_4))
                        {
                            return;
                        }
                    }
                }

                if (selectedCard != null)
                {

                    if (selectedCard.CardType.Equals("AMEX") || selectedCard.CardType.Equals("A"))
                    {
                        enteredCVV = string.Format("{0}{1}{2}{3}", txt_1, txt_2, txt_3, txt_4);
                    }
                    else
                    {
                        enteredCVV = string.Format("{0}{1}{2}", txt_1, txt_2, txt_3);
                    }

                    cardDetails = null;
                    myHomePaymentDetailsEnterCvvLayout.Visibility = ViewStates.Gone;
                    myHomePaymentDetailsOverlay.Visibility = ViewStates.Gone;
                    ShowHideKeyboard(myHomePaymentDetailsTxtNumber_1, false);
                    //InitiatePaymentRequest();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void AddNewCard()
        {
            try
            {
                selectedPaymentMethod = METHOD_CREDIT_CARD;
                if (IsValidPayableAmount())
                {
                    Intent nextIntent = new Intent();
                    nextIntent.PutExtra(MyHomeConstants.REGISTERED_CARDS, JsonConvert.SerializeObject(this.presenter?.GetRegisteredCards()));
                    nextIntent.SetClass(this, typeof(AddCardActivity));
                    StartActivityForResult(nextIntent, MyHomeConstants.PAYMENT_ADD_CARD_REQUEST_CODE);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            try
            {
                if (requestCode == MyHomeConstants.PAYMENT_ADD_CARD_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        if (data != null)
                        {
                            cardDetails = JsonConvert.DeserializeObject<MPCardDetails>(data.GetStringExtra("extra"));
                            selectedPaymentMethod = METHOD_CREDIT_CARD;
                            //InitiatePaymentRequest();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}

