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
using myTNB.Extensions;
using myTNB.Home.Bill.Receipt;

namespace myTNB
{
    public partial class ReceiptViewController : UIViewController
    {
        public ReceiptViewController(IntPtr handle) : base(handle)
        {
        }

        public Action OnDone { get; set; }

        const int START_PDF_Y_LOC = 600;
        const int END_PDF_Y_LOC = 50;
        const int ACCNUM_X_LOC = 220;
        const int AMT_X_LOC = 400;
        const float PADDING = 10f;
        const float INNER_PADDING = 20f;
        const float LBL_WIDTH_PADDING = INNER_PADDING * 2;

        ReceiptResponseModel _receipt = new ReceiptResponseModel();
        //UIWebView _webViewReceipt;
        string _pdfFilePath = string.Empty;

        public string MerchatTransactionID = string.Empty;
        public bool isCCFlow = false;
        string paymentMethod = String.Empty;

        UIView _headerView;
        UIView _footerView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ActivityIndicator.Show();
            Task[] taskList = new Task[] { GetReceipt() };
            Task.WaitAll(taskList);
            if (_receipt != null && _receipt?.d != null && _receipt?.d?.data != null && _receipt?.d?.didSucceed == true)
            {
                paymentMethod = _receipt?.d?.data?.payMethod;
                CreatePDF();
                SetSubviews();
            }
            else
            {
                var alert = UIAlertController.Create("PDFNoReceipt".Translate(), "", UIAlertControllerStyle.Alert);
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
            titleBarComponent.SetTitle("PDFNavTitle".Translate());
            titleBarComponent.SetNotificationVisibility(false);
            titleBarComponent.SetNotificationImage("IC-Header-Share");
            titleBarComponent.SetNotificationAction(new UITapGestureRecognizer(() =>
            {
                var viewer = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(_pdfFilePath));
                UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
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
                    OnDone?.Invoke();
                }

            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void SetSubviews()
        {
            //if (_webViewReceipt != null)
            //{
            //    _webViewReceipt.RemoveFromSuperview();
            //}
            //_webViewReceipt = new UIWebView(new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 86 : 64, View.Frame.Width, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 86 : 64)));
            //_webViewReceipt.Delegate = new WebViewDelegate(View);
            //string nsURL = _pdfFilePath;
            //if (File.Exists(_pdfFilePath))
            //{
            //    nsURL = _pdfFilePath;
            //}
            //_webViewReceipt.LoadRequest(new NSUrlRequest(new NSUrl(nsURL)));
            //_webViewReceipt.ScalesPageToFit = true;
            //View.AddSubview(_webViewReceipt);

            View.BackgroundColor = myTNBColor.SilverChalice();
            tableViewReceipt.Frame = new CGRect(0 + PADDING, DeviceHelper.IsIphoneXUpResolution() ? 86 + PADDING : 64 + PADDING, View.Frame.Width - (PADDING * 2), View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 86 + PADDING : 64 + PADDING));
            tableViewReceipt.BackgroundColor = myTNBColor.SilverChalice();
            tableViewReceipt.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            _headerView = new UIView(new CGRect(0, 0, tableViewReceipt.Frame.Width, 377));
            _headerView.BackgroundColor = UIColor.White;

            UIImageView imageView = new UIImageView(new CGRect(0, 0, _headerView.Frame.Width, DeviceHelper.GetScaledHeight(80)));
            imageView.Image = UIImage.FromBundle("Receipt-Header");
            //imageView.ContentMode = UIViewContentMode.ScaleToFill;

            UILabel paymentTitle = new UILabel(new CGRect(INNER_PADDING, imageView.Frame.GetMaxY() + INNER_PADDING, _headerView.Frame.Width - LBL_WIDTH_PADDING, 26));
            paymentTitle.TextAlignment = UITextAlignment.Left;
            paymentTitle.Font = myTNBFont.MuseoSans20_500();
            paymentTitle.TextColor = myTNBColor.PowerBlue();
            paymentTitle.Text = "PDFTitle".Translate();

            UILabel msgTitle = new UILabel(new CGRect(INNER_PADDING, paymentTitle.Frame.GetMaxY() + INNER_PADDING, _headerView.Frame.Width - LBL_WIDTH_PADDING, 16));
            msgTitle.TextAlignment = UITextAlignment.Left;
            msgTitle.Font = myTNBFont.MuseoSans14_500();
            msgTitle.TextColor = myTNBColor.TunaGrey();
            msgTitle.Text = "PDFSalutation".Translate();

            UILabel msgBody = new UILabel(new CGRect(INNER_PADDING, msgTitle.Frame.GetMaxY() + INNER_PADDING, _headerView.Frame.Width - LBL_WIDTH_PADDING, 80));
            msgBody.TextAlignment = UITextAlignment.Left;
            msgBody.Font = myTNBFont.MuseoSans14_500();
            msgBody.TextColor = myTNBColor.TunaGrey();
            msgBody.Text = string.Format("PDFMessageFull".Translate(), paymentMethod);
            msgBody.Lines = 0;
            msgBody.LineBreakMode = UILineBreakMode.WordWrap;

            UIView viewLineTop = new UIView(new CGRect(INNER_PADDING, msgBody.Frame.GetMaxY() + INNER_PADDING, _headerView.Frame.Width - LBL_WIDTH_PADDING, 1));
            viewLineTop.BackgroundColor = myTNBColor.PlatinumGrey();

            UILabel lblReference = new UILabel(new CGRect(INNER_PADDING, viewLineTop.Frame.GetMaxY() + INNER_PADDING, _headerView.Frame.Width - LBL_WIDTH_PADDING, 16));
            lblReference.TextAlignment = UITextAlignment.Left;
            lblReference.Font = myTNBFont.MuseoSans10_500();
            lblReference.TextColor = myTNBColor.SilverChalice();
            lblReference.Text = "PDFRefNumber".Translate();

            UILabel lblReferenceValue = new UILabel(new CGRect(INNER_PADDING, lblReference.Frame.GetMaxY(), _headerView.Frame.Width - LBL_WIDTH_PADDING, 16));
            lblReferenceValue.TextAlignment = UITextAlignment.Left;
            lblReferenceValue.Font = myTNBFont.MuseoSans14_500();
            lblReferenceValue.TextColor = myTNBColor.TunaGrey();
            lblReferenceValue.Text = _receipt?.d?.data?.referenceNum;

            UIView viewLineBottom = new UIView(new CGRect(INNER_PADDING, lblReferenceValue.Frame.GetMaxY() + INNER_PADDING, _headerView.Frame.Width - LBL_WIDTH_PADDING, 1));
            viewLineBottom.BackgroundColor = myTNBColor.PlatinumGrey();

            _headerView.AddSubviews(new UIView[] { imageView, paymentTitle, msgTitle, msgBody, viewLineTop, lblReference, lblReferenceValue, viewLineBottom });

            _footerView = new UIView(new CGRect(0, 0, tableViewReceipt.Frame.Width, 400));
            _footerView.BackgroundColor = UIColor.White;

            UILabel lblTrxDate = new UILabel(new CGRect(INNER_PADDING, INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 16));
            lblTrxDate.TextAlignment = UITextAlignment.Left;
            lblTrxDate.Font = myTNBFont.MuseoSans10_500();
            lblTrxDate.TextColor = myTNBColor.SilverChalice();
            lblTrxDate.Text = "PDFTrnxDate".Translate();

            UILabel lblTrxDateValue = new UILabel(new CGRect(INNER_PADDING, lblTrxDate.Frame.GetMaxY(), _footerView.Frame.Width - LBL_WIDTH_PADDING, 16));
            lblTrxDateValue.TextAlignment = UITextAlignment.Left;
            lblTrxDateValue.Font = myTNBFont.MuseoSans14_500();
            lblTrxDateValue.TextColor = myTNBColor.TunaGrey();
            lblTrxDateValue.Text = _receipt?.d?.data?.payTransDate;

            UIView viewLine2 = new UIView(new CGRect(INNER_PADDING, lblTrxDateValue.Frame.GetMaxY() + INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 1));
            viewLine2.BackgroundColor = myTNBColor.PlatinumGrey();

            UILabel lblTrxID = new UILabel(new CGRect(INNER_PADDING, viewLine2.Frame.GetMaxY() + INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 16));
            lblTrxID.TextAlignment = UITextAlignment.Left;
            lblTrxID.Font = myTNBFont.MuseoSans10_500();
            lblTrxID.TextColor = myTNBColor.SilverChalice();
            lblTrxID.Text = "PDFTrnxId".Translate();

            UILabel lblTrxIDValue = new UILabel(new CGRect(INNER_PADDING, lblTrxID.Frame.GetMaxY(), _footerView.Frame.Width - LBL_WIDTH_PADDING, 16));
            lblTrxIDValue.TextAlignment = UITextAlignment.Left;
            lblTrxIDValue.Font = myTNBFont.MuseoSans14_500();
            lblTrxIDValue.TextColor = myTNBColor.TunaGrey();
            lblTrxIDValue.Text = _receipt?.d?.data?.payTransID;

            UIView viewLine3 = new UIView(new CGRect(INNER_PADDING, lblTrxIDValue.Frame.GetMaxY() + INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 1));
            viewLine3.BackgroundColor = myTNBColor.PlatinumGrey();

            UILabel lblTrxMethod = new UILabel(new CGRect(INNER_PADDING, viewLine3.Frame.GetMaxY() + INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 16));
            lblTrxMethod.TextAlignment = UITextAlignment.Left;
            lblTrxMethod.Font = myTNBFont.MuseoSans10_500();
            lblTrxMethod.TextColor = myTNBColor.SilverChalice();
            lblTrxMethod.Text = "PDFTrnxMethod".Translate();

            UILabel lblTrxMethodValue = new UILabel(new CGRect(INNER_PADDING, lblTrxMethod.Frame.GetMaxY(), _footerView.Frame.Width - LBL_WIDTH_PADDING, 16));
            lblTrxMethodValue.TextAlignment = UITextAlignment.Left;
            lblTrxMethodValue.Font = myTNBFont.MuseoSans14_500();
            lblTrxMethodValue.TextColor = myTNBColor.TunaGrey();
            lblTrxMethodValue.Text = _receipt?.d?.data?.payMethod;

            UIView viewLine4 = new UIView(new CGRect(INNER_PADDING, lblTrxMethodValue.Frame.GetMaxY() + INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 1));
            viewLine4.BackgroundColor = myTNBColor.PlatinumGrey();

            UILabel lblTotalAmount = new UILabel(new CGRect(INNER_PADDING, viewLine4.Frame.GetMaxY() + (INNER_PADDING + 10), _footerView.Frame.Width - LBL_WIDTH_PADDING, 16));
            lblTotalAmount.TextAlignment = UITextAlignment.Left;
            lblTotalAmount.Font = myTNBFont.MuseoSans14_500();
            lblTotalAmount.TextColor = myTNBColor.TunaGrey();
            lblTotalAmount.Text = "PDFTotalAmount".Translate();

            UILabel lblTotalAmountValue = new UILabel(new CGRect(INNER_PADDING, lblTotalAmount.Frame.GetMaxY(), _footerView.Frame.Width - LBL_WIDTH_PADDING, 26));
            lblTotalAmountValue.TextAlignment = UITextAlignment.Left;
            lblTotalAmountValue.Font = myTNBFont.MuseoSans24_500();
            lblTotalAmountValue.TextColor = myTNBColor.TunaGrey();
            lblTotalAmountValue.Text = _receipt?.d?.data?.payAmt;

            UIView viewLine5 = new UIView(new CGRect(INNER_PADDING, lblTotalAmountValue.Frame.GetMaxY() + (INNER_PADDING + 10), _footerView.Frame.Width - LBL_WIDTH_PADDING, 1));
            viewLine5.BackgroundColor = myTNBColor.PlatinumGrey();

            UILabel lblNote = new UILabel(new CGRect(INNER_PADDING, viewLine5.Frame.GetMaxY() + INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 26));
            lblNote.TextAlignment = UITextAlignment.Left;
            lblNote.Font = myTNBFont.MuseoSans10_500();
            lblNote.TextColor = myTNBColor.SilverChalice();
            lblNote.Text = "PDFNote".Translate();
            lblNote.Lines = 0;
            lblNote.LineBreakMode = UILineBreakMode.WordWrap;

            _footerView.AddSubviews(new UIView[] { lblTrxDate, lblTrxDateValue, viewLine2, lblTrxID, lblTrxIDValue, viewLine3, lblTrxMethod, lblTrxMethodValue, viewLine4, lblTotalAmount, lblTotalAmountValue, viewLine5, lblNote });

            tableViewReceipt.TableHeaderView = _headerView;
            tableViewReceipt.TableFooterView = _footerView;

            tableViewReceipt.Source = new ReceiptTableViewDataSource(_receipt);
            tableViewReceipt.ReloadData();
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
            string pdfFileName = string.Format("Receipt-{0}.pdf", _receipt?.d?.data?.referenceNum);
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _pdfFilePath = Path.Combine(documentsPath, pdfFileName);
            if (File.Exists(_pdfFilePath))
            {
                File.Delete(_pdfFilePath);
            }
            FileStream fs = new FileStream(_pdfFilePath, FileMode.Create);
            Document document = new Document(PageSize.A4, 40, 40, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            var blueColour = new iTextSharp.text.Color(28.0f / 255.0f, 121.0f / 255.0f, 202.0f / 255.0f, 1.0f);
            var tunaGreyColour = new iTextSharp.text.Color(73.0f / 255.0f, 73.0f / 255.0f, 74.0f / 255.0f, 1.0f);
            var silverChaliceColour = new iTextSharp.text.Color(0.65f, 0.65f, 0.65f, 1.0f);
            Font titleFont = new Font(FontFactory.GetFont(myTNBFont.FONTNAME_500, 30f, blueColour));
            Font detailsFont = new Font(FontFactory.GetFont(myTNBFont.FONTNAME_500, 24f, tunaGreyColour));
            Font labelFont = new Font(FontFactory.GetFont(myTNBFont.FONTNAME_500, 18f, silverChaliceColour));
            Font totalAmounFont = new Font(FontFactory.GetFont(myTNBFont.FONTNAME_500, 48f, tunaGreyColour));

            string filepath = Environment.CurrentDirectory;
            var headerImage = iTextSharp.text.Image.GetInstance(filepath + "/tnbReceiptLogoHeader.jpg");

            document.Open();
            PdfContentByte cb = writer.DirectContent;

            PdfPTable grayLine = new PdfPTable(1);
            grayLine.TotalWidth = document.PageSize.Width - 40;
            WriteGrayContent(grayLine);

            headerImage.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
            headerImage.SetAbsolutePosition(0, document.PageSize.Height - headerImage.Height + 82);

            document.Add(headerImage);
            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(new Paragraph(Environment.NewLine, detailsFont));
            document.Add(new Paragraph("PDFTitle".Translate(), titleFont));
            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(new Paragraph("PDFSalutation".Translate(), detailsFont));
            document.Add(new Paragraph(Environment.NewLine, detailsFont));
            document.Add(new Paragraph("PDFMessagePartOne".Translate(), detailsFont));
            document.Add(new Paragraph(string.Format("PDFMessagePartTwo".Translate(), paymentMethod), detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));
#if true
            document.Add(new Paragraph("PDFRefNumber".Translate(), labelFont));
            document.Add(new Paragraph(_receipt?.d?.data?.referenceNum, detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            foreach (var item in _receipt?.d?.data?.accMultiPay)
            {
                document.Add(new Paragraph("PDFAcctNumber".Translate(), labelFont));
                document.Add(new Paragraph(item.accountNum, detailsFont));
                document.Add(new Paragraph(Environment.NewLine, labelFont));
                document.Add(new Paragraph("PDFAcctName".Translate(), labelFont));
                document.Add(new Paragraph(!string.IsNullOrEmpty(item.AccountOwnerName) ? item.AccountOwnerName : Environment.NewLine, detailsFont));
                document.Add(new Paragraph(Environment.NewLine, labelFont));
                document.Add(new Paragraph("PDFAmnt".Translate(), labelFont));
                document.Add(new Paragraph(item.itmAmt, detailsFont));

                document.Add(new Paragraph(Environment.NewLine, titleFont));
                document.Add(grayLine);
                document.Add(new Paragraph(Environment.NewLine, labelFont));
            }

            document.Add(new Paragraph("PDFTrnxDate".Translate(), labelFont));
            document.Add(new Paragraph(_receipt?.d?.data?.payTransDate, detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph("PDFTrnxId".Translate(), labelFont));
            document.Add(new Paragraph(_receipt?.d?.data?.payTransID, detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph("PDFTrnxMethod".Translate(), labelFont));
            document.Add(new Paragraph(_receipt?.d?.data?.payMethod, detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph("PDFTotalAmount".Translate(), detailsFont));
            document.Add(new Paragraph(_receipt?.d?.data?.payAmt, totalAmounFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph("PDFNote".Translate(), labelFont));
#else
            WriteContent(writer, baseFont, document);
#endif

            document.Close();
            writer.Close();
            fs.Close();
        }

        /// <summary>
        /// Generates the gray line.
        /// </summary>
        /// <param name="tableLayout">Table layout.</param>
        private void WriteGrayContent(PdfPTable tableLayout)
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

        /// <summary>
        /// Writes the content table top.
        /// </summary>
        /// <param name="tableLayout">Table layout.</param>
        /// <param name="pdfFont">Pdf font.</param>
        private void WriteContentTableTop(PdfPTable tableLayout, Font pdfFont)
        {
            float[] widths = { 39, 5, 56 };

            tableLayout.SetWidths(widths);
            tableLayout.WidthPercentage = 100;

            AddCellToBody(tableLayout, "Reference Number", pdfFont);
            AddCellToBody(tableLayout, ":", pdfFont);
            AddCellToBody(tableLayout, _receipt.d.data.referenceNum, pdfFont);

            AddCellToBody(tableLayout, "Transaction Date", pdfFont);
            AddCellToBody(tableLayout, ":", pdfFont);
            AddCellToBody(tableLayout, _receipt.d.data.payTransDate, pdfFont);

            AddCellToBody(tableLayout, "Amount (RM)", pdfFont);
            AddCellToBody(tableLayout, ":", pdfFont);
            AddCellToBody(tableLayout, _receipt.d.data.payAmt, pdfFont);

        }

        /// <summary>
        /// Writes the content table account list.
        /// </summary>
        /// <param name="tableLayout">Table layout.</param>
        /// <param name="pdfFont">Pdf font.</param>
        private void WriteContentTableAccountList(PdfPTable tableLayout, Font pdfFont)
        {
            float[] widths = { 39, 5, 56 };

            tableLayout.SetWidths(widths);
            tableLayout.WidthPercentage = 100;

            foreach (var item in _receipt.d.data.accMultiPay)
            {
                AddCellToBody(tableLayout, "Account Number", pdfFont);
                AddCellToBody(tableLayout, ":", pdfFont);
                AddCellToBody(tableLayout, item.accountNum, pdfFont);

                AddCellToBody(tableLayout, "Name", pdfFont);
                AddCellToBody(tableLayout, ":", pdfFont);
                AddCellToBody(tableLayout, !string.IsNullOrEmpty(item.AccountOwnerName) ? item.AccountOwnerName : Environment.NewLine, pdfFont);

                AddCellToBody(tableLayout, "Amount (RM)", pdfFont);
                AddCellToBody(tableLayout, ":", pdfFont);
                AddCellToBody(tableLayout, item.itmAmt, pdfFont);

                AddCellToBody(tableLayout, Environment.NewLine, pdfFont);
                AddCellToBody(tableLayout, Environment.NewLine, pdfFont);
                AddCellToBody(tableLayout, Environment.NewLine, pdfFont);
            }
        }

        /// <summary>
        /// Writes the content table bottom.
        /// </summary>
        /// <param name="tableLayout">Table layout.</param>
        /// <param name="pdfFont">Pdf font.</param>
        private void WriteContentTableBottom(PdfPTable tableLayout, Font pdfFont)
        {
            float[] widths = { 39, 5, 56 };

            tableLayout.SetWidths(widths);
            tableLayout.WidthPercentage = 100;

            AddCellToBody(tableLayout, "Transaction Method", pdfFont);
            AddCellToBody(tableLayout, ":", pdfFont);
            AddCellToBody(tableLayout, _receipt.d.data.payMethod, pdfFont);

            AddCellToBody(tableLayout, "Transaction ID", pdfFont);
            AddCellToBody(tableLayout, ":", pdfFont);
            AddCellToBody(tableLayout, _receipt.d.data.payTransID, pdfFont);
        }

        /// <summary>
        /// Adds the cell to body.
        /// </summary>
        /// <param name="tableLayout">Table layout.</param>
        /// <param name="cellText">Cell text.</param>
        /// <param name="pdfFont">Pdf font.</param>
        private static void AddCellToBody(PdfPTable tableLayout, string cellText, Font pdfFont)
        {
            PdfPCell cell = new PdfPCell(new Phrase(cellText, pdfFont))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                PaddingLeft = 0,
                PaddingTop = 5,
                PaddingRight = 5,
                PaddingBottom = 5,
                BackgroundColor = iTextSharp.text.Color.WHITE,
                Border = 0
            };
            tableLayout.AddCell(cell);

        }

        /// <summary>
        /// Writes the content by fixed location. Legacy.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="baseFont">Base font.</param>
        /// <param name="document">Document.</param>
        private void WriteContent(PdfWriter writer, BaseFont baseFont, Document document)
        {
            PdfContentByte cb = writer.DirectContent;
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