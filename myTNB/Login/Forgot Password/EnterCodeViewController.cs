using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.DataManager;

namespace myTNB
{
    public partial class EnterCodeViewController : UIViewController
    {
        public EnterCodeViewController(IntPtr handle) : base(handle)
        {
        }

        public string EmailAddress = string.Empty;

        BaseResponseModel _resetCodeList = new BaseResponseModel();
        TextFieldHelper _textFieldHelper = new TextFieldHelper();

        UIView _viewTokenFieldContainer;
        UIImage _loadingImg = UIImage.FromBundle("Loading");
        UIImage _loadedImg = UIImage.FromBundle("Loaded");
        UIView _loadingView;
        UIView _segment;
        UILabel _lblError;
        UIImageView _loadingImage;
        UILabel _resendLabel;
        UIView _viewPinSent;
        UITapGestureRecognizer _onResendPin;

        bool _isKeyboardDismissed = false;
        string _token = string.Empty;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddBackButton();
            SetViews();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            InitializeVerifyPinSentView();
        }

        internal void AddBackButton()
        {
            NavigationItem.HidesBackButton = true;
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        void SetViews()
        {
            UILabel lblDescription = new UILabel(new CGRect(18, 16, View.Frame.Width - 36, 36));
            lblDescription.Font = myTNBFont.MuseoSans14();
            lblDescription.TextColor = myTNBColor.TunaGrey();
            lblDescription.LineBreakMode = UILineBreakMode.WordWrap;
            lblDescription.Lines = 0;
            lblDescription.Text = "Please enter the 4 digit code that was sent to " + EmailAddress;
            lblDescription.TextAlignment = UITextAlignment.Left;
            View.AddSubview(lblDescription);

            UILabel lblResendToken = new UILabel(new CGRect(18, 158, View.Frame.Width - 36, 16));
            lblResendToken.Font = myTNBFont.MuseoSans12();
            lblResendToken.TextColor = myTNBColor.TunaGrey();
            lblResendToken.LineBreakMode = UILineBreakMode.WordWrap;
            lblResendToken.Lines = 0;
            lblResendToken.Text = "Didn’t receive the email?";
            lblResendToken.TextAlignment = UITextAlignment.Center;
            View.AddSubview(lblResendToken);

            CreateTokenField();

            int xLocation = ((int)View.Bounds.Width - 140) / 2;
            int yLocation = 180;
            int width = 140;
            int height = 48;
            _loadingView = new UIView(new CGRect(xLocation, yLocation, width, height));
            _loadingView.BackgroundColor = UIColor.White;
            _loadingView.Layer.CornerRadius = 5.0f;
            _loadingView.Layer.BorderWidth = 1.0f;
            _loadingView.Layer.BorderColor = myTNBColor.FreshGreen().CGColor;
            View.AddSubview(_loadingView);
            _loadingImage = new UIImageView(new CGRect(14, 13, 24, 24));
            _resendLabel = new UILabel(new CGRect(41, 15, 100, 20));
            _segment = new UIView(new CGRect(0, 0, 0, height));
            _loadingView.AddSubview(_segment);
            _loadingView.AddSubview(_loadingImage);
            _loadingView.AddSubview(_resendLabel);
            _onResendPin = new UITapGestureRecognizer(() =>
            {
                Console.WriteLine("ON TAP RESEND");
                ClearTokenField();
                ExecuteSendResetPasswordCodeCall();
            });
            AnimateResendView();
        }

        void CreateTokenField()
        {
            UITextField txtFieldToken;
            UIView viewLine;
            _viewTokenFieldContainer = new UIView(new CGRect(66, 70, View.Frame.Width - 132, 40));
            _lblError = new UILabel(new CGRect(0, _viewTokenFieldContainer.Frame.Height - 14, _viewTokenFieldContainer.Frame.Width, 14));
            _lblError.Font = myTNBFont.MuseoSans9();
            _lblError.TextColor = myTNBColor.Tomato();
            _lblError.TextAlignment = UITextAlignment.Left;
            _lblError.Text = "Invalid code.";
            _lblError.Hidden = true;
            float txtFieldWidth = ((float)_viewTokenFieldContainer.Frame.Width - 36) / 4;
            float xLocation = 0;
            for (int i = 0; i < 4; i++)
            {
                int index = i;
                txtFieldToken = new UITextField(new CGRect(xLocation, 0, txtFieldWidth, 24));
                txtFieldToken.Placeholder = "-";
                txtFieldToken.TextColor = myTNBColor.TunaGrey();
                txtFieldToken.Font = myTNBFont.MuseoSans16();
                txtFieldToken.Tag = index + 1;
                txtFieldToken.KeyboardType = UIKeyboardType.NumberPad;
                txtFieldToken.AutocorrectionType = UITextAutocorrectionType.No;
                txtFieldToken.AutocapitalizationType = UITextAutocapitalizationType.None;
                txtFieldToken.SpellCheckingType = UITextSpellCheckingType.No;
                txtFieldToken.ReturnKeyType = UIReturnKeyType.Done;
                txtFieldToken.TextAlignment = UITextAlignment.Center;
                txtFieldToken.ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    var newLength = textField.Text.Length + replacementString.Length - range.Length;
                    return newLength <= 1;
                };
                _textFieldHelper.CreateDoneButton(txtFieldToken);
                SetTextFieldEvents(txtFieldToken);

                viewLine = new UIView(new CGRect(xLocation, 25, txtFieldWidth, 1));
                viewLine.BackgroundColor = myTNBColor.PlatinumGrey();
                viewLine.Tag = 6;

                _viewTokenFieldContainer.AddSubview(viewLine);
                _viewTokenFieldContainer.AddSubview(txtFieldToken);

                xLocation += 12 + txtFieldWidth;
            }
            _viewTokenFieldContainer.AddSubview(_lblError);
            View.AddSubview(_viewTokenFieldContainer);
        }

        void AnimateResendView()
        {
            UIView.Animate(30, 1, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _segment.Frame = new CGRect(0, 0, 140, 48);
                //Fresh green with 24% opacity
                _segment.BackgroundColor = new UIColor(red: 0.13f, green: 0.74f, blue: 0.30f, alpha: 0.24f);
                _loadingImage.Image = _loadingImg;
                _resendLabel.Text = "Resend (30)";
                _resendLabel.TextColor = myTNBColor.FreshGreen();
            }, () =>
            {
                _segment.Frame = new CGRect(0, 0, 140, 48);
                _segment.BackgroundColor = myTNBColor.FreshGreen();
                _segment.Layer.CornerRadius = 5.0f;
                _loadingImage.Frame = new CGRect(25, 13, 24, 24);
                _resendLabel.Frame = new CGRect(55, 15, 85, 20);
                _resendLabel.Text = "Resend";
                _resendLabel.TextColor = UIColor.White;
                _loadingImage.Image = _loadedImg;
                _loadingView.AddGestureRecognizer(_onResendPin);
            });
        }

        void ClearTokenField()
        {
            UITextField txtFieldToken1 = _viewTokenFieldContainer.ViewWithTag(1) as UITextField;
            txtFieldToken1.Text = string.Empty;
            UITextField txtFieldToken2 = _viewTokenFieldContainer.ViewWithTag(2) as UITextField;
            txtFieldToken2.Text = string.Empty;
            UITextField txtFieldToken3 = _viewTokenFieldContainer.ViewWithTag(3) as UITextField;
            txtFieldToken3.Text = string.Empty;
            UITextField txtFieldToken4 = _viewTokenFieldContainer.ViewWithTag(4) as UITextField;
            txtFieldToken4.Text = string.Empty;
        }

        void SetTextFieldEvents(UITextField textField)
        {
            textField.EditingChanged += (sender, e) =>
            {
                if (textField.Text.Length == 1)
                {
                    textField.ResignFirstResponder();
                    _isKeyboardDismissed = true;
                    int nextTag = (int)textField.Tag + 1;
                    if (nextTag < 5)
                    {
                        UIResponder nextResponder = textField.Superview.ViewWithTag(nextTag);
                        nextResponder.BecomeFirstResponder();
                        _isKeyboardDismissed = false;
                    }
                }
                ValidateFields(_isKeyboardDismissed);
            };
            textField.EditingDidBegin += (sender, e) =>
            {
            };
            textField.ShouldEndEditing = (sender) =>
            {
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
        }

        void InitializeVerifyPinSentView()
        {
            _viewPinSent = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 64));
            _viewPinSent.BackgroundColor = myTNBColor.SunGlow();
            _viewPinSent.Layer.CornerRadius = 2.0f;
            _viewPinSent.Hidden = true;

            UILabel lblPinSent = new UILabel(new CGRect(16, 16, _viewPinSent.Frame.Width - 32, 32));
            lblPinSent.TextAlignment = UITextAlignment.Left;
            lblPinSent.Font = myTNBFont.MuseoSans12();
            lblPinSent.TextColor = myTNBColor.TunaGrey();
            lblPinSent.Text = "An email containing the activation pin has been sent to your email Address.";
            lblPinSent.Lines = 0;
            lblPinSent.LineBreakMode = UILineBreakMode.WordWrap;

            _viewPinSent.AddSubview(lblPinSent);

            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            currentWindow.AddSubview(_viewPinSent);
        }

        internal void ShowViewPinSent()
        {
            _viewPinSent.Hidden = false;
            _viewPinSent.Alpha = 1.0f;
            UIView.Animate(5, 1, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _viewPinSent.Alpha = 0.0f;
            }, () =>
            {
                _viewPinSent.Hidden = true;
            });
        }

        void DisplayAlertView(string title, string message)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
        }

        void ValidateFields(bool isKeyboardDismissed)
        {
            UITextField txtFieldToken1 = _viewTokenFieldContainer.ViewWithTag(1) as UITextField;
            UITextField txtFieldToken2 = _viewTokenFieldContainer.ViewWithTag(2) as UITextField;
            UITextField txtFieldToken3 = _viewTokenFieldContainer.ViewWithTag(3) as UITextField;
            UITextField txtFieldToken4 = _viewTokenFieldContainer.ViewWithTag(4) as UITextField;

            if (!string.IsNullOrEmpty(txtFieldToken1.Text) && !string.IsNullOrEmpty(txtFieldToken2.Text)
               && !string.IsNullOrEmpty(txtFieldToken3.Text) && !string.IsNullOrEmpty(txtFieldToken4.Text) && isKeyboardDismissed)
            {
                ActivityIndicator.Show();
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            _token = txtFieldToken1.Text + txtFieldToken2.Text + txtFieldToken3.Text + txtFieldToken4.Text;
                            ExecuteResetPasswordWithTokenCall();
                        }
                        else
                        {
                            DisplayAlertView("No Data Connection", "Please check your data connection and try again.");
                            ActivityIndicator.Hide();
                        }
                    });
                });
            }
        }

        internal void ExecuteSendResetPasswordCodeCall()
        {
            ActivityIndicator.Show();
            SendResetPasswordCode().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (ServiceCall.ValidateBaseResponse(_resetCodeList))
                    {
                        ShowViewPinSent();
                        _segment.RemoveFromSuperview();
                        _loadingImage.RemoveFromSuperview();
                        _resendLabel.RemoveFromSuperview();
                        _loadingImage = new UIImageView(new CGRect(14, 13, 24, 24));
                        _resendLabel = new UILabel(new CGRect(41, 15, 100, 20));
                        _segment = new UIView(new CGRect(0, 0, 0, 48));
                        _loadingView.AddSubview(_segment);
                        _loadingView.AddSubview(_loadingImage);
                        _loadingView.AddSubview(_resendLabel);
                        _loadingView.RemoveGestureRecognizer(_onResendPin);
                        AnimateResendView();
                    }
                    else
                    {
                        DisplayAlertView("Error", "Error in sending reset code.");
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
                    username = EmailAddress,
                    userEmail = EmailAddress
                };
                _resetCodeList = serviceManager.BaseServiceCall("SendResetPasswordCode", requestParameter);
            });
        }

        internal void ExecuteResetPasswordWithTokenCall()
        {
            _resetCodeList = new BaseResponseModel();
            ResetPasswordWithToken().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (ServiceCall.ValidateBaseResponse(_resetCodeList))
                    {
                        var sharedPreference = NSUserDefaults.StandardUserDefaults;
                        sharedPreference.SetBool(true, "isPasswordResetCodeSent");
                        sharedPreference.SetString(_resetCodeList.d.message, "resetPasswordMessage");
                        sharedPreference.Synchronize();
                        UIStoryboard storyBoard = UIStoryboard.FromName("ForgotPassword", null);
                        PasswordResetSuccessViewController viewController =
                            storyBoard.InstantiateViewController("PasswordResetSuccessViewController") as PasswordResetSuccessViewController;
                        viewController.IsChangePassword = false;
                        viewController.EmailAddress = EmailAddress;
                        NavigationController.PushViewController(viewController, false);
                    }
                    else
                    {
                        DisplayAlertView("Reset password with token error.", _resetCodeList.d.message);
                    }

                    ActivityIndicator.Hide();
                });
            });
        }

        internal Task ResetPasswordWithToken()
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
                    username = EmailAddress,
                    token = _token
                };
                _resetCodeList = serviceManager.BaseServiceCall("ResetPasswordWithToken", requestParameter);
            });
        }
    }
}