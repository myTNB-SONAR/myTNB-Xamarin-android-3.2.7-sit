﻿using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using iTextSharp.text;
using iTextSharp.text.pdf;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.ViewReceipt.MVP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using static myTNB.AndroidApp.Src.MyTNBService.Response.AccountReceiptResponse;

namespace myTNB.AndroidApp.Src.ViewReceipt.Activity
{
    [Activity(Label = "Payment Receipt"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Dashboard")]
    public class ViewReceiptMultiAccountNewDesignActivty : BaseActivityCustom, ViewReceiptMultiAccountNewDesignContract.IView
    {
        private string TAG = "View Receipt Activity";
        private string RECEPT_NO;

        [BindView(Resource.Id.progressBar)]
        ProgressBar mProgressBar;

        [BindView(Resource.Id.scrollView)]
        ScrollView baseView;

        [BindView(Resource.Id.receipt_titile)]
        TextView receiptTitle;

        [BindView(Resource.Id.dear_customer)]
        TextView dearCustomer;

        [BindView(Resource.Id.thanks_text)]
        TextView thanksText;

        [BindView(Resource.Id.pleased_text)]
        TextView pleasedText;

        [BindView(Resource.Id.reference_number_text)]
        TextView referenceNumberText;

        [BindView(Resource.Id.reference_number_value)]
        TextView referenceNumberValue;

        [BindView(Resource.Id.account_layout)]
        LinearLayout accountLayout;

        [BindView(Resource.Id.txn_date_text)]
        TextView txnDateText;

        [BindView(Resource.Id.txn_date_value)]
        TextView txnDateValue;

        [BindView(Resource.Id.txn_id_text)]
        TextView txnIdText;

        [BindView(Resource.Id.txn_id_value)]
        TextView txnIdValue;

        [BindView(Resource.Id.txn_method_text)]
        TextView txnMethodText;

        [BindView(Resource.Id.txn_method_value)]
        TextView txnMethodValue;

        [BindView(Resource.Id.total_amt_text)]
        TextView totalAmtText;

        [BindView(Resource.Id.total_amt_value)]
        TextView totalAmtValue;

        [BindView(Resource.Id.note_text)]
        TextView noteText;

        [BindView(Resource.Id.linePaymentType)]
        View linePaymentType;

        [BindView(Resource.Id.txn_paymentType_text)]
        TextView paymentTypeText;

        [BindView(Resource.Id.txn_paymentType_value)]
        TextView paymentTypeValue;

        [BindView(Resource.Id.line_bankRef_value)]
        View line_bankRef_value;

        [BindView(Resource.Id.txn_bankRef_text)]
        TextView txnBankRefText;

        [BindView(Resource.Id.txn_bankRef_value)]
        TextView txnBankRefValue;

        [BindView(Resource.Id.line_AcqrBank_value)]
        View line_AcqrBank_value;

        [BindView(Resource.Id.txn_AcqrBank_text)]
        TextView txnAcqrBankText;

        [BindView(Resource.Id.txn_AcqrBank_value)]
        TextView txnAcqrBankValue;

        ViewReceiptMultiAccountNewDesignPresenter mPresenter = null;
        ViewReceiptMultiAccountNewDesignContract.IUserActionsListener iPresenter = null;

        private AlertDialog mGetReceiptDialog;
        private Snackbar mErrorMessageSnackBar;

        private GetPaymentReceiptResponse response = null;
        private string selectedAccountNumber, detailedInfoNumber;
        private bool isOwnedAccount, showAllReceipt, isApplicationReceipt;
        private readonly string PAGE_ID = "Receipt";

        public override int ResourceId()
        {
            return Resource.Layout.ViewBillReceiptNewDesign;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            baseView.Visibility = ViewStates.Gone;
            try
            {
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);

                mPresenter = new ViewReceiptMultiAccountNewDesignPresenter(this);

                mProgressBar.Visibility = ViewStates.Gone;

                TextViewUtils.SetMuseoSans500Typeface(noteText, totalAmtValue, totalAmtText
                    , txnMethodValue, txnMethodText, txnIdValue, txnIdText, txnDateValue
                    , txnDateText, referenceNumberValue, referenceNumberText, pleasedText
                    , thanksText, dearCustomer, receiptTitle, paymentTypeText, paymentTypeValue
                    , txnBankRefText, txnAcqrBankText);

                mGetReceiptDialog = new AlertDialog.Builder(this)
                  .SetTitle("Get Receipt")
                  .SetMessage("Please wait while we are getting receipt details...")
                  .SetCancelable(false)
                  .Create();

                receiptTitle.Text = GetLabelByLanguage("title");
                dearCustomer.Text = GetLabelByLanguage("salutation");
                thanksText.Text = GetLabelByLanguage("messagePartOne");

                referenceNumberText.Text = GetLabelByLanguage("referenceNumber").ToUpper();
                txnDateText.Text = GetLabelByLanguage("trnDate").ToUpper();
                txnIdText.Text = GetLabelByLanguage("trnID").ToUpper();
                txnMethodText.Text = GetLabelByLanguage("trnMethod").ToUpper();
                totalAmtText.Text = GetLabelCommonByLanguage("totalAmountRM").ToUpper();
                noteText.Text = GetLabelByLanguage("note");
                txnBankRefText.Text = GetLabelByLanguage("trnBankReference").ToUpper();
                txnAcqrBankText.Text = GetLabelByLanguage("trnAcquirerBank").ToUpper();

                TextViewUtils.SetTextSize10(referenceNumberText, txnDateText
                    , txnIdText, txnMethodText, noteText, paymentTypeText, txnBankRefText
                    , txnAcqrBankText);
                TextViewUtils.SetTextSize14(dearCustomer, thanksText, totalAmtText);
                TextViewUtils.SetTextSize20(receiptTitle);

                Android.OS.Bundle extras = Intent.Extras;

                if (extras != null && extras.ContainsKey("ReceiptResponse"))
                {
                    GetPaymentReceiptResponse result = JsonConvert.DeserializeObject<GetPaymentReceiptResponse>(extras.GetString("ReceiptResponse"));
                    OnShowReceiptDetails(result);
                }
                else
                {
                    if (extras.ContainsKey("SELECTED_ACCOUNT_NUMBER"))
                    {
                        selectedAccountNumber = extras.GetString("SELECTED_ACCOUNT_NUMBER");
                    }
                    if (extras.ContainsKey("DETAILED_INFO_NUMBER"))
                    {
                        detailedInfoNumber = extras.GetString("DETAILED_INFO_NUMBER");
                    }
                    if (extras.ContainsKey("IS_OWNED_ACCOUNT"))
                    {
                        isOwnedAccount = extras.GetBoolean("IS_OWNED_ACCOUNT");
                    }
                    if (extras.ContainsKey("IS_SHOW_ALL_RECEIPT"))
                    {
                        showAllReceipt = extras.GetBoolean("IS_SHOW_ALL_RECEIPT");
                    }
                    if (extras.ContainsKey("IsApplicationReceipt"))
                    {
                        isApplicationReceipt = extras.GetBoolean("IsApplicationReceipt");
                    }
                    if (ConnectionUtils.HasInternetConnection(this))
                    {
                        this.iPresenter.GetReceiptDetails(selectedAccountNumber, detailedInfoNumber, isOwnedAccount, showAllReceipt);
                    }
                    else
                    {
                        ShowErrorMessage(Utility.GetLocalizedErrorLabel("noDataConnectionMessage"));
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ViewBillStatementMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

         public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_share:
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)                                          //Starting android 13 asking notification permission
                    {
                        CreatePDF(response);
                    }
                    else
                    {
                        if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
                        {
                            RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);
                        }
                        else
                        {
                            CreatePDF(response);
                        }
                    }
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void WriteGrayContent(PdfPTable tableLayout)
        {
            try
            {
                tableLayout.WidthPercentage = 100;

                PdfPCell cell = new PdfPCell(new Phrase(null, null))
                {
                    PaddingLeft = 0,
                    PaddingTop = 0,
                    PaddingRight = 0,
                    PaddingBottom = 0,
                    BackgroundColor = new iTextSharp.text.Color(228.0f / 255.0f, 228.0f / 255.0f, 228.0f / 255.0f, 1.0f),
                    Border = 0,
                    FixedHeight = 2,
                };
                tableLayout.AddCell(cell);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void CreatePDF(GetPaymentReceiptResponse response)
        {
            mProgressBar.Visibility = ViewStates.Gone;
            ShowGetReceiptDialog();
            Task.Run(() =>
            {
                Log.Debug(TAG, "Receipt :" + response.GetData());
                RECEPT_NO = response.GetData().referenceNum;
                try
                {
                    string rootPath = this.FilesDir.AbsolutePath;

                    if (FileUtils.IsExternalStorageReadable() && FileUtils.IsExternalStorageWritable())
                    {
                        rootPath = this.GetExternalFilesDir(null).AbsolutePath;
                    }

                    var directory = System.IO.Path.Combine(rootPath, "pdf");
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var path = System.IO.Path.Combine(directory, RECEPT_NO + ".pdf");

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    var fs = new FileStream(path, FileMode.Create);


                    Document document = new Document(PageSize.A4, 25, 25, 30, 30);

                    PdfWriter writer = PdfWriter.GetInstance(document, fs);

                    iTextSharp.text.Color blueColour = new iTextSharp.text.Color(28.0f / 255.0f, 121.0f / 255.0f, 202.0f / 255.0f, 1.0f);
                    var tunaGreyColour = new iTextSharp.text.Color(73.0f / 255.0f, 73.0f / 255.0f, 74.0f / 255.0f, 1.0f);
                    var silverChaliceColour = new iTextSharp.text.Color(0.65f, 0.65f, 0.65f, 1.0f);

                    AssetManager assets = this.Assets;
                    var bytes = default(byte[]);
                    using (StreamReader reader = new StreamReader(assets.Open("fonts/MuseoSans_500.otf")))
                    {
                        using (var memstream = new MemoryStream())
                        {
                            reader.BaseStream.CopyTo(memstream);
                            bytes = memstream.ToArray();
                        }
                    }

                    BaseFont titleBf = BaseFont.CreateFont("MuseoSans_500.otf", BaseFont.IDENTITY_H, true, false, bytes, null);

                    Font titleFont = new Font(titleBf, 30f, 0, blueColour);
                    Font detailsFont = new Font(titleBf, 24f, 0, tunaGreyColour);
                    Font labelFont = new Font(titleBf, 18f, 0, silverChaliceColour);
                    Font totalAmounFont = new Font(titleBf, 48f, 0, tunaGreyColour);

                    Drawable d = ContextCompat.GetDrawable(this, Resource.Drawable.tnb_receipt_logo_header);
                    Bitmap bitmap = ((BitmapDrawable)d).Bitmap;
                    MemoryStream stream = new MemoryStream();
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                    byte[] bitmapdata = stream.ToArray();

                    //string filepath = Android.OS.Environment.;
                    var headerImage = Image.GetInstance(bitmapdata);

                    document.Open();

                    //document.Open();
                    PdfContentByte cb = writer.DirectContent;

                    PdfPTable grayLine = new PdfPTable(1);
                    grayLine.TotalWidth = document.PageSize.Width - 40;
                    WriteGrayContent(grayLine);

                    headerImage.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                    float y = document.PageSize.Height - document.TopMargin - headerImage.Height;

                    if (y < 700)
                    {
                        float diff = 700 - y;
                        y = y + diff;
                    }

                    headerImage.SetAbsolutePosition(0, y);

                    document.Add(headerImage);
                    document.Add(new Paragraph(Environment.NewLine, titleFont));
                    document.Add(new Paragraph(Environment.NewLine, titleFont));
                    document.Add(new Paragraph(Environment.NewLine, titleFont));
                    document.Add(new Paragraph(Environment.NewLine, detailsFont));
                    document.Add(new Paragraph(GetLabelByLanguage("title"), titleFont));
                    document.Add(new Paragraph(Environment.NewLine, titleFont));
                    document.Add(new Paragraph(GetLabelByLanguage("salutation"), detailsFont));
                    document.Add(new Paragraph(Environment.NewLine, detailsFont));
                    document.Add(new Paragraph(GetLabelByLanguage("messagePartOne"), detailsFont));
                    document.Add(new Paragraph(string.Format(GetLabelByLanguage("messagePartTwo")
                        , response.GetData().payMethod), detailsFont));

                    document.Add(new Paragraph(Environment.NewLine, titleFont));
                    document.Add(grayLine);
                    document.Add(new Paragraph(Environment.NewLine, labelFont));

                    document.Add(new Paragraph(GetLabelByLanguage("referenceNumber").ToUpper(), labelFont));
                    document.Add(new Paragraph(response.GetData().referenceNum, detailsFont));

                    document.Add(new Paragraph(Environment.NewLine, titleFont));
                    document.Add(grayLine);
                    document.Add(new Paragraph(Environment.NewLine, labelFont));

                    foreach (var item in response.GetData().accMultiPay)
                    {
                        document.Add(new Paragraph(GetLabelCommonByLanguage("accountNo").ToUpper(), labelFont));
                        document.Add(new Paragraph(item.accountNum, detailsFont));
                        document.Add(new Paragraph(Environment.NewLine, labelFont));
                        document.Add(new Paragraph(GetLabelByLanguage("accountHolder").ToUpper(), labelFont));
                        document.Add(new Paragraph(!string.IsNullOrEmpty(item.accountOwnerName) ? item.accountOwnerName : Environment.NewLine, detailsFont));
                        document.Add(new Paragraph(Environment.NewLine, labelFont));
                        document.Add(new Paragraph(GetLabelCommonByLanguage("amountRM").ToUpper(), labelFont));
                        document.Add(new Paragraph(item.itmAmt, detailsFont));

                        document.Add(new Paragraph(Environment.NewLine, titleFont));
                        document.Add(grayLine);
                        document.Add(new Paragraph(Environment.NewLine, labelFont));
                    }

                    document.Add(new Paragraph(GetLabelByLanguage("trnDate").ToUpper(), labelFont));
                    document.Add(new Paragraph(response.GetData().payTransDate, detailsFont));

                    document.Add(new Paragraph(Environment.NewLine, titleFont));
                    document.Add(grayLine);
                    document.Add(new Paragraph(Environment.NewLine, labelFont));

                    document.Add(new Paragraph(GetLabelByLanguage("trnID").ToUpper(), labelFont));
                    document.Add(new Paragraph(response.GetData().payTransID, detailsFont));

                    document.Add(new Paragraph(Environment.NewLine, titleFont));
                    document.Add(grayLine);
                    document.Add(new Paragraph(Environment.NewLine, labelFont));

                    document.Add(new Paragraph(GetLabelByLanguage("paymentType").ToUpper(), labelFont));
                    document.Add(new Paragraph(response.GetData().paymentType, detailsFont));

                    document.Add(new Paragraph(Environment.NewLine, titleFont));
                    document.Add(grayLine);
                    document.Add(new Paragraph(Environment.NewLine, labelFont));

                    document.Add(new Paragraph(GetLabelByLanguage("trnMethod").ToUpper(), labelFont));
                    document.Add(new Paragraph(response.GetData().payMethod, detailsFont));

                    document.Add(new Paragraph(Environment.NewLine, titleFont));
                    document.Add(grayLine);
                    document.Add(new Paragraph(Environment.NewLine, labelFont));

                    if (response.GetData().payMethod.ToUpper() == "FPX")
                    {
                        document.Add(new Paragraph(GetLabelByLanguage("trnBankReference").ToUpper(), labelFont));
                        if (response.GetData().paymentRefBank != null && !string.IsNullOrEmpty(response.GetData().paymentRefBank))
                        {
                            document.Add(new Paragraph(response.GetData().paymentRefBank, detailsFont));
                        }
                        else
                        {
                            document.Add(new Paragraph(GetLabelByLanguage("emptyValue"), detailsFont));
                        }
                        document.Add(new Paragraph(Environment.NewLine, titleFont));
                        document.Add(grayLine);
                        document.Add(new Paragraph(Environment.NewLine, labelFont));
                    }

                    if (response.GetData().payMethod.ToUpper() == "FPX")
                    {
                        document.Add(new Paragraph(GetLabelByLanguage("trnAcquirerBank").ToUpper(), labelFont));
                        if (response.GetData().paymentAcqBank != null && !string.IsNullOrEmpty(response.GetData().paymentAcqBank))
                        {
                            document.Add(new Paragraph(response.GetData().paymentAcqBank, detailsFont));
                        }
                        else
                        {
                            document.Add(new Paragraph(GetLabelByLanguage("emptyValue"), detailsFont));
                        }
                        document.Add(new Paragraph(Environment.NewLine, titleFont));
                        document.Add(grayLine);
                        document.Add(new Paragraph(Environment.NewLine, labelFont));
                    }

                    document.Add(new Paragraph(GetLabelCommonByLanguage("totalAmountRM").ToUpper(), detailsFont));
                    document.Add(new Paragraph(response.GetData().payAmt, totalAmounFont));

                    document.Add(new Paragraph(Environment.NewLine, titleFont));
                    document.Add(grayLine);
                    document.Add(new Paragraph(Environment.NewLine, labelFont));

                    document.Add(new Paragraph(GetLabelByLanguage("note"), labelFont));


                    document.Close();
                    writer.Close();
                    fs.Close();

                    if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
                    {
                        mErrorMessageSnackBar.Dismiss();
                    }

                    RunOnUiThread(() =>
                    {
                        mProgressBar.Visibility = ViewStates.Gone;
                        OnSharePDF(path);
                        HideGetReceiptDialog();
                    });

                }
                catch (Exception e)
                {
                    Log.Debug("ViewReceiptActivity", e.StackTrace);
                    RunOnUiThread(() =>
                    {
                        mProgressBar.Visibility = ViewStates.Gone;
                        HideGetReceiptDialog();
                    });
                    Utility.LoggingNonFatalError(e);
                }


            });
        }

        public void ShowGetReceiptDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideGetReceiptDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowErrorMessage(string msg)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(baseView, msg, Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close")
                    , delegate
                    {
                        mErrorMessageSnackBar.Dismiss();
                    });
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }

        public void SetPresenter(ViewReceiptMultiAccountNewDesignContract.IUserActionsListener userActionListener)
        {
            this.iPresenter = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown && !IsFinishing;
        }

        public void OnShowReceiptDetails(GetPaymentReceiptResponse response)
        {
            baseView.Visibility = ViewStates.Visible;
            try
            {
                this.response = response;
                Log.Debug(TAG, "Receipt :" + response.GetData());
                MultiReceiptDetails receiptDetails = response.GetData();
                RECEPT_NO = receiptDetails.referenceNum;

                List<AccMultiPay> accounts = receiptDetails.accMultiPay;

                pleasedText.Text = string.Format(GetLabelByLanguage("messagePartTwo"), receiptDetails.payMethod);
                referenceNumberValue.Text = receiptDetails.referenceNum;
                txnDateValue.Text = receiptDetails.payTransDate;
                txnIdValue.Text = receiptDetails.payTransID;
                txnMethodValue.Text = receiptDetails.payMethod;
                totalAmtValue.Text = receiptDetails.payAmt;

                if (receiptDetails.payMethod.ToUpper() == "FPX")
                {
                    if (receiptDetails.paymentRefBank != null && !string.IsNullOrEmpty(receiptDetails.paymentRefBank))
                    {
                        txnBankRefValue.Text = receiptDetails.paymentRefBank;
                    }
                    else
                    {
                        txnBankRefValue.Text = GetLabelByLanguage("emptyValue");
                    }
                }
                else
                {
                    line_bankRef_value.Visibility = ViewStates.Gone;
                    txnBankRefValue.Visibility = ViewStates.Gone;
                    txnBankRefText.Visibility = ViewStates.Gone;
                }

                if (receiptDetails.payMethod.ToUpper() == "FPX")
                {
                    if (receiptDetails.paymentAcqBank != null && !string.IsNullOrEmpty(receiptDetails.paymentAcqBank))
                    {
                        txnAcqrBankValue.Text = receiptDetails.paymentAcqBank;
                    }
                    else
                    {
                        txnAcqrBankValue.Text = GetLabelByLanguage("emptyValue");
                    }
                }
                else
                {
                    line_AcqrBank_value.Visibility = ViewStates.Gone;
                    txnAcqrBankValue.Visibility = ViewStates.Gone;
                    txnAcqrBankText.Visibility = ViewStates.Gone;
                }

                if (isApplicationReceipt)
                {
                    paymentTypeText.Text = GetLabelByLanguage("paymentType").ToUpper();
                    paymentTypeValue.Text = receiptDetails.paymentType;
                }
                linePaymentType.Visibility = isApplicationReceipt ? ViewStates.Visible : ViewStates.Gone;
                paymentTypeText.Visibility = isApplicationReceipt ? ViewStates.Visible : ViewStates.Gone;
                paymentTypeValue.Visibility = isApplicationReceipt ? ViewStates.Visible : ViewStates.Gone;

                TextViewUtils.SetTextSize14(pleasedText, referenceNumberValue, txnDateValue
                    , txnIdValue, txnMethodValue, paymentTypeValue);
                TextViewUtils.SetTextSize20(totalAmtValue);

                if (accountLayout.ChildCount > 0)
                {
                    accountLayout.RemoveAllViews();
                }

                if (accounts != null && accounts.Count() > 0)
                {
                    foreach (AccMultiPay acct in accounts)
                    {
                        View view = LayoutInflater.From(this).Inflate(Resource.Layout.ViewBillReceiptAccountDetails, baseView, false);
                        TextView accNumberText = view.FindViewById<TextView>(Resource.Id.acc_number_text);
                        TextView accNumberValue = view.FindViewById<TextView>(Resource.Id.acc_number_value);
                        TextView accNameText = view.FindViewById<TextView>(Resource.Id.acc_name_text);
                        TextView accNameValue = view.FindViewById<TextView>(Resource.Id.acc_name_value);
                        TextView accAmtText = view.FindViewById<TextView>(Resource.Id.acc_amt_text);
                        TextView accAmtValue = view.FindViewById<TextView>(Resource.Id.acc_amt_value);

                        TextViewUtils.SetMuseoSans500Typeface(accNumberText, accNumberValue
                            , accNameText, accNameValue, accAmtText, accAmtValue);
                        TextViewUtils.SetTextSize10(accNumberText, accNameText, accAmtText);
                        TextViewUtils.SetTextSize14(accNumberValue, accNameValue, accAmtValue);

                        accNumberText.Text = GetLabelCommonByLanguage("accountNo").ToUpper();
                        accNameText.Text = GetLabelByLanguage("accountHolder").ToUpper();
                        accAmtText.Text = GetLabelCommonByLanguage("amountRM").ToUpper();

                        accNumberValue.Text = acct.accountNum;
                        accNameValue.Text = acct.accountOwnerName;
                        accAmtValue.Text = acct.itmAmt;
                        accountLayout.AddView(view);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                ShowPaymentReceiptError();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (requestCode == Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE)
            {
                if (Utility.IsPermissionHasCount(grantResults))
                {
                    if (grantResults[0] == Permission.Granted)
                    {
                        RunOnUiThread(() =>
                        {
                            CreatePDF(response);
                        });
                    }
                }
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                FirebaseAnalyticsUtils.SetScreenName(this, "View Payment Receipt");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public void ShowPaymentReceiptError()
        {
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(Utility.GetLocalizedErrorLabel("defaultErrorTitle"))
                .SetMessage(Utility.GetLocalizedErrorLabel("receiptErrorMsg"))
                .SetContentGravity(GravityFlags.Center)
                .SetCTALabel(Utility.GetLocalizedCommonLabel("ok"))
                .SetCTAaction(Finish)
                .Build()
                .Show();
        }

        private void OnSharePDF(string savedPDFPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(savedPDFPath))
                {
                    Java.IO.File file = new Java.IO.File(savedPDFPath);
                    Android.Net.Uri fileUri = FileProvider.GetUriForFile(this,
                                                ApplicationContext.PackageName + ".fileprovider", file);

                    Intent shareIntent = new Intent(Intent.ActionSend);
                    shareIntent.SetType("application/pdf");
                    shareIntent.PutExtra(Intent.ExtraStream, fileUri);
                    StartActivity(Intent.CreateChooser(shareIntent, Utility.GetLocalizedLabel("Profile", "share")));
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}