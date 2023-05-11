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

