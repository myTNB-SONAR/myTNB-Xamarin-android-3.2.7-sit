using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.DataManager;
using System.Timers;
using myTNB.Login.ForgotPassword;

namespace myTNB
{
    public partial class EnterCodeViewController : CustomUIViewController
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
        bool _isTokenInvalid = false;
        string _token = string.Empty;

        Timer timer;
        const double INTERVAL = 1000f;
        public int timerCtr = 30;

        public override void ViewDidLoad()
        {
            PageName = ForgotPasswordConstants.Pagename_EnterCode;
            base.ViewDidLoad();
            timer = new Timer();
            timer.Interval = INTERVAL;
            timer.Elapsed += TimerElapsed;
            timer.AutoReset = true;
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
            NavigationItem.Title = GetI18NValue(ForgotPasswordConstants.I18N_Title);
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        void SetViews()
        {
            UILabel lblDescription = new UILabel(new CGRect(18, 8, View.Frame.Width - 36, 60));
            lblDescription.Font = MyTNBFont.MuseoSans16_300;
            lblDescription.TextColor = MyTNBColor.TunaGrey();
            lblDescription.LineBreakMode = UILineBreakMode.WordWrap;
            lblDescription.Lines = 0;
            lblDescription.Text = string.Format(GetI18NValue(ForgotPasswordConstants.I18N_Details), EmailAddress);
            lblDescription.TextAlignment = UITextAlignment.Left;
            View.AddSubview(lblDescription);

            UILabel lblResendToken = new UILabel(new CGRect(18, 158, View.Frame.Width - 36, 16));
            lblResendToken.Font = MyTNBFont.MuseoSans12_300;
            lblResendToken.TextColor = MyTNBColor.TunaGrey();
            lblResendToken.LineBreakMode = UILineBreakMode.WordWrap;
            lblResendToken.Lines = 0;
            lblResendToken.Text = GetI18NValue(ForgotPasswordConstants.I18N_EmailNotReceived);
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
            _loadingView.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            View.AddSubview(_loadingView);
            _loadingImage = new UIImageView(new CGRect(14, 13, 24, 24));
            _resendLabel = new UILabel(new CGRect(41, 15, 100, 20));
            _segment = new UIView(new CGRect(0, 0, 0, height));
            _segment.Layer.CornerRadius = 5.0f;
            _loadingView.AddSubview(_segment);
            _loadingView.AddSubview(_loadingImage);
            _loadingView.AddSubview(_resendLabel);
            _onResendPin = new UITapGestureRecognizer(() =>
            {
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
            _lblError.Font = MyTNBFont.MuseoSans9_300;
            _lblError.TextColor = MyTNBColor.Tomato;
            _lblError.TextAlignment = UITextAlignment.Left;
            _lblError.Text = GetErrorI18NValue(Constants.Error_InvalidCode);
            _lblError.Hidden = true;
            float txtFieldWidth = ((float)_viewTokenFieldContainer.Frame.Width - 36) / 4;
            float xLocation = 0;
            for (int i = 0; i < 4; i++)
            {
                int index = i;
                txtFieldToken = new UITextField(new CGRect(xLocation, 0, txtFieldWidth, 24));
                txtFieldToken.Placeholder = "-";
                txtFieldToken.TextColor = MyTNBColor.TunaGrey();
                txtFieldToken.Font = MyTNBFont.MuseoSans16_300;
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

                viewLine = new UIView(new CGRect(xLocation, 25, txtFieldWidth, 1));
                viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;
                viewLine.Tag = index + 5;

                SetTextFieldEvents(txtFieldToken, viewLine);

                _viewTokenFieldContainer.AddSubview(viewLine);
                _viewTokenFieldContainer.AddSubview(txtFieldToken);

                xLocation += 12 + txtFieldWidth;
            }
            _viewTokenFieldContainer.AddSubview(_lblError);
            View.AddSubview(_viewTokenFieldContainer);
        }
        /// <summary>
        /// Method to execute label update when timer lapses
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (timerCtr > 0)
            {
                InvokeOnMainThread(() =>
                {
                    _resendLabel.Text = string.Format("{0} ({1})", GetI18NValue(ForgotPasswordConstants.I18N_Resend), timerCtr);
                });
                timerCtr = timerCtr - 1;
            }
            else
            {
                timer.Enabled = false;
            }
        }

        void AnimateResendView()
        {
            timerCtr = 30;
            _resendLabel.Text = string.Format("{0} ({1})", GetI18NValue(ForgotPasswordConstants.I18N_Resend), timerCtr);
            _resendLabel.TextColor = MyTNBColor.FreshGreen;
            timer.Enabled = true;
            UIView.Animate(30, 1, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _segment.Frame = new CGRect(0, 0, 140, 48);
                //Fresh green with 24% opacity
                _segment.BackgroundColor = new UIColor(red: 0.13f, green: 0.74f, blue: 0.30f, alpha: 0.24f);
                _loadingImage.Image = _loadingImg;
            }, () =>
            {
                _segment.Frame = new CGRect(0, 0, 140, 48);
                _segment.BackgroundColor = MyTNBColor.FreshGreen;
                _loadingImage.Frame = new CGRect(25, 13, 24, 24);
                _resendLabel.Frame = new CGRect(55, 15, 85, 20);
                _resendLabel.Text = GetI18NValue(ForgotPasswordConstants.I18N_Resend);
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

        void SetTextFieldEvents(UITextField textField, UIView viewLine)
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
                        UIView vLine = _viewTokenFieldContainer.ViewWithTag((int)textField.Tag + 5) as UIView;
                        vLine.BackgroundColor = MyTNBColor.PowerBlue;
                    }
                }
                ValidateFields(_isKeyboardDismissed);
                IsPinInvalid();
                UpdateTextFieldColor();
            };
            textField.EditingDidBegin += (sender, e) =>
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
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
            textField.EditingDidEnd += (sender, e) =>
            {
                viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;
            };
        }

        void InitializeVerifyPinSentView()
        {
            _viewPinSent = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 64));
            _viewPinSent.BackgroundColor = MyTNBColor.SunGlow;
            _viewPinSent.Layer.CornerRadius = 2.0f;
            _viewPinSent.Hidden = true;

            UILabel lblPinSent = new UILabel(new CGRect(16, 16, _viewPinSent.Frame.Width - 32, 32));
            lblPinSent.TextAlignment = UITextAlignment.Left;
            lblPinSent.Font = MyTNBFont.MuseoSans12_300;
            lblPinSent.TextColor = MyTNBColor.TunaGrey();
            lblPinSent.Text = "Login_PinSentMessage".Translate();
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
            UIAlertController alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(alert, animated: true, completionHandler: null);
        }

        /// <summary>
        /// Updates the color of the text field based on OTP validity input
        /// </summary>
        internal void UpdateTextFieldColor()
        {
            for (int i = 0; i < 4; i++)
            {
                UITextField txtField = _viewTokenFieldContainer.ViewWithTag(i + 1) as UITextField;
                if (txtField != null)
                {
                    txtField.TextColor = (_isTokenInvalid) ? MyTNBColor.Tomato : MyTNBColor.TunaGrey();
                }

                if (_isTokenInvalid)
                {
                    UIView viewLine = _viewTokenFieldContainer.ViewWithTag(i + 5) as UIView;
                    if (viewLine != null)
                    {
                        viewLine.BackgroundColor = MyTNBColor.Tomato;
                    }
                }
            }

            if (_isTokenInvalid)
            {
                _isTokenInvalid = false;
            }
        }
        /// <summary>
        /// Determines when to hide/show the error message based on OTP validity input
        /// </summary>
        internal void IsPinInvalid()
        {
            _lblError.Hidden = !_isTokenInvalid;
        }

        void ValidateFields(bool isKeyboardDismissed)
        {
            UITextField txtFieldToken1 = _viewTokenFieldContainer.ViewWithTag(1) as UITextField;
            UITextField txtFieldToken2 = _viewTokenFieldContainer.ViewWithTag(2) as UITextField;
            UITextField txtFieldToken3 = _viewTokenFieldContainer.ViewWithTag(3) as UITextField;
            UITextField txtFieldToken4 = _viewTokenFieldContainer.ViewWithTag(4) as UITextField;

            if (txtFieldToken1 != null && txtFieldToken2 != null && txtFieldToken3 != null && txtFieldToken4 != null)
            {
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
                                AlertHandler.DisplayNoDataAlert(this);
                                ActivityIndicator.Hide();
                            }
                        });
                    });
                }
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
                        AlertHandler.DisplayServiceError(this, _resetCodeList?.d?.message);
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
                        NavigationController?.PushViewController(viewController, false);
                    }
                    else
                    {
                        _isTokenInvalid = true;
                        IsPinInvalid();
                        UpdateTextFieldColor();
                        AlertHandler.DisplayServiceError(this, _resetCodeList?.d?.message);
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