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
using System.IO;
using System.Net;

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
        string _imgUrl = string.Empty;
        string _startDateStr = string.Empty;
        string _endDateStr = string.Empty;
        double _delay;
        bool _isGetDynamicDone, _isTaskDelayDone, _isLoadMasterDataDone, _splashIsShown, _hasProceeded;
        int _timeOut = 2500;
        MasterDataResponseModel _masterDataResponse = new MasterDataResponseModel();
        UIView maintenanceView;
        public AppLaunchViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            imgViewAppLaunch = new UIImageView(UIImage.FromBundle("AppLaunch"));
            imgViewAppLaunch.ContentMode = UIViewContentMode.ScaleAspectFill;
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
                                ProcessMasterData();
                            }
                            else
                            {
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

        public async Task DelayTask()
        {
            await Task.Delay(_timeOut);
            _isTaskDelayDone = true;
            DelayTaskCompletion();
        }

        private void DelayTaskCompletion()
        {
            InvokeOnMainThread(() =>
            {
                if (!_isLoadMasterDataDone) // GetAppMasterLaunch API Call is not yet done
                {
                    GetDynamicSplashData();
                    if (_isGetDynamicDone && !_splashIsShown) // GetDynamicSplash has finished loading AND image is still not loaded
                    {
                        ShowDefaultSplashImage();
                    }
                    else if (!_isGetDynamicDone && !_splashIsShown) // GetDynamicSplash is not yet finished AND image is not loaded
                    {
                        if (File.Exists(_imageFilePath))
                        {
                            _imgSplash = UIImage.FromFile(_imageFilePath);
                            _splashIsShown = true;
                            imgViewAppLaunch.ContentMode = UIViewContentMode.ScaleAspectFill;
                            UIView.Transition(imgViewAppLaunch, 0.0,
                                UIViewAnimationOptions.TransitionCrossDissolve,
                                () => { imgViewAppLaunch.Image = _imgSplash; },
                                () =>
                                {
                                    ShowSplashScreenWithDelay(_delay);
                                }
                            );
                        }
                        else
                        {
                            ShowDefaultSplashImage();
                        }
                    }
                }
                else
                {
                    if (!_splashIsShown)
                    {
                        ShowDefaultSplashImage();
                    }
                    if (!_hasProceeded)
                    {
                        ProcessMasterData();
                    }
                }
            });
        }

        private void LoadMasterDataCompletion()
        {
            InvokeOnMainThread(() =>
            {
                if (_isTaskDelayDone)
                {
                    if (!_splashIsShown)
                    {
                        ShowDefaultSplashImage();
                    }
                    ProcessMasterData();
                }
            });
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            GetUserEntity();
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

                        InvokeInBackground(async () =>
                        {
                            _isGetDynamicDone = await GetDynamicSplash();
                            InvokeOnMainThread(() =>
                            {
                                PrepareDynamicSplash(_isTaskDelayDone);
                            });
                        });

                        var tasks = new List<Task>
                        {
                            Task.Run(DelayTask),
                            Task.Run(LoadMasterData)
                        };
                        await Task.WhenAll(tasks);
                    }

                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
                    }
                });
            });
        }

        private void GetDynamicSplashData()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            string cachedData = sharedPreference.StringForKey("AppLaunchImageData");
            AppLaunchImageResponseModel model;
            List<AppLaunchImageModel> appLaunchData = new List<AppLaunchImageModel>();
            if (!string.IsNullOrEmpty(cachedData) && !string.IsNullOrWhiteSpace(cachedData))
            {
                model = JsonConvert.DeserializeObject<AppLaunchImageResponseModel>(cachedData);
                if (model != null)
                {
                    foreach (AppLaunchImageModel obj in model.Data)
                    {
                        AppLaunchImageModel item = new AppLaunchImageModel
                        {
                            Title = obj.Title,
                            Description = obj.Description,
                            Image = obj.Image,
                            StartDateTime = obj.StartDateTime,
                            EndDateTime = obj.EndDateTime,
                            ShowForSeconds = obj.ShowForSeconds
                        };
                        appLaunchData.Add(item);
                    }
                    _imgUrl = appLaunchData[0].Image;
                    _startDateStr = appLaunchData[0].StartDateTime;
                    _endDateStr = appLaunchData[0].EndDateTime;
                    _delay = double.Parse(appLaunchData[0].ShowForSeconds);
                }
            }
        }

        private void PrepareDynamicSplash(bool timeOut)
        {
            GetDynamicSplashData();
            if (isValidSplashTimestamp)
            {
                if (!string.IsNullOrEmpty(_startDateStr) && !string.IsNullOrEmpty(_endDateStr))
                {
                    if (IsValidDate(_startDateStr, _endDateStr))
                    {
                        if (!string.IsNullOrEmpty(_imgUrl) && !string.IsNullOrWhiteSpace(_imgUrl))
                        {
                            DownloadSplashImage(_imgUrl);
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_startDateStr) && !string.IsNullOrEmpty(_endDateStr))
                {
                    if (IsValidDate(_startDateStr, _endDateStr))
                    {
                        if (File.Exists(_imageFilePath))
                        {
                            if (!_splashIsShown)
                            {
                                _imgSplash = UIImage.FromFile(_imageFilePath);
                                _splashIsShown = true;
                                imgViewAppLaunch.ContentMode = UIViewContentMode.ScaleAspectFill;
                                UIView.Transition(imgViewAppLaunch, 0.0,
                                    UIViewAnimationOptions.TransitionCrossDissolve,
                                    () => { imgViewAppLaunch.Image = _imgSplash; },
                                    () =>
                                    {
                                        if (!_isTaskDelayDone)
                                        {
                                            ShowSplashScreenWithDelay(_delay);
                                        }
                                    }
                                );
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(_imgUrl) && !string.IsNullOrWhiteSpace(_imgUrl))
                            {
                                DownloadSplashImage(_imgUrl);
                            }
                        }
                    }
                }
            }
        }

        private bool IsValidDate(string startDateStr, string endDateStr)
        {
            bool res = false;
            try
            {
                DateTime startDate = DateTime.ParseExact(startDateStr, "yyyyMMddTHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(endDateStr, "yyyyMMddTHHmmss", System.Globalization.CultureInfo.InvariantCulture);

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

        private void DownloadSplashImage(string url)
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
                            if (!_splashIsShown)
                            {
                                _splashIsShown = true;
                                _imgSplash = UIImage.FromFile(_imageFilePath);
                                imgViewAppLaunch.ContentMode = UIViewContentMode.ScaleAspectFill;
                                UIView.Transition(imgViewAppLaunch, 0.0,
                                    UIViewAnimationOptions.TransitionCrossDissolve,
                                    () => { imgViewAppLaunch.Image = _imgSplash; },
                                    () =>
                                    {
                                        if (!_isTaskDelayDone)
                                        {
                                            ShowSplashScreenWithDelay(_delay);
                                        }
                                    }
                                );
                            }
                        });
                    }
                };

                bool result = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (result)
                {
                    webClient.DownloadDataAsync(new Uri(url));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Image load Error: " + e.Message);
            }
        }

        private void ShowDefaultSplashImage()
        {
            if (!_splashIsShown)
            {
                _splashIsShown = true;
                _imgSplash = UIImage.FromBundle("SplashImageDefault");
                imgViewAppLaunch.ContentMode = UIViewContentMode.ScaleAspectFill;
                UIView.Transition(imgViewAppLaunch, 0.0,
                    UIViewAnimationOptions.TransitionCrossDissolve,
                    () => { imgViewAppLaunch.Image = _imgSplash; },
                    () => { }
                );
            }
        }

        private void ShowSplashScreenWithDelay(double delay = 0)
        {
            InvokeOnMainThread(async () =>
            {
                if (delay > 0)
                {
                    int d = (int)(delay * 1000);
                    await Task.Delay(d);
                    if (!_hasProceeded && _isLoadMasterDataDone)
                    {
                        ProcessMasterData();
                    }
                }
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

        private void ProcessMasterData()
        {
            _hasProceeded = true;
            var response = _masterDataResponse;
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

        /// <summary>
        /// Loads the master data.
        /// </summary>
        /// <returns>The master data.</returns>
        private async Task LoadMasterData()
        {
            _masterDataResponse = await ServiceCall.GetAppLaunchMasterData();
            _isLoadMasterDataDone = true;
            LoadMasterDataCompletion();
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

        internal async Task<bool> GetDynamicSplash()
        {
            bool result = false;
            await Task.Run(() =>
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
                        var sharedPreference = NSUserDefaults.StandardUserDefaults;
                        var jsonStr = JsonConvert.SerializeObject(response);
                        sharedPreference.SetString(jsonStr, "AppLaunchImageData");
                        sharedPreference.Synchronize();
                    }
                }
                result = true;
            });

            return result;
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
