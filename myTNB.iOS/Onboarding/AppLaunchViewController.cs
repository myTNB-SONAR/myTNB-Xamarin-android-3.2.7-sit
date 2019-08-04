using Foundation;
using System;
using UIKit;
using CoreAnimation;
using myTNB.Model;
using Cirrious.FluentLayouts.Touch;
using System.Threading.Tasks;
using Newtonsoft.Json;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.SQLite;
using myTNB.DataManager;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using myTNB.Dashboard.DashboardComponents;

namespace myTNB
{
    public partial class AppLaunchViewController : UIViewController
    {
        UIImageView imgViewAppLaunch;
        string _imageSize = string.Empty;
        bool isMaintenance;
        UIView maintenanceView;
        public AppLaunchViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            imgViewAppLaunch = new UIImageView(UIImage.FromBundle("App-Launch-Gradient"));
            var imgViewLogo = new UIImageView(UIImage.FromBundle("New-Launch-Logo"));

            View.AddSubview(imgViewAppLaunch);
            View.AddSubview(imgViewLogo);

            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            var heightMargin = ((float)UIScreen.MainScreen.Bounds.Height / 2) - (DeviceHelper.GetScaledHeight(220) / 2);

            View.AddConstraints(
                imgViewAppLaunch.AtTopOf(View, 0),
                imgViewAppLaunch.AtBottomOf(View, 0),
                imgViewAppLaunch.AtLeftOf(View, 0),
                imgViewAppLaunch.AtRightOf(View, 0),

                imgViewLogo.AtTopOf(View, heightMargin),
                imgViewLogo.WithSameCenterX(View),
                imgViewLogo.Height().EqualTo(DeviceHelper.GetScaledHeight(220)),
                imgViewLogo.Width().EqualTo(DeviceHelper.GetScaledHeight(220))
            );
            //Create DB
            SQLiteHelper.CreateDB();
            CreateCacheTables();

            // clear cache data on App Launch
            BillingAccountEntity.DeleteTable();
            PaymentHistoryEntity.DeleteTable();

            GetUserEntity();

            // clear cached data on Version Update
            ClearCacheForVersionUpdate();
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, HandleAppWillEnterForeground);
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, (float)UIScreen.MainScreen.Bounds.Height, false);
            maintenanceView = gradientViewComponent.GetUI();
            DataManager.DataManager.SharedInstance.CommonI18NDictionary = LanguageManager.Instance.GetCommonValuePairs();
            DataManager.DataManager.SharedInstance.HintI18NDictionary = LanguageManager.Instance.GetHintValuePairs();
            DataManager.DataManager.SharedInstance.ErrorI18NDictionary = LanguageManager.Instance.GetErrorValuePairs();
            DataManager.DataManager.SharedInstance.ImageSize = DeviceHelper.GetImageSize();
        }

        void HandleAppWillEnterForeground(NSNotification notification)
        {
            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseRootVc);

            if (topVc != null)
            {
                if (topVc is AppLaunchViewController)
                {
                    if (isMaintenance)
                    {
                        InvokeOnMainThread(async () =>
                        {
                            if (NetworkUtility.isReachable)
                            {
                                GetUserEntity();
                                await LoadMasterData();
                            }
                            else
                            {
                                Debug.WriteLine("No Network");
                                AlertHandler.DisplayNoDataAlert(this);
                            }
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Clears the cache for version update.
        /// </summary>
        internal void ClearCacheForVersionUpdate()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var appShortVersion = sharedPreference.StringForKey("appShortVersion");
            var appBuildVersion = sharedPreference.StringForKey("appBuildVersion");
            bool clearCache = false;

            if (!string.IsNullOrEmpty(appShortVersion) && !string.IsNullOrEmpty(appBuildVersion))
            {
                if (appShortVersion == AppVersionHelper.GetAppShortVersion())
                {
                    if (appBuildVersion != AppVersionHelper.GetBuildVersion())
                    {
                        clearCache = true;
                        sharedPreference.SetString(AppVersionHelper.GetAppShortVersion(), "appShortVersion");
                        sharedPreference.SetString(AppVersionHelper.GetBuildVersion(), "appBuildVersion");
                    }
                }
                else
                {
                    clearCache = true;
                    sharedPreference.SetString(AppVersionHelper.GetAppShortVersion(), "appShortVersion");
                    sharedPreference.SetString(AppVersionHelper.GetBuildVersion(), "appBuildVersion");
                }
            }
            else
            {
                clearCache = true;
                sharedPreference.SetString(AppVersionHelper.GetAppShortVersion(), "appShortVersion");
                sharedPreference.SetString(AppVersionHelper.GetBuildVersion(), "appBuildVersion");
            }

            if (clearCache)
            {
                BillHistoryEntity.DeleteTable();
                ChartEntity.DeleteTable();
                DueEntity.DeleteTable();
                GetAccountsForAppUpdate();
            }
        }

        public void GetAccountsForAppUpdate()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var isLogin = sharedPreference.BoolForKey(TNBGlobal.PreferenceKeys.LoginState);
            if (isLogin && DataManager.DataManager.SharedInstance.UserEntity != null && DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
            {
                DataManager.DataManager.SharedInstance.User.UserID = DataManager.DataManager.SharedInstance.UserEntity[0]?.userID;
                ServiceCall.GetAccounts().ContinueWith(task =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (DataManager.DataManager.SharedInstance.CustomerAccounts?.d != null
                            && DataManager.DataManager.SharedInstance.CustomerAccounts?.d?.IsSuccess == true)
                        {
                            if (DataManager.DataManager.SharedInstance.CustomerAccounts?.d?.data != null)
                            {
                                DataManager.DataManager.SharedInstance.AccountRecordsList.d
                                           = DataManager.DataManager.SharedInstance.CustomerAccounts.d.data;
                            }

                            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                                && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null)
                            {
                                UserAccountsEntity uaManager = new UserAccountsEntity();
                                uaManager.DeleteTable();
                                uaManager.CreateTable();
                                uaManager.InsertListOfItems(DataManager.DataManager.SharedInstance.AccountRecordsList);
                                DataManager.DataManager.SharedInstance.AccountRecordsList = uaManager.GetCustomerAccountRecordList();
                            }

                            if (DataManager.DataManager.SharedInstance.AccountRecordsList == null
                               || DataManager.DataManager.SharedInstance.AccountRecordsList?.d == null)
                            {
                                DataManager.DataManager.SharedInstance.AccountRecordsList = new CustomerAccountRecordListModel();
                                DataManager.DataManager.SharedInstance.AccountRecordsList.d = new List<CustomerAccountRecordModel>();
                            }
                        }
                    });
                });
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
        }

        internal void SetupSuperViewBackground()
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;

            var gradientLayer = new CAGradientLayer();
            gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
            gradientLayer.Locations = new NSNumber[] { 0.0, 1.5 };
            gradientLayer.Frame = View.Bounds;
            gradientLayer.Opaque = false;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        await LoadMasterData();
                    }
                    else
                    {
                        Debug.WriteLine("No Network");
                        AlertHandler.DisplayNoDataAlert(this);
                    }
                });
            });
        }

        /// <summary>
        /// Checks if app update is required.
        /// </summary>
        /// <returns><c>true</c>, if app update required was ised, <c>false</c> otherwise.</returns>
        private bool IsAppUpdateRequired(ForceUpdateInfoModel forceUpdateData)
        {
            if (forceUpdateData != null && (bool)forceUpdateData?.isIOSForceUpdateOn)
            {
                if (!string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.LatestAppVersion))
                {
                    // if latest app version is higher
                    return string.CompareOrdinal(DataManager.DataManager.SharedInstance.LatestAppVersion, AppVersionHelper.GetAppShortVersion()) > 0;
                }
            }
            return false;
        }

        /// <summary>
        /// Loads the master data.
        /// </summary>
        /// <returns>The master data.</returns>
        private async Task LoadMasterData()
        {
            var response = await ServiceCall.GetAppLaunchMasterData();
            if ((bool)response?.didSucceed)
            {
                if ((bool)response?.status.ToUpper().Equals("MAINTENANCE"))
                {
                    isMaintenance = true;
                    float screenHeight = (float)UIApplication.SharedApplication.KeyWindow.Frame.Height;
                    float screenWidth = (float)UIApplication.SharedApplication.KeyWindow.Frame.Width;
                    float imageWidth = DeviceHelper.GetScaledWidth(151f);
                    float imageHeight = DeviceHelper.GetScaledHeight(136f);
                    float labelWidth = screenWidth - 40f;
                    float lineTextHeight = 24f;

                    UIImageView imageView = new UIImageView(UIImage.FromBundle("Maintenance-Image"))
                    {
                        Frame = new CGRect(DeviceHelper.GetCenterXWithObjWidth(imageWidth), DeviceHelper.GetScaledHeightWithY(90f), imageWidth, imageHeight)
                    };

                    var titleMsg = response?.data?.MaintenanceTitle ?? string.Empty;
                    var descMsg = response?.data?.MaintenanceMessage ?? string.Empty;

                    UILabel lblTitle = new UILabel(new CGRect(DeviceHelper.GetCenterXWithObjWidth(labelWidth), imageView.Frame.GetMaxY() + 24f, labelWidth, 44f))
                    {
                        Text = titleMsg,
                        TextAlignment = UITextAlignment.Center,
                        TextColor = MyTNBColor.SunGlow,
                        Font = MyTNBFont.MuseoSans24_500
                    };

                    NSMutableParagraphStyle msgParagraphStyle = new NSMutableParagraphStyle
                    {
                        Alignment = UITextAlignment.Center,
                        MinimumLineHeight = lineTextHeight,
                        MaximumLineHeight = lineTextHeight
                    };

                    UIStringAttributes msgAttributes = new UIStringAttributes
                    {
                        Font = MyTNBFont.MuseoSans16_300,
                        ForegroundColor = UIColor.White,
                        BackgroundColor = UIColor.Clear,
                        ParagraphStyle = msgParagraphStyle
                    };

                    var attributedText = new NSMutableAttributedString(descMsg);
                    attributedText.AddAttributes(msgAttributes, new NSRange(0, descMsg.Length));

                    UILabel lblDesc = new UILabel()
                    {
                        AttributedText = attributedText,
                        Lines = 0
                    };

                    CGSize cGSize = lblDesc.SizeThatFits(new CGSize(labelWidth, 1000f));
                    lblDesc.Frame = new CGRect(DeviceHelper.GetCenterXWithObjWidth(labelWidth), lblTitle.Frame.GetMaxY() + 8f, labelWidth, cGSize.Height);

                    maintenanceView.AddSubviews(new UIView[] { imageView, lblTitle, lblDesc });
                    if (!maintenanceView.IsDescendantOfView(View))
                    {
                        View.AddSubview(maintenanceView);
                    }
                }
                else
                {
                    isMaintenance = false;
                    var data = response?.data;

                    var iOSIndex = data?.AppVersions?.FindIndex(x => x.IsIos) ?? -1;
                    DataManager.DataManager.SharedInstance.LatestAppVersion = data?.ForceUpdateInfo?.iOSLatestVersion;
                    DataManager.DataManager.SharedInstance.SystemStatus = data?.SystemStatus ?? new List<DowntimeDataModel>();
                    DataManager.DataManager.SharedInstance.SetSystemsAvailability();
                    DataManager.DataManager.SharedInstance.WebLinks = data?.WebLinks ?? new List<WebLinksDataModel>();

                    DataManager.DataManager.SharedInstance.LocationTypes = data?.LocationTypes ?? new List<LocationTypeDataModel>();
                    if (data?.LocationTypes != null)
                    {
                        LocationTypeDataModel allLocationModel = new LocationTypeDataModel();
                        allLocationModel.Id = "all";
                        allLocationModel.Title = "All";
                        allLocationModel.Description = "All";
                        if (DataManager.DataManager.SharedInstance.LocationTypes != null)
                        {
                            DataManager.DataManager.SharedInstance.LocationTypes.Insert(0, allLocationModel);
                        }
                    }

                    DataManager.DataManager.SharedInstance.StatesForFeedBack = data?.States ?? new List<StatesForFeedbackDataModel>();
                    DataManager.DataManager.SharedInstance.FeedbackCategory = data?.FeedbackCategories ?? new List<FeedbackCategoryDataModel>();
                    DataManager.DataManager.SharedInstance.OtherFeedbackType = data?.FeedbackTypes ?? new List<OtherFeedbackTypeDataModel>();

                    var rawNotifGeneralTypes = data?.NotificationTypes ?? new List<NotificationPreferenceModel>();
                    DataManager.DataManager.SharedInstance.NotificationGeneralTypes = rawNotifGeneralTypes.FindAll(item => item?.ShowInFilterList?.ToLower() == "true") ?? new List<NotificationPreferenceModel>();

                    if (data?.NotificationTypes != null)
                    {
                        NotificationPreferenceModel allNotificationItem = new NotificationPreferenceModel();
                        allNotificationItem.Title = "PushNotification_AllNotifications".Translate();
                        allNotificationItem.Id = "all";
                        if (DataManager.DataManager.SharedInstance.NotificationGeneralTypes != null)
                        {
                            DataManager.DataManager.SharedInstance.NotificationGeneralTypes.Insert(0, allNotificationItem);
                        }
                    }
                    if (IsAppUpdateRequired(data?.ForceUpdateInfo))
                    {
                        ForceUpdateInfoModel forceUpdateData = data?.ForceUpdateInfo;
                        AlertHandler.DisplayForceUpdate(forceUpdateData.ModalTitle, forceUpdateData.ModalBody
                            , forceUpdateData.ModalBtnText, OpenUpdateLink);
                    }
                    else
                    {
                        ExecuteSiteCoreCall();
                    }
                }
            }
            else
            {
                AlertHandler.DisplayServiceError(this, response?.message);
            }
        }

        private Task LoadSSMRWalkthrough()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                ApplySSMRTimeStampResponseModel timeStamp = iService.GetApplySSMRWalkthroughTimestampItem();

                bool needsUpdate = true;
                if (timeStamp != null && timeStamp.Data != null && timeStamp.Data.Count > 0 && timeStamp.Data[0] != null
                    && !string.IsNullOrEmpty(timeStamp.Data[0].Timestamp))
                {
                    NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                    string currentTS = sharedPreference.StringForKey("SiteCoreApplySSMRWalkthroughTimeStamp");
                    if (string.IsNullOrEmpty(currentTS) || string.IsNullOrWhiteSpace(currentTS))
                    {
                        sharedPreference.SetString(timeStamp.Data[0].Timestamp, "SiteCoreApplySSMRWalkthroughTimeStamp");
                        sharedPreference.Synchronize();
                    }
                    else
                    {
                        if (currentTS.Equals(timeStamp.Data[0].Timestamp))
                        {
                            needsUpdate = false;
                        }
                        else
                        {
                            sharedPreference.SetString(timeStamp.Data[0].Timestamp, "SiteCoreApplySSMRWalkthroughTimeStamp");
                            sharedPreference.Synchronize();
                        }
                    }
                }

                if (needsUpdate)
                {
                    ApplySSMRResponseModel applySSMrWalkthroughItems = iService.GetApplySSMRWalkthroughItems();
                    if (applySSMrWalkthroughItems != null && applySSMrWalkthroughItems.Data != null && applySSMrWalkthroughItems.Data.Count > 0)
                    {
                        ApplySSMRWalkthroughEntity wsManager = new ApplySSMRWalkthroughEntity();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(applySSMrWalkthroughItems.Data);
                    }
                }
            });
        }

        private Task LoadMeterReadSSMRWalkthrough()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                MeterReadSSMRTimeStampResponseModel timeStamp = iService.GetMeterReadSSMRWalkthroughTimestampItem();

                bool needsUpdate = true;
                if (timeStamp != null && timeStamp.Data != null && timeStamp.Data.Count > 0 && timeStamp.Data[0] != null
                    && !string.IsNullOrEmpty(timeStamp.Data[0].Timestamp))
                {
                    NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                    string currentTS = sharedPreference.StringForKey("SiteCoreMeterReadSSMRWalkthroughTimeStamp");
                    if (string.IsNullOrEmpty(currentTS) || string.IsNullOrWhiteSpace(currentTS))
                    {
                        sharedPreference.SetString(timeStamp.Data[0].Timestamp, "SiteCoreMeterReadSSMRWalkthroughTimeStamp");
                        sharedPreference.Synchronize();
                    }
                    else
                    {
                        if (currentTS.Equals(timeStamp.Data[0].Timestamp))
                        {
                            needsUpdate = false;
                        }
                        else
                        {
                            sharedPreference.SetString(timeStamp.Data[0].Timestamp, "SiteCoreMeterReadSSMRWalkthroughTimeStamp");
                            sharedPreference.Synchronize();
                        }
                    }
                }

                if (needsUpdate)
                {
                    MeterReadSSMRResponseModel meterReadSSMrWalkthroughItems = iService.GetMeterReadSSMRWalkthroughItems();
                    if (meterReadSSMrWalkthroughItems != null && meterReadSSMrWalkthroughItems.Data != null && meterReadSSMrWalkthroughItems.Data.Count > 0)
                    {
                        MeterReadSSMRWalkthroughEntity wsManager = new MeterReadSSMRWalkthroughEntity();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(meterReadSSMrWalkthroughItems.Data);
                    }
                }
            });
        }

        private Task LoadMeterReadSSMRWalkthroughV2()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                MeterReadSSMRTimeStampResponseModel timeStamp = iService.GetMeterReadSSMRWalkthroughTimestampItemV2();

                bool needsUpdate = true;
                if (timeStamp != null && timeStamp.Data != null && timeStamp.Data.Count > 0 && timeStamp.Data[0] != null
                    && !string.IsNullOrEmpty(timeStamp.Data[0].Timestamp))
                {
                    NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                    string currentTS = sharedPreference.StringForKey("SiteCoreMeterReadSSMRWalkthroughTimeStampV2");
                    if (string.IsNullOrEmpty(currentTS) || string.IsNullOrWhiteSpace(currentTS))
                    {
                        sharedPreference.SetString(timeStamp.Data[0].Timestamp, "SiteCoreMeterReadSSMRWalkthroughTimeStampV2");
                        sharedPreference.Synchronize();
                    }
                    else
                    {
                        if (currentTS.Equals(timeStamp.Data[0].Timestamp))
                        {
                            needsUpdate = false;
                        }
                        else
                        {
                            sharedPreference.SetString(timeStamp.Data[0].Timestamp, "SiteCoreMeterReadSSMRWalkthroughTimeStampV2");
                            sharedPreference.Synchronize();
                        }
                    }
                }

                if (needsUpdate)
                {
                    MeterReadSSMRResponseModel meterReadSSMrWalkthroughItems = iService.GetMeterReadSSMRWalkthroughItemsV2();
                    if (meterReadSSMrWalkthroughItems != null && meterReadSSMrWalkthroughItems.Data != null && meterReadSSMrWalkthroughItems.Data.Count > 0)
                    {
                        MeterReadSSMRWalkthroughEntityV2 wsManager = new MeterReadSSMRWalkthroughEntityV2();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(meterReadSSMrWalkthroughItems.Data);
                    }
                }
            });
        }

        private void OpenUpdateLink()
        {
            int index = DataManager.DataManager.SharedInstance.WebLinks?.FindIndex(x => x.Code.ToLower().Equals("ios")) ?? -1;
            if (index > -1)
            {
                string url = DataManager.DataManager.SharedInstance.WebLinks[index].Url;
                if (!string.IsNullOrWhiteSpace(url))
                {
                    if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                    {
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(url), new NSDictionary(), null);
                    }
                    else
                    {
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
                    }
                }
            }
        }

        internal void GetUserEntity()
        {
            UserEntity uManager = new UserEntity();
            DataManager.DataManager.SharedInstance.UserEntity = uManager.GetAllItems();
        }

        internal void ShowOnboarding()
        {
            UIStoryboard onboardingStoryboard = UIStoryboard.FromName("Onboarding", null);
            GenericPageRootViewController onboardingVC = onboardingStoryboard.InstantiateViewController("GenericPageRootViewController") as GenericPageRootViewController;
            onboardingVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            onboardingVC.PageType = GenericPageViewEnum.Type.Onboarding;
            PresentViewController(onboardingVC, true, null);
        }

        internal void ShowPrelogin()
        {
            UIStoryboard loginStoryboard = UIStoryboard.FromName("Login", null);
            UIViewController preloginVC = (UIViewController)loginStoryboard.InstantiateViewController("PreloginViewController");
            preloginVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            PresentViewController(preloginVC, true, null);
        }

        internal void ShowDashboard()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
            ShowViewController(loginVC, this);
        }

        internal async void ExecuteSiteCoreCall()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var isWalkthroughDone = sharedPreference.BoolForKey("isWalkthroughDone");
            GetUserEntity();
            if (SSMRAccounts.IsHideOnboarding)
            {
                ApplySSMRWalkthroughEntity wsManager = new ApplySSMRWalkthroughEntity();
                wsManager.DeleteTable();
            }
            else
            {
                await LoadSSMRWalkthrough();
            }
            await LoadMeterReadSSMRWalkthrough();
            await LoadMeterReadSSMRWalkthroughV2();
            if (isWalkthroughDone)
            {
                await ClearWalkthroughCache();
                var isLogin = sharedPreference.BoolForKey(TNBGlobal.PreferenceKeys.LoginState);
                if (isLogin && DataManager.DataManager.SharedInstance.UserEntity != null && DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
                {
                    DataManager.DataManager.SharedInstance.User.UserID = DataManager.DataManager.SharedInstance.UserEntity[0]?.userID;

                    bool isPhoneVerified = await GetPhoneVerificationStatus();

                    if (isPhoneVerified)
                    {
                        ExecuteGetCutomerRecordsCall();
                    }
                    else
                    {
                        ShowUpdateMobileNumber(true);
                    }
                }
                else
                {
                    ShowPrelogin();
                    UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                }
            }
            else
            {
                _imageSize = DeviceHelper.GetImageSize((int)View.Frame.Width);
                await GetWalkthroughScreens().ContinueWith(task =>
                {
                    InvokeOnMainThread(() =>
                    {
                        ShowOnboarding();
                        UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                    });
                });
            }
        }

        /// <summary>
        /// Creates the cache tables.
        /// </summary>
        private void CreateCacheTables()
        {
            DataManager.DataManager.SharedInstance.CreateUsageHistoryTable();
            DataManager.DataManager.SharedInstance.CreateBillingAccountsTable();
            DataManager.DataManager.SharedInstance.CreateBillHistoryTable();
            DataManager.DataManager.SharedInstance.CreatePaymentHistoryTable();
        }

        private Task ClearWalkthroughCache()
        {
            return Task.Factory.StartNew(() =>
            {
                WalkthroughScreensEntity wsManager = new WalkthroughScreensEntity();
                wsManager.DeleteTable();
            });
        }

        internal Task GetWalkthroughScreens()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS, _imageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                bool isValidTimeStamp = false;
                string timeStampItems = iService.GetTimestampItem();
                TimestampResponseModel timeStampResponse = JsonConvert.DeserializeObject<TimestampResponseModel>(timeStampItems);
                if (timeStampResponse != null && timeStampResponse.Status.Equals("Success")
                    && timeStampResponse.Data != null && timeStampResponse.Data.Count > 0
                    && timeStampResponse.Data[0] != null)
                {
                    var sharedPreference = NSUserDefaults.StandardUserDefaults;
                    string currentTS = sharedPreference.StringForKey("SiteCoreTimeStamp");
                    if (string.IsNullOrEmpty(currentTS) || string.IsNullOrWhiteSpace(currentTS))
                    {
                        sharedPreference.SetString(timeStampResponse.Data[0].Timestamp, "SiteCoreTimeStamp");
                        sharedPreference.Synchronize();
                        isValidTimeStamp = true;
                    }
                    else
                    {
                        if (currentTS.Equals(timeStampResponse.Data[0].Timestamp))
                        {
                            isValidTimeStamp = false;
                        }
                        else
                        {
                            sharedPreference.SetString(timeStampResponse.Data[0].Timestamp, "SiteCoreTimeStamp");
                            sharedPreference.Synchronize();
                            isValidTimeStamp = true;
                        }
                    }
                }
                if (isValidTimeStamp)
                {
                    string tncItems = iService.GetFullRTEPagesItems();
                    TermsAndConditionResponseModel tncResponse = JsonConvert.DeserializeObject<TermsAndConditionResponseModel>(tncItems);
                    if (tncResponse != null && tncResponse.Status.Equals("Success")
                        && tncResponse.Data != null && tncResponse.Data.Count > 0)
                    {
                        TermsAndConditionEntity tncEntity = new TermsAndConditionEntity();
                        tncEntity.DeleteTable();
                        tncEntity.CreateTable();
                        tncEntity.InsertListOfItems(tncResponse.Data);
                    }

                    string walkThroughItems = iService.GetWalkthroughScreenItems();
                    WalkthroughScreensResponseModel walkThroughResponse = JsonConvert.DeserializeObject<WalkthroughScreensResponseModel>(walkThroughItems);
                    if (walkThroughResponse != null && walkThroughResponse.Status.Equals("Success")
                        && walkThroughResponse.Data != null && walkThroughResponse.Data.Count > 0)
                    {
                        WalkthroughScreensEntity wsManager = new WalkthroughScreensEntity();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(walkThroughResponse.Data);
                    }
                }
            });
        }

        internal void ExecuteGetCutomerRecordsCall()
        {
            UserAccountsEntity uaManager = new UserAccountsEntity();
            DataManager.DataManager.SharedInstance.AccountRecordsList = uaManager.GetCustomerAccountRecordList();
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                       && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null
                       && DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0)
            {
                DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList.d[0];
                ShowDashboard();
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            }
            else if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
              && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null
              && DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count == 0)
            {
                ShowDashboard();
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            }
            else
            {
                DataManager.DataManager.SharedInstance.ClearLoginState();
                ShowPrelogin();
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            }
        }

        /// <summary>
        /// Checks if the phone is verified.
        /// </summary>
        /// <returns>The phone is verified.</returns>
        private async Task<bool> GetPhoneVerificationStatus()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            bool isVerified = sharedPreference.BoolForKey(TNBGlobal.PreferenceKeys.PhoneVerification);

            if (!isVerified)
            {
                var response = await ServiceCall.GetPhoneVerificationStatus();

                if (response?.didSucceed == true && response?.data != null)
                {
                    isVerified = response.data.IsVerified;

                    if (isVerified)
                    {
                        sharedPreference.SetBool(true, TNBGlobal.PreferenceKeys.PhoneVerification);
                        sharedPreference.Synchronize();
                    }
                }
                else
                {
                    isVerified = true;
                }
            }
            return isVerified;
        }

        /// <summary>
        /// Shows the update mobile number.
        /// </summary>
        /// <param name="willHideBackButton">If set to <c>true</c> will hide back button.</param>
        private void ShowUpdateMobileNumber(bool willHideBackButton)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("UpdateMobileNumber", null);
            UpdateMobileNumberViewController viewController =
                storyBoard.InstantiateViewController("UpdateMobileNumberViewController") as UpdateMobileNumberViewController;
            if (viewController != null)
            {
                viewController.WillHideBackButton = willHideBackButton;
                viewController.IsFromLogin = true;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            }
            ActivityIndicator.Hide();
        }
    }
}