using System;
using myTNB.Model;
using myTNB.Registration.CustomerAccounts;
using UIKit;
using System.Threading.Tasks;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.SQLite.SQLiteDataManager;
using System.Drawing;
using myTNB.DataManager;
using System.Timers;
using myTNB.Registration.VerifyPin;

namespace myTNB.Registration
{
    public partial class VerifyPinViewController : CustomUIViewController
    {
        public VerifyPinViewController(IntPtr handle) : base(handle) { }

        private NewUserResponseModel _registerAccountList = new NewUserResponseModel();
        private UserAuthenticationResponseModel _authenticationList = new UserAuthenticationResponseModel();
        private RegistrationTokenSMSResponseModel _smsToken = new RegistrationTokenSMSResponseModel();
        private UIView _viewTokenFieldContainer, _loadingView, _segment, _viewPinSent, _commonView, _headerView;
        private UIImage _loadingImg = UIImage.FromBundle("Loading");
        private UIImage _loadedImg = UIImage.FromBundle("Loaded");
        private UILabel _lblError, _resendLabel;
        private UIImageView _loadingImage;
        private UITapGestureRecognizer _onResendPin;

        private bool _isKeyboardDismissed, _isTokenInvalid;
        private string _token = string.Empty;
        private string _mobileNo;

        public bool IsMobileVerification = false;
        public bool IsFromLogin = false;

        private Timer timer;
        const double INTERVAL = 1000f;
        private int timerCtr = 30;

        public override void ViewDidLoad()
        {
            PageName = VerifyPinConstants.Pagename_VerifyPin;
            base.ViewDidLoad();
            // Perform any dditional setup after loading the view, typically from a nib.
            NavigationController.NavigationBar.Hidden = false;
            AddBackButton();
            if (NavigationItem != null)
            {
                if (IsMobileVerification)
                {
                    NavigationItem.SetHidesBackButton(true, false);
                }
                NavigationItem.Title = GetI18NValue(VerifyPinConstants.I18N_Title);
            }

            timer = new Timer
            {
                Interval = INTERVAL,
                AutoReset = true
            };
            timer.Elapsed += TimerElapsed;

            SetViews();
            SetEvents(IsMobileVerification);
            InitializeVerifyPinSentView();
            ShowViewPinSent();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void ShowAccountsVC()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("AccountRecords", null);
            AccountsViewController viewController =
                storyBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
            if (viewController != null)
            {
                viewController._needsUpdate = true;
                viewController.isDashboardFlow = false;
                NavigationController?.PushViewController(viewController, true);
            }
        }

        private void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController?.PopViewController(true);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void CreateTokenField(UIView parentView)
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
                    Font = MyTNBFont.MuseoSans16,
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
                CreateDoneButton(txtFieldToken);

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
            parentView.AddSubview(_viewTokenFieldContainer);
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
            {
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
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

        private void ClearTokenField()
        {
            UITextField txtFieldToken1 = _viewTokenFieldContainer.ViewWithTag(1) as UITextField;
            if (txtFieldToken1 != null)
            {
                txtFieldToken1.Text = string.Empty;
            }
            UITextField txtFieldToken2 = _viewTokenFieldContainer.ViewWithTag(2) as UITextField;
            if (txtFieldToken2 != null)
            {
                txtFieldToken2.Text = string.Empty;
            }
            UITextField txtFieldToken3 = _viewTokenFieldContainer.ViewWithTag(3) as UITextField;
            if (txtFieldToken3 != null)
            {
                txtFieldToken3.Text = string.Empty;
            }
            UITextField txtFieldToken4 = _viewTokenFieldContainer.ViewWithTag(4) as UITextField;
            if (txtFieldToken4 != null)
            {
                txtFieldToken4.Text = string.Empty;
            }
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
            if (_isTokenInvalid)
                _lblError.Hidden = false;
            else
                _lblError.Hidden = true;
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
                        InvokeOnMainThread(async () =>
                        {
                            if (NetworkUtility.isReachable)
                            {
                                _token = txtFieldToken1.Text + txtFieldToken2.Text + txtFieldToken3.Text + txtFieldToken4.Text;
                                if (!IsMobileVerification)
                                {
                                    ExecuteRegisterUserCall();
                                }
                                else
                                {
                                    BaseResponseModel response = await ServiceCall.UpdatePhoneNumber(_mobileNo, _token, IsFromLogin);

                                    if (response?.d?.didSucceed == true)
                                    {
                                        DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo = _mobileNo;
                                        if (!IsFromLogin)
                                        {
                                            DataManager.DataManager.SharedInstance.IsMobileNumberUpdated = true;
                                        }
                                        UserEntity userEntity = new UserEntity();
                                        userEntity.Reset();
                                        userEntity.InsertItem(DataManager.DataManager.SharedInstance.UserEntity[0]);
                                        DataManager.DataManager.SharedInstance.UserEntity = userEntity.GetAllItems();

                                        if (IsFromLogin)
                                        {
                                            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                                            sharedPreference.SetBool(true, TNBGlobal.PreferenceKeys.LoginState);
                                            sharedPreference.SetBool(true, TNBGlobal.PreferenceKeys.PhoneVerification);
                                            sharedPreference.Synchronize();
                                            ExecuteGetCutomerRecordsCall();
                                        }
                                        else
                                        {
                                            DismissViewController(true, null);
                                            ActivityIndicator.Hide();
                                        }

                                    }
                                    else
                                    {
                                        _isTokenInvalid = true;
                                        IsPinInvalid();
                                        UpdateTextFieldColor();
                                        DisplayServiceError(response?.d?.message ?? string.Empty);
                                        ActivityIndicator.Hide();
                                    }
                                }
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

        private void SetViews()
        {
            nfloat locX = !IsMobileVerification ? 0 : _headerView?.Frame.Height ?? 0;
            _commonView = new UIView(new CGRect(0, locX, View.Bounds.Width, 300));

            _mobileNo = DataManager.DataManager.SharedInstance.User.MobileNo;
            UILabel lblDescription = new UILabel(new CGRect(18, 8, View.Frame.Width - 36, 60))
            {
                Font = MyTNBFont.MuseoSans16_300,
                TextColor = MyTNBColor.TunaGrey(),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };
            string desc = !IsMobileVerification ? GetI18NValue(VerifyPinConstants.I18N_OTPRegistration)
                : GetI18NValue(VerifyPinConstants.I18N_OTPMobileUpdate);
            lblDescription.Text = string.Format(desc, _mobileNo);
            lblDescription.TextAlignment = UITextAlignment.Left;
            _commonView.AddSubview(lblDescription);

            UILabel lblResendToken = new UILabel(new CGRect(18, 158, View.Frame.Width - 36, 16))
            {
                Font = MyTNBFont.MuseoSans12_300,
                TextColor = MyTNBColor.TunaGrey(),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Text = GetI18NValue(VerifyPinConstants.I18N_SMSNotreceived),
                TextAlignment = UITextAlignment.Center
            };
            _commonView.AddSubview(lblResendToken);

            CreateTokenField(_commonView);

            int xLocation = ((int)View.Bounds.Width - 140) / 2;
            int yLocation = 180;
            int width = 140;
            int height = 48;
            _loadingView = new UIView(new CGRect(xLocation, yLocation, width, height))
            {
                BackgroundColor = UIColor.White
            };
            _loadingView.Layer.CornerRadius = 5.0f;
            _loadingView.Layer.BorderWidth = 1.0f;
            _loadingView.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _commonView.AddSubview(_loadingView);

            _loadingImage = new UIImageView(new CGRect(14, 13, 24, 24));
            _resendLabel = new UILabel(new CGRect(41, 15, 100, 20));
            _segment = new UIView(new CGRect(0, 0, 0, height));
            _segment.Layer.CornerRadius = 5.0f;
            _loadingView.AddSubview(_segment);
            _loadingView.AddSubview(_loadingImage);
            _loadingView.AddSubview(_resendLabel);

            View.AddSubview(_commonView);

            AnimateResendView();
        }

        /// <summary>
        /// Sets the events.
        /// </summary>
        /// <param name="isMobileUpdate">If set to <c>true</c> is mobile update.</param>
        private void SetEvents(bool isMobileUpdate)
        {
            if (!isMobileUpdate)
            {
                _onResendPin = new UITapGestureRecognizer(() =>
                {
                    ClearTokenField();
                    ExecuteSendRegistrationTokenSMSCall();
                });
            }
            else
            {
                _onResendPin = new UITapGestureRecognizer(async () =>
                {
                    string mobileNo = DataManager.DataManager.SharedInstance.User.MobileNo;
                    if (!string.IsNullOrEmpty(mobileNo))
                    {
                        ClearTokenField();
                        BaseResponseModel response = await ServiceCall.SendUpdatePhoneTokenSMS(mobileNo);
                        if (response?.d?.didSucceed == true)
                        {
                            ShowViewPinSent();
                            CreateResendView();
                            AnimateResendView();
                        }
                        else
                        {
                            DisplayServiceError(_smsToken?.d?.message ?? string.Empty);
                        }
                    }
                });
            }
        }

        private void ExecuteSendRegistrationTokenSMSCall()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        SendRegistrationTokenSMS().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_smsToken != null && _smsToken?.d != null
                                    && _smsToken?.d?.didSucceed == true
                                    && _smsToken?.d?.status?.ToLower() == "success")
                                {
                                    ShowViewPinSent();
                                    CreateResendView();
                                    AnimateResendView();
                                }
                                else
                                {
                                    DisplayServiceError(_smsToken?.d?.message ?? string.Empty);
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        private Task SendRegistrationTokenSMS()
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
                    username = DataManager.DataManager.SharedInstance.User.Email,
                    userEmail = DataManager.DataManager.SharedInstance.User.Email,
                    mobileNo = DataManager.DataManager.SharedInstance.User.MobileNo
                };
                _smsToken = serviceManager.OnExecuteAPI<RegistrationTokenSMSResponseModel>("SendRegistrationTokenSMS_V2", requestParameter);
            });
        }

        private void DisplayRegistrationAlertView(string title, string message)
        {
            UIAlertController alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Cancel), UIAlertActionStyle.Cancel, (obj) =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                UIViewController viewController =
                    storyBoard.InstantiateViewController("RegistrationViewController") as UIViewController;
                NavigationController?.PushViewController(viewController, true);
            }));
            alert.AddAction(UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Retry), UIAlertActionStyle.Default, (obj) =>
            {
                ValidateFields(_isKeyboardDismissed);
            }));
            alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(alert, animated: true, completionHandler: null);
        }

        private void CreateResendView()
        {
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
                _resendLabel.Text = GetCommonI18NValue(Constants.Common_Resend);
                _resendLabel.TextColor = UIColor.White;
                _loadingImage.Image = _loadedImg;
                _loadingView.AddGestureRecognizer(_onResendPin);
            });
        }

        private void ExecuteRegisterUserCall()
        {
            RegisterUser().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_registerAccountList != null && _registerAccountList?.d != null)
                    {
                        NewUserModel newUser = _registerAccountList?.d;
                        if (newUser?.didSucceed == true && newUser?.status?.ToLower() == "success")
                        {
                            ExecuteLoginCall();
                        }
                        else
                        {
                            //for testing
                            //DataManager.DataManager.SharedInstance.User.ICNo = DataManager.DataManager.SharedInstance.User.ICNo;
                            //ExecuteLoginCall();

                            DisplayServiceError(newUser?.message ?? string.Empty);
                            ClearTokenField();
                            ActivityIndicator.Hide();
                        }
                    }
                    else
                    {
                        DisplayServiceError(_registerAccountList?.d?.message ?? string.Empty);
                        ClearTokenField();
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        private Task RegisterUser()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    ipAddress = TNBGlobal.API_KEY_ID,
                    clientType = AppVersionHelper.GetBuildVersion(),
                    activeUserName = TNBGlobal.API_KEY_ID,
                    devicePlatform = TNBGlobal.DEVICE_PLATFORM_IOS,
                    deviceVersion = DeviceHelper.GetOSVersion(),
                    deviceCordova = TNBGlobal.API_KEY_ID,
                    displayName = DataManager.DataManager.SharedInstance.User.DisplayName,
                    username = DataManager.DataManager.SharedInstance.User.Email,
                    email = DataManager.DataManager.SharedInstance.User.Email,
                    token = _token,
                    password = DataManager.DataManager.SharedInstance.User.Password,
                    confirmPassword = DataManager.DataManager.SharedInstance.User.Password,
                    icNo = DataManager.DataManager.SharedInstance.User.ICNo,
                    mobileNo = DataManager.DataManager.SharedInstance.User.MobileNo
                };
                _registerAccountList = serviceManager.OnExecuteAPI<NewUserResponseModel>("CreateNewUserWithToken", requestParameter);
            });
        }

        private void SetLoginLocalData()
        {
            DataManager.DataManager.SharedInstance.User.UserID = _authenticationList?.d?.data?.userID;
            //For testing only
            //DataManager.DataManager.SharedInstance.User.UserID = "c39522f3-76cd-41cb-b08d-c2c639a72e01";
            UserEntity uManager = new UserEntity();
            uManager.DeleteTable();
            uManager.CreateTable();
            uManager.InsertListOfItems(_authenticationList?.d?.data);
            DataManager.DataManager.SharedInstance.UserEntity = uManager.GetAllItems();
        }

        private void ExecuteLoginCall()
        {
            Login().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_authenticationList != null && _authenticationList?.d != null && _authenticationList?.d?.data != null)
                    {
                        if (_authenticationList?.d?.didSucceed == true)
                        {
                            UserAuthenticationModel auth = _authenticationList?.d?.data;
                            SetLoginLocalData();
                            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                            sharedPreference.SetBool(true, TNBGlobal.PreferenceKeys.LoginState);
                            sharedPreference.Synchronize();
                            //PushNotificationHelper.GetNotifications();
                            DataManager.DataManager.SharedInstance.User.Password = string.Empty;
                            ShowAccountsVC();
                        }
                        else
                        {
                            DisplayServiceError(_authenticationList?.d?.message ?? string.Empty);
                            ClearTokenField();
                        }
                        ActivityIndicator.Hide();
                    }
                    else
                    {
                        DisplayServiceError(_authenticationList?.d?.message ?? string.Empty);
                        ClearTokenField();
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private Task Login()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    userName = DataManager.DataManager.SharedInstance.User.Email,
                    password = DataManager.DataManager.SharedInstance.User.Password,
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    ipAddress = TNBGlobal.API_KEY_ID,
                    clientType = AppVersionHelper.GetBuildVersion(),
                    activeUserName = TNBGlobal.API_KEY_ID,
                    devicePlatform = TNBGlobal.DEVICE_PLATFORM_IOS,
                    deviceVersion = DeviceHelper.GetOSVersion(),
                    deviceCordova = TNBGlobal.API_KEY_ID,
                    deviceId = DataManager.DataManager.SharedInstance.UDID,
                    fcmToken = DataManager.DataManager.SharedInstance.FCMToken != null
                        && !string.IsNullOrEmpty(DataManager.DataManager.SharedInstance.FCMToken)
                        && !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.FCMToken)
                            ? DataManager.DataManager.SharedInstance.FCMToken : string.Empty
                };
                _authenticationList = serviceManager.OnExecuteAPI<UserAuthenticationResponseModel>("IsUserAuthenticate", requestParameter);
            });
        }

        private void ExecuteGetCutomerRecordsCall()
        {
            ServiceCall.GetAccounts().ContinueWith(task =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (DataManager.DataManager.SharedInstance.CustomerAccounts != null
                       && DataManager.DataManager.SharedInstance.CustomerAccounts?.d != null
                       && DataManager.DataManager.SharedInstance.CustomerAccounts?.d?.data != null)
                    {
                        DataManager.DataManager.SharedInstance.AccountRecordsList.d
                                   = DataManager.DataManager.SharedInstance.CustomerAccounts?.d?.data;
                    }

                    if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                        && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null)
                    {
                        UserAccountsEntity uaManager = new UserAccountsEntity();
                        uaManager.DeleteTable();
                        uaManager.CreateTable();
                        uaManager.InsertListOfItems(DataManager.DataManager.SharedInstance.AccountRecordsList);
                    }

                    if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                       && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null)
                    {
                        if (DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0)
                        {
                            DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList?.d[0];
                            await ExecuteGetBillAccountDetailsCall();
                        }
                        else
                        {
                            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                            UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                            loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            PresentViewController(loginVC, true, null);
                            ActivityIndicator.Hide();
                        }
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.AccountRecordsList = new CustomerAccountRecordListModel();
                        DataManager.DataManager.SharedInstance.AccountRecordsList.d = new List<CustomerAccountRecordModel>();

                        UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                        UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                        loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(loginVC, true, null);
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        private async Task ExecuteGetBillAccountDetailsCall()
        {
            BillingAccountDetailsResponseModel _billingAccountDetailsList = await ServiceCall.GetBillingAccountDetails();
            if (_billingAccountDetailsList?.d?.didSucceed == true)
            {
                InvokeOnMainThread(() =>
                {
                    if (_billingAccountDetailsList != null && _billingAccountDetailsList?.d != null
                        && _billingAccountDetailsList?.d?.data != null)
                    {
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = _billingAccountDetailsList?.d?.data;
                        UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                        UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                        loginVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(loginVC, true, null);
                        ActivityIndicator.Hide();
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = new BillingAccountDetailsDataModel();
                        DisplayServiceError(_billingAccountDetailsList?.d?.message);
                    }
                    ActivityIndicator.Hide();
                });
            }
        }

        private void CreateDoneButton(UITextField textField)
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            UIBarButtonItem doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                textField.ResignFirstResponder();
                _isKeyboardDismissed = true;
                ValidateFields(_isKeyboardDismissed);
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            textField.InputAccessoryView = toolbar;
        }

        private void InitializeVerifyPinSentView()
        {
            _viewPinSent = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 64))
            {
                BackgroundColor = MyTNBColor.SunGlow
            };
            _viewPinSent.Layer.CornerRadius = 2.0f;
            _viewPinSent.Hidden = true;

            UILabel lblPinSent = new UILabel(new CGRect(16, 16, _viewPinSent.Frame.Width - 32, 32))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans12_300,
                TextColor = MyTNBColor.TunaGrey(),
                Text = GetI18NValue(VerifyPinConstants.I18N_ResendPin),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

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
    }
}