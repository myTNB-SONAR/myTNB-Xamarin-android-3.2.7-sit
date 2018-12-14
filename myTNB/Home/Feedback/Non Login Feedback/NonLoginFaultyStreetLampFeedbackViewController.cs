using Foundation;
using System;
using UIKit;
using CoreGraphics;
using myTNB.Model;
using System.Threading.Tasks;
using CoreLocation;
using Location;
using myTNB.Home.Feedback;
using System.Collections.Generic;
using System.Drawing;

namespace myTNB
{
    public partial class NonLoginFaultyStreetLampFeedbackViewController : UIViewController
    {
        UIView _viewFullName;
        UILabel _lblFullNameTitle;
        UILabel _lblFullNameError;
        UITextField _txtFieldFullName;
        UIView _viewLineFullName;

        UIView _viewMobileNo;
        UILabel _lblMobileNoTitle;
        UILabel _lblMobileNoError;
        UITextField _txtFieldMobileNo;
        UIView _viewLineMobile;

        UIView _viewEmail;
        UILabel _lblEmailTitle;
        UILabel _lblEmailError;
        UITextField _txtFieldEmail;
        UIView _viewLineEmail;

        UIView _viewState;
        UILabel _lblStateTitle;
        UILabel _lblStateError;
        UILabel _lblState;
        UIView _viewLineState;

        UIView _viewLocation;
        UILabel _lblLocationTitle;
        UILabel _lblLocationError;
        UITextField _txtFieldLocation;
        UIView _viewLineLocation;

        UIView _viewPole;
        UILabel _lblPoleTitle;
        UILabel _lblPoleError;
        UITextField _txtFieldPole;
        UIView _viewLinePole;

        UIView _viewFeedback;
        UILabel _lblFeedbackTitle;
        UILabel _lblFeedbackSubTitle;
        UILabel _lblFeedbackError;
        UITextField _txtFieldFeedback;
        UIView _viewLineFeedback;

        UILabel lblMobileNoHint;

        UIButton _btnSubmit;

        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        SubmitFeedbackResponseModel _submitFeedback = new SubmitFeedbackResponseModel();
        public static LocationManager _locManager { get; set; }

        List<ImageDataModel> _capturedImageList = new List<ImageDataModel>();
        UIImageHelper _imageHelper = new UIImageHelper();

        const string ANY_PATTERN = @".*";
        const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        const string NAME_PATTERN = @"^[A-Za-z0-9 _]*[A-Za-z0-9][A-Za-z0-9 \-\\_ _]*$";
        const string MOBILE_NO_PATTERN = @"^[0-9 \+]+$";
        const string IC_NO_PATTERN = @"^[a-zA-Z0-9]+$";

        string _feedbackMessage = string.Empty;
        string _stateID = string.Empty;
        string _location = string.Empty;
        string _poleNumber = string.Empty;
        string _fullName = string.Empty;
        string _mobileNo = string.Empty;
        string _email = string.Empty;

        UIScrollView ScrollView;
        UIScrollView imageContainerScroll;
        int imageWidth = 0;
        nfloat imgViewFaultyLampHeight;
        int capturedImageCount = 0;
        int imageCount = 0;
        const int MAX_IMAGE = 2;

        public NonLoginFaultyStreetLampFeedbackViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddBackButton();
            InitializedSubviews();
            SetVisibility();
            SetEvents();
            AddImageContainer();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (DataManager.DataManager.SharedInstance.StatesForFeedBack != null
                && DataManager.DataManager.SharedInstance.StatesForFeedBack.d != null
                && DataManager.DataManager.SharedInstance.StatesForFeedBack.d.data != null
                && DataManager.DataManager.SharedInstance.StatesForFeedBack.d.data.Count > 0)
            {
                _lblState.Text = DataManager.DataManager.SharedInstance.StatesForFeedBack.d.data[DataManager.DataManager.SharedInstance.CurrentSelectedStateForFeedbackIndex].StateName;
            }
        }

        internal void InitializedSubviews()
        {
            //Scrollview
            ScrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
            ScrollView.BackgroundColor = UIColor.Clear;
            View.AddSubview(ScrollView);

            //Photo Header
            imgViewFaultyLampHeight = (View.Frame.Width * 121) / 320;
            var imgViewFaultyLamp = new UIImageView(new CGRect(0, 0, View.Frame.Width, imgViewFaultyLampHeight));
            imgViewFaultyLamp.Image = UIImage.FromBundle("Faulty-TNB-Lamp");
            ScrollView.AddSubview(imgViewFaultyLamp);

            //Label
            var lblReport = new UILabel(new CGRect(18, 23 + imgViewFaultyLampHeight, View.Frame.Width - 36, 18));
            lblReport.Text = "Reporting faulty street lamps?";
            lblReport.TextColor = myTNBColor.PowerBlue();
            lblReport.Font = myTNBFont.MuseoSans16();
            ScrollView.AddSubview(lblReport);

            //Terms Details
            var txtViewDetails = new UITextView(new CGRect(18, 47 + imgViewFaultyLampHeight, View.Frame.Width - 36, 78));
            var attributedString = new NSMutableAttributedString("You may only report faulty TNB street lamps. Look out for TNB’s blue and red or the logo as shown above. Use the pole no. (e.g. 17493 8E 1) to specify the lamp.");
            var firstAttributes = new UIStringAttributes
            {
                ForegroundColor = myTNBColor.TunaGrey(),
                Font = myTNBFont.MuseoSans14()
            };
            var secondAttributes = new UIStringAttributes
            {
                ForegroundColor = myTNBColor.TunaGrey(),
                Font = UIFont.BoldSystemFontOfSize(14)

            };
            attributedString.SetAttributes(firstAttributes.Dictionary, new NSRange(0, 159));
            attributedString.SetAttributes(secondAttributes.Dictionary, new NSRange(128, 10));

            txtViewDetails.AttributedText = attributedString;
            txtViewDetails.Editable = false;
            txtViewDetails.Selectable = false;
            txtViewDetails.ScrollEnabled = false;
            txtViewDetails.TextContainerInset = new UIEdgeInsets(0, -4, 0, 0);

            ScrollView.AddSubview(txtViewDetails);

            //FullName
            _viewFullName = new UIView((new CGRect(18, 135 + imgViewFaultyLampHeight, View.Frame.Width - 36, 51)));
            _viewFullName.BackgroundColor = UIColor.Clear;

            _lblFullNameTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFullName.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "FULLNAME",
                                                       font: myTNBFont.MuseoSans9(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            _viewFullName.AddSubview(_lblFullNameTitle);

            _lblFullNameError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewFullName.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                        "Invalid fullname",
                                           font: myTNBFont.MuseoSans9(),
                                        foregroundColor: myTNBColor.Tomato(),
                                        strokeWidth: 0
                                       ),
                TextAlignment = UITextAlignment.Left
            };
            _viewFullName.AddSubview(_lblFullNameError);

            _txtFieldFullName = new UITextField
            {
                Frame = new CGRect(0, 12, _viewFullName.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                                                     "Full name",
                                                       font: myTNBFont.MuseoSans16(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            _viewFullName.AddSubview(_txtFieldFullName);

            _viewLineFullName = new UIView((new CGRect(0, 36, _viewFullName.Frame.Width, 1)));
            _viewLineFullName.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewFullName.AddSubview(_viewLineFullName);


            //Mobile No.
            _viewMobileNo = new UIView((new CGRect(18, 202 + imgViewFaultyLampHeight, View.Frame.Width - 36, 51)));
            _viewMobileNo.BackgroundColor = UIColor.Clear;

            _lblMobileNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewMobileNo.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "MOBILE NO.",
                                                       font: myTNBFont.MuseoSans9(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            _viewMobileNo.AddSubview(_lblMobileNoTitle);

            _lblMobileNoError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewMobileNo.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                        "Invalid mobile no.",
                                           font: myTNBFont.MuseoSans9(),
                                        foregroundColor: myTNBColor.Tomato(),
                                        strokeWidth: 0
                                       ),
                TextAlignment = UITextAlignment.Left
            };
            _viewMobileNo.AddSubview(_lblMobileNoError);

            lblMobileNoHint = new UILabel
            {
                Frame = new CGRect(0, 37, _viewMobileNo.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                    "Please include the country code of your phone number.",
                    font: myTNBFont.MuseoSans9(),
                    foregroundColor: myTNBColor.TunaGrey(),
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            lblMobileNoHint.Hidden = true;
            _viewMobileNo.AddSubview(lblMobileNoHint);

            _txtFieldMobileNo = new UITextField
            {
                Frame = new CGRect(0, 12, _viewMobileNo.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                                                     "Mobile no.",
                                                       font: myTNBFont.MuseoSans16(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            _viewMobileNo.AddSubview(_txtFieldMobileNo);

            _viewLineMobile = new UIView((new CGRect(0, 36, _viewMobileNo.Frame.Width, 1)));
            _viewLineMobile.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewMobileNo.AddSubview(_viewLineMobile);

            //Email
            _viewEmail = new UIView((new CGRect(18, 269 + imgViewFaultyLampHeight, View.Frame.Width - 36, 51)));
            _viewEmail.BackgroundColor = UIColor.Clear;

            _lblEmailTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewEmail.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "EMAIL",
                                                       font: myTNBFont.MuseoSans9(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            _viewEmail.AddSubview(_lblEmailTitle);

            _lblEmailError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewEmail.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                        "Invalid email address",
                                           font: myTNBFont.MuseoSans9(),
                                        foregroundColor: myTNBColor.Tomato(),
                                        strokeWidth: 0
                                       ),
                TextAlignment = UITextAlignment.Left
            };
            _viewEmail.AddSubview(_lblEmailError);

            _txtFieldEmail = new UITextField
            {
                Frame = new CGRect(0, 12, _viewEmail.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                                                     "Email",
                                                       font: myTNBFont.MuseoSans16(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            _viewEmail.AddSubview(_txtFieldEmail);

            _viewLineEmail = new UIView((new CGRect(0, 36, _viewEmail.Frame.Width, 1)));
            _viewLineEmail.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewEmail.AddSubview(_viewLineEmail);

            //State 
            _viewState = new UIView((new CGRect(18, 336 + imgViewFaultyLampHeight, View.Frame.Width - 36, 51)));
            _viewState.BackgroundColor = UIColor.Clear;

            _lblStateTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewState.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "STATE",
                                                       font: myTNBFont.MuseoSans9(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            _viewState.AddSubview(_lblStateTitle);

            _lblStateError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewState.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                            "State",
                                               font: myTNBFont.MuseoSans9(),
                                            foregroundColor: myTNBColor.Tomato(),
                                            strokeWidth: 0
                                           ),
                TextAlignment = UITextAlignment.Left
            };
            _viewState.AddSubview(_lblStateError);


            UIImageView imgViewState = new UIImageView(new CGRect(0, 12, 24, 24));
            imgViewState.Image = UIImage.FromBundle("IC-FieldCoordinates");
            _viewState.AddSubview(imgViewState);

            _lblState = new UILabel(new CGRect(30, 12, _viewState.Frame.Width, 24));
            _lblState.AttributedText = new NSAttributedString(
                                            "State",
                                               font: myTNBFont.MuseoSans16(),
                                            foregroundColor: myTNBColor.SilverChalice(),
                                            strokeWidth: 0
            );

            _viewState.AddSubview(_lblState);

            UIImageView imgDropDown = new UIImageView(new CGRect(_viewState.Frame.Width - 30, 12, 24, 24));
            imgDropDown.Image = UIImage.FromBundle("IC-Action-Dropdown");
            _viewState.AddSubview(imgDropDown);

            _viewLineState = new UIView((new CGRect(0, 36, _viewState.Frame.Width, 1)));
            _viewLineState.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewState.AddSubview(_viewLineState);

            UITapGestureRecognizer tapState = new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("FeedbackTableView", null);
                SelectStateViewController selectStateVC =
                    storyBoard.InstantiateViewController("SelectStateViewController") as SelectStateViewController;
                selectStateVC._statesForFeedbackList = DataManager.DataManager.SharedInstance.StatesForFeedBack.d.data;
                NavigationController.PushViewController(selectStateVC, true);

            });
            _viewState.AddGestureRecognizer(tapState);

            //Location Street/Name

            _viewLocation = new UIView((new CGRect(18, 403 + imgViewFaultyLampHeight, View.Frame.Width - 36, 51)));
            _viewLocation.BackgroundColor = UIColor.Clear;

            _lblLocationTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewLocation.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "LOCATION / STREET NAME",
                                                       font: myTNBFont.MuseoSans9(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            _viewLocation.AddSubview(_lblLocationTitle);

            _lblLocationError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewLocation.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                        "Invalid location / Street name",
                                           font: myTNBFont.MuseoSans9(),
                                        foregroundColor: myTNBColor.Tomato(),
                                        strokeWidth: 0
                                       ),
                TextAlignment = UITextAlignment.Left
            };
            _viewLocation.AddSubview(_lblLocationError);

            _txtFieldLocation = new UITextField
            {
                Frame = new CGRect(0, 12, _viewLocation.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                                                     "Location / Street name",
                                                       font: myTNBFont.MuseoSans16(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            _viewLocation.AddSubview(_txtFieldLocation);

            _viewLineLocation = new UIView((new CGRect(0, 36, _viewLocation.Frame.Width, 1)));
            _viewLineLocation.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewLocation.AddSubview(_viewLineLocation);

            //Pole

            _viewPole = new UIView((new CGRect(18, 470 + imgViewFaultyLampHeight, View.Frame.Width - 36, 51)));
            _viewPole.BackgroundColor = UIColor.Clear;

            _lblPoleTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewPole.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "POLE",
                                                       font: myTNBFont.MuseoSans9(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            _viewPole.AddSubview(_lblPoleTitle);

            _lblPoleError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewPole.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                        "Invalid pole",
                                           font: myTNBFont.MuseoSans9(),
                                        foregroundColor: myTNBColor.Tomato(),
                                        strokeWidth: 0
                                       ),
                TextAlignment = UITextAlignment.Left
            };
            _viewPole.AddSubview(_lblPoleError);

            _txtFieldPole = new UITextField
            {
                Frame = new CGRect(0, 12, _viewPole.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                                                     "Pole no.",
                                                       font: myTNBFont.MuseoSans16(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            _viewPole.AddSubview(_txtFieldPole);

            _viewLinePole = new UIView((new CGRect(0, 36, _viewPole.Frame.Width, 1)));
            _viewLinePole.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewPole.AddSubview(_viewLinePole);

            //Feedback

            _viewFeedback = new UIView((new CGRect(18, 537 + imgViewFaultyLampHeight, View.Frame.Width - 36, 51)));
            _viewFeedback.BackgroundColor = UIColor.Clear;

            _lblFeedbackTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFeedback.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "FEEDBACK",
                                                       font: myTNBFont.MuseoSans9(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            _viewFeedback.AddSubview(_lblFeedbackTitle);

            _lblFeedbackError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewFeedback.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                        "Invalid feedback",
                                           font: myTNBFont.MuseoSans9(),
                                        foregroundColor: myTNBColor.Tomato(),
                                        strokeWidth: 0
                                       ),
                TextAlignment = UITextAlignment.Left
            };
            _viewFeedback.AddSubview(_lblFeedbackError);

            _txtFieldFeedback = new UITextField
            {
                Frame = new CGRect(0, 12, _viewFeedback.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                                                     "Feedback",
                                                       font: myTNBFont.MuseoSans16(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            _viewFeedback.AddSubview(_txtFieldFeedback);

            _viewLineFeedback = new UIView((new CGRect(0, 36, _viewFeedback.Frame.Width, 1)));
            _viewLineFeedback.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewFeedback.AddSubview(_viewLineFeedback);

            _lblFeedbackSubTitle = new UILabel(new CGRect(18, _viewFeedback.Frame.Y + 40, View.Frame.Width - 36, 14));
            _lblFeedbackSubTitle.TextColor = myTNBColor.SilverChalice();
            _lblFeedbackSubTitle.Font = myTNBFont.MuseoSans9();
            ScrollView.AddSubview(_lblFeedbackSubTitle);
            HandleFeedbackTextChange();

            //Photo/s Title
            var lblPhotoTitle = new UILabel(new CGRect(18, 594 + imgViewFaultyLampHeight, View.Frame.Width - 36, 14));
            lblPhotoTitle.Text = "ATTACH PHOTO / SCREENSHOT (OPTIONAL)";
            lblPhotoTitle.TextColor = myTNBColor.SilverChalice();
            lblPhotoTitle.Font = myTNBFont.MuseoSans9();
            ScrollView.AddSubview(lblPhotoTitle);

            var lblPhotoSubTitle = new UILabel(new CGRect(18, lblPhotoTitle.Frame.Y + 116, View.Frame.Width - 36, 14));
            lblPhotoSubTitle.Text = "Max 2 files";
            lblPhotoSubTitle.TextColor = myTNBColor.SilverChalice();
            lblPhotoSubTitle.Font = myTNBFont.MuseoSans9();
            ScrollView.AddSubview(lblPhotoSubTitle);

            //Submit Button
            _btnSubmit = new UIButton(UIButtonType.Custom);
            _btnSubmit.Frame = new CGRect(18, 727 + imgViewFaultyLampHeight, View.Frame.Width - 36, 48);
            _btnSubmit.SetTitle("Submit", UIControlState.Normal);
            _btnSubmit.Font = myTNBFont.MuseoSans16();
            _btnSubmit.Layer.CornerRadius = 5.0f;
            _btnSubmit.BackgroundColor = myTNBColor.FreshGreen();
            _btnSubmit.TouchUpInside += (sender, e) =>
            {
                ExecuteSubmitFeedback();
            };

            //ScrollView main subviews
            ScrollView.AddSubview(_viewFullName);
            ScrollView.AddSubview(_viewMobileNo);
            ScrollView.AddSubview(_viewEmail);
            ScrollView.AddSubview(_viewState);
            ScrollView.AddSubview(_viewLocation);
            ScrollView.AddSubview(_viewPole);
            ScrollView.AddSubview(_viewFeedback);
            ScrollView.AddSubview(_btnSubmit);

            //Scrollview content size
            ScrollView.ContentSize = new CGRect(0f, 0f, View.Frame.Width, 1230f).Size;

            _textFieldHelper.CreateTextFieldLeftView(_txtFieldFullName, "Name");
            _textFieldHelper.CreateTextFieldLeftView(_txtFieldMobileNo, "Mobile");
            _textFieldHelper.CreateTextFieldLeftView(_txtFieldEmail, "Email");
            _textFieldHelper.CreateTextFieldLeftView(_txtFieldLocation, "IC-FieldCoordinates");
            CreateTextFieldRightView(_txtFieldLocation, "IC-Action-Location");
            _textFieldHelper.CreateTextFieldLeftView(_txtFieldPole, "Account-Number");
            _textFieldHelper.CreateTextFieldLeftView(_txtFieldFeedback, "IC-Feedback");

            //_txtFieldPole.KeyboardType = UIKeyboardType.NumberPad;
            _txtFieldMobileNo.KeyboardType = UIKeyboardType.NumberPad;
            _txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;

            //_textFieldHelper.CreateDoneButton(_txtFieldPole);
            CreateDoneButton(_txtFieldMobileNo);

            _btnSubmit.Enabled = false;
            _btnSubmit.BackgroundColor = myTNBColor.SilverChalice();
        }

        public void CreateTextFieldRightView(UITextField textField, String imageName)
        {
            var rightView = new UIView((new CGRect(0, 0, 24, 24)));

            var imgForRightView = new UIImageView(UIImage.FromBundle(imageName));
            rightView.AddSubview(imgForRightView);

            rightView.Frame = new CGRect(rightView.Frame.X, rightView.Frame.Y, rightView.Frame.Width, rightView.Frame.Height);
            rightView.ContentMode = UIViewContentMode.Center;
            textField.RightView = rightView;
            textField.RightViewMode = UITextFieldViewMode.Always;

            UITapGestureRecognizer tapLocation = new UITapGestureRecognizer(() =>
            {
                _locManager = new LocationManager();
                _locManager.StartLocationUpdates();
                var isFirstCall = false;
                _locManager.LocMgr.AuthorizationChanged += (sender, e) =>
                {

                    if (e.Status == CLAuthorizationStatus.Authorized || e.Status == CLAuthorizationStatus.AuthorizedWhenInUse || e.Status == CLAuthorizationStatus.AuthorizedAlways)
                    {
                        CLLocation location = _locManager.LocMgr.Location;

                        CLGeocoder geoCoder = new CLGeocoder();
                        geoCoder.ReverseGeocodeLocation(location, (CLPlacemark[] placemarks, NSError error) =>
                        {
                            var reverseGeocodedAddress = placemarks[0].Thoroughfare + " " + placemarks[0].Locality + " " + placemarks[0].AdministrativeArea + " " + placemarks[0].PostalCode + " " + placemarks[0].Country;
                            _txtFieldLocation.Text = reverseGeocodedAddress;
                            SubmitButtonEnable();
                        });
                    }
                    else if (e.Status == CLAuthorizationStatus.NotDetermined)
                    {
                        _locManager.LocMgr.RequestWhenInUseAuthorization();
                        isFirstCall = true;
                    }
                    else if (e.Status == CLAuthorizationStatus.Denied && !isFirstCall)
                    {
                        var alertTitle = "Location Access is not allowed";                         var alertMessage = "Turn On Location Services to allow \"myTNB\" to determine your location";                         var okCancelAlertController = UIAlertController.Create(alertTitle, alertMessage, UIAlertControllerStyle.Alert);
                        okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));                         okCancelAlertController.AddAction(UIAlertAction.Create("Go to Settings", UIAlertActionStyle.Default, alert => NavigateToSettings()));                         PresentViewController(okCancelAlertController, true, null);
                    }
                };

            });
            rightView.AddGestureRecognizer(tapLocation);
        }

        internal void NavigateToSettings()
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
        }

        internal void SetVisibility()
        {
            _lblStateTitle.Hidden = true;
            _lblLocationTitle.Hidden = true;
            _lblPoleTitle.Hidden = true;
            _lblFeedbackTitle.Hidden = true;
            _lblFullNameTitle.Hidden = true;
            _lblMobileNoTitle.Hidden = true;
            _lblEmailTitle.Hidden = true;

            _lblStateError.Hidden = true;
            _lblLocationError.Hidden = true;
            _lblPoleError.Hidden = true;
            _lblFeedbackError.Hidden = true;
            _lblFullNameError.Hidden = true;
            _lblMobileNoError.Hidden = true;
            _lblEmailError.Hidden = true;
        }

        internal void SubmitButtonEnable()
        {
            bool isValidFullName = _textFieldHelper.ValidateTextField(_txtFieldFullName.Text, ANY_PATTERN) && _txtFieldFullName.Text.Length != 0;
            bool isValidMobileNo = _textFieldHelper.ValidateTextField(_txtFieldMobileNo.Text, MOBILE_NO_PATTERN) && _txtFieldMobileNo.Text.Length != 0;
            bool isValidEmail = _textFieldHelper.ValidateTextField(_txtFieldEmail.Text, EMAIL_PATTERN) && _txtFieldEmail.Text.Length != 0;
            bool isValidLocation = _textFieldHelper.ValidateTextField(_txtFieldLocation.Text, ANY_PATTERN) && _txtFieldLocation.Text.Length != 0;
            bool isValidPole = _textFieldHelper.ValidateTextField(_txtFieldPole.Text, ANY_PATTERN);
            bool isValidFeedback = _textFieldHelper.ValidateTextField(_txtFieldFeedback.Text, ANY_PATTERN) && _txtFieldFeedback.Text.Length != 0;

            bool isValid = isValidFullName
                && isValidMobileNo
                && isValidEmail
                && isValidLocation
                && isValidPole
                && isValidFeedback;
            _btnSubmit.Enabled = isValid;
            _btnSubmit.BackgroundColor = isValid ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        internal void SetEvents()
        {
            SetTextFieldEvents(_txtFieldFullName, _lblFullNameTitle
                               , _lblFullNameError, _viewLineFullName
                               , null, ANY_PATTERN);
            SetTextFieldEvents(_txtFieldMobileNo, _lblMobileNoTitle
                               , _lblMobileNoError, _viewLineMobile
                               , lblMobileNoHint, MOBILE_NO_PATTERN);
            SetTextFieldEvents(_txtFieldEmail, _lblEmailTitle
                               , _lblEmailError, _viewLineEmail
                               , null, EMAIL_PATTERN);
            SetTextFieldEvents(_txtFieldLocation, _lblLocationTitle
                               , _lblLocationError, _viewLineLocation
                               , null, ANY_PATTERN);
            SetTextFieldEvents(_txtFieldPole, _lblPoleTitle
                               , _lblPoleError, _viewLinePole
                               , null, ANY_PATTERN);
            SetTextFieldEvents(_txtFieldFeedback, _lblFeedbackTitle
                               , _lblFeedbackError, _viewLineFeedback
                               , null, ANY_PATTERN);
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
                UITextField txtField = sender as UITextField;
                if (txtField == _txtFieldFeedback)
                {
                    HandleFeedbackTextChange();
                }
                lblHint.Hidden = lblError.Hidden ? textField.Text.Length == 0 : true;
                lblTitle.Hidden = textField.Text.Length == 0;
                SubmitButtonEnable();
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                if (textField == _txtFieldMobileNo)
                {
                    if (textField.Text.Length == 0)
                    {
                        textField.Text += "+60 ";
                    }
                }
                lblHint.Hidden = lblError.Hidden ? textField.Text.Length == 0 : true;
                lblTitle.Hidden = textField.Text.Length == 0;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                if (textField == _txtFieldMobileNo)
                {
                    if (textField.Text.Length < 4)
                    {
                        textField.Text = string.Empty;
                    }
                }
                lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);

                lblError.Hidden = isValid || textField.Text.Length == 0;
                lblHint.Hidden = true;
                viewLine.BackgroundColor = isValid || textField.Text.Length == 0 ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                textField.TextColor = isValid || textField.Text.Length == 0 ? myTNBColor.TunaGrey() : myTNBColor.Tomato();

                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                if (textField == _txtFieldMobileNo)
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
                if (txtField == _txtFieldMobileNo)
                {
                    string content = ((UITextField)txtField).Text;
                    string preffix = string.Empty;
                    if (content.Length == 1)
                    {
                        preffix = content.Substring(0, 1);
                        if (preffix.Equals("+") && replacementString.Equals(string.Empty))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else if (txtField == _txtFieldFeedback)
                {
                    var newLength = textField.Text.Length + replacementString.Length - range.Length;
                    return newLength <= TNBGlobal.FeedbackMaxCharCount;
                }
                return true;
            };
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

        void ExecuteSubmitFeedback()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();

                        _feedbackMessage = _txtFieldFeedback.Text;
                        _stateID = DataManager.DataManager.SharedInstance.StatesForFeedBack.d.data[DataManager
                            .DataManager.SharedInstance.CurrentSelectedStateForFeedbackIndex].StateId;
                        _location = _txtFieldLocation.Text;
                        _poleNumber = _txtFieldPole.Text;
                        _fullName = _txtFieldFullName.Text;
                        _mobileNo = _txtFieldMobileNo.Text;
                        _email = _txtFieldEmail.Text;

                        _capturedImageList = new List<ImageDataModel>();
                        ImageDataModel imgData;
                        UIImageView imgView;
                        UIImage resizedImage;

                        foreach (UIView view in imageContainerScroll.Subviews)
                        {
                            if (view.Tag == 10)
                            {
                                for (int i = 0; i < view.Subviews.Length; i++)
                                {
                                    if (view.Subviews[i].Tag == 1)
                                    {
                                        imgData = new ImageDataModel();
                                        imgView = view.Subviews[i] as UIImageView;
                                        resizedImage = _imageHelper.ResizeImage(imgView.Image);
                                        imgData.imageHex = _imageHelper.ConvertImageToHex(resizedImage);
                                        imgData.filesize = _imageHelper.GetImageFileSize(resizedImage).ToString();
                                        imgData.fileName = FeedbackFileNameHelper.GenerateFileName();
                                        _capturedImageList.Add(imgData);
                                    }
                                }
                            }
                        }

                        SubmitFeedback().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_submitFeedback != null && _submitFeedback.d != null
                                   && _submitFeedback.d.isError.Equals("false")
                                   && _submitFeedback.d.data != null)
                                {
                                    UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                                    SubmitFeedbackSuccessViewController submitFeedbackSuccessVC =
                                        storyBoard.InstantiateViewController("SubmitFeedbackSuccessViewController") as SubmitFeedbackSuccessViewController;
                                    submitFeedbackSuccessVC.ServiceReqNo = _submitFeedback.d.data.ServiceReqNo;
                                    submitFeedbackSuccessVC.DateCreated = _submitFeedback.d.data.DateCreated;
                                    NavigationController.PushViewController(submitFeedbackSuccessVC, true);
                                }
                                else
                                {
                                    UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                                    SubmitFeedbackFailedViewController submitFeedbackFailedVC =
                                        storyBoard.InstantiateViewController("SubmitFeedbackFailedViewController") as SubmitFeedbackFailedViewController;
                                    NavigationController.PushViewController(submitFeedbackFailedVC, true);
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        Console.WriteLine("No Network");
                        var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
                    }
                });
            });
        }

        internal void AddImageContainer()
        {
            if (capturedImageCount < MAX_IMAGE)
            {
                if (imageContainerScroll == null)
                {
                    imageContainerScroll = new UIScrollView(new CGRect(18, 609 + imgViewFaultyLampHeight, View.Frame.Width - 36, 94));
                    imageContainerScroll.ScrollEnabled = true;
                    imageContainerScroll.Bounces = false;
                    imageContainerScroll.DirectionalLockEnabled = true;
                    ScrollView.AddSubview(imageContainerScroll);
                }

                UIViewWithDashedLinerBorder dashedLineView = new UIViewWithDashedLinerBorder();
                dashedLineView.Frame = new CGRect(imageWidth, 0, 94, 94);
                dashedLineView.BackgroundColor = UIColor.White;
                dashedLineView.Layer.CornerRadius = 5.0f;
                dashedLineView.Tag = 10;

                UIImageView imgViewAdd = new UIImageView(new CGRect(35, 35, 24, 24));
                imgViewAdd.Image = UIImage.FromBundle("IC-Action-Add-Card");
                imgViewAdd.Tag = 0;

                dashedLineView.AddSubview(imgViewAdd);

                dashedLineView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    UIImagePickerController imgPicker = new UIImagePickerController();
                    ImagePickerDelegate imgPickerDelegate = new ImagePickerDelegate(this);
                    imgPickerDelegate.Type = Enums.FeedbackCategory.NonLoginFaultyStreetLamp;
                    imgPickerDelegate.DashedLineView = dashedLineView;
                    imgPicker.Delegate = imgPickerDelegate;

                    var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

                    alert.AddAction(UIAlertAction.Create("Camera", UIAlertActionStyle.Default, (obj) =>
                    {
                        imgPicker.SourceType = UIImagePickerControllerSourceType.Camera;
                        PresentViewController(imgPicker, true, null);
                    }));

                    alert.AddAction(UIAlertAction.Create("Camera Roll", UIAlertActionStyle.Default, (obj) =>
                    {
                        imgPicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                        PresentViewController(imgPicker, true, null);
                    }));

                    var cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
                    alert.AddAction(cancelAction);
                    PresentViewController(alert, animated: true, completionHandler: null);
                }));

                imageContainerScroll.AddSubview(dashedLineView);
                imageWidth += 18 + 94;
                imageContainerScroll.ContentSize = new CGRect(18, 165, imageWidth, 94).Size;
                capturedImageCount++;
            }
        }

        internal void RepositionImageContent()
        {
            imageWidth = 0;
            foreach (UIView view in imageContainerScroll.Subviews)
            {
                if (view.Tag == 10)
                {
                    view.Frame = new CGRect(imageWidth, 0, 94, 94);
                    imageWidth += 18 + 94;
                }
            }
            imageContainerScroll.ContentSize = new CGRect(18, 165, imageWidth, 94).Size;
        }

        internal void AddImage(UIImage image, UIViewWithDashedLinerBorder view)
        {
            ActivityIndicator.Show();
            UIImageView capturedImageView = new UIImageView(new CGRect(0, 0, view.Frame.Width, view.Frame.Height));
            capturedImageView.Image = image;
            capturedImageView.Tag = 1;

            UIImageView imgDelete = new UIImageView(new CGRect(65, 5, 24, 24));
            imgDelete.Image = UIImage.FromBundle("Delete");

            imageCount++;
            view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                view.RemoveFromSuperview();
                RepositionImageContent();
                capturedImageCount--;
                imageCount--;
                if (imageCount > 0)
                {
                    AddImageContainer();
                }
            }));

            view.AddSubviews(new UIView[] { capturedImageView, imgDelete });
            AddImageContainer();

            ActivityIndicator.Hide();
        }

        Task SubmitFeedback()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    feedbackCategoryId = "2",
                    feedbackTypeId = string.Empty,
                    accountNum = string.Empty,
                    name = _fullName,
                    phoneNum = _mobileNo,
                    email = _email,
                    deviceId = DataManager.DataManager.SharedInstance.UDID,
                    feedbackMesage = _feedbackMessage,
                    stateId = _stateID,
                    location = _location,
                    poleNum = _poleNumber,
                    images = _capturedImageList
                };
                _submitFeedback = serviceManager.SubmitFeedback("SubmitFeedback", requestParameter);
            });
        }

        void CreateDoneButton(UITextField textField)
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                if (textField == _txtFieldMobileNo)
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

        /// <summary>
        /// Handles the feedback text change.
        /// </summary>
        private void HandleFeedbackTextChange()
        {
            int charCount = TNBGlobal.FeedbackMaxCharCount - _txtFieldFeedback.Text.Length;
            string text = string.Format("{0} character{1} left", charCount, charCount != 1 ? "s" : string.Empty);
            _lblFeedbackSubTitle.Text = text;
        }
    }
}