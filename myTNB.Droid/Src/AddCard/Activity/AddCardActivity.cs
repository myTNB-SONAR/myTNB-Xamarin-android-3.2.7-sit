﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using Android.Util;
using Android.Content.PM;
using Android.Support.Design.Widget;
using myTNB_Android.Src.Utils;
using Android.Text;
using myTNB_Android.Src.AddCard.MVP;
using myTNB_Android.Src.MakePayment.Model;
using Newtonsoft.Json;
using Card.IO;
using AFollestad.MaterialDialogs;
using Android.Support.V4.Content;
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
        private List<myTNB_Android.Src.MakePayment.Models.CreditCard> registerdCards = new List<myTNB_Android.Src.MakePayment.Models.CreditCard>();

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

                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey("registeredCards"))
                    {
                        //registerdCards = JsonConvert.DeserializeObject<List<myTNB_Android.Src.MakePayment.Models.CreditCard>>(Intent.Extras.GetString("registeredCards"));
                        registerdCards = DeSerialze<List<myTNB_Android.Src.MakePayment.Models.CreditCard>>(extras.GetString("registeredCards"));
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


                txtCardNo.AddTextChangedListener(new InputFilterCreditCard(txtCardNo));
                txtCardExpDate.SetFilters(new Android.Text.IInputFilter[] { new InputFilterExpDate(txtCardExpDate), new InputFilterLengthFilter(5) });
                txtCardNo.AddTextChangedListener(new InputFilterFormField(txtCardNo, textInputLayoutCardNo));
                txtCardExpDate.AddTextChangedListener(new InputFilterFormField(txtCardExpDate, textInputLayoutCardExpDate));
                txtNameOfCard.AddTextChangedListener(new InputFilterFormField(txtNameOfCard, textInputLayoutNameOfCard));
                txtCVV.AddTextChangedListener(new InputFilterFormField(txtCVV, textInputLayoutCVV));

                btnNext = FindViewById<Button>(Resource.Id.btnNext);
                btnNext.Click += delegate
                {
                    //ValidateCardDetails();
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

                /* string cardNo = txtCardNo.Text.Replace(" ", "");
                string name = txtNameOfCard.Text;
                string exp = txtCardExpDate.Text;
                string cvv = txtCVV.Text;*/

                //txtFeedback.TextChanged += TextChanged;

                txtCardNo.TextChanged += CardTextChange;

                txtNameOfCard.TextChanged += NameTextChange;

                txtCardExpDate.TextChanged += ExpTextChange;

                txtCVV.TextChanged += CvvTextChange;
            } catch(Exception e) {
                Utility.LoggingNonFatalError(e);
            }

        }


        [Preserve]
        private void CardTextChange(object sender, Android.Text.TextChangedEventArgs e)
        {
            string cardNo = txtCardNo.Text.Replace(" ", "");

            try
            {
                textInputLayoutCardNo.Error = null;
                if (String.IsNullOrEmpty(cardNo) || cardNo.Length < 15)
                {
                    DisableSaveButton();
                    if (!String.IsNullOrEmpty(cardNo) && cardNo.Length < 15)
                    {
                        textInputLayoutCardNo.Error = "Invalid Card No.";
                    }
                    //ShowErrorMessage("Invalid Card Number", "Please enter valid card number");
                }
                else if (!LuhnVerification(cardNo))
                {
                    DisableSaveButton();
                    textInputLayoutCardNo.Error = "Invalid Card No.";
                    //ShowErrorMessage("Invalid Card Number", "Please enter valid card number");
                }
                else
                {
                    EnableSaveButton();
                }
            } catch(Exception ex) {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void NameTextChange(object sender, Android.Text.TextChangedEventArgs e)
        {
            
            try {
            string name = txtNameOfCard.Text.Trim();

            if (String.IsNullOrEmpty(name))
            {
                DisableSaveButton();
                //ShowErrorMessage("Invalid Name", "Please enter name of card holder");
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
            try {
            string exp = txtCardExpDate.Text.Trim();

            textInputLayoutCardExpDate.Error = null;
            if (String.IsNullOrEmpty(exp) || exp.Length != 5 || !exp.Contains("/"))
            {
                DisableSaveButton();
                //ShowErrorMessage("Invalid Exp Date", "Please enter expiry date of your card");
                if (!String.IsNullOrEmpty(exp)) {
                    textInputLayoutCardExpDate.Error = "Invalid Card Expiration Date";
                }
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
        private void CvvTextChange(object sender, Android.Text.TextChangedEventArgs e)
        {
            try {
            string cvv = txtCVV.Text.Trim();
            textInputLayoutCVV.Error = null;
            if (String.IsNullOrEmpty(cvv) || cvv.Length < 3)
            {
                DisableSaveButton();

                if (!String.IsNullOrEmpty(cvv)) {
                    textInputLayoutCVV.Error = "Invalid CVV.";
                //ShowErrorMessage("Invalid CVV Code", "Please enter CVV code from the back of your card");    
                }

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

            try {
            if (String.IsNullOrEmpty(cardNo) || cardNo.Length < 15)
            {
                DisableSaveButton();
                //ShowErrorMessage("Invalid Card Number", "Please enter valid card number");
            }
            else if(!LuhnVerification(cardNo))
            {
                DisableSaveButton();
                //ShowErrorMessage("Invalid Card Number", "Please enter valid card number");
            }
            else if (String.IsNullOrEmpty(name))
            {
                DisableSaveButton();
                //ShowErrorMessage("Invalid Name", "Please enter name of card holder");
            }
            else if (String.IsNullOrEmpty(exp) || exp.Length != 5 || !exp.Contains("/"))
            {
                DisableSaveButton();
                //ShowErrorMessage("Invalid Exp Date", "Please enter expiry date of your card");
            }
            else if (String.IsNullOrEmpty(cvv) || cvv.Length < 3)
            {
                DisableSaveButton();
                //ShowErrorMessage("Invalid CVV Code", "Please enter CVV code from the back of your card");
            }
            else if (IsAlreadyRegisteredCard(cardNo))
            {
                DisableSaveButton();
                mDuplicateCardDialog = new MaterialDialog.Builder(this)
                        .Title("Info")
                        .Content("Seems like you are paying with an already saved Credit / Debit Card. Do you want to continue?")
                        .Cancelable(false)
                        .PositiveText("Continue")
                        .OnPositive((dialog, which) => {
                            CardDetails card = new CardDetails(cardNo, name, exp, cvv, saveCard.Checked);
                            Intent finishIntent = new Intent();
                            finishIntent.PutExtra("extra", JsonConvert.SerializeObject(card));
                            SetResult(Result.Ok, finishIntent);
                            Finish();
                        })
                        .NeutralText("Cancel")
                        .OnNeutral((dialog, which) => mDuplicateCardDialog.Dismiss()).Show();
            }
            else{
                btnNext.Enabled = true;
                btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
            }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }


        private void EnableSaveButton() {
            ValidateCardDetails();
        }


        private void DisableSaveButton() {
            btnNext.Enabled = false;
            btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        private void DoSaveCard() {
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
            } catch (Exception e) {
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
            .SetAction("Close", delegate { mErrorMessageSnackBar.Dismiss(); }
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
            intent.PutExtra(CardIOActivity.ExtraScanResult, true);
            StartActivityForResult(intent, REQUEST_SCAN);
        }

        protected override void OnActivityResult(int requestCode, Result result, Intent data)
        {
            try {
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
            else
            {
                Toast.MakeText(this, "Unable to scan card! Please try again...", ToastLength.Long).Show();
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
            foreach (myTNB_Android.Src.MakePayment.Models.CreditCard card in registerdCards)
            {
                string first = card.LastDigits.Substring(0, 6);
                string last = card.LastDigits.Substring(card.LastDigits.Length - 4);
                if(first.Equals(first6Digits) && last.Equals(last4Digits))
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