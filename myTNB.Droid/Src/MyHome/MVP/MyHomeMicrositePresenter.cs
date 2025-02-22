﻿using System;
using System.IO;
using Android.Content;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Bills.AccountStatement.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.Mobile;
using Android.Util;
using System.Net;
using myTNB.AndroidApp.Src.Utils;
using System.Threading;
using System.Threading.Tasks;
using myTNB.AndroidApp.Src.MyHome.Model;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.MyTNBService.Parser;
using Refit;
using myTNB.Mobile.API.Models.ApplicationStatus;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Activity;
using Android.Provider;
using myTNB;
using myTNB.Mobile.API.Managers.Payment;
using myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails;
using System.Linq;
using System.Diagnostics;
using static Android.Provider.Settings;
using System.Reflection;
using static myTNB.AndroidApp.Src.MyTNBService.Response.PaymentTransactionIdResponse;
using myTNB.AndroidApp.Src.DeviceCache;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Billing.MVP;
using System.Web;

namespace myTNB.AndroidApp.Src.MyHome.MVP
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

            MyHomeUtil.Instance.ClearCache();
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
                string ca = Utility.GetParamValueFromKey(MyHomeConstants.PAYMENT_CA, webURL);
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
                string ca = GetPaymentParamValuesFromKey(MyHomeConstants.PAYMENT_CA, webURL);
                string isOwnerString = GetPaymentParamValuesFromKey(MyHomeConstants.PAYMENT_IS_OWNER, webURL);
                string applicationType = GetPaymentParamValuesFromKey(MyHomeConstants.APPLICATION_PAYMENT_TYPE, webURL);
                string applicationRefNo = GetPaymentParamValuesFromKey(MyHomeConstants.APPLICATION_PAYMENT_REF_NO, webURL);

                if (ca.IsValid()
                    && isOwnerString.IsValid()
                    && applicationType.IsValid()
                    && applicationRefNo.IsValid())
                {
                    AccountData selectedAccount = new AccountData()
                    {
                        AccountNum = ca,
                        AccountNickName = HttpUtility.UrlDecode(accountName),
                        AddStreet = HttpUtility.UrlDecode(accountAddress),
                        IsOwner = bool.Parse(isOwnerString)
                    };

                    MyHomeUtil.Instance.SetIsCOTCOAFlow();
                    MyHomeUtil.Instance.SetApplicationType(applicationType);
                    MyHomeUtil.Instance.SetReferenceNo(applicationRefNo);
                    this.mView.ShowBillPayment(selectedAccount);
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
                string accountNum = Utility.GetParamValueFromKey(MyHomeConstants.PAYMENT_CA, webURL);
                string isOwnerString = Utility.GetParamValueFromKey(MyHomeConstants.PAYMENT_IS_OWNER, webURL);

                this.mView.ShowLatestBill(accountNum, bool.Parse(isOwnerString));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                this.mView.ShowGenericError();
            }
        }

        public void OnReloadMicrosite(MyHomeDetails details)
        {
            if (details != null)
            {
                this.mView.ShowProgressDialog();
                Task.Run(() =>
                {
                    _ = GetAccessToken(details);
                });
            }
            else
            {
                this.mView.HideProgressDialog();
                this.mActivity.Finish();
            }
        }

        public string OnGetMyHomeSignature(string ssoDomain, string originURL, string redirectURL, string cancelURL, string accessToken)
        {
            string ssoURL = string.Empty;
            try
            {
                //STUB
                //redirectURL = "https://https://18.139.216.169/Application/Offerings";
                //redirectURL = "https://stagingmyhome.mytnb.com.my/Application/Offerings";
                //redirectURL = "https://devmyhome.mytnb.com.my/Application/MyHomeChecklist/ConnectToMyHome";
                UserEntity user = UserEntity.GetActive();
                string myTNBAccountName = user?.DisplayName ?? string.Empty;
                string signature = SSOManager.Instance.GetMyHomeSignature(myTNBAccountName
                , accessToken
                , user.DeviceId ?? string.Empty
                , DeviceIdUtils.GetAppVersionName().Replace("v", string.Empty)
                , 16
                , (LanguageUtil.GetAppLanguage() == "MS"
                ? LanguageManager.Language.MS
                : LanguageManager.Language.EN).ToString()
                , TextViewUtils.FontInfo ?? "N"
                , originURL
                , redirectURL
                , user.UserID
                , user.IdentificationNo
                , MobileConstants.OSType.int_Android
                , user.Email
                , string.Empty
                , null
                , user.MobileNo
                , cancelURL);

                ssoURL = string.Format(ssoDomain, signature);
                //STUB
                //ssoURL = string.Format("https://18.139.216.169/Sso?s={0}", signature);
                //ssoURL = string.Format("https://devmyhome.mytnb.com.my/Sso?s={0}", signature);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            Log.Debug("[DEBUG]", "ssoURL: " + ssoURL);
            return ssoURL;
        }

        private async Task GetAccessToken(MyHomeDetails details)
        {
            UserEntity user = UserEntity.GetActive();
            string accessToken = await AccessTokenManager.Instance.GetUserServiceAccessToken(user.UserID);
            AccessTokenCache.Instance.SaveUserServiceAccessToken(this.mActivity, accessToken);
            if (accessToken.IsValid())
            {
                this.mActivity.RunOnUiThread(() =>
                {
                    this.mView.HideProgressDialog();
                    this.mView.ReloadWebview(details, accessToken);
                });
            }
            else
            {
                this.mActivity.RunOnUiThread(() =>
                {
                    this.mView.HideProgressDialog();
                    this.mView.ShowGenericError();
                });
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

