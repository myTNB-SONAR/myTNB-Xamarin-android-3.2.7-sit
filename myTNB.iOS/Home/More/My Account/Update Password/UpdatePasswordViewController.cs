using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.DataManager;
using myTNB.MyAccount;

namespace myTNB
{
    public partial class UpdatePasswordViewController : CustomUIViewController
    {
        public UpdatePasswordViewController(IntPtr handle) : base(handle) { }

        const string PASSWORD_PATTERN = @"^.{8,}$"; // @"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8,})$";

        private BaseResponseModelV2 _saveResponse = new BaseResponseModelV2();
        private TextFieldHelper _textFieldHelper = new TextFieldHelper();

        private UITextField txtFieldPassword, txtFieldNewPassword, txtFieldConfirmNewPassword;

        private UIView viewLinePassword, viewLineNewPassword, viewLineConfirmNewPassword
            , viewShowPassword, viewShowNewPassword, viewShowConfirmNewPassword;

        private UILabel lblPasswordTitle, lblNewPasswordTitle, lblConfirmNewPasswordTitle
            , lblPasswordError, lblNewPasswordError, lblConfirmNewPasswordError;

        private UIButton btnSave;

        private string _currentPassword = string.Empty
            , _newPassword = string.Empty
            , _confirmNewPassword = string.Empty;

        public override void ViewDidLoad()
        {
            PageName = MyAccountConstants.Pagename_UpdatePassword;
            base.ViewDidLoad();
            SetNavigationBar();
            AddSaveButton();
            SetSubviews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        private void SetSubviews()
        {
            //Password
            float additionalYValue = 0;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                additionalYValue += 24;
            }
            UIView viewPassword = new UIView((new CGRect(18, 80 + additionalYValue, View.Frame.Width - 36, 51)));
            viewPassword.BackgroundColor = UIColor.Clear;

            lblPasswordTitle = new UILabel(new CGRect(0, 0, viewPassword.Frame.Width, 12))
            {
                Text = GetI18NValue(MyAccountConstants.I18N_CurrentPassword).ToUpper(),
                Font = MyTNBFont.MuseoSans9_300,
                TextColor = MyTNBColor.SilverChalice,
                Hidden = true
            };

            lblPasswordError = new UILabel(new CGRect(0, 37, viewPassword.Frame.Width, 14))
            {
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.Tomato,
                Text = GetErrorI18NValue(Constants.Error_InvalidPassword),
                Hidden = true
            };

            txtFieldPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                     GetI18NValue(MyAccountConstants.I18N_CurrentPassword)
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0
                ),
                TextColor = MyTNBColor.TunaGrey()
            };
            txtFieldPassword.SecureTextEntry = true;

            viewLinePassword = GenericLine.GetLine(new CGRect(0, 36, viewPassword.Frame.Width, 1));

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
            }));

            //New Password
            UIView viewNewPassword = new UIView((new CGRect(18, 137 + additionalYValue, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblNewPasswordTitle = new UILabel(new CGRect(0, 0, viewNewPassword.Frame.Width, 12))
            {
                Text = GetI18NValue(MyAccountConstants.I18N_NewPassword).ToUpper(),
                Font = MyTNBFont.MuseoSans9_300,
                TextColor = MyTNBColor.SilverChalice,
                Hidden = true
            };

            lblNewPasswordError = new UILabel(new CGRect(0, 37, viewNewPassword.Frame.Width, 14))
            {
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.Tomato,
                Text = GetErrorI18NValue(Constants.Error_InvalidPassword),
                Hidden = true
            };

            txtFieldNewPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewNewPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    GetI18NValue(MyAccountConstants.I18N_NewPassword)
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
                Image = UIImage.FromBundle(Constants.IMG_ShowPassword)
            };
            viewShowNewPassword.AddSubview(imgShowNewPassword);
            viewShowNewPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                txtFieldNewPassword.SecureTextEntry = !txtFieldNewPassword.SecureTextEntry;
            }));

            //Confirm Password
            UIView viewConfirmPassword = new UIView((new CGRect(18, 194 + additionalYValue, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblConfirmNewPasswordTitle = new UILabel(new CGRect(0, 0, viewConfirmPassword.Frame.Width, 12))
            {
                Text = GetI18NValue(MyAccountConstants.I18N_ConfirmNewPassword).ToUpper(),
                Font = MyTNBFont.MuseoSans9_300,
                TextColor = MyTNBColor.SilverChalice,
                Hidden = true
            };

            lblConfirmNewPasswordError = new UILabel(new CGRect(0, 37, viewConfirmPassword.Frame.Width, 14))
            {
                Text = GetErrorI18NValue(Constants.Error_MismatchedPassword),
                Font = MyTNBFont.MuseoSans9_300,
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.Tomato,
                Hidden = true
            };

            txtFieldConfirmNewPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewConfirmPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    GetI18NValue(MyAccountConstants.I18N_ConfirmNewPassword)
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
                Image = UIImage.FromBundle(Constants.IMG_ShowPassword)
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

        private void DisplayEyeIcon(UITextField textField)
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

        private void SetTextFieldEvents(UITextField textField, UILabel lblTitle, UILabel lblError, UIView viewLine, string pattern)
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
                    string err = isValid ? GetErrorI18NValue(Constants.Error_MismatchedPassword)
                        : GetErrorI18NValue(Constants.Error_InvalidPassword);
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

        private void SetSaveButtonEnable()
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

        private void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle(GetI18NValue(MyAccountConstants.I18N_Title));
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        private void AddSaveButton()
        {
            btnSave = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution()
                    ? 96 : DeviceHelper.GetScaledHeight(72)), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48)),
                Enabled = false,
                Font = MyTNBFont.MuseoSans16,
                BackgroundColor = MyTNBColor.SilverChalice
            };
            btnSave.SetTitle(GetCommonI18NValue(Constants.Common_Save), UIControlState.Normal);
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.Layer.CornerRadius = 4;
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
                            Task[] taskList = { Save() };
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
                                DisplayServiceError(_saveResponse?.d?.ErrorMessage ?? string.Empty);
                            }
                        }
                        else
                        {
                            DisplayNoDataAlert();
                        }
                        ActivityIndicator.Hide();
                    });
                });
            };
            View.AddSubview(btnSave);
        }

        private Task Save()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    currentPassword = _currentPassword,
                    newPassword = _newPassword,
                    confirmNewPassword = _confirmNewPassword
                };
                _saveResponse = serviceManager.BaseServiceCallV6(MyAccountConstants.Service_ChangePassword, requestParameter);
            });
        }
    }
}