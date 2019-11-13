using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Text;
using myTNB_Android.Src.AddCard.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Adapter;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.MultipleAccountPayment.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static myTNB_Android.Src.MyTNBService.Request.PaymentTransactionIdRequest;

namespace myTNB_Android.Src.MultipleAccountPayment.Fragment
{
    public class MPSelectPaymentMethodFragment : Android.App.Fragment, MPSelectPaymentMethodContract.IView
    {

        private string TOOL_BAR_TITLE = "Select Payment Method";
        private MPSelectPaymentMethodPresenter mPresenter;
        private MPSelectPaymentMethodContract.IUserActionsListener userActionsListener;

        private static int ADD_CARD_REQUEST_CDOE = 1001;
        private static MPCardDetails cardDetails;

        private static string METHOD_CREDIT_CARD = "CC";
        private static string METHOD_FPX = "FPX";
        private static string PARAM3 = "Param3=";
        private string param3 = "0";
        private string selectedPaymentMethod;
        private MPCreditCard selectedCard;

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
        List<MPCreditCard> registerdCards = new List<MPCreditCard>();

        Button btnAddCard;
        Button btnFPXPayment;

        private MaterialDialog mRequestingPaymentDialog;
        private MaterialDialog mGetRegisteredCardsDialog;
        private LoadingOverlay loadingOverlay;
        private Snackbar mErrorMessageSnackBar;


        private SummaryDashBordRequest summaryDashBoardRequest = null;
        DecimalFormat decimalFormat = new DecimalFormat("#,###,###,###,##0.00");

        private bool isClicked = false;

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

                ((PaymentActivity)Activity).SetToolBarTitle(TOOL_BAR_TITLE);
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(Arguments.GetString(Constants.SELECTED_ACCOUNT));
                accountChargeList = JsonConvert.DeserializeObject<List<AccountChargeModel>>(Arguments.GetString("ACCOUNT_CHARGES_LIST"));
                List <MPAccount> accounts = JsonConvert.DeserializeObject<List<MPAccount>>(Arguments.GetString("PAYMENT_ITEMS"));
                foreach (MPAccount item in accounts)
                {
                    CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(item.accountNumber);
                    AccountChargeModel chargeModel = accountChargeList.Find(accountCharge =>
                    {
                        return accountCharge.ContractAccount == item.accountNumber;
                    });

                    if (chargeModel != null)
                    {
                        if (chargeModel.MandatoryCharges.TotalAmount > 0f)
                        {
                            PaymentItemAccountPayment paymentItemAccountPayment = new PaymentItemAccountPayment();
                            paymentItemAccountPayment.AccountOwnerName = customerBillingAccount.OwnerName;
                            paymentItemAccountPayment.AccountNo = chargeModel.ContractAccount;
                            paymentItemAccountPayment.AccountAmount = item.amount.ToString();

                            List<AccountPayment> accountPaymentList = new List<AccountPayment>();
                            chargeModel.MandatoryCharges.ChargeModelList.ForEach(charge =>
                            {
                                AccountPayment accountPayment = new AccountPayment();
                                accountPayment.PaymentType = charge.Key;
                                accountPayment.PaymentAmount = charge.Amount.ToString();
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
                            payItem.AccountAmount = item.amount.ToString();
                            selectedPaymentItemList.Add(payItem);
                        }
                    }

                }
                total = Arguments.GetString("TOTAL");
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
                        AddNewCard();
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
                        selectedPaymentMethod = METHOD_FPX;
                        selectedCard = null;
                        InitiatePaymentRequest();
                    }

                };

                listAddedCards = rootView.FindViewById<ListView>(Resource.Id.listAddedCards);
                cardAdapter = new MPAddCardAdapter(Activity, registerdCards);
                listAddedCards.Adapter = cardAdapter;
                cardAdapter.OnItemClick += OnItemClick;

                TextViewUtils.SetMuseoSans300Typeface(lblTotalAmount);
                TextViewUtils.SetMuseoSans500Typeface(lblCreditDebitCard, lblOtherPaymentMethods, txtTotalAmount);
                TextViewUtils.SetMuseoSans300Typeface(btnAddCard, btnFPXPayment);

                TextViewUtils.SetMuseoSans300Typeface(lblCvvInfo);
                TextViewUtils.SetMuseoSans300Typeface(edtNumber1, edtNumber2, edtNumber3, edtNumber4);

                //if(selectedAccount != null){

                //    txtTotalAmount.Text = decimalFormat.Format(selectedAccount.AmtCustBal).Replace(",","");
                //}
                if (total != null)
                {
                    txtTotalAmount.Text = decimalFormat.Format(double.Parse(total)).Replace(",", "");
                    txtTotalAmount.Enabled = false;
                    txtTotalAmount.ShowSoftInputOnFocus = false;
                }

                GetRegisteredCards();

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return rootView;
        }

        public override void OnResume()
        {
            ((PaymentActivity)Activity).SetToolBarTitle(TOOL_BAR_TITLE);
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

        public void EnterCVVNumber(MPCreditCard card)
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
                            lblCvvInfo.Text = Activity.GetString(Resource.String.cvv_info_4_digit);
                            edtNumber4.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            lblCvvInfo.Text = Activity.GetString(Resource.String.cvv_info_3_digit);
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
            //if (this.mRequestingPaymentDialog != null && !this.mRequestingPaymentDialog.IsShowing)
            //{
            //    this.mRequestingPaymentDialog.Show();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(Activity, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HidePaymentRequestDialog()
        {
            //if (this.mRequestingPaymentDialog != null && this.mRequestingPaymentDialog.IsShowing)
            //{
            //    this.mRequestingPaymentDialog.Dismiss();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
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
                    string apiKeyID = Constants.APP_CONFIG.API_KEY_ID;
                    string custName = selectedPaymentItemList.Count > 1 ? UserEntity.GetActive().DisplayName : selectedPaymentItemList[0].AccountOwnerName;
                    string accNum = selectedAccount.AccountNum;
                    double payableAmt = Double.Parse(txtTotalAmount.Text);
                    string payAm = txtTotalAmount.Text;
                    string custEmail = UserEntity.GetActive().Email;
                    string custPhone = string.IsNullOrEmpty(UserEntity.GetActive().MobileNo) ? "" : UserEntity.GetActive().MobileNo;
                    string sspUserID = UserEntity.GetActive().UserID;//"20225235-290c-484a-a633-607cb51b15e6";
                    string platform = "1"; // 1 Android
                    string paymentMode = selectedPaymentMethod;
                    /* Get user registered cards */
                    string registeredCardId = selectedCard == null ? "" : selectedCard.Id;
                    DeletePaymentHistory();
                    //this.userActionsListener.RequestPayment(apiKeyID, custName, custEmail, custPhone, sspUserID, platform, registeredCardId, paymentMode, total, selectedPaymentItems);
                    this.userActionsListener.InitializePaymentTransaction(custName, custPhone, platform, registeredCardId, paymentMode, total, selectedPaymentItemList);
                }
                else
                {
                    //txtTotalAmount.Error = "Please Enter Valid Payable Amount";
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                //base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == ADD_CARD_REQUEST_CDOE)
                {
                    if (resultCode == Result.Ok)
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

                string action = paymentResponse.requestPayBill.initiatePaymentResult.action;
                string merchantId = paymentResponse.requestPayBill.initiatePaymentResult.payMerchantID;
                string merchantTransId = paymentResponse.requestPayBill.initiatePaymentResult.payMerchant_transID;
                string currencyCode = paymentResponse.requestPayBill.initiatePaymentResult.payCurrencyCode;
                string payAm = paymentResponse.requestPayBill.initiatePaymentResult.payAmount;

                string custEmail = paymentResponse.requestPayBill.initiatePaymentResult.payCustEmail;
                string custName = paymentResponse.requestPayBill.initiatePaymentResult.payCustName;
                string des = paymentResponse.requestPayBill.initiatePaymentResult.payProdDesc;
                string returnURL = paymentResponse.requestPayBill.initiatePaymentResult.payReturnUrl;
                string signature = paymentResponse.requestPayBill.initiatePaymentResult.paySign;
                string mparam1 = paymentResponse.requestPayBill.initiatePaymentResult.payMParam;
                string payMethod = paymentResponse.requestPayBill.initiatePaymentResult.payMethod;
                string platform = paymentResponse.requestPayBill.initiatePaymentResult.platform;
                string accNum = selectedAccount.AccountNum;

                string transType = paymentResponse.requestPayBill.initiatePaymentResult.transactionType;
                string tokenizedHashCodeCC = paymentResponse.requestPayBill.initiatePaymentResult.tokenizedHashCodeCC;
                string custPhone = paymentResponse.requestPayBill.initiatePaymentResult.payCustPhoneNum;

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


            ((PaymentActivity)Activity).nextFragment(this, bundle);
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
            .SetAction("Close", delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }

        public void InitiateWebView(string response)
        {
            Bundle bundle = new Bundle();
            bundle.PutString("html", response);
            ((PaymentActivity)Activity).nextFragment(this, bundle);
        }

        public void InitiateFPXPayment(PaymentTransactionIdResponse response)
        {
            try
            {
                var uri = Android.Net.Uri.Parse(Constants.SERVER_URL.FPX_PAYMENT + response.requestPayBill.initiatePaymentResult.payMerchant_transID + "&" + PARAM3 + param3);

                Bundle bundle = new Bundle();
                bundle.PutString("html_fpx", uri.ToString());
                bundle.PutString("SummaryDashBoardRequest", JsonConvert.SerializeObject(summaryDashBoardRequest));
                ((PaymentActivity)Activity).nextFragment(this, bundle);
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
                    double payableAmt = Double.Parse(txtTotalAmount.Text);
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
                        ShowErrorMessage("For payments more than RM 5000, please use FPX payment mode.");
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
            //if (this.mGetRegisteredCardsDialog != null && !this.mGetRegisteredCardsDialog.IsShowing)
            //{
            //    this.mGetRegisteredCardsDialog.Show();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(Activity, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideGetRegisteredCardDialog()
        {
            //if (this.mGetRegisteredCardsDialog != null && this.mGetRegisteredCardsDialog.IsShowing)
            //{
            //    this.mGetRegisteredCardsDialog.Dismiss();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void GetRegisterCardsResult(MPGetRegisteredCardsResponse response)
        {
            try
            {
                if (response != null)
                {
                    if (response.Data.IsError)
                    {
                        ShowErrorMessage(response.Data.Message);
                    }
                    else
                    {
                        if (response.Data.creditCard != null)
                        {
                            if (response.Data.creditCard.Count() > 0)
                            {
                                List<MPCreditCard> cards = response.Data.creditCard;
                                foreach (MPCreditCard card in cards)
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
                if (response != null)
                {
                    Log.Debug("Initiate Payment Response", "Response Count" + response.ToString());
                    if (response.requestPayBill.ErrorCode != "7200")
                    {
                        ShowErrorMessage(response.requestPayBill.DisplayMessage);
                    }
                    else
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

                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
