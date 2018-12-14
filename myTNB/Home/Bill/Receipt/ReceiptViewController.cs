using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using CoreGraphics;
using myTNB.Model;
using System.Drawing;
using System.Threading.Tasks;

namespace myTNB
{
    public partial class ReceiptViewController : UIViewController
    {
        public ReceiptViewController(IntPtr handle) : base(handle)
        {
        }

        const int START_PDF_Y_LOC = 600;
        const int END_PDF_Y_LOC = 50;
        const int ACCNUM_X_LOC = 220;
        const int AMT_X_LOC = 400;

        ReceiptResponseModel _receipt = new ReceiptResponseModel();
        UIWebView _webViewReceipt;
        string _pdfFilePath = string.Empty;

        public string MerchatTransactionID = string.Empty;
        public bool isCCFlow = false;
        string paymentMethod = String.Empty;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ActivityIndicator.Show();
            paymentMethod = isCCFlow ? "Credit Card / Debit card" : "FPX";
            Task[] taskList = new Task[] { GetReceipt() };
            Task.WaitAll(taskList);
            if (_receipt != null && _receipt.d != null && _receipt.d.data != null)
            {
                CreatePDF();
                SetSubviews();
            }
            else
            {
                var alert = UIAlertController.Create("No Receipt Found", "", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                PresentViewController(alert, animated: true, completionHandler: null);
            }
            ActivityIndicator.Hide();
        }

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Payment Receipt");
            titleBarComponent.SetNotificationVisibility(false);
            titleBarComponent.SetNotificationImage("IC-Header-Share");
            titleBarComponent.SetNotificationAction(new UITapGestureRecognizer(() =>
            {
                var viewer = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(_pdfFilePath));
                viewer.PresentOpenInMenu(new RectangleF(0, -260, 320, 320), this.View, true);
            }));
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                if (File.Exists(_pdfFilePath))
                {
                    File.Delete(_pdfFilePath);
                }
                if (isCCFlow)
                {
                    NavigationController.PopViewController(true);
                }
                else
                {
                    DataManager.DataManager.SharedInstance.IsPaymentDone = true;
                    DismissViewController(true, null);
                }

            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void SetSubviews()
        {
            if (_webViewReceipt != null)
            {
                _webViewReceipt.RemoveFromSuperview();
            }
            _webViewReceipt = new UIWebView(new CGRect(0, DeviceHelper.IsIphoneX() ? 86 : 64, View.Frame.Width, View.Frame.Height));
            _webViewReceipt.Delegate = new WebViewDelegate(View);
            string nsURL = _pdfFilePath;
            if (File.Exists(_pdfFilePath))
            {
                nsURL = _pdfFilePath;
            }
            _webViewReceipt.LoadRequest(new NSUrlRequest(new NSUrl(nsURL)));
            _webViewReceipt.ScalesPageToFit = true;
            View.AddSubview(_webViewReceipt);
        }

        void SetNewPage(ref Document document, ref PdfContentByte cb, ref int xLocation, BaseFont baseFont)
        {
            xLocation = 770;
            cb.EndText();
            document.NewPage();
            cb.BeginText();
            cb.SetFontAndSize(baseFont, 24);
        }

        internal void CreatePDF()
        {
            string pdfFileName = string.Format("Receipt-{0}.pdf", _receipt.d.data.referenceNum);
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _pdfFilePath = Path.Combine(documentsPath, pdfFileName);
            if (File.Exists(_pdfFilePath))
            {
                File.Delete(_pdfFilePath);
            }
            FileStream fs = new FileStream(_pdfFilePath, FileMode.Create);
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            BaseFont baseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font pdfFont = new Font(baseFont, 24, iTextSharp.text.Font.NORMAL);

            document.Open();
            PdfContentByte cb = writer.DirectContent;

            document.Add(new Paragraph("Dear Customer,", pdfFont));
            document.Add(new Paragraph("", pdfFont));
            document.Add(new Paragraph("Thank you for using myTNB.", pdfFont));
            document.Add(new Paragraph(string.Format("We are pleased to inform you that the following online payment via {0} is Successful:", paymentMethod), pdfFont));
            document.Add(new Paragraph("", pdfFont));

            cb.BeginText();
            cb.SetFontAndSize(baseFont, 24);

            cb.SetTextMatrix(20, 600); cb.ShowText("Reference");
            cb.SetTextMatrix(20, 570); cb.ShowText("Number");
            cb.SetTextMatrix(200, 590); cb.ShowText(":");
            cb.SetTextMatrix(220, 590); cb.ShowText(_receipt.d.data.referenceNum);

            cb.SetTextMatrix(20, 540); cb.ShowText("Transaction");
            cb.SetTextMatrix(20, 510); cb.ShowText("Date");
            cb.SetTextMatrix(200, 530); cb.ShowText(":");
            cb.SetTextMatrix(220, 530); cb.ShowText(_receipt.d.data.payTransDate);

            cb.SetTextMatrix(20, 480); cb.ShowText("Amount");
            cb.SetTextMatrix(200, 480); cb.ShowText(":");
            cb.SetTextMatrix(220, 480); cb.ShowText(_receipt.d.data.payAmt);

            cb.SetTextMatrix(20, 440); cb.ShowText("From Account");
            cb.SetTextMatrix(200, 440); cb.ShowText(":");
            cb.SetTextMatrix(220, 440); cb.ShowText(_receipt.d.data.customerName);

            cb.SetTextMatrix(20, 400); cb.ShowText("Account");
            cb.SetTextMatrix(20, 370); cb.ShowText("Number");

            int xLocation = 390;
            if (_receipt.d.data.accMultiPay.Count > 1)
            {
                xLocation = xLocation - 20;
                cb.SetTextMatrix(200, xLocation); cb.ShowText(":");
                foreach (var item in _receipt.d.data.accMultiPay)
                {
                    cb.SetTextMatrix(ACCNUM_X_LOC, xLocation); cb.ShowText(item.accountNum);
                    cb.SetTextMatrix(AMT_X_LOC, xLocation); cb.ShowText(item.itmAmt);
                    xLocation = xLocation - 30;
                    if (xLocation <= 50)
                    {
                        SetNewPage(ref document, ref cb, ref xLocation, baseFont);
                    }
                }
                xLocation = xLocation + 30;
            }
            else
            {
                cb.SetTextMatrix(200, 390); cb.ShowText(":");
                if (_receipt.d.data.accMultiPay.Count == 1)
                {
                    cb.SetTextMatrix(220, 390); cb.ShowText(_receipt.d.data.accMultiPay[0].accountNum);
                }
            }

            if (xLocation <= 50 || (xLocation - 80) <= 50)
            {
                SetNewPage(ref document, ref cb, ref xLocation, baseFont);
            }

            cb.SetTextMatrix(20, xLocation - 50); cb.ShowText("Transaction");
            cb.SetTextMatrix(20, xLocation - 80); cb.ShowText("Method");
            cb.SetTextMatrix(200, xLocation - 60); cb.ShowText(":");
            cb.SetTextMatrix(220, xLocation - 60); cb.ShowText(_receipt.d.data.payMethod);
            xLocation = xLocation - 80;

            if (xLocation <= 50 || (xLocation - 30) <= 50)
            {
                SetNewPage(ref document, ref cb, ref xLocation, baseFont);
            }

            cb.SetTextMatrix(20, xLocation - 30); cb.ShowText("Transaction ID");
            cb.SetTextMatrix(200, xLocation - 30); cb.ShowText(":");
            cb.SetTextMatrix(220, xLocation - 30); cb.ShowText(_receipt.d.data.payTransID);
            xLocation = xLocation - 30;

            if (xLocation - 80 == 0)
            {
                SetNewPage(ref document, ref cb, ref xLocation, baseFont);
            }
            cb.SetTextMatrix(20, xLocation - 80); cb.ShowText("Thank you for using myTNB.");

            cb.EndText();
            document.Close();
            writer.Close();
            fs.Close();
        }

        internal Task GetReceipt()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    merchant_transId = MerchatTransactionID
                };
                _receipt = serviceManager.GetReceipt("GetMultiReceiptByTransId", requestParameter);
            });
        }
    }
}