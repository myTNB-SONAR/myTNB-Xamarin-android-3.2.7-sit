using System;
using UIKit;
using myTNB.Model;
using Foundation;
using CoreGraphics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using myTNB.Login.ForgotPassword;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.DataManager;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.Model.Language;
using myTNB.SitecoreCMS;

namespace myTNB
{
    public partial class LoginViewController : CustomUIViewController
    {
        private readonly Regex EmailRegex = new Regex(@"[a-zA-Z0-9\\+\\.\\_\\%\\-\\+]{1,256}" + "\\@"
                                                      + "[a-zA-Z0-9][a-zA-Z0-9\\-]{0,64}"
                                                      + "("
                                                      + "\\."
                                                      + "[a-zA-Z0-9][a-zA-Z0-9\\-]{0,25}"
                                                      + ")+");
        private string _eMail = string.Empty;
        private string _password = string.Empty;
        private bool IsRememberMe = true;

        private UserAuthenticationResponseModel _authenticationList = new UserAuthenticationResponseModel();
        private BillingAccountDetailsResponseModel _billingAccountDetailsList = new BillingAccountDetailsResponseModel();
        private TextFieldHelper _textFieldHelper = new TextFieldHelper();

        private UIView viewEmail, viewPassword, viewRememberMe, viewForgotPassword, viewShowPassword
            , viewLineEmail, viewLinePassword, viewCheckBox;

        private UILabel lblEmailTitle, lblPasswordTitle, lblRememberMe;

        private UITextField txtFieldEmail, txtFieldPassword;
        private UIImageView imgViewCheckBox;
        private UIScrollView ScrollView;
        private CGRect scrollViewFrame;

        public LoginViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = LoginConstants.PageName;
            base.ViewDidLoad();
            InitializeSubViews();
            Setevents();
            SetupSubViews();
            NotifCenterUtility.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NotifCenterUtility.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            dismissKeyBoard();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        private void InitializeSubViews()
        {
            ScrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height))
            {
                BackgroundColor = UIColor.Clear,
                Bounces = false
            };
            View.AddSubview(ScrollView);

            UIImageView imgLogo = new UIImageView(new CGRect(GetXLocationToCenterObject(GetScaledWidth(40F), View)
                   , DeviceHelper.GetStatusBarHeight() + GetScaledHeight(16F), GetScaledWidth(40F), GetScaledHeight(40F)))
            {
                Image = UIImage.FromBundle(LoginConstants.IMG_TNBLogo)
            };

            UIImageView imgHeader = new UIImageView(new CGRect(0, 0, ViewWidth, GetScaledHeight(220F)))
            {
                Image = UIImage.FromBundle(LoginConstants.IMG_Header),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            UIImageView btnBack = new UIImageView(new CGRect(GetScaledWidth(18F), DeviceHelper.GetStatusBarHeight() + GetScaledHeight(4F), GetScaledWidth(24F), GetScaledHeight(24F)))
            {
                Image = UIImage.FromBundle(LoginConstants.IMG_Back),
                UserInteractionEnabled = true
            };
            btnBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnDismiss();
            }));

            UILabel lblTitle = new UILabel(new CGRect(0, GetYLocationFromFrame(imgHeader.Frame, 12F), ViewWidth, GetScaledHeight(24F)))
            {
                Text = GetI18NValue(LoginConstants.I18N_Title),
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlueTwo,
                Font = TNBFont.MuseoSans_16_500
            };

            ScrollView.AddSubviews(new UIView[] { imgHeader, btnBack, imgLogo, lblTitle });

            viewEmail = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(lblTitle.Frame, 6F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(51F)));

            lblEmailTitle = new UILabel(new CGRect(0, 0, viewEmail.Frame.Width, GetScaledHeight(11.5F)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_10_300,
                TextColor = MyTNBColor.SilverChalice,
                Text = GetCommonI18NValue(Constants.Common_Email).ToUpper()
            };

            txtFieldEmail = new UITextField
            {
                Frame = new CGRect(0, GetScaledHeight(11.1F), viewEmail.Frame.Width, GetScaledHeight(24F)),
                AttributedPlaceholder = new NSAttributedString(
                    GetCommonI18NValue(Constants.Common_Email),
                    font: TNBFont.MuseoSans_16_300,
                    foregroundColor: MyTNBColor.SilverChalice,
                    strokeWidth: 0
                ),
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.CharcoalGrey,
            };
            txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;
            txtFieldEmail.TintColor = MyTNBColor.CharcoalGrey;
            _textFieldHelper.CreateTextFieldLeftView(txtFieldEmail, LoginConstants.IMG_EmailIcon);
            _textFieldHelper.SetKeyboard(txtFieldEmail);

            viewLineEmail = new UIView(new CGRect(0, GetScaledHeight(34.6F), viewEmail.Frame.Width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };

            viewEmail.AddSubviews(new UIView[] { lblEmailTitle, txtFieldEmail, viewLineEmail });

            viewRememberMe = new UIView(new CGRect(BaseMarginWidth16, viewEmail.Frame.GetMaxY(), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(24F)));

            viewCheckBox = new UIView(new CGRect(GetScaledWidth(2F), GetScaledHeight(2F), GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                BackgroundColor = MyTNBColor.WhiteTwo
            };
            viewCheckBox.Layer.CornerRadius = GetScaledWidth(5F);
            viewCheckBox.Layer.BorderColor = UIColor.Clear.CGColor;
            viewCheckBox.Layer.BorderWidth = GetScaledWidth(1F);
            imgViewCheckBox = new UIImageView(new CGRect(0, 0, GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                Image = UIImage.FromBundle(LoginConstants.IMG_RememberIcon),
                ContentMode = UIViewContentMode.ScaleAspectFill,
                BackgroundColor = UIColor.Clear
            };
            viewCheckBox.AddSubview(imgViewCheckBox);

            viewCheckBox.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                IsRememberMe = !IsRememberMe;
                imgViewCheckBox.Hidden = !IsRememberMe;
                viewCheckBox.Layer.BorderColor = IsRememberMe ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;
            }));

            lblRememberMe = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBox.Frame, 8F), GetScaledHeight(4F), viewRememberMe.Frame.Width, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = GetI18NValue(LoginConstants.I18N_RememberEmail)
            };

            viewRememberMe.AddSubviews(new UIView[] { viewCheckBox, lblRememberMe });

            viewPassword = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(viewRememberMe.Frame, 16F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(51F)));

            lblPasswordTitle = new UILabel(new CGRect(0, 0, viewPassword.Frame.Width, GetScaledHeight(12F)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_10_300,
                TextColor = MyTNBColor.SilverChalice,
                Text = GetCommonI18NValue(Constants.Common_Password).ToUpper()
            };

            txtFieldPassword = new UITextField
            {
                Frame = new CGRect(0, GetScaledHeight(11.1F), viewPassword.Frame.Width - GetScaledWidth(34), GetScaledHeight(24F)),
                AttributedPlaceholder = new NSAttributedString(
                    GetCommonI18NValue(Constants.Common_Password),
                    font: TNBFont.MuseoSans_16_300,
                    foregroundColor: MyTNBColor.SilverChalice,
                    strokeWidth: 0
                ),
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.CharcoalGrey,
            };
            txtFieldPassword.KeyboardType = UIKeyboardType.Default;
            txtFieldPassword.TintColor = MyTNBColor.CharcoalGrey;
            txtFieldPassword.SecureTextEntry = true;
            _textFieldHelper.SetKeyboard(txtFieldPassword);
            _textFieldHelper.CreateTextFieldLeftView(txtFieldPassword, LoginConstants.IMG_PasswordIcon);

            viewLinePassword = new UIView(new CGRect(0, GetScaledHeight(34.6F), viewPassword.Frame.Width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };

            viewForgotPassword = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(viewPassword.Frame, 4F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(16F)));
            UILabel lblForgotPassword = new UILabel(new CGRect(0, 0, viewForgotPassword.Frame.Width, GetScaledHeight(16F)))
            {
                TextAlignment = UITextAlignment.Right,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(LoginConstants.I18N_ForgotPassword)
            };
            viewForgotPassword.AddSubview(lblForgotPassword);

            viewShowPassword = new UIView(new CGRect(viewPassword.Frame.Width - GetScaledWidth(24F), GetScaledHeight(11.1F), GetScaledWidth(24F), GetScaledHeight(24F)))
            {
                Hidden = true
            };
            UIImageView imgShowPassword = new UIImageView(new CGRect(0, 0, GetScaledWidth(24F), GetScaledHeight(24F)));
            imgShowPassword.Image = UIImage.FromBundle(LoginConstants.IMG_ShowPasswordIcon);
            viewShowPassword.AddSubview(imgShowPassword);
            viewShowPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                imgShowPassword.Image = UIImage.FromBundle(txtFieldPassword.SecureTextEntry
                    ? LoginConstants.IMG_HidePasswordIcon : LoginConstants.IMG_ShowPasswordIcon);
                txtFieldPassword.SecureTextEntry = !txtFieldPassword.SecureTextEntry;
            }));
            viewPassword.AddSubviews(new UIView[] { lblPasswordTitle, txtFieldPassword, viewShowPassword, viewLinePassword });

            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
            string emailString = sharedPreference.StringForKey(TNBGlobal.PreferenceKeys.RememberEmail);
            if (!string.IsNullOrEmpty(emailString))
            {
                txtFieldEmail.Text = emailString;
                txtFieldEmail.LeftViewMode = UITextFieldViewMode.Never;
            }
            ScrollView.AddSubviews(new UIView[] { viewEmail, viewRememberMe, viewPassword, viewForgotPassword });

            UIButton btnLogin = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(BaseMarginWidth16, ViewHeight + DeviceHelper.GetStatusBarHeight() - GetScaledHeight(48F) - GetScaledHeight(16F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(48F))
            };
            btnLogin.Font = TNBFont.MuseoSans_16_500;
            btnLogin.SetTitle(GetI18NValue(LoginConstants.I18N_LogIn), UIControlState.Normal);
            btnLogin.Layer.CornerRadius = GetScaledHeight(4F);
            btnLogin.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnLogin.BackgroundColor = MyTNBColor.FreshGreen;
            btnLogin.TouchUpInside += (sender, e) =>
            {
                OnLogin();
            };
            View.AddSubview(btnLogin);

            UILabel registerLbl = new UILabel(new CGRect(BaseMarginWidth16, btnLogin.Frame.GetMinY() - GetScaledHeight(32F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(16F)))
            {
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlueTwo,
                Text = GetI18NValue(LoginConstants.I18N_RegisterAcctNow),
                UserInteractionEnabled = true
            };
            registerLbl.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ShowRegister();
            }));
            View.AddSubview(registerLbl);

            UILabel descLbl = new UILabel(new CGRect(BaseMarginWidth16, registerLbl.Frame.GetMinY() - GetScaledHeight(16F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(16F)))
            {
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Text = GetI18NValue(LoginConstants.I18N_DontHaveAcct),
                UserInteractionEnabled = true
            };
            View.AddSubview(descLbl);
            ViewHelper.AdjustFrameSetHeight(ScrollView, View.Frame.Height - (View.Frame.Height - descLbl.Frame.GetMinY()));
            ScrollView.ContentSize = new CGRect(0, 0, View.Frame.Width, View.Frame.Height - (View.Frame.Height - descLbl.Frame.GetMinY())).Size;
            scrollViewFrame = ScrollView.Frame;
        }

        private void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            bool visible = notification.Name == UIKeyboard.WillShowNotification;
            UIView.BeginAnimations("AnimateForKeyboard");
            UIView.SetAnimationBeginsFromCurrentState(true);
            UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
            UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

            if (visible && txtFieldPassword.IsFirstResponder)
            {
                CGRect r = UIKeyboard.BoundsFromNotification(notification);
                CGRect viewFrame = View.Bounds;
                nfloat currentViewHeight = viewFrame.Height - r.Height;
                ScrollView.Frame = new CGRect(0, -DeviceHelper.GetStatusBarHeight(), ScrollView.Frame.Width, currentViewHeight);
            }
            else
            {
                ScrollView.Frame = scrollViewFrame;
            }
            UIView.CommitAnimations();
        }

        /// <summary>
        /// Method to remember or not the email based user selection when logging in
        /// </summary>
        private void RememberMe()
        {
            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
            if (IsRememberMe)
            {
                sharedPreference.SetString(_eMail, TNBGlobal.PreferenceKeys.RememberEmail);
            }
            else
            {
                sharedPreference.SetString(string.Empty, TNBGlobal.PreferenceKeys.RememberEmail);
            }
            sharedPreference.Synchronize();
        }

        private void Setevents()
        {
            viewForgotPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ShowForgotPassword();
            }));

            txtFieldEmail.EditingChanged += (sender, e) =>
            {
                lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
            };

            txtFieldEmail.EditingDidBegin += (sender, e) =>
            {
                lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
                viewLineEmail.BackgroundColor = MyTNBColor.WaterBlueTwo;
                txtFieldEmail.LeftViewMode = UITextFieldViewMode.Never;
            };

            txtFieldEmail.ShouldEndEditing = (sender) =>
            {
                lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
                return true;
            };

            txtFieldEmail.EditingDidEnd += (sender, e) =>
            {
                viewLineEmail.BackgroundColor = MyTNBColor.VeryLightPinkThree;
                if (txtFieldEmail.Text.Length == 0)
                {
                    txtFieldEmail.LeftViewMode = UITextFieldViewMode.UnlessEditing;
                }
            };

            txtFieldEmail.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };

            txtFieldEmail.ShouldBeginEditing = (sender) =>
            {
                return true;
            };

            txtFieldPassword.EditingChanged += (sender, e) =>
            {
                lblPasswordTitle.Hidden = txtFieldPassword.Text.Length == 0;
                viewShowPassword.Hidden = txtFieldPassword.Text.Length == 0;
            };

            txtFieldPassword.EditingDidBegin += (sender, e) =>
            {
                lblPasswordTitle.Hidden = txtFieldPassword.Text.Length == 0;
                viewShowPassword.Hidden = txtFieldPassword.Text.Length == 0;
                viewLinePassword.BackgroundColor = MyTNBColor.WaterBlueTwo;
                txtFieldPassword.LeftViewMode = UITextFieldViewMode.Never;
            };

            txtFieldPassword.ShouldEndEditing = (sender) =>
            {
                lblPasswordTitle.Hidden = txtFieldPassword.Text.Length == 0;
                viewShowPassword.Hidden = txtFieldPassword.Text.Length == 0;
                return true;
            };

            txtFieldPassword.EditingDidEnd += (sender, e) =>
            {
                viewLinePassword.BackgroundColor = MyTNBColor.VeryLightPinkThree;
                if (txtFieldPassword.Text.Length == 0)
                    txtFieldPassword.LeftViewMode = UITextFieldViewMode.UnlessEditing;
            };

            txtFieldPassword.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };

            txtFieldPassword.ShouldBeginEditing = (sender) =>
            {
                return true;
            };
        }

        private void ShowEmptyEmailError()
        {
            DisplayToast(GetI18NValue(LoginConstants.I18N_EmailRequired));
        }

        private void ShowInvalidEmailError()
        {
            DisplayToast(GetErrorI18NValue(Constants.Error_InvalidEmailAddress));
        }

        private void ShowEmptyPasswordError()
        {
            DisplayToast(GetI18NValue(LoginConstants.I18N_PasswordRequired));
        }

        private void ShowServerError(string errorMessage)
        {
            DisplayToast(errorMessage ?? GetErrorI18NValue(Constants.Error_DefaultErrorMessage));
        }

        private void OnLogin()
        {
            _eMail = txtFieldEmail.Text.Trim();
            _password = txtFieldPassword.Text.Trim();
            if (string.IsNullOrWhiteSpace(_eMail) || string.IsNullOrEmpty(_eMail))
            {
                ShowEmptyEmailError();
                return;
            }

            if (!EmailRegex.IsMatch(_eMail))
            {
                ShowInvalidEmailError();
                return;
            }

            if (string.IsNullOrWhiteSpace(_password) || string.IsNullOrEmpty(_password))
            {
                ShowEmptyPasswordError();
                return;
            }

            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        RememberMe();
                        ExecuteLoginCall();
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void SetLoginLocalData()
        {
            DataManager.DataManager.SharedInstance.User.UserID = _authenticationList.d.data.userID;
            //For testing only
            UserEntity uManager = new UserEntity();
            uManager.DeleteTable();
            uManager.CreateTable();
            uManager.InsertListOfItems(_authenticationList.d.data);
            DataManager.DataManager.SharedInstance.UserEntity = uManager.GetAllItems();
        }

        private void ExecuteLoginCall()
        {
            Login().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_authenticationList != null && _authenticationList?.d != null)
                    {
                        UserAuthenticationDataModel userAuthenticationModel = _authenticationList.d;
                        if (userAuthenticationModel.IsSuccess)
                        {
                            SetLoginLocalData();
                            if (LanguageUtility.DidUserChangeLanguage)
                            {
                                LanguageUtility.SaveLanguagePreference().ContinueWith(langTask =>
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        LanguageUtility.DidUserChangeLanguage = false;
                                        ProcessLogin((bool)userAuthenticationModel?.data?.IsVerifiedPhone);
                                    });
                                });
                            }
                            else
                            {
                                LanguageUtility.GetLanguagePreference().ContinueWith(langTask =>
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        if (!LanguageUtility.IsSameAsCurrentLanguage)
                                        {
                                            OnChangeLanguage((bool)userAuthenticationModel?.data?.IsVerifiedPhone);
                                        }
                                        else
                                        {
                                            ProcessLogin((bool)userAuthenticationModel?.data?.IsVerifiedPhone);
                                        }
                                    });
                                });
                            }
                        }
                        else
                        {
                            ShowServerError(userAuthenticationModel?.ErrorMessage);
                            ActivityIndicator.Hide();
                        }
                    }
                    else
                    {
                        ShowServerError(_authenticationList?.d?.ErrorMessage);
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        private void ProcessLogin(bool isPhoneVerified)
        {
            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
            bool isPasswordResetCodeSent = sharedPreference.BoolForKey("isPasswordResetCodeSent");
            if (isPasswordResetCodeSent)
            {
                //Display Password Reset
                UIStoryboard storyBoard = UIStoryboard.FromName("ForgotPassword", null);
                ResetPasswordViewController viewController =
                    storyBoard.InstantiateViewController("ResetPasswordViewController") as ResetPasswordViewController;
                viewController._username = _eMail;
                viewController._currentPassword = _password;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
                ActivityIndicator.Hide();
                txtFieldEmail.Text = string.Empty;
                txtFieldPassword.Text = string.Empty;
            }
            else
            {
                //TODO: RRA, remove temp code isVerified. 
                //bool isVerified = false;
                //if (isVerified)
                if (isPhoneVerified)
                {
                    sharedPreference.SetBool(true, TNBGlobal.PreferenceKeys.LoginState);
                    sharedPreference.SetBool(true, TNBGlobal.PreferenceKeys.PhoneVerification);
                    sharedPreference.Synchronize();
                    ExecuteGetCutomerRecordsCall();
                }
                else
                {
                    ShowUpdateMobileNumber(false);
                }
            }
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
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
            ActivityIndicator.Hide();
        }

        private Task Login()
        {
            return Task.Factory.StartNew(() =>
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                DataManager.DataManager.SharedInstance.FCMToken = sharedPreference.StringForKey("FCMToken");
                DataManager.DataManager.SharedInstance.UDID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    usrInf = new
                    {
                        eid = _eMail,
                        sspuid = string.Empty,
                        did = DataManager.DataManager.SharedInstance.UDID,
                        ft = DataManager.DataManager.SharedInstance.FCMToken != null
                        && !string.IsNullOrEmpty(DataManager.DataManager.SharedInstance.FCMToken)
                        && !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.FCMToken)
                            ? DataManager.DataManager.SharedInstance.FCMToken : string.Empty,
                        lang = TNBGlobal.APP_LANGUAGE,
                        sec_auth_k1 = TNBGlobal.API_KEY_ID,
                        sec_auth_k2 = string.Empty,
                        ses_param1 = string.Empty,
                        ses_param2 = string.Empty
                    },
                    deviceInf = new
                    {
                        DeviceId = DataManager.DataManager.SharedInstance.UDID,
                        AppVersion = AppVersionHelper.GetAppShortVersion(),
                        OsType = TNBGlobal.DEVICE_PLATFORM_IOS,
                        OsVersion = DeviceHelper.GetOSVersion(),
                        DeviceDesc = TNBGlobal.APP_LANGUAGE
                    },
                    clientType = AppVersionHelper.GetBuildVersion(),
                    password = _password
                };
                _authenticationList = serviceManager.OnExecuteAPIV6<UserAuthenticationResponseModel>(LoginConstants.Service_Login, requestParameter);
            });
        }

        private void ExecuteGetBillAccountDetailsCall()
        {
            GetBillingAccountDetails().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_billingAccountDetailsList != null && _billingAccountDetailsList?.d != null
                        && _billingAccountDetailsList?.d?.data != null)
                    {
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = _billingAccountDetailsList.d.data;
                        if (!DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount)
                        {
                            DataManager.DataManager.SharedInstance.SaveToBillingAccounts(DataManager.DataManager.SharedInstance.BillingAccountDetails
                                , DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                        }

                        UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                        UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                        loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        ShowViewController(loginVC, this);
                        ActivityIndicator.Hide();
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = new BillingAccountDetailsDataModel();
                        DisplayServiceError(_billingAccountDetailsList?.d?.message);
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private Task GetBillingAccountDetails()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    CANum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum
                };
                _billingAccountDetailsList = serviceManager.OnExecuteAPI<BillingAccountDetailsResponseModel>("GetBillingAccountDetails", requestParameter);
            });
        }

        private void SetupSubViews()
        {
            //Setup Corner Radius
            lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
            lblPasswordTitle.Hidden = txtFieldPassword.Text.Length == 0;
        }

        private void dismissKeyBoard()
        {
            txtFieldEmail.ResignFirstResponder();
            txtFieldPassword.ResignFirstResponder();
        }

        private void ShowForgotPassword()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("ForgotPassword", null);
            ForgotPasswordViewController viewController =
                storyBoard.InstantiateViewController("ForgotPasswordViewController") as ForgotPasswordViewController;
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
        }

        private void ShowRegister()
        {
            DisplayPage("Registration", "RegistrationViewController");
        }

        private void OnDismiss()
        {
            this.DismissViewController(true, null);
        }

        private void DisplayPage(string storyboardName, string storyboardID)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName(storyboardName, null);
            UIViewController viewController =
                storyBoard.InstantiateViewController(storyboardID) as UIViewController;
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
        }

        private void ExecuteGetCutomerRecordsCall()
        {
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

                        if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                           && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null)
                        {
                            if (DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0)
                            {
                                DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList.d[0];
                                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                                UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                                loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                ShowViewController(loginVC, this);
                                ActivityIndicator.Hide();
                            }
                            else
                            {
                                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                                UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                                loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                ShowViewController(loginVC, this);
                                ActivityIndicator.Hide();
                            }
                        }
                        else
                        {
                            DataManager.DataManager.SharedInstance.AccountRecordsList = new CustomerAccountRecordListModel();
                            DataManager.DataManager.SharedInstance.AccountRecordsList.d = new List<CustomerAccountRecordModel>();

                            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                            UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                            loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            ShowViewController(loginVC, this);
                            ActivityIndicator.Hide();
                        }
                        txtFieldEmail.Text = string.Empty;
                        txtFieldPassword.Text = string.Empty;
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.ClearLoginState();
                        ShowServerError(DataManager.DataManager.SharedInstance.CustomerAccounts?.d?.DisplayMessage);
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        #region Language
        /*Todo: Do service calls and set lang
         * 1. Call site core
         * 2. Call Applaunch master data
         * 3. Clear Usage cache for service call content
        */
        private bool _isMasterDataDone, _isSitecoreDone;
        private void OnChangeLanguage(bool isPhoneVerified)
        {
            int index = 0;
            if (TNBGlobal.APP_LANGUAGE != "EN")
            {
                index = 1;
            }
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        LanguageUtility.SetAppLanguageByIndex(index);
                        InvokeOnMainThread(async () =>
                        {
                            List<Task> taskList = new List<Task>{
                                OnGetAppLaunchMasterData(isPhoneVerified),
                                OnExecuteSiteCore(isPhoneVerified)
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

        private void ChangeLanguageCallback(bool isPhoneVerified)
        {
            if (_isMasterDataDone && _isSitecoreDone)
            {
                InvokeOnMainThread(() =>
                {
                    //Todo: Check success and fail States
                    ClearCache();
                    Debug.WriteLine("Change Language Done");
                    NotifCenterUtility.PostNotificationName("LanguageDidChange", new NSObject());
                    ProcessLogin(isPhoneVerified);
                });
            }
        }

        private void ClearCache()
        {
            AccountUsageCache.ClearCache();
            AccountUsageSmartCache.ClearCache();
        }

        private Task OnGetAppLaunchMasterData(bool isPhoneVerified)
        {
            return Task.Factory.StartNew(() =>
            {
                AppLaunchResponseModel response = ServiceCall.GetAppLaunchMasterData().Result;
                AppLaunchMasterCache.AddAppLaunchResponseData(response);
                _isMasterDataDone = true;
                ChangeLanguageCallback(isPhoneVerified);
            });
        }

        private Task OnExecuteSiteCore(bool isPhoneVerified)
        {
            return Task.Factory.StartNew(async () =>
            {
                await SitecoreServices.Instance.OnExecuteSitecoreCall(true);
                _isSitecoreDone = true;
                ChangeLanguageCallback(isPhoneVerified);
            });
        }
        #endregion
    }
}