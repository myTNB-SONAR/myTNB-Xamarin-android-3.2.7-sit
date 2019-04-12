using System;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using myTNB.DataManager;
using myTNB.Model;
using UIKit;


namespace myTNB.Login.ForgotPassword
{
    public partial class ResetPasswordViewController : UIViewController
    {
        public ResetPasswordViewController(IntPtr handle) : base(handle)
        {
        }

        public string _username = string.Empty;
        public string _currentPassword = string.Empty;
        string _password = string.Empty;
        string _confirmPassword = string.Empty;

        UILabel lblPasswordTitle;
        UITextField txtFieldPassword;
        UILabel lblPasswordError;
        UIView viewLinePassword;

        UILabel lblConfirmPasswordTitle;
        UITextField txtFieldConfirmPassword;
        UILabel lblConfirmPasswordError;
        UIView viewLineConfirmPassword;

        UIView viewShowConfirmPassword;
        UIView viewShowPassword;

        UILabel lblTitle;

        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        BaseResponseModel _changePasswordList = new BaseResponseModel();
        const string PASSWORD_PATTERN = @"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8,})$";

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            this.NavigationItem.HidesBackButton = true;
            AddBackButton();

            InitializeSubviews();

            SetStaticFields();
            SetViews();
            SetEvents();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        internal void InitializeSubviews()
        {
            lblTitle = new UILabel(new CGRect(18, 16, View.Frame.Width - 36, 18));
            lblTitle.TextColor = myTNBColor.PowerBlue();
            lblTitle.Font = myTNBFont.MuseoSans16_500();
            lblTitle.Text = "Login_EnterPassword".Translate();
            View.AddSubview(lblTitle);

            lblDescription.TextColor = myTNBColor.TunaGrey();
            lblDescription.Font = myTNBFont.MuseoSans14();
            btnSubmit.Layer.CornerRadius = 5f;

            #region Password
            UIView viewPassword = new UIView((new CGRect(18, 98, View.Frame.Width - 36, 51)));
            viewPassword.BackgroundColor = UIColor.Clear;

            lblPasswordTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewPassword.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    "Common_Password".Translate().ToUpper()
                    , font: myTNBFont.MuseoSans9_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewPassword.AddSubview(lblPasswordTitle);

            lblPasswordError = new UILabel
            {
                Frame = new CGRect(0, 37, viewPassword.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "Hint_Password".Translate()
                    , font: myTNBFont.MuseoSans9_300()
                    , foregroundColor: myTNBColor.Tomato()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewPassword.AddSubview(lblPasswordError);

            txtFieldPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Login_EnterNewPassword".Translate()
                    , font: myTNBFont.MuseoSans16_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            txtFieldPassword.KeyboardType = UIKeyboardType.Default;
            txtFieldPassword.ReturnKeyType = UIReturnKeyType.Done;
            txtFieldPassword.SecureTextEntry = true;
            viewPassword.AddSubview(txtFieldPassword);

            viewLinePassword = new UIView((new CGRect(0, 36, viewPassword.Frame.Width, 1)));
            viewLinePassword.BackgroundColor = myTNBColor.PlatinumGrey();
            viewPassword.AddSubview(viewLinePassword);

            viewShowPassword = new UIView(new CGRect(viewPassword.Frame.Width - 30, 12, 24, 24));
            viewShowPassword.Hidden = true;
            UIImageView imgShowPassword = new UIImageView(new CGRect(0, 0, 24, 24));
            imgShowPassword.Image = UIImage.FromBundle("IC-Action-Show-Password");
            viewShowPassword.AddSubview(imgShowPassword);
            viewShowPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                txtFieldPassword.SecureTextEntry = !txtFieldPassword.SecureTextEntry;
            }));
            viewPassword.AddSubview(viewShowPassword);

            View.AddSubview(viewPassword);
            #endregion


            #region Confirm Password
            UIView viewConfirmPassword = new UIView((new CGRect(18, 156, View.Frame.Width - 36, 51)));
            viewConfirmPassword.BackgroundColor = UIColor.Clear;

            lblConfirmPasswordTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewConfirmPassword.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    "Common_ConfirmPassword".Translate().ToUpper()
                    , font: myTNBFont.MuseoSans9_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewConfirmPassword.AddSubview(lblConfirmPasswordTitle);

            lblConfirmPasswordError = new UILabel(new CGRect(0, 37, viewConfirmPassword.Frame.Width, 14));
            lblConfirmPasswordError.TextAlignment = UITextAlignment.Left;
            lblConfirmPasswordError.Font = myTNBFont.MuseoSans9_300();
            lblConfirmPasswordError.TextColor = myTNBColor.Tomato();

            viewConfirmPassword.AddSubview(lblConfirmPasswordError);

            txtFieldConfirmPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewConfirmPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Login_ConfirmNewPassword".Translate()
                    , font: myTNBFont.MuseoSans16()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            txtFieldConfirmPassword.KeyboardType = UIKeyboardType.Default;
            txtFieldConfirmPassword.ReturnKeyType = UIReturnKeyType.Done;
            txtFieldConfirmPassword.SecureTextEntry = true;
            viewConfirmPassword.AddSubview(txtFieldConfirmPassword);

            viewLineConfirmPassword = new UIView((new CGRect(0, 36, viewConfirmPassword.Frame.Width, 1)));
            viewLineConfirmPassword.BackgroundColor = myTNBColor.PlatinumGrey();
            viewConfirmPassword.AddSubview(viewLineConfirmPassword);

            viewShowConfirmPassword = new UIView(new CGRect(viewConfirmPassword.Frame.Width - 30, 12, 24, 24));
            viewShowConfirmPassword.Hidden = true;
            UIImageView imgShowConfirmPassword = new UIImageView(new CGRect(0, 0, 24, 24));
            imgShowConfirmPassword.Image = UIImage.FromBundle("IC-Action-Show-Password");
            viewShowConfirmPassword.AddSubview(imgShowConfirmPassword);
            viewConfirmPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (txtFieldConfirmPassword.SecureTextEntry)
                {
                    txtFieldConfirmPassword.SecureTextEntry = false;
                }
                else
                {
                    txtFieldConfirmPassword.SecureTextEntry = true;
                }
            }));
            viewConfirmPassword.AddSubview(viewShowConfirmPassword);

            View.AddSubview(viewConfirmPassword);
            #endregion
        }

        internal void SetStaticFields()
        {
            lblDescription.Text = "Login_ChangePasswordNote".Translate();
        }

        internal void SetViews()
        {
            lblPasswordTitle.Hidden = true;
            lblPasswordError.Hidden = true;
            lblConfirmPasswordTitle.Hidden = true;
            lblConfirmPasswordError.Hidden = true;
            btnSubmit.Enabled = false;
            btnSubmit.BackgroundColor = myTNBColor.PlatinumGrey();
            _textFieldHelper.CreateTextFieldLeftView(txtFieldPassword, "Password");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldConfirmPassword, "Password");
            btnSubmit.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution()
                ? 184 : DeviceHelper.GetScaledHeight(130)), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48));
        }

        internal void SetEvents()
        {
            btnSubmit.TouchUpInside += (sender, e) =>
            {
                ActivityIndicator.Show();
                _password = txtFieldPassword.Text;
                _confirmPassword = txtFieldConfirmPassword.Text;

                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            ExecuteChangePasswordCall();
                        }
                        else
                        {
                            AlertHandler.DisplayNoDataAlert(this);
                            ActivityIndicator.Hide();
                        }
                    });
                });
            };
            SetTextFieldEvents(txtFieldPassword, lblPasswordTitle, lblPasswordError, viewLinePassword, PASSWORD_PATTERN);
            SetTextFieldEvents(txtFieldConfirmPassword, lblConfirmPasswordTitle, lblConfirmPasswordError, viewLineConfirmPassword, PASSWORD_PATTERN);
        }

        internal void SetKeyboard(UITextField textField)
        {
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.ReturnKeyType = UIReturnKeyType.Done;
        }

        void DisplayEyeIcon(UITextField textField)
        {
            if (textField == txtFieldPassword)
            {
                viewShowPassword.Hidden = textField.Text.Length == 0;
            }
            if (textField == txtFieldConfirmPassword)
            {
                viewShowConfirmPassword.Hidden = textField.Text.Length == 0;
            }
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel labelTitle, UILabel lblError, UIView viewLine, string pattern)
        {
            SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                labelTitle.Hidden = textField.Text.Length == 0;
                DisplayEyeIcon(textField);
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                labelTitle.Hidden = textField.Text.Length == 0;
                DisplayEyeIcon(textField);
                textField.LeftViewMode = UITextFieldViewMode.Never;
                viewLine.BackgroundColor = myTNBColor.PowerBlue();
            };
            textField.ShouldEndEditing = (sender) =>
            {
                labelTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                //Handling for Confirm Password
                if (textField == txtFieldConfirmPassword)
                {
                    bool isMatch = txtFieldPassword.Text.Equals(txtFieldConfirmPassword.Text);
                    string errMsg = isValid ? "Error_MismatchedPassword".Translate()
                        : "Hint_Password".Translate();
                    lblError.Text = errMsg;
                    isValid = isValid && isMatch;
                }
                DisplayEyeIcon(textField);
                lblError.Hidden = isValid || textField.Text.Length == 0;
                viewLine.BackgroundColor = isValid || textField.Text.Length == 0 ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                textField.TextColor = isValid || textField.Text.Length == 0 ? myTNBColor.TunaGrey() : myTNBColor.Tomato();
                SetSubmitButtonEnable();
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                {
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
                }
            };
        }

        internal void SetSubmitButtonEnable()
        {
            bool isValid = _textFieldHelper.ValidateTextField(txtFieldPassword.Text, PASSWORD_PATTERN)
                && _textFieldHelper.ValidateTextField(txtFieldConfirmPassword.Text, PASSWORD_PATTERN)
                && txtFieldPassword.Text.Equals(txtFieldConfirmPassword.Text);
            btnSubmit.Enabled = isValid;
            btnSubmit.BackgroundColor = isValid ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                OnShowLogin();
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal void OnShowLogin()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Login", null);
            LoginViewController viewController =
                storyBoard.InstantiateViewController("LoginViewController") as LoginViewController;
            this.DismissViewController(true, null);
        }

        internal void ExecuteChangePasswordCall()
        {
            ChangePassword().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (ServiceCall.ValidateBaseResponse(_changePasswordList))
                    {
                        var sharedPreference = NSUserDefaults.StandardUserDefaults;
                        sharedPreference.RemoveObject("isPasswordResetCodeSent");
                        sharedPreference.RemoveObject("resetPasswordMessage");

                        UIStoryboard storyBoard = UIStoryboard.FromName("ForgotPassword", null);
                        PasswordResetSuccessViewController viewController =
                            storyBoard.InstantiateViewController("PasswordResetSuccessViewController") as PasswordResetSuccessViewController;
                        if (viewController != null)
                        {
                            viewController.IsChangePassword = true;
                            NavigationController?.PushViewController(viewController, false);
                        }
                    }
                    else
                    {
                        AlertHandler.DisplayServiceError(this, _changePasswordList?.d?.message);
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        internal Task ChangePassword()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    ipAddress = TNBGlobal.API_KEY_ID,
                    clientType = TNBGlobal.API_KEY_ID,
                    activeUserName = TNBGlobal.API_KEY_ID,
                    devicePlatform = TNBGlobal.API_KEY_ID,
                    deviceVersion = TNBGlobal.API_KEY_ID,
                    deviceCordova = TNBGlobal.API_KEY_ID,
                    username = _username,
                    email = _username,
                    currentPassword = _currentPassword,
                    newPassword = _password,
                    confirmNewPassword = _confirmPassword
                };
                _changePasswordList = serviceManager.BaseServiceCall("ChangeNewPassword", requestParameter);
            });
        }
    }
}