using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using CoreGraphics;
using System.Drawing;
using System.Threading.Tasks;
using myTNB.Home.Bill.Receipt;
using ITFont = iTextSharp.text.Font;

namespace myTNB
{
    public partial class ReceiptViewController : CustomUIViewController
    {
        public ReceiptViewController(IntPtr handle) : base(handle) { }

        public Action OnDone { set; get; }

        enum LabelType
        {
            Title,
            Value
        }

        private const float PADDING = 10f;
        private const float INNER_PADDING = 20f;
        private const float LBL_WIDTH_PADDING = INNER_PADDING * 2;

        private GetPaymentReceiptResponseModel _receipt = new GetPaymentReceiptResponseModel();
        private string _pdfFilePath = string.Empty;

        public bool showAllReceipts;
        public string DetailedInfoNumber = string.Empty;
        public bool isCCFlow = false;
        private string paymentMethod = string.Empty;

        private UIView _headerView, _footerView;

        public override void ViewDidLoad()
        {
            PageName = ReceiptConstants.Pagename_Receipt;
            base.ViewDidLoad();
            SetNavigationBar();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            tableViewReceipt.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        InvokeInBackground(async () =>
                        {
                            await GetPaymentReceipt();
                            InvokeOnMainThread(() =>
                            {
                                if (_receipt != null && _receipt.d != null &&
                                    _receipt.d.IsSuccess && _receipt.d.data != null)
                                {
                                    paymentMethod = _receipt.d.data.payMethod ?? string.Empty;
                                    CreatePDF();
                                    SetSubviews();
                                }
                                else
                                {
                                    DisplayGenericAlert(GetErrorI18NValue(Constants.Error_DefaultErrorTitle), GetI18NValue(ReceiptConstants.I18N_ReceiptErrorMsg), (obj) => { BackButtonAction(); });
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });

        }

        private void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle(GetI18NValue(ReceiptConstants.I18N_Title));
            titleBarComponent.SetPrimaryVisibility(false);
            titleBarComponent.SetPrimaryImage("IC-Header-Share");
            titleBarComponent.SetPrimaryAction(new UITapGestureRecognizer(() =>
            {
                UIDocumentInteractionController viewer = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(_pdfFilePath));
                UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
                viewer.PresentOpenInMenu(new RectangleF(0, -260, 320, 320), this.View, true);
            }));
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                BackButtonAction();
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        private void BackButtonAction()
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
                DismissViewController(true, null);
                OnDone?.Invoke();
            }
        }

        private void SetSubviews()
        {
            View.BackgroundColor = MyTNBColor.SilverChalice;
            tableViewReceipt.Frame = new CGRect(0 + PADDING, DeviceHelper.IsIphoneXUpResolution()
                ? 86 + PADDING : 64 + PADDING, View.Frame.Width - (PADDING * 2)
                , View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 86 + PADDING : 64 + PADDING));
            tableViewReceipt.BackgroundColor = MyTNBColor.SilverChalice;

            _headerView = new UIView(new CGRect(0, 0, tableViewReceipt.Frame.Width, 377))
            {
                BackgroundColor = UIColor.White
            };

            UIImageView imageView = new UIImageView(new CGRect(0, 0, _headerView.Frame.Width, DeviceHelper.GetScaledHeight(80)))
            {
                Image = UIImage.FromBundle("Receipt-Header")
            };
            //imageView.ContentMode = UIViewContentMode.ScaleToFill;

            UILabel paymentTitle = GetLabel(LabelType.Value
                , new CGRect(INNER_PADDING, imageView.Frame.GetMaxY() + INNER_PADDING, _headerView.Frame.Width - LBL_WIDTH_PADDING, 26)
                , GetI18NValue(ReceiptConstants.I18N_Title)
                , MyTNBFont.MuseoSans20_500
                , MyTNBColor.PowerBlue);

            UILabel msgTitle = GetLabel(LabelType.Value
                , new CGRect(INNER_PADDING, paymentTitle.Frame.GetMaxY() + INNER_PADDING, _headerView.Frame.Width - LBL_WIDTH_PADDING, 16)
                , GetI18NValue(ReceiptConstants.I18N_Salutation));

            UILabel msgBody = GetLabel(LabelType.Value
                , new CGRect(INNER_PADDING, msgTitle.Frame.GetMaxY() + INNER_PADDING, _headerView.Frame.Width - LBL_WIDTH_PADDING, 80)
                , string.Format(GetI18NValue(ReceiptConstants.I18N_FullMessage), paymentMethod));

            UIView viewLineTop = new UIView(new CGRect(INNER_PADDING, msgBody.Frame.GetMaxY() + INNER_PADDING, _headerView.Frame.Width - LBL_WIDTH_PADDING, 1));
            viewLineTop.BackgroundColor = MyTNBColor.PlatinumGrey;

            UILabel lblReference = GetLabel(LabelType.Title
                , new CGRect(INNER_PADDING, viewLineTop.Frame.GetMaxY() + INNER_PADDING, _headerView.Frame.Width - LBL_WIDTH_PADDING, 16)
                , GetI18NValue(ReceiptConstants.I18N_ReferenceNumber));

            UILabel lblReferenceValue = GetLabel(LabelType.Value
                , new CGRect(INNER_PADDING, lblReference.Frame.GetMaxY(), _headerView.Frame.Width - LBL_WIDTH_PADDING, 16)
                , _receipt?.d?.data?.referenceNum);

            UIView viewLineBottom = new UIView(new CGRect(INNER_PADDING, lblReferenceValue.Frame.GetMaxY() + INNER_PADDING, _headerView.Frame.Width - LBL_WIDTH_PADDING, 1));
            viewLineBottom.BackgroundColor = MyTNBColor.PlatinumGrey;

            _headerView.AddSubviews(new UIView[] {imageView,paymentTitle,msgTitle,msgBody,viewLineTop
                ,lblReference,lblReferenceValue,viewLineBottom});

            _footerView = new UIView(new CGRect(0, 0, tableViewReceipt.Frame.Width, 400));
            _footerView.BackgroundColor = UIColor.White;

            UILabel lblTrxDate = GetLabel(LabelType.Title
               , new CGRect(INNER_PADDING, INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 16)
               , GetI18NValue(ReceiptConstants.I18N_TrnDate));

            UILabel lblTrxDateValue = GetLabel(LabelType.Value
               , new CGRect(INNER_PADDING, lblTrxDate.Frame.GetMaxY(), _footerView.Frame.Width - LBL_WIDTH_PADDING, 16)
               , _receipt?.d?.data?.payTransDate);

            UIView viewLine2 = new UIView(new CGRect(INNER_PADDING, lblTrxDateValue.Frame.GetMaxY() + INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 1));
            viewLine2.BackgroundColor = MyTNBColor.PlatinumGrey;

            UILabel lblTrxID = GetLabel(LabelType.Title
              , new CGRect(INNER_PADDING, viewLine2.Frame.GetMaxY() + INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 16)
              , GetI18NValue(ReceiptConstants.I18N_TrnID));

            UILabel lblTrxIDValue = GetLabel(LabelType.Value
               , new CGRect(INNER_PADDING, lblTrxID.Frame.GetMaxY(), _footerView.Frame.Width - LBL_WIDTH_PADDING, 16)
               , _receipt?.d?.data?.payTransID);

            UIView viewLine3 = new UIView(new CGRect(INNER_PADDING, lblTrxIDValue.Frame.GetMaxY() + INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 1));
            viewLine3.BackgroundColor = MyTNBColor.PlatinumGrey;

            UILabel lblTrxMethod = GetLabel(LabelType.Title
              , new CGRect(INNER_PADDING, viewLine3.Frame.GetMaxY() + INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 16)
              , GetI18NValue(ReceiptConstants.I18N_TrnMethod));

            UILabel lblTrxMethodValue = GetLabel(LabelType.Value
               , new CGRect(INNER_PADDING, lblTrxMethod.Frame.GetMaxY(), _footerView.Frame.Width - LBL_WIDTH_PADDING, 16)
               , _receipt?.d?.data?.payMethod);

            UIView viewLine4 = new UIView(new CGRect(INNER_PADDING, lblTrxMethodValue.Frame.GetMaxY() + INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 1));
            viewLine4.BackgroundColor = MyTNBColor.PlatinumGrey;

            UILabel lblTotalAmount = GetLabel(LabelType.Value
                , new CGRect(INNER_PADDING, viewLine4.Frame.GetMaxY() + (INNER_PADDING + 10), _footerView.Frame.Width - LBL_WIDTH_PADDING, 16)
                , GetCommonI18NValue(Constants.Common_TotalAmountRM));

            UILabel lblTotalAmountValue = GetLabel(LabelType.Value
                , new CGRect(INNER_PADDING, lblTotalAmount.Frame.GetMaxY(), _footerView.Frame.Width - LBL_WIDTH_PADDING, 26)
                , _receipt?.d?.data?.payAmt
                , MyTNBFont.MuseoSans24_500
                , MyTNBColor.TunaGrey());

            UIView viewLine5 = new UIView(new CGRect(INNER_PADDING, lblTotalAmountValue.Frame.GetMaxY() + (INNER_PADDING + 10), _footerView.Frame.Width - LBL_WIDTH_PADDING, 1));
            viewLine5.BackgroundColor = MyTNBColor.PlatinumGrey;

            UILabel lblNote = GetLabel(LabelType.Title
                , new CGRect(INNER_PADDING, viewLine5.Frame.GetMaxY() + INNER_PADDING, _footerView.Frame.Width - LBL_WIDTH_PADDING, 26)
                , GetI18NValue(ReceiptConstants.I18N_Note));

            _footerView.AddSubviews(new UIView[] {lblTrxDate,lblTrxDateValue,viewLine2,lblTrxID
                ,lblTrxIDValue,viewLine3,lblTrxMethod,lblTrxMethodValue,viewLine4,lblTotalAmount
                ,lblTotalAmountValue,viewLine5,lblNote });

            tableViewReceipt.TableHeaderView = _headerView;
            tableViewReceipt.TableFooterView = _footerView;

            tableViewReceipt.Source = new ReceiptTableViewDataSource(_receipt);
            tableViewReceipt.ReloadData();
        }

        private void CreatePDF()
        {
            string pdfFileName = string.Format(GetI18NValue(ReceiptConstants.I18N_ReceiptFilename), _receipt?.d?.data?.referenceNum);
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _pdfFilePath = Path.Combine(documentsPath, pdfFileName);
            if (File.Exists(_pdfFilePath))
            {
                File.Delete(_pdfFilePath);
            }
            FileStream fs = new FileStream(_pdfFilePath, FileMode.Create);
            Document document = new Document(PageSize.A4, 40, 40, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            iTextSharp.text.Color blueColour = new iTextSharp.text.Color(28.0f / 255.0f, 121.0f / 255.0f, 202.0f / 255.0f, 1.0f);
            iTextSharp.text.Color tunaGreyColour = new iTextSharp.text.Color(73.0f / 255.0f, 73.0f / 255.0f, 74.0f / 255.0f, 1.0f);
            iTextSharp.text.Color silverChaliceColour = new iTextSharp.text.Color(0.65f, 0.65f, 0.65f, 1.0f);
            ITFont titleFont = new ITFont(FontFactory.GetFont(MyTNBFont.FONTNAME_500, 30f, blueColour));
            ITFont detailsFont = new ITFont(FontFactory.GetFont(MyTNBFont.FONTNAME_500, 24f, tunaGreyColour));
            ITFont labelFont = new ITFont(FontFactory.GetFont(MyTNBFont.FONTNAME_500, 18f, silverChaliceColour));
            ITFont totalAmounFont = new ITFont(FontFactory.GetFont(MyTNBFont.FONTNAME_500, 48f, tunaGreyColour));

            string filepath = Environment.CurrentDirectory;
            iTextSharp.text.Image headerImage = iTextSharp.text.Image.GetInstance(filepath + "/tnbReceiptLogoHeader.jpg");

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
            document.Add(new Paragraph(GetI18NValue(ReceiptConstants.I18N_Title), titleFont));
            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(new Paragraph(GetI18NValue(ReceiptConstants.I18N_Salutation), detailsFont));
            document.Add(new Paragraph(Environment.NewLine, detailsFont));
            document.Add(new Paragraph(GetI18NValue(ReceiptConstants.I18N_MessagePartOne), detailsFont));
            document.Add(new Paragraph(string.Format(GetI18NValue(ReceiptConstants.I18N_MessagePartTwo), paymentMethod), detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));
            document.Add(new Paragraph(GetI18NValue(ReceiptConstants.I18N_ReferenceNumber), labelFont));
            document.Add(new Paragraph(_receipt?.d?.data?.referenceNum, detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            foreach (MultiPayDataModel item in _receipt?.d?.data?.accMultiPay)
            {
                document.Add(new Paragraph(GetCommonI18NValue(Constants.Common_AccountNo), labelFont));
                document.Add(new Paragraph(item.accountNum, detailsFont));
                document.Add(new Paragraph(Environment.NewLine, labelFont));
                document.Add(new Paragraph(GetI18NValue(ReceiptConstants.I18N_AccountHolder), labelFont));
                document.Add(new Paragraph(!string.IsNullOrEmpty(item.AccountOwnerName) ? item.AccountOwnerName : Environment.NewLine, detailsFont));
                document.Add(new Paragraph(Environment.NewLine, labelFont));
                document.Add(new Paragraph(GetCommonI18NValue(Constants.Common_AmountRM), labelFont));
                document.Add(new Paragraph(item.itmAmt, detailsFont));

                document.Add(new Paragraph(Environment.NewLine, titleFont));
                document.Add(grayLine);
                document.Add(new Paragraph(Environment.NewLine, labelFont));
            }

            document.Add(new Paragraph(GetI18NValue(ReceiptConstants.I18N_TrnDate), labelFont));
            document.Add(new Paragraph(_receipt?.d?.data?.payTransDate, detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph(GetI18NValue(ReceiptConstants.I18N_TrnID), labelFont));
            document.Add(new Paragraph(_receipt?.d?.data?.payTransID, detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph(GetI18NValue(ReceiptConstants.I18N_TrnMethod), labelFont));
            document.Add(new Paragraph(_receipt?.d?.data?.payMethod, detailsFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph(GetCommonI18NValue(Constants.Common_TotalAmountRM), detailsFont));
            document.Add(new Paragraph(_receipt?.d?.data?.payAmt, totalAmounFont));

            document.Add(new Paragraph(Environment.NewLine, titleFont));
            document.Add(grayLine);
            document.Add(new Paragraph(Environment.NewLine, labelFont));

            document.Add(new Paragraph(GetI18NValue(ReceiptConstants.I18N_Note), labelFont));

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

        private async Task GetPaymentReceipt()
        {
            ServiceManager serviceManager = new ServiceManager();
            object request = new
            {
                serviceManager.usrInf,
                contractAccount = showAllReceipts ? string.Empty : DataManager.DataManager.SharedInstance.SelectedAccount?.accNum ?? string.Empty,
                isOwnedAccount = showAllReceipts ? true : DataManager.DataManager.SharedInstance.SelectedAccount.IsOwnedAccount,
                detailedInfoNumber = DetailedInfoNumber,
                showAllReceipts
            };
            _receipt = serviceManager.OnExecuteAPIV6<GetPaymentReceiptResponseModel>(ReceiptConstants.Service_GetPaymentReceipt, request);
        }

        private UILabel GetLabel(LabelType type, CGRect frame, string text, UIFont font = null, UIColor txtColor = null)
        {
            bool isTitle = type == LabelType.Title;
            return new UILabel
            {
                Frame = frame,
                TextAlignment = UITextAlignment.Left,
                Font = font != null ? font : (isTitle ? MyTNBFont.MuseoSans10_500 : MyTNBFont.MuseoSans14_500),
                TextColor = txtColor != null ? txtColor : (isTitle ? MyTNBColor.SilverChalice : MyTNBColor.TunaGrey()),
                Text = text,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
        }
    }
}