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
                tableViewReceipt.Hidden = false;
                paymentMethod = _receipt?.d?.data?.payMethod;
                CreatePDF();
                SetSubviews();
            }
            else
            {
                tableViewReceipt.Hidden = true;
                AlertHandler.DisplayGenericAlert(this, "Receipt_NoReceipt".Translate(), string.Empty);
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
            titleBarComponent.SetTitle("Receipt_Title".Translate());
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
            View.BackgroundColor = MyTNBColor.SilverChalice;
            tableViewReceipt.Frame = new CGRect(0 + PADDING, DeviceHelper.IsIphoneXUpResolution() ? 86 + PADDING
                : 64 + PADDING, View.Frame.Width - (PADDING * 2), View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 86 + PADDING : 64 + PADDING));
            tableViewReceipt.BackgroundColor = MyTNBColor.SilverChalice;
            tableViewReceipt.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            _headerView = new UIView(new CGRect(0, 0, tableViewReceipt.Frame.Width, 377))
            {
                BackgroundColor = UIColor.White
            };

            UIImageView imageView = new UIImageView(new CGRect(0, 0, _headerView.Frame.Width, DeviceHelper.GetScaledHeight(80)))
            {
                Image = UIImage.FromBundle("Receipt-Header")
            };

            UILabel paymentTitle = new UILabel(new CGRect(INNER_PADDING, imageView.Frame.GetMaxY() + INNER_PADDING
                , _headerView.Frame.Width - LBL_WIDTH_PADDING, 26))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans20_500,
                TextColor = MyTNBColor.PowerBlue,
                Text = "Receipt_Title".Translate()
            };

            UILabel msgTitle = new UILabel(new CGRect(INNER_PADDING, paymentTitle.Frame.GetMaxY() + INNER_PADDING
                , _headerView.Frame.Width - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey(),
                Text = "Receipt_Salutation".Translate()
            };

            UILabel msgBody = new UILabel(new CGRect(INNER_PADDING, msgTitle.Frame.GetMaxY() + INNER_PADDING
                , _headerView.Frame.Width - LBL_WIDTH_PADDING, 80))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey(),
                Text = string.Format("Receipt_MessageFull".Translate(), paymentMethod),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            UIView viewLineTop = GenericLine.GetLine(new CGRect(INNER_PADDING, msgBody.Frame.GetMaxY() + INNER_PADDING
                , _headerView.Frame.Width - LBL_WIDTH_PADDING, 1));

            UILabel lblReference = new UILabel(new CGRect(INNER_PADDING, viewLineTop.Frame.GetMaxY() + INNER_PADDING
                , _headerView.Frame.Width - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans10_500,
                TextColor = MyTNBColor.SilverChalice,
                Text = "Receipt_ReferenceNumber".Translate().ToUpper()
            };

            UILabel lblReferenceValue = new UILabel(new CGRect(INNER_PADDING, lblReference.Frame.GetMaxY()
                , _headerView.Frame.Width - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey(),
                Text = _receipt?.d?.data?.referenceNum
            };

            UIView viewLineBottom = GenericLine.GetLine(new CGRect(INNER_PADDING
                , lblReferenceValue.Frame.GetMaxY() + INNER_PADDING
                , _headerView.Frame.Width - LBL_WIDTH_PADDING, 1));

            _headerView.AddSubviews(new UIView[] { imageView, paymentTitle, msgTitle, msgBody
                , viewLineTop, lblReference, lblReferenceValue, viewLineBottom });

            _footerView = new UIView(new CGRect(0, 0, tableViewReceipt.Frame.Width, 400))
            {
                BackgroundColor = UIColor.White
            };

            UILabel lblTrxDate = new UILabel(new CGRect(INNER_PADDING, INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans10_500,
                TextColor = MyTNBColor.SilverChalice,
                Text = "Receipt_TrnDate".Translate()
            };

            UILabel lblTrxDateValue = new UILabel(new CGRect(INNER_PADDING
                , lblTrxDate.Frame.GetMaxY(), _footerView.Frame.Width - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey(),
                Text = _receipt?.d?.data?.payTransDate
            };

            UIView viewLine2 = GenericLine.GetLine(new CGRect(INNER_PADDING, lblTrxDateValue.Frame.GetMaxY() + INNER_PADDING
                , _footerView.Frame.Width - LBL_WIDTH_PADDING, 1));

            UILabel lblTrxID = new UILabel(new CGRect(INNER_PADDING, viewLine2.Frame.GetMaxY() + INNER_PADDING
                , _footerView.Frame.Width - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans10_500,
                TextColor = MyTNBColor.SilverChalice,
                Text = "Receipt_TrnId".Translate()
            };

            UILabel lblTrxIDValue = new UILabel(new CGRect(INNER_PADDING, lblTrxID.Frame.GetMaxY()
                , _footerView.Frame.Width - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey(),
                Text = _receipt?.d?.data?.payTransID
            };

            UIView viewLine3 = GenericLine.GetLine(new CGRect(INNER_PADDING, lblTrxIDValue.Frame.GetMaxY() + INNER_PADDING
                , _footerView.Frame.Width - LBL_WIDTH_PADDING, 1));

            UILabel lblTrxMethod = new UILabel(new CGRect(INNER_PADDING, viewLine3.Frame.GetMaxY() + INNER_PADDING
                , _footerView.Frame.Width - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans10_500,
                TextColor = MyTNBColor.SilverChalice,
                Text = "Receipt_TrnMethod".Translate()
            };

            UILabel lblTrxMethodValue = new UILabel(new CGRect(INNER_PADDING, lblTrxMethod.Frame.GetMaxY()
                , _footerView.Frame.Width - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey(),
                Text = _receipt?.d?.data?.payMethod
            };

            UIView viewLine4 = GenericLine.GetLine(new CGRect(INNER_PADDING, lblTrxMethodValue.Frame.GetMaxY() + INNER_PADDING
                , _footerView.Frame.Width - LBL_WIDTH_PADDING, 1));

            UILabel lblTotalAmount = new UILabel(new CGRect(INNER_PADDING, viewLine4.Frame.GetMaxY() + (INNER_PADDING + 10)
                , _footerView.Frame.Width - LBL_WIDTH_PADDING, 16))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey(),
                Text = "Common_TotalAmount(RM)".Translate()
            };

            UILabel lblTotalAmountValue = new UILabel(new CGRect(INNER_PADDING, lblTotalAmount.Frame.GetMaxY()
                , _footerView.Frame.Width - LBL_WIDTH_PADDING, 26))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans24_500,
                TextColor = MyTNBColor.TunaGrey(),
                Text = _receipt?.d?.data?.payAmt
            };

            UIView viewLine5 = GenericLine.GetLine(new CGRect(INNER_PADDING, lblTotalAmountValue.Frame.GetMaxY() + (INNER_PADDING + 10)
                , _footerView.Frame.Width - LBL_WIDTH_PADDING, 1));

            UILabel lblNote = new UILabel(new CGRect(INNER_PADDING, viewLine5.Frame.GetMaxY() + INNER_PADDING
                , _footerView.Frame.Width - LBL_WIDTH_PADDING, 26))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans10_500,
                TextColor = MyTNBColor.SilverChalice,
                Text = "Receipt_Note".Translate(),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            _footerView.AddSubviews(new UIView[] { lblTrxDate, lblTrxDateValue, viewLine2
                , lblTrxID, lblTrxIDValue, viewLine3, lblTrxMethod, lblTrxMethodValue
                , viewLine4, lblTotalAmount, lblTotalAmountValue, viewLine5, lblNote });

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
            string pdfFileName = string.Format("Receipt_Filename".Translate(), _receipt?.d?.data?.referenceNum);
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
            Font titleFont = new Font(FontFactory.GetFont(MyTNBFont.FONTNAME_500, 30f, blueColour));
            Font detailsFont = new Font(FontFactory.GetFont(MyTNBFont.FONTNAME_500, 24f, tunaGreyColour));
            Font labelFont = new Font(FontFactory.GetFont(MyTNBFont.FONTNAME_500, 18f, silverChaliceColour));
            Font totalAmounFont = new Font(FontFactory.GetFont(MyTNBFont.FONTNAME_500, 48f, tunaGreyColour));

            string filepath = Environment.CurrentDirectory;
            var headerImage = iTextSharp.text.Image.GetInstance(filepath + "/tnbReceiptLogoHeader.jpg");

            document.Open();
            PdfContentByte cb = writer.DirectContent;

            PdfPTable grayLine = new PdfPTable(1)
            {
                TotalWidth = document.PageSize.Width - 40
            };
            WriteGrayContent(grayLine);

            headerImage.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
            headerImage.SetAbsolutePosition(0, document.PageSize.Height - headerImage.Height + 82);

            document.Add(headerImage);
            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(new Paragraph(Environment.NewLine, detailsFont));
            document.Add(new Paragraph("Receipt_Title".Translate(), titleFont));
            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(new Paragraph("Receipt_Salutation".Translate(), detailsFont));
            document.Add(new Paragraph(Environment.NewLine, detailsFont));
            document.Add(new Paragraph("Receipt_MessagePartOne".Translate(), detailsFont));
            document.Add(new Paragraph(string.Format("Receipt_MessagePartTwo".Translate(), paymentMethod), detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph("Receipt_ReferenceNumber".Translate(), labelFont));
            document.Add(new Paragraph(_receipt?.d?.data?.referenceNum, detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            foreach (var item in _receipt?.d?.data?.accMultiPay)
            {
                document.Add(new Paragraph("Common_AccountNumber".Translate().ToUpper(), labelFont));
                document.Add(new Paragraph(item.accountNum, detailsFont));
                document.Add(new Paragraph(Environment.NewLine, labelFont));
                document.Add(new Paragraph("Receipt_AccountName".Translate().ToUpper(), labelFont));
                document.Add(new Paragraph(!string.IsNullOrEmpty(item.AccountOwnerName) ? item.AccountOwnerName : Environment.NewLine, detailsFont));
                document.Add(new Paragraph(Environment.NewLine, labelFont));
                document.Add(new Paragraph("Common_Amount(RM)".Translate().ToUpper(), labelFont));
                document.Add(new Paragraph(item.itmAmt, detailsFont));

                document.Add(new Paragraph(Environment.NewLine, titleFont));
                document.Add(grayLine);
                document.Add(new Paragraph(Environment.NewLine, labelFont));
            }

            document.Add(new Paragraph("Receipt_TrnDate".Translate(), labelFont));
            document.Add(new Paragraph(_receipt?.d?.data?.payTransDate, detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph("Receipt_TrnId".Translate(), labelFont));
            document.Add(new Paragraph(_receipt?.d?.data?.payTransID, detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph("Receipt_TrnMethod".Translate(), labelFont));
            document.Add(new Paragraph(_receipt?.d?.data?.payMethod, detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph("Common_TotalAmount(RM)".Translate(), detailsFont));
            document.Add(new Paragraph(_receipt?.d?.data?.payAmt, totalAmounFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph("Receipt_Note".Translate(), labelFont));

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