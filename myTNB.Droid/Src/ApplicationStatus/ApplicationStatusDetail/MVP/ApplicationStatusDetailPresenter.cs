using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Com.Airbnb.Lottie.Network;
using myTNB.Mobile;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.MyHome;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP
{
    public class ApplicationStatusDetailPresenter
    {
        ApplicationStatusDetailContract.IView mView;
        private BaseAppCompatActivity mActivity;
        private CancellationTokenSource cts;
        public string _filePath;
        public string _fileExtension;
        public string _fileTitle;

        public ApplicationStatusDetailPresenter(ApplicationStatusDetailContract.IView view, BaseAppCompatActivity activity, ISharedPreferences pref)
        {
            mView = view;
            mActivity = activity;
            cts = new CancellationTokenSource();
        }

        public ApplicationStatusDetailPresenter(ApplicationStatusDetailContract.IView view, BaseAppCompatActivity activity)
        {
            mView = view;
            mActivity = activity;
            cts = new CancellationTokenSource();
        }

        public List<NewAppModel> OnGeneraNewAppTutorialNoActionList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsNoActionTitle"), // "Keep track of your applications.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsNoActionMessage"),//"Your submitted applications will automatically appear here so you can view their status. Use the filter to search through the list easily."
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            return newList;
        }

        public List<NewAppModel> OnGeneraNewAppTutorialActionList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsActionOneTitle"), //"Keep track of your applications.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsActionOneMessage"), //"Your submitted applications will automatically appear so you can view their status. Search and save applications submitted by others.",
                ItemCount = 0,
                DisplayMode = "Extra",
                IsButtonShow = false
            });
            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsActionTwoTitle"), //"Search other application status.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsActionTwoMessage"), // "Search and save applications submitted by others with your preferred reference number.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            return newList;
        }

        public List<NewAppModel> OnGeneraNewAppTutorialInProgressList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsInProgressTitle"), //"Keep track of your applications.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusDetailsInProgressMessage"), //"Your submitted applications will automatically appear so you can view their status. Search and save applications submitted by others.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });
            return newList;
        }

        public async Task DownloadFile(string webURL)
        {
            this.mActivity.RunOnUiThread(() =>
            {
                this.mView.ShowProgressDialog();
            });

            await Task.Run(() =>
            {
                _filePath = GetFilePathOnDownload(webURL);
            }, cts.Token);

            this.mActivity.RunOnUiThread(() =>
            {
                if (_filePath.IsValid() && _fileExtension.IsValid())
                {
                    this.mView.ShareDownloadedFile(_filePath, _fileExtension, _fileTitle);
                }

                this.mView.HideProgressDialog();
            });
        }

        private string GetDecryptedDownloadURL(string encryptedValue)
        {
            string url = string.Empty;
            url = SecurityManager.Instance.AES256_Decrypt(AWSConstants.MyHome_SaltKey, AWSConstants.MyHome_Passphrase, encryptedValue);
            return url;
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

        public void OnGetApplicationDetail(GetApplicationStatusDisplay statusDisplay, UpdateType updateType, string toastMessage = "")
        {
            this.mView.ShowProgressDialog();
            Task.Run(() =>
            {
                _ = GetApplicationDetail(statusDisplay, updateType, toastMessage);
            });
        }

        private async Task GetApplicationDetail(GetApplicationStatusDisplay statusDisplay, UpdateType updateType, string toastMessage = "")
        {
            ApplicationDetailDisplay response = await ApplicationStatusManager.Instance.GetApplicationDetail(statusDisplay.SavedApplicationID
                , statusDisplay.ApplicationDetail.ApplicationId
                , statusDisplay.ApplicationTypeCode
                , UserEntity.GetActive().UserID ?? string.Empty
                , statusDisplay.ApplicationDetail.Email ?? string.Empty
                , statusDisplay.System);

            this.mActivity.RunOnUiThread(() =>
            {
                this.mView.HideProgressDialog();
                if (response.StatusDetail.IsSuccess)
                {
                    Bundle extras = new Bundle();
                    extras.PutString(MyHomeConstants.APPLICATION_DETAIL_RESPONSE, JsonConvert.SerializeObject(response.Content));
                    this.mView.OnScreenLoad(extras);
                }

                this.mView.GetApplicationDetailOnResult(response, updateType, toastMessage);
            });
        }

        public void OnGetAccessToken(int resultCode, string cancelUrl)
        {
            this.mView.ShowProgressDialog();
            Task.Run(() =>
            {
                _ = GetAccessToken(resultCode, cancelUrl);
            });
        }

        private async Task GetAccessToken(int resultCode, string cancelUrl)
        {
            UserEntity user = UserEntity.GetActive();
            string accessToken = await AccessTokenManager.Instance.GetUserServiceAccessToken(user.UserID);
            AccessTokenCache.Instance.SaveUserServiceAccessToken(this.mActivity, accessToken);
            this.mActivity.RunOnUiThread(() =>
            {
                this.mView.HideProgressDialog();
                if (accessToken.IsValid())
                {
                    this.mView.NavigateToMicrosite(accessToken, resultCode, cancelUrl);
                }
                else
                {
                    this.mView.OnShowGenericErrorPopUp();
                }
            });
        }

        public void OnDeleteDraft(string refNo, SupplyOfferingType type, bool isCOTExistingOwner = false)
        {
            this.mView.ShowProgressDialog();
            Task.Run(() =>
            {
                _ = PostDeleteDraft(refNo, type, isCOTExistingOwner);
            });
        }

        private async Task PostDeleteDraft(string refNo, SupplyOfferingType type, bool isCOTExistingOwner = false)
        {
            UserEntity user = UserEntity.GetActive();

            bool isSuccess = false;
            string accessToken = string.Empty;
            string message = string.Empty;

            switch (type)
            {
                case SupplyOfferingType.NC:
                    var responseNC = await myTNB.Mobile.AWS.ApplicationStatusManager.Instance.PostDeleteNCDraft(refNo, user.UserID, AccessTokenCache.Instance.GetUserServiceAccessToken(this.mActivity));
                    if (responseNC != null &&
                        responseNC.StatusDetail != null)
                    {
                        isSuccess = responseNC.StatusDetail.IsSuccess;
                        accessToken = responseNC.StatusDetail.AccessToken;
                        message = responseNC.StatusDetail.Message;
                    }
                    break;
                case SupplyOfferingType.COT:
                    var responseCOT = await myTNB.Mobile.AWS.ApplicationStatusManager.Instance.PostDeleteCOTDraft(refNo, user.UserID, AccessTokenCache.Instance.GetUserServiceAccessToken(this.mActivity));
                    if (responseCOT != null &&
                        responseCOT.StatusDetail != null)
                    {
                        isSuccess = responseCOT.StatusDetail.IsSuccess;
                        accessToken = responseCOT.StatusDetail.AccessToken;
                        string isCOTExistingOwnerMsg = Utility.GetLocalizedCommonLabel(I18NConstants.Cancelled_Application_COT);
                        message = isCOTExistingOwner ? isCOTExistingOwnerMsg : responseCOT.StatusDetail.Message;
                    }
                    break;
                case SupplyOfferingType.COA:
                    var responseCOA = await myTNB.Mobile.AWS.ApplicationStatusManager.Instance.PostDeleteCOADraft(refNo, user.UserID, AccessTokenCache.Instance.GetUserServiceAccessToken(this.mActivity));
                    if (responseCOA != null &&
                        responseCOA.StatusDetail != null)
                    {
                        isSuccess = responseCOA.StatusDetail.IsSuccess;
                        accessToken = responseCOA.StatusDetail.AccessToken;
                        message = responseCOA.StatusDetail.Message;
                    }
                    break;
                default:
                    break;
            }

            if (isSuccess)
            {
                AccessTokenCache.Instance.SaveUserServiceAccessToken(this.mActivity, accessToken);
            }

            this.mView.DeleteDraftOnResult(isSuccess, message);
        }
    }
}