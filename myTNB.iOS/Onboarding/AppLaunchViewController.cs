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
using myTNB.SitecoreCMS.Service;
using System.IO;
using System.Net;
using System.Threading;

namespace myTNB
{
    public partial class AppLaunchViewController : UIViewController
    {
        UIImageView imgViewAppLaunch;
        UIImage _imgSplash;
        string _imageSize = string.Empty;
        string _imageFilePath;
        bool isMaintenance;
        bool isValidSplashTimestamp;
        UIView maintenanceView;
        public AppLaunchViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            imgViewAppLaunch = new UIImageView(UIImage.FromBundle("AppLaunch"));
            View.AddSubview(imgViewAppLaunch);

            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            View.AddConstraints(
                imgViewAppLaunch.AtTopOf(View, 0),
                imgViewAppLaunch.AtBottomOf(View, 0),
                imgViewAppLaunch.AtLeftOf(View, 0),
                imgViewAppLaunch.AtRightOf(View, 0)
            );

            //Create DB
            SQLiteHelper.CreateDB();
            CreateCacheTables();

            // clear cache data on App Launch
            BillingAccountEntity.DeleteTable();
            PaymentHistoryEntity.DeleteTable();

            // clear cached data on Version Update
            ClearCacheForVersionUpdate();
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, HandleAppWillEnterForeground);
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, (float)UIScreen.MainScreen.Bounds.Height, false);
            maintenanceView = gradientViewComponent.GetUI();
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
                        _imageSize = DeviceHelper.GetImageSize((int)View.Frame.Width);
                        string splashFileName = "SplashImage.png";
                        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
                        _imageFilePath = Path.Combine(documentsPath, splashFileName);

                        await GetAppLaunchImage();

                        string imgUrl = string.Empty;
                        string startDateStr = string.Empty;
                        string endDateStr = string.Empty;
                        int delay = 0;
                        List<AppLaunchImageModel> appLaunchData = new List<AppLaunchImageModel>();
                        AppLaunchImageEntity wsManager = new AppLaunchImageEntity();
                        List<AppLaunchImageModel> appLaunchList = wsManager.GetAllItems();
                        if (appLaunchList.Count > 0)
                        {
                            appLaunchData = new List<AppLaunchImageModel>();
                            foreach (var entity in appLaunchList)
                            {
                                AppLaunchImageModel item = new AppLaunchImageModel
                                {
                                    Title = entity.Title,
                                    Description = entity.Description,
                                    Image = entity.Image,
                                    StartDateTime = entity.StartDateTime,
                                    EndDateTime = entity.EndDateTime,
                                    ShowForSeconds = entity.ShowForSeconds
                                };
                                appLaunchData.Add(item);
                            }
                            imgUrl = appLaunchData[0].Image;
                            startDateStr = appLaunchData[0].StartDateTime;
                            endDateStr = appLaunchData[0].EndDateTime;
                            delay = int.Parse(appLaunchData[0].ShowForSeconds);
                        }
                        else
                        {
                            ShowDefaultSplashImage();
                        }

                        if (isValidSplashTimestamp)
                        {
                            if (!string.IsNullOrEmpty(startDateStr) && !string.IsNullOrEmpty(endDateStr))
                            {
                                if (IsValidDate(startDateStr, endDateStr))
                                {
                                    Debug.WriteLine("Current date is valid");
                                    if (!string.IsNullOrEmpty(imgUrl) && !string.IsNullOrWhiteSpace(imgUrl))
                                    {
                                        DownloadSplashImage(imgUrl, delay);
                                    }
                                    else
                                    {
                                        ShowDefaultSplashImage();
                                    }
                                }
                                else
                                {
                                    ShowDefaultSplashImage();
                                }
                            }
                            else
                            {
                                ShowDefaultSplashImage();
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(startDateStr) && !string.IsNullOrEmpty(endDateStr))
                            {
                                if (IsValidDate(startDateStr, endDateStr))
                                {
                                    Debug.WriteLine("Current date is valid");
                                    if (File.Exists(_imageFilePath))
                                    {
                                        _imgSplash = UIImage.FromFile(_imageFilePath);
                                        UIView.Animate(3, 0, UIViewAnimationOptions.TransitionCrossDissolve
                                            , () =>
                                            {
                                                imgViewAppLaunch.Image = _imgSplash;
                                            }
                                            , () =>
                                            {
                                                Debug.WriteLine("Show Saved Image!");
                                                ProceedToNextScreen(delay);
                                            }
                                        );
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(imgUrl) && !string.IsNullOrWhiteSpace(imgUrl))
                                        {
                                            DownloadSplashImage(imgUrl, delay);
                                        }
                                        else
                                        {
                                            ShowDefaultSplashImage();
                                        }
                                    }
                                }
                                else
                                {
                                    ShowDefaultSplashImage();
                                }
                            }
                            else
                            {
                                ShowDefaultSplashImage();
                            }
                        }
                    }
                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
                    }
                });
            });
        }

        private bool IsValidDate(string startDateStr, string endDateStr)
        {
            bool res = false;
            try
            {
                int syear = Int32.Parse(startDateStr.Substring(0, 4));
                int smonth = Int32.Parse(startDateStr.Substring(4, 2));
                int sday = Int32.Parse(startDateStr.Substring(6, 2));

                int eyear = Int32.Parse(endDateStr.Substring(0, 4));
                int emonth = Int32.Parse(endDateStr.Substring(4, 2));
                int eday = Int32.Parse(endDateStr.Substring(6, 2));

                DateTime startDate = new DateTime(syear, smonth, sday);
                DateTime endDate = new DateTime(eyear, emonth, eday);

                int svalue = DateTime.Compare(DateTime.Now, startDate);
                int evalue = DateTime.Compare(DateTime.Now, endDate);

                res = svalue >= 0 && evalue <= 0;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Parse Error: " + e.Message);
            }
            return res;
        }

        private void DownloadSplashImage(string url, int delay = 0)
        {
            try
            {
                var webClient = new WebClient();
                webClient.DownloadDataCompleted += (s, args) =>
                {
                    var data = args?.Result;
                    if (data != null)
                    {
                        File.WriteAllBytes(_imageFilePath, data);
                        InvokeOnMainThread(() =>
                        {
                            _imgSplash = UIImage.FromFile(_imageFilePath);
                            imgViewAppLaunch.Image = _imgSplash;
                            Debug.WriteLine("Show DOWNLOADED Image!");
                            ProceedToNextScreen(delay);
                        });
                    }
                    else
                    {
                        ShowDefaultSplashImage();
                    }
                };

                bool result = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (result)
                {
                    webClient.DownloadDataAsync(new Uri(url));
                }
                else
                {
                    ShowDefaultSplashImage();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Image load Error: " + e.Message);
                ShowDefaultSplashImage();
            }
        }

        private void ShowDefaultSplashImage()
        {
            _imgSplash = UIImage.FromBundle("SplashImageDefault");
            imgViewAppLaunch.Image = _imgSplash;
            Debug.WriteLine("Show DEFAULT Image!");
            ProceedToNextScreen();
        }

        private void ProceedToNextScreen(int delay = 0)
        {
            Debug.WriteLine("ProceedToNextScreen()");
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        Thread.Sleep(delay * 1000);
                        GetUserEntity();
                        await LoadMasterData();
                    }
                    else
                    {
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
            UIViewController onboardingVC = (UIViewController)onboardingStoryboard.InstantiateViewController("OnboardingRootViewController");
            onboardingVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
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
            if (isWalkthroughDone)
            {
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

        internal Task GetAppLaunchImage()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS, _imageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                bool isValidTimeStamp = false;
                string appLaunchImageTS = iService.GetAppLaunchImageTimestampItem();
                TimestampResponseModel timestamp = JsonConvert.DeserializeObject<TimestampResponseModel>(appLaunchImageTS);
                if (timestamp != null && timestamp.Status.Equals("Success")
                    && timestamp.Data != null && timestamp.Data.Count > 0
                    && timestamp.Data[0] != null)
                {
                    var sharedPreference = NSUserDefaults.StandardUserDefaults;
                    string currentTS = sharedPreference.StringForKey("AppLaunchImageTimeStamp");
                    if (string.IsNullOrEmpty(currentTS) || string.IsNullOrWhiteSpace(currentTS))
                    {
                        sharedPreference.SetString(timestamp.Data[0].Timestamp, "AppLaunchImageTimeStamp");
                        sharedPreference.Synchronize();
                        isValidTimeStamp = true;
                    }
                    else
                    {
                        if (currentTS.Equals(timestamp.Data[0].Timestamp))
                        {
                            isValidTimeStamp = false;
                        }
                        else
                        {
                            sharedPreference.SetString(timestamp.Data[0].Timestamp, "AppLaunchImageTimeStamp");
                            sharedPreference.Synchronize();
                            isValidTimeStamp = true;
                        }
                    }
                }
                isValidSplashTimestamp = isValidTimeStamp;
                if (isValidTimeStamp)
                {
                    string items = iService.GetAppLaunchImageItem();
                    AppLaunchImageResponseModel response = JsonConvert.DeserializeObject<AppLaunchImageResponseModel>(items);
                    if (response != null && response.Status.Equals("Success")
                        && response.Data != null && response.Data.Count > 0)
                    {
                        AppLaunchImageEntity wsManager = new AppLaunchImageEntity();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(response.Data);
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