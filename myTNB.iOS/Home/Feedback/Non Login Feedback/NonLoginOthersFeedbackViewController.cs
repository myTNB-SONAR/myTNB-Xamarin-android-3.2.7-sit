using Foundation;
using System;
using UIKit;
using CoreGraphics;
using myTNB.Model;
using System.Threading.Tasks;
using myTNB.Home.Feedback;
using System.Collections.Generic;
using System.Drawing;
using myTNB.Customs;
using myTNB.Extensions;

namespace myTNB
{
    public partial class NonLoginOthersFeedbackViewController : UIViewController
    {
        public NonLoginOthersFeedbackViewController(IntPtr handle) : base(handle)
        {
        }

        UITextField _txtFieldFullName;
        UILabel _lblFullNameTitle;
        UILabel _lblFullNameError;
        UIView _viewFullName;
        UIView _viewLineFullName;

        UITextField _txtFieldMobileNo;
        UILabel _lblMobileNoTitle;
        UILabel _lblMobileNoError;
        UIView _viewMobileNo;
        UIView _viewLineMobileNo;

        UITextField _txtFieldEmail;
        UILabel _lblEmailTitle;
        UILabel _lblEmailError;
        UIView _viewEmail;
        UIView _viewLineEmail;
        UILabel _lblFeedbackType;
        UILabel _lblFeedbackTypeTitle;
        UILabel _lblFeedbackTypeError;
        UIImageView imgViewAccountNumber;
        UIView _viewFeedbackType;
        UIView _viewLineFeedbackType;
        UIImageView _iconFeedback;
        UIView _viewUploadPhoto;
        UILabel _lblPhotoTitle;

        UILabel _lblFeedbackTitle;
        UILabel _lblFeedbackSubTitle;
        UILabel _lblFeedbackError;
        UIView _viewFeedback;
        UIView _viewLineFeedback;

        UILabel lblMobileNoHint;
        UIView _btnSubmitContainer;
        UIButton _btnSubmit;

        UITapGestureRecognizer _tapImage;

        FeedbackTextView _feedbackTextView = new FeedbackTextView();
        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        SubmitFeedbackResponseModel _submitFeedback = new SubmitFeedbackResponseModel();

        const string ANY_PATTERN = @".*";
        const string EMAIL_PATTERN = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        const string PASSWORD_PATTERN = @".*";
        const string MOBILE_NO_PATTERN = @"^[0-9 \+]+$";
        const string IC_NO_PATTERN = @"^[a-zA-Z0-9]+$";
        const string ACCOUNT_NO_PATTERN = @"^[0-9]{12,14}$";

        string _feedbackMessage = string.Empty;
        string _fullName = string.Empty;
        string _mobileNo = string.Empty;
        string _email = string.Empty;

        List<ImageDataModel> _capturedImageList = new List<ImageDataModel>();
        UIImageHelper _imageHelper = new UIImageHelper();

        UIScrollView ScrollView;
        UIScrollView imageContainerScroll;
        int imageWidth = 0;
        int capturedImageCount = 0;
        int imageCount = 0;
        const int MAX_IMAGE = 2;
        const float ZERO_MARGIN = 0f;
        const float TXTVIEW_DEFAULT_MARGIN = 24f;

        float _viewFeedbackYCoord = 284f;

        float _viewPhotoYCoord = 270f;
        float _feedbackMargin = 3f;
        float _universalMargin = 7f;
        float _objMargin = 15f;
        float _viewPhotoMargin = 20f;
        float _scrollViewHeight = 0.0f;
        float _feedbackFieldHeight = 38f;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            InitializedSubviews();
            AddBackButton();
            SetEvents();
            SetVisibility();
            AddImageContainer();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            var currIndex = DataManager.DataManager.SharedInstance.CurrentSelectedFeedbackTypeIndex;
            if (DataManager.DataManager.SharedInstance.OtherFeedbackType != null
                && currIndex < DataManager.DataManager.SharedInstance.OtherFeedbackType.Count)
            {
                _lblFeedbackType.Text = DataManager.DataManager.SharedInstance.OtherFeedbackType[currIndex].FeedbackTypeName;
            }
        }

        internal void InitializedSubviews()
        {
            //Scrollview
            ScrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
            ScrollView.BackgroundColor = UIColor.Clear;
            View.AddSubview(ScrollView);

            //FullName
            _viewFullName = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 51)));
            _viewFullName.BackgroundColor = UIColor.Clear;

            _lblFullNameTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFullName.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "FULLNAME",
                                                       font: myTNBFont.MuseoSans11_300(),
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
                                           font: myTNBFont.MuseoSans11_300(),
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
                                                       font: myTNBFont.MuseoSans18_300(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            _viewFullName.AddSubview(_txtFieldFullName);

            _viewLineFullName = new UIView((new CGRect(0, 36, _viewFullName.Frame.Width, 1)));
            _viewLineFullName.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewFullName.AddSubview(_viewLineFullName);

            //Mobile no.
            _viewMobileNo = new UIView((new CGRect(18, 83, View.Frame.Width - 36, 51)));
            _viewMobileNo.BackgroundColor = UIColor.Clear;

            _lblMobileNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewMobileNo.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "MOBILE NO.",
                                                       font: myTNBFont.MuseoSans11_300(),
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
                                           font: myTNBFont.MuseoSans11_300(),
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
                    font: myTNBFont.MuseoSans11_300(),
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
                                                       font: myTNBFont.MuseoSans18_300(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            _txtFieldMobileNo.KeyboardType = UIKeyboardType.NumberPad;
            CreateDoneButton(_txtFieldMobileNo);
            _viewMobileNo.AddSubview(_txtFieldMobileNo);

            _viewLineMobileNo = new UIView((new CGRect(0, 36, _viewMobileNo.Frame.Width, 1)));
            _viewLineMobileNo.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewMobileNo.AddSubview(_viewLineMobileNo);

            //Email

            _viewEmail = new UIView((new CGRect(18, 150, View.Frame.Width - 36, 51)));
            _viewEmail.BackgroundColor = UIColor.Clear;

            _lblEmailTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewEmail.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "EMAIL",
                                                       font: myTNBFont.MuseoSans11_300(),
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
                                           font: myTNBFont.MuseoSans11_300(),
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
                                                       font: myTNBFont.MuseoSans18_300(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            _txtFieldEmail.KeyboardType = UIKeyboardType.EmailAddress;
            _viewEmail.AddSubview(_txtFieldEmail);

            _viewLineEmail = new UIView((new CGRect(0, 36, _viewEmail.Frame.Width, 1)));
            _viewLineEmail.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewEmail.AddSubview(_viewLineEmail);


            //Feedback Type
            _viewFeedbackType = new UIView((new CGRect(18, 217, View.Frame.Width - 36, 51)));
            _viewFeedbackType.BackgroundColor = UIColor.Clear;

            _lblFeedbackTypeTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFeedbackType.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "FEEDBACK TYPE",
                                                       font: myTNBFont.MuseoSans11_300(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            _viewFeedbackType.AddSubview(_lblFeedbackTypeTitle);

            _lblFeedbackTypeError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewFeedbackType.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                            "Invalid feedback type",
                                               font: myTNBFont.MuseoSans11_300(),
                                            foregroundColor: myTNBColor.Tomato(),
                                            strokeWidth: 0
                                           ),
                TextAlignment = UITextAlignment.Left
            };
            _viewFeedbackType.AddSubview(_lblFeedbackTypeError);


            imgViewAccountNumber = new UIImageView(new CGRect(0, 12, 24, 24));
            imgViewAccountNumber.Image = UIImage.FromBundle("IC-FieldFeedback");
            _viewFeedbackType.AddSubview(imgViewAccountNumber);

            _lblFeedbackType = new UILabel(new CGRect(30, 12, _viewFeedbackType.Frame.Width, 24));
            _lblFeedbackType.AttributedText = new NSAttributedString(
                                            "Feedback type",
                                               font: myTNBFont.MuseoSans18_300(),
                                            foregroundColor: myTNBColor.SilverChalice(),
                                            strokeWidth: 0
            );//Noted: Temp Number, will create a list for this later.
            _lblFeedbackType.Font = myTNBFont.MuseoSans18_300();
            _lblFeedbackType.TextColor = myTNBColor.TunaGrey();

            _viewFeedbackType.AddSubview(_lblFeedbackType);

            UIImageView imgDropDown = new UIImageView(new CGRect(_viewFeedbackType.Frame.Width - 30, 12, 24, 24));
            imgDropDown.Image = UIImage.FromBundle("IC-Action-Dropdown");
            _viewFeedbackType.AddSubview(imgDropDown);

            _viewLineFeedbackType = new UIView((new CGRect(0, 36, _viewFeedbackType.Frame.Width, 1)));
            _viewLineFeedbackType.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewFeedbackType.AddSubview(_viewLineFeedbackType);

            UITapGestureRecognizer tapFeedbackType = new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("FeedbackTableView", null);
                FeedbackTypeViewController feedbackTypeVC =
                    storyBoard.InstantiateViewController("FeedbackTypeViewController") as FeedbackTypeViewController;
                feedbackTypeVC._feedbackTypeList = DataManager.DataManager.SharedInstance.OtherFeedbackType;
                NavigationController.PushViewController(feedbackTypeVC, true);

            });
            _viewFeedbackType.AddGestureRecognizer(tapFeedbackType);


            //Feedback

            _viewFeedback = new UIView((new CGRect(18, _viewFeedbackYCoord, View.Frame.Width - 36, 51)));
            _viewFeedback.BackgroundColor = UIColor.Clear;

            _lblFeedbackTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFeedback.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "FEEDBACK",
                                                       font: myTNBFont.MuseoSans11_300(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            _viewFeedback.AddSubview(_lblFeedbackTitle);

            _feedbackTextView = new FeedbackTextView
            {
                Frame = new CGRect(TXTVIEW_DEFAULT_MARGIN, 12, View.Frame.Width - 60, 36),
                Editable = true,
                Font = myTNBFont.MuseoSans18_300(),
                TextAlignment = UITextAlignment.Left,
                TextColor = myTNBColor.TunaGrey(),
                BackgroundColor = UIColor.Clear,
                EnablesReturnKeyAutomatically = true,
                TranslatesAutoresizingMaskIntoConstraints = true,
                ScrollEnabled = true,
            };

            _iconFeedback = new UIImageView(new CGRect(0, _feedbackTextView.Frame.Height / 2, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Feedback")
            };

            _feedbackTextView.SetPlaceholder("Feedback");
            _feedbackTextView.CreateDoneButton();
            _viewFeedback.AddSubview(_feedbackTextView);
            _viewFeedback.AddSubview(_iconFeedback);

            _feedbackTextView.ShouldChangeText = (txtView, range, replacementString) =>
            {
                var newLength = txtView.Text.Length + replacementString.Length - range.Length;
                return newLength <= 250;
            };

            //Feedback
            _viewUploadPhoto = new UIView((new CGRect(18, _viewPhotoYCoord + _feedbackTextView.Frame.GetMaxY() + _viewPhotoMargin, View.Frame.Width - 36, 180)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblFeedbackError = new UILabel
            {
                Frame = new CGRect(0, 0, _viewUploadPhoto.Frame.Width - 36, 14),
                AttributedText = new NSAttributedString(
                                        "Invalid feedback",
                                           font: myTNBFont.MuseoSans11_300(),
                                        foregroundColor: myTNBColor.Tomato(),
                                        strokeWidth: 0
                                       ),
                TextAlignment = UITextAlignment.Left
            };
            _viewUploadPhoto.AddSubview(_lblFeedbackError);

            _viewLineFeedback = new UIView((new CGRect(0, _feedbackTextView.Frame.GetMaxY() + _feedbackMargin, _viewFeedback.Frame.Width, 1)))
            {
                BackgroundColor = myTNBColor.PlatinumGrey()
            };
            _viewFeedback.AddSubview(_viewLineFeedback);

            _lblFeedbackSubTitle = new UILabel(new CGRect(0, 0, _viewUploadPhoto.Frame.Width - 36, 14))
            {
                TextColor = myTNBColor.SilverChalice(),
                Font = myTNBFont.MuseoSans11_300()
            };
            _viewUploadPhoto.AddSubview(_lblFeedbackSubTitle);
            HandleFeedbackTextViewChange();

            //Photo/s Title
            _lblPhotoTitle = new UILabel(new CGRect(0, _lblFeedbackSubTitle.Frame.GetMaxY() + _objMargin, View.Frame.Width - 36, 14))
            {
                Text = "ATTACH PHOTO / SCREENSHOT (OPTIONAL)",
                TextColor = myTNBColor.SilverChalice(),
                Font = myTNBFont.MuseoSans11_300()
            };
            _viewUploadPhoto.AddSubview(_lblPhotoTitle);

            var lblPhotoSubTitle = new UILabel(new CGRect(0, _lblFeedbackSubTitle.Frame.GetMaxY() + 135, View.Frame.Width - 36, 14))
            {
                Text = "Max 2 files",
                TextColor = myTNBColor.SilverChalice(),
                Font = myTNBFont.MuseoSans11_300()
            };
            _viewUploadPhoto.AddSubview(lblPhotoSubTitle);

            _btnSubmitContainer = new UIView(new CGRect(0, (View.Frame.Height - DeviceHelper.GetScaledHeight(145)), View.Frame.Width, DeviceHelper.GetScaledHeight(100)));
            _btnSubmitContainer.BackgroundColor = UIColor.White;
            View.AddSubview(_btnSubmitContainer);

            //Submit Button
            _btnSubmit = new UIButton(UIButtonType.Custom);
            _btnSubmit.Frame = new CGRect(18, DeviceHelper.GetScaledHeight(18), _btnSubmitContainer.Frame.Width - 36, 48);
            _btnSubmit.SetTitle("Submit", UIControlState.Normal);
            _btnSubmit.Font = myTNBFont.MuseoSans18_300();
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
            ScrollView.AddSubview(_viewFeedbackType);
            ScrollView.AddSubview(_viewFeedback);
            ScrollView.AddSubview(_viewUploadPhoto);

            _textFieldHelper.CreateTextFieldLeftView(_txtFieldFullName, "Name");
            _textFieldHelper.CreateTextFieldLeftView(_txtFieldMobileNo, "Mobile");
            _textFieldHelper.CreateTextFieldLeftView(_txtFieldEmail, "Email");

            _btnSubmit.Enabled = false;
            _btnSubmit.BackgroundColor = myTNBColor.SilverChalice();

            _btnSubmitContainer.AddSubview(_btnSubmit);

            _scrollViewHeight = (float)((_viewUploadPhoto.Frame.GetMaxY() + (_btnSubmitContainer.Frame.Height + 50f)));
            ScrollView.ContentSize = new CGRect(0f, 0f, View.Frame.Width, _scrollViewHeight).Size;
        }

        /// <summary>
        /// Sets the text view events.
        /// </summary>
        /// <param name="textView">Text view.</param>
        /// <param name="lblTitle">Lbl title.</param>
        /// <param name="lblError">Lbl error.</param>
        /// <param name="viewLine">View line.</param>
        /// <param name="lblHint">Lbl hint.</param>
        /// <param name="pattern">Pattern.</param>
        internal void SetTextViewEvents(FeedbackTextView textView, UILabel lblTitle
                                         , UILabel lblError, UIView viewLine
                                         , UILabel lblHint, string pattern)
        {
            if (lblHint == null)
            {
                lblHint = new UILabel();
            }
            _feedbackTextView.SetKeyboard();
            textView.Changed += (sender, e) => {
                FeedbackTextView txtView = sender as FeedbackTextView;
                if (txtView == _feedbackTextView)
                {
                    HandleFeedbackTextViewChange();

                    var frame = new CGRect();
                    frame = _feedbackTextView.Frame;
                    frame.Height = _feedbackTextView.ContentSize.Height <= TNBGlobal.FEEDBACK_FIELD_MAX_HEIGHT ? _feedbackTextView.ContentSize.Height : TNBGlobal.FEEDBACK_FIELD_MAX_HEIGHT;
                    _feedbackTextView.Frame = frame;
                    ScrollView.ContentSize = new CGRect(0f, 0f, View.Frame.Width, _scrollViewHeight + _feedbackTextView.Frame.Height - _feedbackFieldHeight).Size;
                    _viewFeedback.Frame = ViewHelper.UpdateFeedbackViewYCoord(_viewFeedbackYCoord, 0f, _viewFeedback, (float)(51f + _feedbackTextView.Frame.Height));
                    _viewLineFeedback.Frame = ViewHelper.UpdateFeedbackViewYCoord((float)_feedbackTextView.Frame.GetMaxY(), _feedbackMargin, _viewLineFeedback, (float)_viewLineFeedback.Frame.Height);
                    _viewUploadPhoto.Frame = ViewHelper.UpdateFeedbackViewYCoord(_viewPhotoYCoord + (float)_feedbackTextView.Frame.GetMaxY(), 20f, _viewUploadPhoto, (float)_viewUploadPhoto.Frame.Height);

                    if (txtView.Text.Length > 0)
                    {
                        _feedbackTextView.SetPlaceholderHidden(true);
                    }
                    else
                    {
                        _feedbackTextView.SetPlaceholderHidden(false);
                    }
                }
                lblHint.Hidden = lblError.Hidden ? _feedbackTextView.Text.Length == 0 : true;
                lblTitle.Hidden = _feedbackTextView.Text.Length == 0;
                SubmitButtonEnable();
            };
            textView.ShouldBeginEditing = (sender) =>
            {
                var frame = new CGRect();
                frame = _feedbackTextView.Frame;
                _iconFeedback.Hidden = true;
                frame.X = ZERO_MARGIN;
                _feedbackTextView.Frame = frame;
                viewLine.BackgroundColor = myTNBColor.PowerBlue();
                lblError.Hidden = true;
                _lblFeedbackSubTitle.Hidden = false;
                textView.TextColor = myTNBColor.TunaGrey();
                return true;
            };
            textView.ShouldEndEditing = (sender) =>
            {
                lblTitle.Hidden = textView.Text.Length == 0;
                bool isValid = _feedbackTextView.ValidateTextView(textView.Text, pattern);

                lblError.Hidden = isValid || textView.Text.Length == 0;
                _lblFeedbackSubTitle.Hidden = !lblError.Hidden;
                lblHint.Hidden = true;
                viewLine.BackgroundColor = isValid || textView.Text.Length == 0 ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                textView.TextColor = isValid || textView.Text.Length == 0 ? myTNBColor.TunaGrey() : myTNBColor.Tomato();

                if (textView.Text.Length == 0)
                {
                    var frame = new CGRect();
                    frame = _feedbackTextView.Frame;
                    frame.X = 24f;
                    _feedbackTextView.Frame = frame;
                    _iconFeedback.Hidden = false;
                    _feedbackTextView.SetPlaceholderHidden(false);
                }

                return true;
            };
            textView.ShouldChangeText += (txtView, range, replacementString) =>
            {
                if (txtView == _feedbackTextView)
                {
                    var newLength = textView.Text.Length + replacementString.Length - range.Length;
                    return newLength <= TNBGlobal.FeedbackMaxCharCount;
                }
                return true;
            };
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
                lblHint.Hidden = lblError.Hidden ? textField.Text.Length == 0 : true;
                lblTitle.Hidden = textField.Text.Length == 0;
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                if (textField == _txtFieldMobileNo)
                {
                    if (textField.Text.Length == 0)
                    {
                        textField.Text += TNBGlobal.MobileNoPrefix;
                    }
                }
                lblHint.Hidden = lblError.Hidden ? textField.Text.Length == 0 : true;
                lblTitle.Hidden = textField.Text.Length == 0;
                textField.LeftViewMode = UITextFieldViewMode.Never;
                viewLine.BackgroundColor = myTNBColor.PowerBlue();
            };
            textField.ShouldEndEditing = (sender) =>
            {
                bool isValid = true;
                bool isEmptyAllowed = true;
                if (textField == _txtFieldMobileNo)
                {
                    if (textField.Text.Length < 4)
                    {
                        textField.Text = string.Empty;
                    }
                    pattern = MOBILE_NO_PATTERN;
                    isValid = _textFieldHelper.ValidateMobileNumberLength(textField.Text);
                    isEmptyAllowed = false;
                }
                else if (textField == _txtFieldFullName)
                {
                    isValid = isValid && !string.IsNullOrWhiteSpace(textField.Text);
                    isEmptyAllowed = false;
                }
                lblTitle.Hidden = textField.Text.Length == 0;
                isValid = isValid && _textFieldHelper.ValidateTextField(textField.Text, pattern);

                bool isNormal = isValid || (textField.Text.Length == 0 && isEmptyAllowed);
                lblError.Hidden = isNormal;
                lblHint.Hidden = true;
                viewLine.BackgroundColor = isNormal ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                textField.TextColor = isNormal ? myTNBColor.TunaGrey() : myTNBColor.Tomato();

                SubmitButtonEnable();

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
                bool isCharValid = !string.IsNullOrEmpty(replacementString)
                                          ? _textFieldHelper.ValidateTextField(replacementString, pattern)
                                          : true;
                if (txtField == _txtFieldMobileNo)
                {
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
                else if (txtField == _txtFieldFullName)
                {
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

        internal void SubmitButtonEnable()
        {
            bool isValidFullName = _textFieldHelper.ValidateTextField(_txtFieldFullName.Text, TNBGlobal.CustomerNamePattern) && !string.IsNullOrWhiteSpace(_txtFieldFullName.Text);
            bool isValidMobileNo = _textFieldHelper.ValidateTextField(_txtFieldMobileNo.Text, MOBILE_NO_PATTERN)
                                                   && _textFieldHelper.ValidateMobileNumberLength(_txtFieldMobileNo.Text);
            bool isValidEmail = _textFieldHelper.ValidateTextField(_txtFieldEmail.Text, EMAIL_PATTERN);
            bool isValidFeedback = _feedbackTextView.ValidateTextView(_feedbackTextView.Text, ANY_PATTERN) && _feedbackTextView.Text.Length != 0;

            bool isValid = isValidFullName
                && isValidMobileNo
                && isValidEmail
                && isValidFeedback;
            _btnSubmit.Enabled = isValid;
            _btnSubmit.BackgroundColor = isValid ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        internal void SetEvents()
        {
            SetTextFieldEvents(_txtFieldFullName, _lblFullNameTitle
                               , _lblFullNameError, _viewLineFullName
                               , null, TNBGlobal.CustomerNamePattern);
            SetTextFieldEvents(_txtFieldMobileNo, _lblMobileNoTitle
                               , _lblMobileNoError, _viewLineMobileNo
                               , lblMobileNoHint, TNBGlobal.MobileNoPattern);
            SetTextFieldEvents(_txtFieldEmail, _lblEmailTitle
                               , _lblEmailError, _viewLineEmail
                               , null, EMAIL_PATTERN);
            SetTextViewEvents(_feedbackTextView, _lblFeedbackTitle
                               , _lblFeedbackError, _viewLineFeedback
                               , null, ANY_PATTERN);
        }

        internal void SetVisibility()
        {
            if (_lblFeedbackType.Text != string.Empty)
            {
                _lblFeedbackTypeTitle.Hidden = false;
                imgViewAccountNumber.Hidden = true;
                var frame = new CGRect();
                frame = _lblFeedbackType.Frame;
                frame.X = ZERO_MARGIN;
                _lblFeedbackType.Frame = frame;
            }

            _lblFullNameTitle.Hidden = true;
            _lblMobileNoTitle.Hidden = true;
            _lblEmailTitle.Hidden = true;
            _lblFeedbackTitle.Hidden = true;

            _lblFullNameError.Hidden = true;
            _lblMobileNoError.Hidden = true;
            _lblEmailError.Hidden = true;
            _lblFeedbackTypeError.Hidden = true;
            _lblFeedbackError.Hidden = true;
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
                        _feedbackMessage = _feedbackTextView.Text;
                        _fullName = _txtFieldFullName.Text;
                        _mobileNo = _textFieldHelper.TrimAllSpaces(_txtFieldMobileNo.Text);
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
                                    ToastHelper.DisplayAlertView(this, "SubmitFeedbackErrTitle".Translate(), _submitFeedback?.d?.message);

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
                        var alert = UIAlertController.Create("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate(), UIAlertControllerStyle.Alert);
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
                    imageContainerScroll = new UIScrollView(new CGRect(0, _lblPhotoTitle.Frame.GetMaxY() + _universalMargin, View.Frame.Width - 36, 94));
                    imageContainerScroll.ScrollEnabled = true;
                    imageContainerScroll.Bounces = false;
                    imageContainerScroll.DirectionalLockEnabled = true;
                    _viewUploadPhoto.AddSubview(imageContainerScroll);
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

                _tapImage = new UITapGestureRecognizer(() =>
                {
                    if (imageCount >= MAX_IMAGE)
                    {
                        return;
                    }

                    UIImagePickerController imgPicker = new UIImagePickerController();
                    ImagePickerDelegate imgPickerDelegate = new ImagePickerDelegate(this);
                    imgPickerDelegate.Type = Enums.FeedbackCategory.NonLoginOthers;
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
                });

                dashedLineView.AddGestureRecognizer(_tapImage);

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

            UIView imgView = new UIView(new CGRect(65, 0, 29, 29));
            imgView.BackgroundColor = UIColor.Clear;
            UIImageView imgDelete = new UIImageView(new CGRect(2, 2, 24, 24));
            imgDelete.Image = UIImage.FromBundle("Delete");
            imageCount++;
            imgView.AddSubview(imgDelete);
            imgView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
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

            view.AddSubviews(new UIView[] { capturedImageView, imgView });
            view.RemoveGestureRecognizer(_tapImage);
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
                    feedbackCategoryId = "3",
                    feedbackTypeId = DataManager.DataManager.SharedInstance.OtherFeedbackType[DataManager.DataManager.SharedInstance.CurrentSelectedFeedbackTypeIndex].FeedbackTypeId,
                    accountNum = string.Empty,
                    name = _fullName,
                    phoneNum = _mobileNo,
                    email = _email,
                    deviceId = DataManager.DataManager.SharedInstance.UDID,
                    feedbackMesage = _feedbackMessage,
                    stateId = string.Empty,
                    location = string.Empty,
                    poleNum = string.Empty,
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
        private void HandleFeedbackTextViewChange()
        {
            int charCount = TNBGlobal.FeedbackMaxCharCount - _feedbackTextView.Text.Length;
            string text = string.Format("{0} character{1} left", charCount, charCount != 1 ? "s" : string.Empty);
            _lblFeedbackSubTitle.Text = text;
        }
    }
}