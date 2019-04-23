using Foundation;
using System;
using UIKit;
using CoreGraphics;
using CoreAnimation;

namespace myTNB
{
    public partial class PreloginViewController : UIViewController
    {
        public PreloginViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetBackgroundView();
            SetSubviews();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        void SetBackgroundView()
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            var gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor },
                Locations = new NSNumber[] { 0, 1 },
                Frame = View.Bounds
            };
            View.Layer.InsertSublayer(gradientLayer, 0);
        }

        void SetSubviews()
        {
            UIImageView imgLogo = new UIImageView(new CGRect((View.Frame.Width / 2) - DeviceHelper.GetScaledWidth(22)
                , DeviceHelper.GetScaledSize(6.3F), DeviceHelper.GetScaledWidth(44), DeviceHelper.GetScaledHeight(44)))
            {
                Image = UIImage.FromBundle("app_logo")
            };

            UIImageView imgPrelogin = new UIImageView(new CGRect((View.Frame.Width / 2) - DeviceHelper.GetScaledWidth(75)
                , DeviceHelper.GetScaledSize(16.9F), DeviceHelper.GetScaledWidth(150), DeviceHelper.GetScaledHeight(100)))
            {
                Image = UIImage.FromBundle("IC-Display-Prelogin")
            };

            UILabel lblbWelcome = new UILabel(new CGRect(0, DeviceHelper.GetScaledSize(35.6F), View.Frame.Width, 24))
            {
                Text = "Login_Welcome".Translate(),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.SunGlow,
                Font = MyTNBFont.MuseoSans24
            };

            UILabel lblSubtitle = new UILabel(new CGRect(0, DeviceHelper.GetScaledSize(40.5F), View.Frame.Width, 24))
            {
                Text = "Login_ManageYourAccount".Translate(),
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Font = MyTNBFont.MuseoSans16
            };

            UIView viewCTA = new UIView(new CGRect(18, DeviceHelper.GetScaledSize(47.5F), View.Frame.Width - 36, 48));

            UIButton btnRegister = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(0, 0, (viewCTA.Frame.Width - 4) / 2, 48)
            };
            btnRegister.SetTitle("Common_Register".Translate(), UIControlState.Normal);
            btnRegister.Layer.CornerRadius = 4.0f;
            btnRegister.Layer.BorderWidth = 1.0f;
            btnRegister.Layer.BorderColor = UIColor.White.CGColor;
            btnRegister.BackgroundColor = UIColor.Clear;
            btnRegister.TouchUpInside += (sender, e) =>
            {
                OnRegister();
            };

            UIButton btnLogin = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(((viewCTA.Frame.Width) / 2) + 2, 0, (viewCTA.Frame.Width - 4) / 2, 48)
            };
            btnLogin.SetTitle("Login_Login".Translate(), UIControlState.Normal);
            btnLogin.Layer.CornerRadius = 4.0f;
            btnLogin.Layer.BorderWidth = 1.0f;
            btnLogin.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnLogin.BackgroundColor = MyTNBColor.FreshGreen;
            btnLogin.TouchUpInside += (sender, e) =>
            {
                OnLogin();
            };

            viewCTA.AddSubviews(new UIView[] { btnRegister, btnLogin });

            UIView viewLine = new UIView(new CGRect(18, DeviceHelper.GetScaledSize(58.8F), View.Frame.Width - 36, 1))
            {
                BackgroundColor = UIColor.FromRGBA(255, 255, 255, 50)
            };

            UILabel lblQuickAccess = new UILabel(new CGRect(18, DeviceHelper.GetScaledSize(62F), View.Frame.Width - 36, 18))
            {
                Text = "Login_QuickAccess".Translate(),
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Font = MyTNBFont.MuseoSans16
            };

            UIView viewQuickAccess = new UIView(new CGRect(0, DeviceHelper.GetScaledSize(66.2F)
                , View.Frame.Width, DeviceHelper.IsIphone4() ? 160 : 168));

            UIView viewFindUs = new UIView(new CGRect(18, 0
                , viewQuickAccess.Frame.Width - 36, DeviceHelper.IsIphone4() ? 42 : 44));
            viewFindUs.Layer.CornerRadius = 5F;
            viewFindUs.BackgroundColor = UIColor.White;

            UIImageView imgFindUs = new UIImageView(new CGRect(10, 10, 24, 24))
            {
                Image = UIImage.FromBundle("Locate-Blue")
            };

            UILabel lblFindUs = new UILabel(new CGRect(50, 10, viewFindUs.Frame.Width - 50, 24))
            {
                Text = "Login_FindUs".Translate(),
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans14
            };

            viewFindUs.AddSubviews(new UIView[] { imgFindUs, lblFindUs });

            UIView viewFeedback = new UIView(new CGRect(18, DeviceHelper.IsIphone4() ? 48 : 50
                , viewQuickAccess.Frame.Width - 36, DeviceHelper.IsIphone4() ? 42 : 44));
            viewFeedback.Layer.CornerRadius = 5F;
            viewFeedback.BackgroundColor = UIColor.White;

            UIImageView imgFeedback = new UIImageView(new CGRect(10, 10, 24, 24))
            {
                Image = UIImage.FromBundle("Feedback-Blue")
            };

            UILabel lblFeedback = new UILabel(new CGRect(50, 10, viewFindUs.Frame.Width - 50, 24))
            {
                Text = "Login_Feedback".Translate(),
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans14
            };

            viewFeedback.AddSubviews(new UIView[] { imgFeedback, lblFeedback });

            UIView viewCallUs = new UIView(new CGRect(18, DeviceHelper.IsIphone4() ? 96 : 100
                , viewQuickAccess.Frame.Width - 36, DeviceHelper.IsIphone4() ? 42 : 44));
            viewCallUs.Layer.CornerRadius = 5F;
            viewCallUs.BackgroundColor = UIColor.White;

            UIImageView imgCallUs = new UIImageView(new CGRect(10, 10, 24, 24))
            {
                Image = UIImage.FromBundle("Call")
            };

            UILabel lblCallUs = new UILabel(new CGRect(50, 10, viewFindUs.Frame.Width - 50, 24))
            {
                Text = "Login_CallUs".Translate(),
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans14
            };

            if (DataManager.DataManager.SharedInstance.WebLinks != null)
            {
                int index = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbcl"));
                if (index > -1)
                {
                    lblCallUs.Text = DataManager.DataManager.SharedInstance.WebLinks[index].Title;
                }
            }


            viewCallUs.AddSubviews(new UIView[] { imgCallUs, lblCallUs });

            UILabel lblDetails = new UILabel(new CGRect(18, DeviceHelper.IsIphone4() ? 144 : 150
                 , viewQuickAccess.Frame.Width - 36, 18))
            {
                Text = "Login_Note".Translate(),
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Font = MyTNBFont.MuseoSans12
            };

            viewFindUs.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                GoToFindUs();
            }));

            viewFeedback.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                GoToFeedback();
            }));

            viewCallUs.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                CallCustomerService();
            }));

            viewQuickAccess.AddSubviews(new UIView[] { viewFindUs, viewFeedback, viewCallUs, lblDetails });

            View.AddSubviews(new UIView[] { imgLogo, imgPrelogin, lblbWelcome
                , lblSubtitle, viewCTA, viewLine, lblQuickAccess, viewQuickAccess});
        }

        internal void OnRegister()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
            UIViewController viewController =
                storyBoard.InstantiateViewController("RegistrationViewController") as UIViewController;
            var navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
        }

        internal void OnLogin()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Login", null);
            UIViewController loginVC = storyBoard.InstantiateViewController("LoginViewController") as UIViewController;
            ShowViewController(loginVC, this);
        }

        void ShowBrowser(string url)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrWhiteSpace(url))
            {
                string title = "Login_Promotions".Translate();
                UIStoryboard storyBoard = UIStoryboard.FromName("Browser", null);
                BrowserViewController viewController =
                    storyBoard.InstantiateViewController("BrowserViewController") as BrowserViewController;
                if (viewController != null)
                {
                    viewController.NavigationTitle = title;
                    viewController.URL = url;
                    viewController.IsDelegateNeeded = false;
                    var navController = new UINavigationController(viewController);
                    PresentViewController(navController, true, null);
                }
                return;
            }
            AlertHandler.DisplayServiceError(this, string.Empty);
        }

        void CallCustomerService()
        {
            if (DataManager.DataManager.SharedInstance.WebLinks != null)
            {
                int index = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbcl"));
                if (index > -1)
                {
                    string number = DataManager.DataManager.SharedInstance.WebLinks[index].Url;
                    if (!string.IsNullOrEmpty(number) && !string.IsNullOrWhiteSpace(number))
                    {
                        NSUrl url = new NSUrl(new Uri("tel:" + number).AbsoluteUri);
                        UIApplication.SharedApplication.OpenUrl(url);
                        return;
                    }
                }
            }
            AlertHandler.DisplayServiceError(this, string.Empty);
        }

        void GoToFindUs()
        {
            DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex = 0;
            DataManager.DataManager.SharedInstance.PreviousStoreTypeIndex = 0;
            DataManager.DataManager.SharedInstance.IsSameStoreType = false;
            DataManager.DataManager.SharedInstance.SelectedLocationTypeID = "all";
            UIStoryboard storyBoard = UIStoryboard.FromName("FindUs", null);
            FindUsViewController viewController =
                storyBoard.InstantiateViewController("FindUsViewController") as FindUsViewController;
            var navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
        }

        void GoToFeedback()
        {
            DataManager.DataManager.SharedInstance.IsPreloginFeedback = true;
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            FeedbackViewController feedbackVC =
                storyBoard.InstantiateViewController("FeedbackViewController") as FeedbackViewController;
            if (feedbackVC != null)
            {
                feedbackVC.isFromPreLogin = true;
                var navController = new UINavigationController(feedbackVC);
                PresentViewController(navController, true, null);
            }
        }
    }
}