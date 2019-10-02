using System;
using UIKit;
using myTNB.Model;
using CoreAnimation;
using Foundation;
using CoreGraphics;
using Cirrious.FluentLayouts.Touch;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using myTNB.Login.ForgotPassword;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.DataManager;
using System.Collections.Generic;

namespace myTNB
{
    public partial class LoginViewController : CustomUIViewController
    {
        readonly Regex EmailRegex = new Regex(@"[a-zA-Z0-9\\+\\.\\_\\%\\-\\+]{1,256}" + "\\@"
                                                      + "[a-zA-Z0-9][a-zA-Z0-9\\-]{0,64}"
                                                      + "("
                                                      + "\\."
                                                      + "[a-zA-Z0-9][a-zA-Z0-9\\-]{0,25}"
                                                      + ")+");
        string _eMail = string.Empty;
        string _password = string.Empty;
        bool IsRememberMe = true;

        UserAuthenticationResponseModel _authenticationList = new UserAuthenticationResponseModel();
        BillingAccountDetailsResponseModel _billingAccountDetailsList = new BillingAccountDetailsResponseModel();
        TextFieldHelper _textFieldHelper = new TextFieldHelper();

        UIView viewEmail, viewPassword, viewRememberMe, viewForgotPassword, viewShowPassword
            , viewLineEmail, viewLinePassword, viewCheckBox;

        UILabel lblEmailTitle, lblPasswordTitle, lblRememberMe;

        UITextField txtFieldEmail, txtFieldPassword;
        UIImageView imgViewCheckBox;

        public LoginViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddBackButton();
            InitializeSubViews();
            Setevents();
            SetupFonts();
            SetupSuperViewBackground();
            SetupSubViews();
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
            txtFieldEmail.Text = string.Empty;
            txtFieldPassword.Text = string.Empty;
        }

        void InitializeSubViews()
        {
            viewEmail = new UIView(new CGRect(18, 184, View.Frame.Width - 36, 51));

            lblEmailTitle = new UILabel(new CGRect(0, 0, viewEmail.Frame.Width, 12))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans9_300,
                TextColor = MyTNBColor.LightGray,
                Text = "Common_Email".Translate().ToUpper()
            };

            txtFieldEmail = new UITextField
            {
                Frame = new CGRect(0, 12, viewEmail.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Common_Email".Translate(),
                    font: MyTNBFont.MuseoSans18_300,
                    foregroundColor: MyTNBColor.LightGray,
                    strokeWidth: 0
                ),
                TextColor = UIColor.White
            };
            txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;
            txtFieldEmail.TintColor = MyTNBColor.SunGlow;
            _textFieldHelper.CreateTextFieldLeftView(txtFieldEmail, "email_white");
            _textFieldHelper.SetKeyboard(txtFieldEmail);

            viewLineEmail = new UIView((new CGRect(0, 36, viewEmail.Frame.Width, 1)));
            viewLineEmail.BackgroundColor = UIColor.White;

            viewEmail.AddSubviews(new UIView[] { lblEmailTitle, txtFieldEmail, viewLineEmail });

            viewPassword = new UIView(new CGRect(18, 241, View.Frame.Width - 36, 53));

            lblPasswordTitle = new UILabel(new CGRect(0, 0, viewPassword.Frame.Width, 12))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans9_300,
                TextColor = MyTNBColor.LightGray,
                Text = "Common_Password".Translate().ToUpper()
            };

            txtFieldPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Common_Password".Translate(),
                    font: MyTNBFont.MuseoSans18_300,
                    foregroundColor: MyTNBColor.LightGray,
                    strokeWidth: 0
                ),
                TextColor = UIColor.White
            };
            txtFieldPassword.KeyboardType = UIKeyboardType.Default;
            txtFieldPassword.TintColor = MyTNBColor.SunGlow;
            txtFieldPassword.SecureTextEntry = true;
            _textFieldHelper.SetKeyboard(txtFieldPassword);
            _textFieldHelper.CreateTextFieldLeftView(txtFieldPassword, "password_white");

            viewLinePassword = new UIView((new CGRect(0, 36, viewPassword.Frame.Width, 1)))
            {
                BackgroundColor = UIColor.White
            };

            viewForgotPassword = new UIView(new CGRect(0, 37, viewPassword.Frame.Width, 16));
            UILabel lblForgetPassword = new UILabel(new CGRect(0, 0, viewForgotPassword.Frame.Width, 16))
            {
                TextAlignment = UITextAlignment.Right,
                Font = MyTNBFont.MuseoSans12_500,
                TextColor = UIColor.White,
                Text = "Login_ForgotPassword".Translate()
            };
            viewForgotPassword.AddSubview(lblForgetPassword);

            viewShowPassword = new UIView(new CGRect(viewPassword.Frame.Width - 30, 12, 24, 24))
            {
                Hidden = true
            };
            UIImageView imgShowPassword = new UIImageView(new CGRect(0, 0, 24, 24));
            imgShowPassword.Image = UIImage.FromBundle("IC-Action-Show-PasswordWhite");
            viewShowPassword.AddSubview(imgShowPassword);
            viewShowPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                imgShowPassword.Image = UIImage.FromBundle(txtFieldPassword.SecureTextEntry
                    ? "IC-Action-Hide-PasswordWhite" : "IC-Action-Show-PasswordWhite");
                txtFieldPassword.SecureTextEntry = !txtFieldPassword.SecureTextEntry;
            }));

            viewPassword.AddSubviews(new UIView[] { lblPasswordTitle, txtFieldPassword
                , viewLinePassword, viewForgotPassword, viewShowPassword });

            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var emailString = sharedPreference.StringForKey(TNBGlobal.PreferenceKeys.RememberEmail);
            if (!string.IsNullOrEmpty(emailString))
            {
                txtFieldEmail.Text = emailString;
                txtFieldEmail.LeftViewMode = UITextFieldViewMode.Never;
            }
            viewRememberMe = new UIView(new CGRect(18, viewPassword.Frame.GetMaxY() + DeviceHelper.GetScaledHeight(20), View.Frame.Width - 36, 25));

            viewCheckBox = new UIView(new CGRect(0, 0, 24, 24));
            imgViewCheckBox = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("Payment-Checkbox-Active")
            };
            viewCheckBox.AddSubview(imgViewCheckBox);

            viewCheckBox.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                IsRememberMe = !IsRememberMe;
                imgViewCheckBox.Image = UIImage.FromBundle(IsRememberMe
                    ? "Payment-Checkbox-Active" : "Payment-Checkbox-White-Inactive");
            }));

            lblRememberMe = new UILabel(new CGRect(viewCheckBox.Frame.Width + 6, 4, viewRememberMe.Frame.Width, 16))
            {
                Font = MyTNBFont.MuseoSans12_500,
                TextColor = UIColor.White,
                Text = "Login_RememberMe".Translate()
            };

            viewRememberMe.AddSubviews(new UIView[] { viewCheckBox, lblRememberMe });

            View.AddSubviews(new UIView[] { viewEmail, viewPassword, viewRememberMe });
        }
        /// <summary>
        /// Method to remember or not the email based user selection when logging in
        /// </summary>
        private void RememberMe()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
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

        void SetupSuperViewBackground()
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            var gradientLayer = new CAGradientLayer();
            gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
            gradientLayer.Locations = new NSNumber[] { 0, 1 };
            gradientLayer.Frame = View.Bounds;
            View.Layer.InsertSublayer(gradientLayer, 0);
        }

        void Setevents()
        {

            btnLogin.TouchUpInside += (sender, e) =>
            {
                OnLogin();
            };

            viewForgotPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ShowForgotPassword();
            }));

            btnRegister.TouchUpInside += (sender, e) =>
            {
                ShowRegister();
            };

            txtFieldEmail.EditingChanged += (sender, e) =>
            {
                lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
            };

            txtFieldEmail.EditingDidBegin += (sender, e) =>
            {
                lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
                viewLineEmail.BackgroundColor = MyTNBColor.SunGlow;
                txtFieldEmail.LeftViewMode = UITextFieldViewMode.Never;
            };

            txtFieldEmail.ShouldEndEditing = (sender) =>
            {
                lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
                return true;
            };

            txtFieldEmail.EditingDidEnd += (sender, e) =>
            {
                viewLineEmail.BackgroundColor = UIColor.White;
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
                viewLinePassword.BackgroundColor = MyTNBColor.SunGlow;
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
                viewLinePassword.BackgroundColor = UIColor.White;
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

        void ShowEmptyEmailError()
        {
            DisplayToast("Error_EmailRequired".Translate());
        }

        void ShowInvalidEmailError()
        {
            DisplayToast("Invalid_Email".Translate());
        }

        void ShowEmptyPasswordError()
        {
            DisplayToast("Error_PasswordRequired".Translate());
        }

        void ShowServerError(string errorMessage)
        {
            DisplayToast(errorMessage ?? "Error_DefaultMessage".Translate());
        }

        void OnLogin()
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

        void SetLoginLocalData()
        {
            DataManager.DataManager.SharedInstance.User.UserID = _authenticationList.d.data.userID;
            //For testing only
            UserEntity uManager = new UserEntity();
            uManager.DeleteTable();
            uManager.CreateTable();
            uManager.InsertListOfItems(_authenticationList.d.data);
            DataManager.DataManager.SharedInstance.UserEntity = uManager.GetAllItems();
        }

        void ExecuteLoginCall()
        {
            Login().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_authenticationList != null && _authenticationList?.d != null)
                    {
                        var userAuthenticationModel = _authenticationList.d;
                        if (userAuthenticationModel?.isError == "false" && userAuthenticationModel?.status != "failed")
                        {
                            SetLoginLocalData();
                            var sharedPreference = NSUserDefaults.StandardUserDefaults;
                            var isPasswordResetCodeSent = sharedPreference.BoolForKey("isPasswordResetCodeSent");
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
                            }
                            else
                            {
                                //TODO: RRA, remove temp code isVerified. 
                                //bool isVerified = false;
                                //if (isVerified)
                                if ((bool)userAuthenticationModel?.data?.IsVerifiedPhone)
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
                        else
                        {
                            ShowServerError(userAuthenticationModel?.message);
                            ActivityIndicator.Hide();
                        }
                    }
                    else
                    {
                        ShowServerError(_authenticationList?.d?.message);
                        ActivityIndicator.Hide();
                    }
                });
            });
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

        Task Login()
        {
            return Task.Factory.StartNew(() =>
            {
                var sharedPreference = NSUserDefaults.StandardUserDefaults;
                DataManager.DataManager.SharedInstance.FCMToken = sharedPreference.StringForKey("FCMToken");
                DataManager.DataManager.SharedInstance.UDID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    userName = _eMail,
                    password = _password,
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    ipAddress = TNBGlobal.API_KEY_ID,
                    clientType = AppVersionHelper.GetBuildVersion(),
                    activeUserName = TNBGlobal.API_KEY_ID,
                    devicePlatform = TNBGlobal.DEVICE_PLATFORM_IOS,
                    deviceVersion = DeviceHelper.GetOSVersion(),
                    deviceCordova = TNBGlobal.API_KEY_ID,
                    deviceId = DataManager.DataManager.SharedInstance.UDID,
                    fcmToken = DataManager.DataManager.SharedInstance.FCMToken != null
                        && !string.IsNullOrEmpty(DataManager.DataManager.SharedInstance.FCMToken)
                        && !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.FCMToken)
                            ? DataManager.DataManager.SharedInstance.FCMToken : string.Empty
                };
                _authenticationList = serviceManager.OnExecuteAPI<UserAuthenticationResponseModel>("IsUserAuthenticate", requestParameter);
            });
        }

        void ExecuteGetBillAccountDetailsCall()
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

        Task GetBillingAccountDetails()
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

        void SetupSubViews()
        {
            //Setup Corner Radius
            btnLogin.Layer.CornerRadius = 5;
            lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
            lblPasswordTitle.Hidden = txtFieldPassword.Text.Length == 0;
        }

        void SetupFonts()
        {
            //Labels
            lblWelcome.Font = MyTNBFont.MuseoSans26_500;
            lblWelcome.Text = "Login_Welcome".Translate();
            lblAccountLogin.Font = MyTNBFont.MuseoSans18_300;
            lblAccountLogin.Text = "Login_AccountLogin".Translate();
            lblNoAccount.Font = MyTNBFont.MuseoSans12_300;
            lblNoAccount.Text = "Login_NoTNBAccount".Translate();

            //Buttons
            btnRegister.TitleLabel.Font = MyTNBFont.MuseoSans12_500;
            btnRegister.SetTitle("Login_RegisterForAccount".Translate(), UIControlState.Normal);
            btnLogin.TitleLabel.Font = MyTNBFont.MuseoSans16_500;
            btnLogin.SetTitle("Login_Login".Translate(), UIControlState.Normal);
        }

        void dismissKeyBoard()
        {
            txtFieldEmail.ResignFirstResponder();
            txtFieldPassword.ResignFirstResponder();
        }

        void ShowForgotPassword()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("ForgotPassword", null);
            ForgotPasswordViewController viewController =
                storyBoard.InstantiateViewController("ForgotPasswordViewController") as ForgotPasswordViewController;
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
        }

        void ShowRegister()
        {
            DisplayPage("Registration", "RegistrationViewController");
        }

        void AddBackButton()
        {
            UIButton btnBack = new UIButton(UIButtonType.Custom);
            btnBack.Frame = new CGRect(0f, 0f, 0f, 0f);
            btnBack.SetImage(UIImage.FromBundle("Back-White"), UIControlState.Normal);
            btnBack.Layer.BorderColor = UIColor.White.CGColor;
            btnBack.BackgroundColor = UIColor.Clear;

            btnBack.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(btnBack);
            View.AddConstraints(
                btnBack.AtTopOf(View, DeviceHelper.IsIphoneXUpResolution() ? 50 : 26)
                , btnBack.AtLeftOf(View, 18)
                , btnBack.Width().EqualTo(24)
                , btnBack.Height().EqualTo(24)
             );

            btnBack.TouchUpInside += (sender, e) =>
            {
                OnDismiss();
            };
        }

        void OnDismiss()
        {
            this.DismissViewController(true, null);
        }

        void DisplayPage(string storyboardName, string storyboardID)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName(storyboardName, null);
            UIViewController viewController =
                storyBoard.InstantiateViewController(storyboardID) as UIViewController;
            UINavigationController navController = new UINavigationController(viewController);
            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(navController, true, null);
        }

        void ExecuteGetCutomerRecordsCall()
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
                    }
                    else
                    {
                        ShowServerError(DataManager.DataManager.SharedInstance.CustomerAccounts?.d?.DisplayMessage);
                        ActivityIndicator.Hide();
                    }
                });
            });
        }
    }
}