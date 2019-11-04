using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class PreloginViewController : CustomUIViewController
    {
        public PreloginViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = PreloginConstants.PageName;
            base.ViewDidLoad();
            SetSubviews();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        private void SetSubviews()
        {
            UIImageView imgLogo = new UIImageView(new CGRect(GetXLocationToCenterObject(GetScaledWidth(40F), View)
                , DeviceHelper.GetStatusBarHeight() + GetScaledHeight(16F), GetScaledWidth(40F), GetScaledHeight(40F)))
            {
                Image = UIImage.FromBundle(PreloginConstants.IMG_TNBLogo)
            };

            UIImageView imgHeader = new UIImageView(new CGRect(0, 0, ViewWidth, GetScaledHeight(220F)))
            {
                Image = UIImage.FromBundle(PreloginConstants.IMG_Header),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            UILabel lblWelcome = new UILabel(new CGRect(0, GetYLocationFromFrame(imgHeader.Frame, 12F), ViewWidth, GetScaledHeight(24F)))
            {
                Text = GetI18NValue(PreloginConstants.I18N_WelcomeTitle),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlueTwo,
                Font = TNBFont.MuseoSans_16_500
            };

            UILabel lblSubtitle = new UILabel(new CGRect(GetScaledWidth(24F), GetYLocationFromFrame(lblWelcome.Frame, 4F), ViewWidth - (GetScaledWidth(24F) * 2), GetScaledHeight(32F)))
            {
                Text = GetI18NValue(PreloginConstants.I18N_Tagline),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WarmGrey,
                Font = TNBFont.MuseoSans_12_300,
                Lines = 0
            };

            UIView viewCTA = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(lblSubtitle.Frame, 20F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(48F)));

            UIButton btnRegister = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(0, 0, (viewCTA.Frame.Width / 2) - GetScaledWidth(2F), GetScaledHeight(48F))
            };
            btnRegister.Font = TNBFont.MuseoSans_16_500;
            btnRegister.SetTitle(GetI18NValue(PreloginConstants.I18N_Register), UIControlState.Normal);
            btnRegister.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            btnRegister.Layer.CornerRadius = GetScaledHeight(4F);
            btnRegister.Layer.BorderWidth = GetScaledWidth(1F);
            btnRegister.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnRegister.BackgroundColor = UIColor.White;
            btnRegister.TouchUpInside += (sender, e) =>
            {
                OnRegister();
            };

            UIButton btnLogin = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(btnRegister.Frame.GetMaxX() + GetScaledWidth(4F), 0, (viewCTA.Frame.Width / 2) - GetScaledWidth(2F), GetScaledHeight(48F))
            };
            btnLogin.Font = TNBFont.MuseoSans_16_500;
            btnLogin.SetTitle(GetI18NValue(PreloginConstants.I18N_Login), UIControlState.Normal);
            btnLogin.Layer.CornerRadius = GetScaledHeight(4F);
            btnLogin.Layer.BorderWidth = GetScaledWidth(1F);
            btnLogin.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnLogin.BackgroundColor = MyTNBColor.FreshGreen;
            btnLogin.TouchUpInside += (sender, e) =>
            {
                OnLogin();
            };

            viewCTA.AddSubviews(new UIView[] { btnRegister, btnLogin });

            UIView viewLine = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(viewCTA.Frame, 15.5F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };

            UIView viewQuickAccess = new UIView(new CGRect(0, GetYLocationFromFrame(viewLine.Frame, 15.5F)
                , ViewWidth, GetScaledHeight(176)))
            {
                ClipsToBounds = false
            };

            UILabel lblQuickAccess = new UILabel(new CGRect(0, 0, ViewWidth, GetScaledHeight(24F)))
            {
                Text = GetI18NValue(PreloginConstants.I18N_QuickAccess),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlueTwo,
                Font = TNBFont.MuseoSans_16_500
            };

            UIView viewFindUs = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(lblQuickAccess.Frame, 8F)
                , viewQuickAccess.Frame.Width / 3 - GetScaledWidth(18.67F), GetScaledHeight(84F)))
            {
                ClipsToBounds = false
            };
            viewFindUs.Layer.CornerRadius = GetScaledHeight(5F);
            viewFindUs.BackgroundColor = UIColor.White;
            AddCardShadow(ref viewFindUs);

            UIImageView imgFindUs = new UIImageView(new CGRect(GetXLocationToCenterObject(GetScaledWidth(28F), viewFindUs), GetScaledHeight(12F), GetScaledWidth(28F), GetScaledHeight(28F)))
            {
                Image = UIImage.FromBundle(PreloginConstants.IMG_FindUsIcon)
            };

            UILabel lblFindUs = new UILabel(new CGRect(0, GetYLocationFromFrame(imgFindUs.Frame, 12F), viewFindUs.Frame.Width, GetScaledHeight(14F)))
            {
                Text = GetI18NValue(PreloginConstants.I18N_FindUs),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_10_500
            };

            viewFindUs.AddSubviews(new UIView[] { imgFindUs, lblFindUs });

            UIView viewCallUs = new UIView(new CGRect(GetXLocationFromFrame(viewFindUs.Frame, 12F), GetYLocationFromFrame(lblQuickAccess.Frame, 8F)
               , viewQuickAccess.Frame.Width / 3 - GetScaledWidth(18.67F), GetScaledHeight(84F)))
            {
                ClipsToBounds = false
            };
            viewCallUs.Layer.CornerRadius = GetScaledHeight(5F);
            viewCallUs.BackgroundColor = UIColor.White;
            AddCardShadow(ref viewCallUs);

            UIImageView imgCallUs = new UIImageView(new CGRect(GetXLocationToCenterObject(GetScaledWidth(28F), viewCallUs), GetScaledHeight(12F), GetScaledWidth(28F), GetScaledHeight(28F)))
            {
                Image = UIImage.FromBundle(PreloginConstants.IMG_CallUsIcon)
            };

            UILabel lblCallUs = new UILabel(new CGRect(0, GetYLocationFromFrame(imgCallUs.Frame, 12F), viewCallUs.Frame.Width, GetScaledHeight(14F)))
            {
                Text = GetI18NValue(PreloginConstants.I18N_CallUs),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_10_500
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

            UIView viewFeedback = new UIView(new CGRect(GetXLocationFromFrame(viewCallUs.Frame, 12F), GetYLocationFromFrame(lblQuickAccess.Frame, 8F)
                , viewQuickAccess.Frame.Width / 3 - GetScaledWidth(18.67F), GetScaledHeight(84F)));
            viewFeedback.Layer.CornerRadius = GetScaledHeight(5F);
            viewFeedback.BackgroundColor = UIColor.White;
            AddCardShadow(ref viewFeedback);

            UIImageView imgFeedback = new UIImageView(new CGRect(GetXLocationToCenterObject(GetScaledWidth(28F), viewFeedback), GetScaledHeight(12F), GetScaledWidth(28F), GetScaledHeight(28F)))
            {
                Image = UIImage.FromBundle(PreloginConstants.IMG_FeedbackIcon)
            };

            UILabel lblFeedback = new UILabel(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(imgFeedback.Frame, 4F), viewFeedback.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(28F)))
            {
                Text = GetI18NValue(PreloginConstants.I18N_Feedback),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_10_500,
                Lines = 0
            };

            viewFeedback.AddSubviews(new UIView[] { imgFeedback, lblFeedback });

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

            CustomUIView changeLanguageView = new CustomUIView(new CGRect(0
                , viewFeedback.Frame.GetMaxY() + GetScaledHeight(20), ViewWidth, GetScaledHeight(18)));

            changeLanguageView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DisplayCustomAlert(GetCommonI18NValue(Constants.Common_ChangeLanguageTitle)
                    , GetCommonI18NValue(Constants.Common_ChangeLanguageMessage)
                    , new System.Collections.Generic.Dictionary<string, Action> {
                        { GetCommonI18NValue(Constants.Common_ChangeLanguageNo), null}
                        ,{ GetCommonI18NValue(Constants.Common_ChangeLanguageYes) ,null} }
                    , UITextAlignment.Center
                    , UITextAlignment.Center);
            }));

            UILabel lblChangeLanguage = new UILabel(new CGRect(new CGPoint(0, 0), changeLanguageView.Frame.Size))
            {
                Text = GetI18NValue(PreloginConstants.I18N_ChangeLanguage),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlueTwo,
                Font = TNBFont.MuseoSans_12_500,
                Lines = 0
            };
            changeLanguageView.AddSubview(lblChangeLanguage);
            viewQuickAccess.AddSubviews(new UIView[] { lblQuickAccess, viewFindUs, viewCallUs, viewFeedback, changeLanguageView });

            View.AddSubviews(new UIView[] { imgHeader, imgLogo, lblWelcome
                , lblSubtitle, viewCTA, viewLine, viewQuickAccess});
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 5f;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue60.CGColor;
            view.Layer.ShadowOpacity = .8f;
            view.Layer.ShadowOffset = new CGSize(0, 8);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }

        private void OnRegister()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
            UIViewController viewController =
                storyBoard.InstantiateViewController("RegistrationViewController") as UIViewController;
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
        }

        private void OnLogin()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Login", null);
            UIViewController loginVC = storyBoard.InstantiateViewController("LoginViewController") as UIViewController;
            loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            ShowViewController(loginVC, this);
        }

        private void CallCustomerService()
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
            DisplayServiceError(string.Empty);
        }

        private void GoToFindUs()
        {
            DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex = 0;
            DataManager.DataManager.SharedInstance.PreviousStoreTypeIndex = 0;
            DataManager.DataManager.SharedInstance.IsSameStoreType = false;
            DataManager.DataManager.SharedInstance.SelectedLocationTypeID = "all";
            UIStoryboard storyBoard = UIStoryboard.FromName("FindUs", null);
            FindUsViewController viewController =
                storyBoard.InstantiateViewController("FindUsViewController") as FindUsViewController;
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
        }

        private void GoToFeedback()
        {
            DataManager.DataManager.SharedInstance.IsPreloginFeedback = true;
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            FeedbackViewController feedbackVC =
                storyBoard.InstantiateViewController("FeedbackViewController") as FeedbackViewController;
            if (feedbackVC != null)
            {
                feedbackVC.isFromPreLogin = true;
                UINavigationController navController = new UINavigationController(feedbackVC);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }
    }
}
