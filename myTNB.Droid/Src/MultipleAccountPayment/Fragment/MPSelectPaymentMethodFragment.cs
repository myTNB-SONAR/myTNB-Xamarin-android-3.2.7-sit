using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Text;
using myTNB_Android.Src.AddCard.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Maintenance.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Models;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Adapter;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.MultipleAccountPayment.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static myTNB_Android.Src.MyTNBService.Request.PaymentTransactionIdRequest;
using System.Globalization;
using Google.Android.Material.Snackbar;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile;
using myTNB_Android.Src.SessionCache;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.DeviceCache;

namespace myTNB_Android.Src.MultipleAccountPayment.Fragment
{
    public class MPSelectPaymentMethodFragment : AndroidX.Fragment.App.Fragment, MPSelectPaymentMethodContract.IView
    {
        private string TOOL_BAR_TITLE = "Select Payment Method";
        private MPSelectPaymentMethodPresenter mPresenter;
        private MPSelectPaymentMethodContract.IUserActionsListener userActionsListener;

        private static int ADD_CARD_REQUEST_CDOE = 1001;
        private static MPCardDetails cardDetails;

        private static string METHOD_CREDIT_CARD = "CC";
        private static string METHOD_FPX = "FPX";
        private string param3 = "0";
        private string selectedPaymentMethod;
        private CreditCard selectedCard;

        FrameLayout baseView;
        EditText txtTotalAmount;
        TextView lblTotalAmount;
        TextView lblCreditDebitCard;
        TextView lblOtherPaymentMethods;

        TextView lblCvvInfo;
        TextView lblBack;
        EditText edtNumber1;
        EditText edtNumber2;
        EditText edtNumber3;
        EditText edtNumber4;
        private string enteredCVV;

        LinearLayout mainLayout;
        LinearLayout enterCvvLayout;
        View overlay;

        AccountData selectedAccount;
        private List<AccountChargeModel> accountChargeList;
        List<PaymentItem> selectedPaymentItemList;
        string total;
        ListView listAddedCards;
        MPAddCardAdapter cardAdapter;
        List<MPCardDetails> cards = new List<MPCardDetails>();
        List<CreditCard> registerdCards = new List<CreditCard>();

        Button btnAddCard;
        Button btnFPXPayment;

        private MaterialDialog mRequestingPaymentDialog;
        private MaterialDialog mGetRegisteredCardsDialog;
        private Snackbar mErrorMessageSnackBar;

        private SummaryDashBordRequest summaryDashBoardRequest = null;
        DecimalFormat decimalFormat = new DecimalFormat("#,###,###,###,##0.00", new DecimalFormatSymbols(Java.Util.Locale.Us));

        private bool isClicked = false;

        //Mark: Application Payment
        private bool IsApplicationPayment;
        private ApplicationPaymentDetail ApplicationPaymentDetail;
        private string ApplicationType = string.Empty;
        private string SearchTerm = string.Empty;
        private string ApplicationSystem = string.Empty;
        private string StatusId = string.Empty;
        private string StatusCode = string.Empty;

        public bool IsActive()
        {
            return IsVisible;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

        }

        public override void OnPause()
        {
            base.OnPause();
            isClicked = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View rootView = inflater.Inflate(Resource.Layout.SelectPaymentMethodView, container, false);
            selectedPaymentItemList = new List<PaymentItem>();
            try
            {
                mPresenter = new MPSelectPaymentMethodPresenter(this);
                mRequestingPaymentDialog = new MaterialDialog.Builder(Activity)
                    .Title(GetString(Resource.String.initiate_payment_progress_title))
                    .Content(GetString(Resource.String.initiate_payment_progress_message))
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                mGetRegisteredCardsDialog = new MaterialDialog.Builder(Activity)
                    .Title(GetString(Resource.String.getregisteredcards_progress_title))
                    .Content(GetString(Resource.String.getregisteredcards_progress_message))
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                ((PaymentActivity)Activity).SetToolBarTitle(Utility.GetLocalizedLabel("SelectPaymentMethod", "title"));
                if (Arguments.ContainsKey("ISAPPLICATIONPAYMENT") && Arguments.GetBoolean("ISAPPLICATIONPAYMENT"))
                {
                    IsApplicationPayment = true;

                    if (Arguments.ContainsKey("APPLICATIONPAYMENTDETAILS"))
                    {
                        ApplicationPaymentDetail = JsonConvert.DeserializeObject<ApplicationPaymentDetail>(Arguments.GetString("APPLICATIONPAYMENTDETAILS"));
                    }
                    if (Arguments.ContainsKey("ApplicationType"))
                    {
                        ApplicationType = Arguments.GetString("ApplicationType");
                    }
                    if (Arguments.ContainsKey("SearchTerm"))
                    {
                        SearchTerm = Arguments.GetString("SearchTerm");
                    }
                    if (Arguments.ContainsKey("ApplicationSystem"))
                    {
                        ApplicationSystem = Arguments.GetString("ApplicationSystem");
                    }
                    if (Arguments.ContainsKey("StatusId"))
                    {
                        StatusId = Arguments.GetString("StatusId");
                    }
                    if (Arguments.ContainsKey("StatusCode"))
                    {
                        StatusCode = Arguments.GetString("StatusCode");
                    }
                }

                if (Arguments.ContainsKey(Constants.SELECTED_ACCOUNT))
                {
                    selectedAccount = JsonConvert.DeserializeObject<AccountData>(Arguments.GetString(Constants.SELECTED_ACCOUNT));
                }
                if (Arguments.ContainsKey("ACCOUNT_CHARGES_LIST"))
                {
                    accountChargeList = JsonConvert.DeserializeObject<List<AccountChargeModel>>(Arguments.GetString("ACCOUNT_CHARGES_LIST"));
                }
                if (Arguments.ContainsKey("PAYMENT_ITEMS"))
                {
                    List<MPAccount> accounts = JsonConvert.DeserializeObject<List<MPAccount>>(Arguments.GetString("PAYMENT_ITEMS"));
                    if (accounts != null)
                    {
                        Activity.RunOnUiThread(async () =>
                        {
                            if (DBRUtility.Instance.ShouldShowHomeDBRCard)
                            {
                                List<string> dbrCAForPaymentList = new List<string>();
                                List<string> dbrCAList = EligibilitySessionCache.Instance.IsFeatureEligible(EligibilitySessionCache.Features.DBR
                                        , EligibilitySessionCache.FeatureProperty.TargetGroup)
                                    ? DBRUtility.Instance.GetDBRCAs()
                                    : AccountTypeCache.Instance.DBREligibleCAs;
                                for (int i = 0; i < dbrCAList.Count; i++)
                                {
                                    int index = accounts.FindIndex(x => x.accountNumber == dbrCAList[i]);
                                    if (index > -1)
                                    {
                                        dbrCAForPaymentList.Add(accounts[index].accountNumber);
                                    }
                                }

                                if (dbrCAForPaymentList != null && dbrCAForPaymentList.Count > 0)
                                {
                                    PostMultiBillRenderingResponse multiBillRenderingResponse = await DBRManager.Instance.PostMultiBillRendering(dbrCAForPaymentList
                                        , AccessTokenCache.Instance.GetAccessToken(Activity));
                                    if (multiBillRenderingResponse != null
                                        && multiBillRenderingResponse.StatusDetail != null
                                        && multiBillRenderingResponse.StatusDetail.IsSuccess
                                        && multiBillRenderingResponse.Content != null
                                        && multiBillRenderingResponse.Content.Count > 0)
                                    {
                                        for (int j = 0; j < dbrCAForPaymentList.Count; j++)
                                        {
                                            int index = multiBillRenderingResponse.Content.FindIndex(x =>
                                                x.ContractAccountNumber == dbrCAForPaymentList[j]
                                                && x.DBRType == MobileEnums.DBRTypeEnum.Paper);
                                            if (index > -1)
                                            {
                                                PaymentActivity.CAsWithPaperBillList.Add(dbrCAForPaymentList[index]);
                                            }
                                        }
                                    }
                                }
                            }

                            foreach (MPAccount item in accounts)
                            {
                                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(item.accountNumber);
                                AccountChargeModel chargeModel = accountChargeList.Find(accountCharge =>
                                {
                                    return accountCharge.ContractAccount == item.accountNumber;
                                });

                                CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                                if (chargeModel != null)
                                {
                                    if (chargeModel.MandatoryCharges.TotalAmount > 0f)
                                    {
                                        PaymentItemAccountPayment paymentItemAccountPayment = new PaymentItemAccountPayment();
                                        paymentItemAccountPayment.AccountOwnerName = customerBillingAccount.OwnerName;
                                        paymentItemAccountPayment.AccountNo = chargeModel.ContractAccount;
                                        paymentItemAccountPayment.AccountAmount = item.amount.ToString(currCult);
                                        paymentItemAccountPayment.dbrEnabled = PaymentActivity.CAsWithPaperBillList.FindIndex(x => x == item.accountNumber) > -1;

                                        List<AccountPayment> accountPaymentList = new List<AccountPayment>();
                                        chargeModel.MandatoryCharges.ChargeModelList.ForEach(charge =>
                                        {
                                            AccountPayment accountPayment = new AccountPayment();
                                            accountPayment.PaymentType = charge.Key;
                                            accountPayment.PaymentAmount = charge.Amount.ToString(currCult);
                                            accountPaymentList.Add(accountPayment);
                                        });
                                        paymentItemAccountPayment.AccountPayments = accountPaymentList;
                                        selectedPaymentItemList.Add(paymentItemAccountPayment);
                                    }
                                    else
                                    {
                                        PaymentItem payItem = new PaymentItem();
                                        payItem.AccountOwnerName = customerBillingAccount.OwnerName;
                                        payItem.AccountNo = chargeModel.ContractAccount;
                                        payItem.AccountAmount = item.amount.ToString(currCult);
                                        payItem.dbrEnabled = PaymentActivity.CAsWithPaperBillList.FindIndex(x => x == item.accountNumber) > -1;
                                        selectedPaymentItemList.Add(payItem);
                                    }
                                }
                            }
                        });
                    }
                }
                if (Arguments.ContainsKey("TOTAL"))
                {
                    total = Arguments.GetString("TOTAL");
                }
                if (selectedPaymentItemList.Count > 1)
                {
                    param3 = "1";
                }
                else
                {
                    param3 = "0";
                }

                baseView = rootView.FindViewById<FrameLayout>(Resource.Id.baseView);
                txtTotalAmount = rootView.FindViewById<EditText>(Resource.Id.amount_edittext);
                lblTotalAmount = rootView.FindViewById<TextView>(Resource.Id.lblTotalAmount);
                lblCreditDebitCard = rootView.FindViewById<TextView>(Resource.Id.lblCreditDebitCard);
                lblOtherPaymentMethods = rootView.FindViewById<TextView>(Resource.Id.lblOtherPayment);

                lblCvvInfo = rootView.FindViewById<TextView>(Resource.Id.lblCVVInfo);
                lblBack = rootView.FindViewById<TextView>(Resource.Id.lblBack);
                edtNumber1 = rootView.FindViewById<EditText>(Resource.Id.txtNumber_1);
                edtNumber2 = rootView.FindViewById<EditText>(Resource.Id.txtNumber_2);
                edtNumber3 = rootView.FindViewById<EditText>(Resource.Id.txtNumber_3);
                edtNumber4 = rootView.FindViewById<EditText>(Resource.Id.txtNumber_4);
                edtNumber1.TextChanged += TxtNumber_1_TextChanged;
                edtNumber2.TextChanged += TxtNumber_2_TextChanged;
                edtNumber3.TextChanged += TxtNumber_3_TextChanged;
                edtNumber4.TextChanged += TxtNumber_4_TextChanged;

                mainLayout = rootView.FindViewById<LinearLayout>(Resource.Id.mainLayout);
                enterCvvLayout = rootView.FindViewById<LinearLayout>(Resource.Id.enterCvvLayout);
                overlay = rootView.FindViewById<View>(Resource.Id.overlay);
                overlay.Visibility = ViewStates.Gone;
                overlay.Click += delegate
                {
                    enterCvvLayout.Visibility = ViewStates.Gone;
                    overlay.Visibility = ViewStates.Gone;
                    ShowHideKeyboard(edtNumber1, false);

                };

                TextViewUtils.SetMuseoSans500Typeface(lblBack);
                lblBack.Text = Utility.GetLocalizedCommonLabel("back");
                lblBack.Click += delegate
                {
                    enterCvvLayout.Visibility = ViewStates.Gone;
                    overlay.Visibility = ViewStates.Gone;
                    ShowHideKeyboard(edtNumber1, false);
                };

                btnAddCard = rootView.FindViewById<Button>(Resource.Id.btnAddCard);
                btnAddCard.Click += delegate
                {
                    DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
                    if (pgCCEntity.IsDown)
                    {
                        ShowErrorMessage(pgCCEntity.DowntimeMessage);
                    }
                    else
                    {
                        HideErrorMessageSnakebar();
                        AddNewCard();
                        DynatraceHelper.OnTrack(DynatraceConstants.WEBVIEW_PAYMENT_CC);
                    }
                };

                btnFPXPayment = rootView.FindViewById<Button>(Resource.Id.btnFPXPayment);
                btnFPXPayment.Click += delegate
                {
                    DownTimeEntity pgFPXEntity = DownTimeEntity.GetByCode(Constants.PG_FPX_SYSTEM);
                    if (pgFPXEntity.IsDown)
                    {
                        ShowErrorMessage(pgFPXEntity.DowntimeMessage);
                    }
                    else
                    {
                        HideErrorMessageSnakebar();
                        selectedPaymentMethod = METHOD_FPX;
                        selectedCard = null;
                        InitiatePaymentRequest();
                        DynatraceHelper.OnTrack(DynatraceConstants.WEBVIEW_PAYMENT_FPX);
                    }
                };

                listAddedCards = rootView.FindViewById<ListView>(Resource.Id.listAddedCards);
                cardAdapter = new MPAddCardAdapter(Activity, registerdCards);
                listAddedCards.Adapter = cardAdapter;
                cardAdapter.OnItemClick += OnItemClick;

                TextViewUtils.SetMuseoSans300Typeface(lblTotalAmount);
                TextViewUtils.SetMuseoSans500Typeface(lblCreditDebitCard, lblOtherPaymentMethods, txtTotalAmount);
                TextViewUtils.SetMuseoSans300Typeface(btnAddCard, btnFPXPayment);

                TextViewUtils.SetTextSize10(lblTotalAmount);
                TextViewUtils.SetTextSize16(txtTotalAmount, btnFPXPayment, btnAddCard);
                TextViewUtils.SetTextSize18(lblCreditDebitCard, lblOtherPaymentMethods);
                TextViewUtils.SetTextSize22(edtNumber1, edtNumber2, edtNumber3, edtNumber4);

                TextViewUtils.SetMuseoSans300Typeface(lblCvvInfo);
                TextViewUtils.SetMuseoSans300Typeface(edtNumber1, edtNumber2, edtNumber3, edtNumber4);

                lblCreditDebitCard.Text = Utility.GetLocalizedLabel("Common", "cards");
                lblOtherPaymentMethods.Text = Utility.GetLocalizedLabel("SelectPaymentMethod", "otherPaymentMethods");
                lblTotalAmount.Text = Utility.GetLocalizedLabel("Common", "totalAmountRM").ToUpper();
                btnAddCard.Text = Utility.GetLocalizedLabel("SelectPaymentMethod", "addCard");
                btnFPXPayment.Text = Utility.GetLocalizedLabel("SelectPaymentMethod", "fpxTitle");

                //if(selectedAccount != null){

                //    txtTotalAmount.Text = decimalFormat.Format(selectedAccount.AmtCustBal).Replace(",","");
                //}
                if (total != null)
                {
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                    txtTotalAmount.Text = decimalFormat.Format(double.Parse(total, currCult)).Replace(",", "");
                    txtTotalAmount.Enabled = false;
                    txtTotalAmount.ShowSoftInputOnFocus = false;
                }

                GetRegisteredCards();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] OnCreate Error: " + e.Message);
                Utility.LoggingNonFatalError(e);
            }
            return rootView;
        }

        public override void OnResume()
        {
            ((PaymentActivity)Activity).SetToolBarTitle(Utility.GetLocalizedLabel("SelectPaymentMethod", "title"));
            base.OnResume();

            isClicked = false;
        }

        void OnItemClick(object sender, int position)
        {
            try
            {
                DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
                if (pgCCEntity.IsDown)
                {
                    ShowErrorMessage(pgCCEntity.DowntimeMessage);
                }
                else
                {
                    HideErrorMessageSnakebar();
                    selectedPaymentMethod = METHOD_CREDIT_CARD;
                    if (IsValidPayableAmount())
                    {
                        selectedCard = cardAdapter.GetCardDetailsAt(position);
                        //cardDetails = null;
                        //InitiatePaymentRequest();
                        EnterCVVNumber(selectedCard); // -- CVV Enabled --
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void AddNewCard()
        {
            try
            {
                selectedPaymentMethod = METHOD_CREDIT_CARD;
                if (IsValidPayableAmount())
                {
                    if (!isClicked)
                    {
                        isClicked = true;
                        Intent nextIntent = new Intent();
                        nextIntent.PutExtra("registeredCards", JsonConvert.SerializeObject(registerdCards));
                        nextIntent.SetClass(Activity, typeof(AddCardActivity));
                        StartActivityForResult(nextIntent, ADD_CARD_REQUEST_CDOE);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateListViewHeight(ListView myListView)
        {
            try
            {
                if (cardAdapter == null)
                {
                    return;
                }
                // get listview height
                int totalHeight = 0;
                int adapterCount = cardAdapter.Count;
                for (int size = 0; size < adapterCount; size++)
                {
                    View listItem = cardAdapter.GetView(size, null, myListView);
                    listItem.Measure(0, 0);
                    totalHeight += listItem.MeasuredHeight;
                }
                // Change Height of ListView
                myListView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, totalHeight);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetPresenter(MPSelectPaymentMethodContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            //throw new NotImplementedException();
        }

        public void EnterCVVNumber(CreditCard card)
        {
            try
            {
                if (enterCvvLayout.Visibility != ViewStates.Visible)
                {
                    enterCvvLayout.Visibility = ViewStates.Visible;
                    edtNumber1.RequestFocus();
                    overlay.Visibility = ViewStates.Visible;

                    ShowHideKeyboard(edtNumber1, true);

                    edtNumber1.Text = "";
                    edtNumber2.Text = "";
                    edtNumber3.Text = "";
                    edtNumber4.Text = "";
                    if (card != null)
                    {
                        if (card.CardType.Equals("AMEX") || card.CardType.Equals("A"))
                        {
                            lblCvvInfo.Text = Utility.GetLocalizedLabel("SelectPaymentMethod", "cvvFourDigitMessage");
                            edtNumber4.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            lblCvvInfo.Text = Utility.GetLocalizedLabel("SelectPaymentMethod", "cvvThreeDigitMessage");
                            edtNumber4.Visibility = ViewStates.Gone;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowHideKeyboard(EditText edt, bool flag)
        {
            try
            {
                InputMethodManager inputMethodManager = Activity.GetSystemService(Context.InputMethodService) as InputMethodManager;
                if (flag)
                {
                    inputMethodManager.ShowSoftInput(edt, ShowFlags.Forced);
                    inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
                }
                else
                {
                    inputMethodManager.HideSoftInputFromWindow(mainLayout.WindowToken, 0);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowPaymentRequestDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this.Activity);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HidePaymentRequestDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this.Activity);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InitiatePaymentRequest()
        {
            try
            {
                if (IsValidPayableAmount())
                {
                    string custName = string.Empty;
                    if (IsApplicationPayment)
                    {
                        custName = UserEntity.GetActive().DisplayName ?? string.Empty;
                    }
                    else if (selectedPaymentItemList != null && selectedPaymentItemList.Count > 0)
                    {
                        custName = selectedPaymentItemList.Count > 1 ? UserEntity.GetActive().DisplayName ?? string.Empty
                            : selectedPaymentItemList[0].AccountOwnerName ?? string.Empty;
                    }

                    string custPhone = string.IsNullOrEmpty(UserEntity.GetActive().MobileNo)
                        ? string.Empty
                        : UserEntity.GetActive().MobileNo ?? string.Empty;
                    string platform = Constants.DEVICE_PLATFORM; // 1 Android
                    string paymentMode = selectedPaymentMethod;
                    /* Get user registered cards */
                    string registeredCardId = selectedCard == null ? string.Empty : selectedCard.Id;
                    DeletePaymentHistory();

                    if (IsApplicationPayment)
                    {
                        MyTNBService.Request.BaseRequest baseRequest = new MyTNBService.Request.BaseRequest();
                        this.userActionsListener.InitializeApplicationPaymentTransaction(baseRequest.usrInf
                            , custName
                            , custPhone
                            , platform
                            , registeredCardId
                            , paymentMode
                            , total
                            , ApplicationType
                            , SearchTerm
                            , ApplicationSystem
                            , StatusId
                            , StatusCode
                            , ApplicationPaymentDetail);
                    }
                    else
                    {
                        this.userActionsListener.InitializePaymentTransaction(custName
                            , custPhone
                            , platform
                            , registeredCardId
                            , paymentMode
                            , total
                            , selectedPaymentItemList);
                    }
                }
                else
                {
                    //txtTotalAmount.Error = "Please Enter Valid Payable Amount";
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] InitiatePaymentRequest: " + e.Message);
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            try
            {
                //base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == ADD_CARD_REQUEST_CDOE)
                {
                    if (resultCode == (int)Result.Ok)
                    {
                        if (data != null)
                        {
                            cardDetails = JsonConvert.DeserializeObject<MPCardDetails>(data.GetStringExtra("extra"));
                            //cards.Add(card);
                            //cardAdapter.NotifyDataSetChanged();
                            //listAddedCards.Visibility = ViewStates.Visible;
                            //UpdateListViewHeight(listAddedCards);
                            selectedPaymentMethod = METHOD_CREDIT_CARD;
                            InitiatePaymentRequest();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InitiateSubmitPayment(PaymentTransactionIdResponse paymentResponse, MPCardDetails card)
        {
            try
            {
                string apiKeyID = Constants.APP_CONFIG.API_KEY_ID;
                PaymentTransactionIdResponse.InitiatePaymentResult initiatePaymentResult = paymentResponse.GetData();
                string action = initiatePaymentResult.action;
                string merchantId = initiatePaymentResult.payMerchantID;
                string merchantTransId = initiatePaymentResult.payMerchant_transID;
                string currencyCode = initiatePaymentResult.payCurrencyCode;
                string payAm = initiatePaymentResult.payAmount;

                string custEmail = initiatePaymentResult.payCustEmail;
                string custName = initiatePaymentResult.payCustName;
                string des = initiatePaymentResult.payProdDesc;
                string returnURL = initiatePaymentResult.payReturnUrl;
                string signature = initiatePaymentResult.paySign;
                string mparam1 = initiatePaymentResult.payMParam;
                string payMethod = initiatePaymentResult.payMethod;
                string platform = initiatePaymentResult.platform;
                string accNum = selectedAccount?.AccountNum ?? string.Empty;

                string transType = initiatePaymentResult.transactionType;
                string tokenizedHashCodeCC = initiatePaymentResult.tokenizedHashCodeCC;
                string custPhone = initiatePaymentResult.payCustPhoneNum;

                Bundle bundle = new Bundle();
                bundle.PutString("apiKeyID", apiKeyID);
                bundle.PutString("action", action);
                bundle.PutString("merchantId", merchantId);
                bundle.PutString("merchantTransId", merchantTransId);
                bundle.PutString("currencyCode", currencyCode);
                bundle.PutString("payAm", payAm);

                bundle.PutString("custName", custName);
                bundle.PutString("custEmail", custEmail);
                bundle.PutString("des", des);
                bundle.PutString("custPhone", custPhone);
                bundle.PutString("returnURL", returnURL);
                bundle.PutString("signature", signature);
                bundle.PutString("mparam1", mparam1);
                bundle.PutString("payMethod", payMethod);
                bundle.PutString("platform", platform);
                bundle.PutString("accNum", accNum);

                bundle.PutString("transType", transType);
                bundle.PutString("tokenizedHashCodeCC", tokenizedHashCodeCC);

                bundle.PutString("SummaryDashBoardRequest", JsonConvert.SerializeObject(summaryDashBoardRequest));

                if (card != null)
                {
                    string cardNo = card.cardNo;
                    string cardExpM = card.CardExpMonth;
                    string cardExpY = card.CardExpYear;
                    string cardCvv = card.cardCVV;
                    string cardType = card.GetCreditCardType();
                    bool saveCard = card.saveCard;

                    bundle.PutString("cardNo", cardNo);
                    bundle.PutString("cardExpM", cardExpM);
                    bundle.PutString("cardExpY", cardExpY);
                    bundle.PutString("cardCvv", cardCvv);
                    bundle.PutString("cardType", cardType);
                    bundle.PutBoolean("saveCard", saveCard);
                }
                else
                {
                    bundle.PutBoolean("registeredCard", true);
                    bundle.PutString("cardCvv", enteredCVV);
                }


            ((PaymentActivity)Activity).NextFragment(this, bundle);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            //this.userActionsListener.SubmitPayment(apiKeyID, merchantId, accNum, payAm, custName, custEmail, custPhone, mparam1, des, cardNo, custName, cardExpM, cardExpY, cardCvv);
        }

        //public void SaveInitiatePaymentResponse(MPInitiatePaymentResponse response)
        //{
        //    try
        //    {
        //        if (response != null)
        //        {
        //            Log.Debug("Initiate Payment Response", "Response Count" + response.ToString());
        //            if (response.requestPayBill.IsError)
        //            {
        //                ShowErrorMessage(response.requestPayBill.Message);
        //            }
        //            else
        //            {
        //                if (selectedPaymentMethod.Equals(METHOD_CREDIT_CARD))
        //                {
        //                    InitiateSubmitPayment(response, cardDetails);
        //                }
        //                else
        //                {
        //                    InitiateFPXPayment(response);
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Utility.LoggingNonFatalError(e);
        //    }
        //}

        public void ShowErrorMessage(string message)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(baseView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }

        public void ShowErrorMessageWithOK(string message)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(baseView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("ok"), delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(6);

            mErrorMessageSnackBar.Show();
        }

        private void HideErrorMessageSnakebar()
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }
        }

        public void InitiateWebView(string response)
        {
            Bundle bundle = new Bundle();
            bundle.PutString("html", response);
            ((PaymentActivity)Activity).NextFragment(this, bundle);
        }

        public void InitiateFPXPayment(PaymentTransactionIdResponse response)
        {
            try
            {
                PaymentTransactionIdResponse.InitiatePaymentResult initiatePaymentResult = response.GetData();
                string parameter1 = "Param1=3";
                string parameter2 = "Param2=" + initiatePaymentResult.payMerchant_transID;
                string parameter3 = "Param3=" + param3;
                string langProp = "lang=" + LanguageUtil.GetAppLanguage().ToUpper();
                var uri = Android.Net.Uri.Parse(initiatePaymentResult.action +
                    "?" + parameter1 + "&" + parameter2 + "&" + parameter3 + "&" + langProp);

                Bundle bundle = new Bundle();
                bundle.PutString("html_fpx", uri.ToString());
                bundle.PutString("SummaryDashBoardRequest", JsonConvert.SerializeObject(summaryDashBoardRequest));
                ((PaymentActivity)Activity).NextFragment(this, bundle);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public bool IsValidPayableAmount()
        {
            bool isValid = true;
            try
            {
                if (String.IsNullOrEmpty(txtTotalAmount.Text) || txtTotalAmount.Text.Equals("0.00"))
                {
                    isValid = false;
                    txtTotalAmount.Error = "Please Selcet/Enter Valid Payable Amount";
                }
                else
                {
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                    double payableAmt = double.Parse(txtTotalAmount.Text, currCult);
                    if (payableAmt < 1)
                    {
                        isValid = false;
                        //txtTotalAmount.Error = "Minimum amount for online payment is 1RM";
                        ShowErrorMessage("Minimum amount for online payment is 1RM");
                    }
                    else if (payableAmt > 5000 && selectedPaymentMethod.Equals(METHOD_CREDIT_CARD))
                    {
                        isValid = false;
                        //txtTotalAmount.Error = "For payments more than RM 5000, please use FPX payment mode.";
                        ShowErrorMessage(Utility.GetLocalizedLabel("SelectPaymentMethod", "maxCCAmountMessage"));
                    }

                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return isValid;
        }

        private void GetRegisteredCards()
        {
            string apiKeyID = Constants.APP_CONFIG.API_KEY_ID;
            string custEmail = UserEntity.GetActive().Email;
            this.userActionsListener.GetRegisterdCards(apiKeyID, custEmail);
        }

        public void ShowGetRegisteredCardDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this.Activity);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideGetRegisteredCardDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this.Activity);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void GetRegisterCardsResult(RegisteredCardsResponse response)
        {
            try
            {
                if (response != null)
                {
                    if (!response.IsSuccessResponse())
                    {
                        ShowErrorMessageWithOK(Utility.GetLocalizedErrorLabel("paymentCCErrorMsg"));
                    }
                    else
                    {
                        if (response.GetData() != null)
                        {
                            if (response.GetData().Count() > 0)
                            {
                                List<CreditCard> cards = response.GetData();
                                foreach (CreditCard card in cards)
                                {
                                    registerdCards.Add(card);
                                }
                                cardAdapter.NotifyDataSetChanged();
                                listAddedCards.Visibility = ViewStates.Visible;
                                UpdateListViewHeight(listAddedCards);
                            }
                        }
                        else
                        {
                            Log.Debug("Card Data", "Card Data : No Data!!!");
                        }
                    }
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
                    edtNumber1.ClearFocus();
                    edtNumber2.RequestFocus();
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
                    edtNumber2.ClearFocus();
                    edtNumber3.RequestFocus();
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
                    edtNumber3.ClearFocus();
                    edtNumber4.RequestFocus();
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
                string txt_1 = edtNumber1.Text;
                string txt_2 = edtNumber2.Text;
                string txt_3 = edtNumber3.Text;
                string txt_4 = edtNumber4.Text;
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
                    enterCvvLayout.Visibility = ViewStates.Gone;
                    overlay.Visibility = ViewStates.Gone;
                    ShowHideKeyboard(edtNumber1, false);
                    InitiatePaymentRequest();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DeletePaymentHistory()
        {
            try
            {
                List<string> accounts = new List<string>();
                foreach (PaymentItem item in selectedPaymentItemList)
                {
                    if (item.AccountNo != null)
                    {
                        PaymentHistoryEntity.RemoveAccountData(item.AccountNo);
                        AccountDataEntity.RemoveAccountData(item.AccountNo);
                        SelectBillsEntity.RemoveAll();
                        SummaryDashBoardAccountEntity.RemoveAll();
                        //SummaryDashBoardAccountEntity.RemoveAll();
                        //SummaryDashBoardAccountEntity.RemoveAccountData(item.AccountNo);
                        SummaryDashBoardAccountEntity summaryEntity = SummaryDashBoardAccountEntity.GetItemByAccountNo(item.AccountNo);
                        if (summaryEntity != null)
                        {
                            accounts.Add(item.AccountNo);
                        }
                    }
                }

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = DeviceId(this.Activity),
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                summaryDashBoardRequest = new SummaryDashBordRequest();
                summaryDashBoardRequest.AccNum = accounts;
                summaryDashBoardRequest.usrInf = currentUsrInf;

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            //SummaryDashBoardApiCall.GetSummaryDetails(summaryDashBoardRequest);
        }

        private string DeviceId(Android.App.Activity mActivity)
        {
            var deviceUuid = "";
            try
            {
                var androidID = Android.Provider.Settings.Secure.GetString(mActivity.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                deviceUuid = DeviceIdUtils.GenerateDeviceIdentifier(mActivity, androidID);
                return deviceUuid.ToString();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return deviceUuid.ToString();
        }

        public void SetInitiatePaymentResponse(PaymentTransactionIdResponse response)
        {
            try
            {
                if (response != null && response.Response != null)
                {
                    Log.Debug("Initiate Payment Response", "Response Count" + response.ToString());
                    if (response.IsSuccessResponse())
                    {
                        if (selectedPaymentMethod.Equals(METHOD_CREDIT_CARD))
                        {
                            InitiateSubmitPayment(response, cardDetails);
                        }
                        else
                        {
                            InitiateFPXPayment(response);
                        }
                    }
                    else if (response.Response.ErrorCode == "7000")
                    {
                        string title = "";
                        string message = "";
                        if (!string.IsNullOrEmpty(response.Response.DisplayTitle))
                        {
                            title = response.Response.DisplayTitle;
                        }
                        if (!string.IsNullOrEmpty(response.Response.DisplayMessage))
                        {
                            message = response.Response.DisplayMessage;
                        }
                        Intent maintenanceScreen = new Intent(this.Activity, typeof(MaintenanceActivity));
                        maintenanceScreen.PutExtra(Constants.MAINTENANCE_TITLE_KEY, title);
                        maintenanceScreen.PutExtra(Constants.MAINTENANCE_MESSAGE_KEY, message);
                        StartActivity(maintenanceScreen);
                    }
                    else
                    {
                        string txt = "";
                        if (!string.IsNullOrEmpty(response.Response.DisplayMessage))
                        {
                            txt = response.Response.DisplayMessage;
                        }
                        else
                        {
                            txt = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
                        }
                        ShowErrorMessage(response.Response.DisplayMessage);
                    }

                }
                else
                {
                    ShowErrorMessage(Utility.GetLocalizedErrorLabel("defaultErrorMessage"));
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
