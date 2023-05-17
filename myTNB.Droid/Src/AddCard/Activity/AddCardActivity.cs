using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Card.IO;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB_Android.Src.AddCard.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;

namespace myTNB_Android.Src.AddCard.Activity
{
    [Activity(Label = "Add Card"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.AddCard")]
    public class AddCardActivity : BaseToolbarAppCompatActivity, AddCardContract.IView
    {

        private AddCardPresenter mPresenter;
        private AddCardContract.IUserActionsListener userActionsListener;

        private static readonly int REQUEST_SCAN = 5020;
        private List<MultipleAccountPayment.Models.CreditCard> registerdCards = new List<MultipleAccountPayment.Models.CreditCard>();

        private MaterialDialog mDuplicateCardDialog;
        Snackbar mErrorMessageSnackBar;

        FrameLayout rootView;
        TextView txtTitle;
        TextInputLayout textInputLayoutCardNo;
        TextInputLayout textInputLayoutNameOfCard;
        TextInputLayout textInputLayoutCardExpDate;
        TextInputLayout textInputLayoutCVV;

        EditText txtCardNo;
        EditText txtNameOfCard;
        EditText txtCardExpDate;
        EditText txtCVV;

        CheckBox saveCard;

        Button btnNext;
        ImageButton btnScan;

        private static char space = ' ';

        public override int ResourceId()
        {
            return Resource.Layout.AddCardView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                this.mPresenter = new AddCardPresenter(this);

                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey("registeredCards"))
                    {
                        registerdCards = DeSerialze<List<MultipleAccountPayment.Models.CreditCard>>(extras.GetString("registeredCards"));
                    }
                }
                rootView = FindViewById<FrameLayout>(Resource.Id.rootView);
                txtTitle = FindViewById<TextView>(Resource.Id.txtInfoTitle);
                textInputLayoutCardNo = FindViewById<TextInputLayout>(Resource.Id.textInputLayoutCardNo);
                textInputLayoutNameOfCard = FindViewById<TextInputLayout>(Resource.Id.textInputLayoutNameOnCard);
                textInputLayoutCardExpDate = FindViewById<TextInputLayout>(Resource.Id.textInputLayoutCardExpDate);
                textInputLayoutCVV = FindViewById<TextInputLayout>(Resource.Id.textInputLayoutCardCVV);

                txtCardNo = FindViewById<EditText>(Resource.Id.txtCardNo);
                txtNameOfCard = FindViewById<EditText>(Resource.Id.txtNameOnCard);
                txtCardExpDate = FindViewById<EditText>(Resource.Id.txtCardExpDate);
                txtCVV = FindViewById<EditText>(Resource.Id.txtCardCVV);

                TextViewUtils.SetMuseoSans300Typeface(txtTitle);
                TextViewUtils.SetMuseoSans300Typeface(txtCardNo, txtNameOfCard, txtCardExpDate, txtCVV);
                TextViewUtils.SetMuseoSans300Typeface(textInputLayoutCardNo, textInputLayoutNameOfCard, textInputLayoutCardExpDate, textInputLayoutCVV);

                SetToolBarTitle(Utility.GetLocalizedLabel("AddCard", "title"));
                txtTitle.Text = Utility.GetLocalizedLabel("AddCard", "acceptedCardsMessage");
                textInputLayoutCardNo.Hint = Utility.GetLocalizedLabel("AddCard", "cardNumber");
                textInputLayoutNameOfCard.Hint = Utility.GetLocalizedLabel("AddCard", "nameOnCard");
                textInputLayoutCardExpDate.Hint = Utility.GetLocalizedLabel("AddCard", "hintCardExpiry");
                textInputLayoutCVV.Hint = Utility.GetLocalizedLabel("AddCard", "cvv");

                txtCardNo.AddTextChangedListener(new InputFilterCreditCard(txtCardNo));
                txtCardExpDate.SetFilters(new Android.Text.IInputFilter[] { new InputFilterExpDate(txtCardExpDate), new InputFilterLengthFilter(5) });
                txtCardNo.AddTextChangedListener(new InputFilterFormField(txtCardNo, textInputLayoutCardNo));
                txtCardExpDate.AddTextChangedListener(new InputFilterFormField(txtCardExpDate, textInputLayoutCardExpDate));
                txtNameOfCard.AddTextChangedListener(new InputFilterFormField(txtNameOfCard, textInputLayoutNameOfCard));
                txtCVV.AddTextChangedListener(new InputFilterFormField(txtCVV, textInputLayoutCVV));

                btnNext = FindViewById<Button>(Resource.Id.btnNext);
                btnNext.Click += delegate
                {
                    DoSaveCard();
                };

                btnScan = FindViewById<ImageButton>(Resource.Id.scan);
                btnScan.Click += delegate
                {
                    OnScanCard();
                };

                saveCard = FindViewById<CheckBox>(Resource.Id.chk_save_card);

                TextViewUtils.SetMuseoSans300Typeface(saveCard);
                TextViewUtils.SetMuseoSans500Typeface(btnNext);

                saveCard.Text = Utility.GetLocalizedLabel("AddCard", "saveCardMessage");
                btnNext.Text = Utility.GetLocalizedLabel("Common", "next");
                txtCardNo.TextChanged += CardTextChange;

                txtNameOfCard.TextChanged += NameTextChange;

                txtCardExpDate.TextChanged += ExpTextChange;

                txtCVV.TextChanged += CvvTextChange;

                TextViewUtils.SetTextSize16(txtTitle, saveCard, btnNext);

                SetTheme(TextViewUtils.IsLargeFonts
                    ? Resource.Style.Theme_AddCardLarge
                    : Resource.Style.Theme_AddCard);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Add Credit Card");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        [Preserve]
        private void CardTextChange(object sender, Android.Text.TextChangedEventArgs e)
        {
            string cardNo = txtCardNo.Text.Replace(" ", "");

            try
            {
                if (cardNo.Length == 0)
                {
                    textInputLayoutCardNo.Error = null;
                }


                if (String.IsNullOrEmpty(cardNo) || cardNo.Length < 15)
                {
                    DisableSaveButton();
                    if (!String.IsNullOrEmpty(cardNo) && cardNo.Length < 15)
                    {

                        if (textInputLayoutCardNo.Error != Utility.GetLocalizedLabel("AddCard", "invalidCardNumber"))
                        {
                            textInputLayoutCardNo.Error = Utility.GetLocalizedLabel("AddCard", "invalidCardNumber");
                        }
                    }
                }
                else if (!LuhnVerification(cardNo))
                {
                    DisableSaveButton();
                    if (textInputLayoutCardNo.Error != Utility.GetLocalizedLabel("AddCard", "invalidCardNumber"))
                    {
                        textInputLayoutCardNo.Error = Utility.GetLocalizedLabel("AddCard", "invalidCardNumber");
                    }

                }
                else
                {
                    EnableSaveButton();
                    textInputLayoutCardNo.Error = null;
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void NameTextChange(object sender, Android.Text.TextChangedEventArgs e)
        {

            try
            {
                string name = txtNameOfCard.Text.Trim();

                if (String.IsNullOrEmpty(name))
                {
                    DisableSaveButton();
                }
                else
                {
                    EnableSaveButton();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void ExpTextChange(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                string exp = txtCardExpDate.Text.Trim();


                if (exp.Length == 0)
                {
                    textInputLayoutCardExpDate.Error = null;
                }


                if (String.IsNullOrEmpty(exp) || exp.Length != 5 || !exp.Contains("/"))
                {
                    DisableSaveButton();
                    if (!String.IsNullOrEmpty(exp))
                    {

                        if (textInputLayoutCardExpDate.Error != Utility.GetLocalizedLabel("AddCard", "invalidCardExpiry"))
                        {
                            textInputLayoutCardExpDate.Error = Utility.GetLocalizedLabel("AddCard", "invalidCardExpiry");
                        }

                    }
                }
                else
                {
                    textInputLayoutCardExpDate.Error = null;
                    EnableSaveButton();

                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void CvvTextChange(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                string cvv = txtCVV.Text.Trim();

                if (cvv.Length == 0)
                {
                    textInputLayoutCVV.Error = null;
                }


                if (String.IsNullOrEmpty(cvv) || cvv.Length < 3)
                {
                    DisableSaveButton();

                    if (!String.IsNullOrEmpty(cvv))
                    {
                        if (textInputLayoutCVV.Error != Utility.GetLocalizedLabel("AddCard", "invalidCVV"))
                        {
                            textInputLayoutCVV.Error = Utility.GetLocalizedLabel("AddCard", "invalidCVV");
                        }

                    }

                }
                else
                {
                    textInputLayoutCVV.Error = null;
                    EnableSaveButton();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }


        public override void OnBackPressed()
        {
            Intent finishIntent = new Intent();
            SetResult(Result.Canceled, finishIntent);
            Finish();

        }

        public void Start()
        {

        }

        public void ValidateCardDetails()
        {
            string cardNo = txtCardNo.Text.Replace(" ", "");
            string name = txtNameOfCard.Text;
            string exp = txtCardExpDate.Text;
            string cvv = txtCVV.Text;

            try
            {
                if (String.IsNullOrEmpty(cardNo) || cardNo.Length < 15)
                {
                    DisableSaveButton();
                }
                else if (!LuhnVerification(cardNo))
                {
                    DisableSaveButton();
                }
                else if (String.IsNullOrEmpty(name))
                {
                    DisableSaveButton();
                }
                else if (String.IsNullOrEmpty(exp) || exp.Length != 5 || !exp.Contains("/"))
                {
                    DisableSaveButton();
                }
                else if (String.IsNullOrEmpty(cvv) || cvv.Length < 3)
                {
                    DisableSaveButton();
                }
                else if (IsAlreadyRegisteredCard(cardNo))
                {
                    DisableSaveButton();
                    mDuplicateCardDialog = new MaterialDialog.Builder(this)
                            .Title("Info")
                            .Content(Utility.GetLocalizedLabel("AddCard", "savedCardMessage"))
                            .Cancelable(false)
                            .PositiveText("Continue")
                            .OnPositive((dialog, which) =>
                            {
                                CardDetails card = new CardDetails(cardNo, name, exp, cvv, saveCard.Checked);
                                Intent finishIntent = new Intent();
                                finishIntent.PutExtra("extra", JsonConvert.SerializeObject(card));
                                SetResult(Result.Ok, finishIntent);
                                Finish();
                            })
                            .NeutralText(Utility.GetLocalizedCommonLabel("cancel"))
                            .OnNeutral((dialog, which) => mDuplicateCardDialog.Dismiss()).Show();
                }
                else
                {
                    btnNext.Enabled = true;
                    btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }


        private void EnableSaveButton()
        {
            ValidateCardDetails();
        }


        private void DisableSaveButton()
        {
            btnNext.Enabled = false;
            btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        private void DoSaveCard()
        {
            try
            {
                string cardNo = txtCardNo.Text.Replace(" ", "");
                string name = txtNameOfCard.Text;
                string exp = txtCardExpDate.Text;
                string cvv = txtCVV.Text;

                CardDetails card = new CardDetails(cardNo, name, exp, cvv, saveCard.Checked);
                Intent finishIntent = new Intent();
                finishIntent.PutExtra("extra", JsonConvert.SerializeObject(card));
                SetResult(Result.Ok, finishIntent);
                Finish();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void AddCreditCard()
        {

        }

        public void ShowErrorMessage(string title, string message)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }

        public void SetPresenter(AddCardContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public void OnScanCard()
        {
            Intent intent = new Intent(this, typeof(CardIOActivity));
            intent.PutExtra(CardIOActivity.ExtraRequireExpiry, true);
            intent.PutExtra(CardIOActivity.ExtraScanExpiry, true);
            intent.PutExtra(CardIOActivity.ExtraRequireCvv, false);
            intent.PutExtra(CardIOActivity.ExtraRequirePostalCode, true);
            intent.PutExtra(CardIOActivity.ExtraUsePaypalActionbarIcon, false);
            intent.PutExtra(CardIOActivity.ExtraUseCardioLogo, false);
            intent.PutExtra(CardIOActivity.ExtraSuppressManualEntry, false);
            intent.PutExtra(CardIOActivity.ExtraSuppressConfirmation, true);
            intent.PutExtra(CardIOActivity.ExtraKeepApplicationTheme, true);
            intent.PutExtra(CardIOActivity.ExtraCapturedCardImage, false);
            string txt = Utility.GetLocalizedLabel("CardScanner", "scanInstructions");
            intent.PutExtra(CardIOActivity.ExtraScanInstructions, txt);
            intent.PutExtra(CardIOActivity.ExtraScanResult, true);
            StartActivityForResult(intent, REQUEST_SCAN);

            try
            {
                FirebaseAnalyticsUtils.LogClickEvent(this, "Credit Card Scan");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnActivityResult(int requestCode, Result result, Intent data)
        {
            try
            {
                if (requestCode == REQUEST_SCAN && data != null
                    && data.HasExtra(CardIOActivity.ExtraScanResult))
                {
                    var card = data.GetParcelableExtra(CardIOActivity.ExtraScanResult)
                         .JavaCast<CreditCard>();
                    if (card != null)
                    {
                        txtCardNo.Text = card.CardNumber;
                        if (card.ExpiryMonth != 0 && card.ExpiryYear != 0)
                        {
                            string cardExpDate = card.ExpiryMonth + "" + card.ExpiryYear;
                            txtCardExpDate.Text = cardExpDate;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private bool IsAlreadyRegisteredCard(string cardNumber)
        {
            bool flag = false;
            string first6Digits = cardNumber.Substring(0, 6);
            string last4Digits = cardNumber.Substring(cardNumber.Length - 4);
            foreach (MultipleAccountPayment.Models.CreditCard card in registerdCards)
            {
                string first = card.LastDigits.Substring(0, 6);
                string last = card.LastDigits.Substring(card.LastDigits.Length - 4);
                if (first.Equals(first6Digits) && last.Equals(last4Digits))
                {
                    flag = true;
                    break;
                }

            }

            return flag;
        }

        public bool LuhnVerification(string creditCardNumber)
        {
            bool isValid = false;
            if (string.IsNullOrEmpty(creditCardNumber))
            {
                return isValid;
            }
            int sumOfDigits = creditCardNumber.Where((e) => e >= '0' && e <= '9')
                            .Reverse()
                            .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                            .Sum((e) => e / 10 + e % 10);
            isValid = sumOfDigits % 10 == 0;
            Console.WriteLine("isValid: " + isValid);
            return isValid;
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

    }
}
