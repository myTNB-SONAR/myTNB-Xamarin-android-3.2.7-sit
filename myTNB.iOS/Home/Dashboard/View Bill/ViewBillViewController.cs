using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.Model;
using System.Drawing;
using System.Diagnostics;
using myTNB.Home.Dashboard.ViewBill;
using WebKit;

namespace myTNB
{
    public partial class ViewBillViewController : CustomUIViewController
    {
        public ViewBillViewController(IntPtr handle) : base(handle) { }

        private WKWebView _webViewBill;
        private BillHistoryResponseModel _billHistory = new BillHistoryResponseModel();
        private string _url, _pdfFilePath, _titleSuffix, _formattedDate;

        public bool IsFromUsage { set; private get; }
        public bool IsFromHome { set; private get; }
        public bool IsFromBillSelection { set; private get; }
        public bool IsFromHomeForSingleAcct { set; private get; }
        public CustomerAccountRecordModel SelectedAccount = new CustomerAccountRecordModel();
        public string BillingNumber { set; private get; } = string.Empty;
        public Action OnDone;

        public override void ViewDidLoad()
        {
            PageName = ViewBillConstants.Pagename_ViewBill;
            base.ViewDidLoad();
            NavigationItem.HidesBackButton = true;

            if (DataManager.DataManager.SharedInstance != null
                && DataManager.DataManager.SharedInstance.SelectedAccount != null)
            {
                _titleSuffix = GetI18NValue(DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount
                    ? ViewBillConstants.I18N_TitleAdvice : ViewBillConstants.I18N_TitleBill);
            }

            if (IsFromBillSelection)
            {
                _titleSuffix = GetI18NValue(SelectedAccount.accountCategoryId.Equals("2")
                    ? ViewBillConstants.I18N_TitleAdvice : ViewBillConstants.I18N_TitleBill);
            }
            else
            {
                _titleSuffix = GetI18NValue(DataManager.DataManager.SharedInstance.SelectedAccount.accountCategoryId.Equals("2")
                    ? ViewBillConstants.I18N_TitleAdvice : ViewBillConstants.I18N_TitleBill);
            }

            SetNavigationItems();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        await ExecuteGetBillHistoryCall();
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });

        }

        private void SetNavigationItems()
        {
            NavigationItem.Title = _titleSuffix;
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(Constants.IMG_Back), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                OnDone?.Invoke();

                if (IsFromHome && !IsFromHomeForSingleAcct)
                {
                    NavigationController?.PopViewController(true);
                }
                else
                {
                    DismissViewController(true, null);
                }
            });
            NavigationItem.LeftBarButtonItem = btnBack;

            UIBarButtonItem btnDownload = new UIBarButtonItem(UIImage.FromBundle("IC-Header-Share"), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                if (!string.IsNullOrEmpty(_pdfFilePath))
                {
                    if (File.Exists(_pdfFilePath))
                    {
                        UIDocumentInteractionController viewer = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(_pdfFilePath));
                        UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
                        viewer.PresentOpenInMenu(new RectangleF(0, -260, 320, 320), this.View, true);
                    }
                    else
                    {
                        try
                        {
                            ActivityIndicator.Show();
                            WebClient webClient = new WebClient();
                            webClient.DownloadDataCompleted += (s, args) =>
                            {
                                if (args != null)
                                {
                                    byte[] data = args.Result;
                                    if (data != null)
                                    {
                                        File.WriteAllBytes(_pdfFilePath, data);
                                        InvokeOnMainThread(() =>
                                        {
                                            ActivityIndicator.Hide();
                                            UIDocumentInteractionController viewer = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(_pdfFilePath));
                                            UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
                                            viewer?.PresentOpenInMenu(new RectangleF(0, -260, 320, 320), this.View, true);
                                        });
                                    }
                                }
                            };
                            if (!string.IsNullOrEmpty(_url))
                            {
                                bool result = Uri.TryCreate(_url, UriKind.Absolute, out Uri uriResult)
                                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                if (result)
                                {
                                    webClient.DownloadDataAsync(new Uri(_url));
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            Debug.WriteLine("Error: " + err.Message);
                        }
                    }
                }
            });
            NavigationItem.RightBarButtonItem = btnDownload;
        }

        internal void SetSubviews()
        {
            _webViewBill = new WKWebView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height), new WKWebViewConfiguration())
            {
                NavigationDelegate = new WebWKNavigationDelegate(View)
            };
            if (!string.IsNullOrEmpty(_url))
            {
                _webViewBill.LoadRequest(new NSUrlRequest(new NSUrl(_url)));
                _webViewBill.ScrollView.Delegate = new ScrollViewDelegate(_webViewBill.ScrollView);
                if(UIDevice.CurrentDevice.CheckSystemVersion(13,0))// check iOS version
                {
                    _webViewBill.ScalesLargeContentImage = true;
                }
            }
            View.AddSubview(_webViewBill);
        }

        internal Task GetUrlString()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                Dictionary<string, string> requestParams = new Dictionary<string, string>{
                        {"apiKeyID", TNBGlobal.API_KEY_ID},
                        {"lang", TNBGlobal.APP_LANGUAGE}
                    };
                if (IsFromUsage || IsFromHome)
                {
                    requestParams.Add("contractAccount", IsFromBillSelection ? SelectedAccount.accNum : DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                    _url = serviceManager.GetPDFServiceURL("GetBillPDF", requestParams);
                }
                else
                {
                    requestParams.Add("billingNo", BillingNumber);
                    _url = serviceManager.GetPDFServiceURL("GetBillPDFByBillNo", requestParams);
                }
            });
        }

        internal void GetFilePath()
        {
            string billingNo;
            if (IsFromHome || IsFromUsage)
            {
                billingNo = _billHistory.d.data[0].BillingNo;
            }
            else
            {
                billingNo = BillingNumber;
            }
            string pdfFileName = string.Format("{0}_{1}{2}.pdf", IsFromBillSelection ? SelectedAccount.accNum : DataManager.DataManager.SharedInstance.SelectedAccount.accNum, billingNo, _formattedDate);
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _pdfFilePath = Path.Combine(documentsPath, pdfFileName);
        }

        private async Task ExecuteGetBillHistoryCall()
        {
            ActivityIndicator.Show();
            await GetBillHistory().ContinueWith(task =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (_billHistory != null &&
                        _billHistory.d != null &&
                        _billHistory.d.IsSuccess &&
                        _billHistory.d.data != null &&
                        _billHistory.d.data?.Count > 0)
                    {
                        SetNavigationTitle();
                        GetFilePath();
                        await GetUrlString().ContinueWith(getURLTask =>
                        {
                            InvokeOnMainThread(SetSubviews);
                        });
                    }
                    else
                    {
                        string title = _billHistory != null && _billHistory.d != null
                            && _billHistory.d.DisplayTitle.IsValid()
                                ? _billHistory.d.DisplayTitle
                                : GetErrorI18NValue(Constants.Error_DefaultErrorTitle);
                        string errMsg = _billHistory != null && _billHistory.d != null
                            && _billHistory.d.DisplayMessage.IsValid()
                                ? _billHistory.d.DisplayMessage
                                : GetErrorI18NValue(Constants.Error_DefaultServiceErrorMessage);
                        DisplayGenericAlert(title, errMsg, (obj) =>
                        {
                            if (IsFromHome && !IsFromHomeForSingleAcct)
                            {
                                NavigationController.PopViewController(true);
                            }
                            else
                            {
                                DismissViewController(true, null);
                            }
                        });
                    }
                });
            });
            ActivityIndicator.Hide();
        }

        private Task GetBillHistory()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                string emailAddress = string.Empty;
                if (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
                {
                    emailAddress = DataManager.DataManager.SharedInstance.UserEntity[0]?.email ?? string.Empty;
                }
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    contractAccount = IsFromBillSelection ? SelectedAccount.accNum : DataManager.DataManager.SharedInstance.SelectedAccount.accNum,
                    isOwnedAccount = IsFromBillSelection ? SelectedAccount.isOwned : DataManager.DataManager.SharedInstance.SelectedAccount.isOwned,
                };
                _billHistory = serviceManager.OnExecuteAPIV6<BillHistoryResponseModel>(ViewBillConstants.Service_GetBillHistory, requestParameter);
            });
        }

        private void SetNavigationTitle()
        {
            string formattedDate = string.Empty;
            if (_billHistory != null &&
                _billHistory.d != null &&
                _billHistory.d.IsSuccess &&
                _billHistory.d.data != null &&
                _billHistory.d.data.Count > 0)
            {
                string billDate = string.Empty;
                if (IsFromHome || IsFromUsage)
                {
                    billDate = _billHistory.d.data[0].DtBill;
                }
                else
                {
                    if (!string.IsNullOrEmpty(BillingNumber) && !string.IsNullOrWhiteSpace(BillingNumber))
                    {
                        int indx = GetSelectedIndex(BillingNumber);
                        if (indx > -1 && indx < _billHistory.d.data.Count)
                        {
                            billDate = _billHistory.d?.data[indx].DtBill;
                        }
                    }
                }
                formattedDate = DateHelper.GetFormattedDate(billDate, "MMM yyyy");
                _formattedDate = DateHelper.GetFormattedDate(billDate, "MMMyyyy");
            }
            NavigationItem.Title = string.Format("{0} {1}", formattedDate, _titleSuffix);
        }

        private int GetSelectedIndex(string billNo)
        {
            return _billHistory.d.data.FindIndex(x => x.BillingNo.Equals(billNo));
        }
    }
}