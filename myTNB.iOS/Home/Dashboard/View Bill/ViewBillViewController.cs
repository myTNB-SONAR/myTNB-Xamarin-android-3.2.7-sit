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
    public partial class ViewBillViewController : UIViewController
    {
        public ViewBillViewController(IntPtr handle) : base(handle)
        {
        }
        UIWebView _webViewBill;
        BillHistoryResponseModel _billHistory = new BillHistoryResponseModel();
        string _url = string.Empty;
        string _pdfFilePath = string.Empty;
        string _titleSuffix = string.Empty;
        string _formattedDate = string.Empty;

        public int selectedIndex = -1;
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
                if (_billHistory != null && _billHistory?.d != null && _billHistory?.d?.data != null && _billHistory?.d?.data?.Count > 0)
                {
                    Dictionary<string, string> requestParams = new Dictionary<string, string>(){
                    {"apiKeyID", TNBGlobal.API_KEY_ID},
                    {"accNum", DataManager.DataManager.SharedInstance.SelectedAccount.accNum},
                    {"billingNo", selectedIndex > -1 && selectedIndex < _billHistory.d.data.Count
                    ? _billHistory.d.data[selectedIndex].BillingNo
                    : _billHistory.d.data[0].BillingNo}
                    };
                    _url = serviceManager.GetPDFServiceURL(selectedIndex > -1 ? "GetBillPDFByBillNo" : "GetBillPDF", requestParams);
                }
            });
        }

        internal void GetFilePath()
        {
            string pdfFileName = DataManager.DataManager.SharedInstance.SelectedAccount.accNum + _formattedDate + ".pdf";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _pdfFilePath = Path.Combine(documentsPath, pdfFileName);
        }

        private async Task ExecuteGetBillHistoryCall()
        {
            await GetBillHistory().ContinueWith(task =>
            {
                InvokeOnMainThread(async () =>
                {
                    SetNavigationTitle();
                    GetFilePath();
                    await GetUrlString().ContinueWith(getURLTask =>
                    {
                        InvokeOnMainThread(SetSubviews);
                    });
                });
            });
        }

        internal Task GetBillHistory()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                var emailAddress = string.Empty;
                if (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
                {
                    emailAddress = DataManager.DataManager.SharedInstance.UserEntity[0]?.email;
                }
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum,
                    isOwner = DataManager.DataManager.SharedInstance.SelectedAccount.isOwned,
                    email = emailAddress
                };
                _billHistory = serviceManager.GetBillHistory("GetBillHistory", requestParameter);
            });
        }

        internal void SetNavigationTitle()
        {
            string billDate = string.Empty;
            string formattedDate = string.Empty;
            if (_billHistory != null && _billHistory?.d != null
                && _billHistory?.d?.data != null && _billHistory?.d?.data?.Count > 0)
            {
                billDate = selectedIndex > -1 && selectedIndex < _billHistory?.d?.data?.Count
                    ? _billHistory?.d?.data[selectedIndex]?.DtBill
                    : _billHistory?.d?.data[0]?.DtBill;
                formattedDate = DateHelper.GetFormattedDate(billDate, "MMM yyyy");
                _formattedDate = DateHelper.GetFormattedDate(billDate, "MMMyyyy");
            }
            NavigationItem.Title = string.Format("{0} {1}", formattedDate, _titleSuffix);
        }
    }
}