using System;
using UIKit;
using myTNB.Model;
using System.Threading.Tasks;
using Foundation;
using CoreGraphics;
using myTNB.DataManager;
using myTNB.Login.ForgotPassword;

namespace myTNB
{
    public partial class ForgotPasswordViewController : CustomUIViewController
    {
        public ForgotPasswordViewController(IntPtr handle) : base(handle) { }

        private UILabel lblEmailTitle, lblEmailError;
        private UITextField txtFieldEmail;
        private UIView viewLineEmail;

        private TextFieldHelper _textFieldHelper = new TextFieldHelper();
        private BaseResponseModelV2 _resetCodeList = new BaseResponseModelV2();

        private const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        private string _email = string.Empty;

        public override void ViewDidLoad()
        {
            PageName = ForgotPasswordConstants.Pagename_ForgotPassword;
            base.ViewDidLoad();
            Title = GetI18NValue(ForgotPasswordConstants.I18N_Title);
            NavigationItem.HidesBackButton = true;

            InitializedSubViews();
            AddBackButton();
            SetEvents();
            SetViews();
        }

        private void InitializedSubViews()
        {
            lblTitle.Frame = new CGRect(18, 19, View.Frame.Width - 36, 18);
            lblTitle.TextColor = MyTNBColor.PowerBlue;
            lblTitle.Font = MyTNBFont.MuseoSans16_500;
            lblTitle.Text = GetI18NValue(ForgotPasswordConstants.I18N_SubTitle);

            lblDescription.Frame = new CGRect(18, 40, View.Frame.Width - 36, 36);
            lblDescription.TextColor = MyTNBColor.TunaGrey();
            lblDescription.Font = MyTNBFont.MuseoSans14_300;
            lblDescription.TextAlignment = UITextAlignment.Left;
            lblDescription.Lines = 0;
            lblDescription.LineBreakMode = UILineBreakMode.WordWrap;
            lblDescription.Text = GetI18NValue(ForgotPasswordConstants.I18N_Details);

            btnSubmit.Layer.CornerRadius = 5f;

            #region Email
            UIView viewEmail = new UIView((new CGRect(18, 94, View.Frame.Width - 36, 51)));
            viewEmail.BackgroundColor = UIColor.Clear;

            lblEmailTitle = new UILabel(new CGRect(0, 0, viewEmail.Frame.Width, 12));
            lblEmailTitle.Font = MyTNBFont.MuseoSans9_300;
            lblEmailTitle.TextColor = MyTNBColor.SilverChalice;
            lblEmailTitle.Text = GetCommonI18NValue(Constants.Common_Email).ToUpper();
            lblEmailTitle.TextAlignment = UITextAlignment.Left;

            lblEmailError = new UILabel(new CGRect(0, 37, viewEmail.Frame.Width, 14));
            lblEmailError.Font = MyTNBFont.MuseoSans9_300;
            lblEmailError.TextColor = MyTNBColor.Tomato;
            lblEmailError.Text = GetErrorI18NValue(Constants.Error_InvalidEmailAddress);
            lblEmailError.TextAlignment = UITextAlignment.Left;

            txtFieldEmail = new UITextField(new CGRect(0, 12, viewEmail.Frame.Width, 24));
            txtFieldEmail.AttributedPlaceholder = new NSAttributedString(
                GetCommonI18NValue(Constants.Common_Email).ToUpper()
                , font: MyTNBFont.MuseoSans16_300
                , foregroundColor: MyTNBColor.SilverChalice
                , strokeWidth: 0
            );
            txtFieldEmail.TextColor = MyTNBColor.TunaGrey();
            txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;
            txtFieldEmail.ReturnKeyType = UIReturnKeyType.Done;

            viewLineEmail = new UIView((new CGRect(0, 36, viewEmail.Frame.Width, 1)));
            viewLineEmail.BackgroundColor = MyTNBColor.PlatinumGrey;

            viewEmail.AddSubviews(new UIView[] { lblEmailTitle, lblEmailError, txtFieldEmail, viewLineEmail });

            View.AddSubview(viewEmail);
            #endregion
        }

        private void SetViews()
        {
            _textFieldHelper.CreateTextFieldLeftView(txtFieldEmail, "Email");
            lblEmailTitle.Hidden = true;
            lblEmailError.Hidden = true;
            btnSubmit.Enabled = false;
            btnSubmit.SetTitle(GetCommonI18NValue(Constants.Common_Submit), UIControlState.Normal);
            btnSubmit.BackgroundColor = MyTNBColor.PlatinumGrey;
            btnSubmit.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution()
                ? 184 : DeviceHelper.GetScaledHeight(136)), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48));
        }

        private void SetEvents()
        {
            btnSubmit.TouchUpInside += (sender, e) =>
            {
                _email = txtFieldEmail.Text;
                ActivityIndicator.Show();
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            ExecuteSendResetPasswordCodeCall();
                        }
                        else
                        {
                            DisplayNoDataAlert();
                            ActivityIndicator.Hide();
                        }
                    });
                });
            };
            SetTextFieldEvents(txtFieldEmail, lblEmailTitle, lblEmailError, viewLineEmail, EMAIL_PATTERN);
            txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;
        }

        private void SetKeyboard(UITextField textField)
        {
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.ReturnKeyType = UIReturnKeyType.Done;
        }

        private void SetTextFieldEvents(UITextField textField, UILabel textFieldTitle, UILabel textFieldError, UIView viewLine, string pattern)
        {
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                textField.LeftViewMode = UITextFieldViewMode.Never;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                textFieldError.Hidden = isValid || textField.Text.Length == 0;
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
            textField.EditingDidBegin += (sender, e) =>
            {
                textFieldTitle.Hidden = false;
            };
            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
            };
        }

        private void SetSubmitButtonEnable()
        {
            bool isValid = _textFieldHelper.ValidateTextField(txtFieldEmail.Text, EMAIL_PATTERN);
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
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void OnShowLogin()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Login", null);
            LoginViewController viewController =
                storyBoard.InstantiateViewController("LoginViewController") as LoginViewController;
            this.DismissViewController(true, null);
        }

        private void ExecuteSendResetPasswordCodeCall()
        {
            SendResetPasswordCode().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (ServiceCall.ValidateBaseResponse(_resetCodeList))
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("ForgotPassword", null);
                        EnterCodeViewController viewController =
                            storyBoard.InstantiateViewController("EnterCodeViewController") as EnterCodeViewController;
                        viewController.EmailAddress = _email;
                        NavigationController?.PushViewController(viewController, true);
                    }
                    else
                    {
                        DisplayServiceError(_resetCodeList?.d?.ErrorMessage);
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private Task SendResetPasswordCode()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                string fcmToken = DataManager.DataManager.SharedInstance.FCMToken != null
                    ? DataManager.DataManager.SharedInstance.FCMToken : string.Empty;
                object requestParameter = new
                {
                    usrInf = new
                    {
                        eid = _email,
                        sspuid = DataManager.DataManager.SharedInstance.User.UserID,
                        did = DataManager.DataManager.SharedInstance.UDID,
                        ft = fcmToken,
                        lang = TNBGlobal.APP_LANGUAGE,
                        sec_auth_k1 = TNBGlobal.API_KEY_ID,
                        sec_auth_k2 = string.Empty,
                        ses_param1 = string.Empty,
                        ses_param2 = string.Empty
                    },
                    serviceManager.deviceInf
                };
                _resetCodeList = serviceManager.BaseServiceCallV6(ForgotPasswordConstants.Service_SendResetPasswordCode, requestParameter);
            });
        }
    }
}