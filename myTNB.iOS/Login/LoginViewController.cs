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

        public LoginViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = LoginConstants.PageName;
            base.ViewDidLoad();
            InitializeSubViews();
            Setevents();
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
        }

        void InitializeSubViews()
        {
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

            View.AddSubviews(new UIView[] { imgHeader, btnBack, imgLogo, lblTitle });

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

            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var emailString = sharedPreference.StringForKey(TNBGlobal.PreferenceKeys.RememberEmail);
            if (!string.IsNullOrEmpty(emailString))
            {
                txtFieldEmail.Text = emailString;
                txtFieldEmail.LeftViewMode = UITextFieldViewMode.Never;
            }
            View.AddSubviews(new UIView[] { viewEmail, viewRememberMe, viewPassword, viewForgotPassword });

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

        void Setevents()
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
                                txtFieldEmail.Text = string.Empty;
                                txtFieldPassword.Text = string.Empty;
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
            lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
            lblPasswordTitle.Hidden = txtFieldPassword.Text.Length == 0;
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
    }
}