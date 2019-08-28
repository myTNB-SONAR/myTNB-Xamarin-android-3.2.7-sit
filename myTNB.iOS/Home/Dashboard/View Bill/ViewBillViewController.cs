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

namespace myTNB
{
    public partial class ViewBillViewController : CustomUIViewController
    {
        public ViewBillViewController(IntPtr handle) : base(handle)
        {
        }
        private UIWebView _webViewBill;
        private BillHistoryResponseModel _billHistory = new BillHistoryResponseModel();
        private string _url = string.Empty;
        private string _pdfFilePath = string.Empty;
        private string _titleSuffix = string.Empty;
        private string _formattedDate = string.Empty;

        public int selectedIndex = -1;
        public bool IsFromUsage { set; private get; }
        public string BillingNumber { set; private get; } = string.Empty;
        public Action OnDone;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationItem.HidesBackButton = true;

            _titleSuffix = DataManager.DataManager.SharedInstance.SelectedAccount.accountCategoryId.Equals("2")
                ? "ViewBill_Advice".Translate() : "ViewBill_Title".Translate();
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
                        AlertHandler.DisplayNoDataAlert(this);
                    }
                });
            });

        }

        internal void SetNavigationItems()
        {
            NavigationItem.Title = _titleSuffix;
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle("Back-White"), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                OnDone?.Invoke();
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;

            UIBarButtonItem btnDownload = new UIBarButtonItem(UIImage.FromBundle("IC-Header-Share"), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                if (!string.IsNullOrEmpty(_pdfFilePath))
                {
                    if (File.Exists(_pdfFilePath))
                    {
                        var viewer = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(_pdfFilePath));
                        UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
                        viewer.PresentOpenInMenu(new RectangleF(0, -260, 320, 320), this.View, true);
                    }
                    else
                    {
                        try
                        {
                            ActivityIndicator.Show();
                            var webClient = new WebClient();
                            webClient.DownloadDataCompleted += (s, args) =>
                            {
                                var data = args?.Result;
                                if (data != null)
                                {
                                    File.WriteAllBytes(_pdfFilePath, data);
                                    InvokeOnMainThread(() =>
                                    {
                                        ActivityIndicator.Hide();
                                        var viewer = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(_pdfFilePath));
                                        UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
                                        viewer?.PresentOpenInMenu(new RectangleF(0, -260, 320, 320), this.View, true);
                                    });
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
            _webViewBill = new UIWebView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height))
            {
                Delegate = new WebViewDelegate(View)
            };
            if (!string.IsNullOrEmpty(_url))
            {
                _webViewBill.LoadRequest(new NSUrlRequest(new NSUrl(_url)));
                _webViewBill.ScrollView.Delegate = new ScrollViewDelegate(_webViewBill.ScrollView);
                _webViewBill.ScalesPageToFit = true;
            }
            View.AddSubview(_webViewBill);
        }

        internal Task GetUrlString()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                if (IsFromUsage)
                {
                    Dictionary<string, string> requestParams = new Dictionary<string, string>(){
                        {"apiKeyID", TNBGlobal.API_KEY_ID},
                        {"accNum", DataManager.DataManager.SharedInstance.SelectedAccount.accNum}
                    };
                    _url = serviceManager.GetPDFServiceURL("GetBillPDF", requestParams);
                }
                else
                {
                    Dictionary<string, string> requestParams = new Dictionary<string, string>(){
                        {"apiKeyID", TNBGlobal.API_KEY_ID},
                        {"billingNo", BillingNumber}
                    };
                    _url = serviceManager.GetPDFServiceURL("GetBillPDFByBillNo", requestParams);
                }
            });
        }

        internal void GetFilePath()
        {
            string billingNo = selectedIndex > -1 && selectedIndex < _billHistory?.d?.data?.Count
                    ? _billHistory?.d?.data[selectedIndex]?.BillingNo
                    : _billHistory?.d?.data[0]?.BillingNo;
            string pdfFileName = string.Format("{0}_{1}{2}.pdf", DataManager.DataManager.SharedInstance.SelectedAccount.accNum, billingNo, _formattedDate);
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
                    if (_billHistory != null && _billHistory?.d != null
                        && _billHistory.d.IsSuccess
                        && _billHistory?.d?.data != null
                        && _billHistory?.d?.data?.Count > 0)
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
                        AlertHandler.DisplayServiceError(this, _billHistory?.d?.ErrorMessage, (obj) =>
                        {
                            DismissViewController(true, null);
                        });
                    }
                });
            });
            ActivityIndicator.Hide();
        }

        internal Task GetBillHistory()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                var emailAddress = string.Empty;
                if (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
                {
                    emailAddress = DataManager.DataManager.SharedInstance.UserEntity[0]?.email ?? string.Empty;
                }
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    contractAccount = DataManager.DataManager.SharedInstance.SelectedAccount.accNum,
                    isOwnedAccount = DataManager.DataManager.SharedInstance.SelectedAccount.isOwned,
                };
                _billHistory = serviceManager.OnExecuteAPIV6<BillHistoryResponseModel>("GetBillHistory", requestParameter);
            });
        }

        internal void SetNavigationTitle()
        {
            string formattedDate = string.Empty;
            if (_billHistory != null && _billHistory?.d != null
                && _billHistory?.d?.data != null && _billHistory?.d?.data?.Count > 0)
            {
                string billDate = selectedIndex > -1 && selectedIndex < _billHistory?.d?.data?.Count
                    ? _billHistory?.d?.data[selectedIndex]?.DtBill
                    : _billHistory?.d?.data[0]?.DtBill;
                formattedDate = DateHelper.GetFormattedDate(billDate, "MMM yyyy");
                _formattedDate = DateHelper.GetFormattedDate(billDate, "MMMyyyy");
            }
            NavigationItem.Title = string.Format("{0} {1}", formattedDate, _titleSuffix);
        }
    }
}