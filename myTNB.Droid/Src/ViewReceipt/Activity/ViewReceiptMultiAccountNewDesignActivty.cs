﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using iTextSharp.text;
using iTextSharp.text.pdf;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using myTNB_Android.Src.ViewReceipt.Model;
using myTNB_Android.Src.ViewReceipt.MVP;

namespace myTNB_Android.Src.ViewReceipt.Activity
{
    [Activity(Label = "Payment Receipt"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Dashboard")]
    public class ViewReceiptMultiAccountNewDesignActivty : BaseToolbarAppCompatActivity, ViewReceiptMultiAccountNewDesignContract.IView
    {
        private string TAG = "View Receipt Activity";
        private string RECEPT_NO = "101010010";

        [BindView(Resource.Id.progressBar)]
        ProgressBar mProgressBar;

        [BindView(Resource.Id.scrollView)]
        ScrollView baseView;

        [BindView(Resource.Id.receipt_titile)]
        TextView receiptTitile;

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
        TextView txnDSateText;

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

        ViewReceiptMultiAccountNewDesignPresenter mPresenter = null;
        ViewReceiptMultiAccountNewDesignContract.IUserActionsListener iPresenter = null;

        private AlertDialog mGetReceiptDialog;
        private Snackbar mErrorMessageSnackBar;
        private bool downloadClicked = false;

        private static Snackbar mErrorNoInternet;

        AccountData selectedAccount;
        BillHistory selectedBill;
        GetMultiReceiptByTransIdResponse response = null;

        private LoadingOverlay loadingOverlay;

        string pleaseTextStr = "We are pleased to inform you that the following online payment via {0} is Successful:";

        public override int ResourceId()
        {
            return Resource.Layout.ViewBillReceiptNewDesign;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            baseView.Visibility = ViewStates.Gone;
            try {
            mPresenter = new ViewReceiptMultiAccountNewDesignPresenter(this);
            // Create your application here


            mProgressBar.Visibility = ViewStates.Gone;
        

        TextViewUtils.SetMuseoSans500Typeface(noteText, totalAmtValue, totalAmtText, txnMethodValue, txnMethodText,
                                                  txnIdValue, txnIdText, txnDateValue, txnDSateText, referenceNumberValue,
                                                  referenceNumberText, pleasedText, thanksText, dearCustomer, receiptTitile);

            mGetReceiptDialog = new AlertDialog.Builder(this)
              .SetTitle("Get Receipt")
              .SetMessage("Please wait while we are getting receipt details...")
              .SetCancelable(false)
              .Create();

           
            string apiKeyID = Constants.APP_CONFIG.API_KEY_ID;
            string merchantTransId = Intent.Extras.GetString("merchantTransId");
            if (ConnectionUtils.HasInternetConnection(this)) {
            this.iPresenter.GetReceiptDetails(apiKeyID, merchantTransId);
            } else {
                ShowErrorMessage(GetString(Resource.String.no_internet_connection));
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ViewBillReceiptMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_download:
                    if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                    {
                        downloadClicked = true;
                        //OnDownloadPDF();
                        createPDF(response);

                    }
                    else
                    {
                        RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);
                    }                    
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }


        private void WriteGrayContent(PdfPTable tableLayout)
        {
            try {
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

        public void createPDF(GetMultiReceiptByTransIdResponse response)
        {
            if (downloadClicked)
            {
                if (response != null)
                {
                    if (response.receipt.Status.Equals("success"))
                    {
                        mProgressBar.Visibility = ViewStates.Gone;
                        ShowGetReceiptDialog();

                        RunOnUiThread(() =>
                        {
                            Log.Debug(TAG, "Receipt :" + response.receipt.receiptDetails);
                            RECEPT_NO = response.receipt.receiptDetails.referenceNum;
                            try
                            {
                                var directory = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory, "pdf").ToString();
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


                                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 25, 25, 30, 30);

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
                                //Font titleFont = new Font(FontFactory.GetFont(titleBf, 30f, blueColour));
                                //Font detailsFont = new Font(FontFactory.GetFont(myTNBFont.FONTNAME_500, 24f, tunaGreyColour));
                                //Font labelFont = new Font(FontFactory.GetFont(myTNBFont.FONTNAME_500, 18f, silverChaliceColour));
                                //Font totalAmounFont = new Font(FontFactory.GetFont(myTNBFont.FONTNAME_500, 48f, tunaGreyColour));

                                //Font titleFont = new Font(FontFactory.GetFont(FontFactory.TIMES_BOLD, 30f, blueColour));
                                //Font detailsFont = new Font(FontFactory.GetFont(FontFactory.TIMES_BOLD, 24f, tunaGreyColour));
                                //Font labelFont = new Font(FontFactory.GetFont(FontFactory.TIMES_BOLD, 18f, silverChaliceColour));
                                //Font totalAmounFont = new Font(FontFactory.GetFont(FontFactory.TIMES_BOLD, 48f, tunaGreyColour));


                                Drawable d = ContextCompat.GetDrawable(this, Resource.Drawable.tnb_receipt_logo_header);
                                Bitmap bitmap = ((BitmapDrawable)d).Bitmap;
                                MemoryStream stream = new MemoryStream();
                                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                                byte[] bitmapdata = stream.ToArray();

                                //string filepath = Android.OS.Environment.;
                                var headerImage = iTextSharp.text.Image.GetInstance(bitmapdata);

                                document.Open();



                                //document.Open();
                                PdfContentByte cb = writer.DirectContent;

                                PdfPTable grayLine = new PdfPTable(1);
                                grayLine.TotalWidth = document.PageSize.Width - 40;
                                WriteGrayContent(grayLine);

                                headerImage.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                                //headerImage.SetAbsolutePosition(0, document.PageSize.Height - headerImage.Height + 5);//+ 30);

                                //headerImage.SetAbsolutePosition(0, document.PageSize.Height - headerImage.Height + 5);//+ 30);

                                float y = document.PageSize.Height - document.TopMargin - headerImage.Height;

                                if (y < 700) {
                                    float diff = 700 - y;
                                    y = y + diff;
                                }

                                headerImage.SetAbsolutePosition(0, y);

                                document.Add(headerImage);
                                document.Add(new Paragraph(System.Environment.NewLine, titleFont));
                                document.Add(new Paragraph(System.Environment.NewLine, titleFont));
                                document.Add(new Paragraph(System.Environment.NewLine, titleFont));
                                document.Add(new Paragraph(System.Environment.NewLine, detailsFont));
                                document.Add(new Paragraph(GetString(Resource.String.receipt_title), titleFont));
                                document.Add(new Paragraph(System.Environment.NewLine, titleFont));
                                document.Add(new Paragraph(GetString(Resource.String.receipt_dear_customer), detailsFont));
                                document.Add(new Paragraph(System.Environment.NewLine, detailsFont));
                                document.Add(new Paragraph(GetString(Resource.String.receipt_payment_thank_you), detailsFont));
                                document.Add(new Paragraph(string.Format(pleaseTextStr,
                                                                         response.receipt.receiptDetails.payMethod), detailsFont));

                                document.Add(new Paragraph(System.Environment.NewLine, titleFont));
                                document.Add(grayLine);
                                document.Add(new Paragraph(System.Environment.NewLine, labelFont));

                                document.Add(new Paragraph(GetString(Resource.String.receipt_reference_number), labelFont));
                                document.Add(new Paragraph(response.receipt.receiptDetails.referenceNum, detailsFont));

                                document.Add(new Paragraph(System.Environment.NewLine, titleFont));
                                document.Add(grayLine);
                                document.Add(new Paragraph(System.Environment.NewLine, labelFont));

                                foreach (var item in response.receipt.receiptDetails.accMultiPay)
                                {
                                    document.Add(new Paragraph(GetString(Resource.String.receipt_account_number), labelFont));
                                    document.Add(new Paragraph(item.accountNum, detailsFont));
                                    document.Add(new Paragraph(System.Environment.NewLine, labelFont));
                                    document.Add(new Paragraph(GetString(Resource.String.receipt_account_holder_name), labelFont));
                                    document.Add(new Paragraph(!string.IsNullOrEmpty(item.accountOwnerName) ? item.accountOwnerName : System.Environment.NewLine, detailsFont));
                                    document.Add(new Paragraph(System.Environment.NewLine, labelFont));
                                    document.Add(new Paragraph(GetString(Resource.String.receipt_account_amount), labelFont));
                                    document.Add(new Paragraph(item.itmAmt, detailsFont));

                                    document.Add(new Paragraph(System.Environment.NewLine, titleFont));
                                    document.Add(grayLine);
                                    document.Add(new Paragraph(System.Environment.NewLine, labelFont));
                                }

                                document.Add(new Paragraph(GetString(Resource.String.receipt_date), labelFont));
                                document.Add(new Paragraph(response.receipt.receiptDetails.payTransDate, detailsFont));

                                document.Add(new Paragraph(System.Environment.NewLine, titleFont));
                                document.Add(grayLine);
                                document.Add(new Paragraph(System.Environment.NewLine, labelFont));

                                document.Add(new Paragraph(GetString(Resource.String.receipt_transcation_id), labelFont));
                                document.Add(new Paragraph(response.receipt.receiptDetails.payTransID, detailsFont));

                                document.Add(new Paragraph(System.Environment.NewLine, titleFont));
                                document.Add(grayLine);
                                document.Add(new Paragraph(System.Environment.NewLine, labelFont));

                                document.Add(new Paragraph(GetString(Resource.String.receipt_transaction_method), labelFont));
                                document.Add(new Paragraph(response.receipt.receiptDetails.payMethod, detailsFont));

                                document.Add(new Paragraph(System.Environment.NewLine, titleFont));
                                document.Add(grayLine);
                                document.Add(new Paragraph(System.Environment.NewLine, labelFont));

                                document.Add(new Paragraph(GetString(Resource.String.receipt_total_amount), detailsFont));
                                document.Add(new Paragraph(response.receipt.receiptDetails.payAmt, totalAmounFont));

                                document.Add(new Paragraph(System.Environment.NewLine, titleFont));
                                document.Add(grayLine);
                                document.Add(new Paragraph(System.Environment.NewLine, labelFont));

                                document.Add(new Paragraph(GetString(Resource.String.receipt_note_text), labelFont));


                                document.Close();
                                writer.Close();
                                fs.Close();

                                if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
                                {
                                    mErrorMessageSnackBar.Dismiss();
                                }

                                mErrorMessageSnackBar = Snackbar.Make(baseView, "File has been downloaded to location :" + path, Snackbar.LengthIndefinite)
                                .SetAction("Open", delegate
                                {
                                    Java.IO.File file = new Java.IO.File(path);
                                    Android.Net.Uri fileUri = FileProvider.GetUriForFile(this, 
                                            ApplicationContext.PackageName + ".provider", file);

                                    Intent intent = new Intent(Intent.ActionView);
                                    intent.SetDataAndType(fileUri, "application/pdf");
                                    intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                    StartActivity(intent);
                                    mErrorMessageSnackBar.Dismiss();
                                }
                                );

                                View v = mErrorMessageSnackBar.View;
                                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                                tv.SetMaxLines(5);
                                Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
                                btn.SetTextColor(Android.Graphics.Color.Yellow);
                                mErrorMessageSnackBar.Show();
                                downloadClicked = false;
                                mProgressBar.Visibility = ViewStates.Gone;
                                HideGetReceiptDialog();

                            }
                            catch (Exception e)
                            {
                                Log.Debug("ViewReceiptActivity", e.StackTrace);
                                downloadClicked = false;
                                mProgressBar.Visibility = ViewStates.Gone;
                                HideGetReceiptDialog();
                                Utility.LoggingNonFatalError(e);
                            }
                        });
                    }

                } else {
                    if (!ConnectionUtils.HasInternetConnection(this)) {
                            ShowErrorMessage(GetString(Resource.String.no_internet_connection));
                    }
                }
            }
            downloadClicked = false;
        }

        public void ShowGetReceiptDialog()
        {
            //if (this.mGetReceiptDialog != null && !this.mGetReceiptDialog.IsShowing)
            //{
            //    this.mGetReceiptDialog.Show();
            //}
            try {
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideGetReceiptDialog()
        {
            //if (this.mGetReceiptDialog != null && this.mGetReceiptDialog.IsShowing)
            //{
            //    this.mGetReceiptDialog.Dismiss();
            //}
            try {
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

        public void ShowErrorMessage(string msg)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(baseView, msg, Snackbar.LengthIndefinite)
            .SetAction("Close", delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }

        public void OnDownloadPDF()
        {
            createPDF(response);
        }

        public void SetPresenter(ViewReceiptMultiAccountNewDesignContract.IUserActionsListener userActionListener)
        {
            this.iPresenter = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown && !IsFinishing;
        }

        public void OnShowReceiptDetails(GetMultiReceiptByTransIdResponse response)
        {
            baseView.Visibility = ViewStates.Visible;
            try {
            this.response = response;
            if (response != null) {
                if (response.receipt.Status.Equals("success"))
                {
                    Log.Debug(TAG, "Receipt :" + response.receipt.receiptDetails);
                    MultiReceiptDetails receiiptDetails = response.receipt.receiptDetails;
                    RECEPT_NO = receiiptDetails.referenceNum;

                    List<AccMultiPay> accounts = receiiptDetails.accMultiPay;

                    pleasedText.Text = string.Format(pleaseTextStr, receiiptDetails.payMethod);

                    referenceNumberValue.Text = receiiptDetails.referenceNum;

                    txnDateValue.Text = receiiptDetails.payTransDate;

                    txnIdValue.Text = receiiptDetails.payTransID;

                    txnMethodValue.Text = receiiptDetails.payMethod;

                    totalAmtValue.Text = receiiptDetails.payAmt;

                    if (accountLayout.ChildCount > 0) {
                        accountLayout.RemoveAllViews();
                    }

                    if (accounts != null && accounts.Count() > 0) {
                    foreach (AccMultiPay acct in accounts)
                    {
                            View view = LayoutInflater.From(this).Inflate(Resource.Layout.ViewBillReceiptAccountDetails, baseView, false);
                            TextView accNumberText = view.FindViewById<TextView>(Resource.Id.acc_number_text);
                            TextView accNumberValue = view.FindViewById<TextView>(Resource.Id.acc_number_value);
                            TextView accNameText = view.FindViewById<TextView>(Resource.Id.acc_name_text);
                            TextView accNameValue = view.FindViewById<TextView>(Resource.Id.acc_name_value);
                            TextView accAmtText = view.FindViewById<TextView>(Resource.Id.acc_amt_text);
                            TextView accAmtValue = view.FindViewById<TextView>(Resource.Id.acc_amt_value);


                            TextViewUtils.SetMuseoSans500Typeface(accNumberText, accNumberValue, accNameText, 
                                                                  accNameValue, accAmtText, accAmtValue);

                            accNumberValue.Text = acct.accountNum;
                            accNameValue.Text = acct.accountOwnerName;
                            accAmtValue.Text = acct.itmAmt;


                            accountLayout.AddView(view);

                    }
                    }

                }          
            }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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
                            downloadClicked = true;
                            createPDF(response);
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
