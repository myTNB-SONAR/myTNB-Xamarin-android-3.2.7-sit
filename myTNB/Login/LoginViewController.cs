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
    public partial class LoginViewController : UIViewController
    {
		readonly Regex EmailRegex = new Regex(@"[a-zA-Z0-9\\+\\.\\_\\%\\-\\+]{1,256}" 
                                                      + "\\@" 
                                                      + "[a-zA-Z0-9][a-zA-Z0-9\\-]{0,64}" 
                                                      + "(" 
                                                      + "\\." 
                                                      + "[a-zA-Z0-9][a-zA-Z0-9\\-]{0,25}" 
                                                      + ")+");
        string _eMail = string.Empty;
        string _password = string.Empty;
        bool isAnimating = false;

        UserAuthenticationResponseModel _authenticationList = new UserAuthenticationResponseModel();
        BillingAccountDetailsResponseModel _billingAccountDetailsList = new BillingAccountDetailsResponseModel();
        TextFieldHelper _textFieldHelper = new TextFieldHelper();

        UIView viewEmail;
        UIView viewPassword;

        UILabel lblEmailTitle;
        UILabel lblPasswordTitle;

        UITextField txtFieldEmail;
        UITextField txtFieldPassword;

        UIView viewForgotPassword;
        UIView viewShowPassword;

        public LoginViewController (IntPtr handle) : base (handle)
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
            HideDialog();
            dismissKeyBoard();
		}

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            viewNotifDialog.Hidden = true;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (viewNotifDialog.Hidden == false)
            {
                viewNotifDialog.Hidden = true;
            }
            if (viewErrorDialog.Hidden == false)
            {
                viewErrorDialog.Hidden = true;
            }
            txtFieldEmail.Text = "";
            txtFieldPassword.Text = "";
        }

        void InitializeSubViews(){
            viewEmail = new UIView(new CGRect(18, 184, View.Frame.Width - 36, 51));

            lblEmailTitle = new UILabel(new CGRect(0,0,viewEmail.Frame.Width, 12));
            lblEmailTitle.TextAlignment = UITextAlignment.Left;
            lblEmailTitle.Font = myTNBFont.MuseoSans9();
            lblEmailTitle.TextColor = myTNBColor.LightGray();
            lblEmailTitle.Text = "EMAIL";

            txtFieldEmail = new UITextField
            {
                Frame = new CGRect(0, 12, viewEmail.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Email",
                    font: myTNBFont.MuseoSans16(),
                    foregroundColor: myTNBColor.LightGray(),
                    strokeWidth: 0
                ),
                TextColor = UIColor.White
            };
            txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;
            _textFieldHelper.CreateTextFieldLeftView(txtFieldEmail, "email_white");
            _textFieldHelper.SetKeyboard(txtFieldEmail);

            UIView viewLineEmail = new UIView((new CGRect(0, 36, viewEmail.Frame.Width, 1)));
            viewLineEmail.BackgroundColor = UIColor.White;

            viewEmail.AddSubviews(new UIView[] { lblEmailTitle, txtFieldEmail, viewLineEmail });

            viewPassword = new UIView(new CGRect(18, 241, View.Frame.Width - 36, 53));

            lblPasswordTitle = new UILabel(new CGRect(0, 0, viewPassword.Frame.Width, 12));
            lblPasswordTitle.TextAlignment = UITextAlignment.Left;
            lblPasswordTitle.Font = myTNBFont.MuseoSans9();
            lblPasswordTitle.TextColor = myTNBColor.LightGray();
            lblPasswordTitle.Text = "PASSWORD";

            txtFieldPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Password",
                    font: myTNBFont.MuseoSans16(),
                    foregroundColor: myTNBColor.LightGray(),
                    strokeWidth: 0
                ),
                TextColor = UIColor.White
            };
            txtFieldPassword.KeyboardType = UIKeyboardType.Default;
            txtFieldPassword.SecureTextEntry = true;
            _textFieldHelper.SetKeyboard(txtFieldPassword);
            _textFieldHelper.CreateTextFieldLeftView(txtFieldPassword, "password_white");

            UIView viewLinePassword = new UIView((new CGRect(0, 36, viewPassword.Frame.Width, 1)));
            viewLinePassword.BackgroundColor = UIColor.White;

            viewForgotPassword = new UIView(new CGRect(0, 37, viewPassword.Frame.Width, 16));
            UILabel lblForgetPassword = new UILabel(new CGRect(0, 0, viewForgotPassword.Frame.Width, 16));
            lblForgetPassword.TextAlignment = UITextAlignment.Right;
            lblForgetPassword.Font = myTNBFont.MuseoSans12();
            lblForgetPassword.TextColor = myTNBColor.LightGray();
            lblForgetPassword.Text = "Forgot password?";
            viewForgotPassword.AddSubview(lblForgetPassword);

            viewShowPassword = new UIView(new CGRect(viewPassword.Frame.Width - 30, 12, 24, 24));
            viewShowPassword.Hidden = true;
            UIImageView imgShowPassword = new UIImageView(new CGRect(0, 0, 24, 24));
            imgShowPassword.Image = UIImage.FromBundle("IC-Action-Show-PasswordWhite");
            viewShowPassword.AddSubview(imgShowPassword);
            viewShowPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (txtFieldPassword.SecureTextEntry)
                {
                    txtFieldPassword.SecureTextEntry = false;
                }
                else
                {
                    txtFieldPassword.SecureTextEntry = true;
                }
            }));

            viewPassword.AddSubviews(new UIView[] { lblPasswordTitle, txtFieldPassword
                , viewLinePassword, viewForgotPassword, viewShowPassword });

            View.AddSubviews(new UIView[]{viewEmail, viewPassword});
        }

		void SetupSuperViewBackground()
		{
			var startColor = myTNBColor.GradientPurpleDarkElement();
			var endColor = myTNBColor.GradientPurpleLightElement();
			var gradientLayer = new CAGradientLayer();
			gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
			gradientLayer.Locations = new NSNumber[] { 0, 1 };
			gradientLayer.Frame = View.Bounds;
			View.Layer.InsertSublayer(gradientLayer, 0);
		}

        void Setevents(){
            
            btnLogin.TouchUpInside += (sender, e) => {
                OnLogin();
            };

            viewForgotPassword.AddGestureRecognizer(new UITapGestureRecognizer(()=>{
                ShowForgotPassword();
            }));

            btnRegister.TouchUpInside += (sender, e) => {
                ShowRegister();
            };

            txtFieldEmail.EditingChanged += (sender, e) => {
                lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
            };

			txtFieldEmail.EditingDidBegin += (sender, e) => {
                lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
			};

			txtFieldEmail.ShouldEndEditing = (sender) => {
                lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
				return true;
			};

            txtFieldEmail.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };

			txtFieldEmail.ShouldBeginEditing = (sender) => {
				HideDialog();
				return true;
			};

            txtFieldPassword.EditingChanged += (sender, e) => {
                lblPasswordTitle.Hidden = txtFieldPassword.Text.Length == 0;
                viewShowPassword.Hidden = txtFieldPassword.Text.Length == 0;
			};

			txtFieldPassword.EditingDidBegin += (sender, e) => {
                lblPasswordTitle.Hidden = txtFieldPassword.Text.Length == 0;
                viewShowPassword.Hidden = txtFieldPassword.Text.Length == 0;
			};

			txtFieldPassword.ShouldEndEditing = (sender) => {
                lblPasswordTitle.Hidden = txtFieldPassword.Text.Length == 0;
                viewShowPassword.Hidden = txtFieldPassword.Text.Length == 0;
				return true;
			};

			txtFieldPassword.ShouldReturn = (sender) => {
				sender.ResignFirstResponder();
				return false;
			};

			txtFieldPassword.ShouldBeginEditing = (sender) => {
				HideDialog();
				return true;
			};
        }

		void showEmptyEmailError() {
			lblErrorMessage.Text = "Email field is required.";
            ShowViewPinSent(viewErrorDialog);
		}

        void showInvalidEmailError() {
			lblErrorMessage.Text = "Invalid email address format.";
            ShowViewPinSent(viewErrorDialog);
        }

        void showEmptyPasswordError() {
			lblErrorMessage.Text = "Password field is required.";
            ShowViewPinSent(viewErrorDialog);
        }

        void ShowServerError(string errorMessage) {
            lblNotifMessage.Text = errorMessage;
            ShowViewPinSent(viewNotifDialog);
        }

        void ShowViewPinSent(UIView viewDialog)
        {
            if(!isAnimating){
                isAnimating = true;
            }else{
                isAnimating = false;
                viewDialog.Hidden = true;
            }
            viewDialog.Hidden = false;
            viewDialog.Alpha = 1.0f;
            UIView.Animate(5, 1, UIViewAnimationOptions.CurveEaseOut, () => {
                viewDialog.Alpha = 0.0f;
            }, () => {
                viewDialog.Hidden = true;
                viewDialog.Hidden = false;
            });
        }

		void OnLogin(){
            _eMail = txtFieldEmail.Text.Trim();
            _password = txtFieldPassword.Text.Trim();
            if (string.IsNullOrWhiteSpace(_eMail) || string.IsNullOrEmpty(_eMail))
            {
                showEmptyEmailError();
                return;
            }

			if (!EmailRegex.IsMatch(_eMail))
			{
                showInvalidEmailError();
				return;
			}

			if (string.IsNullOrWhiteSpace(_password) || string.IsNullOrEmpty(_password))
			{
                showEmptyPasswordError();
				return;
			}

			NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
			{
				InvokeOnMainThread(() => {
					if (NetworkUtility.isReachable)
					{
                        ActivityIndicator.Show();
                        ExecuteLoginCall();
					}
					else
					{
                        Console.WriteLine("No Network");
                        var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
					}
				});
			});
        }

        void SetLoginLocalData(){
            DataManager.DataManager.SharedInstance.User.UserID = _authenticationList.d.data.userID;
            //For testing only
            UserEntity uManager = new UserEntity();
            uManager.DeleteTable();
            uManager.CreateTable();
            uManager.InsertListOfItems(_authenticationList.d.data);
            DataManager.DataManager.SharedInstance.UserEntity = uManager.GetAllItems();
        }

        void ExecuteLoginCall(){
            Login().ContinueWith(task => { 
                InvokeOnMainThread(()=>{
                    if(_authenticationList != null && _authenticationList.d != null){
                        var userAuthenticationModel = _authenticationList.d;
                        if (userAuthenticationModel.isError == "false" && userAuthenticationModel.status != "failed")
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
                                var navController = new UINavigationController(viewController);
                                PresentViewController(navController, true, null);
                                ActivityIndicator.Hide();
                            }
                            else
                            {
                                sharedPreference.SetBool(true, "isLogin");
                                sharedPreference.Synchronize();
                                ExecuteGetCutomerRecordsCall();
                            }
                        }
                        else
                        {
                            ShowServerError(userAuthenticationModel.message);
                            ActivityIndicator.Hide();
                        }
                    }else{
                        ShowServerError("Something went wrong. Please try again later.");
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        Task Login(){
            return Task.Factory.StartNew(() => {
                var sharedPreference = NSUserDefaults.StandardUserDefaults;
                DataManager.DataManager.SharedInstance.FCMToken = sharedPreference.StringForKey("FCMToken");
                DataManager.DataManager.SharedInstance.UDID = UIDevice.CurrentDevice.IdentifierForVendor.ToString();
				ServiceManager serviceManager = new ServiceManager();
				object requestParameter = new
				{
                    userName = _eMail,
                    password = _password,
                    apiKeyID = TNBGlobal.API_KEY_ID,
					ipAddress = TNBGlobal.API_KEY_ID,
					clientType = TNBGlobal.API_KEY_ID,
					activeUserName = TNBGlobal.API_KEY_ID,
					devicePlatform = TNBGlobal.API_KEY_ID,
					deviceVersion = TNBGlobal.API_KEY_ID,
                    deviceCordova = TNBGlobal.API_KEY_ID,
                    deviceId = DataManager.DataManager.SharedInstance.UDID,
                    fcmToken = DataManager.DataManager.SharedInstance.FCMToken != null
                                          && !string.IsNullOrEmpty(DataManager.DataManager.SharedInstance.FCMToken)
                                          && !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.FCMToken)
                                          ? DataManager.DataManager.SharedInstance.FCMToken
                                          : ""
				};
				_authenticationList = serviceManager.GetUserAuthentication("IsUserAuthenticate", requestParameter);
			});
        }

        void ExecuteGetCutomerRecordsCall()
        {
            ServiceCall.GetCustomerBillingAccountList().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if(DataManager.DataManager.SharedInstance.CustomerAccounts != null 
                       && DataManager.DataManager.SharedInstance.CustomerAccounts.d != null
                       && DataManager.DataManager.SharedInstance.CustomerAccounts.d.data != null){
                        DataManager.DataManager.SharedInstance.AccountRecordsList.d 
                                   = DataManager.DataManager.SharedInstance.CustomerAccounts.d.data;
                    }

                    if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                        && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
                    {
                        UserAccountsEntity uaManager = new UserAccountsEntity();
                        uaManager.DeleteTable();
                        uaManager.CreateTable();
                        uaManager.InsertListOfItems(DataManager.DataManager.SharedInstance.AccountRecordsList);
                    }

                    if(DataManager.DataManager.SharedInstance.AccountRecordsList != null
                       && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null){
                        if(DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count > 0){
                            DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList.d[0];
                            ExecuteGetBillAccountDetailsCall();
                        }else{
                            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                            UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                            ShowViewController(loginVC, this);
                            ActivityIndicator.Hide();
                        }
                    }else{
                        DataManager.DataManager.SharedInstance.AccountRecordsList = new CustomerAccountRecordListModel();
                        DataManager.DataManager.SharedInstance.AccountRecordsList.d = new List<CustomerAccountRecordModel>();

                        UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                        UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                        ShowViewController(loginVC, this);
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        void ExecuteGetBillAccountDetailsCall()
        {
            GetBillingAccountDetails().ContinueWith(task =>
            {
                InvokeOnMainThread(() => {
                    if (_billingAccountDetailsList != null && _billingAccountDetailsList.d != null
                        && _billingAccountDetailsList.d.data != null)
                    {
                        PushNotificationHelper.GetNotifications();
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = _billingAccountDetailsList.d.data;
                        UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                        UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                        ShowViewController(loginVC, this);
                        ActivityIndicator.Hide();
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = new BillingAccountDetailsDataModel();
                        var alert = UIAlertController.Create("Unable to Login", "Something went wrong. Please try again later.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
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
                _billingAccountDetailsList = serviceManager.GetBillingAccountDetails("GetBillingAccountDetails", requestParameter);
            });
        }

        void SetupSubViews(){             //Setup Corner Radius             btnLogin.Layer.CornerRadius = 5;
            viewErrorDialog.Layer.CornerRadius = 5;
            viewNotifDialog.Layer.CornerRadius = 5;

            lblEmailTitle.Hidden = txtFieldEmail.Text.Length == 0;
            lblPasswordTitle.Hidden = txtFieldPassword.Text.Length == 0;
        }

        void SetupFonts() {
            //Labels
            lblWelcome.Font = myTNBFont.MuseoSans24();             lblAccountLogin.Font = myTNBFont.MuseoSans16();             lblNoAccount.Font = myTNBFont.MuseoSans12();              //Buttons             btnRegister.TitleLabel.Font = myTNBFont.MuseoSans12();             btnLogin.TitleLabel.Font = myTNBFont.MuseoSans16();         }

        void HideDialog() {
			if (viewErrorDialog.Hidden == false)
			{
				viewErrorDialog.Hidden = true;
			}
			else if (viewNotifDialog.Hidden == false)
			{
				viewNotifDialog.Hidden = true;
			}
        }

        void dismissKeyBoard() {
			txtFieldEmail.ResignFirstResponder();
			txtFieldPassword.ResignFirstResponder();
        }

        void ShowForgotPassword() {
			UIStoryboard storyBoard = UIStoryboard.FromName("ForgotPassword", null);
			ForgotPasswordViewController viewController =
				storyBoard.InstantiateViewController("ForgotPasswordViewController") as ForgotPasswordViewController;
			var navController = new UINavigationController(viewController);
			PresentViewController(navController, true, null);
		}

        void ShowRegister() {
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
                btnBack.AtTopOf(View, DeviceHelper.IsIphoneX() ? 50 : 26)
                , btnBack.AtLeftOf(View, 18)
                , btnBack.Width().EqualTo(24)
                , btnBack.Height().EqualTo(24)
			 );

			btnBack.TouchUpInside += (sender, e) => {
				OnDismiss();
			};
		}

		void OnDismiss()
		{
            UIStoryboard loginStoryboard = UIStoryboard.FromName("Login", null);
            UIViewController preloginVC = (UIViewController)loginStoryboard.InstantiateViewController("PreloginViewController");
            preloginVC.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            PresentViewController(preloginVC, true, null);
		}

        void DisplayPage(string storyboardName, string storyboardID){
            UIStoryboard storyBoard = UIStoryboard.FromName(storyboardName, null);
            UIViewController viewController =
                storyBoard.InstantiateViewController(storyboardID) as UIViewController;
            var navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
        }
    }
}