using System;
using System.IO;
using Android.Content;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Bills.AccountStatement.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB.Mobile;
using Android.Util;
using System.Net;
using myTNB_Android.Src.Utils;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.MyHome.Model;
using System.Collections.Generic;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.MyTNBService.Parser;
using Refit;
using myTNB.Mobile.API.Models.ApplicationStatus;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using Android.Provider;
using myTNB;
using myTNB.Mobile.API.Managers.Payment;
using myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails;
using System.Linq;
using System.Diagnostics;
using static Android.Provider.Settings;
using System.Reflection;

namespace myTNB_Android.Src.MyHome.MVP
{
    public class MyHomeMicrositePresenter : MyHomeMicrositeContract.IUserActionsListener
    {
        private readonly MyHomeMicrositeContract.IView mView;
        private BaseAppCompatActivity mActivity;
        private Context mContext;

        private CancellationTokenSource cts;
        public string _filePath;
        public string _fileExtension;
        public string _fileTitle;
        private MyHomePaymentDetailsModel _paymentDetailsModel;
        private ApplicationDetailDisplay _paymentDisplayModel;

        public MyHomeMicrositePresenter(MyHomeMicrositeContract.IView view, BaseAppCompatActivity activity, Context context)
        {
            this.mActivity = activity;
            this.mContext = context;
            this.mView = view;
            this.mView?.SetPresenter(this);
        }

        public void OnInitialize()
        {
            cts = new CancellationTokenSource();
            _filePath = string.Empty;
            _fileExtension = string.Empty;
            _fileTitle = string.Empty;
            _paymentDetailsModel = new MyHomePaymentDetailsModel();

            this.mView?.SetUpViews();
        }

        public void Start() { }

        public string GetFilePath()
        {
            return _filePath;
        }

        private string GetDecryptedDownloadURL(string encryptedValue)
        {
            string url = string.Empty;
            url = SecurityManager.Instance.AES256_Decrypt(AWSConstants.MyHome_SaltKey, AWSConstants.MyHome_Passphrase, encryptedValue);
            return url;
        }

        public async Task ViewFile(string webURL)
        {
            this.mView.ShowProgressDialog();

            await Task.Run(() =>
            {
                _filePath = GetFilePathOnDownload(webURL);
            }, cts.Token);

            if (_filePath.IsValid() && _fileExtension.IsValid())
            {
                this.mView.ViewDownloadedFile(_filePath, _fileExtension, _fileTitle);
            }

            this.mView.HideProgressDialog();
        }

        public async Task DownloadFile(string webURL)
        {
            this.mView.ShowProgressDialog();

            await Task.Run(() =>
            {
                _filePath = GetFilePathOnDownload(webURL);
            }, cts.Token);

            if (_filePath.IsValid() && _fileExtension.IsValid())
            {
                this.mView.ShareDownloadedFile(_filePath, _fileExtension, _fileTitle);
            }

            this.mView.HideProgressDialog();
        }

        public string GetFilePathOnDownload(string webURL)
        {
            string path = string.Empty;
            try
            {
                _fileExtension = MyHomeConstants.EXTENSION_JPG;
                string ext = Utility.GetParamValueFromKey(MyHomeConstants.EXTENSION, webURL);
                if (ext.IsValid())
                {
                    _fileExtension = ext.ToLower();
                }

                string title = Utility.GetParamValueFromKey(MyHomeConstants.TITLE, webURL);
                if (title.IsValid())
                {
                    _fileTitle = title;
                }

                string decryptedURL = string.Empty;
                var parameters = webURL.Split(MyHomeConstants.FILE_KEY);
                if (parameters.Length == 2)
                {
                    decryptedURL = GetDecryptedDownloadURL(parameters[1]);
                }

                if (decryptedURL.IsValid())
                {
                    string fileName = MyHomeConstants.DEFAULT_FILENAME + MyHomeConstants.FULL_STOP + _fileExtension;
                    fileName = title;

                    string rootPath = this.mActivity.FilesDir.AbsolutePath;
                    if (Utils.FileUtils.IsExternalStorageReadable() && Utils.FileUtils.IsExternalStorageWritable())
                    {
                        rootPath = this.mActivity.GetExternalFilesDir(null).AbsolutePath;
                    }

                    var directory = System.IO.Path.Combine(rootPath, MyHomeConstants.MYHOME);
                    if (!System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.CreateDirectory(directory);
                    }

                    path = System.IO.Path.Combine(directory, fileName);

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(decryptedURL, path);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                this.mView.ShowGenericError();
            }
            return path;
        }

        public void GetPaymentDetails(string webURL)
        {
            try
            {
                string ca = Utility.GetParamValueFromKey(MyHomeConstants.PAYMENT_CA.ToLower(), webURL);
                if (ca.IsValid())
                {
                    _paymentDetailsModel.AccountNumber = ca;
                }

                string nickName = Utility.GetParamValueFromKey(MyHomeConstants.PAYMENT_ACCOUNT_NICKNAME, webURL);
                if (nickName.IsValid())
                {
                    _paymentDetailsModel.AccountNickName = nickName;
                }

                string address = Utility.GetParamValueFromKey(MyHomeConstants.PAYMENT_ACCOUNT_PREMISE, webURL);
                if (address.IsValid())
                {
                    _paymentDetailsModel.AccountAddress = address;
                }

                string isOwnedStr = Utility.GetParamValueFromKey(MyHomeConstants.PAYMENT_IS_OWNER, webURL);
                if (isOwnedStr.IsValid())
                {
                    _paymentDetailsModel.IsOwned = bool.Parse(isOwnedStr);
                }

                this.mView?.ShowProgressDialog();

                Task.Run(() =>
                {
                    _ = OnGetAccountCharges();
                });
                }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                this.mView.ShowGenericError();
            }
        }

        public void GetPaymentInfo(string webURL)
        {
            try
            {
                string accountName = GetPaymentParamValuesFromKey(MyHomeConstants.APPLICATION_PAYMENT_ACCOUNT_NAME, webURL);
                string accountAddress = GetPaymentParamValuesFromKey(MyHomeConstants.APPLICATION_PAYMENT_PREMISE, webURL);
                string mobileNo = GetPaymentParamValuesFromKey(MyHomeConstants.APPLICATION_PAYMENT_MOBILE_NO, webURL);
                string applicationType = GetPaymentParamValuesFromKey(MyHomeConstants.APPLICATION_PAYMENT_TYPE, webURL);
                string searchTerm = GetPaymentParamValuesFromKey(MyHomeConstants.APPLICATION_PAYMENT_SEARCH_TERM, webURL);
                string system = GetPaymentParamValuesFromKey(MyHomeConstants.APPLICATION_PAYMENT_SYSTEM, webURL);
                string statusId = GetPaymentParamValuesFromKey(MyHomeConstants.APPLICATION_PAYMENT_STATUS_ID, webURL);
                string statusCode = GetPaymentParamValuesFromKey(MyHomeConstants.APPLICATION_PAYMENT_STATUS_CODE, webURL);
                string srNumber = GetPaymentParamValuesFromKey(MyHomeConstants.APPLICATION_PAYMENT_SR_NUMBER, webURL);

                string applicationPaymentDetailStr = GetPaymentParamValuesFromKey(MyHomeConstants.APPLICATION_PAYMENT_DETAIL, webURL);
                if (mobileNo.IsValid()
                    && applicationType.IsValid()
                    && searchTerm.IsValid()
                    && system.IsValid()
                    && statusId.IsValid()
                    && statusCode.IsValid()
                    && srNumber.IsValid()
                    && applicationPaymentDetailStr.IsValid())
                {
                    ApplicationPaymentDetail paymentDetail = JsonConvert.DeserializeObject<ApplicationPaymentDetail>(applicationPaymentDetailStr);
                    if (paymentDetail != null)
                    {
                        _paymentDisplayModel = new ApplicationDetailDisplay()
                        {
                            Content = new GetApplicationStatusDisplay()
                        };

                        _paymentDisplayModel.Content.ApplicationStatusDetail = new ApplicationStatusDetailDisplayModel();
                        _paymentDisplayModel.Content.ApplicationStatusDetail.IsPayment = true;
                        _paymentDisplayModel.Content.ApplicationDetail = new ApplicationDetailDisplayModel();
                        _paymentDisplayModel.Content.ApplicationDetail.ApplicationId = searchTerm;
                        _paymentDisplayModel.Content.System = system;
                        _paymentDisplayModel.Content.ApplicationTypeCode = applicationType;
                        _paymentDisplayModel.Content.applicationPaymentDetail = paymentDetail;
                        _paymentDisplayModel.Content.PaymentDisplay = new PaymentDisplayModel
                        {
                            outstandingChargesAmount = paymentDetail.outstandingChargesAmount,
                            latestBillAmount = paymentDetail.latestBillAmount,
                            oneTimeChargesAmount = paymentDetail.oneTimeChargesAmount,
                            oneTimeChargesDetail = paymentDetail.oneTimeChargesDetail,
                            totalPayableAmount = paymentDetail.totalPayableAmount,
                            caNo = paymentDetail.caNo,
                            sdDocumentNo = paymentDetail.sdDocumentNo,
                            srNo = paymentDetail.srNo,
                            hasInvoiceAttachment = paymentDetail.hasInvoiceAttachment
                        };

                        this.mView.ShowProgressDialog();

                        Task.Run(() =>
                        {
                            _ = GetApplicationsPaidDetails(srNumber, statusId, statusCode, applicationType);
                        });
                    }
                    else
                    {
                        this.mView.ShowGenericError();
                    }
                }
                else
                {
                    this.mView.ShowGenericError();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                this.mView.ShowGenericError();
            }
        }

        private string GetPaymentParamValuesFromKey(string key, string urlString)
        {
            string value = string.Empty;
            try
            {
                if (!urlString.IsValid())
                {
                    return value;
                }

                var segment = urlString?.Split(Constants.AMPERSAND);
                if (segment.Length > 0)
                {
                    foreach (var pair in segment)
                    {
                        string pattern = string.Format(Constants.PATTERN, key);
                        Regex regex = new Regex(pattern);
                        Match match = regex.Match(pair);
                        if (match.Success)
                        {
                            value = pair.Replace(string.Format(Constants.REPLACE_KEY, key), string.Empty);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return value;
        }

        private async Task GetApplicationsPaidDetails(string srNum, string statusId, string statusCode, string applicationType)
        {
            PostApplicationsPaidDetailsResponse paymentResponse = await PaymentManager.Instance.GetApplicationsPaidDetails(AppInfoManager.Instance.GetPlatformUserInfo()
                , srNum
                , statusId
                , statusCode
                , applicationType);

            ParsePaidDetailsResponse(paymentResponse);
            SetPaymentDisplay();

            this.mActivity.RunOnUiThread(() =>
            {
                this.mView.HideProgressDialog();
                this.mView.ShowApplicationPayment(_paymentDisplayModel.Content);
            });
        }

        private void ParsePaidDetailsResponse(PostApplicationsPaidDetailsResponse response)
        {
            try
            {
                if (response != null && response.D != null && response.D.IsError == "false")
                {
                    if (response.D.Data != null && response.D.Data.Count > 0)
                    {
                        _paymentDisplayModel.Content.ReceiptDisplay = response.D.Data.Select(x => new ReceiptDisplay
                        {
                            SRNumber = x.SRNumber,
                            MerchantTransID = x.MerchantTransID,
                            PaymentDoneDate = x.PaymentDoneDate,
                            Amount = x.Amount,
                            AccNumber = x.AccNumber,
                            IsPaymentSuccess = x.IsPaymentSuccess,
                            AccountPayments = x.AccountPayments
                        }).ToList();
                    }

                    _paymentDisplayModel.Content.IsPaymentAllowed = response.D.AllowApplicationPayment;
                    _paymentDisplayModel.Content.IsPaymentEnabled = !response.D.ApplicationPaymentDisabled;
                    _paymentDisplayModel.Content.IsPaymentAvailable = !response.D.ApplicationPaymentUnavailable;
                    _paymentDisplayModel.Content.IsTNGEnableApplicationStatus = !response.D.IsTngDisableAtApplicationPayment;

                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG][ParsePaidDetailsResponse Payment Receipt]General Exception: " + ex.Message);
#endif
                this.mActivity.RunOnUiThread(() =>
                {
                    this.mView.HideProgressDialog();
                });
            }

            try
            {
                if (_paymentDisplayModel.Content.IsTaxInvoiceDisplayed)
                {
                    if (response != null && response.D != null && response.D.IsError == "false"
                        && response.D.Data != null && response.D.Data.Count > 0)
                    {
                        _paymentDisplayModel.Content.TaxInvoiceDisplay = new TaxInvoiceDisplay();
                        for (int i = 0; i < response.D.Data.Count; i++)
                        {
                            PostApplicationsPaidDetailsDataModel paymentDetail = response.D.Data[i];
                            List<AccountPaymentsModel> accountPayments = paymentDetail.AccountPayments;
                            if (accountPayments != null && accountPayments.Count > 0)
                            {
                                for (int j = 0; j < accountPayments.Count; j++)
                                {
                                    AccountPaymentsModel accountPayment = accountPayments[j];
                                    if (accountPayment.PaymentType == "CONNECTIONCHARGES")
                                    {
                                        _paymentDisplayModel.Content.TaxInvoiceDisplay = new TaxInvoiceDisplay
                                        {
                                            SRNumber = paymentDetail.SRNumber,
                                            Amount = accountPayment.PaymentAmount
                                        };
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG][ParsePaidDetailsResponse Tax Invoice]General Exception: " + ex.Message);
#endif
                this.mActivity.RunOnUiThread(() =>
                {
                    this.mView.HideProgressDialog();
                });
            }
        }

        private void SetPaymentDisplay()
        {
            try
            {
                if (_paymentDisplayModel.Content.applicationPaymentDetail != null)
                {
                    _paymentDisplayModel.Content.PaymentDetailsList = new List<TitleValueModel>();
                    Dictionary<string, List<SelectorModel>> selectors = LanguageManager.Instance.GetSelectorsByPage("ApplicationStatusPaymentDetails");
                    if (selectors != null
                        && selectors.ContainsKey("chargesMapping")
                        && selectors["chargesMapping"] is List<SelectorModel> mappingList
                        && mappingList != null
                        && mappingList.Count > 0)
                    {
                        for (int i = 0; i < mappingList.Count; i++)
                        {
                            SelectorModel item = mappingList[i];
                            if (GetObjectValue(_paymentDisplayModel.Content.applicationPaymentDetail.oneTimeChargesDetail, item.Key) is object value
                                && value != null)
                            {
                                if (Convert.ToDouble(value) is double convertedValue && convertedValue > 0)
                                {
                                    _paymentDisplayModel.Content.PaymentDetailsList.Add(new TitleValueModel
                                    {
                                        Title = item.Description,
                                        Value = convertedValue.ToAmountDisplayString(true)
                                    });
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] SetPaymentDisplay Error: " + e.Message);
                this.mActivity.RunOnUiThread(() =>
                {
                    this.mView.HideProgressDialog();
                });
            }
        }

        private object GetObjectValue(object props
            , string key)
        {
            object value = null;
            try
            {
                Type type = props.GetType();
                if (type == null)
                {
                    return value;
                }
                PropertyInfo property = type.GetProperty(key);
                if (property == null)
                {
                    return value;
                }
                object objectValue = property.GetValue(props, null);
                if (objectValue == null)
                {
                    return value;
                }
                value = objectValue;
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] GetObjectValue App Details Error: " + e.Message);
                this.mActivity.RunOnUiThread(() =>
                {
                    this.mView.HideProgressDialog();
                });
            }
            return value;
        }

        public void GetLatestBill(string webURL)
        {
            try
            {
                string accountNum = Utility.GetParamValueFromKey(MyHomeConstants.PAYMENT_CA.ToLower(), webURL);
                string isOwnerString = Utility.GetParamValueFromKey(MyHomeConstants.PAYMENT_IS_OWNER, webURL);

                this.mView.ShowLatestBill(accountNum, bool.Parse(isOwnerString));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                this.mView.ShowGenericError();
            }
        }

        private void OnGetRegisteredCards()
        {
            Task.Run(() =>
            {
                _ = GetRegisteredCards();
            });
        }

        private async Task OnGetAccountCharges()
        {
            AccountChargesResponse accountChargesResponse = await ServiceApiImpl.Instance.GetAccountsCharges(new AccountsChargesRequest(new List<string>() {
                _paymentDetailsModel.AccountNumber },
                _paymentDetailsModel.IsOwned));

            if (accountChargesResponse.IsSuccessResponse())
            {
                _paymentDetailsModel.AccountChargesResponse = accountChargesResponse;
                Utility.SetIsPayDisableNotFromAppLaunch(!accountChargesResponse.Response.IsPayEnabled);
                MyTNBAppToolTipData.GetInstance().SetBillMandatoryChargesTooltipModelList(BillingResponseParser.GetMandatoryChargesTooltipModelList(accountChargesResponse.GetData().MandatoryChargesPopUpDetails));
                OnGetRegisteredCards();
            }
            else
            {
                this.mActivity.RunOnUiThread(() =>
                {
                    this.mView.HideProgressDialog();
                    this.mView?.ShowGenericError();
                });
            }
        }

        private async Task GetRegisteredCards()
        {
            RegisteredCardsResponse registeredCardsResponse = await ServiceApiImpl.Instance.GetRegisteredCards(new RegisteredCardsRequest(_paymentDetailsModel.IsOwned));
            if (registeredCardsResponse.IsSuccessResponse())
            {
                _paymentDetailsModel.RegisteredCardsResponse = registeredCardsResponse;
                this.mActivity.RunOnUiThread(() =>
                {
                    this.mView?.HideProgressDialog();
                    this.mView?.ShowPaymentDetails(_paymentDetailsModel);
                });
            }
            else
            {
                this.mActivity.RunOnUiThread(() =>
                {
                    this.mView?.HideProgressDialog();
                    this.mView?.ShowGenericError();
                });
            }
        }
    }
}

