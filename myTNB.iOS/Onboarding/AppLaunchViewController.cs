using Foundation;
using System;
using UIKit;
using CoreAnimation;
using myTNB.Model;
using System.Threading.Tasks;
using Newtonsoft.Json;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.SQLite;
using myTNB.DataManager;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using myTNB.Dashboard.DashboardComponents;
using System.IO;
using System.Net;
using myTNB.SitecoreCMS;

namespace myTNB
{
    public enum OnboardingEnum
    {
        None = 0,
        FreshInstall,
        AppUpdate
    }

    public partial class AppLaunchViewController : UIViewController
    {
        UIImageView imgViewAppLaunch;
        UIImage _imgSplash;
        string _imageFilePath;
        bool isMaintenance;
        string _imgUrl = string.Empty;
        string _startDateStr = string.Empty;
        string _endDateStr = string.Empty;
        double _delay;
        bool _isGetDynamicDone, _isTaskDelayDone, _isLoadMasterDataDone, _splashIsShown, _hasProceeded, _splashDelayIsDone;
        OnboardingEnum _onboardingEnum;
        int _timeOut = 4000;
        UIView maintenanceView;

        public AppLaunchViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            imgViewAppLaunch = new UIImageView(new CGRect(new CGPoint(0, 0), View.Frame.Size))
            {
                Image = UIImage.FromBundle("AppLaunch"),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            View.AddSubview(imgViewAppLaunch);

            //Create DB
            SQLiteHelper.CreateDB();
            CreateCacheTables();

            // clear cache data on App Launch
            BillingAccountEntity.DeleteTable();
            PaymentHistoryEntity.DeleteTable();

            GetUserEntity();

            // clear cached data on Version Update
            ClearCacheForVersionUpdate();
            NotifCenterUtility.AddObserver(UIApplication.WillEnterForegroundNotification, HandleAppWillEnterForeground);
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, (float)UIScreen.MainScreen.Bounds.Height, false);
            maintenanceView = gradientViewComponent.GetUI();
            LanguageUtility.SetLanguageGlobals();
            DataManager.DataManager.SharedInstance.ImageSize = DeviceHelper.GetImageSize();
            if (!LanguageUtility.IsSaveSuccess)
            {
                InvokeInBackground(() =>
                {
                    LanguageUtility.SaveLanguagePreference().ContinueWith(langTask => { });
                });
            }
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
            var appShortVersion = sharedPreference.StringForKey(AppLaunchConstants.STR_AppShortVersion);
            var appBuildVersion = sharedPreference.StringForKey(AppLaunchConstants.STR_AppBuildVersion);

            if (!string.IsNullOrEmpty(appShortVersion) && !string.IsNullOrEmpty(appBuildVersion))
            {
                if (appShortVersion == AppVersionHelper.GetAppShortVersion())
                {
                    if (appBuildVersion != AppVersionHelper.GetBuildVersion())
                    {
                        _onboardingEnum = OnboardingEnum.AppUpdate;
                    }
                }
                else
                {
                    _onboardingEnum = OnboardingEnum.AppUpdate;
                }
            }
            else
            {
                _onboardingEnum = OnboardingEnum.FreshInstall;
            }

            if (_onboardingEnum == OnboardingEnum.AppUpdate)
            {
                BillHistoryEntity.DeleteTable();
                ChartEntity.DeleteTable();
                DueEntity.DeleteTable();
                //sharedPreference.RemoveObject(Constants.Key_PromotionTimestamp);
            }
        }

        private void UpdateVersionUpdate()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            sharedPreference.SetString(AppVersionHelper.GetAppShortVersion(), AppLaunchConstants.STR_AppShortVersion);
            sharedPreference.SetString(AppVersionHelper.GetBuildVersion(), AppLaunchConstants.STR_AppBuildVersion);
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
                                DataManager.DataManager.SharedInstance.AccountRecordsList.d = DataManager.DataManager.SharedInstance.GetCombinedAcctList();
                            }

                            if (DataManager.DataManager.SharedInstance.AccountRecordsList == null
                               || DataManager.DataManager.SharedInstance.AccountRecordsList?.d == null)
                            {
                                DataManager.DataManager.SharedInstance.AccountRecordsList = new CustomerAccountRecordListModel();
                                DataManager.DataManager.SharedInstance.AccountRecordsList.d = new List<CustomerAccountRecordModel>();
                            }
                            ProcessOnboarding(_onboardingEnum);
                        }
                        else
                        {
                            ShowOnboardingWithNormalLaunch();
                        }
                    });
                });
            }
            else
            {
                ProcessOnboarding(_onboardingEnum);
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
                            if (!string.IsNullOrEmpty(_startDateStr) && !string.IsNullOrEmpty(_endDateStr))
                            {
                                if (IsValidDate(_startDateStr, _endDateStr))
                                {
                                    _imgSplash = UIImage.FromFile(_imageFilePath);
                                    _splashIsShown = true;
                                    imgViewAppLaunch.ContentMode = UIViewContentMode.ScaleAspectFill;
                                    UIView.Transition(imgViewAppLaunch, 0.5,
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
                    if (!_splashIsShown)
                    {
                        ShowDefaultSplashImage();
                    }
                    else if (!_splashDelayIsDone)
                    {
                        ShowSplashScreenWithDelay(_delay);
                    }
                    else if (!_hasProceeded && _splashDelayIsDone)
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
                    else
                    {
                        if (!_hasProceeded && _splashDelayIsDone)
                        {
                            ProcessMasterData();
                        }
                    }
                }
            });
        }

        public override void ViewDidAppear(bool animated)
        {
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
            {
                TextColor = UIColor.White,
                Font = TNBFont.MuseoSans_16_500
            });

            base.ViewDidAppear(animated);
            GetUserEntity();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                if (NetworkUtility.isReachable)
                {
                    string splashFileName = "SplashImage.png";
                    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
                    _imageFilePath = Path.Combine(documentsPath, splashFileName);

                    InvokeInBackground(async () =>
                    {
                        _isGetDynamicDone = await SitecoreServices.Instance.LoadDynamicSplash();
                        if (SitecoreServices.Instance.SplashHasNewTimestamp && File.Exists(_imageFilePath))
                        {
                            try
                            {
                                File.Delete(_imageFilePath);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("Error deleting splash image: " + e.Message);
                            }
                        }
                        InvokeOnMainThread(() =>
                        {
                            PrepareDynamicSplash();
                        });
                        var tasks = new List<Task>
                        {
                            Task.Run(DelayTask),
                            Task.Run(LoadMasterData),
                            Task.Run(LoadLanguage)
                        };
                        await Task.WhenAll(tasks);
                    });
                }

                else
                {
                    AlertHandler.DisplayNoDataAlert(this);
                }
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

        private void PrepareDynamicSplash()
        {
            GetDynamicSplashData();
            if (SitecoreServices.Instance.SplashHasNewTimestamp)
            {
                if (!string.IsNullOrEmpty(_startDateStr) && !string.IsNullOrEmpty(_endDateStr))
                {
                    if (IsValidDate(_startDateStr, _endDateStr))
                    {
                        if (!string.IsNullOrEmpty(_imgUrl) && !string.IsNullOrWhiteSpace(_imgUrl))
                        {
                            DownloadSplashImage(_imgUrl);
                        }
                        else
                        {
                            ShowDefaultSplashImage();
                        }
                    }
                    else
                    {
                        if (File.Exists(_imageFilePath))
                        {
                            try
                            {
                                File.Delete(_imageFilePath);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("Error deleting splash image: " + e.Message);
                            }
                        }
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
                                UIView.Transition(imgViewAppLaunch, 0.5,
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
                            else
                            {
                                ShowDefaultSplashImage();
                            }
                        }
                    }
                    else
                    {
                        if (File.Exists(_imageFilePath))
                        {
                            try
                            {
                                File.Delete(_imageFilePath);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("Error deleting splash image: " + e.Message);
                            }
                        }
                        ShowDefaultSplashImage();
                    }
                }
                else
                {
                    ShowDefaultSplashImage();
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
                WebClient webClient = new WebClient();
                webClient.DownloadDataCompleted += (s, args) =>
                {
                    try
                    {
                        if (args != null)
                        {
                            byte[] data = args.Result;
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
                                        UIView.Transition(imgViewAppLaunch, 0.5,
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
                    catch (Exception e)
                    {
                        Debug.WriteLine("Image load Error: " + e.Message);
                    }
                };

                bool result = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (result)
                {
                    webClient.DownloadDataAsync(new Uri(url));
                }
            }
            catch (MonoTouchException m) { Debug.WriteLine("Image load Error: " + m.Message); }
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
                UIView.Transition(imgViewAppLaunch, 0.5,
                    UIViewAnimationOptions.TransitionCrossDissolve,
                    () => { imgViewAppLaunch.Image = _imgSplash; },
                    () =>
                    {
                        ShowSplashScreenWithDelay(1);
                    }
                );
            }
        }

        private void ShowSplashScreenWithDelay(double delay = 1)
        {
            InvokeOnMainThread(async () =>
            {
                if (delay > 0)
                {
                    int d = (int)(delay * 1000);
                    await Task.Delay(d);
                    _splashDelayIsDone = true;
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

        private void PrepareMaintenanceScreen(AppLaunchMasterDataModel response)
        {
            isMaintenance = true;
            float screenWidth = (float)UIApplication.SharedApplication.KeyWindow.Frame.Width;
            float imageWidth = DeviceHelper.GetScaledWidth(151f);
            float imageHeight = DeviceHelper.GetScaledHeight(136f);
            float labelWidth = screenWidth - 40f;
            float lineTextHeight = 24f;

            UIImageView imageView = new UIImageView(UIImage.FromBundle(AppLaunchConstants.IMG_MaintenanceIcon))
            {
                Frame = new CGRect(DeviceHelper.GetCenterXWithObjWidth(imageWidth), DeviceHelper.GetScaledHeightWithY(90f), imageWidth, imageHeight)
            };

            var titleMsg = response?.DisplayTitle;
            var descMsg = response?.DisplayMessage;

            titleMsg = !string.IsNullOrEmpty(titleMsg) ? titleMsg : LanguageUtility.GetCommonI18NValue(Constants.Common_MaintenanceTitle);
            descMsg = !string.IsNullOrEmpty(descMsg) ? descMsg : LanguageUtility.GetCommonI18NValue(Constants.Common_MaintenanceMsg);

            UILabel lblTitle = new UILabel(new CGRect(DeviceHelper.GetCenterXWithObjWidth(labelWidth), imageView.Frame.GetMaxY() + 24f, labelWidth, 44f))
            {
                Text = titleMsg,
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.SunGlow,
                Font = MyTNBFont.MuseoSans24_500,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            nfloat newTitleHeight = lblTitle.GetLabelHeight(1000);
            lblTitle.Frame = new CGRect(lblTitle.Frame.Location, new CGSize(lblTitle.Frame.Width, newTitleHeight));

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

        private void ProcessMasterData()
        {
            _hasProceeded = true;
            var response = AppLaunchMasterCache.GetAppLaunchResponse();
            if (response != null &&
                response.d != null)
            {
                if (response.d.IsSuccess && response.d.data != null)
                {
                    isMaintenance = false;
                    var data = AppLaunchMasterCache.GetAppLaunchMasterData();
                    if (IsAppUpdateRequired(data?.ForceUpdateInfo))
                    {
                        ForceUpdateInfoModel forceUpdateData = data?.ForceUpdateInfo;
                        AlertHandler.DisplayForceUpdate(forceUpdateData.ModalTitle, forceUpdateData.ModalBody
                            , forceUpdateData.ModalBtnText, OpenUpdateLink);
                    }
                    else
                    {
                        ExecuteLaunch();
                    }

                    InvokeInBackground(async () =>
                    {
                        await SitecoreServices.Instance.OnExecuteSitecoreCall();
                        NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                        sharedPreference.SetBool(AppLaunchMasterCache.IsOCRDown, "IsOCRDown");
                        sharedPreference.Synchronize();
                    });
                }
                else if (response.d.IsMaintenance)
                {
                    PrepareMaintenanceScreen(response.d);
                }
                else
                {
                    AlertHandler.DisplayServiceError(this, response.d.DisplayMessage);
                }
            }
            else
            {
                AlertHandler.DisplayServiceError(this, string.Empty);
            }
        }

        /// <summary>
        /// Loads the master data.
        /// </summary>
        /// <returns>The master data.</returns>
        private async Task LoadMasterData()
        {
            var response = await ServiceCall.GetAppLaunchMasterData();
            AppLaunchMasterCache.AddAppLaunchResponseData(response);
            _isLoadMasterDataDone = true;
            LoadMasterDataCompletion();
        }

        private async Task LoadLanguage()
        {
            await SitecoreServices.Instance.LoadLanguage();
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

        internal void ShowOnboarding(List<OnboardingItemModel> model, bool isToUpdateMobileNo = false)
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var isLogin = sharedPreference.BoolForKey(TNBGlobal.PreferenceKeys.LoginState);
            UIStoryboard onboardingStoryboard = UIStoryboard.FromName("Onboarding", null);
            OnboardingViewController onboardingVC = onboardingStoryboard.InstantiateViewController("OnboardingViewController") as OnboardingViewController;
            onboardingVC.onboardingEnum = _onboardingEnum;
            onboardingVC.isToUpdateMobileNo = isToUpdateMobileNo;
            onboardingVC.isLogin = isLogin;
            onboardingVC.model = model;
            onboardingVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            onboardingVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(onboardingVC, true, null);
            UpdateVersionUpdate();
        }

        internal void ShowPrelogin()
        {
            UIStoryboard loginStoryboard = UIStoryboard.FromName("Login", null);
            UIViewController preloginVC = (UIViewController)loginStoryboard.InstantiateViewController("PreloginViewController");
            preloginVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            preloginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(preloginVC, true, null);
        }

        internal void ShowDashboard()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
            loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            ShowViewController(loginVC, this);
        }

        internal void ExecuteLaunch()
        {
            if (_onboardingEnum == OnboardingEnum.AppUpdate)
            {
                GetAccountsForAppUpdate();
            }
            else if (_onboardingEnum == OnboardingEnum.FreshInstall)
            {
                ProcessOnboarding(_onboardingEnum);
            }
            else
            {
                ProceedOnNormalLaunch();
                UpdateVersionUpdate();
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

        private void ProcessOnboarding(OnboardingEnum onboardingEnum, bool isToUpdateMobileNo = false)
        {
            List<OnboardingItemModel> onboardingData = GetOnboardingData(onboardingEnum);
            if (onboardingData != null &&
                onboardingData.Count > 0)
            {
                ShowOnboarding(onboardingData, isToUpdateMobileNo);
            }
        }

        private List<OnboardingItemModel> GetOnboardingData(OnboardingEnum onboardingEnum)
        {
            List<OnboardingItemModel> onboardingData = new List<OnboardingItemModel>();
            try
            {
                string jsonFilename = onboardingEnum == OnboardingEnum.FreshInstall ? "JSON/FreshInstallOnboarding.json" : "JSON/AppUpdateOnboarding.json";
                string dataJson = File.ReadAllText(jsonFilename);
                OnboardingResponseModel respModel = JsonConvert.DeserializeObject<OnboardingResponseModel>(dataJson);
                if (AppLaunchMasterCache.IsRewardsDisabled)
                {
                    int rIndx = respModel.Data.FindIndex(x => x.ID == "5");
                    if (rIndx > -1)
                    {
                        respModel.Data.RemoveAt(rIndx);
                    }

                    int iIndx = respModel.Data.FindIndex(x => x.ID == "6");
                    if (iIndx > -1)
                    {
                        respModel.Data.RemoveAt(iIndx);
                    }
                }
                else
                {
                    int iIndx = respModel.Data.FindIndex(x => x.ID == "7");
                    if (iIndx > -1)
                    {
                        respModel.Data.RemoveAt(iIndx);
                    }
                }
                onboardingData = respModel?.Data;
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR: " + e.Message);
            }
            return onboardingData;
        }

        internal void ShowOnboardingWithNormalLaunch()
        {
            InvokeInBackground(async () =>
            {
                bool isPhoneVerified = await GetPhoneVerificationStatus();
                InvokeOnMainThread(() =>
                {
                    if (isPhoneVerified)
                    {
                        ExecuteGetCustomerRecordsCall(true);
                    }
                    else
                    {
                        ProcessOnboarding(_onboardingEnum, true);
                    }
                });
            });
        }

        internal void ProceedOnNormalLaunch()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var isLogin = sharedPreference.BoolForKey(TNBGlobal.PreferenceKeys.LoginState);
            if (isLogin && DataManager.DataManager.SharedInstance.UserEntity != null && DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
            {
                DataManager.DataManager.SharedInstance.User.UserID = DataManager.DataManager.SharedInstance.UserEntity[0]?.userID;

                InvokeInBackground(async () =>
                {
                    bool isPhoneVerified = await GetPhoneVerificationStatus();
                    InvokeOnMainThread(() =>
                    {
                        if (isPhoneVerified)
                        {
                            ExecuteGetCustomerRecordsCall(false);
                        }
                        else
                        {
                            ShowUpdateMobileNumber(true);
                        }
                    });
                });
            }
            else
            {
                ShowPrelogin();
            }
        }

        internal void ExecuteGetCustomerRecordsCall(bool forGetAccountsFailed = false)
        {
            UserAccountsEntity uaManager = new UserAccountsEntity();
            DataManager.DataManager.SharedInstance.AccountRecordsList = uaManager.GetCustomerAccountRecordList();
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                       && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null
                       && DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0)
            {
                DataManager.DataManager.SharedInstance.AccountRecordsList.d = DataManager.DataManager.SharedInstance.GetCombinedAcctList();
                DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList.d[0];

                if (forGetAccountsFailed)
                {
                    ProcessOnboarding(_onboardingEnum);
                }
                else
                {
                    ShowDashboard();
                }
            }
            else if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
              && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null
              && DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count == 0)
            {
                if (forGetAccountsFailed)
                {
                    ProcessOnboarding(_onboardingEnum);
                }
                else
                {
                    ShowDashboard();
                }
            }
            else
            {
                DataManager.DataManager.SharedInstance.ClearLoginState();
                if (forGetAccountsFailed)
                {
                    ProcessOnboarding(_onboardingEnum);
                }
                else
                {
                    ShowPrelogin();
                }
            }
        }

        /// <summary>
        /// Checks if the phone is verified.
        /// </summary>
        /// <returns>The phone is verified.</returns>
        private async Task<bool> GetPhoneVerificationStatus()
        {
            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
            bool isVerified = sharedPreference.BoolForKey(TNBGlobal.PreferenceKeys.PhoneVerification);

            if (!isVerified)
            {
                PhoneVerificationStatusResponseModel response = await ServiceCall.GetPhoneVerificationStatus();
                if (response != null && response.d != null && response.d.IsSuccess)
                {
                    isVerified = response.d.data.IsVerified;

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
            UpdateMobileNoViewController viewController = new UpdateMobileNoViewController()
            {
                WillHideBackButton = willHideBackButton,
                IsFromLogin = true
            };
            UINavigationController navController = new UINavigationController(viewController)
            {
                ModalPresentationStyle = UIModalPresentationStyle.FullScreen
            };
            PresentViewController(navController, true, null);
            ActivityIndicator.Hide();
        }
    }
}