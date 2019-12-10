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
        public EnterCodeViewController(IntPtr handle) : base(handle) { }

        public string EmailAddress = string.Empty;

        private BaseResponseModelV2 _resetCodeList = new BaseResponseModelV2();
        private TextFieldHelper _textFieldHelper = new TextFieldHelper();

        private UIView _viewTokenFieldContainer, _loadingView, _segment, _viewPinSent;
        private UIImage _loadingImg = UIImage.FromBundle("Loading");
        private UIImage _loadedImg = UIImage.FromBundle("Loaded");

        private UILabel _lblError, _resendLabel;
        private UIImageView _loadingImage;
        private UITapGestureRecognizer _onResendPin;

        private bool _isKeyboardDismissed, _isTokenInvalid;
        private string _token = string.Empty;

        private Timer timer;
        private const double INTERVAL = 1000f;
        private int timerCtr = 30;
        private int margin = 0;

        public override void ViewDidLoad()
        {
            PageName = ForgotPasswordConstants.Pagename_EnterCode;
            base.ViewDidLoad();
            timer = new Timer();
            timer.Interval = INTERVAL;
            timer.Elapsed += TimerElapsed;
            timer.AutoReset = true;
            if (TNBGlobal.APP_LANGUAGE == "MS")
            {
                margin += 60;
            }
            AddBackButton();
            SetViews();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            InitializeVerifyPinSentView();
        }

        private void AddBackButton()
        {
            NavigationItem.HidesBackButton = true;
            NavigationItem.Title = GetI18NValue(ForgotPasswordConstants.I18N_Title);
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void SetViews()
        {
            UILabel lblDescription = new UILabel(new CGRect(18, 8, View.Frame.Width - 36, 60))
            {
                Font = MyTNBFont.MuseoSans16_300,
                TextColor = MyTNBColor.TunaGrey(),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = string.Format(GetI18NValue(ForgotPasswordConstants.I18N_Details), EmailAddress),
                TextAlignment = UITextAlignment.Left
            };
            View.AddSubview(lblDescription);

            UILabel lblResendToken = new UILabel(new CGRect(18, 158, View.Frame.Width - 36, 16))
            {
                Font = MyTNBFont.MuseoSans12_300,
                TextColor = MyTNBColor.TunaGrey(),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = GetI18NValue(ForgotPasswordConstants.I18N_EmailNotReceived),
                TextAlignment = UITextAlignment.Center
            };
            View.AddSubview(lblResendToken);

            CreateTokenField();

            int xLocation = ((int)View.Bounds.Width - (140 + margin)) / 2;
            int yLocation = 180;
            int width = 140 + margin;
            int height = 48;
            _loadingView = new UIView(new CGRect(xLocation, yLocation, width, height))
            {
                BackgroundColor = UIColor.White
            };
            _loadingView.Layer.CornerRadius = 5.0f;
            _loadingView.Layer.BorderWidth = 1.0f;
            _loadingView.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            View.AddSubview(_loadingView);
            _loadingImage = new UIImageView(new CGRect(14, 13, 24, 24));
            _resendLabel = new UILabel(new CGRect(41, 15, 100 + margin, 20));
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

        private void CreateTokenField()
        {
            UITextField txtFieldToken;
            UIView viewLine;
            _viewTokenFieldContainer = new UIView(new CGRect(66, 70, View.Frame.Width - 132, 40));
            _lblError = new UILabel(new CGRect(0, _viewTokenFieldContainer.Frame.Height - 14, _viewTokenFieldContainer.Frame.Width, 14))
            {
                Font = MyTNBFont.MuseoSans9_300,
                TextColor = MyTNBColor.Tomato,
                TextAlignment = UITextAlignment.Left,
                Text = GetErrorI18NValue(Constants.Error_InvalidCode),
                Hidden = true
            };
            float txtFieldWidth = ((float)_viewTokenFieldContainer.Frame.Width - 36) / 4;
            float xLocation = 0;
            for (int i = 0; i < 4; i++)
            {
                int index = i;
                txtFieldToken = new UITextField(new CGRect(xLocation, 0, txtFieldWidth, 24))
                {
                    Placeholder = "-",
                    TextColor = MyTNBColor.TunaGrey(),
                    Font = MyTNBFont.MuseoSans16_300,
                    Tag = index + 1,
                    KeyboardType = UIKeyboardType.NumberPad,
                    AutocorrectionType = UITextAutocorrectionType.No,
                    AutocapitalizationType = UITextAutocapitalizationType.None,
                    SpellCheckingType = UITextSpellCheckingType.No,
                    ReturnKeyType = UIReturnKeyType.Done,
                    TextAlignment = UITextAlignment.Center,
                    ShouldChangeCharacters = (textField, range, replacementString) =>
                    {
                        nint newLength = textField.Text.Length + replacementString.Length - range.Length;
                        return newLength <= 1;
                    }
                };
                _textFieldHelper.CreateDoneButton(txtFieldToken);

                viewLine = new UIView(new CGRect(xLocation, 25, txtFieldWidth, 1))
                {
                    BackgroundColor = MyTNBColor.PlatinumGrey,
                    Tag = index + 5
                };

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
                    _resendLabel.Text = string.Format("{0} ({1})", GetCommonI18NValue(Constants.Common_Resend), timerCtr);
                });
                timerCtr = timerCtr - 1;
            }
            else
            {
                timer.Enabled = false;
            }
        }

        private void AnimateResendView()
        {
            timerCtr = 30;
            _resendLabel.Text = string.Format("{0} ({1})", GetCommonI18NValue(Constants.Common_Resend), timerCtr);
            _resendLabel.TextColor = MyTNBColor.FreshGreen;
            timer.Enabled = true;
            UIView.Animate(30, 1, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _segment.Frame = new CGRect(0, 0, 140 + margin, 48);
                //Fresh green with 24% opacity
                _segment.BackgroundColor = new UIColor(red: 0.13f, green: 0.74f, blue: 0.30f, alpha: 0.24f);
                _loadingImage.Image = _loadingImg;
            }, () =>
            {
                _segment.Frame = new CGRect(0, 0, 140 + margin, 48);
                _segment.BackgroundColor = MyTNBColor.FreshGreen;
                _loadingImage.Frame = new CGRect(25, 13, 24, 24);
                _resendLabel.Frame = new CGRect(55, 15, 85 + margin, 20);
                _resendLabel.Text = GetCommonI18NValue(Constants.Common_Resend);
                _resendLabel.TextColor = UIColor.White;
                _loadingImage.Image = _loadedImg;
                _loadingView.AddGestureRecognizer(_onResendPin);
            });
        }

        private void ClearTokenField()
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

        private void SetTextFieldEvents(UITextField textField, UIView viewLine)
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

        private void InitializeVerifyPinSentView()
        {
            _viewPinSent = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 64));
            _viewPinSent.BackgroundColor = MyTNBColor.SunGlow;
            _viewPinSent.Layer.CornerRadius = 2.0f;
            _viewPinSent.Hidden = true;

            UILabel lblPinSent = new UILabel(new CGRect(16, 16, _viewPinSent.Frame.Width - 32, 32));
            lblPinSent.TextAlignment = UITextAlignment.Left;
            lblPinSent.Font = MyTNBFont.MuseoSans12_300;
            lblPinSent.TextColor = MyTNBColor.TunaGrey();
            lblPinSent.Text = GetI18NValue(ForgotPasswordConstants.I18N_ResendPinMessage);
            lblPinSent.Lines = 0;
            lblPinSent.LineBreakMode = UILineBreakMode.WordWrap;

            _viewPinSent.AddSubview(lblPinSent);

            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            currentWindow.AddSubview(_viewPinSent);
        }

        private void ShowViewPinSent()
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

        private void DisplayAlertView(string title, string message)
        {
            UIAlertController alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(alert, animated: true, completionHandler: null);
        }

        /// <summary>
        /// Updates the color of the text field based on OTP validity input
        /// </summary>
        private void UpdateTextFieldColor()
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
        private void IsPinInvalid()
        {
            _lblError.Hidden = !_isTokenInvalid;
        }

        private void ValidateFields(bool isKeyboardDismissed)
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
                                DisplayNoDataAlert();
                                ActivityIndicator.Hide();
                            }
                        });
                    });
                }
            }
        }

        private void ExecuteSendResetPasswordCodeCall()
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
                        _resendLabel = new UILabel(new CGRect(41, 15, 100 + margin, 20));
                        _segment = new UIView(new CGRect(0, 0, 0, 48));
                        _loadingView.AddSubview(_segment);
                        _loadingView.AddSubview(_loadingImage);
                        _loadingView.AddSubview(_resendLabel);
                        _loadingView.RemoveGestureRecognizer(_onResendPin);
                        AnimateResendView();
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
                        eid = EmailAddress,
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

        private void ExecuteResetPasswordWithTokenCall()
        {
            _resetCodeList = new BaseResponseModelV2();
            ResetPasswordWithToken().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (ServiceCall.ValidateBaseResponse(_resetCodeList))
                    {
                        NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                        sharedPreference.SetBool(true, "isPasswordResetCodeSent");
                        sharedPreference.SetString(_resetCodeList.d.ErrorMessage, "resetPasswordMessage");
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
                        DisplayServiceError(_resetCodeList?.d?.ErrorMessage);
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private Task ResetPasswordWithToken()
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
                        eid = EmailAddress,
                        sspuid = DataManager.DataManager.SharedInstance.User.UserID,
                        did = DataManager.DataManager.SharedInstance.UDID,
                        ft = fcmToken,
                        lang = TNBGlobal.APP_LANGUAGE,
                        sec_auth_k1 = TNBGlobal.API_KEY_ID,
                        sec_auth_k2 = string.Empty,
                        ses_param1 = string.Empty,
                        ses_param2 = string.Empty
                    },
                    token = _token
                };
                _resetCodeList = serviceManager.BaseServiceCallV6(ForgotPasswordConstants.Service_ResetPasswordWithToken, requestParameter);
            });
        }
    }
}