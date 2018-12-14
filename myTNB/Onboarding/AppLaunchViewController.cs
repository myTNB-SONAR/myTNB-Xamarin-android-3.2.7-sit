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
            var imgViewLogo = new UIImageView(UIImage.FromBundle("Logo"));
            var imgViewLogoTitle = new UIImageView(UIImage.FromBundle("Logo-Title"));
            var imgViewTagline = new UIImageView(UIImage.FromBundle("Tagline"));
            //containerView = new UIView();

            View.AddSubview(imgViewAppLaunch);
            View.AddSubview(imgViewLogo);
            View.AddSubview(imgViewLogoTitle);
            View.AddSubview(imgViewTagline);
            //View.AddSubview(containerView);  

            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            View.AddConstraints(
                imgViewAppLaunch.AtTopOf(View, 0),
                imgViewAppLaunch.AtBottomOf(View, 0),
                imgViewAppLaunch.AtLeftOf(View, 0),
                imgViewAppLaunch.AtRightOf(View, 0),

                imgViewLogo.AtTopOf(View, 200),
                imgViewLogo.WithSameCenterX(View),
                imgViewLogo.Height().EqualTo(120),
                imgViewLogo.Width().EqualTo(120),

                imgViewLogoTitle.AtTopOf(imgViewLogo, 100),
                imgViewLogoTitle.WithSameCenterX(View),
                imgViewLogoTitle.Height().EqualTo(42),
                imgViewLogoTitle.Width().EqualTo(198),

                imgViewTagline.AtTopOf(imgViewLogo, 130),
                imgViewTagline.WithSameCenterX(View),
                imgViewTagline.Height().EqualTo(25),
                imgViewTagline.Width().EqualTo(113)
            //containerView.AtTopOf(View, 0),
            //containerView.AtBottomOf(View, 0),
            //containerView.AtLeftOf(View, 0),
            //containerView.AtRightOf(View, 0)
            );
            //Create DB
            SQLiteHelper.CreateDB();

        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
        }

        internal void SetupSuperViewBackground()
        {
            var startColor = myTNBColor.GradientPurpleDarkElement();
            var endColor = myTNBColor.GradientPurpleLightElement();

            var gradientLayer = new CAGradientLayer();
            gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
            gradientLayer.Locations = new NSNumber[] { 0.0, 1.5 };
            gradientLayer.Frame = View.Bounds;
            gradientLayer.Opaque = false;
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
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        Task[] taskList = new Task[] {
                            GetWebLinks()
                            , GetLocationTypes()
                            , GetStatesForFeedback()
                            , GetFeedbackCategory()
                            , GetOtherFeedbackType()
                            , PushNotificationHelper.GetAppNotificationTypes()
                        };
                        Task.WaitAll(taskList);
                        ExecuteSiteCoreCall();
                    }
                    else
                    {
                        Console.WriteLine("No Network");
                        UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                        UserAccountsEntity uaManager = new UserAccountsEntity();
                        CustomerAccountRecordListModel accountRecords = uaManager.GetCustomerAccountRecordList();
                        if (accountRecords != null && accountRecords.d != null)
                        {
                            DataManager.DataManager.SharedInstance.AccountRecordsList = accountRecords;
                            if (accountRecords.d.Count > 0)
                            {
                                DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList.d[0];
                            }
                        }
                        var sharedPreference = NSUserDefaults.StandardUserDefaults;
                        GetUserEntity();
                        var isLogin = sharedPreference.BoolForKey("isLogin");
                        var shouldUpdateDb = IsDbUpdateNeeded();
                        if (isLogin && !shouldUpdateDb && DataManager.DataManager.SharedInstance.UserEntity != null && DataManager.DataManager.SharedInstance.UserEntity.Count > 0)
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
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var isLogin = sharedPreference.BoolForKey("isLogin");
            if(isLogin){
                PushNotificationHelper.GetNotifications();
            }
            DataManager.DataManager.SharedInstance.CreateUsageHistoryTable();
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
            ShowViewController(loginVC, this);
        }

        internal void ExecuteSiteCoreCall()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var isWalkthroughDone = sharedPreference.BoolForKey("isWalkthroughDone");
            GetUserEntity();
            _imageSize = DeviceHelper.GetImageSize((int)View.Frame.Width);
            GetWalkthroughScreens().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (isWalkthroughDone)
                    {
                        var isLogin = sharedPreference.BoolForKey("isLogin");
                        var shouldUpdateDb = IsDbUpdateNeeded();
                        if (isLogin && !shouldUpdateDb 
                            && DataManager.DataManager.SharedInstance.UserEntity != null && DataManager.DataManager.SharedInstance.UserEntity.Count > 0)
                        {
                            DataManager.DataManager.SharedInstance.User.UserID = DataManager.DataManager.SharedInstance.UserEntity[0].userID;
                            ExecuteGetCutomerRecordsCall();
                        }
                        else
                        {
                            if(shouldUpdateDb)
                            {
                                DataManager.DataManager.SharedInstance.ClearLoginState();
                            }
                            ShowPrelogin();
                            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                        }
                    }
                    else
                    {
                        ShowOnboarding();
                        UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                    }
                });
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

        internal void DisplayAlertView(string title, string message)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
        }

        internal void ExecuteGetCutomerRecordsCall()
        {
            UserAccountsEntity uaManager = new UserAccountsEntity();
            DataManager.DataManager.SharedInstance.AccountRecordsList = uaManager.GetCustomerAccountRecordList();
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                       && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null
                       && DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count > 0)
            {
                DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList.d[0];
                ExecuteGetBillAccountDetailsCall();
            }
            else if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
              && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null
              && DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count == 0)
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

        internal void ExecuteGetBillAccountDetailsCall()
        {
            GetBillingAccountDetails().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_billingAccountDetailsList != null && _billingAccountDetailsList.d != null
                       && _billingAccountDetailsList.d.data != null)
                    {
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = _billingAccountDetailsList.d.data;
                        ShowDashboard();
                        UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.ClearLoginState();
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = new BillingAccountDetailsDataModel();
                        var alert = UIAlertController.Create("Error in fetching account details.", "There is an error in the server, please login again.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
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
                DataManager.DataManager.SharedInstance.WebLinks = serviceManager.GetWebLinks("GetWebLinks", requestParameter);
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
                DataManager.DataManager.SharedInstance.LocationTypes = serviceManager.GetLocationTypes("GetLocationTypes", requestParameter);
                LocationTypeDataModel allLocationModel = new LocationTypeDataModel();
                allLocationModel.Id = "all";
                allLocationModel.Title = "All";
                allLocationModel.Description = "All";
                if (DataManager.DataManager.SharedInstance.LocationTypes != null
                   && DataManager.DataManager.SharedInstance.LocationTypes.d != null
                   && DataManager.DataManager.SharedInstance.LocationTypes.d.data != null)
                {
                    DataManager.DataManager.SharedInstance.LocationTypes.d.data.Insert(0, allLocationModel);
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
                DataManager.DataManager.SharedInstance.StatesForFeedBack = serviceManager.GetStatesForFeedback("GetStatesForFeedback", requestParameter);
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
                DataManager.DataManager.SharedInstance.FeedbackCategory = serviceManager.GetFeedbackCategory("GetFeedbackCategory", requestParameter);
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
                DataManager.DataManager.SharedInstance.OtherFeedbackType = serviceManager.GetOtherFeedbackType("GetOtherFeedbackType", requestParameter);
            });
        }


        /// <summary>
        /// Checks if db update is needed.
        /// </summary>
        /// <returns><c>true</c>, if db update needed was ised, <c>false</c> otherwise.</returns>
        public bool IsDbUpdateNeeded()
        {
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
        }
    }
}