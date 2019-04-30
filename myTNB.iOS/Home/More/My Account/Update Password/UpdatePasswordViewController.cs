﻿using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.DataManager;

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
            lblPasswordTitle.Text = "Common_CurrentPassword".Translate().ToUpper();
            lblPasswordTitle.Font = MyTNBFont.MuseoSans9_300;
            lblPasswordTitle.TextColor = MyTNBColor.SilverChalice;
            lblPasswordTitle.Hidden = true;

            lblPasswordError = new UILabel(new CGRect(0, 37, viewPassword.Frame.Width, 14));
            lblPasswordError.Font = MyTNBFont.MuseoSans9_300;
            lblPasswordError.TextAlignment = UITextAlignment.Left;
            lblPasswordError.TextColor = MyTNBColor.Tomato;
            lblPasswordError.Text = "Invalid_OldPassword".Translate();
            lblPasswordError.Hidden = true;

            txtFieldPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Common_CurrentPassword".Translate()
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0
                ),
                TextColor = MyTNBColor.TunaGrey()
            };
            txtFieldPassword.SecureTextEntry = true;

            viewLinePassword = GenericLine.GetLine(new CGRect(0, 36, viewPassword.Frame.Width, 1));

            viewShowPassword = new UIView(new CGRect(viewPassword.Frame.Width - 30, 12, 24, 24));
            viewShowPassword.Hidden = true;
            UIImageView imgShowPassword = new UIImageView(new CGRect(0, 0, 24, 24));
            imgShowPassword.Image = UIImage.FromBundle("IC-Action-Show-Password");
            viewShowPassword.AddSubview(imgShowPassword);
            viewShowPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                txtFieldPassword.SecureTextEntry = !txtFieldPassword.SecureTextEntry;
            }));

            //New Password
            UIView viewNewPassword = new UIView((new CGRect(18, 137 + additionalYValue, View.Frame.Width - 36, 51)));
            viewNewPassword.BackgroundColor = UIColor.Clear;

            lblNewPasswordTitle = new UILabel(new CGRect(0, 0, viewNewPassword.Frame.Width, 12));
            lblNewPasswordTitle.Text = "Common_NewPassword".Translate().ToUpper();
            lblNewPasswordTitle.Font = MyTNBFont.MuseoSans9_300;
            lblNewPasswordTitle.TextColor = MyTNBColor.SilverChalice;
            lblNewPasswordTitle.Hidden = true;

            lblNewPasswordError = new UILabel(new CGRect(0, 37, viewNewPassword.Frame.Width, 14));
            lblNewPasswordError.Font = MyTNBFont.MuseoSans9_300;
            lblNewPasswordError.TextAlignment = UITextAlignment.Left;
            lblNewPasswordError.TextColor = MyTNBColor.Tomato;
            lblNewPasswordError.Text = "Hint_Password".Translate();
            lblNewPasswordError.Hidden = true;

            txtFieldNewPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewNewPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Common_NewPassword".Translate()
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0
                ),
                TextColor = MyTNBColor.TunaGrey()
            };
            txtFieldNewPassword.SecureTextEntry = true;

            viewLineNewPassword = GenericLine.GetLine(new CGRect(0, 36, viewNewPassword.Frame.Width, 1));

            viewShowNewPassword = new UIView(new CGRect(viewNewPassword.Frame.Width - 30, 12, 24, 24))
            {
                Hidden = true
            };
            UIImageView imgShowNewPassword = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Action-Show-Password")
            };
            viewShowNewPassword.AddSubview(imgShowNewPassword);
            viewShowNewPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                txtFieldNewPassword.SecureTextEntry = !txtFieldNewPassword.SecureTextEntry;
            }));

            //Confirm Password
            UIView viewConfirmPassword = new UIView((new CGRect(18, 194 + additionalYValue, View.Frame.Width - 36, 51)));
            viewConfirmPassword.BackgroundColor = UIColor.Clear;

            lblConfirmNewPasswordTitle = new UILabel(new CGRect(0, 0, viewConfirmPassword.Frame.Width, 12));
            lblConfirmNewPasswordTitle.Text = "Common_ConfirmPassword".Translate().ToUpper();
            lblConfirmNewPasswordTitle.Font = MyTNBFont.MuseoSans9_300;
            lblConfirmNewPasswordTitle.TextColor = MyTNBColor.SilverChalice;
            lblConfirmNewPasswordTitle.Hidden = true;

            lblConfirmNewPasswordError = new UILabel(new CGRect(0, 37, viewConfirmPassword.Frame.Width, 14));
            lblConfirmNewPasswordError.Text = "Error_MismatchedPassword".Translate();
            lblConfirmNewPasswordError.Font = MyTNBFont.MuseoSans9_300;
            lblConfirmNewPasswordError.TextAlignment = UITextAlignment.Left;
            lblConfirmNewPasswordError.TextColor = MyTNBColor.Tomato;
            lblConfirmNewPasswordError.Hidden = true;

            txtFieldConfirmNewPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewConfirmPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Common_ConfirmPassword".Translate()
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0
                ),
                TextColor = MyTNBColor.TunaGrey()
            };
            txtFieldConfirmNewPassword.SecureTextEntry = true;

            viewLineConfirmNewPassword = GenericLine.GetLine(new CGRect(0, 36, viewConfirmPassword.Frame.Width, 1));

            viewShowConfirmNewPassword = new UIView(new CGRect(viewConfirmPassword.Frame.Width - 30, 12, 24, 24))
            {
                Hidden = true
            };
            UIImageView imgShowConfirmNewPassword = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Action-Show-Password")
            };
            viewShowConfirmNewPassword.AddSubview(imgShowConfirmNewPassword);
            viewShowConfirmNewPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                txtFieldConfirmNewPassword.SecureTextEntry = !txtFieldConfirmNewPassword.SecureTextEntry;
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
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                if (textField == txtFieldConfirmNewPassword)
                {
                    bool isMatch = txtFieldNewPassword.Text.Equals(txtFieldConfirmNewPassword.Text);
                    string err = isValid ? "Error_MismatchedPassword".Translate()
                        : "Hint_Password".Translate();
                    lblError.Text = err;
                    isValid = isValid && isMatch;
                }
                if (textField == txtFieldPassword)
                {
                    isValid = true;
                }
                DisplayEyeIcon(textField);
                lblError.Hidden = isValid || textField.Text.Length == 0;
                viewLine.BackgroundColor = isValid || textField.Text.Length == 0 ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isValid || textField.Text.Length == 0 ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;

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
            btnSave.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Manage_PasswordTitle".Translate());
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
            btnSave.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution()
                ? 96 : DeviceHelper.GetScaledHeight(72)), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48));
            btnSave.Layer.CornerRadius = 4;
            btnSave.BackgroundColor = MyTNBColor.SilverChalice;
            btnSave.SetTitle("Common_Save".Translate(), UIControlState.Normal);
            btnSave.Font = MyTNBFont.MuseoSans16;
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
                                //viewLinePassword.BackgroundColor = myTNBColor.Tomato;
                                //txtFieldPassword.TextColor = myTNBColor.Tomato;
                                //btnSave.Enabled = false;
                                //btnSave.BackgroundColor = myTNBColor.SilverChalice;
                                AlertHandler.DisplayServiceError(this, _saveResponse?.d?.message);
                            }
                        }
                        else
                        {
                            AlertHandler.DisplayNoDataAlert(this);
                        }
                        ActivityIndicator.Hide();
                    });
                });
            };
            View.AddSubview(btnSave);
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