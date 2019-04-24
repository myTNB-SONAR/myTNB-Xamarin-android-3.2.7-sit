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

namespace myTNB
{
    public partial class AppLaunchViewController : UIViewController
    {
        //For future reference, don't delete
        //UIView containerView;
        UIImageView imgViewAppLaunch;
        BillingAccountDetailsResponseModel _billingAccountDetailsList = new BillingAccountDetailsResponseModel();
        string _imageSize = string.Empty;
        public AppLaunchViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            imgViewAppLaunch = new UIImageView(UIImage.FromBundle("App-Launch-Gradient"));
            var imgViewLogo = new UIImageView(UIImage.FromBundle("New-Launch-Logo"));
            //var imgViewLogoTitle = new UIImageView(UIImage.FromBundle("Logo-Title"));
            //var imgViewTagline = new UIImageView(UIImage.FromBundle("Tagline"));
            //containerView = new UIView();

            View.AddSubview(imgViewAppLaunch);
            View.AddSubview(imgViewLogo);
            //View.AddSubview(imgViewLogoTitle);
            //View.AddSubview(imgViewTagline);
            //View.AddSubview(containerView);  

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

            //imgViewLogoTitle.AtTopOf(imgViewLogo, 100),
            //imgViewLogoTitle.WithSameCenterX(View),
            //imgViewLogoTitle.Height().EqualTo(42),
            //imgViewLogoTitle.Width().EqualTo(198),

            //imgViewTagline.AtTopOf(imgViewLogo, 130),
            //imgViewTagline.WithSameCenterX(View),
            //imgViewTagline.Height().EqualTo(25),
            //imgViewTagline.Width().EqualTo(113)
            //containerView.AtTopOf(View, 0),
            //containerView.AtBottomOf(View, 0),
            //containerView.AtLeftOf(View, 0),
            //containerView.AtRightOf(View, 0)
            );
            //Create DB
            SQLiteHelper.CreateDB();
            CreateCacheTables();

            // clear cache data on App Launch
            BillingAccountEntity.DeleteTable();
            PaymentHistoryEntity.DeleteTable();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
        }

        internal void SetupSuperViewBackground()
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;

            var gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor },
                Locations = new NSNumber[] { 0.0, 1.5 },
                Frame = View.Bounds,
                Opaque = false
            };
            //containerView.Layer.InsertSublayer(gradientLayer, 0);
            //imgViewAppLaunch.AddSubview(containerView);
            //imgViewAppLaunch.BringSubviewToFront(containerView);
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
#if true
                        GetUserEntity();
                        await LoadMasterData();
#else
                        Task[] taskList = new Task[] {
                            GetWebLinks()
                            , GetLocationTypes()
                            , GetStatesForFeedback()
                            , GetFeedbackCategory()
                            , GetOtherFeedbackType()
                            , PushNotificationHelper.GetAppNotificationTypes()
                        };
                        Task.WaitAll(taskList);
#endif
                        if (!IsAppUpdateRequired())
                        {
                            ExecuteSiteCoreCall();
                        }
                        else
                        {
                            // show force update
                            UIStoryboard storyBoard = UIStoryboard.FromName("Onboarding", null);
                            var viewController =
                                storyBoard.InstantiateViewController("AppUpdateViewController") as AppUpdateViewController;
                            var navController = new UINavigationController(viewController);
                            navController.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
                            navController.SetNavigationBarHidden(true, false);
                            PresentViewController(navController, false, null);
                            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                        }
                    }
                    else
                    {
                        UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                        UserAccountsEntity uaManager = new UserAccountsEntity();
                        CustomerAccountRecordListModel accountRecords = uaManager.GetCustomerAccountRecordList();
                        if (accountRecords != null && accountRecords?.d != null)
                        {
                            DataManager.DataManager.SharedInstance.AccountRecordsList = accountRecords;
                            if (accountRecords.d.Count > 0)
                            {
                                DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList.d[0];
                            }
                        }
                        var sharedPreference = NSUserDefaults.StandardUserDefaults;

                        var isLogin = sharedPreference.BoolForKey(TNBGlobal.PreferenceKeys.LoginState);
                        var shouldUpdateDb = IsDbUpdateNeeded();
                        if (isLogin && !shouldUpdateDb && DataManager.DataManager.SharedInstance.UserEntity != null && DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
                        {
                            DataManager.DataManager.SharedInstance.User.UserID = DataManager.DataManager.SharedInstance.UserEntity[0].userID;
                            ShowDashboard();
                        }
                        else
                        {
                            if (shouldUpdateDb)
                            {
                                DataManager.DataManager.SharedInstance.ClearLoginState();
                            }
                            ShowPrelogin();
                        }
                        UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                    }
                });
            });
        }

        /// <summary>
        /// Checks if app update is required.
        /// </summary>
        /// <returns><c>true</c>, if app update required was ised, <c>false</c> otherwise.</returns>
        private bool IsAppUpdateRequired()
        {
            bool res = false;
            if (!string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.LatestAppVersion))
            {
                // if latest app version is higher
                res = string.CompareOrdinal(DataManager.DataManager.SharedInstance.LatestAppVersion, AppVersionHelper.GetAppShortVersion()) > 0;
            }
            return res;
        }

        /// <summary>
        /// Loads the master data.
        /// </summary>
        /// <returns>The master data.</returns>
        private async Task LoadMasterData()
        {
            var response = await ServiceCall.GetAppLaunchMasterData();
            if (response.didSucceed)
            {
                var data = response.data;

                var iOSIndex = data?.AppVersions?.FindIndex(x => x.IsIos) ?? -1;
                DataManager.DataManager.SharedInstance.LatestAppVersion = (iOSIndex > -1) ? data.AppVersions[iOSIndex].Version : string.Empty;

                DataManager.DataManager.SharedInstance.SystemStatus = data?.SystemStatus ?? new List<DowntimeDataModel>();
                DataManager.DataManager.SharedInstance.SetSystemsAvailability();

                DataManager.DataManager.SharedInstance.WebLinks = data?.WebLinks ?? new List<WebLinksDataModel>();

                DataManager.DataManager.SharedInstance.LocationTypes = data?.LocationTypes ?? new List<LocationTypeDataModel>();
                if (data?.LocationTypes != null)
                {
                    LocationTypeDataModel allLocationModel = new LocationTypeDataModel();
                    allLocationModel.Id = "all";
                    allLocationModel.Title = "Common_All".Translate();
                    allLocationModel.Description = "Common_All".Translate();
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
                var shouldUpdateDb = IsDbUpdateNeeded();
                if (isLogin && !shouldUpdateDb
                    && DataManager.DataManager.SharedInstance.UserEntity != null && DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
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
                    if (shouldUpdateDb)
                    {
                        DataManager.DataManager.SharedInstance.ClearLoginState();
                    }
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
            DataManager.DataManager.SharedInstance.CreateDuesTable();
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

        internal void ExecuteGetBillAccountDetailsCall()
        {
            GetBillingAccountDetails().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_billingAccountDetailsList != null && _billingAccountDetailsList?.d != null
                        && _billingAccountDetailsList?.d?.data != null && _billingAccountDetailsList?.d?.didSucceed == true)
                    {
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = _billingAccountDetailsList.d.data;
                        if (!DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount)
                        {
                            DataManager.DataManager.SharedInstance.SaveToBillingAccounts(DataManager.DataManager.SharedInstance.BillingAccountDetails,
                                                                                         DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                        }
                        ShowDashboard();
                        UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.ClearLoginState();
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = new BillingAccountDetailsDataModel();
                        string errorMessage = _billingAccountDetailsList?.d?.message ?? "Error_DefaultMessage".Translate();
                        var alert = UIAlertController.Create("Error_DefaultTitle".Translate(), errorMessage, UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Common_Ok".Translate(), UIAlertActionStyle.Cancel, (obj) =>
                        {
                            ShowPrelogin();
                        }));
                        PresentViewController(alert, animated: true, completionHandler: null);
                    }
                    UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                });
            });
        }

        internal Task GetBillingAccountDetails()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    CANum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum
                };
                _billingAccountDetailsList = serviceManager.GetBillingAccountDetails("GetBillingAccountDetails", requestParameter);
            });
        }

        Task GetWebLinks()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID
                };
                var response = serviceManager.GetWebLinks("GetWebLinks", requestParameter);
                DataManager.DataManager.SharedInstance.WebLinks = response?.d?.data;
            });
        }

        Task GetLocationTypes()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID
                };
                var response = serviceManager.GetLocationTypes("GetLocationTypes", requestParameter);
                DataManager.DataManager.SharedInstance.LocationTypes = response?.d?.data;
                LocationTypeDataModel allLocationModel = new LocationTypeDataModel();
                allLocationModel.Id = "all";
                allLocationModel.Title = "Common_All".Translate();
                allLocationModel.Description = "Common_All".Translate();
                if (DataManager.DataManager.SharedInstance.LocationTypes != null)
                {
                    DataManager.DataManager.SharedInstance.LocationTypes.Insert(0, allLocationModel);
                }
            });
        }

        Task GetStatesForFeedback()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID
                };
                var response = serviceManager.GetStatesForFeedback("GetStatesForFeedback", requestParameter);
                DataManager.DataManager.SharedInstance.StatesForFeedBack = response?.d?.data;
            });
        }

        Task GetFeedbackCategory()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID
                };
                var response = serviceManager.GetFeedbackCategory("GetFeedbackCategory", requestParameter);
                DataManager.DataManager.SharedInstance.FeedbackCategory = response?.d?.data;
            });
        }

        Task GetOtherFeedbackType()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID
                };
                var response = serviceManager.GetOtherFeedbackType("GetOtherFeedbackType", requestParameter);
                DataManager.DataManager.SharedInstance.OtherFeedbackType = response?.d?.data;
            });
        }


        /// <summary>
        /// Checks if db update is needed.
        /// </summary>
        /// <returns><c>true</c>, if db update needed was ised, <c>false</c> otherwise.</returns>
        public bool IsDbUpdateNeeded()
        {
#if true
            // temporary turn off
            return false;
#else
            bool res = true;
            string versionOfLastRunKey = "VersionOfLastRun";
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var versionOfLastRun = sharedPreference.StringForKey(versionOfLastRunKey);
            var currVersion = AppVersionHelper.GetAppShortVersion();

            if(!string.IsNullOrEmpty(versionOfLastRun))
            {
                res = string.CompareOrdinal(versionOfLastRun, currVersion) != 0;
            }

            sharedPreference.SetString(currVersion, versionOfLastRunKey);
            sharedPreference.Synchronize();

            return res;
#endif
        }
    }
}