using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.DataManager;
using myTNB.Extensions;

namespace myTNB
{
    public partial class UpdatePasswordViewController : UIViewController
    {
        public UpdatePasswordViewController(IntPtr handle) : base(handle)
        {
        }

        const string PASSWORD_PATTERN = @"^.{8,}$"; // @"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8,})$";

        BaseResponseModel _saveResponse = new BaseResponseModel();
        TextFieldHelper _textFieldHelper = new TextFieldHelper();

        UITextField txtFieldPassword;
        UITextField txtFieldNewPassword;
        UITextField txtFieldConfirmNewPassword;

        UIView viewLinePassword;
        UIView viewLineNewPassword;
        UIView viewLineConfirmNewPassword;

        UILabel lblPasswordTitle;
        UILabel lblNewPasswordTitle;
        UILabel lblConfirmNewPasswordTitle;

        UILabel lblPasswordError;
        UILabel lblNewPasswordError;
        UILabel lblConfirmNewPasswordError;

        UIView viewShowPassword;
        UIView viewShowNewPassword;
        UIView viewShowConfirmNewPassword;

        UIButton btnSave;

        string _currentPassword = string.Empty;
        string _newPassword = string.Empty;
        string _confirmNewPassword = string.Empty;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            AddSaveButton();
            SetSubviews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        internal void SetSubviews()
        {
            //Password
            float additionalYValue = 0;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                additionalYValue += 24;
            }
            UIView viewPassword = new UIView((new CGRect(18, 80 + additionalYValue, View.Frame.Width - 36, 51)));
            viewPassword.BackgroundColor = UIColor.Clear;

            lblPasswordTitle = new UILabel(new CGRect(0, 0, viewPassword.Frame.Width, 12));
            lblPasswordTitle.Text = "CURRENT PASSWORD";
            lblPasswordTitle.Font = myTNBFont.MuseoSans9_300();
            lblPasswordTitle.TextColor = myTNBColor.SilverChalice();
            lblPasswordTitle.Hidden = true;

            lblPasswordError = new UILabel(new CGRect(0, 37, viewPassword.Frame.Width, 14));
            lblPasswordError.Font = myTNBFont.MuseoSans9_300();
            lblPasswordError.TextAlignment = UITextAlignment.Left;
            lblPasswordError.TextColor = myTNBColor.Tomato();
            lblPasswordError.Text = "Invalid old password.";
            lblPasswordError.Hidden = true;

            txtFieldPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Current Password"
                    , font: myTNBFont.MuseoSans16_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            txtFieldPassword.SecureTextEntry = true;

            viewLinePassword = new UIView((new CGRect(0, 36, viewPassword.Frame.Width, 1)));
            viewLinePassword.BackgroundColor = myTNBColor.PlatinumGrey();

            viewShowPassword = new UIView(new CGRect(viewPassword.Frame.Width - 30, 12, 24, 24));
            viewShowPassword.Hidden = true;
            UIImageView imgShowPassword = new UIImageView(new CGRect(0, 0, 24, 24));
            imgShowPassword.Image = UIImage.FromBundle("IC-Action-Show-Password");
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

            //New Password
            UIView viewNewPassword = new UIView((new CGRect(18, 137 + additionalYValue, View.Frame.Width - 36, 51)));
            viewNewPassword.BackgroundColor = UIColor.Clear;

            lblNewPasswordTitle = new UILabel(new CGRect(0, 0, viewNewPassword.Frame.Width, 12));
            lblNewPasswordTitle.Text = "NEW PASSWORD";
            lblNewPasswordTitle.Font = myTNBFont.MuseoSans9_300();
            lblNewPasswordTitle.TextColor = myTNBColor.SilverChalice();
            lblNewPasswordTitle.Hidden = true;

            lblNewPasswordError = new UILabel(new CGRect(0, 37, viewNewPassword.Frame.Width, 14));
            lblNewPasswordError.Font = myTNBFont.MuseoSans9_300();
            lblNewPasswordError.TextAlignment = UITextAlignment.Left;
            lblNewPasswordError.TextColor = myTNBColor.Tomato();
            lblNewPasswordError.Text = "Password must have at least 8 alphanumeric characters.";
            lblNewPasswordError.Hidden = true;

            txtFieldNewPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewNewPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "New Password"
                    , font: myTNBFont.MuseoSans16_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            txtFieldNewPassword.SecureTextEntry = true;

            viewLineNewPassword = new UIView((new CGRect(0, 36, viewNewPassword.Frame.Width, 1)));
            viewLineNewPassword.BackgroundColor = myTNBColor.PlatinumGrey();

            viewShowNewPassword = new UIView(new CGRect(viewNewPassword.Frame.Width - 30, 12, 24, 24));
            viewShowNewPassword.Hidden = true;
            UIImageView imgShowNewPassword = new UIImageView(new CGRect(0, 0, 24, 24));
            imgShowNewPassword.Image = UIImage.FromBundle("IC-Action-Show-Password");
            viewShowNewPassword.AddSubview(imgShowNewPassword);
            viewShowNewPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (txtFieldNewPassword.SecureTextEntry)
                {
                    txtFieldNewPassword.SecureTextEntry = false;
                }
                else
                {
                    txtFieldNewPassword.SecureTextEntry = true;
                }
            }));

            //Confirm Password
            UIView viewConfirmPassword = new UIView((new CGRect(18, 194 + additionalYValue, View.Frame.Width - 36, 51)));
            viewConfirmPassword.BackgroundColor = UIColor.Clear;

            lblConfirmNewPasswordTitle = new UILabel(new CGRect(0, 0, viewConfirmPassword.Frame.Width, 12));
            lblConfirmNewPasswordTitle.Text = "CONFIRM NEW PASSWORD";
            lblConfirmNewPasswordTitle.Font = myTNBFont.MuseoSans9_300();
            lblConfirmNewPasswordTitle.TextColor = myTNBColor.SilverChalice();
            lblConfirmNewPasswordTitle.Hidden = true;

            lblConfirmNewPasswordError = new UILabel(new CGRect(0, 37, viewConfirmPassword.Frame.Width, 14));
            lblConfirmNewPasswordError.Text = "Your password and confirmation password do not match.";
            lblConfirmNewPasswordError.Font = myTNBFont.MuseoSans9_300();
            lblConfirmNewPasswordError.TextAlignment = UITextAlignment.Left;
            lblConfirmNewPasswordError.TextColor = myTNBColor.Tomato();
            lblConfirmNewPasswordError.Hidden = true;

            txtFieldConfirmNewPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewConfirmPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Confirm New Password"
                    , font: myTNBFont.MuseoSans16_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            txtFieldConfirmNewPassword.SecureTextEntry = true;

            viewLineConfirmNewPassword = new UIView((new CGRect(0, 36, viewConfirmPassword.Frame.Width, 1)));
            viewLineConfirmNewPassword.BackgroundColor = myTNBColor.PlatinumGrey();

            viewShowConfirmNewPassword = new UIView(new CGRect(viewConfirmPassword.Frame.Width - 30, 12, 24, 24));
            viewShowConfirmNewPassword.Hidden = true;
            UIImageView imgShowConfirmNewPassword = new UIImageView(new CGRect(0, 0, 24, 24));
            imgShowConfirmNewPassword.Image = UIImage.FromBundle("IC-Action-Show-Password");
            viewShowConfirmNewPassword.AddSubview(imgShowConfirmNewPassword);
            viewShowConfirmNewPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (txtFieldConfirmNewPassword.SecureTextEntry)
                {
                    txtFieldConfirmNewPassword.SecureTextEntry = false;
                }
                else
                {
                    txtFieldConfirmNewPassword.SecureTextEntry = true;
                }
            }));

            viewPassword.AddSubviews(new UIView[] { lblPasswordTitle, txtFieldPassword
                , lblPasswordError, viewShowPassword, viewLinePassword });
            viewNewPassword.AddSubviews(new UIView[] { lblNewPasswordTitle, txtFieldNewPassword
                , lblNewPasswordError, viewShowNewPassword, viewLineNewPassword });
            viewConfirmPassword.AddSubviews(new UIView[] { lblConfirmNewPasswordTitle
                , txtFieldConfirmNewPassword, lblConfirmNewPasswordError,viewShowConfirmNewPassword
                , viewLineConfirmNewPassword });
            View.AddSubviews(new UIView[] { viewPassword, viewNewPassword, viewConfirmPassword });

            _textFieldHelper.CreateTextFieldLeftView(txtFieldPassword, "Password");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldNewPassword, "Password");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldConfirmNewPassword, "Password");

            SetTextFieldEvents(txtFieldPassword, lblPasswordTitle, lblPasswordError, viewLinePassword, PASSWORD_PATTERN);
            SetTextFieldEvents(txtFieldNewPassword, lblNewPasswordTitle, lblNewPasswordError, viewLineNewPassword, PASSWORD_PATTERN);
            SetTextFieldEvents(txtFieldConfirmNewPassword, lblConfirmNewPasswordTitle, lblConfirmNewPasswordError, viewLineConfirmNewPassword, PASSWORD_PATTERN);
        }

        void DisplayEyeIcon(UITextField textField)
        {
            if (textField == txtFieldPassword)
            {
                viewShowPassword.Hidden = textField.Text.Length == 0;
            }
            if (textField == txtFieldNewPassword)
            {
                viewShowNewPassword.Hidden = textField.Text.Length == 0;
            }
            if (textField == txtFieldConfirmNewPassword)
            {
                viewShowConfirmNewPassword.Hidden = textField.Text.Length == 0;
            }
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel lblTitle, UILabel lblError, UIView viewLine, string pattern)
        {
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                DisplayEyeIcon(textField);
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                DisplayEyeIcon(textField);
                textField.LeftViewMode = UITextFieldViewMode.Never;
                viewLine.BackgroundColor = myTNBColor.PowerBlue();
            };
            textField.ShouldEndEditing = (sender) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                if (textField == txtFieldConfirmNewPassword)
                {
                    bool isMatch = txtFieldNewPassword.Text.Equals(txtFieldConfirmNewPassword.Text);
                    string err = isValid ? "Your password and confirmation password do not match."
                        : "Password must have at least 8 alphanumeric characters.";
                    lblError.Text = err;
                    isValid = isValid && isMatch;
                }
                if (textField == txtFieldPassword)
                {
                    isValid = true;
                }
                DisplayEyeIcon(textField);
                lblError.Hidden = isValid || textField.Text.Length == 0;
                viewLine.BackgroundColor = isValid || textField.Text.Length == 0 ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                textField.TextColor = isValid || textField.Text.Length == 0 ? myTNBColor.TunaGrey() : myTNBColor.Tomato();

                SetSaveButtonEnable();
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
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
            };
        }

        internal void SetSaveButtonEnable()
        {
            bool isValidPW = txtFieldPassword.Text.Length > 0;
            bool isValidNewPW = _textFieldHelper.ValidateTextField(txtFieldNewPassword.Text, PASSWORD_PATTERN);
            bool isValidConfirmNewPW = _textFieldHelper.ValidateTextField(txtFieldConfirmNewPassword.Text, PASSWORD_PATTERN);
            bool isValid = isValidPW
                && isValidNewPW
                && isValidConfirmNewPW
                && txtFieldNewPassword.Text.Equals(txtFieldConfirmNewPassword.Text);
            btnSave.Enabled = isValid;
            btnSave.BackgroundColor = isValid ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Update Password");
            titleBarComponent.SetNotificationVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void AddSaveButton()
        {
            btnSave = new UIButton(UIButtonType.Custom);
            btnSave.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 96 : DeviceHelper.GetScaledHeight(72)), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48));
            btnSave.Layer.CornerRadius = 4;
            btnSave.BackgroundColor = myTNBColor.SilverChalice();
            btnSave.SetTitle("Save", UIControlState.Normal);
            btnSave.Font = myTNBFont.MuseoSans16();
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.Enabled = false;
            btnSave.TouchUpInside += (sender, e) =>
            {
                ActivityIndicator.Show();
                _currentPassword = txtFieldPassword.Text;
                _newPassword = txtFieldNewPassword.Text;
                _confirmNewPassword = txtFieldConfirmNewPassword.Text;
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            Task[] taskList = new Task[] { Save() };
                            Task.WaitAll(taskList);
                            if (ServiceCall.ValidateBaseResponse(_saveResponse))
                            {
                                DataManager.DataManager.SharedInstance.IsPasswordUpdated = true;
                                DismissViewController(true, null);
                            }
                            else
                            {
                                //lblPasswordError.Hidden = false;
                                //viewLinePassword.BackgroundColor = myTNBColor.Tomato();
                                //txtFieldPassword.TextColor = myTNBColor.Tomato();
                                //btnSave.Enabled = false;
                                //btnSave.BackgroundColor = myTNBColor.SilverChalice();
                                DisplayAlertMessage("Update Password Error", _saveResponse?.d?.message);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No Network");
                            DisplayAlertMessage("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate());
                        }
                        ActivityIndicator.Hide();
                    });
                });
            };
            View.AddSubview(btnSave);
        }

        internal void DisplayAlertMessage(string title, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "DefaultErrorMessage".Translate();
            }
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
        }

        internal Task Save()
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
                    username = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                    currentPassword = _currentPassword,
                    newPassword = _newPassword,
                    confirmNewPassword = _confirmNewPassword
                };
                _saveResponse = serviceManager.BaseServiceCall("ChangeNewPassword", requestParameter);
            });
        }
    }
}