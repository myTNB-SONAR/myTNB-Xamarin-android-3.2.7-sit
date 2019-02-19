using System;
using System.Threading.Tasks;
using myTNB.Model;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Drawing;
using myTNB.Extensions;

namespace myTNB.Registration
{
    public partial class RegistrationViewController : UIViewController
    {
        UITextField txtFieldName;
        UITextField txtFieldICNo;
        UITextField txtFieldMobileNo;
        UITextField txtFieldEmail;
        UITextField txtFieldConfirmEmail;
        UITextField txtFieldPassword;
        UITextField txtFieldConfirmPassword;
        UITextView txtViewDetails;
        UIButton btnRegister;
        UIView btnRegisterContainer;

        UIView viewLineName;
        UIView viewLineICNo;
        UIView viewLineMobileNo;
        UIView viewLineEmail;
        UIView viewLineConfirmEmail;
        UIView viewLinePassword;
        UIView viewLineConfirmPassword;

        UILabel lblNameTitle;
        UILabel lblICNoTitle;
        UILabel lblMobileNoTitle;
        UILabel lblEmailTitle;
        UILabel lblConfirmEmailTitle;
        UILabel lblPasswordTitle;
        UILabel lblConfirmPasswordTitle;

        UILabel lblNameError;
        UILabel lblICNoError;
        UILabel lblMobileNoError;
        UILabel lblEmailError;
        UILabel lblConfirmEmailError;
        UILabel lblPasswordError;
        UILabel lblConfirmPasswordError;

        UILabel lblNameHint;
        UILabel lblICNoHint;
        UILabel lblMobileNoHint;
        UILabel lblEmailHint;
        UILabel lblConfirmEmailHint;
        UILabel lblPasswordHint;
        UILabel lblConfirmPasswordHint;

        UIView viewShowConfirmPassword;
        UIView viewShowPassword;
        UIScrollView ScrollView;
        CGRect scrollViewFrame;
        public RegistrationViewController(IntPtr handle) : base(handle)
        {
        }
        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        RegistrationTokenSMSResponseModel _smsToken = new RegistrationTokenSMSResponseModel();

        string _eMail = string.Empty;
        string _token = string.Empty;
        string _password = string.Empty;
        string _fullName = string.Empty;
        string _icNo = string.Empty;
        string _mobileNo = string.Empty;

        const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        const string PASSWORD_PATTERN = @"^.{8,}$"; //@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8,})$";
        const string MOBILE_NO_PATTERN = @"^[0-9 \+]+$";
        const string IC_NO_PATTERN = @"^[a-zA-Z0-9]+$";
        const string TOKEN_PATTERN = @"^[0-9]{4,4}$";

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            this.NavigationItem.HidesBackButton = true;
            InitializedSubviews();
            AddBackButton();
            SetVisibility();
            SetEvents();
            SetViews();
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
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

        internal void InitializedSubviews()
        {

            //Scrollview
            ScrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
            ScrollView.BackgroundColor = UIColor.Clear;
            View.AddSubview(ScrollView);

            //FullName 
            UIView viewFullName = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 51)));
            viewFullName.BackgroundColor = UIColor.Clear;

            lblNameTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewFullName.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    "FULL NAME"
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewFullName.AddSubview(lblNameTitle);

            lblNameError = new UILabel
            {
                Frame = new CGRect(0, 37, viewFullName.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "Invalid full name"
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.Tomato()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewFullName.AddSubview(lblNameError);

            txtFieldName = new UITextField
            {
                Frame = new CGRect(0, 12, viewFullName.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Full Name"
                    , font: myTNBFont.MuseoSans16()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            viewFullName.AddSubview(txtFieldName);

            viewLineName = new UIView((new CGRect(0, 36, viewFullName.Frame.Width, 1)));
            viewLineName.BackgroundColor = myTNBColor.PlatinumGrey();
            viewFullName.AddSubview(viewLineName);

            //IC Number
            UIView viewICNumber = new UIView((new CGRect(18, 83, View.Frame.Width - 36, 51)));
            viewICNumber.BackgroundColor = UIColor.Clear;

            lblICNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewICNumber.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    "IC, ROC or PASSPORT NO."
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewICNumber.AddSubview(lblICNoTitle);

            lblICNoError = new UILabel
            {
                Frame = new CGRect(0, 37, viewICNumber.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "Invalid IC, ROC or Passport No."
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.Tomato()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewICNumber.AddSubview(lblICNoError);

            /*lblICNoHint = new UILabel
            {
                Frame = new CGRect(0, 37, viewICNumber.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "IC, ROC or Passport No.",
                    font: myTNBFont.MuseoSans9(),
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
                AttributedPlaceholder = new NSAttributedString(
                    "IC, ROC or Passport No."
                    , font: myTNBFont.MuseoSans16()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            viewICNumber.AddSubview(txtFieldICNo);

            viewLineICNo = new UIView((new CGRect(0, 36, viewICNumber.Frame.Width, 1)));
            viewLineICNo.BackgroundColor = myTNBColor.PlatinumGrey();
            viewICNumber.AddSubview(viewLineICNo);

            //Mobile Number
            UIView viewMobileNumber = new UIView((new CGRect(18, 150, View.Frame.Width - 36, 51)));
            viewMobileNumber.BackgroundColor = UIColor.Clear;

            lblMobileNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewMobileNumber.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    "MOBILE NO."
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewMobileNumber.AddSubview(lblMobileNoTitle);

            lblMobileNoError = new UILabel
            {
                Frame = new CGRect(0, 37, viewMobileNumber.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "Invalid mobile no."
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.Tomato()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewMobileNumber.AddSubview(lblMobileNoError);

            lblMobileNoHint = new UILabel
            {
                Frame = new CGRect(0, 37, viewMobileNumber.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "Please include the country code of your phone number.",
                    font: myTNBFont.MuseoSans9(),
                    foregroundColor: myTNBColor.TunaGrey(),
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            lblMobileNoHint.Hidden = true;
            viewMobileNumber.AddSubview(lblMobileNoHint);

            txtFieldMobileNo = new UITextField
            {
                Frame = new CGRect(0, 12, viewMobileNumber.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Mobile No."
                    , font: myTNBFont.MuseoSans16()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            viewMobileNumber.AddSubview(txtFieldMobileNo);

            viewLineMobileNo = new UIView((new CGRect(0, 36, viewMobileNumber.Frame.Width, 1)));
            viewLineMobileNo.BackgroundColor = myTNBColor.PlatinumGrey();
            viewMobileNumber.AddSubview(viewLineMobileNo);

            //Eeeend
            UIView viewEmail = new UIView((new CGRect(18, 217, View.Frame.Width - 36, 51)));
            viewEmail.BackgroundColor = UIColor.Clear;

            lblEmailTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewEmail.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    "EMAIL"
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewEmail.AddSubview(lblEmailTitle);

            lblEmailError = new UILabel
            {
                Frame = new CGRect(0, 37, viewEmail.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "Invalid email address."
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.Tomato()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };

            viewEmail.AddSubview(lblEmailError);

            txtFieldEmail = new UITextField
            {
                Frame = new CGRect(0, 12, viewEmail.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Email"
                    , font: myTNBFont.MuseoSans16()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            viewEmail.AddSubview(txtFieldEmail);

            viewLineEmail = new UIView((new CGRect(0, 36, viewEmail.Frame.Width, 1)));
            viewLineEmail.BackgroundColor = myTNBColor.PlatinumGrey();
            viewEmail.AddSubview(viewLineEmail);

            //Confirm Email
            UIView viewConfirmEmail = new UIView((new CGRect(18, 284, View.Frame.Width - 36, 51)));
            viewConfirmEmail.BackgroundColor = UIColor.Clear;

            lblConfirmEmailTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewConfirmEmail.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    "CONFIRM EMAIL"
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewConfirmEmail.AddSubview(lblConfirmEmailTitle);

            lblConfirmEmailError = new UILabel(new CGRect(0, 37, viewConfirmEmail.Frame.Width, 14));
            lblConfirmEmailError.Font = myTNBFont.MuseoSans9();
            lblConfirmEmailError.TextAlignment = UITextAlignment.Left;
            lblConfirmEmailError.TextColor = myTNBColor.Tomato();
            viewConfirmEmail.AddSubview(lblConfirmEmailError);

            txtFieldConfirmEmail = new UITextField
            {
                Frame = new CGRect(0, 12, viewConfirmEmail.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Confirm Email"
                    , font: myTNBFont.MuseoSans16()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            viewConfirmEmail.AddSubview(txtFieldConfirmEmail);

            viewLineConfirmEmail = new UIView((new CGRect(0, 36, viewConfirmEmail.Frame.Width, 1)));
            viewLineConfirmEmail.BackgroundColor = myTNBColor.PlatinumGrey();
            viewConfirmEmail.AddSubview(viewLineConfirmEmail);

            //Password
            UIView viewPassword = new UIView((new CGRect(18, 351, View.Frame.Width - 36, 51)));
            viewPassword.BackgroundColor = UIColor.Clear;

            lblPasswordTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewPassword.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    "PASSWORD"
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewPassword.AddSubview(lblPasswordTitle);

            lblPasswordError = new UILabel
            {
                Frame = new CGRect(0, 37, viewPassword.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "Password must have at least 8 alphanumeric characters."
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.Tomato()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewPassword.AddSubview(lblPasswordError);

            lblPasswordHint = new UILabel
            {
                Frame = new CGRect(0, 37, viewPassword.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "Password must have at least 8 alphanumeric characters."
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.TunaGrey()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            lblPasswordHint.Hidden = true;
            viewPassword.AddSubview(lblPasswordHint);

            txtFieldPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Password"
                    , font: myTNBFont.MuseoSans16()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            txtFieldPassword.SecureTextEntry = true;
            viewPassword.AddSubview(txtFieldPassword);

            viewShowPassword = new UIView(new CGRect(viewPassword.Frame.Width - 30, 12, 24, 24));
            viewShowPassword.Hidden = true;
            UIImageView imgShowPassword = new UIImageView(new CGRect(0, 0, 24, 24));
            imgShowPassword.Image = UIImage.FromBundle("IC-Action-Show-Password");
            viewShowPassword.AddSubview(imgShowPassword);
            viewShowPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (txtFieldPassword.SecureTextEntry)
                {
                    txtFieldPassword.SecureTextEntry = false;
                    imgShowPassword.Image = UIImage.FromBundle("IC-Action-Hide-Password");
                }
                else
                {
                    txtFieldPassword.SecureTextEntry = true;
                    imgShowPassword.Image = UIImage.FromBundle("IC-Action-Show-Password");
                }
            }));
            viewPassword.AddSubview(viewShowPassword);

            viewLinePassword = new UIView((new CGRect(0, 36, viewPassword.Frame.Width, 1)));
            viewLinePassword.BackgroundColor = myTNBColor.PlatinumGrey();
            viewPassword.AddSubview(viewLinePassword);

            //Confirm Password
            UIView viewConfirmPassword = new UIView((new CGRect(18, 418, View.Frame.Width - 36, 51)));
            viewConfirmPassword.BackgroundColor = UIColor.Clear;

            lblConfirmPasswordTitle = new UILabel
            {
                Frame = new CGRect(0, 0, viewConfirmPassword.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                    "CONFIRM PASSWORD"
                    , font: myTNBFont.MuseoSans9()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            viewConfirmPassword.AddSubview(lblConfirmPasswordTitle);

            lblConfirmPasswordError = new UILabel(new CGRect(0, 37, viewConfirmEmail.Frame.Width, 14));
            lblConfirmPasswordError.Font = myTNBFont.MuseoSans9();
            lblConfirmPasswordError.TextAlignment = UITextAlignment.Left;
            lblConfirmPasswordError.TextColor = myTNBColor.Tomato();

            viewConfirmPassword.AddSubview(lblConfirmPasswordError);

            txtFieldConfirmPassword = new UITextField
            {
                Frame = new CGRect(0, 12, viewConfirmPassword.Frame.Width - 30, 24),
                AttributedPlaceholder = new NSAttributedString(
                    "Confirm Password"
                    , font: myTNBFont.MuseoSans16()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0
                ),
                TextColor = myTNBColor.TunaGrey()
            };
            txtFieldConfirmPassword.SecureTextEntry = true;
            viewConfirmPassword.AddSubview(txtFieldConfirmPassword);

            viewShowConfirmPassword = new UIView(new CGRect(viewConfirmPassword.Frame.Width - 30, 12, 24, 24));
            viewShowConfirmPassword.Hidden = true;
            UIImageView imgShowConfirmPassword = new UIImageView(new CGRect(0, 0, 24, 24));
            imgShowConfirmPassword.Image = UIImage.FromBundle("IC-Action-Show-Password");
            viewShowConfirmPassword.AddSubview(imgShowConfirmPassword);
            viewConfirmPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (txtFieldConfirmPassword.SecureTextEntry)
                {
                    txtFieldConfirmPassword.SecureTextEntry = false;
                    imgShowConfirmPassword.Image = UIImage.FromBundle("IC-Action-Hide-Password");
                }
                else
                {
                    txtFieldConfirmPassword.SecureTextEntry = true;
                    imgShowConfirmPassword.Image = UIImage.FromBundle("IC-Action-Show-Password");
                }
            }));
            viewConfirmPassword.AddSubview(viewShowConfirmPassword);

            viewLineConfirmPassword = new UIView((new CGRect(0, 36, viewConfirmPassword.Frame.Width, 1)));
            viewLineConfirmPassword.BackgroundColor = myTNBColor.PlatinumGrey();
            viewConfirmPassword.AddSubview(viewLineConfirmPassword);

            //Set keyboard types
            txtFieldICNo.KeyboardType = UIKeyboardType.Default;
            txtFieldMobileNo.KeyboardType = UIKeyboardType.NumberPad;
            txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;
            txtFieldConfirmEmail.KeyboardType = UIKeyboardType.EmailAddress;

            //Terms Details
            txtViewDetails = new UITextView(new CGRect(18, 485, View.Frame.Width - 36, 51));
            var attributedString = new NSMutableAttributedString("By registering, you are agreeing to the TNB Terms and Conditions, User Agreement and Privacy policy.");
            var firstAttributes = new UIStringAttributes
            {
                ForegroundColor = myTNBColor.TunaGrey(),
                Font = myTNBFont.MuseoSans12()
            };
            var secondAttributes = new UIStringAttributes
            {
                ForegroundColor = myTNBColor.PowerBlue(),
                Font = myTNBFont.MuseoSans12()
            };
            attributedString.SetAttributes(firstAttributes.Dictionary, new NSRange(0, 39));
            attributedString.SetAttributes(secondAttributes.Dictionary, new NSRange(40, 60));

            txtViewDetails.AttributedText = attributedString;
            txtViewDetails.Editable = false;
            txtViewDetails.Selectable = false;
            txtViewDetails.ScrollEnabled = false;

            UITapGestureRecognizer tap = new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                TermsAndConditionViewController viewController =
                    storyBoard.InstantiateViewController("TermsAndConditionViewController") as TermsAndConditionViewController;
                viewController.isPresentedVC = false;
                NavigationController.PushViewController(viewController, true);
            });
            txtViewDetails.AddGestureRecognizer(tap);

            btnRegisterContainer = new UIView(new CGRect(0, (View.Frame.Height - DeviceHelper.GetScaledHeight(145)), View.Frame.Width, DeviceHelper.GetScaledHeight(100)));
            btnRegisterContainer.BackgroundColor = UIColor.White;
            View.AddSubview(btnRegisterContainer);

            //Register button
            btnRegister = new UIButton(UIButtonType.Custom);
            btnRegister.Frame = new CGRect(18, DeviceHelper.GetScaledHeight(18), btnRegisterContainer.Frame.Width - 36, 48);
            btnRegister.SetTitle("Register", UIControlState.Normal);
            btnRegister.Font = myTNBFont.MuseoSans16();
            btnRegister.Layer.CornerRadius = 5.0f;
            btnRegister.BackgroundColor = myTNBColor.FreshGreen();
            btnRegisterContainer.AddSubview(btnRegister);

            //Scrollview content size
            var addtlHeight = DeviceHelper.IsIphone5() ? 120 : 20;
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

        internal void SetViews()
        {
            _textFieldHelper.CreateTextFieldLeftView(txtFieldName, "Name");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldICNo, "IC");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldMobileNo, "Mobile");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldEmail, "Email");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldConfirmEmail, "Email");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldPassword, "Password");
            _textFieldHelper.CreateTextFieldLeftView(txtFieldConfirmPassword, "Password");

            btnRegister.Enabled = false;
            btnRegister.BackgroundColor = myTNBColor.SilverChalice();
        }

        internal void SetVisibility()
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

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                this.DismissViewController(true, null);
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal void SetEvents()
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
                            DisplayAlertView("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate());
                            ActivityIndicator.Hide();
                        }
                    });
                });

            };

            SetTextFieldEvents(txtFieldName, lblNameTitle, lblNameError
                               , viewLineName, lblNameHint, TNBGlobal.CustomerNamePattern);
            SetTextFieldEvents(txtFieldICNo, lblICNoTitle, lblICNoError
                               , viewLineICNo, lblICNoHint, IC_NO_PATTERN);
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

        internal void SetTextFieldEvents(UITextField textField, UILabel lblTitle
                                         , UILabel lblError, UIView viewLine
                                         , UILabel lblHint, string pattern)
        {
            if (lblHint == null)
            {
                lblHint = new UILabel();
            }
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                lblHint.Hidden = lblError.Hidden ? textField.Text.Length == 0 : true;
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
                lblHint.Hidden = lblError.Hidden ? textField.Text.Length == 0 : true;
                lblTitle.Hidden = textField.Text.Length == 0;
                viewLine.BackgroundColor = myTNBColor.PowerBlue();
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
                    string err = isValid ? "Your email and confirmation email do not match."
                        : "Invalid email address.";
                    lblError.Text = err;
                    isValid = isValid && isMatch;
                }
                //Handling for Confirm Password
                else if (textField == txtFieldConfirmPassword)
                {
                    bool isMatch = txtFieldPassword.Text.Equals(txtFieldConfirmPassword.Text);
                    string err = isValid ? "Your password and confirmation password do not match."
                        : "Password must have at least 8 alphanumeric characters.";
                    lblError.Text = err;
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
                viewLine.BackgroundColor = isValid ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                textField.TextColor = isValid ? myTNBColor.TunaGrey() : myTNBColor.Tomato();

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
                        var count = content.Length + replacementString.Length - range.Length;
                        return count <= TNBGlobal.MobileNumberMaxCharCount;
                    }
                    return false;
                }
                else if (textField == txtFieldName)
                {
                    bool isCharValid = !string.IsNullOrEmpty(replacementString)
                                              ? _textFieldHelper.ValidateTextField(replacementString, pattern)
                                              : true;
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

        internal void SetRegisterButtonEnable()
        {
            bool isValidName = _textFieldHelper.ValidateTextField(txtFieldName.Text, TNBGlobal.CustomerNamePattern)
                && !string.IsNullOrWhiteSpace(txtFieldName.Text);
            bool isValidICNo = _textFieldHelper.ValidateTextField(txtFieldICNo.Text, IC_NO_PATTERN);
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
            btnRegister.BackgroundColor = isValid ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        internal void ExecuteSendRegistrationTokenSMSCall()
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
                            this.NavigationController.PushViewController(viewController, true);
                            ActivityIndicator.Hide();
                        }
                        else
                        {
                            DisplayAlertView("Registration Token Failed", _smsToken.d.message);
                        }
                    }
                    else
                    {
                        DisplayAlertView("Registration Token Failed", "Error in response.");
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        internal Task SendRegistrationTokenSMS()
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
                _smsToken = serviceManager.SendRegistrationTokenSMS("SendRegistrationTokenSMS_V2", requestParameter);
            });
        }

        internal void DisplayAlertView(string title, string message)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, animated: true, completionHandler: null);
        }

        void CreateDoneButton(UITextField textField)
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
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
    }
}