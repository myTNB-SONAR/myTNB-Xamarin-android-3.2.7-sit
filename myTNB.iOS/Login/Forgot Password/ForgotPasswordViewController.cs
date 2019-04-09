using System;
using UIKit;
using myTNB.Model;
using System.Threading.Tasks;
using Foundation;
using CoreGraphics;
using myTNB.DataManager;


namespace myTNB
{
    public partial class ForgotPasswordViewController : UIViewController
    {
        UILabel lblEmailTitle;
        UITextField txtFieldEmail;
        UILabel lblEmailError;
        UIView viewLineEmail;

        public ForgotPasswordViewController(IntPtr handle) : base(handle)
        {
        }

        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        BaseResponseModel _resetCodeList = new BaseResponseModel();

        const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        const string TOKEN_PATTERN = @"^[0-9]{4,4}$";
        string _email = string.Empty;
        string _token = string.Empty;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.Title = "Reset Password";
            this.NavigationItem.HidesBackButton = true;

            InitializedSubViews();
            AddBackButton();
            SetEvents();
            SetViews();
        }

        internal void InitializedSubViews()
        {
            lblTitle.Frame = new CGRect(18, 19, View.Frame.Width - 36, 18);
            lblTitle.TextColor = myTNBColor.PowerBlue();
            lblTitle.Font = myTNBFont.MuseoSans16_500();
            lblTitle.Text = "Please enter your email.";

            lblDescription.Frame = new CGRect(18, 40, View.Frame.Width - 36, 36);
            lblDescription.TextColor = myTNBColor.TunaGrey();
            lblDescription.Font = myTNBFont.MuseoSans14_300();
            lblDescription.TextAlignment = UITextAlignment.Left;
            lblDescription.Lines = 0;
            lblDescription.LineBreakMode = UILineBreakMode.WordWrap;
            lblDescription.Text = "A 4-digit verification code will be sent to this email address.";

            btnSubmit.Layer.CornerRadius = 5f;

            #region Email
            UIView viewEmail = new UIView((new CGRect(18, 94, View.Frame.Width - 36, 51)));
            viewEmail.BackgroundColor = UIColor.Clear;

            lblEmailTitle = new UILabel(new CGRect(0, 0, viewEmail.Frame.Width, 12));
            lblEmailTitle.Font = myTNBFont.MuseoSans9_300();
            lblEmailTitle.TextColor = myTNBColor.SilverChalice();
            lblEmailTitle.Text = "EMAIL";
            lblEmailTitle.TextAlignment = UITextAlignment.Left;

            lblEmailError = new UILabel(new CGRect(0, 37, viewEmail.Frame.Width, 14));
            lblEmailError.Font = myTNBFont.MuseoSans9_300();
            lblEmailError.TextColor = myTNBColor.Tomato();
            lblEmailError.Text = "Invalid email address";
            lblEmailError.TextAlignment = UITextAlignment.Left;

            txtFieldEmail = new UITextField(new CGRect(0, 12, viewEmail.Frame.Width, 24));
            txtFieldEmail.AttributedPlaceholder = new NSAttributedString(
                "Email"
                , font: myTNBFont.MuseoSans16_300()
                , foregroundColor: myTNBColor.SilverChalice()
                , strokeWidth: 0
            );
            txtFieldEmail.TextColor = myTNBColor.TunaGrey();
            txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;
            txtFieldEmail.ReturnKeyType = UIReturnKeyType.Done;

            viewLineEmail = new UIView((new CGRect(0, 36, viewEmail.Frame.Width, 1)));
            viewLineEmail.BackgroundColor = myTNBColor.PlatinumGrey();

            viewEmail.AddSubviews(new UIView[] { lblEmailTitle, lblEmailError, txtFieldEmail, viewLineEmail });

            View.AddSubview(viewEmail);
            #endregion
        }

        internal void SetViews()
        {
            _textFieldHelper.CreateTextFieldLeftView(txtFieldEmail, "Email");
            lblEmailTitle.Hidden = true;
            lblEmailError.Hidden = true;
            btnSubmit.Enabled = false;
            btnSubmit.BackgroundColor = myTNBColor.PlatinumGrey();
            btnSubmit.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 184 : DeviceHelper.GetScaledHeight(136)), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48));
        }

        internal void SetEvents()
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
                            Console.WriteLine("No Network");
                            DisplayAlertMessage("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate());
                            ActivityIndicator.Hide();
                        }
                    });
                });
            };
            SetTextFieldEvents(txtFieldEmail, lblEmailTitle, lblEmailError, viewLineEmail, EMAIL_PATTERN);
            txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;
        }

        internal void SetKeyboard(UITextField textField)
        {
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.ReturnKeyType = UIReturnKeyType.Done;
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel textFieldTitle, UILabel textFieldError, UIView viewLine, string pattern)
        {
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;
                viewLine.BackgroundColor = myTNBColor.PowerBlue();
                textField.LeftViewMode = UITextFieldViewMode.Never;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                textFieldError.Hidden = isValid || textField.Text.Length == 0;
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

        internal void SetSubmitButtonEnable()
        {
            bool isValid = _textFieldHelper.ValidateTextField(txtFieldEmail.Text, EMAIL_PATTERN);
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

        internal void ExecuteSendResetPasswordCodeCall()
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
                        string errorMsg = "Error in sending reset code.";
                        if (_resetCodeList != null && _resetCodeList.d != null
                           && !string.IsNullOrEmpty(_resetCodeList.d.message)
                           && !string.IsNullOrWhiteSpace(_resetCodeList.d.message))
                        {
                            errorMsg = _resetCodeList.d.message;
                        }
                        DisplayAlertMessage("Error", errorMsg);
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        internal Task SendResetPasswordCode()
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
                    username = _email,
                    userEmail = _email
                };
                _resetCodeList = serviceManager.BaseServiceCall("SendResetPasswordCode", requestParameter);
            });
        }

        internal void DisplayAlertMessage(string title, string message)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
        }
    }
}