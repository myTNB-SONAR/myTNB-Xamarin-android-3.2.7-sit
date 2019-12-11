using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using myTNB.Model;
using myTNB.DataManager;
using myTNB.Registration;
using myTNB.MyAccount;

namespace myTNB
{
    public partial class UpdateMobileNumberViewController : CustomUIViewController
    {
        public UpdateMobileNumberViewController(IntPtr handle) : base(handle) { }

        const string MOBILE_NO_PATTERN = @"^[0-9 \+]+$";
        private UILabel lblMobileNoTitle, lblMobileNoError, lblMobileNoHint;
        private UITextField txtFieldMobileNo;
        private UIView viewLineMobileNo;
        private UIButton btnSave;

        private TextFieldHelper _textFieldHelper = new TextFieldHelper();
        private string _mobileNo = string.Empty;
        private string _navTitle;

        public bool WillHideBackButton;
        public bool IsFromLogin;

        public override void ViewDidLoad()
        {
            PageName = MyAccountConstants.Pagename_UpdateMobileNumber;
            base.ViewDidLoad();
            SetNavigationBar();
            AddSaveButton();
            SetSubviews();

            if (IsFromLogin)
            {
                DisplayToast(GetI18NValue(MyAccountConstants.I18N_VerifyDeviceMessage));
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.NavigationBar.Hidden = true;

            string mobileNo = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                ? _textFieldHelper.TrimAllSpaces(DataManager.DataManager.SharedInstance.UserEntity[0]?.mobileNo)
                : string.Empty;
            txtFieldMobileNo.Text = _textFieldHelper.FormatMobileNo(mobileNo);
            SetVisibility();
            SetSaveButtonEnable();
        }

        internal void SetSubviews()
        {
            nfloat marginY = 16;
            UILabel info;
            UIView viewMobileNumber;
            if (IsFromLogin)
            {
                info = new UILabel
                {
                    Frame = new CGRect(18, DeviceHelper.IsIphoneXUpResolution() ? 104 : 80, View.Frame.Width - 36, 36),
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Lines = 0,
                    Font = MyTNBFont.MuseoSans14_300,
                    TextColor = MyTNBColor.TunaGrey(),
                    Text = GetI18NValue(MyAccountConstants.I18N_Details)
                };
                View.AddSubview(info);
                viewMobileNumber = new UIView((new CGRect(18, info.Frame.GetMaxY() + marginY, View.Frame.Width - 36, 51)));
            }
            else
            {
                viewMobileNumber = new UIView((new CGRect(18, (DeviceHelper.IsIphoneXUpResolution() ? 104 : 80), View.Frame.Width - 36, 51)));
            }

            viewMobileNumber.BackgroundColor = UIColor.Clear;

            lblMobileNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewMobileNumber.Frame.Width, 12),
                AttributedText = new NSAttributedString(GetCommonI18NValue(Constants.Common_MobileNo).ToUpper()
                    , font: MyTNBFont.MuseoSans9_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };
            viewMobileNumber.AddSubview(lblMobileNoTitle);

            lblMobileNoError = new UILabel
            {
                Frame = new CGRect(0, 37, viewMobileNumber.Frame.Width, 14),
                AttributedText = new NSAttributedString(GetErrorI18NValue(Constants.Error_InvalidMobileNumber)
                    , font: MyTNBFont.MuseoSans9_300
                    , foregroundColor: MyTNBColor.Tomato
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };
            viewMobileNumber.AddSubview(lblMobileNoError);

            lblMobileNoHint = new UILabel
            {
                Frame = new CGRect(0, 37, viewMobileNumber.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    GetHintI18NValue(Constants.Hint_MobileNumber),
                    font: MyTNBFont.MuseoSans9_300,
                    foregroundColor: MyTNBColor.TunaGrey(),
                    strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };
            viewMobileNumber.AddSubview(lblMobileNoHint);

            txtFieldMobileNo = new UITextField
            {
                Frame = new CGRect(0, 12, viewMobileNumber.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(GetCommonI18NValue(Constants.Common_MobileNo)
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0),
                TextColor = MyTNBColor.TunaGrey(),
                KeyboardType = UIKeyboardType.NumberPad
            };
            _textFieldHelper.CreateDoneButton(txtFieldMobileNo);
            _textFieldHelper.CreateTextFieldLeftView(txtFieldMobileNo, "Mobile");
            viewMobileNumber.AddSubview(txtFieldMobileNo);

            viewLineMobileNo = GenericLine.GetLine(new CGRect(0, 36, viewMobileNumber.Frame.Width, 1));
            viewMobileNumber.AddSubview(viewLineMobileNo);
            View.AddSubview(viewMobileNumber);
            SetTextFieldEvents(txtFieldMobileNo, lblMobileNoTitle, lblMobileNoError, viewLineMobileNo, lblMobileNoHint, MOBILE_NO_PATTERN);
        }

        internal void SetVisibility()
        {
            if (txtFieldMobileNo.Text != string.Empty)
            {
                lblMobileNoTitle.Hidden = false;
                txtFieldMobileNo.LeftViewMode = UITextFieldViewMode.Never;
            }
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel lblTitle
            , UILabel lblError, UIView viewLine, UILabel lblHint, string pattern)
        {
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;

                if (textField == txtFieldMobileNo)
                {
                    if (textField.Text.Length == 0)
                    {
                        textField.Text += TNBGlobal.MobileNoPrefix;
                    }
                }
                lblHint.Hidden = !lblError.Hidden || textField.Text.Length == 0;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                lblHint.Hidden = true;
                lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text.Replace("+", string.Empty), pattern);

                if (textField == txtFieldMobileNo)
                {
                    isValid = isValid && _textFieldHelper.ValidateMobileNumberLength(textField.Text);
                }

                lblError.Hidden = isValid;
                viewLine.BackgroundColor = isValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
                SetSaveButtonEnable();
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                lblTitle.Hidden = false;
            };
            textField.ShouldChangeCharacters += (txtField, range, replacementString) =>
            {
                if (textField == txtFieldMobileNo)
                {
                    bool isCharValid = _textFieldHelper.ValidateTextField(replacementString, TNBGlobal.MobileNoPattern);
                    if (!isCharValid)
                        return false;

                    if (range.Location >= TNBGlobal.MobileNoPrefix.Length)
                    {
                        string content = _textFieldHelper.TrimAllSpaces(((UITextField)txtField).Text);
                        nint count = content.Length + replacementString.Length - range.Length;
                        return count <= TNBGlobal.MobileNumberMaxCharCount;
                    }
                    return false;
                }
                return true;
            };
        }

        internal void SetSaveButtonEnable()
        {
            bool isValidMobileNo;
            string textStr = txtFieldMobileNo.Text?.Trim();
            if (!string.IsNullOrEmpty(textStr))
            {
                if (IsFromLogin)
                {
                    isValidMobileNo = _textFieldHelper.ValidateTextField(textStr.Replace("+", string.Empty), MOBILE_NO_PATTERN);
                }
                else
                {
                    string mobileNo = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                        ? _textFieldHelper.TrimAllSpaces(DataManager.DataManager.SharedInstance.UserEntity[0]?.mobileNo)
                        : string.Empty;
                    isValidMobileNo = _textFieldHelper.ValidateTextField(textStr.Replace("+", string.Empty), MOBILE_NO_PATTERN)
                        && !mobileNo.Equals(_textFieldHelper.TrimAllSpaces(txtFieldMobileNo.Text));
                }
                isValidMobileNo = isValidMobileNo && _textFieldHelper.ValidateMobileNumberLength(textStr);
                btnSave.Enabled = isValidMobileNo;
                btnSave.BackgroundColor = isValidMobileNo ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
            }
        }

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            _navTitle = GetI18NValue(IsFromLogin ? MyAccountConstants.I18N_VerifyDeviceTitle : MyAccountConstants.I18N_UpdateMobileTitle);
            titleBarComponent.SetTitle(_navTitle);
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(WillHideBackButton);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void AddSaveButton()
        {
            btnSave = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution()
                    ? 96 : DeviceHelper.GetScaledHeight(72)), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans16_500
            };
            btnSave.Layer.CornerRadius = 4;
            btnSave.SetTitle(GetCommonI18NValue(Constants.Common_Next), UIControlState.Normal);
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.TouchUpInside += async (sender, e) =>
            {
                ActivityIndicator.Show();
                _mobileNo = txtFieldMobileNo.Text.Replace(" ", string.Empty);
                BaseResponseModelV2 response = await ServiceCall.SendUpdatePhoneTokenSMS(_mobileNo);

                if (ServiceCall.ValidateBaseResponse(response))
                {
                    DataManager.DataManager.SharedInstance.User.MobileNo = _mobileNo;
                    UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                    VerifyPinViewController viewController = storyBoard.InstantiateViewController("VerifyPinViewController") as VerifyPinViewController;
                    viewController.IsMobileVerification = true;
                    viewController.IsFromLogin = IsFromLogin;
                    NavigationController.PushViewController(viewController, true);
                    ActivityIndicator.Hide();
                }
                else
                {
                    DisplayServiceError(response?.d?.DisplayMessage ?? string.Empty);
                    ActivityIndicator.Hide();
                }
            };
            View.AddSubview(btnSave);
        }
    }
}