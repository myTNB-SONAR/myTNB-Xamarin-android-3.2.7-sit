using System;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using myTNB.DataManager;
using myTNB.Model;
using UIKit;

namespace myTNB.Login.ForgotPassword
{
    public partial class ResetPasswordViewController : CustomUIViewController
    {
        public ResetPasswordViewController(IntPtr handle) : base(handle) { }

        public string _username = string.Empty;
        public string _currentPassword = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;

        private UILabel lblPasswordTitle, lblConfirmPasswordTitle, lblConfirmPasswordError, lblPasswordError;
        private UITextField txtFieldPassword, txtFieldConfirmPassword;
        private UIView viewLinePassword, viewLineConfirmPassword, viewShowConfirmPassword, viewShowPassword;

        private TextFieldHelper _textFieldHelper = new TextFieldHelper();
        private BaseResponseModelV2 _changePasswordList = new BaseResponseModelV2();
        const string PASSWORD_PATTERN = @"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8,})$";

        public override void ViewDidLoad()
        {
            PageName = ForgotPasswordConstants.Pagename_ResetPassword;
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            NavigationItem.HidesBackButton = true;
            AddBackButton();

            InitializeSubviews();
            SetViews();
            SetEvents();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void InitializeSubviews()
        {
            UILabel lblTitle = new UILabel(new CGRect(18, 16, View.Frame.Width - 36, 18))
            {
                TextColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans16_500,
                Text = GetI18NValue(ForgotPasswordConstants.I18N_SubTitle)
            };
            View.AddSubview(lblTitle);

            UILabel lblDescription = new UILabel(new CGRect(18, lblTitle.Frame.GetMaxY(), View.Frame.Width - 36, 18))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14_300,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = GetI18NValue(ForgotPasswordConstants.I18N_Details)
            };
            View.AddSubview(lblDescription);

            nfloat newDescriptionHeight = lblDescription.GetLabelHeight(1000);
            lblDescription.Frame = new CGRect(lblDescription.Frame.Location, new CGSize(lblDescription.Frame.Width, newDescriptionHeight));
            btnSubmit.Layer.CornerRadius = 5f;

            #region Password
            UIView viewPassword = new UIView((new CGRect(18, lblDescription.Frame.GetMaxY() + 18, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblPasswordTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewPassword.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    GetCommonI18NValue(Constants.Common_Password).ToUpper()
                    , font: MyTNBFont.MuseoSans9_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewPassword.AddSubview(lblPasswordTitle);

            lblPasswordError = new UILabel
            {
                Frame = new CGRect(0, 37, viewPassword.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    GetErrorI18NValue(Constants.Error_InvalidPassword)
                    , font: MyTNBFont.MuseoSans9_300
                    , foregroundColor: MyTNBColor.Tomato
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewPassword.AddSubview(lblPasswordError);

            txtFieldPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    GetI18NValue(ForgotPasswordConstants.I18N_NewPassword)
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0
                ),
                TextColor = MyTNBColor.TunaGrey(),
                KeyboardType = UIKeyboardType.Default,
                ReturnKeyType = UIReturnKeyType.Done,
                SecureTextEntry = true
            };

            viewPassword.AddSubview(txtFieldPassword);

            viewLinePassword = new UIView((new CGRect(0, 36, viewPassword.Frame.Width, 1)))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };
            viewPassword.AddSubview(viewLinePassword);

            viewShowPassword = new UIView(new CGRect(viewPassword.Frame.Width - 30, 12, 24, 24))
            {
                Hidden = true
            };
            UIImageView imgShowPassword = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle(Constants.IMG_ShowPassword)
            };
            viewShowPassword.AddSubview(imgShowPassword);
            viewShowPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                txtFieldPassword.SecureTextEntry = !txtFieldPassword.SecureTextEntry;
                imgShowPassword.Image = UIImage.FromBundle(txtFieldPassword.SecureTextEntry
                    ? Constants.IMG_ShowPassword : Constants.IMG_HidePassword);
            }));
            viewPassword.AddSubview(viewShowPassword);

            View.AddSubview(viewPassword);
            #endregion

            #region Confirm Password
            UIView viewConfirmPassword = new UIView((new CGRect(18, 156, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblConfirmPasswordTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewConfirmPassword.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    GetI18NValue(ForgotPasswordConstants.I18N_ConfirmPassword).ToUpper()
                    , font: MyTNBFont.MuseoSans9_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewConfirmPassword.AddSubview(lblConfirmPasswordTitle);

            lblConfirmPasswordError = new UILabel(new CGRect(0, 37, viewConfirmPassword.Frame.Width, 14))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans9_300,
                TextColor = MyTNBColor.Tomato
            };

            viewConfirmPassword.AddSubview(lblConfirmPasswordError);

            txtFieldConfirmPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewConfirmPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    GetI18NValue(ForgotPasswordConstants.I18N_ConfirmNewPassword)
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0
                ),
                TextColor = MyTNBColor.TunaGrey(),
                KeyboardType = UIKeyboardType.Default,
                ReturnKeyType = UIReturnKeyType.Done,
                SecureTextEntry = true
            };
            viewConfirmPassword.AddSubview(txtFieldConfirmPassword);

            viewLineConfirmPassword = new UIView((new CGRect(0, 36, viewConfirmPassword.Frame.Width, 1)));
            viewLineConfirmPassword.BackgroundColor = MyTNBColor.PlatinumGrey;
            viewConfirmPassword.AddSubview(viewLineConfirmPassword);

            viewShowConfirmPassword = new UIView(new CGRect(viewConfirmPassword.Frame.Width - 30, 12, 24, 24));
            viewShowConfirmPassword.Hidden = true;
            UIImageView imgShowConfirmPassword = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle(Constants.IMG_ShowPassword)
            };
            viewShowConfirmPassword.AddSubview(imgShowConfirmPassword);
            viewConfirmPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                txtFieldConfirmPassword.SecureTextEntry = !txtFieldConfirmPassword.SecureTextEntry;
                imgShowConfirmPassword.Image = UIImage.FromBundle(txtFieldConfirmPassword.SecureTextEntry
                    ? Constants.IMG_ShowPassword : Constants.IMG_HidePassword);
            }));
            viewConfirmPassword.AddSubview(viewShowConfirmPassword);

            View.AddSubview(viewConfirmPassword);
            #endregion
        }

        private void SetViews()
        {
            lblPasswordTitle.Hidden = true;
            lblPasswordError.Hidden = true;
            lblConfirmPasswordTitle.Hidden = true;
            lblConfirmPasswordError.Hidden = true;
            btnSubmit.Enabled = false;
            btnSubmit.SetTitle(GetCommonI18NValue(Constants.Common_Submit), UIControlState.Normal);
            btnSubmit.BackgroundColor = MyTNBColor.PlatinumGrey;
            _textFieldHelper.CreateTextFieldLeftView(txtFieldPassword, "Password");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldConfirmPassword, "Password");
            btnSubmit.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution()
                ? 184 : DeviceHelper.GetScaledHeight(130)), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48));
        }

        private void SetEvents()
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
                            DisplayNoDataAlert();
                            ActivityIndicator.Hide();
                        }
                    });
                });
            };
            SetTextFieldEvents(txtFieldPassword, lblPasswordTitle, lblPasswordError, viewLinePassword, PASSWORD_PATTERN);
            SetTextFieldEvents(txtFieldConfirmPassword, lblConfirmPasswordTitle, lblConfirmPasswordError, viewLineConfirmPassword, PASSWORD_PATTERN);
        }

        private void SetKeyboard(UITextField textField)
        {
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.ReturnKeyType = UIReturnKeyType.Done;
        }

        private void DisplayEyeIcon(UITextField textField)
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

        private void SetTextFieldEvents(UITextField textField, UILabel labelTitle, UILabel lblError, UIView viewLine, string pattern)
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
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                labelTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                //Handling for Confirm Password
                if (textField == txtFieldConfirmPassword)
                {
                    bool isMatch = txtFieldPassword.Text.Equals(txtFieldConfirmPassword.Text);
                    string errMsg = isValid ? GetErrorI18NValue(Constants.Error_MismatchedPassword)
                        : GetErrorI18NValue(Constants.Error_InvalidPassword);
                    lblError.Text = errMsg;
                    isValid = isValid && isMatch;
                }
                DisplayEyeIcon(textField);
                lblError.Hidden = isValid || textField.Text.Length == 0;
                viewLine.BackgroundColor = isValid || textField.Text.Length == 0 ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isValid || textField.Text.Length == 0 ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
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

        private void SetSubmitButtonEnable()
        {
            bool isValid = _textFieldHelper.ValidateTextField(txtFieldPassword.Text, PASSWORD_PATTERN)
                && _textFieldHelper.ValidateTextField(txtFieldConfirmPassword.Text, PASSWORD_PATTERN)
                && txtFieldPassword.Text.Equals(txtFieldConfirmPassword.Text);
            btnSubmit.Enabled = isValid;
            btnSubmit.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }

        private void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                OnShowLogin();
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            Title = GetI18NValue(ForgotPasswordConstants.I18N_Title);
        }

        private void OnShowLogin()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Login", null);
            LoginViewController viewController =
                storyBoard.InstantiateViewController("LoginViewController") as LoginViewController;
            DismissViewController(true, null);
        }

        private void ExecuteChangePasswordCall()
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
                        DisplayServiceError(_changePasswordList?.d?.DisplayMessage ?? string.Empty);
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private Task ChangePassword()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    currentPassword = _currentPassword,
                    newPassword = _password,
                    confirmNewPassword = _confirmPassword
                };
                _changePasswordList = serviceManager.BaseServiceCallV6(ForgotPasswordConstants.Service_ChangeNewPassword, requestParameter);
            });
        }
    }
}