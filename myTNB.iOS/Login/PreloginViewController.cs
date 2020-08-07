using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using myTNB.DataManager;
using myTNB.SitecoreCMS;

namespace myTNB
{
    public partial class PreloginViewController : CustomUIViewController
    {
        public PreloginViewController(IntPtr handle) : base(handle) { }

        private UILabel _lblWelcome, _lblSubtitle, _lblQuickAccess, _lblFindUs
            , _lblCallUs, _lblFeedback, _lblChangeLanguage;
        private CustomUIButtonV2 _btnRegister, _btnLogin;
        private bool _isMasterDataDone, _isSitecoreDone;

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

        protected override void LanguageDidChange(NSNotification notification)
        {
            base.LanguageDidChange(notification);
            _lblWelcome.Text = GetI18NValue(PreloginConstants.I18N_WelcomeTitle);
            _lblSubtitle.Text = GetI18NValue(PreloginConstants.I18N_Tagline);
            _lblQuickAccess.Text = GetI18NValue(PreloginConstants.I18N_QuickAccess);
            _lblFindUs.Text = GetI18NValue(PreloginConstants.I18N_FindUs);
            _lblCallUs.Text = GetI18NValue(PreloginConstants.I18N_CallUs);

            if (DataManager.DataManager.SharedInstance.WebLinks != null)
            {
                int index = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbcl"));
                if (index > -1)
                {
                    _lblCallUs.Text = DataManager.DataManager.SharedInstance.WebLinks[index].Title;
                }
            }
            _lblFeedback.Text = GetI18NValue(PreloginConstants.I18N_Enquiry);//PreloginConstants.I18N_Feedback
            _lblChangeLanguage.Text = GetI18NValue(PreloginConstants.I18N_ChangeLanguage);
            _btnRegister.SetTitle(GetI18NValue(PreloginConstants.I18N_Register), UIControlState.Normal);
            _btnLogin.SetTitle(GetI18NValue(PreloginConstants.I18N_Login), UIControlState.Normal);
            _isMasterDataDone = false;
            _isSitecoreDone = false;
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

            _lblWelcome = new UILabel(new CGRect(0, GetYLocationFromFrame(imgHeader.Frame, 12F), ViewWidth, GetScaledHeight(24F)))
            {
                Text = GetI18NValue(PreloginConstants.I18N_WelcomeTitle),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlueTwo,
                Font = TNBFont.MuseoSans_16_500
            };

            _lblSubtitle = new UILabel(new CGRect(GetScaledWidth(24F), GetYLocationFromFrame(_lblWelcome.Frame, 4F)
                , ViewWidth - (GetScaledWidth(24F) * 2), GetScaledHeight(32F)))
            {
                Text = GetI18NValue(PreloginConstants.I18N_Tagline),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WarmGrey,
                Font = TNBFont.MuseoSans_12_300,
                Lines = 0
            };

            UIView viewCTA = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(_lblSubtitle.Frame, 20F)
                , ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(48F)));

            _btnRegister = new CustomUIButtonV2()
            {
                Frame = new CGRect(0, 0, (viewCTA.Frame.Width / 2) - GetScaledWidth(2F), GetScaledHeight(48F)),
                BackgroundColor = UIColor.White,
                Font = TNBFont.MuseoSans_16_500
            };
            _btnRegister.SetTitle(GetI18NValue(PreloginConstants.I18N_Register), UIControlState.Normal);
            _btnRegister.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _btnRegister.Layer.CornerRadius = GetScaledHeight(4F);
            _btnRegister.Layer.BorderWidth = GetScaledWidth(1F);
            _btnRegister.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnRegister.TouchUpInside += (sender, e) =>
            {
                OnRegister();
            };

            _btnLogin = new CustomUIButtonV2()
            {
                Frame = new CGRect(_btnRegister.Frame.GetMaxX() + GetScaledWidth(4F)
                    , 0, (viewCTA.Frame.Width / 2) - GetScaledWidth(2F), GetScaledHeight(48F)),
                Font = TNBFont.MuseoSans_16_500,
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnLogin.SetTitle(GetI18NValue(PreloginConstants.I18N_Login), UIControlState.Normal);
            _btnLogin.Layer.CornerRadius = GetScaledHeight(4F);
            _btnLogin.Layer.BorderWidth = GetScaledWidth(1F);
            _btnLogin.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnLogin.TouchUpInside += (sender, e) =>
            {
                OnLogin();
            };

            viewCTA.AddSubviews(new UIView[] { _btnRegister, _btnLogin });

            UIView viewLine = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(viewCTA.Frame, 15.5F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };

            UIView viewQuickAccess = new UIView(new CGRect(0, GetYLocationFromFrame(viewLine.Frame, 15.5F)
                , ViewWidth, GetScaledHeight(176)))
            {
                ClipsToBounds = false
            };

            _lblQuickAccess = new UILabel(new CGRect(0, 0, ViewWidth, GetScaledHeight(24F)))
            {
                Text = GetI18NValue(PreloginConstants.I18N_QuickAccess),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlueTwo,
                Font = TNBFont.MuseoSans_16_500
            };

            UIView viewFindUs = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(_lblQuickAccess.Frame, 8F)
                , viewQuickAccess.Frame.Width / 3 - GetScaledWidth(18.67F), GetScaledHeight(84F)))
            {
                ClipsToBounds = false
            };
            viewFindUs.Layer.CornerRadius = GetScaledHeight(5F);
            viewFindUs.BackgroundColor = UIColor.White;
            AddCardShadow(ref viewFindUs);

            UIImageView imgFindUs = new UIImageView(new CGRect(GetXLocationToCenterObject(GetScaledWidth(28F)
                , viewFindUs), GetScaledHeight(12F), GetScaledWidth(28F), GetScaledHeight(28F)))
            {
                Image = UIImage.FromBundle(PreloginConstants.IMG_FindUsIcon)
            };

            _lblFindUs = new UILabel(new CGRect(0, GetYLocationFromFrame(imgFindUs.Frame, 12F)
               , viewFindUs.Frame.Width, GetScaledHeight(14F)))
            {
                Text = GetI18NValue(PreloginConstants.I18N_FindUs),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_10_500
            };

            viewFindUs.AddSubviews(new UIView[] { imgFindUs, _lblFindUs });

            UIView viewCallUs = new UIView(new CGRect(GetXLocationFromFrame(viewFindUs.Frame, 12F), GetYLocationFromFrame(_lblQuickAccess.Frame, 8F)
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

            _lblCallUs = new UILabel(new CGRect(0, GetYLocationFromFrame(imgCallUs.Frame, 12F), viewCallUs.Frame.Width, GetScaledHeight(14F)))
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
                    _lblCallUs.Text = DataManager.DataManager.SharedInstance.WebLinks[index].Title;
                }
            }

            viewCallUs.AddSubviews(new UIView[] { imgCallUs, _lblCallUs });

            UIView viewFeedback = new UIView(new CGRect(GetXLocationFromFrame(viewCallUs.Frame, 12F), GetYLocationFromFrame(_lblQuickAccess.Frame, 8F)
                , viewQuickAccess.Frame.Width / 3 - GetScaledWidth(18.67F), GetScaledHeight(84F)));
            viewFeedback.Layer.CornerRadius = GetScaledHeight(5F);
            viewFeedback.BackgroundColor = UIColor.White;
            AddCardShadow(ref viewFeedback);

            UIImageView imgFeedback = new UIImageView(new CGRect(GetXLocationToCenterObject(GetScaledWidth(28F), viewFeedback), GetScaledHeight(12F), GetScaledWidth(28F), GetScaledHeight(28F)))
            {
                Image = UIImage.FromBundle(PreloginConstants.IMG_FeedbackIcon)
            };

            _lblFeedback = new UILabel(new CGRect(GetScaledWidth(8), GetYLocationFromFrame(imgFeedback.Frame, 4F)
                , viewFeedback.Frame.Width - GetScaledWidth(16), GetScaledHeight(28F)))
            {
                Text = GetI18NValue(PreloginConstants.I18N_Enquiry), //PreloginConstants.I18N_Feedback
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_10_500,
                Lines = 0
            };

            viewFeedback.AddSubviews(new UIView[] { imgFeedback, _lblFeedback });

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
                DisplayCustomAlert(GetFormattedLangKey(Constants.Common_ChangeLanguageTitle)
                    , GetFormattedLangKey(Constants.Common_ChangeLanguageMessage)
                    , new Dictionary<string, Action> {
                        { GetFormattedLangKey( Constants.Common_ChangeLanguageNo), null}
                        ,{ GetFormattedLangKey( Constants.Common_ChangeLanguageYes)
                        ,()=>{ OnChangeLanguage(); } } }
                    , UITextAlignment.Center
                    , UITextAlignment.Center);
            }));

            _lblChangeLanguage = new UILabel(new CGRect(new CGPoint(0, 0), changeLanguageView.Frame.Size))
            {
                Text = GetI18NValue(PreloginConstants.I18N_ChangeLanguage),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlueTwo,
                Font = TNBFont.MuseoSans_12_500,
                Lines = 0
            };
            changeLanguageView.AddSubview(_lblChangeLanguage);
            viewQuickAccess.AddSubviews(new UIView[] { _lblQuickAccess, viewFindUs, viewCallUs, viewFeedback, changeLanguageView });

            View.AddSubviews(new UIView[] { imgHeader, imgLogo, _lblWelcome
                , _lblSubtitle, viewCTA, viewLine, viewQuickAccess});
        }

        private string GetFormattedLangKey(string key)
        {
            return GetCommonI18NValue(string.Format("{0}_{1}", key, TNBGlobal.APP_LANGUAGE));
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

        #region Language
        /*Todo: Do service calls and set lang
         * 1. Call site core
         * 2. Call Applaunch master data
         * 3. Clear Usage cache for service call content
        */
        private void OnChangeLanguage()
        {
            int index = 0;
            if (TNBGlobal.APP_LANGUAGE == "EN")
            {
                index = 1;
            }
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        LanguageUtility.SetAppLanguageByIndex(index);
                        InvokeOnMainThread(async () =>
                        {
                            List<Task> taskList = new List<Task>{
                                OnGetAppLaunchMasterData(),
                                OnExecuteSiteCore()
                           };
                            await Task.WhenAll(taskList.ToArray());
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void ChangeLanguageCallback()
        {
            if (_isMasterDataDone && _isSitecoreDone)
            {
                if (SitecoreServices.IsForcedUpdate)
                {
                    Task.Factory.StartNew(() =>
                    {
                        ChangeLanguageCallback();
                    });
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        ClearCache();
                        Debug.WriteLine("Change Language Done");
                        NotifCenterUtility.PostNotificationName("LanguageDidChange", new NSObject());
                        LanguageUtility.DidUserChangeLanguage = true;
                        ActivityIndicator.Hide();
                    });
                }
            }
        }

        private void ClearCache()
        {
            DataManager.DataManager.SharedInstance.IsSameAccount = false;
            AccountUsageCache.ClearCache();
            AccountUsageSmartCache.ClearCache();
        }

        private Task OnGetAppLaunchMasterData()
        {
            return Task.Factory.StartNew(() =>
            {
                AppLaunchResponseModel response = ServiceCall.GetAppLaunchMasterData().Result;
                AppLaunchMasterCache.AddAppLaunchResponseData(response);
                _isMasterDataDone = true;
                ChangeLanguageCallback();
            });
        }

        private Task OnExecuteSiteCore()
        {
            return Task.Factory.StartNew(async () =>
            {
                await SitecoreServices.Instance.OnExecuteSitecoreCall(true);
                _isSitecoreDone = true;
                ChangeLanguageCallback();
            });
        }
        #endregion
    }
}