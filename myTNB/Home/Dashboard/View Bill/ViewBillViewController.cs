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
        string _titleSuffix = "Bill";
        public int selectedIndex = -1;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationItem.HidesBackButton = true;
            GetFilePath();
            _titleSuffix = DataManager.DataManager.SharedInstance.SelectedAccount.accountCategoryId.Equals("2")
                                      ? "Advice" : "Bill";
            SetNavigationItems();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ExecuteGetBillHistoryCall();
                    }
                    else
                    {
                        Console.WriteLine("No Network");
                        var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
                    }
                });
            });

        }

        internal void SetNavigationItems()
        {
            NavigationItem.Title = _titleSuffix;
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle("Back-White"), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;

            UIBarButtonItem btnDownload = new UIBarButtonItem(UIImage.FromBundle("IC-Header-Share"), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                if (!string.IsNullOrEmpty(_url))
                {
                    if (File.Exists(_pdfFilePath))
                    {
                        var viewer = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(_pdfFilePath));
                        viewer.PresentOpenInMenu(new RectangleF(0, -260, 320, 320), this.View, true);
                    }
                    else
                    {
                        ActivityIndicator.Show();
                        var webClient = new WebClient();
                        webClient.DownloadDataCompleted += (s, args) =>
                        {
                            var data = args.Result;
                            File.WriteAllBytes(_pdfFilePath, data);
                            InvokeOnMainThread(() =>
                            {
                                ActivityIndicator.Hide();
                                var viewer = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(_pdfFilePath));
                                viewer.PresentOpenInMenu(new RectangleF(0, -260, 320, 320), this.View, true);
                            });
                        };
                        webClient.DownloadDataAsync(new Uri(_url));
                    }
                }
            });
            NavigationItem.RightBarButtonItem = btnDownload;
        }

        internal void SetSubviews()
        {
            _webViewBill = new UIWebView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
            //_webViewBill = new UIWebView(new CGRect(0, 64, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height));
            _webViewBill.Delegate = new WebViewDelegate(View);
            if(!string.IsNullOrEmpty(_url)){
                string nsURL = _url;
                if (File.Exists(_pdfFilePath))
                {
                    nsURL = _pdfFilePath;
                }
                _webViewBill.LoadRequest(new NSUrlRequest(new NSUrl(nsURL)));
                _webViewBill.ScrollView.Delegate = new ScrollViewDelegate(_webViewBill.ScrollView);
                _webViewBill.ScalesPageToFit = true;
            }
            View.AddSubview(_webViewBill);
        }

        internal void GetUrlString()
        {
            ServiceManager serviceManager = new ServiceManager();
            if (_billHistory != null && _billHistory.d != null && _billHistory.d.data != null)
            {
                Dictionary<string, string> requestParams = new Dictionary<string, string>(){
                {"apiKeyID", TNBGlobal.API_KEY_ID},
                {"accNum", DataManager.DataManager.SharedInstance.BillingAccountDetails.accNum},
                {"billingNo", selectedIndex > -1
                    ? _billHistory.d.data[selectedIndex].BillingNo
                    : _billHistory.d.data[0].BillingNo}
            };
                _url = serviceManager.GetPDFServiceURL(selectedIndex > -1 ? "GetBillPDFByBillNo" : "GetBillPDF", requestParams);
            }
            Console.WriteLine(_url);
        }

        internal void GetFilePath()
        {
            string pdfFileName = DataManager.DataManager.SharedInstance.BillingAccountDetails.accNum + DateHelper.GetFormattedDate(DataManager.DataManager.SharedInstance.BillingAccountDetails.dateBill
                , "MMMyyyy") + ".pdf";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _pdfFilePath = Path.Combine(documentsPath, pdfFileName);
        }

        internal void ExecuteGetBillHistoryCall()         {             GetBillHistory().ContinueWith(task =>             {                 InvokeOnMainThread(() =>
                {                     GetUrlString();                     SetSubviews();                     SetNavigationTitle();                 });             });         } 
        internal Task GetBillHistory()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum,
                    isOwner = DataManager.DataManager.SharedInstance.SelectedAccount.isOwned,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email
                };
                _billHistory = serviceManager.GetBillHistory("GetBillHistory", requestParameter);
            });
        }

        internal void SetNavigationTitle()
        {
            string billDate = string.Empty;
            string formattedDate = string.Empty;
            if (_billHistory != null && _billHistory.d != null && _billHistory.d.data != null)
            {
                billDate = selectedIndex > -1
                ? _billHistory.d.data[selectedIndex].DtBill
                : _billHistory.d.data[0].DtBill;
                formattedDate = DateHelper.GetFormattedDate(billDate, "MMM yyyy");
            }
            NavigationItem.Title = string.Format("{0} {1}", formattedDate, _titleSuffix);
        }
    }
}