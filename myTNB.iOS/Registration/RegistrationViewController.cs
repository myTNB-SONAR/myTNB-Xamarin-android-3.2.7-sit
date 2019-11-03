﻿using System;
using System.Threading.Tasks;
using myTNB.Model;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Drawing;

namespace myTNB.Registration
{
    public partial class RegistrationViewController : CustomUIViewController
    {
        private UITextField txtFieldName, txtFieldICNo, txtFieldMobileNo, txtFieldEmail
            , txtFieldConfirmEmail, txtFieldPassword, txtFieldConfirmPassword;
        private UITextView txtViewDetails;
        private UIButton btnRegister;
        private UIView viewLineName, viewLineICNo, viewLineMobileNo, viewLineEmail
            , viewLineConfirmEmail, viewLinePassword, viewLineConfirmPassword
            , viewShowConfirmPassword, viewShowPassword, btnRegisterContainer;
        private UILabel lblNameTitle, lblICNoTitle, lblMobileNoTitle, lblEmailTitle
            , lblConfirmEmailTitle, lblPasswordTitle, lblConfirmPasswordTitle
            , lblNameError, lblICNoError, lblMobileNoError, lblEmailError
            , lblConfirmEmailError, lblPasswordError, lblConfirmPasswordError
            , lblNameHint, lblMobileNoHint, lblEmailHint
            , lblConfirmEmailHint, lblPasswordHint, lblICNoHint, lblConfirmPasswordHint;
        private UIScrollView ScrollView;
        private CGRect scrollViewFrame;

        public RegistrationViewController(IntPtr handle) : base(handle) { }

        private TextFieldHelper _textFieldHelper = new TextFieldHelper();
        private RegistrationTokenSMSResponseModel _smsToken = new RegistrationTokenSMSResponseModel();

        private string _eMail = string.Empty, _token = string.Empty
            , _password = string.Empty, _fullName = string.Empty
            , _icNo = string.Empty, _mobileNo = string.Empty;

        const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        const string PASSWORD_PATTERN = @"^.{8,}$"; //@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8,})$";
        const string MOBILE_NO_PATTERN = @"^[0-9 \+]+$";

        public override void ViewDidLoad()
        {
            PageName = RegisterConstants.Pagename_Register;
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            NavigationItem.HidesBackButton = true;
            Title = GetI18NValue(RegisterConstants.I18N_Title);
            InitializedSubviews();
            AddBackButton();
            SetVisibility();
            SetEvents();
            SetViews();
            NotifCenterUtility.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NotifCenterUtility.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            AutomaticallyAdjustsScrollViewInsets = false;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void InitializedSubviews()
        {

            //Scrollview
            ScrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height))
            {
                BackgroundColor = UIColor.Clear
            };
            View.AddSubview(ScrollView);

            //FullName 
            UIView viewFullName = new UIView(new CGRect(18, 16, View.Frame.Width - 36, 51))
            {
                BackgroundColor = UIColor.Clear
            };

            lblNameTitle = GetTitleLabel(GetCommonI18NValue(Constants.Common_Fullname));
            lblNameError = GetErrorLabel(GetErrorI18NValue(Constants.Error_InvalidFullname));
            txtFieldName = GetUITextField(GetCommonI18NValue(Constants.Common_Fullname));
            viewLineName = GenericLine.GetLine(new CGRect(0, 36, viewFullName.Frame.Width, 1));

            viewFullName.AddSubviews(new UIView[] { lblNameTitle, lblNameError
                , txtFieldName, viewLineName });

            //IC Number
            UIView viewICNumber = new UIView((new CGRect(18, 83, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblICNoTitle = GetTitleLabel(GetCommonI18NValue(Constants.Common_IDNumber));
            lblICNoError = GetErrorLabel(GetErrorI18NValue(Constants.Error_InvalidIDNumber));

            /*lblICNoHint = new UILabel
            {
                Frame = new CGRect(0, 37, viewICNumber.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "IC, ROC or Passport No.",
                    font: myTNBFont.MuseoSans9,
                    foregroundColor: myTNBColor.TunaGrey(),
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            lblICNoHint.Hidden = true;
            viewICNumber.AddSubview(lblICNoHint);*/

            txtFieldICNo = new UITextField
            {
                Frame = new CGRect(0, 12, viewICNumber.Frame.Width, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_IDNumber)
                    , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };
            viewLineICNo = GenericLine.GetLine(new CGRect(0, 36, viewICNumber.Frame.Width, 1));
            viewICNumber.AddSubviews(new UIView[] { lblICNoTitle, lblICNoError, txtFieldICNo, viewLineICNo });

            //Mobile Number
            UIView viewMobileNumber = new UIView((new CGRect(18, 150, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblMobileNoTitle = GetTitleLabel(GetCommonI18NValue(Constants.Common_MobileNo));
            lblMobileNoError = GetErrorLabel(GetErrorI18NValue(Constants.Error_InvalidMobileNumber));

            lblMobileNoHint = new UILabel
            {
                Frame = new CGRect(0, 37, viewMobileNumber.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString(GetHintI18NValue(Constants.Hint_MobileNumber)
                    , AttributedStringUtility.AttributedStringType.Hint),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            txtFieldMobileNo = GetUITextField(GetCommonI18NValue(Constants.Common_MobileNo));
            viewLineMobileNo = GenericLine.GetLine(new CGRect(0, 36, viewMobileNumber.Frame.Width, 1));
            viewMobileNumber.AddSubviews(new UIView[] { lblMobileNoTitle, lblMobileNoError
                , lblMobileNoHint, txtFieldMobileNo, viewLineMobileNo });

            //Email
            UIView viewEmail = new UIView((new CGRect(18, 217, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblEmailTitle = GetTitleLabel(GetCommonI18NValue(Constants.Common_Email));
            lblEmailError = GetErrorLabel(GetErrorI18NValue(Constants.Error_InvalidEmailAddress));
            txtFieldEmail = GetUITextField(GetCommonI18NValue(Constants.Common_Email));
            viewLineEmail = GenericLine.GetLine(new CGRect(0, 36, viewEmail.Frame.Width, 1));
            viewEmail.AddSubviews(new UIView[] { lblEmailTitle, lblEmailError
                , txtFieldEmail, viewLineEmail });

            //Confirm Email
            UIView viewConfirmEmail = new UIView((new CGRect(18, 284, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblConfirmEmailTitle = GetTitleLabel(GetI18NValue(RegisterConstants.I18N_ConfirmEmail));
            lblConfirmEmailError = GetErrorLabel(GetErrorI18NValue(Constants.Error_InvalidEmailAddress));

            txtFieldConfirmEmail = GetUITextField(GetI18NValue(RegisterConstants.I18N_ConfirmEmail));
            viewLineConfirmEmail = GenericLine.GetLine(new CGRect(0, 36, viewConfirmEmail.Frame.Width, 1));
            viewConfirmEmail.AddSubviews(new UIView[] { lblConfirmEmailTitle
                , lblConfirmEmailError, txtFieldConfirmEmail, viewLineConfirmEmail });

            //Password
            UIView viewPassword = new UIView((new CGRect(18, 351, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblPasswordTitle = GetTitleLabel(GetCommonI18NValue(Constants.Common_Password));
            lblPasswordError = GetErrorLabel(GetHintI18NValue(Constants.Hint_Password));

            lblPasswordHint = new UILabel
            {
                Frame = new CGRect(0, 37, viewPassword.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString(GetHintI18NValue(Constants.Hint_Password)
                    , AttributedStringUtility.AttributedStringType.Hint),
                TextAlignment = UITextAlignment.Left
            };
            lblPasswordHint.Hidden = true;

            txtFieldPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_Password)
                    , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };
            txtFieldPassword.SecureTextEntry = true;

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
                imgShowPassword.Image = UIImage.FromBundle(txtFieldPassword.SecureTextEntry
                    ? Constants.IMG_HidePassword : Constants.IMG_ShowPassword);
            }));
            viewLinePassword = GenericLine.GetLine(new CGRect(0, 36, viewPassword.Frame.Width, 1));
            viewPassword.AddSubviews(new UIView[] { lblPasswordTitle, lblPasswordError
                , lblPasswordHint, txtFieldPassword, viewShowPassword, viewLinePassword });

            //Confirm Password
            UIView viewConfirmPassword = new UIView((new CGRect(18, 418, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            lblConfirmPasswordTitle = GetTitleLabel(GetCommonI18NValue(Constants.Common_ConfirmPassword));

            lblConfirmPasswordError = new UILabel(new CGRect(0, 37, viewConfirmEmail.Frame.Width, 14))
            {
                Font = MyTNBFont.MuseoSans10_300,
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.Tomato
            };

            txtFieldConfirmPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewConfirmPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetCommonI18NValue(Constants.Common_ConfirmPassword)
                    , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };
            txtFieldConfirmPassword.SecureTextEntry = true;

            viewShowConfirmPassword = new UIView(new CGRect(viewConfirmPassword.Frame.Width - 30, 12, 24, 24))
            {
                Hidden = true
            };
            UIImageView imgShowConfirmPassword = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle(Constants.IMG_ShowPassword)
            };
            viewShowConfirmPassword.AddSubview(imgShowConfirmPassword);
            viewConfirmPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                txtFieldConfirmPassword.SecureTextEntry = !txtFieldConfirmPassword.SecureTextEntry;
                imgShowConfirmPassword.Image = UIImage.FromBundle(txtFieldConfirmPassword.SecureTextEntry
                    ? Constants.IMG_HidePassword : Constants.IMG_ShowPassword);
            }));
            viewLineConfirmPassword = GenericLine.GetLine(new CGRect(0, 36, viewConfirmPassword.Frame.Width, 1));
            viewConfirmPassword.AddSubview(viewLineConfirmPassword);
            viewConfirmPassword.AddSubviews(new UIView[] { lblConfirmPasswordTitle
                , lblConfirmPasswordError, txtFieldConfirmPassword, viewShowConfirmPassword });

            //Set keyboard types
            txtFieldICNo.KeyboardType = UIKeyboardType.Default;
            txtFieldMobileNo.KeyboardType = UIKeyboardType.NumberPad;
            txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;
            txtFieldConfirmEmail.KeyboardType = UIKeyboardType.EmailAddress;

            //Terms Details
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(RegisterConstants.I18N_TNC)
                , ref htmlBodyError, TNBFont.FONTNAME_300, (float)TNBFont.GetFontSize(12F));
            NSMutableAttributedString mutableHTMLFooter = new NSMutableAttributedString(htmlBody);

            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.PowerBlue,
                UnderlineStyle = NSUnderlineStyle.None,
                UnderlineColor = UIColor.Clear
            };
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.TunaGrey()
            }, new NSRange(0, htmlBody.Length));
            txtViewDetails = new UITextView
            {
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                TextAlignment = UITextAlignment.Left
            };
            txtViewDetails.Delegate = new TextViewDelegate(new Action<NSUrl>((url) =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                TermsAndConditionViewController viewController =
                    storyBoard.InstantiateViewController("TermsAndConditionViewController") as TermsAndConditionViewController;
                if (viewController != null)
                {
                    viewController.isPresentedVC = true;
                    UINavigationController navController = new UINavigationController(viewController);
                    navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(navController, true, null);
                }
            }));

            //Resize
            CGSize size = txtViewDetails.SizeThatFits(new CGSize(36, GetScaledHeight(160)));
            txtViewDetails.Frame = new CGRect(18, 485, View.Frame.Width - 36, size.Height);

            btnRegisterContainer = new UIView(new CGRect(0, (View.Frame.Height - DeviceHelper.GetScaledHeight(145))
                , View.Frame.Width, DeviceHelper.GetScaledHeight(100)))
            {
                BackgroundColor = UIColor.White
            };
            View.AddSubview(btnRegisterContainer);

            //Register button
            btnRegister = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, DeviceHelper.GetScaledHeight(18), btnRegisterContainer.Frame.Width - 36, 48),
                BackgroundColor = MyTNBColor.FreshGreen,
                Font = MyTNBFont.MuseoSans16
            };
            btnRegister.SetTitle(GetI18NValue(RegisterConstants.I18N_CTATitle), UIControlState.Normal);
            btnRegister.Layer.CornerRadius = 5.0f;
            btnRegisterContainer.AddSubview(btnRegister);

            //Scrollview content size
            int addtlHeight = DeviceHelper.IsIphone5() ? 120 : 50;
            ScrollView.ContentSize = new CGRect(0f, 0f, View.Frame.Width, UIScreen.MainScreen.Bounds.Height + addtlHeight).Size;

            //ScrollView main subviews
            ScrollView.AddSubviews(new UIView[] {viewFullName,  viewICNumber,viewMobileNumber, viewEmail
                ,viewConfirmEmail, viewPassword , viewConfirmPassword, txtViewDetails });

            scrollViewFrame = ScrollView.Frame;
        }

        void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            bool visible = notification.Name == UIKeyboard.WillShowNotification;
            UIView.BeginAnimations("AnimateForKeyboard");
            UIView.SetAnimationBeginsFromCurrentState(true);
            UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
            UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

            if (visible)
            {
                CGRect r = UIKeyboard.BoundsFromNotification(notification);
                CGRect viewFrame = View.Bounds;
                nfloat currentViewHeight = viewFrame.Height - r.Height;
                ScrollView.Frame = new CGRect(ScrollView.Frame.X, ScrollView.Frame.Y, ScrollView.Frame.Width, currentViewHeight);
            }
            else
            {
                ScrollView.Frame = scrollViewFrame;
            }

            UIView.CommitAnimations();
        }

        private void SetViews()
        {
            _textFieldHelper.CreateTextFieldLeftView(txtFieldName, "Name");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldICNo, "IC");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldMobileNo, "Mobile");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldEmail, "Email");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldConfirmEmail, "Email");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldPassword, "Password");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldConfirmPassword, "Password");

            btnRegister.Enabled = false;
            btnRegister.BackgroundColor = MyTNBColor.SilverChalice;
        }

        private void SetVisibility()
        {
            lblNameTitle.Hidden = String.IsNullOrEmpty(txtFieldName.Text);
            lblICNoTitle.Hidden = String.IsNullOrEmpty(txtFieldICNo.Text);
            lblMobileNoTitle.Hidden = String.IsNullOrEmpty(txtFieldMobileNo.Text);
            lblEmailTitle.Hidden = String.IsNullOrEmpty(txtFieldEmail.Text);
            lblConfirmEmailTitle.Hidden = String.IsNullOrEmpty(txtFieldConfirmEmail.Text);
            lblPasswordTitle.Hidden = String.IsNullOrEmpty(txtFieldPassword.Text);
            lblConfirmPasswordTitle.Hidden = String.IsNullOrEmpty(txtFieldConfirmPassword.Text);

            lblNameError.Hidden = true;
            lblICNoError.Hidden = true;
            lblMobileNoError.Hidden = true;
            lblEmailError.Hidden = true;
            lblConfirmEmailError.Hidden = true;
            lblPasswordError.Hidden = true;
            lblConfirmPasswordError.Hidden = true;
        }

        private void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void SetEvents()
        {
            btnRegister.TouchUpInside += (sender, e) =>
            {
                ActivityIndicator.Show();
                _eMail = txtFieldEmail.Text;
                _password = txtFieldPassword.Text;
                _fullName = txtFieldName.Text?.Trim();
                _icNo = txtFieldICNo.Text;
                _mobileNo = _textFieldHelper.TrimAllSpaces(txtFieldMobileNo.Text);

                DataManager.DataManager.SharedInstance.User.Email = _eMail;
                DataManager.DataManager.SharedInstance.User.Password = _password;
                DataManager.DataManager.SharedInstance.User.MobileNo = _mobileNo;
                DataManager.DataManager.SharedInstance.User.DisplayName = _fullName;
                DataManager.DataManager.SharedInstance.User.ICNo = _icNo;

                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            ExecuteSendRegistrationTokenSMSCall();
                        }
                        else
                        {
                            DisplayNoDataAlert();
                            ActivityIndicator.Hide();
                        }
                    });
                });
            };

            SetTextFieldEvents(txtFieldName, lblNameTitle, lblNameError
                , viewLineName, lblNameHint, TNBGlobal.CustomerNamePattern);
            SetTextFieldEvents(txtFieldICNo, lblICNoTitle, lblICNoError
                , viewLineICNo, lblICNoHint, TNBGlobal.IC_NO_PATTERN);
            SetTextFieldEvents(txtFieldMobileNo, lblMobileNoTitle, lblMobileNoError
                , viewLineMobileNo, lblMobileNoHint, MOBILE_NO_PATTERN);
            SetTextFieldEvents(txtFieldEmail, lblEmailTitle, lblEmailError
                , viewLineEmail, lblEmailHint, EMAIL_PATTERN);
            SetTextFieldEvents(txtFieldConfirmEmail, lblConfirmEmailTitle, lblConfirmEmailError
                , viewLineConfirmEmail, lblConfirmEmailHint, EMAIL_PATTERN);
            SetTextFieldEvents(txtFieldPassword, lblPasswordTitle, lblPasswordError
                , viewLinePassword, lblPasswordHint, PASSWORD_PATTERN);
            SetTextFieldEvents(txtFieldConfirmPassword, lblConfirmPasswordTitle, lblConfirmPasswordError
                , viewLineConfirmPassword, lblConfirmPasswordHint, PASSWORD_PATTERN);
            _textFieldHelper.CreateDoneButton(txtFieldICNo);
            CreateDoneButton(txtFieldMobileNo);
        }

        void DisplayEyeIcon(UITextField textField)
        {
            if (textField == txtFieldPassword)
            {
                viewShowPassword.Hidden = textField.Text.Length == 0;
            }
            if (textField == txtFieldConfirmPassword)
            {
                viewShowConfirmPassword.Hidden = textField.Text.Length == 0;
            }
        }

        private void SetTextFieldEvents(UITextField textField, UILabel lblTitle
            , UILabel lblError, UIView viewLine, UILabel lblHint, string pattern)
        {
            if (lblHint == null)
            {
                lblHint = new UILabel();
            }
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                lblHint.Hidden = !lblError.Hidden || textField.Text.Length == 0;
                lblTitle.Hidden = textField.Text.Length == 0;
                DisplayEyeIcon(textField);
                SetRegisterButtonEnable();
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                if (textField == txtFieldMobileNo)
                {
                    if (textField.Text.Length == 0)
                    {
                        textField.Text += TNBGlobal.MobileNoPrefix;
                    }
                }
                lblHint.Hidden = !lblError.Hidden || textField.Text.Length == 0;
                lblTitle.Hidden = textField.Text.Length == 0;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                DisplayEyeIcon(textField);
                textField.LeftViewMode = UITextFieldViewMode.Never;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                //Handling for Confirm Email
                if (textField == txtFieldConfirmEmail)
                {
                    bool isMatch = txtFieldEmail.Text.Equals(txtFieldConfirmEmail.Text);
                    lblError.Text = isValid ? GetErrorI18NValue(Constants.Error_MismatchedEmail)
                        : GetErrorI18NValue(Constants.Error_InvalidEmailAddress);
                    isValid = isValid && isMatch;
                }
                //Handling for Confirm Password
                else if (textField == txtFieldConfirmPassword)
                {
                    bool isMatch = txtFieldPassword.Text.Equals(txtFieldConfirmPassword.Text);
                    lblError.Text = isValid ? GetErrorI18NValue(Constants.Error_MismatchedPassword)
                        : GetHintI18NValue(Constants.Hint_Password);
                    isValid = isValid && isMatch;
                }
                else if (textField == txtFieldMobileNo)
                {
                    if (textField.Text.Length < 4)
                    {
                        textField.Text = string.Empty;
                    }
                    isValid = isValid && _textFieldHelper.ValidateMobileNumberLength(textField.Text);
                }
                else if (textField == txtFieldName)
                {
                    isValid = isValid && !string.IsNullOrWhiteSpace(textField.Text);
                }
                DisplayEyeIcon(textField);
                lblError.Hidden = isValid;
                lblHint.Hidden = true;
                viewLine.BackgroundColor = isValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;

                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                if (textField == txtFieldMobileNo)
                {
                    if (textField.Text.Length < 4)
                    {
                        textField.Text = string.Empty;
                    }
                }
                sender.ResignFirstResponder();
                return false;
            };
            textField.ShouldChangeCharacters += (txtField, range, replacementString) =>
            {
                if (textField == txtFieldMobileNo)
                {
                    bool isCharValid = _textFieldHelper.ValidateTextField(replacementString, TNBGlobal.MobileNoPattern);
                    if (!isCharValid)
                    {
                        return false;
                    }

                    if (range.Location >= TNBGlobal.MobileNoPrefix.Length)
                    {
                        string content = _textFieldHelper.TrimAllSpaces(((UITextField)txtField).Text);
                        nint count = content.Length + replacementString.Length - range.Length;
                        return count <= TNBGlobal.MobileNumberMaxCharCount;
                    }
                    return false;
                }
                else if (textField == txtFieldName)
                {
                    bool isCharValid = string.IsNullOrEmpty(replacementString)
                        || _textFieldHelper.ValidateTextField(replacementString, pattern);
                    if (!isCharValid)
                    {
                        return false;
                    }
                }
                return true;
            };
            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
            };
        }

        private void SetRegisterButtonEnable()
        {
            bool isValidName = _textFieldHelper.ValidateTextField(txtFieldName.Text, TNBGlobal.CustomerNamePattern)
                && !string.IsNullOrWhiteSpace(txtFieldName.Text);
            bool isValidICNo = _textFieldHelper.ValidateTextField(txtFieldICNo.Text, TNBGlobal.IC_NO_PATTERN);
            bool isValidMobileNo = _textFieldHelper.ValidateTextField(txtFieldMobileNo.Text, MOBILE_NO_PATTERN)
                && _textFieldHelper.ValidateMobileNumberLength(txtFieldMobileNo.Text);
            bool isValidEmail = _textFieldHelper.ValidateTextField(txtFieldEmail.Text, EMAIL_PATTERN)
                && _textFieldHelper.ValidateTextField(txtFieldConfirmEmail.Text, EMAIL_PATTERN)
                && txtFieldEmail.Text.Equals(txtFieldConfirmEmail.Text);
            bool isValidPassword = _textFieldHelper.ValidateTextField(txtFieldPassword.Text, PASSWORD_PATTERN)
                && _textFieldHelper.ValidateTextField(txtFieldConfirmPassword.Text, PASSWORD_PATTERN)
                && txtFieldPassword.Text.Equals(txtFieldConfirmPassword.Text);
            bool isValid = isValidName
                && isValidICNo
                && isValidMobileNo
                && isValidEmail
                && isValidPassword;
            btnRegister.Enabled = isValid;
            btnRegister.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }

        private void ExecuteSendRegistrationTokenSMSCall()
        {
            SendRegistrationTokenSMS().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_smsToken != null && _smsToken.d != null)
                    {
                        if (_smsToken.d.isError.Equals("false") && _smsToken.d.status.Equals("success"))
                        {
                            UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                            VerifyPinViewController viewController = storyBoard.InstantiateViewController("VerifyPinViewController") as VerifyPinViewController;
                            NavigationController.PushViewController(viewController, true);
                        }
                        else
                        {
                            DisplayServiceError(_smsToken?.d?.message ?? string.Empty);
                        }
                    }
                    else
                    {
                        DisplayServiceError(_smsToken?.d?.message ?? string.Empty);
                    }
                    ActivityIndicator.Hide();
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
                    username = _eMail,
                    userEmail = _eMail,
                    mobileNo = _mobileNo
                };
                _smsToken = serviceManager.OnExecuteAPI<RegistrationTokenSMSResponseModel>(
                    RegisterConstants.Service_SendRegistrationTokenSMS, requestParameter);
            });
        }

        private void CreateDoneButton(UITextField textField)
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            UIBarButtonItem doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                if (textField == txtFieldMobileNo)
                {
                    if (textField.Text.Length < 4)
                    {
                        textField.Text = string.Empty;
                    }
                }
                textField.ResignFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            textField.InputAccessoryView = toolbar;
        }

        private UILabel GetTitleLabel(string key)
        {
            return new UILabel
            {
                Frame = new CGRect(0, 0, View.Frame.Width - 36, 12),
                AttributedText = AttributedStringUtility.GetAttributedString(key
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left
            };
        }

        private UILabel GetErrorLabel(string key)
        {
            return new UILabel
            {
                Frame = new CGRect(0, 37, View.Frame.Width - 36, 14),
                AttributedText = AttributedStringUtility.GetAttributedString(key
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left
            };
        }
        private UITextField GetUITextField(string key)
        {
            return new UITextField
            {
                Frame = new CGRect(0, 12, View.Frame.Width - 36, 24),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(key
                        , AttributedStringUtility.AttributedStringType.Value),
                TextColor = MyTNBColor.TunaGrey()
            };
        }
    }
}