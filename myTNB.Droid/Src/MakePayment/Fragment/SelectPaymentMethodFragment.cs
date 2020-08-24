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
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MakePayment.Activity;
using myTNB_Android.Src.MakePayment.Adapter;
using myTNB_Android.Src.MakePayment.Model;
using myTNB_Android.Src.MakePayment.Models;
using myTNB_Android.Src.MakePayment.MVP;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB_Android.Src.MakePayment.Fragment
{
    public class SelectPaymentMethodFragment : AndroidX.Fragment.App.Fragment , SelectPaymentMethodContract.IView
    {

        private string TOOL_BAR_TITLE = "Select Payment Method";
        private SelectPaymentMethodPresenter mPresenter;
        private SelectPaymentMethodContract.IUserActionsListener userActionsListener;

        private static int ADD_CARD_REQUEST_CDOE = 1001;
        private static CardDetails cardDetails;

        private static string METHOD_CREDIT_CARD = "CC";
        private static string METHOD_FPX = "FPX";
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
        ListView listAddedCards;
        AddCardAdapter cardAdapter;
        List<CardDetails> cards = new List<CardDetails>();
        List<CreditCard> registerdCards = new List<CreditCard>();

        Button btnAddCard;
        Button btnFPXPayment;

        private MaterialDialog mRequestingPaymentDialog;
        private MaterialDialog mGetRegisteredCardsDialog;
        private Snackbar mErrorMessageSnackBar;

        DecimalFormat decimalFormat = new DecimalFormat("#,###,###,###,##0.00");

        public bool IsActive()
        {
            return IsVisible;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View rootView = inflater.Inflate(Resource.Layout.SelectPaymentMethodView, container, false);
            try
            {
                mPresenter = new SelectPaymentMethodPresenter(this);

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

                ((MakePaymentActivity)Activity).SetToolBarTitle(TOOL_BAR_TITLE);
                selectedAccount = JsonConvert.DeserializeObject<AccountData>(Arguments.GetString(Constants.SELECTED_ACCOUNT));

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
                    AddNewCard();
                };

                btnFPXPayment = rootView.FindViewById<Button>(Resource.Id.btnFPXPayment);
                btnFPXPayment.Click += delegate
                {
                    selectedPaymentMethod = METHOD_FPX;
                    selectedCard = null;
                    InitiatePaymentRequest();

                };

                listAddedCards = rootView.FindViewById<ListView>(Resource.Id.listAddedCards);
                cardAdapter = new AddCardAdapter(Activity, registerdCards);
                listAddedCards.Adapter = cardAdapter;
                cardAdapter.OnItemClick += OnItemClick;


                TextViewUtils.SetMuseoSans500Typeface(lblCreditDebitCard, lblOtherPaymentMethods, txtTotalAmount, lblTotalAmount);
                TextViewUtils.SetMuseoSans300Typeface(btnAddCard, btnFPXPayment);

                TextViewUtils.SetMuseoSans300Typeface(lblCvvInfo);
                TextViewUtils.SetMuseoSans300Typeface(edtNumber1, edtNumber2, edtNumber3, edtNumber4);

                if (selectedAccount != null)
                {

                    txtTotalAmount.Text = decimalFormat.Format(selectedAccount.AmtCustBal).Replace(",", "");
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
            ((MakePaymentActivity)Activity).SetToolBarTitle(TOOL_BAR_TITLE);
            base.OnResume();
        }

        void OnItemClick(object sender, int position)
        {
            try
            {
                selectedPaymentMethod = METHOD_CREDIT_CARD;
                if (IsValidPayableAmount())
                {
                    selectedCard = cardAdapter.GetCardDetailsAt(position);
                    //cardDetails = null;
                    //InitiatePaymentRequest();
                    EnterCVVNumber(selectedCard);
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
                    Intent nextIntent = new Intent();
                    nextIntent.PutExtra("registeredCards", JsonConvert.SerializeObject(registerdCards));
                    nextIntent.SetClass(Activity, typeof(AddCardActivity));
                    StartActivityForResult(nextIntent, ADD_CARD_REQUEST_CDOE);
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

        public void SetPresenter(SelectPaymentMethodContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            throw new NotImplementedException();
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
                    string apiKeyID = Constants.APP_CONFIG.API_KEY_ID;
                    string custName = UserEntity.GetActive().DisplayName;
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
                    this.userActionsListener.RequestPayment(apiKeyID, custName, accNum, payAm, custEmail, custPhone, sspUserID, platform, paymentMode, registeredCardId);
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
                            cardDetails = JsonConvert.DeserializeObject<CardDetails>(data.GetStringExtra("extra"));
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

        public void InitiateSubmitPayment(InitiatePaymentResponse paymentResponse, CardDetails card)
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


            ((MakePaymentActivity)Activity).nextFragment(this, bundle);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            //this.userActionsListener.SubmitPayment(apiKeyID, merchantId, accNum, payAm, custName, custEmail, custPhone, mparam1, des, cardNo, custName, cardExpM, cardExpY, cardCvv);
        }

        public void SaveInitiatePaymentResponse(InitiatePaymentResponse response)
        {
            try
            {
                if (response != null)
                {
                    Log.Debug("Initiate Payment Response", "Response Count" + response.ToString());
                    if (response.requestPayBill.IsError)
                    {
                        ShowErrorMessage(response.requestPayBill.Message);
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

        public void InitiateWebView(string response)
        {
            Bundle bundle = new Bundle();
            bundle.PutString("html", response);
            ((MakePaymentActivity)Activity).nextFragment(this, bundle);
        }

        public void InitiateFPXPayment(InitiatePaymentResponse response)
        {
            var uri = Android.Net.Uri.Parse(Constants.SERVER_URL.FPX_PAYMENT + response.requestPayBill.initiatePaymentResult.payMerchant_transID);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
            ((MakePaymentActivity)Activity).SetResult(Result.Ok);
            ((MakePaymentActivity)Activity).Finish();
            DashboardHomeActivity activity = DashboardHomeActivity.dashboardHomeActivity;
            activity.OnFinish();

        }

        public bool IsValidPayableAmount()
        {
            bool isValid = true;
            try
            {


                if (String.IsNullOrEmpty(txtTotalAmount.Text))
                {
                    isValid = false;
                    txtTotalAmount.Error = "Please Enter Valid Payable Amount";
                }
                else
                {
                    double payableAmt = Double.Parse(txtTotalAmount.Text);
                    if (payableAmt < 1)
                    {
                        isValid = false;
                        txtTotalAmount.Error = "Minimum amount for online payment is 1RM";
                    }
                    else if (payableAmt > 5000 && selectedPaymentMethod.Equals(METHOD_CREDIT_CARD))
                    {
                        isValid = false;
                        txtTotalAmount.Error = "For payments more than RM 5000, please use FPX payment mode.";
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
                if (this.mGetRegisteredCardsDialog != null && !this.mGetRegisteredCardsDialog.IsShowing)
                {
                    this.mGetRegisteredCardsDialog.Show();
                }
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
                if (this.mGetRegisteredCardsDialog != null && this.mGetRegisteredCardsDialog.IsShowing)
                {
                    this.mGetRegisteredCardsDialog.Dismiss();
                }
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
                        ShowErrorMessage(response.Response.DisplayMessage);
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
            try
            {
                CheckValidPin();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
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
    }
}
