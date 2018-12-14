using Foundation;
using UIKit;
using System;
using CoreGraphics;
using CoreAnimation;
using UserNotifications;
using Firebase.CloudMessaging;
using System.Text.RegularExpressions;
using myTNB.Dashboard;
using Facebook.CoreKit;
using Firebase.Core;
using Firebase.Crashlytics;

namespace myTNB
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.

    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {
        // class-level declarations
        public event EventHandler<UserInfoEventArgs> NotificationReceived;
        public override UIWindow Window
        {
            get;
            set;
        }

        public MakePaymentViewController _makePaymentVC;
        public SelectBillsViewController _selectBillsVC;
        public DashboardViewController _dashboardVC;

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var baseVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = GetTopViewController(baseVc);

            if (url.ToString().Contains("mytnbapp://action=recipt&transid") && _dashboardVC != null)
            {
                string absoluteURL = url.ToString();
                Regex regex = new Regex("\\btransid.*\\b");
                Match match = regex.Match(absoluteURL);
                if (match.Success)
                {
                    string transID = match.Value.Replace("transid=", "");
                    UIStoryboard storyBoard = UIStoryboard.FromName("Receipt", null);
                    ReceiptViewController viewController =
                        storyBoard.InstantiateViewController("ReceiptViewController") as ReceiptViewController;
                    viewController.MerchatTransactionID = transID;
                    viewController.isCCFlow = false;
                    var navController = new UINavigationController(viewController);

                    if(!(topVc is ReceiptViewController))
                    {
                        topVc?.PresentViewController(navController, true, null);
                    }

                }
            }

            if (url.ToString().Contains("mytnbapp://action=payoptions"))
            {
                if (!(topVc is SelectPaymentMethodViewController))
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("Payment", null);
                    SelectPaymentMethodViewController viewController =
                        storyBoard.InstantiateViewController("SelectPaymentMethodViewController") as SelectPaymentMethodViewController;
                    var navController = new UINavigationController(viewController);

                    if (_selectBillsVC != null)
                    {
                        viewController.AccountsForPayment = _selectBillsVC._accountsForPayment;
                        viewController.TotalAmount = _selectBillsVC._lblTotalAmountValue.Text;
                        _selectBillsVC.PresentViewController(navController, true, null);
                    }
                    else if (_dashboardVC != null)
                    {
                        _dashboardVC.PresentViewController(navController, true, null);
                    }
                }
            }

            if (url.ToString().Contains("mytnbapp://action=dashboard"))
            {
                if (_makePaymentVC != null){
                    _makePaymentVC.DismissViewController(true, null);
                }
                if(_selectBillsVC != null){
                    _selectBillsVC.DismissViewController(true, null);
                }

                topVc = GetTopViewController(baseVc);
                if (!(topVc is DashboardHomeViewController) && !(topVc is DashboardViewController))
                {
                    ViewHelper.DismissControllersAndSelectTab(topVc, 0, true, true);
                }
            }

            if (url.ToString().Contains("rating"))
            {
                if (!(topVc is RatingViewController))
                {
                    string rateString = default(string);
                    string transId = default(string);
                    var paramsStr = url.Host;

                    var parameters = paramsStr.Split('&');
                    foreach (var pair in parameters)
                    {
                        var item = pair.Split('=');
                        if (item.Length == 2)
                        {
                            var key = item[0];
                            if (key == "rating")
                            {
                                rateString = item[1];
                            }
                            else if (key == "transid")
                            {
                                transId = item[1];
                            }
                        }
                    }

                    UIStoryboard storyBoard = UIStoryboard.FromName("Rating", null);
                    RatingViewController viewController =
                        storyBoard.InstantiateViewController("RatingViewController") as RatingViewController;
                    viewController.Rating = !string.IsNullOrEmpty(rateString) ? int.Parse(rateString) : 0;
                    viewController.TransId = transId;


                    var navController = new UINavigationController(viewController);
                    topVc?.PresentViewController(navController, true, null);
                }

            }

            bool fbHandled = ApplicationDelegate.SharedInstance.OpenUrl(app, url, options);
            if (fbHandled)
            {
                return true;
            }

            return true;
        }

        /// <summary>
        /// Gets the top view controller.
        /// </summary>
        /// <returns>The top view controller.</returns>
        /// <param name="baseVc">Base vc.</param>
        public static UIViewController GetTopViewController(UIViewController baseVc)
        {
            UIViewController topVc;
            if (baseVc is UINavigationController)
            {
                var nav = baseVc as UINavigationController;
                topVc = nav.VisibleViewController;
            }
            else if (baseVc is UITabBarController)
            {
                var nav = baseVc as UITabBarController;
                topVc = nav.SelectedViewController;
            }
            else if (baseVc?.PresentedViewController != null)
            {
                topVc = baseVc?.PresentedViewController;
            }
            else
            {
                return baseVc;
            }

            return GetTopViewController(topVc);
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            DataManager.DataManager.SharedInstance.FCMToken = sharedPreference.StringForKey("FCMToken");
            DataManager.DataManager.SharedInstance.UDID = UIDevice.CurrentDevice.IdentifierForVendor.ToString();
            SetupNavigationBar();
           
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => {
                    Console.WriteLine("isGranted: " + granted);
                    DataManager.DataManager.SharedInstance.IsRegisteredForRemoteNotification = granted;
                });

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;

                // For iOS 10 data message (sent via FCM)
                Messaging.SharedInstance.Delegate = this;
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }
            PushNotificationHelper.RegisterDevice();
            Crashlytics.Configure();
            ApplicationDelegate.SharedInstance.FinishedLaunching(application, launchOptions);

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        
            //Messaging.SharedInstance.Disconnect();
            Messaging.SharedInstance.ShouldEstablishDirectChannel = false;
            Console.WriteLine("Disconnected from FCM");
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
            //connectFCM();
            PushNotificationHelper.ConnectToFCM();
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        public static nfloat GetStatusBarHeight()
        {
          var statusBarSize = UIApplication.SharedApplication.StatusBarFrame.Size;
          return (nfloat)Math.Min(statusBarSize.Height, statusBarSize.Width);
        }
       
        internal void SetupNavigationBar()
        {
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);

            //Set the default frame of Navigation Bar
            var navigationBarFrame = new CGRect(0,0, UIScreen.MainScreen.Bounds.Size.Width, 64);

            //Setup the colors that will be use
            var startColor = myTNBColor.GradientPurpleDarkElement();
            var endColor = myTNBColor.GradientPurpleLightElement();

            //Create an instance of gradient layer with custom setup
            var gradientLayer = new CAGradientLayer
            {
              Frame =
                    new CGRect(navigationBarFrame.X, navigationBarFrame.Y, navigationBarFrame.Width,
                       navigationBarFrame.Height + AppDelegate.GetStatusBarHeight()),
             Colors = new CGColor[] { startColor.CGColor, endColor.CGColor },
               StartPoint = new CGPoint(x: 0.0, y: 0.5),
              EndPoint = new CGPoint(x: 1.0, y: 0.5)
            } ;

            // Render the gradient to UIImage
            UIGraphics.BeginImageContext(gradientLayer.Bounds.Size);
            gradientLayer.RenderInContext(UIGraphics.GetCurrentContext());
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            //Setup the Navigation Bar background
            UINavigationBar.Appearance.SetBackgroundImage(image, UIBarMetrics.Default);

            //Setup the Navigation Bar title
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
            {
                TextColor = UIColor.White,
                Font = myTNBFont.MuseoSans16()
            });

            //Setup the Navigation Bar tint color 
            UINavigationBar.Appearance.TintColor = UIColor.White;
        }

        #region FCM
        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {

        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // Do your magic to handle the notification data
            Messaging.SharedInstance.AppDidReceiveMessage(userInfo);
            Console.WriteLine(userInfo);

            if (DataManager.DataManager.SharedInstance.IsLoggedIn())
            {
                DataManager.DataManager.SharedInstance.IsFromPushNotification = true;
            }

            Console.WriteLine("debug: DidReceiveRemoteNotification");

        }

        // To receive notifications in foreground on iOS 10 devices.
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do your magic to handle the notification data
            Console.WriteLine(notification?.Request?.Content?.UserInfo);
            if (DataManager.DataManager.SharedInstance.IsLoggedIn())
            {
                DataManager.DataManager.SharedInstance.IsFromPushNotification = true;
            }
            Console.WriteLine("debug: WillPresentNotification");

        }

        // Receive data message on iOS 10 devices.
        public void ApplicationReceivedRemoteMessage(RemoteMessage remoteMessage)
        {
            Console.WriteLine(remoteMessage?.AppData);
            if (DataManager.DataManager.SharedInstance.IsLoggedIn())
            {
                DataManager.DataManager.SharedInstance.IsFromPushNotification = true;
            }
            Console.WriteLine("debug: ApplicationReceivedRemoteMessage");

        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            Console.WriteLine(response?.Notification?.Request?.Content?.UserInfo);
            if (DataManager.DataManager.SharedInstance.IsLoggedIn())
            {
                DataManager.DataManager.SharedInstance.IsFromPushNotification = true;
            }
            Console.WriteLine("debug: DidReceiveNotificationResponse");

            completionHandler?.Invoke();
        }

        #endregion
    }
}