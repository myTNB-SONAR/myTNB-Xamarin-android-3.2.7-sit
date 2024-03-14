using Android.App;
using Android.Content;
using Android.Text;
using myTNB.SitecoreCMS.Services;
using myTNB.Android.Src.Base;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.MyTNBService.ServiceImpl;
using myTNB.Android.Src.SelectNotification.Models;
using myTNB.Android.Src.SiteCore;
using myTNB.Android.Src.SitecoreCMS.Model;
using myTNB.Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.Android.Src.NotificationSettings.MVP
{
    public class NotificationSettingsPresenter : NotificationSettingsContract.IUserActionsListener
    {

        private NotificationSettingsContract.IView mView;
        private static int FloatingButtonDefaultTimeOutMillisecond = 4000;
        private int FloatingButtonTimeOutMillisecond = FloatingButtonDefaultTimeOutMillisecond;
        private static int FBContentDefaultTimeOutMillisecond = 4000;
        private int FBContentTimeOutMillisecond = FloatingButtonDefaultTimeOutMillisecond;
        private ISharedPreferences mSharedPref;

        public NotificationSettingsPresenter(NotificationSettingsContract.IView mView, ISharedPreferences mSharedPref)
        {
            this.mView = mView;
            this.mSharedPref = mSharedPref;
            this.mView.SetPresenter(this);
        }

        public async void OnChannelItemClick(NotificationChannelUserPreference item, int position)
        {
            if (item.PreferenceMode == "M")
            {
                return;
            }
            bool isOpted = !item.IsOpted;
            try
            {
                var channelApi = await ServiceApiImpl.Instance.SaveUserNotificationChannelPreference(new SaveUserNotificationChannelPreferenceRequest(item.Id, item.MasterId, isOpted.ToString()));

                if (channelApi.IsSuccessResponse())
                {
                    // TODO : SHOW UPDATE ISOPTED ITEM

                    item.IsOpted = isOpted;
                    if (TextUtils.IsEmpty(item.Id))
                    {
                        item.Id = channelApi.GetData().Id;
                        UserNotificationChannelEntity.UpdateIsOpted(channelApi.GetData().Id, item.Code, isOpted);
                    }
                    else
                    {
                        UserNotificationChannelEntity.UpdateIsOpted(item.Code, isOpted);
                    }
                    this.mView.ShowSuccessUpdatedNotificationChannel(item, position);
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null, item, position);
                }
            }
            catch (System.OperationCanceledException e)
            {
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e, item, position);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException, item, position);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e, item, position);
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void OnTypeItemClick(NotificationTypeUserPreference item, int position, string deviceId)
        {
            if (item.PreferenceMode == "M")
            {
                return;
            }
            bool isOpted = !item.IsOpted;
            try
            {
                var typeApi = await ServiceApiImpl.Instance.SaveUserNotificationTypePreference(new SaveUserNotificationTypePreferenceRequest(item.Id, item.MasterId, isOpted.ToString()));


                if (typeApi.IsSuccessResponse())
                {
                    // TODO : SHOW UPDATE ISOPTED ITEM
                    item.IsOpted = isOpted;
                    NotificationTypesEntity masterEntity = NotificationTypesEntity.GetById(item.MasterId);
                    if (masterEntity != null)
                    {
                        if (TextUtils.IsEmpty(item.Id))
                        {
                            item.Id = typeApi.GetData().Id;
                            UserNotificationTypesEntity.UpdateIsOpted(typeApi.GetData().Id, masterEntity.Code, isOpted);
                        }
                        else
                        {
                            UserNotificationTypesEntity.UpdateIsOpted(masterEntity.Code, isOpted);
                        }

                    }

                    this.mView.ShowSuccessUpdatedNotificationType(item, position);
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null, item, position);
                }
            }
            catch (System.OperationCanceledException e)
            {
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e, item, position);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException, item, position);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e, item, position);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnNotification(string deviceId)
        {
            LoadAllUserPrefNotifications(deviceId);
        }

        private async void LoadAllUserPrefNotifications(string deviceId)
        {
            if (mView.IsActive())
            {
                this.mView.HideShowProgressDialog();
            }
            try
            {
                GetUserNotificationTypePreferencesRequest getUserNotificationTypePref = new GetUserNotificationTypePreferencesRequest();
                var notificationTypesApi = await ServiceApiImpl.Instance.UserNotificationTypePreferences(getUserNotificationTypePref);

                if (notificationTypesApi.IsSuccessResponse())
                {
                    var notificationChannelApi = await ServiceApiImpl.Instance.UserNotificationChannelPreferences(new MyTNBService.Request.BaseRequest());

                    if (mView.IsActive())
                    {
                        this.mView.HideShowProgressDialog();
                    }

                    if (notificationChannelApi.IsSuccessResponse())
                    {
                        UserNotificationTypesEntity.RemoveActive();
                        UserNotificationChannelEntity.RemoveActive();

                        foreach (UserNotificationType notificationType in notificationTypesApi.GetData())
                        {
                            int newRecord = UserNotificationTypesEntity.InsertOrReplace(notificationType);
                            Console.WriteLine(string.Format("New Type Created {0}", newRecord));
                        }

                        foreach (UserNotificationChannel notificationChannel in notificationChannelApi.GetData())
                        {
                            int newRecord = UserNotificationChannelEntity.InsertOrReplace(notificationChannel);
                            Console.WriteLine(string.Format("New Channel Created {0}", newRecord));
                        }

                        ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                        List<UserNotificationTypesEntity> typesList = UserNotificationTypesEntity.ListAllActive();
                        List<UserNotificationChannelEntity> channelsList = UserNotificationChannelEntity.ListAllActive();

                        List<NotificationTypeUserPreference> typeUserPrefList = new List<NotificationTypeUserPreference>();
                        List<NotificationChannelUserPreference> channelUserPrefList = new List<NotificationChannelUserPreference>();

                        foreach (UserNotificationTypesEntity type in typesList)
                        {
                            if (type.ShowInPreference)
                            {
                                if (type.MasterId == "1000020")
                                {
                                    if (MyTNBAccountManagement.GetInstance().IsEBUserVerify() && !MyTNBAccountManagement.GetInstance().COMCLandNEM())
                                    {
                                        typeUserPrefList.Add(NotificationTypeUserPreference.Get(type));
                                    }
                                }
                                else if (type.MasterId == "1000028")
                                {
                                    if (MyTNBAccountManagement.GetInstance().IsSDUserVerify())
                                    {
                                        typeUserPrefList.Add(NotificationTypeUserPreference.Get(type));
                                    }
                                }
                                else
                                {
                                    typeUserPrefList.Add(NotificationTypeUserPreference.Get(type));
                                }
                            }
                        }

                        foreach (UserNotificationChannelEntity channel in channelsList)
                        {
                            if (channel.ShowInPreference)
                            {
                                channelUserPrefList.Add(NotificationChannelUserPreference.Get(channel));
                            }
                        }
                        this.mView.ShowNotificationTypesList(typeUserPrefList);
                        this.mView.ShowNotificationChannelList(channelUserPrefList);
                    }
                    else
                    {
                        // SHOW ERROR
                        this.mView.ShowRetryOptionsApiException(null);
                    }
                }
                else
                {
                    // SHOW ERROR
                    this.mView.ShowRetryOptionsApiException(null);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }


        }

        public void OnGetFloatingButtonTimeStamp()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            Stopwatch sw = Stopwatch.StartNew();
            _ = Task.Run(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    FloatingButtonTimeStampResponseModel responseModel = getItemsService.GetFloatingButtonTimestampItem();
                    sw.Stop();
                    try
                    {
                        if (FloatingButtonTimeOutMillisecond > 0)
                        {
                            FloatingButtonTimeOutMillisecond = FloatingButtonTimeOutMillisecond - (int)sw.ElapsedMilliseconds;
                            if (FloatingButtonTimeOutMillisecond <= 0)
                            {
                                FloatingButtonTimeOutMillisecond = 0;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                    if (responseModel.Status.Equals("Success"))
                    {
                        FloatingButtonParentEntity wtManager = new FloatingButtonParentEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.OnFloatingButtonTimeStampRecieved(responseModel.Data[0].Timestamp);
                    }
                    else
                    {
                        mView.OnFloatingButtonTimeStampRecieved(null);
                    }
                }
                catch (Exception e)
                {
                    mView.OnFloatingButtonTimeStampRecieved(null);
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);

            if (FloatingButtonTimeOutMillisecond > 0)
            {
                _ = Task.Delay(FloatingButtonTimeOutMillisecond).ContinueWith(_ =>
                {
                    if (FloatingButtonTimeOutMillisecond > 0)
                    {
                        FloatingButtonTimeOutMillisecond = 0;
                        OnGetFloatingButtonCache();
                    }
                });
            }
        }

        public void OnGetFloatingButtonItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            Stopwatch sw = Stopwatch.StartNew();
            _ = Task.Run(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    FloatingButtonResponseModel responseModel = getItemsService.GetFloatingButtonItem();
                    sw.Stop();
                    try
                    {
                        if (FloatingButtonTimeOutMillisecond > 0)
                        {
                            FloatingButtonTimeOutMillisecond = FloatingButtonTimeOutMillisecond - (int)sw.ElapsedMilliseconds;
                            if (FloatingButtonTimeOutMillisecond <= 0)
                            {
                                FloatingButtonTimeOutMillisecond = 0;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                    if (responseModel.Status.Equals("Success"))
                    {

                        FloatingButtonEntity wtManager = new FloatingButtonEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);

                        OnGetFloatingButtonCache();
                    }
                    else
                    {
                        OnGetFloatingButtonCache();
                    }
                }
                catch (Exception e)
                {
                    OnGetFloatingButtonCache();
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);

            if (FloatingButtonTimeOutMillisecond > 0)
            {
                _ = Task.Delay(FloatingButtonTimeOutMillisecond).ContinueWith(_ =>
                {
                    if (FloatingButtonTimeOutMillisecond > 0)
                    {
                        FloatingButtonTimeOutMillisecond = 0;
                        OnGetFloatingButtonCache();
                    }
                });
            }
        }

        public Task OnGetFloatingButtonCache()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            return Task.Run(() =>
            {
                try
                {
                    FloatingButtonEntity wtManager = new FloatingButtonEntity();
                    List<FloatingButtonEntity> floatingButtonList = wtManager.GetAllItems();
                    if (floatingButtonList.Count > 0)
                    {
                        FloatingButtonModel item = new FloatingButtonModel()
                        {
                            ID = floatingButtonList[0].ID,
                            Image = floatingButtonList[0].Image,
                            ImageB64 = floatingButtonList[0].ImageB64,
                            Title = floatingButtonList[0].Title,
                            Description = floatingButtonList[0].Description,
                            StartDateTime = floatingButtonList[0].StartDateTime,
                            EndDateTime = floatingButtonList[0].EndDateTime,
                            ShowForSeconds = floatingButtonList[0].ShowForSeconds,
                            ImageBitmap = null
                        };
                        UserSessions.SaveLanguageFBFlag(this.mSharedPref);
                        FloatingButtonUtils.SetFloatingButtonBitmap(item);
                    }
                    else
                    {
                        FloatingButtonTimeOutMillisecond = 0;
                        //if (!this.mView.GetFloatingButtonSiteCoreDoneFlag())
                        //{
                        //    this.mView.SetDefaultAppLaunchImage();
                        //}
                    }
                }
                catch (Exception e)
                {
                    FloatingButtonTimeOutMillisecond = 0;
                    //if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                    //{
                    //    this.mView.SetDefaultAppLaunchImage();
                    //}
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);
        }

        public void OnGetFBContentTimeStamp()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            Stopwatch sw = Stopwatch.StartNew();
            _ = Task.Run(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    FloatingButtonMarketingTimeStampResponseModel responseModel = getItemsService.GetFloatingButtonMarketingTimestampItem();
                    sw.Stop();
                    try
                    {
                        if (FBContentTimeOutMillisecond > 0)
                        {
                            FBContentTimeOutMillisecond = FBContentTimeOutMillisecond - (int)sw.ElapsedMilliseconds;
                            if (FBContentTimeOutMillisecond <= 0)
                            {
                                FBContentTimeOutMillisecond = 0;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                    if (responseModel.Status.Equals("Success"))
                    {
                        FloatingButtonMarketingParentEntity wtManager = new FloatingButtonMarketingParentEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);
                        mView.OnFBContentTimeStampRecieved(responseModel.Data[0].Timestamp);
                    }
                    else
                    {
                        mView.OnFBContentTimeStampRecieved(null);
                    }
                }
                catch (Exception e)
                {
                    mView.OnFBContentTimeStampRecieved(null);
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);

            if (FBContentTimeOutMillisecond > 0)
            {
                _ = Task.Delay(FBContentTimeOutMillisecond).ContinueWith(_ =>
                {
                    if (FBContentTimeOutMillisecond > 0)
                    {
                        FBContentTimeOutMillisecond = 0;
                        OnGetFBContentCache();
                    }
                });
            }
        }

        public void OnGetFBContentItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            Stopwatch sw = Stopwatch.StartNew();
            _ = Task.Run(() =>
            {
                try
                {
                    string density = DPUtils.GetDeviceDensity(Application.Context);
                    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, LanguageUtil.GetAppLanguage());
                    FloatingButtonMarketingResponseModel responseModel = getItemsService.GetFloatingButtonMarketingItem();
                    sw.Stop();
                    try
                    {
                        if (FBContentTimeOutMillisecond > 0)
                        {
                            FBContentTimeOutMillisecond = FBContentTimeOutMillisecond - (int)sw.ElapsedMilliseconds;
                            if (FBContentTimeOutMillisecond <= 0)
                            {
                                FBContentTimeOutMillisecond = 0;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                    if (responseModel.Status.Equals("Success"))
                    {

                        FloatingButtonMarketingEntity wtManager = new FloatingButtonMarketingEntity();
                        wtManager.DeleteTable();
                        wtManager.CreateTable();
                        wtManager.InsertListOfItems(responseModel.Data);

                        OnGetFBContentCache();
                    }
                    else
                    {
                        OnGetFBContentCache();
                    }
                }
                catch (Exception e)
                {
                    OnGetFBContentCache();
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);

            if (FBContentTimeOutMillisecond > 0)
            {
                _ = Task.Delay(FBContentTimeOutMillisecond).ContinueWith(_ =>
                {
                    if (FBContentTimeOutMillisecond > 0)
                    {
                        FBContentTimeOutMillisecond = 0;
                        OnGetFBContentCache();
                    }
                });
            }
        }

        public Task OnGetFBContentCache()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            return Task.Run(() =>
            {
                try
                {
                    FloatingButtonMarketingEntity wtManager = new FloatingButtonMarketingEntity();
                    List<FloatingButtonMarketingEntity> contentList = wtManager.GetAllItems();
                    if (contentList.Count > 0)
                    {
                        FloatingButtonMarketingModel item = new FloatingButtonMarketingModel()
                        {
                            ID = contentList[0].ID,
                            Title = contentList[0].Title,
                            ButtonTitle = contentList[0].ButtonTitle,
                            Description = contentList[0].Description,
                            Description_Images = contentList[0].Description_Images,
                            Infographic_FullView_URL = contentList[0].Infographic_FullView_URL,
                            Infographic_FullView_URL_ImageB64 = contentList[0].Infographic_FullView_URL_ImageB64,
                        };
                        // UserSessions.SaveLanguageFBContentFlag(this.mSharedPref);
                        FloatingButtonMarketingUtils.SetFloatingButtonMarketingBitmap(item);
                    }
                    else
                    {
                        FBContentTimeOutMillisecond = 0;
                        //if (!this.mView.GetFloatingButtonSiteCoreDoneFlag())
                        //{
                        //    this.mView.SetDefaultAppLaunchImage();
                        //}
                    }
                }
                catch (Exception e)
                {
                    FBContentTimeOutMillisecond = 0;
                    //if (!this.mView.GetAppLaunchSiteCoreDoneFlag())
                    //{
                    //    this.mView.SetDefaultAppLaunchImage();
                    //}
                    Utility.LoggingNonFatalError(e);
                }
            }, token.Token);
        }

        public void Start()
        {
            // LOAD TYPES / CHANNELS NOTIFICATIONS LIST
            try
            {
                List<CustomerBillingAccount> smartmeterAccounts = CustomerBillingAccount.SMeterBudgetAccountList();        //smart meter ca                                                                                                                           //energy budget

                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                List<UserNotificationChannelEntity> channelsList = UserNotificationChannelEntity.ListAllActive();
                List<UserNotificationTypesEntity> typesList = UserNotificationTypesEntity.ListAllActive();

                List<NotificationChannelUserPreference> channelUserPrefList = new List<NotificationChannelUserPreference>();
                List<NotificationTypeUserPreference> typeUserPrefList = new List<NotificationTypeUserPreference>();
                List<NotificationTypeUserPreference> filtertypeUserPrefList = new List<NotificationTypeUserPreference>();

                foreach (UserNotificationChannelEntity channel in channelsList)
                {
                    if (channel.ShowInPreference)
                    {
                        channelUserPrefList.Add(NotificationChannelUserPreference.Get(channel));
                    }
                }

                foreach (UserNotificationTypesEntity type in typesList)
                {
                    if (type.ShowInPreference)
                    {
                        if (type.MasterId == "1000020")
                        {
                            if (MyTNBAccountManagement.GetInstance().IsEBUserVerify() && !MyTNBAccountManagement.GetInstance().COMCLandNEM())
                            {
                                typeUserPrefList.Add(NotificationTypeUserPreference.Get(type));
                            }
                        }
                        else if (type.MasterId == "1000028")
                        {
                            if (MyTNBAccountManagement.GetInstance().IsSDUserVerify())
                            {
                                typeUserPrefList.Add(NotificationTypeUserPreference.Get(type));
                            }
                        }
                        else
                        {
                            typeUserPrefList.Add(NotificationTypeUserPreference.Get(type));
                        }
                    }
                }


                this.mView.ShowNotificationTypesList(typeUserPrefList);
                this.mView.ShowNotificationChannelList(channelUserPrefList);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
    }
}